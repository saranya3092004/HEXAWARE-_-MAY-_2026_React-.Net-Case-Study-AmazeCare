import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './DoctorLayout.css';

const NAV_LINKS = [
  { to: '/doctor/appointments', label: 'Appointments', icon: '📅' },
  { to: '/doctor/consultations', label: 'Consultations', icon: '🩺' },
];

export default function DoctorLayout({ children }) {
  const { fullName, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="dl-shell">
      <aside className="dl-sidebar">
        <div className="dl-brand">AmazeCare</div>
        <div className="dl-user">
          <span className="dl-user-avatar">{fullName?.[0] ?? 'D'}</span>
          <div>
            <span className="dl-user-name">{fullName}</span>
            <span className="dl-user-role">Doctor</span>
          </div>
        </div>
        <nav className="dl-nav">
          {NAV_LINKS.map(({ to, label, icon }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `dl-nav-link ${isActive ? 'dl-nav-link--active' : ''}`
              }
            >
              <span className="dl-nav-icon">{icon}</span>
              {label}
            </NavLink>
          ))}
        </nav>
        <button className="dl-logout" onClick={handleLogout}>
          Sign out
        </button>
      </aside>
      <main className="dl-content">{children}</main>
    </div>
  );
}