import apiClient from './client';

export const getDashboard = () =>
  apiClient.get('/admin/dashboard');

export const getAppointmentReport = (params) =>
  apiClient.get('/admin/reports/appointments', { params });

export const getPatientReport = (params) =>
  apiClient.get('/admin/reports/patients', { params });