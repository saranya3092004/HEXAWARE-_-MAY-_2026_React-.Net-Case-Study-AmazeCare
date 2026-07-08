import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './AdminLayout.css';

const NAV_LINKS = [
  { to: '/admin/dashboard',    label: 'Dashboard',    icon: '📊' },
  { to: '/admin/doctors',      label: 'Doctors',      icon: '👨‍⚕️' },
  { to: '/admin/patients',     label: 'Patients',     icon: '🧑‍🤝‍🧑' },
  { to: '/admin/appointments', label: 'Appointments', icon: '📅' },
  { to: '/admin/reports',      label: 'Reports',      icon: '📋' },
];

export default function AdminLayout({ children }) {
  const { fullName, logout } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate('/login', { replace: true });
  }

  return (
    <div className="al-shell">
      <aside className="al-sidebar">
        <div className="al-brand">AmazeCare <span className="al-admin-tag">Admin</span></div>
        <div className="al-user">
          <span className="al-user-avatar">{fullName?.[0] ?? 'A'}</span>
          <div>
            <span className="al-user-name">{fullName}</span>
            <span className="al-user-role">Administrator</span>
          </div>
        </div>
        <nav className="al-nav">
          {NAV_LINKS.map(({ to, label, icon }) => (
            <NavLink
              key={to}
              to={to}
              className={({ isActive }) =>
                `al-nav-link ${isActive ? 'al-nav-link--active' : ''}`
              }
            >
              <span className="al-nav-icon">{icon}</span>
              {label}
            </NavLink>
          ))}
        </nav>
        <button className="al-logout" onClick={handleLogout}>Sign out</button>
      </aside>
      <main className="al-content">{children}</main>
    </div>
  );
}