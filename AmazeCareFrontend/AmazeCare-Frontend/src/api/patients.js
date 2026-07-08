import apiClient from './client';

export const searchPatients = (params) =>
  apiClient.get('/patients', { params });

export const getPatientById = (id) =>
  apiClient.get(`/patients/${id}`);

export const getPatientHistory = (id) =>
  apiClient.get(`/patients/${id}/history`);

export const registerWalkInPatient = (payload) =>
  apiClient.post('/patients', payload);

export const updatePatient = (id, payload) =>
  apiClient.put(`/patients/${id}`, payload);

export const deactivatePatient = (id) =>
  apiClient.delete(`/patients/${id}`);