import { beforeEach, describe, expect, it, vi } from 'vitest'

import { apiClient } from '@/services/apiClient'

import {
  clearAuthSession,
  hasAuthSession,
  loginAuthSession,
  loginDemo,
  readStoredSession,
} from './authSession'

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    post: vi.fn(),
  },
}))

describe('authSession MVP', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_AUTH_MODE', 'api-key')
    vi.stubEnv('VITE_BROKER_ID', '42')
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

  it('creates a backend demo session when cookie auth mode is configured', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    vi.mocked(apiClient.post).mockResolvedValueOnce({
      data: {
        session: {
          mode: 'demo',
          expiresAt: '2026-05-15T12:00:00Z',
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
    expect(session.expiresAt).toBe('2026-05-15T12:00:00Z')
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
})
