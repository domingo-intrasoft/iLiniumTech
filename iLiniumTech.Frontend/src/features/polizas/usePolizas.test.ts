import { flushPromises, mount } from '@vue/test-utils'
import { defineComponent } from 'vue'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import type { SessionContext } from '@/services/session'

import { polizasCatalogsFixture, polizasFixture } from './polizasFixture'

const mocks = vi.hoisted(() => ({
  session: { value: null as SessionContext | null },
  sessionLoading: { value: false },
  sessionError: { value: null as string | null },
  sessionErrorKind: { value: null as 'unauthenticated' | null },
  loadSession: vi.fn(),
  clearAuthSession: vi.fn(),
  getBlockingRuntimeConfigMessage: vi.fn(),
  searchPolizas: vi.fn(),
  getPolizasCatalogs: vi.fn(),
}))

vi.mock('@/features/auth/authSession', () => ({
  clearAuthSession: mocks.clearAuthSession,
}))

vi.mock('@/services/session', () => ({
  useSession: () => ({
    session: mocks.session,
    loading: mocks.sessionLoading,
    error: mocks.sessionError,
    errorKind: mocks.sessionErrorKind,
    loadSession: mocks.loadSession,
  }),
}))

vi.mock('@/services/runtimeConfig', () => ({
  RuntimeConfigError: class RuntimeConfigError extends Error {},
  getBlockingRuntimeConfigMessage: mocks.getBlockingRuntimeConfigMessage,
}))

vi.mock('./polizasApi', () => ({
  searchPolizas: mocks.searchPolizas,
  getPolizasCatalogs: mocks.getPolizasCatalogs,
}))

import { usePolizas } from './usePolizas'

const Harness = defineComponent({
  setup() {
    return {
      polizas: usePolizas(),
    }
  },
  template: '<div />',
})

function backendSession(overrides: Partial<SessionContext> = {}): SessionContext {
  return {
    brokerId: 42,
    entityMainId: 42,
    userId: 7,
    profileId: 9,
    profileTypeId: 'mvp-profile',
    isAdmin: false,
    headerExecutionContextEnabled: true,
    polizasExecutionContextRequired: true,
    permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
    ...overrides,
  }
}

function axiosError(status: number, data: unknown) {
  return {
    isAxiosError: true,
    response: {
      status,
      data,
    },
  }
}

describe('usePolizas', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.session.value = null
    mocks.sessionLoading.value = false
    mocks.sessionError.value = null
    mocks.sessionErrorKind.value = null
    mocks.loadSession.mockReset()
    mocks.clearAuthSession.mockReset()
    mocks.getBlockingRuntimeConfigMessage.mockReset()
    mocks.searchPolizas.mockReset()
    mocks.getPolizasCatalogs.mockReset()
    mocks.getBlockingRuntimeConfigMessage.mockReturnValue(null)
    mocks.searchPolizas.mockResolvedValue(polizasFixture)
    mocks.getPolizasCatalogs.mockResolvedValue(polizasCatalogsFixture)
  })

  it('does not query polizas when the backend session cannot be validated', async () => {
    mocks.sessionError.value = 'La sesion actual no tiene permiso para consultar polizas.'
    mocks.loadSession.mockResolvedValue(null)

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).not.toHaveBeenCalled()
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
    expect(wrapper.vm.polizas.error.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas.',
    )
  })

  it('clears local auth when backend session validation returns 401', async () => {
    mocks.sessionError.value =
      'La sesion no esta autorizada para consultar polizas. Inicia sesion de nuevo si el problema continua.'
    mocks.sessionErrorKind.value = 'unauthenticated'
    mocks.loadSession.mockResolvedValue(null)

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).not.toHaveBeenCalled()
    expect(mocks.clearAuthSession).toHaveBeenCalledTimes(1)
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
  })

  it('blocks backend searches when runtime configuration is incomplete', async () => {
    mocks.getBlockingRuntimeConfigMessage.mockReturnValue(
      'VITE_ILINIUMTECH_API_KEY debe configurarse fuera de Git para usar la API.',
    )

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.loadSession).not.toHaveBeenCalled()
    expect(mocks.searchPolizas).not.toHaveBeenCalled()
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
    expect(wrapper.vm.polizas.error.value).toContain('VITE_ILINIUMTECH_API_KEY')
  })

  it('queries backend polizas after validating a broker session', async () => {
    mocks.loadSession.mockResolvedValue(backendSession())

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.loadSession).toHaveBeenCalled()
    expect(mocks.searchPolizas).toHaveBeenCalledWith({
      numero: undefined,
      cliente: undefined,
      estado: undefined,
      compania: undefined,
      ramo: undefined,
      fechaEfectoDesde: undefined,
      fechaEfectoHasta: undefined,
      page: 1,
      pageSize: 25,
      sort: 'fechaEfecto:desc',
    })
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(false)
    expect(wrapper.vm.polizas.error.value).toBeNull()
    expect(wrapper.vm.polizas.items.value).toEqual(polizasFixture.items)
    expect(wrapper.vm.polizas.total.value).toBe(polizasFixture.total)
  })

  it('blocks backend searches when the session requires a broker but has none', async () => {
    mocks.loadSession.mockResolvedValue(
      backendSession({
        brokerId: null,
        entityMainId: null,
      }),
    )

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).not.toHaveBeenCalled()
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
    expect(wrapper.vm.polizas.error.value).toBe('Configura un broker para consultar polizas.')
  })

  it('blocks backend searches when explicit session permissions omit polizas.read', async () => {
    mocks.loadSession.mockResolvedValue(backendSession({ permissions: ['polizas.catalogs'] }))

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).not.toHaveBeenCalled()
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
    expect(wrapper.vm.polizas.error.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas.',
    )
    expect(wrapper.vm.polizas.runtimeError.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas.',
    )
  })

  it('keeps legacy API key backend mode compatible when /api/me does not declare permissions', async () => {
    mocks.loadSession.mockResolvedValue(
      backendSession({
        authMode: 'ApiKey',
        permissions: [],
      }),
    )

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).toHaveBeenCalled()
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(false)
    expect(wrapper.vm.polizas.error.value).toBeNull()
  })

  it('keeps search available when catalog permission is denied separately', async () => {
    mocks.loadSession.mockResolvedValue(backendSession({ permissions: ['polizas.read'] }))

    const wrapper = mount(Harness)
    await flushPromises()

    expect(mocks.searchPolizas).toHaveBeenCalled()
    expect(mocks.getPolizasCatalogs).not.toHaveBeenCalled()
    expect(wrapper.vm.polizas.catalogsError.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas.',
    )
    expect(wrapper.vm.polizas.contextBlocked.value).toBe(false)
  })

  it('turns backend 403 list failures into an access denied state', async () => {
    mocks.loadSession.mockResolvedValue(backendSession())
    mocks.searchPolizas.mockRejectedValueOnce(
      axiosError(403, {
        error: {
          message: 'User lacks polizas.read',
          correlationId: 'deny-403',
        },
      }),
    )

    const wrapper = mount(Harness)
    await flushPromises()

    expect(wrapper.vm.polizas.contextBlocked.value).toBe(true)
    expect(wrapper.vm.polizas.error.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas. Ref: deny-403.',
    )
    expect(wrapper.vm.polizas.runtimeError.value).toBe(
      'La sesion actual no tiene permiso para consultar polizas. Ref: deny-403.',
    )
  })
})
