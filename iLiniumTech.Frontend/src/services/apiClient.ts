import axios from 'axios'

const apiKey = import.meta.env.VITE_ILINIUMTECH_API_KEY

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5146',
  timeout: 10_000,
  headers: apiKey
    ? {
        'X-ILiniumTech-Api-Key': apiKey,
      }
    : undefined,
})
