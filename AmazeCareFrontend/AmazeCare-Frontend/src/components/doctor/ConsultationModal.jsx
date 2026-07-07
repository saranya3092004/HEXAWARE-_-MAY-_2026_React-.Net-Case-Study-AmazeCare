import { useState } from 'react';
import { updateConsultation } from '../../api/consultations';
import FormField from '../FormField';
import '../../components/forms.css';
import '../patient/Modal.css';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

export default function ConsultationModal({ consultation, mode, onClose, onSuccess }) {
  const [form, setForm] = useState({
    currentSymptoms: consultation.currentSymptoms ?? '',
    physicalExamination: consultation.physicalExamination ?? '',
    treatmentPlan: consultation.treatmentPlan ?? '',
    diagnosis: consultation.diagnosis ?? '',
  });
  const [serverError, setServerError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!form.currentSymptoms.trim()) {
      setServerError('Current symptoms are required.');
      return;
    }
    setServerError('');
    setSubmitting(true);
    try {
      await updateConsultation(consultation.consultationId, form);
      onSuccess();
    } catch (err) {
      if (isUserFacingError(err)) setServerError(err.message);
      else { console.error(err); setServerError(GENERIC_ERROR); }
    } finally {
      setSubmitting(false);
    }
  }

  const isView = mode === 'view';

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-box" style={{ maxWidth: 520 }} onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2 className="modal-title">
            {isView ? 'Consultation details' : 'Edit consultation'}
          </h2>
          <button className="modal-close" onClick={onClose} aria-label="Close">✕</button>
        </div>
        <p className="modal-subtitle">
          Patient: <strong>{consultation.patientName}</strong> ·{' '}
          {new Date(consultation.consultationDate).toLocaleDateString('en-IN')}
        </p>

        {isView ? (
          <div className="consultation-view">
            <div className="cv-field">
              <span className="cv-label">Current symptoms</span>
              <span className="cv-value">{consultation.currentSymptoms || '—'}</span>
            </div>
            <div className="cv-field">
              <span className="cv-label">Physical examination</span>
              <span className="cv-value">{consultation.physicalExamination || '—'}</span>
            </div>
            <div className="cv-field">
              <span className="cv-label">Treatment plan</span>
              <span className="cv-value">{consultation.treatmentPlan || '—'}</span>
            </div>
            <div className="cv-field">
              <span className="cv-label">Diagnosis</span>
              <span className="cv-value">{consultation.diagnosis || '—'}</span>
            </div>
            <div className="modal-actions">
              <button className="btn-primary" style={{ width: 'auto', padding: '0.65rem 1.5rem' }} onClick={onClose}>
                Close
              </button>
            </div>
          </div>
        ) : (
          <form onSubmit={handleSubmit} noValidate>
            {serverError && <div className="form-banner-error">{serverError}</div>}

            <FormField label="Current symptoms" error={null}>
              <textarea
                className="form-input"
                rows={3}
                value={form.currentSymptoms}
                onChange={(e) => update('currentSymptoms', e.target.value)}
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
                placeholder="e.g. Viral fever, Hypertension…"
              />
            </FormField>

            <div className="modal-actions">
              <button type="button" className="btn-sm btn-sm--outline" onClick={onClose}>
                Cancel
              </button>
              <button
                type="submit"
                className="btn-primary"
                style={{ width: 'auto', padding: '0.65rem 1.5rem' }}
                disabled={submitting}
              >
                {submitting ? 'Saving…' : 'Save changes'}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}