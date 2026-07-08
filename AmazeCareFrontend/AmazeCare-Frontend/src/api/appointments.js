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

export const getAvailableSlots = (doctorId, date, excludeAppointmentId = null) => {
  const params = { doctorId, date };
  if (excludeAppointmentId) params.excludeAppointmentId = excludeAppointmentId;
  return apiClient.get('/appointments/slots', { params });
};

export const confirmAppointment = (id) =>
  apiClient.put(`/appointments/${id}/confirm`);

export const rejectAppointment = (id, payload) =>
  apiClient.put(`/appointments/${id}/reject`, payload);

export const completeAppointment = (id) =>
  apiClient.put(`/appointments/${id}/complete`);