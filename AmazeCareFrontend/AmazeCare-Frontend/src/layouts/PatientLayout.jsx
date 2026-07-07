import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './PatientLayout.css';

const NAV_LINKS = [
  { to: '/patient/appointments', label: 'Appointments', icon: '📅' },
  { to: '/patient/book',         label: 'Book Appointment', icon: '➕' },
  { to: '/patient/history',      label: 'Medical History', icon: '📋' },
  { to: '/patient/profile',      label: 'My Profile', icon: '👤' },
];

export default function PatientLayout({ children }) {
  const { fullName, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="pl-shell">
      <aside className="pl-sidebar">
        <div className="pl-brand">AmazeCare</div>
        <div className="pl-user">
          <span className="pl-user-avatar">{fullName?.[0] ?? 'P'}</span>
          <span className="pl-user-name">{fullName}</span>
        </div>
        <nav className="pl-nav">
          {NAV_LINKS.map(({ to, label, icon }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `pl-nav-link ${isActive ? 'pl-nav-link--active' : ''}`
              }
            >
              <span className="pl-nav-icon">{icon}</span>
              {label}
            </NavLink>
          ))}
        </nav>
        <button className="pl-logout" onClick={handleLogout}>
          Sign out
        </button>
      </aside>
      <main className="pl-content">{children}</main>
    </div>
  );
}