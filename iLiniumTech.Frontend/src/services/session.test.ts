import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

vi.mock('./apiClient', () => ({
  apiClient: {
    get: vi.fn(),
  },
}))

function axiosError(status: number, data: unknown) {
  return {
    isAxiosError: true,
    response: {
      status,
      data,
    },
  }
}

describe('session service', () => {
  beforeEach(() => {
    vi.resetModules()
    vi.clearAllMocks()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
  })

  it('fetches the current backend session context from /api/me', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    const { apiClient } = await import('./apiClient')
    vi.mocked(apiClient.get).mockResolvedValueOnce({
      data: {
        brokerId: 42,
        entityMainId: 42,
        userId: 7,
        profileId: 11,
        profileTypeId: 'admin',
        isAdmin: true,
        headerExecutionContextEnabled: true,
        polizasExecutionContextRequired: true,
      },
    })

    const { getSessionContext } = await import('./session')

    const context = await getSessionContext()

    expect(context).toMatchObject({
      brokerId: 42,
      entityMainId: 42,
      userId: 7,
      profileId: 11,
      profileTypeId: 'admin',
      isAdmin: true,
      headerExecutionContextEnabled: true,
      polizasExecutionContextRequired: true,
      allowedBrokerIds: [],
    })
    expect(context.permissions).toBeUndefined()
    expect(apiClient.get).toHaveBeenCalledWith('/api/me')
  })

  it('exposes broker and context through a shared composable state', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    const { apiClient } = await import('./apiClient')
    vi.mocked(apiClient.get).mockResolvedValueOnce({
      data: {
        brokerId: 84,
        entityMainId: 84,
        userId: null,
        profileId: null,
        profileTypeId: null,
        isAdmin: null,
        headerExecutionContextEnabled: false,
        polizasExecutionContextRequired: true,
      },
    })

    const { useSession } = await import('./session')
    const firstConsumer = useSession()
    const secondConsumer = useSession()

    await firstConsumer.loadSession()

    expect(firstConsumer.brokerId.value).toBe(84)
    expect(firstConsumer.hasBrokerContext.value).toBe(true)
    expect(secondConsumer.session.value?.polizasExecutionContextRequired).toBe(true)
    expect(apiClient.get).toHaveBeenCalledTimes(1)
  })

  it('keeps empty context available when /api/me has no broker yet', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    const { apiClient } = await import('./apiClient')
    vi.mocked(apiClient.get).mockResolvedValueOnce({
      data: {
        brokerId: null,
        entityMainId: null,
        userId: null,
        profileId: null,
        profileTypeId: null,
        isAdmin: null,
        headerExecutionContextEnabled: false,
        polizasExecutionContextRequired: false,
      },
    })

    const { useSession } = await import('./session')
    const currentSession = useSession()

    await currentSession.loadSession()

    expect(currentSession.brokerId.value).toBeNull()
    expect(currentSession.hasBrokerContext.value).toBe(false)
    expect(currentSession.error.value).toBeNull()
  })

  it('uses local fixture context without calling /api/me outside backend mode', async () => {
    vi.stubEnv('VITE_BROKER_ID', '42')
    const { apiClient } = await import('./apiClient')
    const { getSessionContext } = await import('./session')

    await expect(getSessionContext()).resolves.toMatchObject({
      brokerId: 42,
      entityMainId: 42,
      userId: null,
      profileId: null,
      profileTypeId: null,
      isAdmin: null,
      headerExecutionContextEnabled: false,
      polizasExecutionContextRequired: false,
      application: { key: 'iliniumtech', name: 'iLiniumTech' },
      allowedBrokerIds: [42],
      permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
      authMode: 'local',
    })
    expect(apiClient.get).not.toHaveBeenCalled()
  })

  it('exposes a friendly error without leaking a failed session', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    const { apiClient } = await import('./apiClient')
    vi.mocked(apiClient.get).mockRejectedValueOnce(new Error('unauthorized'))

    const { useSession } = await import('./session')
    const currentSession = useSession()

    await expect(currentSession.loadSession()).resolves.toBeNull()

    expect(currentSession.session.value).toBeNull()
    expect(currentSession.error.value).toBe('No se pudo cargar la sesion.')
    expect(currentSession.errorKind.value).toBe('unknown')
  })

  it('tracks unauthenticated backend session failures for callers', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
    const { apiClient } = await import('./apiClient')
    vi.mocked(apiClient.get).mockRejectedValueOnce(
      axiosError(401, {
        error: {
          message: 'Expired demo-session cookie.',
          correlationId: 'auth-401',
        },
      }),
    )

    const { useSession } = await import('./session')
    const currentSession = useSession()

    await expect(currentSession.loadSession()).resolves.toBeNull()

    expect(currentSession.errorKind.value).toBe('unauthenticated')
    expect(currentSession.error.value).toContain('Ref: auth-401.')
  })

  it('blocks backend session calls when the API key contract is incomplete', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', '__SET_IN_ENVIRONMENT__')
    const { apiClient } = await import('./apiClient')
    const { useSession } = await import('./session')
    const currentSession = useSession()

    await expect(currentSession.loadSession()).resolves.toBeNull()

    expect(currentSession.error.value).toContain('VITE_ILINIUMTECH_API_KEY')
    expect(currentSession.errorKind.value).toBe('runtime')
    expect(apiClient.get).not.toHaveBeenCalled()
  })
})
