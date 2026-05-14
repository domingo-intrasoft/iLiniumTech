import axios, { AxiosHeaders } from 'axios'

import { getApiHeaders, getRuntimeConfig } from './runtimeConfig'

const runtimeConfig = getRuntimeConfig()
const headers = getApiHeaders(runtimeConfig)

export const apiClient = axios.create({
  baseURL: runtimeConfig.apiBaseUrl,
  timeout: 10_000,
  headers: Object.keys(headers).length > 0 ? headers : undefined,
})

function createCorrelationId() {
  if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
    return `iliniumtech-frontend-${crypto.randomUUID()}`
  }

  return `iliniumtech-frontend-${Date.now()}-${Math.random().toString(36).slice(2)}`
}

apiClient.interceptors.request.use((config) => {
  const requestHeaders = AxiosHeaders.from(config.headers)

  if (!requestHeaders.has('X-Correlation-Id')) {
    requestHeaders.set('X-Correlation-Id', createCorrelationId())
  }

  config.headers = requestHeaders
  return config
})
