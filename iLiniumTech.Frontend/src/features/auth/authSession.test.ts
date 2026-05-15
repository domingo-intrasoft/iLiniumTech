import { beforeEach, describe, expect, it, vi } from 'vitest'

import { clearAuthSession, hasAuthSession, loginDemo, readStoredSession } from './authSession'

describe('authSession MVP', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_BROKER_ID', '42')
    clearAuthSession()
  })

  it('creates a sanitized demo session in sessionStorage', () => {
    const session = loginDemo({ username: 'demo@iliniumtech.local', password: 'demo' })

    expect(session.mode).toBe('demo')
    expect(session.user.displayName).toBe('demo')
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
})
