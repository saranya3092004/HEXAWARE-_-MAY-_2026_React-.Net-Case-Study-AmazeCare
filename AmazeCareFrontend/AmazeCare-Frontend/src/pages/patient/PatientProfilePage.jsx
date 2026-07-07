import { useEffect, useState } from 'react';
import { useAuth } from '../../context/AuthContext';
import { getPatientById, updatePatient } from '../../api/patients';
import PatientLayout from '../../layouts/PatientLayout';
import FormField from '../../components/FormField';
import '../../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';


export default function PatientProfilePage() {
  const { roleSpecificId } = useAuth();
  const [patient, setPatient] = useState(null);
  const [form, setForm] = useState({ fullName: '', phoneNumber: '', email: '' });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [successMsg, setSuccessMsg] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  const GENDER_LABELS = {
  0: 'Male',
  1: 'Female',
  2: 'Other',
  3: 'Prefer not to say',
};

  useEffect(() => {
    async function load() {
      try {
        const { data } = await getPatientById(roleSpecificId);
        const p = data.data;
        setPatient(p);
        setForm({
          fullName: p.fullName,
          phoneNumber: p.phoneNumber,
          email: p.email ?? '',
        });
      } catch (err) {
  if (isUserFacingError(err)) {
    setServerError(err.message);
  } else {
    console.error('Load profile error:', err);
    setServerError(GENERIC_ERROR);
  }
} finally {
        setLoading(false);
      }
    }
    if (roleSpecificId) load();
  }, [roleSpecificId]);

  function validate() {
    const next = {};
    if (!form.fullName.trim()) next.fullName = 'Name is required.';
    if (!form.phoneNumber.trim()) next.phoneNumber = 'Phone number is required.';
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    setSuccessMsg('');
    if (!validate()) return;
    setSubmitting(true);
    try {
      await updatePatient(roleSpecificId, form);
      setSuccessMsg('Profile updated successfully.');
    }  catch (err) {
  if (isUserFacingError(err)) {
    setServerError(err.message);
  } else {
    console.error('Update profile error:', err);
    setServerError(GENERIC_ERROR);
  }
 } finally {
      setSubmitting(false);
    }
  }

  if (loading) return <PatientLayout><div className="spinner">Loading profile…</div></PatientLayout>;

  return (
    <PatientLayout>
      <div className="page-header">
        <h1 className="page-title">My profile</h1>
        <p className="page-subtitle">Update your contact information.</p>
      </div>

      <div className="card" style={{ maxWidth: 480 }}>
        <form onSubmit={handleSubmit} noValidate>
          {serverError && <div className="form-banner-error">{serverError}</div>}
          {successMsg && <div className="form-banner-success">{successMsg}</div>}

          <div style={{ marginBottom: '1.25rem', padding: '0.75rem', background: '#F4F7F6', borderRadius: '8px', fontSize: '0.85rem', color: 'var(--color-text-muted)' }}>
  <strong style={{ color: 'var(--color-text)' }}>Date of birth:</strong>{' '}
  {patient ? new Date(patient.dateOfBirth).toLocaleDateString('en-IN') : '—'}&emsp;
  <strong style={{ color: 'var(--color-text)' }}>Gender:</strong>{' '}
  {patient ? (GENDER_LABELS[patient.gender] ?? '—') : '—'}
</div>

          <FormField label="Full name" error={errors.fullName}>
            <input
              type="text"
              className={`form-input ${errors.fullName ? 'has-error' : ''}`}
              value={form.fullName}
              onChange={(e) => setForm({ ...form, fullName: e.target.value })}
            />
          </FormField>

          <FormField label="Mobile number" error={errors.phoneNumber}>
            <input
              type="tel"
              className={`form-input ${errors.phoneNumber ? 'has-error' : ''}`}
              value={form.phoneNumber}
              onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
            />
          </FormField>

          <FormField label="Email" error={null}>
            <input
              type="email"
              className="form-input"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              placeholder="Optional"
            />
          </FormField>

          <button type="submit" className="btn-primary" disabled={submitting}>
            {submitting ? 'Saving…' : 'Save changes'}
          </button>
        </form>
      </div>
    </PatientLayout>
  );
}