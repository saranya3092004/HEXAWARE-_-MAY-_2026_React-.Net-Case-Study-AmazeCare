import { useState, useEffect, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { searchDoctors, getSpecializations } from '../../api/doctors';
import { bookAppointment, getAvailableSlots } from '../../api/appointments';
import PatientLayout from '../../layouts/PatientLayout';
import FormField from '../../components/FormField';
import '../../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

const today = new Date();
const maxBookableDate = new Date();
maxBookableDate.setDate(today.getDate() + 7);

const minDateStr = today.toISOString().split('T')[0];
const maxDateStr = maxBookableDate.toISOString().split('T')[0];

const VISIT_TYPES = [
  { value: 0, label: 'General Check-up' },
  { value: 1, label: 'Follow-up' },
  { value: 2, label: 'Specific Issue' },
  { value: 3, label: 'Emergency' },
  { value: 4, label: 'Lab Test Only' },
];

export default function BookAppointmentPage() {
  const navigate = useNavigate();
  const [search, setSearch] = useState({ name: '', specialty: '' });
  const [doctors, setDoctors] = useState([]);
  const [searchLoading, setSearchLoading] = useState(false);
  const [searchError, setSearchError] = useState('');
  const [selectedDoctor, setSelectedDoctor] = useState(null);

  const [specializations, setSpecializations] = useState([]);

  const [form, setForm] = useState({
    appointmentDate: '',
    timeSlot: '',
    reason: '',
    visitType: 0,
  });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const [availableSlots, setAvailableSlots] = useState([]);
  const [slotsLoading, setSlotsLoading] = useState(false);

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

  const handleSearch = useCallback(async () => {
    setSearchLoading(true);
    setSearchError('');
    try {
      const { data } = await searchDoctors({
        name: search.name || undefined,
        specialization: search.specialty || undefined,
      });
      setDoctors(data.data ?? []);
    } catch (err) {
      if (isUserFacingError(err)) {
        setSearchError(err.message);
      } else {
        console.error('Doctor search error:', err);
        setSearchError(GENERIC_ERROR);
      }
      setDoctors([]);
    } finally {
      setSearchLoading(false);
    }
  }, [search]);

  useEffect(() => { handleSearch(); }, []);

  useEffect(() => {
    async function loadSlots() {
      if (!selectedDoctor || !form.appointmentDate) {
        setAvailableSlots([]);
        return;
      }
      setSlotsLoading(true);
      try {
        const { data } = await getAvailableSlots(selectedDoctor.doctorId, form.appointmentDate);
        setAvailableSlots(data.data ?? []);
      } catch (err) {
        if (isUserFacingError(err)) {
          setServerError(err.message);
        } else {
          console.error('Get available slots error:', err);
          setServerError(GENERIC_ERROR);
        }
        setAvailableSlots([]);
      } finally {
        setSlotsLoading(false);
      }
    }
    loadSlots();
  }, [selectedDoctor, form.appointmentDate]);

  function validate() {
    const next = {};
    if (!selectedDoctor) next.doctor = 'Select a doctor.';

    if (!form.appointmentDate) {
      next.appointmentDate = 'Choose a date.';
    } else {
      const selected = new Date(form.appointmentDate);
      selected.setHours(0, 0, 0, 0);
      const todayStart = new Date();
      todayStart.setHours(0, 0, 0, 0);
      const maxStart = new Date(todayStart);
      maxStart.setDate(todayStart.getDate() + 7);

      if (selected < todayStart) {
        next.appointmentDate = 'Date cannot be in the past.';
      } else if (selected > maxStart) {
        next.appointmentDate = 'Appointments can only be booked up to 7 days in advance.';
      }
    }

    if (!form.timeSlot.trim()) next.timeSlot = 'Select a time slot.';

    if (!form.reason.trim()) {
      next.reason = 'Briefly describe your symptoms or concern.';
    } else if (form.reason.trim().length < 10) {
      next.reason = 'Please provide a bit more detail (at least 10 characters).';
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleBook(e) {
    e.preventDefault();
    setServerError('');
    if (!validate()) return;
    setSubmitting(true);
    try {
      await bookAppointment({
        doctorId: selectedDoctor.doctorId,
        appointmentDate: form.appointmentDate,
        timeSlot: form.timeSlot,
        reason: form.reason,
        visitType: Number(form.visitType),
      });
      navigate('/patient/appointments');
    } catch (err) {
      if (isUserFacingError(err)) {
        setServerError(err.message);
      } else {
        console.error('Book appointment error:', err);
        setServerError(GENERIC_ERROR);
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <PatientLayout>
      <div className="page-header">
        <h1 className="page-title">Book an appointment</h1>
        <p className="page-subtitle">Search for a doctor and choose a convenient slot.</p>
      </div>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h2 className="section-label" style={{ marginBottom: '1rem' }}>Step 1 — Find a doctor</h2>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr auto', gap: '0.75rem', alignItems: 'flex-end' }}>
          <FormField label="Doctor name" error={null}>
            <input
              type="text"
              className="form-input"
              placeholder="e.g. Poornima"
              value={search.name}
              onChange={(e) => setSearch({ ...search, name: e.target.value })}
            />
          </FormField>
          <FormField label="Specialty" error={null}>
            <select
              className="form-input"
              value={search.specialty}
              onChange={(e) => setSearch({ ...search, specialty: e.target.value })}
            >
              <option value="">All specialties</option>
              {specializations.map((s) => (
                <option key={s} value={s}>{s}</option>
              ))}
            </select>
          </FormField>
          <button
            type="button"
            className="btn-primary"
            style={{ width: 'auto', padding: '0.7rem 1.25rem', marginBottom: '1.25rem' }}
            onClick={handleSearch}
            disabled={searchLoading}
          >
            {searchLoading ? 'Searching…' : 'Search'}
          </button>
        </div>

        {errors.doctor && <p className="form-error">{errors.doctor}</p>}
        {searchError && <div className="form-banner-error" style={{ marginTop: '0.75rem' }}>{searchError}</div>}

        {doctors.length === 0 && !searchLoading && !searchError ? (
          <p style={{ color: 'var(--color-text-muted)', fontSize: '0.9rem' }}>
            No doctors found. Try different search terms.
          </p>
        ) : (
          <div className="doctor-grid">
            {doctors.map((d) => (
              <button
                key={d.doctorId}
                type="button"
                className={`doctor-card ${selectedDoctor?.doctorId === d.doctorId ? 'doctor-card--selected' : ''}`}
                onClick={() => setSelectedDoctor(d)}
              >
                <span className="doctor-card-avatar">{d.name[0]}</span>
                <span className="doctor-card-name">{d.name}</span>
                <span className="doctor-card-spec">{d.specialty}</span>
                <span className="doctor-card-meta">{d.designation} · {d.experienceYears} yrs</span>
              </button>
            ))}
          </div>
        )}
      </div>

      {selectedDoctor && (
        <div className="card">
          <h2 className="section-label" style={{ marginBottom: '1rem' }}>
            Step 2 — Appointment details with Dr. {selectedDoctor.name}
          </h2>

          <form onSubmit={handleBook} noValidate>
            {serverError && <div className="form-banner-error">{serverError}</div>}

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 1rem' }}>
              <FormField label="Preferred date" error={errors.appointmentDate}>
                <input
                  type="date"
                  className={`form-input ${errors.appointmentDate ? 'has-error' : ''}`}
                  value={form.appointmentDate}
                  onChange={(e) => setForm({ ...form, appointmentDate: e.target.value, timeSlot: '' })}
                  min={minDateStr}
                  max={maxDateStr}
                />
              </FormField>

              <FormField label="Preferred time slot" error={errors.timeSlot}>
                <select
                  className={`form-input ${errors.timeSlot ? 'has-error' : ''}`}
                  value={form.timeSlot}
                  onChange={(e) => setForm({ ...form, timeSlot: e.target.value })}
                  disabled={slotsLoading || !form.appointmentDate}
                >
                  <option value="">
                    {slotsLoading ? 'Loading slots…' : !form.appointmentDate ? 'Pick a date first' : 'Select a time slot'}
                  </option>
                  {availableSlots.map((slot) => (
                    <option key={slot} value={slot}>{slot}</option>
                  ))}
                </select>
                {!slotsLoading && form.appointmentDate && availableSlots.length === 0 && (
                  <p style={{ fontSize: '0.8rem', color: 'var(--color-text-muted)', marginTop: '0.25rem' }}>
                    No slots available for this date.
                  </p>
                )}
              </FormField>
            </div>

            <FormField label="Nature of visit" error={null}>
              <select
                className="form-input"
                value={form.visitType}
                onChange={(e) => setForm({ ...form, visitType: e.target.value })}
              >
                {VISIT_TYPES.map(({ value, label }) => (
                  <option key={value} value={value}>{label}</option>
                ))}
              </select>
            </FormField>

            <FormField label="Symptoms or health concern" error={errors.reason}>
              <textarea
                className={`form-input ${errors.reason ? 'has-error' : ''}`}
                rows={3}
                placeholder="Brief description of what you're experiencing…"
                value={form.reason}
                onChange={(e) => setForm({ ...form, reason: e.target.value })}
                style={{ resize: 'vertical' }}
              />
            </FormField>

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem' }}>
              <button
                type="button"
                className="btn-sm btn-sm--outline"
                style={{ padding: '0.65rem 1.25rem' }}
                onClick={() => setSelectedDoctor(null)}
              >
                Change doctor
              </button>
              <button
                type="submit"
                className="btn-primary"
                style={{ width: 'auto', padding: '0.65rem 1.5rem' }}
                disabled={submitting}
              >
                {submitting ? 'Booking…' : 'Confirm booking'}
              </button>
            </div>
          </form>
        </div>
      )}
    </PatientLayout>
  );
}