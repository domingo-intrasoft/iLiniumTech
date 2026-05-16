import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import type { SessionContext } from '@/services/session'

const mocks = vi.hoisted(() => ({
  apiGet: vi.fn(),
}))

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    get: mocks.apiGet,
  },
}))

import { useSession } from '@/services/session'

import PolizasView from './PolizasView.vue'
import { polizasCatalogsFixture, polizasFixture } from './polizasFixture'

async function mountPolizasView(path = '/polizas') {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: { template: '<div />' } },
      { path: '/propuestas', component: { template: '<div />' } },
      { path: '/polizas', name: 'polizas', component: PolizasView },
      { path: '/autos-particulares', component: { template: '<div />' } },
      { path: '/polizas/flotas', component: { template: '<div />' } },
      { path: '/polizas/colectivas', component: { template: '<div />' } },
      { path: '/polizas/:id', name: 'poliza-detail', component: { template: '<div />' } },
      { path: '/recibos', component: { template: '<div />' } },
      { path: '/suplementos', component: { template: '<div />' } },
      { path: '/siniestros', component: { template: '<div />' } },
      { path: '/liq-cia', component: { template: '<div />' } },
      { path: '/liq-col', component: { template: '<div />' } },
      { path: '/informes', component: { template: '<div />' } },
      { path: '/controles', component: { template: '<div />' } },
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push(path)
  await router.isReady()

  const wrapper = mount(PolizasView, {
    global: {
      plugins: [router],
    },
  })

  return { router, wrapper }
}

async function settlePolizasView() {
  await flushPromises()
  await flushPromises()
}

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

function setupBackendApi(session: SessionContext) {
  mocks.apiGet.mockImplementation((url: string) => {
    if (url === '/api/me') {
      return Promise.resolve({ data: session })
    }

    if (url === '/api/polizas') {
      return Promise.resolve({ data: polizasFixture })
    }

    if (url === '/api/polizas/catalogs') {
      return Promise.resolve({ data: polizasCatalogsFixture })
    }

    return Promise.reject(new Error(`Unexpected API call: ${url}`))
  })
}

describe('PolizasView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    mocks.apiGet.mockReset()
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the local read-only shell, filters fixture rows, and exposes no runtime metadata', async () => {
    const { router, wrapper } = await mountPolizasView()
    await settlePolizasView()

    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local')
    expect(wrapper.text()).toContain('Modo local')
    expect(wrapper.get('.summary-header').text()).toContain(`${polizasFixture.total} polizas`)
    expect(wrapper.findAll('tbody tr')).toHaveLength(polizasFixture.total)
    expect(wrapper.text()).toContain('POL-2026-0001')
    expect(wrapper.text()).toContain('POL-2026-0002')

    await wrapper.get('#polizas-filter-poliza').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settlePolizasView()

    expect(router.currentRoute.value.query).toEqual({
      page: '1',
      pageSize: '25',
      numero: '0002',
    })
    expect(wrapper.get('.summary-header').text()).toContain('1 poliza')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('POL-2026-0002')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).not.toContain('POL-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settlePolizasView()

    expect(router.currentRoute.value.query).toEqual({
      page: '1',
      pageSize: '25',
    })
    expect(wrapper.get('.summary-header').text()).toContain(`${polizasFixture.total} polizas`)
    expect(wrapper.findAll('tbody tr')).toHaveLength(polizasFixture.total)
    expect(wrapper.text()).toContain('POL-2026-0001')
    expect(wrapper.text()).toContain('POL-2026-0002')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toContain('connectionString')
    expect(smokeDom).not.toContain('SELECT *')
    expect(smokeDom).not.toContain('AppBuilder')
    expect(smokeDom).not.toMatch(/IAP_|QueryStatic|ComponentDataSource|Pantalla_Polizas/)
  })

  it('initializes filters and detail links from the polizas route query', async () => {
    const { router, wrapper } = await mountPolizasView('/polizas?numero=0002&page=1&pageSize=10')
    await settlePolizasView()

    expect((wrapper.get('#polizas-filter-poliza').element as HTMLInputElement).value).toBe('0002')
    expect(wrapper.get('.summary-header').text()).toContain('1 poliza')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)

    const detailLink = wrapper.get('.table-icon-action')
    expect(detailLink.attributes('href')).toContain('/polizas/POL-1002')
    expect(detailLink.attributes('href')).toContain('numero=0002')
    expect(detailLink.attributes('href')).toContain('page=1')
    expect(detailLink.attributes('href')).toContain('pageSize=10')

    await router.push('/polizas?cliente=Cliente%20anonimo%201&page=1&pageSize=25')
    await settlePolizasView()

    expect((wrapper.get('#polizas-filter-nombreCompleto').element as HTMLInputElement).value).toBe(
      'Cliente anonimo 1',
    )
    expect(wrapper.get('.summary-header').text()).toContain('1 poliza')
    expect(wrapper.text()).toContain('POL-2026-0001')
    expect(wrapper.text()).not.toContain('POL-2026-0002')
  })

  it('keeps detail navigation available when backend session grants polizas.detail', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    setupBackendApi(backendSession())

    const { wrapper } = await mountPolizasView()
    await settlePolizasView()

    expect(wrapper.findAll('a.table-icon-action')).toHaveLength(polizasFixture.items.length)
    expect(wrapper.findAll('a.table-link')).toHaveLength(polizasFixture.items.length)
    expect(wrapper.get('a.table-icon-action').attributes('aria-label')).toContain(
      'Ver detalle de poliza',
    )
  })

  it('disables detail navigation when backend session omits polizas.detail', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    vi.stubEnv('VITE_ILINIUMTECH_API_KEY', 'test-api-key')
    setupBackendApi(
      backendSession({
        permissions: ['polizas.catalogs', 'polizas.read'],
      }),
    )

    const { wrapper } = await mountPolizasView()
    await settlePolizasView()

    const disabledDetailActions = wrapper.findAll('button.table-icon-action')

    expect(disabledDetailActions).toHaveLength(polizasFixture.items.length)
    expect(disabledDetailActions[0].attributes('disabled')).toBeDefined()
    expect(disabledDetailActions[0].attributes('aria-label')).toContain('Detalle no disponible')
    expect(wrapper.find('a.table-icon-action').exists()).toBe(false)
    expect(wrapper.find('a.table-link').exists()).toBe(false)
  })

  it('normalizes unsupported page sizes, invalid pages, invalid dates, and unknown query keys', async () => {
    const { router, wrapper } = await mountPolizasView(
      '/polizas?numero=0001&page=0&pageSize=99&fechaEfectoDesde=nope&fechaEfectoHasta=2026-02-31&metadata=IAP',
    )
    await settlePolizasView()

    expect((wrapper.get('#polizas-filter-poliza').element as HTMLInputElement).value).toBe('0001')
    expect((wrapper.get('#polizas-filter-efectoInicial').element as HTMLInputElement).value).toBe(
      '',
    )
    expect((wrapper.get('#polizas-filter-efectoFinal').element as HTMLInputElement).value).toBe('')
    expect(router.currentRoute.value.query).toEqual({
      page: '1',
      pageSize: '25',
      numero: '0001',
    })
    expect(wrapper.get('.summary-header').text()).toContain('1 poliza')
  })
})
