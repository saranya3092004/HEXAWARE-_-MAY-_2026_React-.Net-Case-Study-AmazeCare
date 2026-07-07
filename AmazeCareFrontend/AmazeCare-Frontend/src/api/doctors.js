import apiClient from './client';

export const searchDoctors = (params) =>
  apiClient.get('/doctors', { params });

export const getDoctorById = (id) =>
  apiClient.get(`/doctors/${id}`);

export const getDoctorAppointments = (id, params) =>
  apiClient.get(`/doctors/${id}/appointments`, { params });

export const createDoctor = (payload) =>
  apiClient.post('/doctors', payload);

export const updateDoctor = (id, payload) =>
  apiClient.put(`/doctors/${id}`, payload);

export const deactivateDoctor = (id) =>
  apiClient.delete(`/doctors/${id}`);