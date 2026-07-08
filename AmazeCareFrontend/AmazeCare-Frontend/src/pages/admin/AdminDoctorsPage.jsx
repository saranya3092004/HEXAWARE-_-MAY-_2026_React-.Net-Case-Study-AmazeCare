import { useEffect, useState, useCallback } from 'react';
import { searchDoctors, createDoctor, updateDoctor, deactivateDoctor, getSpecializations } from '../../api/doctors';
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
  const [editTarget, setEditTarget] = useState(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState({});
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const [specializations, setSpecializations] = useState([]);
  const [addingNewSpecialty, setAddingNewSpecialty] = useState(false);

  useEffect(() => {
    async function loadSpecializations() {
      try {
        const { data } = await getSpecializations();
        setSpecializations(data.data ?? []);
      } catch {
        setSpecializations([]);
      }
    }
    loadSpecializations();
  }, []);

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
    setErrors({});
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
      specialty: doctor.specialization,
      qualification: doctor.qualification,
      designation: doctor.designation,
      experienceYears: doctor.experienceYears,
    });
    setErrors({});
    setFormError('');
    setShowForm(true);
  }

  function validate() {
    const next = {};

    if (!form.name.trim()) next.name = 'Enter the doctor\'s name.';
    if (!form.specialty.trim()) next.specialty = 'Select or enter a specialty.';
    if (!form.qualification.trim()) next.qualification = 'Enter a qualification.';
    if (!form.designation.trim()) next.designation = 'Enter a designation.';

    if (form.experienceYears === '' || form.experienceYears === null) {
      next.experienceYears = 'Enter years of experience.';
    } else if (Number(form.experienceYears) < 0) {
      next.experienceYears = 'Experience cannot be negative.';
    }

    if (!editTarget) {
      if (!form.email.trim()) {
        next.email = 'Enter an email address.';
      } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email.trim())) {
        next.email = 'Enter a valid email address.';
      }
      if (!form.phoneNumber.trim()) {
        next.phoneNumber = 'Enter a phone number.';
      } else if (!/^[6-9]\d{9}$/.test(form.phoneNumber.trim())) {
        next.phoneNumber = 'Enter a valid 10-digit Indian mobile number.';
      }
      if (!form.password) {
        next.password = 'Set an initial password.';
      } else if (form.password.length < 8) {
        next.password = 'Password must be at least 8 characters.';
      }
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setFormError('');
    if (!validate()) return;
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
              <FormField label="Name" error={errors.name}>
                <input
                  className={`form-input ${errors.name ? 'has-error' : ''}`}
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                />
              </FormField>

              <FormField label="Specialty" error={errors.specialty}>
                {addingNewSpecialty ? (
                  <div style={{ display: 'flex', gap: '0.5rem' }}>
                    <input
                      className={`form-input ${errors.specialty ? 'has-error' : ''}`}
                      value={form.specialty}
                      onChange={(e) => setForm({ ...form, specialty: e.target.value })}
                      placeholder="Enter new specialty"
                      autoFocus
                    />
                    <button
                      type="button"
                      className="btn-sm btn-sm--outline"
                      onClick={() => { setAddingNewSpecialty(false); setForm({ ...form, specialty: '' }); }}
                    >
                      Cancel
                    </button>
                  </div>
                ) : (
                  <select
                    className={`form-input ${errors.specialty ? 'has-error' : ''}`}
                    value={form.specialty}
                    onChange={(e) => {
                      if (e.target.value === '__add_new__') {
                        setAddingNewSpecialty(true);
                        setForm({ ...form, specialty: '' });
                      } else {
                        setForm({ ...form, specialty: e.target.value });
                      }
                    }}
                  >
                    <option value="">Select specialty</option>
                    {specializations.map((s) => (
                      <option key={s} value={s}>{s}</option>
                    ))}
                    <option value="__add_new__">+ Add new specialty…</option>
                  </select>
                )}
              </FormField>

              <FormField label="Qualification" error={errors.qualification}>
                <input
                  className={`form-input ${errors.qualification ? 'has-error' : ''}`}
                  value={form.qualification}
                  onChange={(e) => setForm({ ...form, qualification: e.target.value })}
                  placeholder="e.g. MS (OG)"
                />
              </FormField>

              <FormField label="Designation" error={errors.designation}>
                <input
                  className={`form-input ${errors.designation ? 'has-error' : ''}`}
                  value={form.designation}
                  onChange={(e) => setForm({ ...form, designation: e.target.value })}
                  placeholder="e.g. Consultant"
                />
              </FormField>

              <FormField label="Experience (years)" error={errors.experienceYears}>
                <input
                  type="number"
                  className={`form-input ${errors.experienceYears ? 'has-error' : ''}`}
                  value={form.experienceYears}
                  onChange={(e) => setForm({ ...form, experienceYears: e.target.value })}
                  min={0}
                />
              </FormField>

              {!editTarget && (
                <>
                  <FormField label="Email" error={errors.email}>
                    <input
                      type="email"
                      className={`form-input ${errors.email ? 'has-error' : ''}`}
                      value={form.email}
                      onChange={(e) => setForm({ ...form, email: e.target.value })}
                    />
                  </FormField>
                  <FormField label="Phone number" error={errors.phoneNumber}>
                    <input
                      type="tel"
                      className={`form-input ${errors.phoneNumber ? 'has-error' : ''}`}
                      value={form.phoneNumber}
                      onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
                    />
                  </FormField>
                  <FormField label="Initial password" error={errors.password}>
                    <input
                      type="password"
                      className={`form-input ${errors.password ? 'has-error' : ''}`}
                      value={form.password}
                      onChange={(e) => setForm({ ...form, password: e.target.value })}
                    />
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

<div
  className="data-table-wrap"
  style={{ maxHeight: '520px', overflowY: 'auto', border: '1px solid var(--color-border, #e2e2e2)', borderRadius: '8px' }}
>
  {loading ? (
    <div className="spinner">Loading doctors…</div>
  ) : doctors.length === 0 ? (
    <div className="empty-state">
      <p className="empty-state-title">No doctors yet</p>
      <p>Add a doctor to get started.</p>
    </div>
  ) : (
    <table className="data-table">
      <thead style={{ position: 'sticky', top: 0, background: 'var(--color-bg, #fff)', zIndex: 1 }}>
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