import { useEffect, useState, useCallback } from 'react';
import { searchDoctors, createDoctor, updateDoctor, deactivateDoctor } from '../../api/doctors';
import AdminLayout from '../../layouts/AdminLayout';
import FormField from '../../components/FormField';
import '../../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

const emptyForm = {
  name: '', email: '', password: '', phoneNumber: '',
  specialty: '', qualification: '', designation: '', experienceYears: ''
};

export default function AdminDoctorsPage() {
  const [doctors, setDoctors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [editTarget, setEditTarget] = useState(null); // null = add new
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const { data } = await searchDoctors();
      setDoctors(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  function openAdd() {
    setEditTarget(null);
    setForm(emptyForm);
    setFormError('');
    setShowForm(true);
  }

  function openEdit(doctor) {
    setEditTarget(doctor);
   setForm({
  name: doctor.name,
  email: '',
  password: '',
  phoneNumber: '',
  specialty: doctor.specialization,   // already correct — reads from backend's field name
  qualification: doctor.qualification,
  designation: doctor.designation,
  experienceYears: doctor.experienceYears,
});
    setFormError('');
    setShowForm(true);
  }

 async function handleSubmit(e) {
  e.preventDefault();
  setFormError('');
  setSubmitting(true);
  try {
    if (editTarget) {
      await updateDoctor(editTarget.doctorId, {
        name: form.name,
        specialization: form.specialty,
        qualification: form.qualification,
        designation: form.designation,
        experienceYears: Number(form.experienceYears),
      });
    } else {
      const { specialty, ...rest } = form;
      await createDoctor({
        ...rest,
        specialization: specialty,
        experienceYears: Number(form.experienceYears),
      });
    }
    setShowForm(false);
    load();
  } catch (err) {
    if (isUserFacingError(err)) setFormError(err.message);
    else { console.error(err); setFormError(GENERIC_ERROR); }
  } finally {
    setSubmitting(false);
  }
}

  async function handleDeactivate(id) {
    if (!window.confirm('Deactivate this doctor?')) return;
    try {
      await deactivateDoctor(id);
      load();
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    }
  }

  return (
    <AdminLayout>
      <div className="page-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <div>
          <h1 className="page-title">Doctors</h1>
          <p className="page-subtitle">Add, update, or deactivate doctor accounts.</p>
        </div>
        <button className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.25rem' }} onClick={openAdd}>
          + Add doctor
        </button>
      </div>

      {error && <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>{error}</div>}

      {showForm && (
        <div className="card" style={{ marginBottom: '1.5rem', maxWidth: 560 }}>
          <h2 className="section-label" style={{ marginBottom: '1rem' }}>
            {editTarget ? `Edit — ${editTarget.name}` : 'Add new doctor'}
          </h2>
          <form onSubmit={handleSubmit} noValidate>
            {formError && <div className="form-banner-error">{formError}</div>}

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 1rem' }}>
              <FormField label="Name" error={null}>
                <input className="form-input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
              </FormField>
              <FormField label="Specialty" error={null}>
                <input className="form-input" value={form.specialty} onChange={(e) => setForm({ ...form, specialty: e.target.value })} />
              </FormField>
              <FormField label="Qualification" error={null}>
                <input className="form-input" value={form.qualification} onChange={(e) => setForm({ ...form, qualification: e.target.value })} placeholder="e.g. MS (OG)" />
              </FormField>
              <FormField label="Designation" error={null}>
                <input className="form-input" value={form.designation} onChange={(e) => setForm({ ...form, designation: e.target.value })} placeholder="e.g. Consultant" />
              </FormField>
              <FormField label="Experience (years)" error={null}>
                <input type="number" className="form-input" value={form.experienceYears} onChange={(e) => setForm({ ...form, experienceYears: e.target.value })} min={0} />
              </FormField>

              {/* Only show for new doctor */}
              {!editTarget && (
                <>
                  <FormField label="Email" error={null}>
                    <input type="email" className="form-input" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
                  </FormField>
                  <FormField label="Phone number" error={null}>
                    <input type="tel" className="form-input" value={form.phoneNumber} onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })} />
                  </FormField>
                  <FormField label="Initial password" error={null}>
                    <input type="password" className="form-input" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
                  </FormField>
                </>
              )}
            </div>

            <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end', marginTop: '0.5rem' }}>
              <button type="button" className="btn-sm btn-sm--outline" style={{ padding: '0.65rem 1.25rem' }} onClick={() => setShowForm(false)}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.5rem' }} disabled={submitting}>
                {submitting ? 'Saving…' : editTarget ? 'Save changes' : 'Create doctor'}
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="data-table-wrap">
        {loading ? (
          <div className="spinner">Loading doctors…</div>
        ) : doctors.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No doctors yet</p>
            <p>Add a doctor to get started.</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Specialty</th>
                <th>Qualification</th>
                <th>Designation</th>
                <th>Experience</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {doctors.map((d) => (
                <tr key={d.doctorId}>
                  <td>{d.name}</td>
                  <td>{d.specialization}</td>
                  <td>{d.qualification}</td>
                  <td>{d.designation}</td>
                  <td>{d.experienceYears} yrs</td>
                  <td>
                    <span className={`badge ${d.isActive ? 'badge--confirmed' : 'badge--cancelled'}`}>
                      {d.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td style={{ display: 'flex', gap: '0.5rem' }}>
                    <button className="btn-sm btn-sm--outline" onClick={() => openEdit(d)}>Edit</button>
                    {d.isActive && (
                      <button className="btn-sm btn-sm--danger" onClick={() => handleDeactivate(d.doctorId)}>
                        Deactivate
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </AdminLayout>
  );
}