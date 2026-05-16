import { beforeEach, describe, expect, it, vi } from 'vitest'

import { apiClient } from '@/services/apiClient'

import {
  clearAuthSession,
  hasAuthSession,
  isAuthSessionExpired,
  loginAuthSession,
  loginDemo,
  logoutAuthSession,
  readStoredSession,
  switchAuthBroker,
  validateAuthSession,
} from './authSession'

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
  },
}))

describe('authSession MVP', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_AUTH_MODE', 'api-key')
    vi.stubEnv('VITE_BROKER_ID', '42')
    vi.mocked(apiClient.get).mockReset()
    vi.mocked(apiClient.post).mockReset()
    clearAuthSession()
  })

  it('creates a sanitized demo session in sessionStorage', () => {
    const session = loginDemo({ username: 'demo@iliniumtech.local', password: 'demo' })

    expect(session.mode).toBe('demo')
    expect(session.source).toBe('local')
    expect(session.user.displayName).toBe('demo')
    expect(session.user.id).toBe('demo:demo')
    expect(session.currentBrokerId).toBe(42)
    expect(session.permissions).toContain('polizas.read')
    expect(hasAuthSession()).toBe(true)
    expect(readStoredSession()?.sessionId).toBe(session.sessionId)
  })

  it('rejects empty credentials', () => {
    expect(() => loginDemo({ username: '', password: '' })).toThrow(
      'Introduce usuario y contrasena.',
    )
    expect(hasAuthSession()).toBe(false)
  })

  it('clears the stored session', () => {
    loginDemo({ username: 'demo', password: 'demo' })
    clearAuthSession()

    expect(readStoredSession()).toBeNull()
    expect(hasAuthSession()).toBe(false)
  })

  it('discards expired stored sessions before route guards trust them', () => {
    const session = loginDemo({ username: 'demo', password: 'demo' })
    const storageKey = window.sessionStorage.key(0)

    expect(storageKey).toBeTruthy()
    expect(isAuthSessionExpired(session)).toBe(false)

    window.sessionStorage.setItem(
      storageKey!,
      JSON.stringify({ ...session, expiresAt: '2000-01-01T00:00:00.000Z' }),
    )

    expect(readStoredSession()).toBeNull()
    expect(window.sessionStorage.getItem(storageKey!)).toBeNull()
    expect(hasAuthSession()).toBe(false)
  })

  it('creates a backend demo session when cookie auth mode is configured', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    vi.mocked(apiClient.post).mockResolvedValueOnce({
      data: {
        session: {
          mode: 'demo',
          expiresAt: '2099-05-15T12:00:00Z',
        },
        user: {
          id: 'demo:domingo',
          displayName: 'domingo',
        },
        application: {
          key: 'iliniumtech',
          name: 'iLiniumTech',
        },
        currentBrokerId: 42,
        permissions: ['polizas.read'],
      },
    })

    const session = await loginAuthSession({
      username: 'domingo@iliniumtech.local',
      password: 'demo',
    })

    expect(apiClient.post).toHaveBeenCalledWith('/api/auth/login', {
      username: 'domingo@iliniumtech.local',
      password: 'demo',
      brokerId: 42,
    })
    expect(session.source).toBe('backend')
    expect(session.user.displayName).toBe('domingo')
    expect(session.expiresAt).toBe('2099-05-15T12:00:00Z')
    expect(readStoredSession()?.source).toBe('backend')
  })

  it('shows a safe message when backend demo login rejects credentials', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    vi.mocked(apiClient.post).mockRejectedValueOnce({
      isAxiosError: true,
      response: {
        status: 401,
        data: {
          error: {
            code: 'AUTH_INVALID_CREDENTIALS',
            message: 'Invalid credentials.',
          },
        },
      },
    })

    await expect(
      loginAuthSession({
        username: 'domingo',
        password: 'wrong',
      }),
    ).rejects.toThrow('Credenciales no validas.')
    expect(hasAuthSession()).toBe(false)
  })

  it('clears the local session even when backend logout cannot complete', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.post).mockRejectedValueOnce(new Error('backend unavailable'))

    await expect(logoutAuthSession()).resolves.toBeUndefined()

    expect(apiClient.post).toHaveBeenCalledWith('/api/auth/logout')
    expect(hasAuthSession()).toBe(false)
  })

  it('validates backend demo sessions against /api/me before trusting storage', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.get).mockResolvedValueOnce({
      data: {
        brokerId: 42,
        entityMainId: 42,
        userId: 7,
        profileId: 11,
        profileTypeId: 'admin',
        isAdmin: false,
        headerExecutionContextEnabled: false,
        polizasExecutionContextRequired: true,
        user: {
          id: 'demo:backend-user',
          displayName: 'backend-user',
        },
        application: {
          key: 'iliniumtech',
          name: 'iLiniumTech',
        },
        permissions: ['polizas.catalogs', 'polizas.read'],
      },
    })

    await expect(validateAuthSession()).resolves.toBe(true)

    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(readStoredSession()).toMatchObject({
      source: 'backend',
      user: {
        displayName: 'backend-user',
      },
      currentBrokerId: 42,
      permissions: ['polizas.catalogs', 'polizas.read'],
    })
    expect(hasAuthSession()).toBe(true)
  })

  it('clears stored backend sessions when /api/me returns 401', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.get).mockRejectedValueOnce({
      isAxiosError: true,
      response: {
        status: 401,
        data: {
          error: {
            code: 'AUTH_SESSION_EXPIRED',
            correlationId: 'auth-401',
          },
        },
      },
    })

    await expect(validateAuthSession()).resolves.toBe(false)

    expect(readStoredSession()).toBeNull()
    expect(hasAuthSession()).toBe(false)
  })

  it('does not treat unconfirmed backend sessions as valid when confirmation is required', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('backend unavailable'))

    await expect(validateAuthSession({ requireBackendConfirmation: true })).resolves.toBe(false)

    expect(readStoredSession()).not.toBeNull()
  })

  it('updates stored backend session after a validated broker switch', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.post).mockResolvedValueOnce({ data: {} })
    vi.mocked(apiClient.get).mockResolvedValueOnce({
      data: {
        brokerId: 84,
        entityMainId: 84,
        userId: 7,
        profileId: 11,
        profileTypeId: 'admin',
        isAdmin: false,
        headerExecutionContextEnabled: false,
        polizasExecutionContextRequired: true,
        user: {
          id: 'demo:backend-user',
          displayName: 'backend-user',
        },
        application: {
          key: 'iliniumtech',
          name: 'iLiniumTech',
        },
        allowedBrokerIds: [42, 84],
        permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
        authMode: 'DemoSession',
      },
    })

    await expect(switchAuthBroker(84)).resolves.toMatchObject({
      brokerId: 84,
      allowedBrokerIds: [42, 84],
    })

    expect(apiClient.post).toHaveBeenCalledWith('/api/auth/broker', { brokerId: 84 })
    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(readStoredSession()).toMatchObject({
      source: 'backend',
      currentBrokerId: 84,
      permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
    })
  })

  it('clears stored backend sessions when broker switch returns 401', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.post).mockRejectedValueOnce({
      isAxiosError: true,
      response: {
        status: 401,
        data: {
          error: {
            code: 'AUTH_SESSION_EXPIRED',
            correlationId: 'broker-401',
          },
        },
      },
    })

    await expect(switchAuthBroker(84)).rejects.toMatchObject({
      response: {
        status: 401,
      },
    })

    expect(readStoredSession()).toBeNull()
    expect(hasAuthSession()).toBe(false)
  })

  it('clears stored backend sessions when broker switch cannot confirm /api/me', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'domingo', password: 'demo' })
    vi.mocked(apiClient.post).mockResolvedValueOnce({ data: {} })
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('backend unavailable'))

    await expect(switchAuthBroker(84)).rejects.toThrow(
      'No se pudo confirmar el broker activo tras cambiarlo.',
    )

    expect(readStoredSession()).toBeNull()
    expect(hasAuthSession()).toBe(false)
  })
})
