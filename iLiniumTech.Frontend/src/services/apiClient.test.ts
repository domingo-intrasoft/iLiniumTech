import { afterEach, describe, expect, it, vi } from 'vitest'

describe('apiClient headers', () => {
  afterEach(() => {
    vi.unstubAllEnvs()
    vi.resetModules()
  })

  it('sends configured development broker id to the backend', async () => {
    vi.stubEnv('VITE_BROKER_ID', 'dev-broker-1')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-Broker-Id']).toBe('dev-broker-1')
  })

  it('keeps broker header unset when no broker id is configured', async () => {
    vi.stubEnv('VITE_BROKER_ID', '')

    const { apiClient } = await import('./apiClient')

    expect(apiClient.defaults.headers['X-Broker-Id']).toBeUndefined()
  })
})
