import axios from 'axios'

const apiKey = import.meta.env.VITE_ILINIUMTECH_API_KEY
const brokerId = import.meta.env.VITE_BROKER_ID
const headers = {
  ...(apiKey ? { 'X-ILiniumTech-Api-Key': apiKey } : {}),
  ...(brokerId ? { 'X-Broker-Id': brokerId } : {}),
}

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5146',
  timeout: 10_000,
  headers: Object.keys(headers).length > 0 ? headers : undefined,
})
