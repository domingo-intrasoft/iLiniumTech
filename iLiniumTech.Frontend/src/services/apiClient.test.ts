import { afterEach, describe, expect, it, vi } from 'vitest'

describe('apiClient headers', () => {
  afterEach(() => {
    vi.unstubAllEnvs()
    vi.resetModules()
  })

  it('sends configured development broker id to the backend', async () => {
    vi.stubEnv('VITE_BROKER_ID', '42')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-Broker-Id']).toBe('42')
  })

  it('keeps broker header unset when no broker id is configured', async () => {
    vi.stubEnv('VITE_BROKER_ID', '')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-Broker-Id']).toBeUndefined()
  })

  it('keeps invalid broker ids out of request headers', async () => {
    vi.stubEnv('VITE_BROKER_ID', 'dev-broker-1')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-Broker-Id']).toBeUndefined()
  })

  it('does not send placeholder API keys', async () => {
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', '__SET_IN_ENVIRONMENT__')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-ILiniumTech-Api-Key']).toBeUndefined()
  })
})
