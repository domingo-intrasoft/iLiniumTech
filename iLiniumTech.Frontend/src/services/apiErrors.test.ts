import { describe, expect, it } from 'vitest'

import { RuntimeConfigError } from './runtimeConfig'
import { toPolizasUserMessage } from './apiErrors'

function axiosError(status: number, data: unknown) {
  return {
    isAxiosError: true,
    response: {
      status,
      data,
    },
  }
}

describe('api error presenter', () => {
  it('maps backend error codes to safe product copy with correlation id', () => {
    const message = toPolizasUserMessage(
      axiosError(400, {
        error: {
          code: 'POLIZAS_CONTEXT_REQUIRED',
          message: 'Broker context is required for SQL polizas requests.',
          correlationId: 'req-123',
        },
      }),
      'fallback',
    )

    expect(message).toBe('Configura un broker antes de consultar polizas. Ref: req-123.')
  })

  it('does not echo unsafe backend messages', () => {
    const message = toPolizasUserMessage(
      axiosError(500, {
        error: {
          code: 'UNKNOWN',
          message: 'SELECT * FROM Pantalla_Polizas',
          correlationId: 'trace-1',
        },
      }),
      'fallback',
    )

    expect(message).toBe('La API de polizas no esta disponible temporalmente. Ref: trace-1.')
    expect(message).not.toContain('SELECT')
  })

  it('maps 401 responses to session copy without exposing backend detail', () => {
    const message = toPolizasUserMessage(
      axiosError(401, {
        error: {
          message: 'Missing or invalid API key',
          correlationId: 'auth-401',
        },
      }),
      'fallback',
    )

    expect(message).toBe(
      'La sesion no esta autorizada para consultar polizas. Inicia sesion de nuevo si el problema continua. Ref: auth-401.',
    )
    expect(message).not.toContain('API key')
  })

  it('maps 403 responses to access denied copy with safe correlation id', () => {
    const message = toPolizasUserMessage(
      axiosError(403, {
        error: {
          message: 'User lacks polizas.read',
          correlationId: 'forbidden-1',
        },
      }),
      'fallback',
    )

    expect(message).toBe(
      'La sesion actual no tiene permiso para consultar polizas. Ref: forbidden-1.',
    )
    expect(message).not.toContain('polizas.read')
  })

  it('surfaces frontend runtime contract errors directly', () => {
    const message = toPolizasUserMessage(
      new RuntimeConfigError('VITE_ILINIUMTECH_API_KEY debe configurarse.'),
      'fallback',
    )

    expect(message).toContain('VITE_ILINIUMTECH_API_KEY')
  })
})
