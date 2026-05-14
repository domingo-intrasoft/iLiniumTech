import { afterEach, describe, expect, it, vi } from 'vitest'

describe('runtime config contract', () => {
  afterEach(() => {
    vi.unstubAllEnvs()
    vi.resetModules()
  })

  it('allows the static fixture mode without backend secrets', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', '')

    const { getBlockingRuntimeConfigMessage, getRuntimeConfig } = await import('./runtimeConfig')
    const config = getRuntimeConfig()

    expect(config.backendEnabled).toBe(false)
    expect(config.issues).toEqual([])
    expect(getBlockingRuntimeConfigMessage(config)).toBeNull()
  })

  it('blocks backend mode when the API key is missing or a placeholder', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', '__SET_IN_ENVIRONMENT__')

    const { getApiHeaders, getBlockingRuntimeConfigMessage, getRuntimeConfig } =
      await import('./runtimeConfig')
    const config = getRuntimeConfig()

    expect(config.issues.map((issue) => issue.code)).toContain('ILINIUMTECH_API_KEY_MISSING')
    expect(getBlockingRuntimeConfigMessage(config)).toContain('VITE_ILINIUMTECH_API_KEY')
    expect(getApiHeaders(config)['X-ILiniumTech-Api-Key']).toBeUndefined()
  })

  it('keeps backend broker context compatible with the API integer contract', async () => {
    vi.stubEnv('VITE_BROKER_ID', 'abc')

    const { getApiHeaders, getRuntimeConfig } = await import('./runtimeConfig')
    const config = getRuntimeConfig()

    expect(config.issues.map((issue) => issue.code)).toContain('ILINIUMTECH_BROKER_ID_INVALID')
    expect(getApiHeaders(config)['X-Broker-Id']).toBeUndefined()
  })
})
