import { useState } from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import AuthLayout from '../layouts/AuthLayout';
import FormField from '../components/FormField';
import { loginUser } from '../api/auth';
import { useAuth } from '../context/AuthContext';
import '../components/forms.css';
import { isUserFacingError, GENERIC_ERROR } from '../utils/errors';

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();

  const bookingIntent = location.state?.bookingIntent;
  const doctorName = location.state?.doctorName;
  const justRegistered = location.state?.registered === true;

  const [form, setForm] = useState({ email: '', password: '' });
  const [errors, setErrors] = useState({});
  const [serverError, setServerError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  function validate() {
    const next = {};
    if (!form.email.trim()) next.email = 'Enter your email address.';
    if (!form.password) next.password = 'Enter your password.';
    setErrors(next);
    return Object.keys(next).length === 0;
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setServerError('');
    if (!validate()) return;

    setIsSubmitting(true);
    try {
      const response = await loginUser(form);
      const { token, role, roleSpecificId, userId, fullName, expiry } = response.data;

      login({ token, role, roleSpecificId, userId, fullName, expiry });

      if (role === 'Doctor') navigate('/doctor/appointments');
      else if (role === 'Admin') navigate('/admin/dashboard');
      else navigate('/patient/appointments');
    } catch (err) {
      if (isUserFacingError(err)) {
        setServerError(err.message);
      } else {
        console.error('Login error:', err);
        setServerError(GENERIC_ERROR);
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout
      eyebrow="Welcome back"
      title="Sign in to AmazeCare"
      subtitle="Manage appointments, view your medical history, and connect with your doctors."
    >
      <form onSubmit={handleSubmit} noValidate>

        {/* Registration success banner */}
        {justRegistered && (
          <div className="form-banner-success">
            Account created — sign in to continue.
          </div>
        )}

        {/* Booking intent banner — shown when redirected from home page */}
        {bookingIntent && (
          <div className="form-banner-success">
            Sign in to book an appointment
            {doctorName ? ` with ${doctorName}` : ''}.
          </div>
        )}

        {/* Server / API error banner */}
        {serverError && (
          <div className="form-banner-error">{serverError}</div>
        )}

        <FormField label="Email" error={errors.email}>
          <input
            type="email"
            className={`form-input ${errors.email ? 'has-error' : ''}`}
            value={form.email}
            onChange={(e) => setForm({ ...form, email: e.target.value })}
            placeholder="you@example.com"
            autoComplete="email"
          />
        </FormField>

        <FormField label="Password" error={errors.password}>
          <input
            type="password"
            className={`form-input ${errors.password ? 'has-error' : ''}`}
            value={form.password}
            onChange={(e) => setForm({ ...form, password: e.target.value })}
            placeholder="••••••••"
            autoComplete="current-password"
          />
        </FormField>

        <button type="submit" className="btn-primary" disabled={isSubmitting}>
          {isSubmitting ? 'Signing in…' : 'Sign in'}
        </button>

        <p className="auth-switch">
          New to AmazeCare? <Link to="/register">Create an account</Link>
        </p>

      </form>
    </AuthLayout>
  );
}