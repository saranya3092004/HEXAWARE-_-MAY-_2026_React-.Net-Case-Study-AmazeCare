import { useEffect, useState, useCallback } from 'react';
import { getAppointments, cancelAppointment, rescheduleAppointment } from '../../api/appointments';
import AdminLayout from '../../layouts/AdminLayout';
import RescheduleModal from '../../components/patient/RescheduleModal';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

function StatusBadge({ status }) {
  return <span className={`badge badge--${status.toLowerCase()}`}>{status}</span>;
}

export default function AdminAppointmentsPage() {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [actionError, setActionError] = useState('');
  const [rescheduleTarget, setRescheduleTarget] = useState(null);
  const [filter, setFilter] = useState({ status: '', fromDate: '', toDate: '' });

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = {};
      if (filter.status) params.status = filter.status;
      if (filter.fromDate) params.fromDate = filter.fromDate;
      if (filter.toDate) params.toDate = filter.toDate;
      const { data } = await getAppointments(params);
      setAppointments(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => { load(); }, [load]);

  async function handleCancel(id) {
    const reason = window.prompt('Reason for cancellation:');
    if (!reason?.trim()) return;
    setActionError('');
    try {
      await cancelAppointment(id, { reason });
      load();
    } catch (err) {
      if (isUserFacingError(err)) setActionError(err.message);
      else { console.error(err); setActionError(GENERIC_ERROR); }
    }
  }

  return (
    <AdminLayout>
      <div className="page-header">
        <h1 className="page-title">Appointments</h1>
        <p className="page-subtitle">View, reschedule, and manage all appointments.</p>
      </div>

      {(error || actionError) && (
        <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>
          {error || actionError}
        </div>
      )}

      {/* Filters */}
      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr auto', gap: '0.75rem', alignItems: 'flex-end' }}>
          <div>
            <label className="form-label">Status</label>
            <select
              className="form-input"
              value={filter.status}
              onChange={(e) => setFilter({ ...filter, status: e.target.value })}
            >
              <option value="">All statuses</option>
              <option value="Pending">Pending</option>
              <option value="Confirmed">Confirmed</option>
              <option value="Completed">Completed</option>
              <option value="Cancelled">Cancelled</option>
              <option value="Rejected">Rejected</option>
              <option value="Rescheduled">Rescheduled</option>
            </select>
          </div>
          <div>
            <label className="form-label">From date</label>
            <input
              type="date"
              className="form-input"
              value={filter.fromDate}
              onChange={(e) => setFilter({ ...filter, fromDate: e.target.value })}
            />
          </div>
          <div>
            <label className="form-label">To date</label>
            <input
              type="date"
              className="form-input"
              value={filter.toDate}
              onChange={(e) => setFilter({ ...filter, toDate: e.target.value })}
            />
          </div>
          <button
            className="btn-primary"
            style={{ width: 'auto', padding: '0.7rem 1.25rem', marginBottom: '0' }}
            onClick={load}
          >
            Filter
          </button>
        </div>
      </div>

      <div className="data-table-wrap">
        {loading ? (
          <div className="spinner">Loading…</div>
        ) : appointments.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No appointments found</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Patient</th>
                <th>Doctor</th>
                <th>Date</th>
                <th>Time slot</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {appointments.map((a) => (
                <tr key={a.appointmentId}>
                  <td>#{a.appointmentId}</td>
                  <td>{a.patientName}</td>
                  <td>{a.doctorName}</td>
                  <td>{new Date(a.appointmentDate).toLocaleDateString('en-IN')}</td>
                  <td>{a.timeSlot}</td>
                  <td><StatusBadge status={a.status} /></td>
                  <td style={{ display: 'flex', gap: '0.5rem' }}>
                    {!['Completed', 'Cancelled', 'Rejected'].includes(a.status) && (
                      <>
                        <button
                          className="btn-sm btn-sm--outline"
                          onClick={() => setRescheduleTarget(a)}
                        >
                          Reschedule
                        </button>
                        <button
                          className="btn-sm btn-sm--danger"
                          onClick={() => handleCancel(a.appointmentId)}
                        >
                          Cancel
                        </button>
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {rescheduleTarget && (
        <RescheduleModal
          appointment={rescheduleTarget}
          onClose={() => setRescheduleTarget(null)}
          onSuccess={() => { setRescheduleTarget(null); load(); }}
        />
      )}
    </AdminLayout>
  );
}