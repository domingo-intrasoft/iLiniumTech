import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import SuplementosView from './SuplementosView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata/i
const secretOrSensitiveMarkers =
  /[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}|\b\d{9}\b|\b\d{3}[-.\s]?\d{3}[-.\s]?\d{3}\b|IBAN|documento|telefono|direccion/i

async function mountSuplementosView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: { template: '<div />' } },
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: { template: '<div />' } },
      { path: '/propuestas', component: { template: '<div />' } },
      { path: '/polizas', component: { template: '<div />' } },
      { path: '/autos-particulares', component: { template: '<div />' } },
      { path: '/polizas/flotas', component: { template: '<div />' } },
      { path: '/polizas/colectivas', component: { template: '<div />' } },
      { path: '/recibos', component: { template: '<div />' } },
      { path: '/suplementos', component: SuplementosView },
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
  await router.push('/suplementos')
  await router.isReady()

  return mount(SuplementosView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleSuplementosView() {
  await flushPromises()
  await flushPromises()
}

describe('SuplementosView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only fixture screen with pagination and disabled actions', async () => {
    const wrapper = await mountSuplementosView()
    await settleSuplementosView()

    expect(wrapper.text()).toContain('Suplementos')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos sanitizados')
    expect(wrapper.text()).toContain('Sin workflows ni escrituras')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 suplementos')
    expect(wrapper.get('caption').text()).toContain(
      'Suplementos fixture read-only: 1-2 de 4 suplementos',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('SUP-2026-0001')
    expect(wrapper.text()).toContain('SUP-2026-0002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleSuplementosView()

    expect(wrapper.text()).toContain('SUP-2026-0003')
    expect(wrapper.text()).toContain('SUP-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(7)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
  })

  it('filters fixture rows with a simple local search', async () => {
    const wrapper = await mountSuplementosView()
    await settleSuplementosView()

    await wrapper.get('#suplementos-filter-texto').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleSuplementosView()

    expect(wrapper.get('.summary-header').text()).toContain('1 suplemento')
    expect(wrapper.get('caption').text()).toContain(
      'Suplementos fixture read-only: 1-1 de 1 suplemento',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('SUP-2026-0002')
    expect(wrapper.text()).toContain('Revision de condiciones')
    expect(wrapper.text()).not.toContain('SUP-2026-0001')
  })

  it('shows an empty state for filters without fixture matches', async () => {
    const wrapper = await mountSuplementosView()
    await settleSuplementosView()

    await wrapper.get('#suplementos-filter-situacion').setValue('Validado')
    await wrapper.get('#suplementos-filter-fecha').setValue('2026-05-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleSuplementosView()

    expect(wrapper.get('.summary-header').text()).toContain('0 suplementos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay suplementos fixture para los filtros actuales.')
  })

  it('does not expose runtime markers, secrets, or sensitive fixture fields', async () => {
    const wrapper = await mountSuplementosView()
    await settleSuplementosView()

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
  })
})
