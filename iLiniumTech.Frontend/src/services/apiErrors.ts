import axios from 'axios'

import { RuntimeConfigError } from './runtimeConfig'

interface BackendErrorResponse {
  error?: {
    code?: string
    message?: string
    correlationId?: string | null
  }
}

const SAFE_BACKEND_MESSAGES = new Map<string, string>([
  ['POLIZAS_CONTEXT_REQUIRED', 'Configura un broker antes de consultar polizas.'],
  ['POLIZAS_CONTEXT_INVALID', 'El contexto de broker no es valido.'],
  ['POLIZAS_VALIDATION_ERROR', 'La busqueda contiene filtros no validos.'],
  ['POLIZAS_CONFIGURATION_ERROR', 'La API de polizas no esta configurada para atender consultas.'],
  ['POLIZAS_UNEXPECTED_ERROR', 'La API de polizas devolvio un error inesperado.'],
])

function safeCorrelationId(value: string | null | undefined) {
  if (!value || !/^[a-zA-Z0-9._:-]{1,100}$/.test(value)) {
    return null
  }

  return value
}

function withCorrelationId(message: string, correlationId: string | null | undefined) {
  const safeId = safeCorrelationId(correlationId)
  return safeId ? `${message} Ref: ${safeId}.` : message
}

export function toPolizasUserMessage(error: unknown, fallback: string) {
  if (error instanceof RuntimeConfigError) {
    return error.message
  }

  if (!axios.isAxiosError<BackendErrorResponse>(error)) {
    return fallback
  }

  const status = error.response?.status
  const backendError = error.response?.data?.error
  const backendCode = backendError?.code

  if (backendCode && SAFE_BACKEND_MESSAGES.has(backendCode)) {
    return withCorrelationId(SAFE_BACKEND_MESSAGES.get(backendCode)!, backendError?.correlationId)
  }

  if (status === 401) {
    return 'No se pudo autenticar con la API de polizas. Revisa la API key del entorno.'
  }

  if (status === 403) {
    return 'La sesion actual no tiene permiso para consultar polizas.'
  }

  if (status === 404) {
    return 'No se encontro la poliza solicitada.'
  }

  if (status === 400) {
    return withCorrelationId('La API rechazo la consulta de polizas.', backendError?.correlationId)
  }

  if (status && status >= 500) {
    return withCorrelationId(
      'La API de polizas no esta disponible temporalmente.',
      backendError?.correlationId,
    )
  }

  if (error.code === 'ECONNABORTED') {
    return 'La API de polizas tardo demasiado en responder.'
  }

  return fallback
}
