export default function PatientProfilePage() {
  const { roleSpecificId } = useAuth();
  const [patient, setPatient] = useState(null);
  const [form, setForm] = useState({ fullName: '', phoneNumber: '', email: '' });
  const [isEditing, setIsEditing] = useState(false);   // ← new
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [successMsg, setSuccessMsg] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  // ... load() unchanged ...

  function handleEditClick() {
    setIsEditing(true);
    setServerError('');
    setSuccessMsg('');
  }

  function handleCancelEdit() {
    // Reset form to current patient data
    setForm({
      fullName: patient.fullName,
      phoneNumber: patient.phoneNumber,
      email: patient.email ?? '',
    });
    setErrors({});
    setServerError('');
    setIsEditing(false);
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
      setIsEditing(false);   // ← exit edit mode on success
      // Reload patient to reflect changes
      const { data } = await getPatientById(roleSpecificId);
      setPatient(data.data);
    } catch (err) {
      if (isUserFacingError(err)) setServerError(err.message);
      else { console.error(err); setServerError(GENERIC_ERROR); }
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) return <PatientLayout><div className="spinner">Loading profile…</div></PatientLayout>;

  return (
    <PatientLayout>
      <div className="page-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 className="page-title">My profile</h1>
          <p className="page-subtitle">View and update your personal information.</p>
        </div>
        {!isEditing && (
          <button className="btn-sm btn-sm--outline" style={{ padding: '0.5rem 1rem' }} onClick={handleEditClick}>
            Edit profile
          </button>
        )}
      </div>

      <div className="card" style={{ maxWidth: 480 }}>
        {serverError && <div className="form-banner-error">{serverError}</div>}
        {successMsg && <div className="form-banner-success">{successMsg}</div>}

        {/* Read-only info that never changes */}
        <div style={{ marginBottom: '1.25rem', padding: '0.75rem', background: '#F4F7F6', borderRadius: '8px', fontSize: '0.85rem', color: 'var(--color-text-muted)' }}>
          <strong style={{ color: 'var(--color-text)' }}>Date of birth:</strong>{' '}
          {patient ? new Date(patient.dateOfBirth).toLocaleDateString('en-IN') : '—'}&emsp;
          <strong style={{ color: 'var(--color-text)' }}>Gender:</strong>{' '}
          {patient?.gender ?? '—'}
        </div>

        {isEditing ? (
          <form onSubmit={handleSubmit} noValidate>
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

            <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end' }}>
              <button type="button" className="btn-sm btn-sm--outline" style={{ padding: '0.65rem 1.25rem' }} onClick={handleCancelEdit}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.5rem' }} disabled={submitting}>
                {submitting ? 'Saving…' : 'Save changes'}
              </button>
            </div>
          </form>
        ) : (
          /* Read-only view */
          <div className="profile-view">
            <div className="cv-field">
              <span className="cv-label">Full name</span>
              <span className="cv-value">{patient?.fullName}</span>
            </div>
            <div className="cv-field">
              <span className="cv-label">Mobile number</span>
              <span className="cv-value">{patient?.phoneNumber}</span>
            </div>
            <div className="cv-field">
              <span className="cv-label">Email</span>
              <span className="cv-value">{patient?.email || '—'}</span>
            </div>
          </div>
        )}
      </div>
    </PatientLayout>
  );
}