import { flushPromises, mount } from '@vue/test-utils'
import { defineComponent } from 'vue'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import type { SessionContext } from '@/services/session'

import { polizasCatalogsFixture, polizasFixture } from './polizasFixture'

const mocks = vi.hoisted(() => ({
  session: { value: null as SessionContext | null },
  sessionLoading: { value: false },
  sessionError: { value: null as string | null },
  loadSession: vi.fn(),
  getBlockingRuntimeConfigMessage: vi.fn(),
  searchPolizas: vi.fn(),
  getPolizasCatalogs: vi.fn(),
}))

vi.mock('@/services/session', () => ({
  useSession: () => ({
    session: mocks.session,
    loading: mocks.sessionLoading,
    error: mocks.sessionError,
    loadSession: mocks.loadSession,
  }),
}))

vi.mock('@/services/runtimeConfig', () => ({
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

describe('usePolizas', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.session.value = null
    mocks.sessionLoading.value = false
    mocks.sessionError.value = null
    mocks.loadSession.mockReset()
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
})
