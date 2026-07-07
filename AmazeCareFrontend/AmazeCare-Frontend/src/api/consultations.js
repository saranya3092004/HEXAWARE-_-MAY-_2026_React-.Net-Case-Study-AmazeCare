import apiClient from './client';

export const getConsultations = () =>
  apiClient.get('/consultations');

export const getConsultationById = (id) =>
  apiClient.get(`/consultations/${id}`);

export const createConsultation = (payload) =>
  apiClient.post('/consultations', payload);

export const updateConsultation = (id, payload) =>
  apiClient.put(`/consultations/${id}`, payload);