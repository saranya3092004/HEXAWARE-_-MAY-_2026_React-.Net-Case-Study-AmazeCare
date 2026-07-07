import { useEffect, useState, useCallback } from 'react';
import { getConsultations } from '../../api/consultations';
import DoctorLayout from '../../layouts/DoctorLayout';
import ConsultationModal from '../../components/doctor/ConsultationModal';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

export default function DoctorConsultationsPage() {
  const [consultations, setConsultations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selected, setSelected] = useState(null);
  const [modalMode, setModalMode] = useState('view'); // 'view' | 'edit'

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const { data } = await getConsultations();
      setConsultations(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  if (loading) {
    return (
      <DoctorLayout>
        <div className="spinner">Loading consultations…</div>
      </DoctorLayout>
    );
  }

  return (
    <DoctorLayout>
      <div className="page-header">
        <h1 className="page-title">Consultations</h1>
        <p className="page-subtitle">View and update consultation records.</p>
      </div>

      {error && (
        <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>
          {error}
        </div>
      )}

      <div className="data-table-wrap">
        {consultations.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No consultations yet</p>
            <p>Consultations appear here after you record them for a confirmed appointment.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Patient</th>
                <th>Symptoms</th>
                <th>Diagnosis</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {consultations.map((c) => (
                <tr key={c.consultationId}>
                  <td>{new Date(c.consultationDate).toLocaleDateString('en-IN')}</td>
                  <td>{c.patientName}</td>
                  <td style={{ maxWidth: 200, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                    {c.currentSymptoms}
                  </td>
                  <td>{c.diagnosis ?? '—'}</td>
                  <td style={{ display: 'flex', gap: '0.5rem' }}>
                    <button
                      className="btn-sm btn-sm--outline"
                      onClick={() => { setSelected(c); setModalMode('view'); }}
                    >
                      View
                    </button>
                    <button
                      className="btn-sm btn-sm--confirm"
                      onClick={() => { setSelected(c); setModalMode('edit'); }}
                    >
                      Edit
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      {selected && (
        <ConsultationModal
          consultation={selected}
          mode={modalMode}
          onClose={() => setSelected(null)}
          onSuccess={() => { setSelected(null); load(); }}
        />
      )}
    </DoctorLayout>
  );
}