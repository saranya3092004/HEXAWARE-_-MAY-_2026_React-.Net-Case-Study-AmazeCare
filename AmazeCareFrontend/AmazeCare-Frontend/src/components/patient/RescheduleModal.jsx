import { useState, useEffect } from 'react';
import { rescheduleAppointment, getAvailableSlots } from '../../api/appointments';
import FormField from '../FormField';
import '../../components/forms.css';
import './Modal.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

const today = new Date();
const maxBookableDate = new Date();
maxBookableDate.setDate(today.getDate() + 7);

const minDateStr = today.toISOString().split('T')[0];
const maxDateStr = maxBookableDate.toISOString().split('T')[0];

export default function RescheduleModal({ appointment, onClose, onSuccess }) {
  const [form, setForm] = useState({ newAppointmentDate: '', newTimeSlot: '' });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const [availableSlots, setAvailableSlots] = useState([]);
  const [slotsLoading, setSlotsLoading] = useState(false);

  useEffect(() => {
    async function loadSlots() {
      if (!form.newAppointmentDate) {
        setAvailableSlots([]);
        return;
      }
      setSlotsLoading(true);
      try {
const { data } = await getAvailableSlots(appointment.doctorId, form.newAppointmentDate, appointment.appointmentId);      setAvailableSlots(data.data ?? []);
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
  }, [appointment.doctorId, form.newAppointmentDate]);

  function validate() {
    const next = {};

    if (!form.newAppointmentDate) {
      next.newAppointmentDate = 'Choose a new date.';
    } else {
      const selected = new Date(form.newAppointmentDate);
      selected.setHours(0, 0, 0, 0);
      const todayStart = new Date();
      todayStart.setHours(0, 0, 0, 0);
      const maxStart = new Date(todayStart);
      maxStart.setDate(todayStart.getDate() + 7);

      if (selected < todayStart) {
        next.newAppointmentDate = 'Date cannot be in the past.';
      } else if (selected > maxStart) {
        next.newAppointmentDate = 'Appointments can only be rescheduled up to 7 days in advance.';
      }
    }

    if (!form.newTimeSlot.trim()) next.newTimeSlot = 'Select a time slot.';
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    if (!validate()) return;
    setSubmitting(true);
    try {
      await rescheduleAppointment(appointment.appointmentId, {
        newAppointmentDate: form.newAppointmentDate,
        newTimeSlot: form.newTimeSlot,
      });
      onSuccess();
    } catch (err) {
      if (isUserFacingError(err)) {
        setServerError(err.message);
      } else {
        console.error('Reschedule error:', err);
        setServerError(GENERIC_ERROR);
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-box" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2 className="modal-title">Reschedule appointment</h2>
          <button className="modal-close" onClick={onClose} aria-label="Close">✕</button>
        </div>
        <p className="modal-subtitle">
          Currently booked with <strong>{appointment.doctorName}</strong> on{' '}
          {new Date(appointment.appointmentDate).toLocaleDateString('en-IN')} at {appointment.timeSlot}.
        </p>

        <form onSubmit={handleSubmit} noValidate>
          {serverError && <div className="form-banner-error">{serverError}</div>}

          <FormField label="New date" error={errors.newAppointmentDate}>
            <input
              type="date"
              className={`form-input ${errors.newAppointmentDate ? 'has-error' : ''}`}
              value={form.newAppointmentDate}
              onChange={(e) => setForm({ ...form, newAppointmentDate: e.target.value, newTimeSlot: '' })}
              min={minDateStr}
              max={maxDateStr}
            />
          </FormField>

          <FormField label="New time slot" error={errors.newTimeSlot}>
            <select
              className={`form-input ${errors.newTimeSlot ? 'has-error' : ''}`}
              value={form.newTimeSlot}
              onChange={(e) => setForm({ ...form, newTimeSlot: e.target.value })}
              disabled={slotsLoading || !form.newAppointmentDate}
            >
              <option value="">
                {slotsLoading ? 'Loading slots…' : !form.newAppointmentDate ? 'Pick a date first' : 'Select a time slot'}
              </option>
              {availableSlots.map((slot) => (
                <option key={slot} value={slot}>{slot}</option>
              ))}
            </select>
            {!slotsLoading && form.newAppointmentDate && availableSlots.length === 0 && (
              <p style={{ fontSize: '0.8rem', color: 'var(--color-text-muted)', marginTop: '0.25rem' }}>
                No slots available for this date.
              </p>
            )}
          </FormField>

          <div className="modal-actions">
            <button type="button" className="btn-sm btn-sm--outline" onClick={onClose}>
              Keep original
            </button>
            <button type="submit" className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.5rem' }} disabled={submitting}>
              {submitting ? 'Saving…' : 'Confirm reschedule'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}