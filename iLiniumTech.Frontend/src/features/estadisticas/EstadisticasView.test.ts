import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import EstadisticasView from './EstadisticasView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|vw_rpt_|IapMenu|datasource/i
const realDataMarkers =
  /[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}|\b\d{8}[A-Z]\b|\b\d{9}\b|\b\d{3}[-.\s]?\d{3}[-.\s]?\d{3}\b|IBAN|ES\d{22}|password=|pwd=/i

async function mountEstadisticasView() {
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
      { path: '/siniestros', component: { template: '<div />' } },
      { path: '/liq-cia', component: { template: '<div />' } },
      { path: '/liq-col', component: { template: '<div />' } },
      { path: '/informes', component: { template: '<div />' } },
      { path: '/controles', component: { template: '<div />' } },
      { path: '/estadisticas', component: EstadisticasView },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/estadisticas')
  await router.isReady()

  return mount(EstadisticasView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleEstadisticasView() {
  await flushPromises()
  await flushPromises()
}

describe('EstadisticasView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only statistics MVP with candidate KPIs and pagination', async () => {
    const wrapper = await mountEstadisticasView()
    await settleEstadisticasView()

    expect(wrapper.text()).toContain('Estadisticas')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('KPIs no confirmados')
    expect(wrapper.text()).toContain('Sin drilldown ni exportacion')
    expect(wrapper.text()).toContain('SDD/UAT pendiente')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 KPIs candidatos')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('EST-2026-0001')
    expect(wrapper.text()).toContain('EST-2026-0002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleEstadisticasView()

    expect(wrapper.text()).toContain('EST-2026-0003')
    expect(wrapper.text()).toContain('EST-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters candidate KPIs locally by text, area, state, and period', async () => {
    const wrapper = await mountEstadisticasView()
    await settleEstadisticasView()

    await wrapper.get('#estadisticas-filter-texto').setValue('recibos')
    await wrapper.get('#estadisticas-filter-area').setValue('Recibos')
    await wrapper.get('#estadisticas-filter-estado').setValue('Candidata')
    await wrapper.get('#estadisticas-filter-periodo').setValue('Trimestral')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleEstadisticasView()

    expect(wrapper.get('.summary-header').text()).toContain('1 KPI candidato')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('EST-2026-0002')
    expect(wrapper.text()).toContain('Distribucion candidata de recibos')
    expect(wrapper.text()).not.toContain('EST-2026-0001')
  })

  it('shows an empty state for filters without candidate matches', async () => {
    const wrapper = await mountEstadisticasView()
    await settleEstadisticasView()

    await wrapper.get('#estadisticas-filter-texto').setValue('kpi real confirmado')
    await wrapper.get('#estadisticas-filter-area').setValue('Polizas')
    await wrapper.get('#estadisticas-filter-periodo').setValue('Anual')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleEstadisticasView()

    expect(wrapper.get('.summary-header').text()).toContain('0 KPIs candidatos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay KPIs candidatos para los filtros actuales.')
  })

  it('keeps refresh/export/drilldown actions disabled and exposes no real data', async () => {
    const wrapper = await mountEstadisticasView()
    await settleEstadisticasView()

    const refreshButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Refrescar'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))
    const drilldownButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Drilldown'))

    expect(refreshButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(drilldownButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Acciones de estadisticas bloqueadas"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: no hay KPIs aprobados ni API')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(realDataMarkers)
  })
})
