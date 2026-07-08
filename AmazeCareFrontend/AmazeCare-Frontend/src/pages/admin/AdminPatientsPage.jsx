import { useEffect, useState, useCallback } from 'react';
import {
  searchPatients,
  registerWalkInPatient,
  updatePatient,
  deactivatePatient,
} from '../../api/patients';
import AdminLayout from '../../layouts/AdminLayout';
import FormField from '../../components/FormField';
import '../../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

const GENDER_LABELS = {
  0: 'Male',
  1: 'Female',
  2: 'Other',
  3: 'Prefer not to say',
};

const emptyForm = {
  fullName: '',
  dateOfBirth: '',
  gender: 0,
  address: '',
  phoneNumber: '',
  email: '',
};

export default function AdminPatientsPage() {
  const [patients, setPatients] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  const [showForm, setShowForm] = useState(false);
  const [editTarget, setEditTarget] = useState(null); // null = add new
  const [form, setForm] = useState(emptyForm);
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const { data } = await searchPatients({ searchTerm: searchTerm || undefined });
      setPatients(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) setError(err.message);
      else { console.error(err); setError(GENERIC_ERROR); }
    } finally {
      setLoading(false);
    }
  }, [searchTerm]);

  useEffect(() => { load(); }, [load]);

  function handleSearchSubmit(e) {
    e.preventDefault();
    load();
  }

  function openAdd() {
    setEditTarget(null);
    setForm(emptyForm);
    setFormError('');
    setShowForm(true);
  }

  function openEdit(patient) {
    setEditTarget(patient);
    setForm({
      fullName: patient.fullName,
      dateOfBirth: '', // not editable via UpdatePatientRequest, kept blank
      gender: patient.gender, // read-only display only, not sent on update
      address: '', // PatientResponse doesn't return address, left blank
      phoneNumber: patient.phoneNumber,
      email: patient.email ?? '',
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
        await updatePatient(editTarget.patientId, {
          fullName: form.fullName,
          phoneNumber: form.phoneNumber,
          address: form.address || null,
          email: form.email || null,
        });
      } else {
        await registerWalkInPatient({
          fullName: form.fullName,
          dateOfBirth: form.dateOfBirth,
          gender: Number(form.gender),
          address: form.address || null,
          phoneNumber: form.phoneNumber,
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
    if (!window.confirm('Deactivate this patient?')) return;
    try {
      await deactivatePatient(id);
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
          <h1 className="page-title">Patients</h1>
          <p className="page-subtitle">View, register, and manage patient accounts.</p>
        </div>
        <button className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.25rem' }} onClick={openAdd}>
          + Register walk-in patient
        </button>
      </div>

      {error && <div className="form-banner-error" style={{ marginBottom: '1.5rem' }}>{error}</div>}

      <form onSubmit={handleSearchSubmit} style={{ marginBottom: '1.5rem', display: 'flex', gap: '0.75rem', alignItems: 'flex-end' }}>
        <FormField label="Search by name or phone" error={null}>
          <input
            type="text"
            className="form-input"
            placeholder="e.g. Priya or 98765..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </FormField>
        <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '0.7rem 1.25rem' }}>
          Search
        </button>
      </form>

      {showForm && (
        <div className="card" style={{ marginBottom: '1.5rem', maxWidth: 560 }}>
          <h2 className="section-label" style={{ marginBottom: '1rem' }}>
            {editTarget ? `Edit — ${editTarget.fullName}` : 'Register walk-in patient'}
          </h2>
          <form onSubmit={handleSubmit} noValidate>
            {formError && <div className="form-banner-error">{formError}</div>}

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 1rem' }}>
              <FormField label="Full name" error={null}>
                <input
                  className="form-input"
                  value={form.fullName}
                  onChange={(e) => setForm({ ...form, fullName: e.target.value })}
                />
              </FormField>

              <FormField label="Phone number" error={null}>
                <input
                  type="tel"
                  className="form-input"
                  value={form.phoneNumber}
                  onChange={(e) => setForm({ ...form, phoneNumber: e.target.value })}
                />
              </FormField>

              {/* Only for new patient — not part of UpdatePatientRequest */}
              {!editTarget && (
                <>
                  <FormField label="Date of birth" error={null}>
                    <input
                      type="date"
                      className="form-input"
                      value={form.dateOfBirth}
                      onChange={(e) => setForm({ ...form, dateOfBirth: e.target.value })}
                      max={new Date().toISOString().split('T')[0]}
                    />
                  </FormField>

                  <FormField label="Gender" error={null}>
                    <select
                      className="form-input"
                      value={form.gender}
                      onChange={(e) => setForm({ ...form, gender: Number(e.target.value) })}
                    >
                      <option value={0}>Male</option>
                      <option value={1}>Female</option>
                      <option value={2}>Other</option>
                      <option value={3}>Prefer Not To Say</option>
                    </select>
                  </FormField>
                </>
              )}

              <FormField label="Address" error={null}>
                <input
                  className="form-input"
                  value={form.address}
                  onChange={(e) => setForm({ ...form, address: e.target.value })}
                  placeholder="Optional"
                />
              </FormField>

              {/* Only for edit — not part of RegisterWalkInPatientRequest */}
              {editTarget && (
                <FormField label="Email" error={null}>
                  <input
                    type="email"
                    className="form-input"
                    value={form.email}
                    onChange={(e) => setForm({ ...form, email: e.target.value })}
                    placeholder="Optional"
                  />
                </FormField>
              )}
            </div>

            <div style={{ display: 'flex', gap: '0.75rem', justifyContent: 'flex-end', marginTop: '0.5rem' }}>
              <button type="button" className="btn-sm btn-sm--outline" style={{ padding: '0.65rem 1.25rem' }} onClick={() => setShowForm(false)}>
                Cancel
              </button>
              <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.5rem' }} disabled={submitting}>
                {submitting ? 'Saving…' : editTarget ? 'Save changes' : 'Register patient'}
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="data-table-wrap">
        {loading ? (
          <div className="spinner">Loading patients…</div>
        ) : patients.length === 0 ? (
          <div className="empty-state">
            <p className="empty-state-title">No patients found</p>
          </div>
        ) : (
          <table className="data-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Age</th>
                <th>Gender</th>
                <th>Phone</th>
                <th>Email</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {patients.map((p) => (
                <tr key={p.patientId}>
                  <td>{p.fullName}</td>
                  <td>{p.age}</td>
                  <td>{GENDER_LABELS[p.gender] ?? '—'}</td>
                  <td>{p.phoneNumber}</td>
                  <td>{p.email ?? '—'}</td>
                  <td>
                    <span className={`badge ${p.isActive ? 'badge--confirmed' : 'badge--cancelled'}`}>
                      {p.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td style={{ display: 'flex', gap: '0.5rem' }}>
                    <button className="btn-sm btn-sm--outline" onClick={() => openEdit(p)}>Edit</button>
                    {p.isActive && (
                      <button className="btn-sm btn-sm--danger" onClick={() => handleDeactivate(p.patientId)}>
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