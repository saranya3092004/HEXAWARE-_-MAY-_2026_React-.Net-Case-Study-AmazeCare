import './AuthLayout.css';

function PulseLine() {
  return (
    <svg
      className="pulse-line"
      viewBox="0 0 400 60"
      fill="none"
      aria-hidden="true"
    >
      <path
        d="M0 30 H140 L160 10 L180 50 L200 30 H400"
        stroke="currentColor"
        strokeWidth="2"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
    </svg>
  );
}

export default function AuthLayout({ eyebrow, title, subtitle, children }) {
  return (
    <div className="auth-shell">
      <aside className="auth-brand">
        <div className="auth-brand-content">
          <span className="auth-brand-mark">AmazeCare</span>
          <PulseLine />
          <p className="auth-brand-tagline">
            Appointments, records, and care — in one place.
          </p>
        </div>
      </aside>

      <main className="auth-panel">
        <div className="auth-card">
          {eyebrow && <span className="auth-eyebrow">{eyebrow}</span>}
          <h1 className="auth-title">{title}</h1>
          {subtitle && <p className="auth-subtitle">{subtitle}</p>}
          {children}
        </div>
      </main>
    </div>
  );
}