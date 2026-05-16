import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import LiquidacionesCompaniaView from './LiquidacionesCompaniaView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata|datasource/i
const secretOrSensitiveMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|secret|token|iban|cuenta bancaria|documento|direccion|telefono|email|@|\b\d{8}[A-Z]\b/i
const realFinancialOrBankingMarkers =
  /[A-Z]{2}\d{2}[A-Z0-9]{11,30}|\b\d+[.,]\d{2}\s*EUR\b|factura real/i

async function mountLiquidacionesCompaniaView() {
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
      { path: '/liq-cia', component: LiquidacionesCompaniaView },
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
  await router.push('/liq-cia')
  await router.isReady()

  return mount(LiquidacionesCompaniaView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleLiquidacionesCompaniaView() {
  await flushPromises()
  await flushPromises()
}

describe('LiquidacionesCompaniaView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only company settlements fixture screen with disabled actions', async () => {
    const wrapper = await mountLiquidacionesCompaniaView()
    await settleLiquidacionesCompaniaView()

    expect(wrapper.text()).toContain('Liquidaciones de compania')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos minimizados')
    expect(wrapper.text()).toContain('Operaciones financieras bloqueadas')
    expect(wrapper.text()).toContain('Sin importes reales')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 liquidaciones')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('LCIA-2026-0001')
    expect(wrapper.text()).toContain('LCIA-2026-0002')
    expect(wrapper.text()).toContain('Resultado demo B')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleLiquidacionesCompaniaView()

    expect(wrapper.text()).toContain('LCIA-2026-0003')
    expect(wrapper.text()).toContain('LCIA-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(8)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
  })

  it('filters fixture rows by reference, company, status, office, and date', async () => {
    const wrapper = await mountLiquidacionesCompaniaView()
    await settleLiquidacionesCompaniaView()

    await wrapper.get('#liq-cia-filter-texto').setValue('sur')
    await wrapper.get('#liq-cia-filter-estado').setValue('En revision')
    await wrapper.get('#liq-cia-filter-oficina').setValue('costa')
    await wrapper.get('#liq-cia-filter-fecha').setValue('2026-02-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleLiquidacionesCompaniaView()

    expect(wrapper.get('.summary-header').text()).toContain('1 liquidacion')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('LCIA-2026-0002')
    expect(wrapper.text()).toContain('Compania demo sur')
    expect(wrapper.text()).not.toContain('LCIA-2026-0001')
  })

  it('shows an empty state for filters without fixture matches', async () => {
    const wrapper = await mountLiquidacionesCompaniaView()
    await settleLiquidacionesCompaniaView()

    await wrapper.get('#liq-cia-filter-estado').setValue('Cerrada')
    await wrapper.get('#liq-cia-filter-fecha').setValue('2026-04-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleLiquidacionesCompaniaView()

    expect(wrapper.get('.summary-header').text()).toContain('0 liquidaciones')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain(
      'No hay liquidaciones de compania fixture para los filtros actuales.',
    )
  })

  it('does not expose real data, unsafe runtime markers, or operative financial actions', async () => {
    const wrapper = await mountLiquidacionesCompaniaView()
    await settleLiquidacionesCompaniaView()

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
    expect(smokeDom).not.toMatch(realFinancialOrBankingMarkers)
    expect(
      wrapper.findAll('button[disabled]').some((button) => button.text().includes('Exportar')),
    ).toBe(true)
    expect(
      wrapper.find('input[aria-label="Operaciones financieras bloqueadas"]').attributes('disabled'),
    ).toBeDefined()
  })
})
