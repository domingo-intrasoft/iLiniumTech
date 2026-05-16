import { createMemoryHistory } from 'vue-router'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { clearAuthSession, loginDemo } from '@/features/auth/authSession'
import { apiClient } from '@/services/apiClient'

import { createIliniumRouter, routes } from './index'

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    get: vi.fn(),
  },
}))

describe('router auth guard', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_AUTH_MODE', 'api-key')
    vi.mocked(apiClient.get).mockReset()
    clearAuthSession()
  })

  it('redirects protected product routes to login when there is no MVP session', async () => {
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/polizas')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('login')
    expect(router.currentRoute.value.query.redirect).toBe('/polizas')
  })

  it('allows protected routes when a MVP session exists', async () => {
    loginDemo({ username: 'demo', password: 'demo' })
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/clientes')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('clientes')
  })

  it('redirects protected routes when the stored MVP session has expired', async () => {
    const session = loginDemo({ username: 'demo', password: 'demo' })
    const storageKey = window.sessionStorage.key(0)
    window.sessionStorage.setItem(
      storageKey!,
      JSON.stringify({ ...session, expiresAt: '2000-01-01T00:00:00.000Z' }),
    )
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/polizas')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('login')
    expect(router.currentRoute.value.query.redirect).toBe('/polizas')
  })

  it('redirects authenticated users away from login', async () => {
    loginDemo({ username: 'demo', password: 'demo' })
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/login')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('polizas')
  })

  it('redirects backend demo sessions to login when /api/me reports expiration', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'demo', password: 'demo' })
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
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/polizas')
    await router.isReady()

    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(router.currentRoute.value.name).toBe('login')
    expect(router.currentRoute.value.query.redirect).toBe('/polizas')
  })

  it('redirects protected backend demo routes to login when /api/me is unavailable despite stored local session', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'demo', password: 'demo' })
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('backend unavailable'))
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/clientes')
    await router.isReady()

    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(router.currentRoute.value.name).toBe('login')
    expect(router.currentRoute.value.query.redirect).toBe('/clientes')
  })

  it('allows backend demo sessions only after /api/me validates the cookie session', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'demo', password: 'demo' })
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
      },
    })
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/polizas')
    await router.isReady()

    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(router.currentRoute.value.name).toBe('polizas')
  })

  it('keeps login visible when backend demo confirmation is unavailable', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    loginDemo({ username: 'demo', password: 'demo' })
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('backend unavailable'))
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/login')
    await router.isReady()

    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
    expect(router.currentRoute.value.name).toBe('login')
  })

  it('registers MVP routes for the menu pages without exposing metadata routes', () => {
    const routePaths = routes.map((route) => route.path)

    expect(routePaths).toEqual(
      expect.arrayContaining([
        '/agenda',
        '/clientes',
        '/propuestas',
        '/recibos',
        '/suplementos',
        '/siniestros',
        '/liq-cia',
        '/liq-col',
        '/informes',
        '/controles',
        '/estadisticas',
        '/administracion',
        '/configuracion',
        '/conectividad',
        '/by-aunna',
        '/logs',
        '/polizas/flotas',
        '/polizas/colectivas',
      ]),
    )
    expect(JSON.stringify(routes)).not.toMatch(/IAP_|QueryStatic|ComponentDataSource/)
  })
})
