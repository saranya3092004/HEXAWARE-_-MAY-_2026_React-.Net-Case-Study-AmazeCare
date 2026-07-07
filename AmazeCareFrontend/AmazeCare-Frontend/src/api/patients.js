import apiClient from './client';

export const getPatientById = (id) =>
  apiClient.get(`/patients/${id}`);

export const updatePatient = (id, payload) =>
  apiClient.put(`/patients/${id}`, payload);

export const getPatientHistory = (id) =>
  apiClient.get(`/patients/${id}/history`);