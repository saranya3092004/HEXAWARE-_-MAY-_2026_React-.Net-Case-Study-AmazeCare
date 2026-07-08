import { useEffect, useState } from 'react';
import { getDashboard } from '../../api/admin';
import AdminLayout from '../../layouts/AdminLayout';
import { isUserFacingError, GENERIC_ERROR } from '../../utils/errors';

function StatCard({ label, value, icon }) {
  return (
    <div className="stat-card">
      <span className="stat-icon">{icon}</span>
      <span className="stat-value">{value}</span>
      <span className="stat-label">{label}</span>
    </div>
  );
}

export default function AdminDashboardPage() {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function load() {
      try {
        const { data } = await getDashboard();
        setStats(data.data);
      } catch (err) {
        if (isUserFacingError(err)) setError(err.message);
        else { console.error(err); setError(GENERIC_ERROR); }
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  if (loading) return <AdminLayout><div className="spinner">Loading dashboard…</div></AdminLayout>;

  return (
    <AdminLayout>
      <div className="page-header">
        <h1 className="page-title">Dashboard</h1>
        <p className="page-subtitle">Overview of AmazeCare today.</p>
      </div>

      {error && <div className="form-banner-error">{error}</div>}

      {stats && (
        <div className="stat-grid">
          <StatCard label="Total patients" value={stats.totalPatients} icon="🧑‍🤝‍🧑" />
          <StatCard label="Total doctors" value={stats.totalDoctors} icon="👨‍⚕️" />
          <StatCard label="Today's appointments" value={stats.todaysAppointments} icon="📅" />
          <StatCard label="Pending appointments" value={stats.pendingAppointments} icon="⏳" />
        </div>
      )}
    </AdminLayout>
  );
}