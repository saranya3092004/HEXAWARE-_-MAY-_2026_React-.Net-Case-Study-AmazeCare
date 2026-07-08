import { useEffect, useState, useCallback } from 'react';
import { useAuth } from '../../context/AuthContext';
import { getAppointments, cancelAppointment } from '../../api/appointments';
import PatientLayout from '../../layouts/PatientLayout';
import RescheduleModal from '../../components/patient/RescheduleModal';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

function StatusBadge({ status }) {
  return (
    <span className={`badge badge--${status.toLowerCase()}`}>{status}</span>
  );
}

const TABS = [
  { key: 'upcoming', label: 'Upcoming' },
  { key: 'completed', label: 'Completed' },
  { key: 'cancelledOrRejected', label: 'Cancelled / Rejected' },
];

export default function PatientAppointmentsPage() {
  const { roleSpecificId } = useAuth();
  const [upcoming, setUpcoming] = useState([]);
  const [completed, setCompleted] = useState([]);
  const [cancelledOrRejected, setCancelledOrRejected] = useState([]);
  const [activeTab, setActiveTab] = useState('upcoming');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [rescheduleTarget, setRescheduleTarget] = useState(null);
  const [actionError, setActionError] = useState('');

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const { data } = await getAppointments();
      const all = data.data ?? [];
      const now = new Date();
      setUpcoming(
        all.filter(
          (a) =>
            !['Completed', 'Cancelled', 'Rejected'].includes(a.status) &&
            new Date(a.appointmentDate) >= now
        )
      );
      setCompleted(all.filter((a) => a.status === 'Completed'));
      setCancelledOrRejected(all.filter((a) => ['Cancelled', 'Rejected'].includes(a.status)));
    } catch (err) {
      if (isUserFacingError(err)) {
        setError(err.message);
      } else {
        console.error('Load appointments error:', err);
        setError(GENERIC_ERROR);
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  async function handleCancel(id) {
    const reason = window.prompt('Reason for cancellation:');
    if (!reason?.trim()) return;
    setActionError('');
    try {
      await cancelAppointment(id, { reason });
      load();
    } catch (err) {
      if (isUserFacingError(err)) {
        setActionError(err.message);
      } else {
        console.error('Cancel appointment error:', err);
        setActionError(GENERIC_ERROR);
      }
    }
  }

  if (loading) return <PatientLayout><div className="spinner">Loading appointments…</div></PatientLayout>;

  const activeList =
    activeTab === 'upcoming' ? upcoming :
    activeTab === 'completed' ? completed :
    cancelledOrRejected;

  return (
    <PatientLayout>
      <div className="page-header">
        <h1 className="page-title">My Appointments</h1>
        <p className="page-subtitle">Track upcoming visits and past consultations.</p>
      </div>

      {(error || actionError) && (
        <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>
          {error || actionError}
        </div>
      )}

      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem' }}>
        {TABS.map((tab) => (
          <button
            key={tab.key}
            type="button"
            className={activeTab === tab.key ? 'btn-sm btn-sm--confirm' : 'btn-sm btn-sm--outline'}
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div className="data-table-wrap">
        {activeList.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">
              {activeTab === 'upcoming' && 'No upcoming appointments'}
              {activeTab === 'completed' && 'No completed appointments yet'}
              {activeTab === 'cancelledOrRejected' && 'Nothing here'}
            </p>
            {activeTab === 'upcoming' && <p>Book a new appointment to get started.</p>}
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Doctor</th>
                <th>Date</th>
                <th>Time slot</th>
                <th>Reason</th>
                <th>Status</th>
                {activeTab === 'upcoming' && <th>Actions</th>}
                {activeTab === 'cancelledOrRejected' && <th>Cancellation reason</th>}
              </tr>
            </thead>
            <tbody>
              {activeList.map((a) => (
                <tr key={a.appointmentId}>
                  <td>{a.doctorName}</td>
                  <td>{new Date(a.appointmentDate).toLocaleDateString('en-IN')}</td>
                  <td>{a.timeSlot}</td>
                  <td>{a.reason ?? '—'}</td>
                  <td><StatusBadge status={a.status} /></td>
                  {activeTab === 'upcoming' && (
                    <td style={{ display: 'flex', gap: '0.5rem' }}>
                      <button className="btn-sm btn-sm--outline" onClick={() => setRescheduleTarget(a)}>
                        Reschedule
                      </button>
                      <button className="btn-sm btn-sm--danger" onClick={() => handleCancel(a.appointmentId)}>
                        Cancel
                      </button>
                    </td>
                  )}
                  {activeTab === 'cancelledOrRejected' && (
                    <td>{a.cancellationReason ?? '—'}</td>
                  )}
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
    </PatientLayout>
  );
}