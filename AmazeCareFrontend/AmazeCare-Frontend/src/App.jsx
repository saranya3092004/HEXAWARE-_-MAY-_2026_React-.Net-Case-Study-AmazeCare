import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import PatientAppointmentsPage from './pages/patient/PatientAppointmentsPage';
import BookAppointmentPage from './pages/patient/BookAppointmentPage';
import PatientHistoryPage from './pages/patient/PatientHistoryPage';
import PatientProfilePage from './pages/patient/PatientProfilePage';
import DoctorAppointmentsPage from './pages/doctor/DoctorAppointmentsPage';
import DoctorConsultationsPage from './pages/doctor/DoctorConsultationsPage';
import CreateConsultationPage from './pages/doctor/CreateConsultationPage';

function RootRedirect() {
  const { isAuthenticated, role } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (role === 'Doctor') return <Navigate to="/doctor/appointments" replace />;
  if (role === 'Admin') return <Navigate to="/admin/dashboard" replace />;
  return <Navigate to="/patient/appointments" replace />;
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<RootRedirect />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Patient routes */}
        <Route
                   path="/patient/appointments"
          element={<ProtectedRoute allowedRoles={['User']}><PatientAppointmentsPage /></ProtectedRoute>}
        />
        <Route
          path="/patient/book"
          element={<ProtectedRoute allowedRoles={['User']}><BookAppointmentPage /></ProtectedRoute>}
        />
        <Route
          path="/patient/history"
          element={<ProtectedRoute allowedRoles={['User']}><PatientHistoryPage /></ProtectedRoute>}
        />
        <Route
          path="/patient/profile"
          element={<ProtectedRoute allowedRoles={['User']}><PatientProfilePage /></ProtectedRoute>}
        />

        {/* Doctor routes */}
        <Route
  path="/doctor/appointments"
  element={
    <ProtectedRoute allowedRoles={['Doctor']}>
      <DoctorAppointmentsPage />
    </ProtectedRoute>
  }
/>
<Route
  path="/doctor/consultations"
  element={
    <ProtectedRoute allowedRoles={['Doctor']}>
      <DoctorConsultationsPage />
    </ProtectedRoute>
  }
/>
<Route
  path="/doctor/consultations/new"
  element={
    <ProtectedRoute allowedRoles={['Doctor']}>
      <CreateConsultationPage />
    </ProtectedRoute>
  }
/>

        {/* Admin routes */}
        <Route
          path="/admin/*"
          element={
            <ProtectedRoute allowedRoles={['Admin']}>
              <div>Admin Dashboard — coming next</div>
            </ProtectedRoute>
          }
        />

        <Route path="/unauthorized" element={<div>You are not authorized to view this page.</div>} />
      </Routes>
    </BrowserRouter>
  );
}