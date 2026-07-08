import { useEffect, useState, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { getDoctorAppointments } from '../../api/doctors';
import { confirmAppointment, rejectAppointment } from '../../api/appointments';
import DoctorLayout from '../../layouts/DoctorLayout';
import RescheduleModal from '../../components/patient/RescheduleModal';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

function StatusBadge({ status }) {
  return (
    <span className={`badge badge--${status.toLowerCase()}`}>{status}</span>
  );
}

export default function DoctorAppointmentsPage() {
  const { roleSpecificId } = useAuth();
  const navigate = useNavigate();

  const [upcoming, setUpcoming] = useState([]);
  const [completed, setCompleted] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [actionError, setActionError] = useState('');
  const [rescheduleTarget, setRescheduleTarget] = useState(null);

  const load = useCallback(async () => {
    if (!roleSpecificId) {
      setLoading(false);
      setError('Could not identify your doctor account. Please sign in again.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const { data } = await getDoctorAppointments(roleSpecificId);
      const all = data.data ?? [];

      setUpcoming(
        all.filter(
          (a) => !['Completed', 'Cancelled', 'Rejected'].includes(a.status)
        )
      );
      setCompleted(all.filter((a) => a.status === 'Completed'));
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, [roleSpecificId]);

  useEffect(() => { load(); }, [load]);

  async function handleConfirm(id) {
    setActionError('');
    try {
      await confirmAppointment(id);
      load();
    } catch (err) {
      if (isUserFacingError(err)) setActionError(err.message);
      else { console.error(err); setActionError(GENERIC_ERROR); }
    }
  }

  async function handleReject(id) {
    const reason = window.prompt('Reason for rejection:');
    if (!reason?.trim()) return;
    setActionError('');
    try {
      await rejectAppointment(id, { reason });
      load();
    } catch (err) {
      if (isUserFacingError(err)) setActionError(err.message);
      else { console.error(err); setActionError(GENERIC_ERROR); }
    }
  }

  if (loading) {
    return (
      <DoctorLayout>
        <div className="spinner">Loading appointments…</div>
      </DoctorLayout>
    );
  }

  return (
    <DoctorLayout>
      <div className="page-header">
        <h1 className="page-title">Appointments</h1>
        <p className="page-subtitle">
          Manage your upcoming and completed appointments.
        </p>
      </div>

      {(error || actionError) && (
        <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>
          {error || actionError}
        </div>
      )}

      {/* ── Upcoming ─────────────────────────────────────────── */}
      <h2 className="section-label">Upcoming</h2>
      <div className="data-table-wrap" style={{ marginBottom: '2.5rem' }}>
        {upcoming.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No upcoming appointments</p>
            <p>New appointment requests will appear here.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Patient</th>
                <th>Date</th>
                <th>Time slot</th>
                <th>Reason</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {upcoming.map((a) => (
                <tr key={a.appointmentId}>
                  <td>{a.patientName}</td>
                  <td>
                    {new Date(a.appointmentDate).toLocaleDateString('en-IN')}
                  </td>
                  <td>{a.timeSlot}</td>
                  <td>{a.reason ?? '—'}</td>
                  <td>
                    <StatusBadge status={a.status} />
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>

                      {/* ── Pending: Confirm / Reject / Reschedule ── */}
                      {a.status === 'Pending' && (
                        <>
                          <button
                            className="btn-sm btn-sm--confirm"
                            onClick={() => handleConfirm(a.appointmentId)}
                          >
                            Confirm
                          </button>
                          <button
                            className="btn-sm btn-sm--danger"
                            onClick={() => handleReject(a.appointmentId)}
                          >
                            Reject
                          </button>
                          <button
                            className="btn-sm btn-sm--outline"
                            onClick={() => setRescheduleTarget(a)}
                          >
                            Reschedule
                          </button>
                        </>
                      )}

                      {/* ── Confirmed: Record Consultation / Reschedule ── */}
                      {a.status === 'Confirmed' && (
                        <>
                          <button
                            className="btn-sm btn-sm--confirm"
                            onClick={() =>
                              navigate(
                                `/doctor/consultations/new?appointmentId=${a.appointmentId}`
                              )
                            }
                          >
                            Record consultation
                          </button>
                          <button
                            className="btn-sm btn-sm--outline"
                            onClick={() => setRescheduleTarget(a)}
                          >
                            Reschedule
                          </button>
                        </>
                      )}

                    {/* ── Rescheduled: Confirm / Reject the new time, or reschedule again ── */}
{a.status === 'Rescheduled' && (
  <>
    <button className="btn-sm btn-sm--confirm" onClick={() => handleConfirm(a.appointmentId)}>
      Confirm
    </button>
    <button className="btn-sm btn-sm--danger" onClick={() => handleReject(a.appointmentId)}>
      Reject
    </button>
   <button
  className="btn-sm btn-sm--outline"
onClick={() => setRescheduleTarget({ ...a, doctorId: a.doctorId ?? roleSpecificId })}>
  Reschedule
</button>
  </>
)}

                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* ── Completed ─────────────────────────────────────────── */}
      <h2 className="section-label">Completed</h2>
      <div className="data-table-wrap">
        {completed.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No completed appointments yet</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Patient</th>
                <th>Date</th>
                <th>Time slot</th>
                <th>Reason</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {completed.map((a) => (
                <tr key={a.appointmentId}>
                  <td>{a.patientName}</td>
                  <td>
                    {new Date(a.appointmentDate).toLocaleDateString('en-IN')}
                  </td>
                  <td>{a.timeSlot}</td>
                  <td>{a.reason ?? '—'}</td>
                  <td>
                    <StatusBadge status={a.status} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {/* ── Reschedule modal ──────────────────────────────────── */}
       {rescheduleTarget && (
              <RescheduleModal
                appointment={rescheduleTarget}
                onClose={() => setRescheduleTarget(null)}
                onSuccess={() => { setRescheduleTarget(null); load(); }}
              />
            )}
    </DoctorLayout>
  );
}