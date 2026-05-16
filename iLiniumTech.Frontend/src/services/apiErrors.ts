import axios from 'axios'

import { RuntimeConfigError } from './runtimeConfig'

interface BackendErrorResponse {
  error?: {
    code?: string
    message?: string
    correlationId?: string | null
  }
}

export type PolizasUserErrorKind =
  | 'runtime'
  | 'unauthenticated'
  | 'forbidden'
  | 'notFound'
  | 'validation'
  | 'configuration'
  | 'server'
  | 'timeout'
  | 'unknown'

export interface PolizasUserError {
  kind: PolizasUserErrorKind
  message: string
  status?: number
}

function unwrapKnownUserErrorCause(error: unknown) {
  if (error instanceof Error && error.name === 'BrokerSwitchVerificationError') {
    return (error as Error & { cause?: unknown }).cause ?? error
  }

  return error
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

export function isPolizasAccessError(error: PolizasUserError) {
  return error.kind === 'unauthenticated' || error.kind === 'forbidden'
}

export function toPolizasUserError(error: unknown, fallback: string): PolizasUserError {
  const effectiveError = unwrapKnownUserErrorCause(error)

  if (effectiveError instanceof RuntimeConfigError) {
    return { kind: 'runtime', message: effectiveError.message }
  }

  if (!axios.isAxiosError<BackendErrorResponse>(effectiveError)) {
    return { kind: 'unknown', message: fallback }
  }

  const status = effectiveError.response?.status
  const backendError = effectiveError.response?.data?.error
  const backendCode = backendError?.code

  if (backendCode && SAFE_BACKEND_MESSAGES.has(backendCode)) {
    const kind: PolizasUserErrorKind =
      backendCode === 'POLIZAS_VALIDATION_ERROR'
        ? 'validation'
        : backendCode === 'POLIZAS_CONFIGURATION_ERROR'
          ? 'configuration'
          : 'unknown'

    return {
      kind,
      status,
      message: withCorrelationId(
        SAFE_BACKEND_MESSAGES.get(backendCode)!,
        backendError?.correlationId,
      ),
    }
  }

  if (status === 401) {
    return {
      kind: 'unauthenticated',
      status,
      message: withCorrelationId(
        'La sesion no esta autorizada para consultar polizas. Inicia sesion de nuevo si el problema continua.',
        backendError?.correlationId,
      ),
    }
  }

  if (status === 403) {
    return {
      kind: 'forbidden',
      status,
      message: withCorrelationId(
        'La sesion actual no tiene permiso para consultar polizas.',
        backendError?.correlationId,
      ),
    }
  }

  if (status === 404) {
    return { kind: 'notFound', status, message: 'No se encontro la poliza solicitada.' }
  }

  if (status === 400) {
    return {
      kind: 'validation',
      status,
      message: withCorrelationId(
        'La API rechazo la consulta de polizas.',
        backendError?.correlationId,
      ),
    }
  }

  if (status && status >= 500) {
    return {
      kind: 'server',
      status,
      message: withCorrelationId(
        'La API de polizas no esta disponible temporalmente.',
        backendError?.correlationId,
      ),
    }
  }

  if (effectiveError.code === 'ECONNABORTED') {
    return { kind: 'timeout', message: 'La API de polizas tardo demasiado en responder.' }
  }

  return { kind: 'unknown', status, message: fallback }
}

export function toPolizasUserMessage(error: unknown, fallback: string) {
  return toPolizasUserError(error, fallback).message
}
