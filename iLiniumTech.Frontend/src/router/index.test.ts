import { createMemoryHistory } from 'vue-router'
import { beforeEach, describe, expect, it } from 'vitest'

import { clearAuthSession, loginDemo } from '@/features/auth/authSession'

import { createIliniumRouter, routes } from './index'

describe('router auth guard', () => {
  beforeEach(() => {
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
