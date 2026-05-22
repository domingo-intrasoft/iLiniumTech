import axios from 'axios'

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5146',
  timeout: 10_000,
  headers: {
    'X-ILiniumTech-Api-Key': import.meta.env.VITE_API_KEY ?? 'dev-api-key-safe-123',
  },
})
