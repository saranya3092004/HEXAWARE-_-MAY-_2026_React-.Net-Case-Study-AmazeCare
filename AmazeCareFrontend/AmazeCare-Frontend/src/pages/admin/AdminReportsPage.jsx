import { useState } from 'react';
import { getAppointmentReport, getPatientReport } from '../../api/admin';
import AdminLayout from '../../layouts/AdminLayout';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

export default function AdminReportsPage() {
  const [tab, setTab] = useState('appointments');
  const [appointmentData, setAppointmentData] = useState([]);
  const [patientData, setPatientData] = useState([]);
  const [filter, setFilter] = useState({ fromDate: '', toDate: '', status: '' });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function loadAppointments() {
    setLoading(true);
    setError('');
    try {
      const params = {};
      if (filter.fromDate) params.fromDate = filter.fromDate;
      if (filter.toDate) params.toDate = filter.toDate;
      if (filter.status) params.status = filter.status;
      const { data } = await getAppointmentReport(params);
      setAppointmentData(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }

  async function loadPatients() {
    setLoading(true);
    setError('');
    try {
      const params = {};
      if (filter.fromDate) params.fromDate = filter.fromDate;
      if (filter.toDate) params.toDate = filter.toDate;
      const { data } = await getPatientReport(params);
      setPatientData(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }

  function handleGenerate() {
    if (tab === 'appointments') loadAppointments();
    else loadPatients();
  }

  return (
    <AdminLayout>
      <div className="page-header">
        <h1 className="page-title">Reports</h1>
        <p className="page-subtitle">Generate appointment and patient registration reports.</p>
      </div>

      {/* Tab switcher */}
      <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '1.5rem' }}>
        {['appointments', 'patients'].map((t) => (
          <button
            key={t}
            className={`btn-sm ${tab === t ? 'btn-sm--confirm' : 'btn-sm--outline'}`}
            style={{ padding: '0.5rem 1rem', textTransform: 'capitalize' }}
            onClick={() => setTab(t)}
          >
            {t}
          </button>
        ))}
      </div>

      {/* Filters */}
      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr 1fr auto', gap: '0.75rem', alignItems: 'flex-end' }}>
          <div>
            <label className="form-label">From date</label>
            <input type="date" className="form-input" value={filter.fromDate}
              onChange={(e) => setFilter({ ...filter, fromDate: e.target.value })} />
          </div>
          <div>
            <label className="form-label">To date</label>
            <input type="date" className="form-input" value={filter.toDate}
              onChange={(e) => setFilter({ ...filter, toDate: e.target.value })} />
          </div>
          {tab === 'appointments' && (
            <div>
              <label className="form-label">Status</label>
              <select className="form-input" value={filter.status}
                onChange={(e) => setFilter({ ...filter, status: e.target.value })}>
                <option value="">All</option>
                <option>Pending</option>
                <option>Confirmed</option>
                <option>Completed</option>
                <option>Cancelled</option>
                <option>Rejected</option>
                <option>Rescheduled</option>
              </select>
            </div>
          )}
          <button className="btn-primary" style={{ width: 'auto', padding: '0.7rem 1.25rem', marginBottom: '0' }} onClick={handleGenerate}>
            Generate
          </button>
        </div>
      </div>

      {error && <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>{error}</div>}

      {/* Results */}
      {loading ? (
        <div className="spinner">Generating report…</div>
      ) : tab === 'appointments' && appointmentData.length > 0 ? (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th><th>Patient</th><th>Doctor</th><th>Date</th><th>Time slot</th><th>Status</th>
              </tr>
            </thead>
            <tbody>
              {appointmentData.map((a) => (
                <tr key={a.appointmentId}>
                  <td>#{a.appointmentId}</td>
                  <td>{a.patientName}</td>
                  <td>{a.doctorName}</td>
                  <td>{new Date(a.appointmentDate).toLocaleDateString('en-IN')}</td>
                  <td>{a.timeSlot}</td>
                  <td><span className={`badge badge--${a.status.toLowerCase()}`}>{a.status}</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : tab === 'patients' && patientData.length > 0 ? (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>ID</th><th>Name</th><th>Gender</th><th>Phone</th><th>Registered</th>
              </tr>
            </thead>
            <tbody>
              {patientData.map((p) => (
                <tr key={p.patientId}>
                  <td>#{p.patientId}</td>
                  <td>{p.fullName}</td>
                  <td>{p.gender}</td>
                  <td>{p.phoneNumber}</td>
                  <td>{new Date(p.createdAt).toLocaleDateString('en-IN')}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}
    </AdminLayout>
  );
}