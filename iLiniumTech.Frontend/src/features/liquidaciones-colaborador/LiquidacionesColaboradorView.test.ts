import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import LiquidacionesColaboradorView from './LiquidacionesColaboradorView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata|vw_rpt_|datasource/i
const secretOrSensitiveMarkers =
  /[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}|\b\d{8}[A-Z]\b|\b\d{9}\b|IBAN|cuenta bancaria|password=|pwd=|token|secret/i
const realFinancialMarkers = /\b\d{1,3}(?:[.,]\d{3})*[.,]\d{2}\s?(?:EUR|€)|\b\d+[.,]\d{2}\b/

async function mountLiquidacionesColaboradorView() {
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
      { path: '/liq-col', component: LiquidacionesColaboradorView },
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
  await router.push('/liq-col')
  await router.isReady()

  return mount(LiquidacionesColaboradorView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleLiquidacionesColaboradorView() {
  await flushPromises()
  await flushPromises()
}

describe('LiquidacionesColaboradorView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only collaborator settlements fixture screen with blocked actions', async () => {
    const wrapper = await mountLiquidacionesColaboradorView()
    await settleLiquidacionesColaboradorView()

    expect(wrapper.text()).toContain('Liquidaciones de colaborador')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos minimizados y sanitizados')
    expect(wrapper.text()).toContain('Detalle y conceptos no operativos')
    expect(wrapper.text()).toContain('Comisiones y liquidos bloqueados')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 liquidaciones')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('LC-2026-0001')
    expect(wrapper.text()).toContain('Colaborador anonimo B')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleLiquidacionesColaboradorView()

    expect(wrapper.text()).toContain('LC-2026-0003')
    expect(wrapper.text()).toContain('LC-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(10)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(4)
  })

  it('filters fixture rows by reference, collaborator, state, office, and date', async () => {
    const wrapper = await mountLiquidacionesColaboradorView()
    await settleLiquidacionesColaboradorView()

    await wrapper.get('#liq-col-filter-texto').setValue('anonimo c')
    await wrapper.get('#liq-col-filter-estado').setValue('Bloqueada')
    await wrapper.get('#liq-col-filter-oficina').setValue('Oficina demo este')
    await wrapper.get('#liq-col-filter-fecha').setValue('2026-03-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleLiquidacionesColaboradorView()

    expect(wrapper.get('.summary-header').text()).toContain('1 liquidacion')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('LC-2026-0003')
    expect(wrapper.text()).toContain('Colaborador anonimo C')
    expect(wrapper.text()).not.toContain('LC-2026-0001')
  })

  it('shows an empty state for filters without fixture matches', async () => {
    const wrapper = await mountLiquidacionesColaboradorView()
    await settleLiquidacionesColaboradorView()

    await wrapper.get('#liq-col-filter-estado').setValue('Cerrada')
    await wrapper.get('#liq-col-filter-fecha').setValue('2026-05-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleLiquidacionesColaboradorView()

    expect(wrapper.get('.summary-header').text()).toContain('0 liquidaciones')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay liquidaciones fixture para los filtros actuales.')
  })

  it('does not expose unsafe runtime markers, secrets, documents, bank data, or real amounts', async () => {
    const wrapper = await mountLiquidacionesColaboradorView()
    await settleLiquidacionesColaboradorView()

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
    expect(smokeDom).not.toMatch(realFinancialMarkers)
  })
})
