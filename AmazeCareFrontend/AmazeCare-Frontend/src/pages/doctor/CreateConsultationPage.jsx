import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { createConsultation } from '../../api/consultations';
import DoctorLayout from '../../layouts/DoctorLayout';
import FormField from '../../components/FormField';
import '../../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

export default function CreateConsultationPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const appointmentId = Number(searchParams.get('appointmentId'));

  const [form, setForm] = useState({
    currentSymptoms: '',
    physicalExamination: '',
    treatmentPlan: '',
    diagnosis: '',
  });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  function validate() {
    const next = {};
    if (!form.currentSymptoms.trim()) next.currentSymptoms = 'Current symptoms are required.';
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    if (!validate()) return;
    if (!appointmentId) {
      setServerError('No appointment selected.');
      return;
    }
    setSubmitting(true);
    try {
      await createConsultation({
        appointmentId,
        ...form,
      });
      navigate('/doctor/consultations');
    } catch (err) {
      if (isUserFacingError(err)) setServerError(err.message);
      else { console.error(err); setServerError(GENERIC_ERROR); }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <DoctorLayout>
      <div className="page-header">
        <h1 className="page-title">Record consultation</h1>
        <p className="page-subtitle">Capture the consultation details for this appointment.</p>
      </div>

      <div className="card" style={{ maxWidth: 560 }}>
        <form onSubmit={handleSubmit} noValidate>
          {serverError && <div className="form-banner-error">{serverError}</div>}

          <FormField label="Current symptoms *" error={errors.currentSymptoms}>
            <textarea
              className={`form-input ${errors.currentSymptoms ? 'has-error' : ''}`}
              rows={3}
              value={form.currentSymptoms}
              onChange={(e) => update('currentSymptoms', e.target.value)}
              placeholder="Describe the patient's current symptoms and concerns…"
              style={{ resize: 'vertical' }}
            />
          </FormField>

          <FormField label="Physical examination" error={null}>
            <textarea
              className="form-input"
              rows={3}
              value={form.physicalExamination}
              onChange={(e) => update('physicalExamination', e.target.value)}
              placeholder="Vital signs, appearance, observable symptoms…"
              style={{ resize: 'vertical' }}
            />
          </FormField>

          <FormField label="Treatment plan" error={null}>
            <textarea
              className="form-input"
              rows={2}
              value={form.treatmentPlan}
              onChange={(e) => update('treatmentPlan', e.target.value)}
              placeholder="Recommended tests or next steps…"
              style={{ resize: 'vertical' }}
            />
          </FormField>

          <FormField label="Diagnosis" error={null}>
            <input
              type="text"
              className="form-input"
              value={form.diagnosis}
              onChange={(e) => update('diagnosis', e.target.value)}
              placeholder="e.g. Viral fever, Type 2 Diabetes…"
            />
          </FormField>

          <div style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.75rem' }}>
            <button
              type="button"
              className="btn-sm btn-sm--outline"
              style={{ padding: '0.65rem 1.25rem' }}
              onClick={() => navigate('/doctor/appointments')}
            >
              Cancel
            </button>
            <button
              type="submit"
              className="btn-primary"
              style={{ width: 'auto', padding: '0.65rem 1.5rem' }}
              disabled={submitting}
            >
              {submitting ? 'Saving…' : 'Save consultation'}
            </button>
          </div>
        </form>
      </div>
    </DoctorLayout>
  );
}