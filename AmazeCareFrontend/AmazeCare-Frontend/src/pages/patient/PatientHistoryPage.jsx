import { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { getPatientHistory } from '../../api/patients';
import PatientLayout from '../../layouts/PatientLayout';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';


export default function PatientHistoryPage() {
  const { roleSpecificId } = useAuth();
  const [history, setHistory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function load() {
      try {
        const { data } = await getPatientHistory(roleSpecificId);
        setHistory(data.data);
      } catch (err) {
  if (isUserFacingError(err)) {
    setError(err.message);
  } else {
    console.error('Load history error:', err);
    setError(GENERIC_ERROR);
  }
} finally {
        setLoading(false);
      }
    }
    if (roleSpecificId) load();
  }, [roleSpecificId]);

  if (loading) return <PatientLayout><div className="spinner">Loading history…</div></PatientLayout>;

  return (
    <PatientLayout>
      <div className="page-header">
        <h1 className="page-title">Medical history</h1>
        <p className="page-subtitle">Your past consultations and diagnoses.</p>
      </div>

      {error && <div className="form-banner-error">{error}</div>}

      {!history?.consultations?.length ? (
        <div className="empty-state">
          <p className="empty-state-title">No consultations yet</p>
          <p>Your medical history will appear here after your first completed appointment.</p>
        </div>
      ) : (
        <div className="data-table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Doctor</th>
                <th>Symptoms</th>
                <th>Diagnosis</th>
                <th>Treatment plan</th>
              </tr>
            </thead>
            <tbody>
              {history.consultations.map((c) => (
                <tr key={c.consultationId}>
                  <td>{new Date(c.consultationDate).toLocaleDateString('en-IN')}</td>
                  <td>{c.doctorName}</td>
                  <td>{c.currentSymptoms}</td>
                  <td>{c.diagnosis ?? '—'}</td>
                  <td>{c.treatmentPlan ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PatientLayout>
  );
}