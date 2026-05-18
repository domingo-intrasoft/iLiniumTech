import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import { siniestrosFixture } from './siniestrosFixture'
import SiniestrosView from './SiniestrosView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata/i

async function mountSiniestrosView() {
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
      { path: '/suplementos', component: { template: '<div />' } },
      { path: '/siniestros', component: SiniestrosView },
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
  await router.push('/siniestros')
  await router.isReady()

  return mount(SiniestrosView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleSiniestrosView() {
  await flushPromises()
  await flushPromises()
}

describe('SiniestrosView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    vi.unstubAllGlobals()
    useSession().resetSession()
  })

  it('renders the read-only fixture shell, filters rows, and exposes no unsafe runtime details', async () => {
    const fetchMock = vi.fn()
    vi.stubGlobal('fetch', fetchMock)

    const wrapper = await mountSiniestrosView()
    await settleSiniestrosView()

    expect(wrapper.text()).toContain('Siniestros')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos sanitizados')
    expect(wrapper.text()).toContain('Detalle y exportacion pendientes')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('3 siniestros')
    expect(wrapper.get('caption').text()).toContain(
      'Siniestros fixture read-only: 1-3 de 3 siniestros',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(3)
    expect(wrapper.text()).toContain('SIN-2026-0001')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).toContain('Equipo tramitacion C')

    await wrapper.get('#siniestros-filter-referencia').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleSiniestrosView()

    expect(wrapper.get('.summary-header').text()).toContain('1 siniestro')
    expect(wrapper.get('caption').text()).toContain(
      'Siniestros fixture read-only: 1-1 de 1 siniestro',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('SIN-2026-0002')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).not.toContain('SIN-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settleSiniestrosView()

    expect(wrapper.get('.summary-header').text()).toContain('3 siniestros')
    expect(wrapper.findAll('tbody tr')).toHaveLength(3)

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(7)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(3)
    expect(wrapper.get('#siniestros-blocked-actions').text()).toContain('contrato API')
    expect(
      wrapper.get('button[aria-label="Ver detalle pendiente"]').attributes('aria-describedby'),
    ).toBe('siniestros-blocked-actions')
    expect(
      wrapper.get('button[aria-label="Opciones de estado"]').attributes('aria-describedby'),
    ).toBe('siniestros-blocked-actions')
    expect(wrapper.get('button.table-icon-action').attributes('aria-describedby')).toBe(
      'siniestros-blocked-actions',
    )

    const smokeDom = wrapper.html()
    const fixturePayload = JSON.stringify(siniestrosFixture)
    expect(fetchMock).not.toHaveBeenCalled()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(fixturePayload).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
    expect(fixturePayload).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
    expect(smokeDom).not.toMatch(/\b\d{4}\s?[A-Z]{3}\b/)
    expect(fixturePayload).not.toMatch(/\b\d{4}\s?[A-Z]{3}\b/)
    expect(fixturePayload).not.toMatch(/\b\d{8}[A-Z]\b/i)
  })

  it('shows the empty state for unsupported fixture filters', async () => {
    const wrapper = await mountSiniestrosView()
    await settleSiniestrosView()

    await wrapper.get('#siniestros-filter-estado').setValue('Cerrado')
    await wrapper.get('#siniestros-filter-fecha').setValue('2026-04-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleSiniestrosView()

    expect(wrapper.get('.summary-header').text()).toContain('0 siniestros')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay siniestros fixture para los filtros actuales.')
  })
})
