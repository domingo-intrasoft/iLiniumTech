import { createMemoryHistory } from 'vue-router'
import { beforeEach, describe, expect, it } from 'vitest'

import { clearAuthSession, loginDemo } from '@/features/auth/authSession'

import { createIliniumRouter } from './index'

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

    await router.push('/polizas')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('polizas')
  })

  it('redirects authenticated users away from login', async () => {
    loginDemo({ username: 'demo', password: 'demo' })
    const router = createIliniumRouter(createMemoryHistory())

    await router.push('/login')
    await router.isReady()

    expect(router.currentRoute.value.name).toBe('polizas')
  })
})
