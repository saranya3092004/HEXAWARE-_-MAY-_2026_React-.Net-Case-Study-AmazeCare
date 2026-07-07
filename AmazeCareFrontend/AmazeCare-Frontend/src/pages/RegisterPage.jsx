import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthLayout from '../layouts/AuthLayout';
import FormField from '../components/FormField';
import { registerPatient } from '../api/auth';
import '../components/forms.css';
import { useAuth } from '../context/AuthContext';
import { isUserFacingError, GENERIC_ERROR } from '../utils/errors';


const initialForm = {
  fullName: '',
  email: '',
  phoneNumber: '',
  password: '',
  confirmPassword: '',
  dateOfBirth: '',
  gender: 0,
};

export default function RegisterPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { login } = useAuth();

  function update(field, value) {
    setForm((prev) => ({ ...prev, [field]: value }));
  }

  function validate() {
    const next = {};
    if (!form.fullName.trim()) next.fullName = 'Enter your full name.';
    if (!form.email.trim()) next.email = 'Enter your email address.';
    if (!form.phoneNumber.trim()) {
      next.phoneNumber = 'Enter your mobile number.';
    } else if (!/^[6-9]\d{9}$/.test(form.phoneNumber.trim())) {
      next.phoneNumber = 'Enter a valid 10-digit Indian mobile number.';
    }
    if (!form.dateOfBirth) next.dateOfBirth = 'Enter your date of birth.';
    if (!form.password) {
      next.password = 'Create a password.';
    } else if (form.password.length < 8) {
      next.password = 'Password must be at least 8 characters.';
    }
    if (form.confirmPassword !== form.password) {
      next.confirmPassword = 'Passwords do not match.';
    }
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    if (!validate()) return;

 setIsSubmitting(true);
try {
  const response = await registerPatient({
    fullName: form.fullName,
    email: form.email,
    phoneNumber: form.phoneNumber,
    password: form.password,
    dateOfBirth: form.dateOfBirth,
    gender: form.gender,
  });

login({
  token: response.token,
  role: response.role,
  roleSpecificId: response.roleSpecificId,
  userId: response.userId,
  fullName: response.fullName,
});  navigate('/patient/appointments');
} catch (err) {
  if (isUserFacingError(err)) {
    setServerError(err.message);
  } else {
    console.error('Register error:', err);
    setServerError(GENERIC_ERROR);
  }
} finally {
  setIsSubmitting(false);
}
  }

  return (
    <AuthLayout
      eyebrow="Get started"
      title="Create your account"
      subtitle="Book appointments and keep your medical history in one place."
    >
      <form onSubmit={handleSubmit} noValidate>
        {serverError && <div className="form-banner-error">{serverError}</div>}

        <FormField label="Full name" error={errors.fullName}>
          <input
            type="text"
            className={`form-input ${errors.fullName ? 'has-error' : ''}`}
            value={form.fullName}
            onChange={(e) => update('fullName', e.target.value)}
            placeholder="Arjun Kumar"
            autoComplete="name"
          />
        </FormField>

        <FormField label="Email" error={errors.email}>
          <input
            type="email"
            className={`form-input ${errors.email ? 'has-error' : ''}`}
            value={form.email}
            onChange={(e) => update('email', e.target.value)}
            placeholder="you@example.com"
            autoComplete="email"
          />
        </FormField>

        <FormField label="Mobile number" error={errors.phoneNumber}>
          <input
            type="tel"
            className={`form-input ${errors.phoneNumber ? 'has-error' : ''}`}
            value={form.phoneNumber}
            onChange={(e) => update('phoneNumber', e.target.value)}
            placeholder="9876543210"
            autoComplete="tel"
          />
        </FormField>

        <FormField label="Date of birth" error={errors.dateOfBirth}>
          <input
            type="date"
            className={`form-input ${errors.dateOfBirth ? 'has-error' : ''}`}
            value={form.dateOfBirth}
            onChange={(e) => update('dateOfBirth', e.target.value)}
          />
        </FormField>

        <FormField label="Gender">
          <select
            className="form-input"
            value={form.gender}
            onChange={(e) => update('gender',  Number(e.target.value))}
          >
            <option value={0}>Male</option>
            <option value={1}>Female</option>
            <option value={2}>Other</option>
            <option value={3}>Prefer Not To Say</option>

          </select>
        </FormField>

        <FormField label="Password" error={errors.password}>
          <input
            type="password"
            className={`form-input ${errors.password ? 'has-error' : ''}`}
            value={form.password}
            onChange={(e) => update('password', e.target.value)}
            placeholder="At least 8 characters"
            autoComplete="new-password"
          />
        </FormField>

        <FormField label="Confirm password" error={errors.confirmPassword}>
          <input
            type="password"
            className={`form-input ${errors.confirmPassword ? 'has-error' : ''}`}
            value={form.confirmPassword}
            onChange={(e) => update('confirmPassword', e.target.value)}
            placeholder="Re-enter your password"
            autoComplete="new-password"
          />
        </FormField>

        <button type="submit" className="btn-primary" disabled={isSubmitting}>
          {isSubmitting ? 'Creating account…' : 'Create account'}
        </button>

        <p className="auth-switch">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </form>
    </AuthLayout>
  );
}