import apiClient from './client';

export const getAppointments = (params) =>
  apiClient.get('/appointments', { params });

export const getAppointmentById = (id) =>
  apiClient.get(`/appointments/${id}`);

export const bookAppointment = (payload) =>
  apiClient.post('/appointments', payload);

export const cancelAppointment = (id, payload) =>
  apiClient.put(`/appointments/${id}/cancel`, payload);

export const rescheduleAppointment = (id, payload) =>
  apiClient.put(`/appointments/${id}/reschedule`, payload);

export function getAvailableSlots(doctorId, date) {
  return apiClient.get('/appointments/available-slots', { params: { doctorId, date } });
}

export const confirmAppointment = (id) =>
  apiClient.put(`/appointments/${id}/confirm`);

export const rejectAppointment = (id, payload) =>
  apiClient.put(`/appointments/${id}/reject`, payload);

export const completeAppointment = (id) =>
  apiClient.put(`/appointments/${id}/complete`);