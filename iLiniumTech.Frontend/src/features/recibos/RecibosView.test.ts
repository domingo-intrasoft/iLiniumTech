import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import RecibosView from './RecibosView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata/i
const secretOrSensitiveMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|secret|token|iban|cuenta bancaria|direccion|telefono|email|@/i

async function mountRecibosView() {
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
      { path: '/recibos', component: RecibosView },
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
  await router.push('/recibos')
  await router.isReady()

  return mount(RecibosView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleRecibosView() {
  await flushPromises()
  await flushPromises()
}

describe('RecibosView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only fixture shell, filters rows, and exposes no unsafe runtime details', async () => {
    const wrapper = await mountRecibosView()
    await settleRecibosView()

    expect(wrapper.text()).toContain('Recibos')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos sanitizados')
    expect(wrapper.text()).toContain('Detalle, exportacion y cobro pendientes')
    expect(wrapper.text()).toContain('Importes demo anonimizados')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('3 recibos')
    expect(wrapper.findAll('tbody tr')).toHaveLength(3)
    expect(wrapper.text()).toContain('REC-2026-0001')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).toContain('Importe demo C')

    await wrapper.get('#recibos-filter-recibo').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleRecibosView()

    expect(wrapper.get('.summary-header').text()).toContain('1 recibo')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('REC-2026-0002')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).not.toContain('REC-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settleRecibosView()

    expect(wrapper.get('.summary-header').text()).toContain('3 recibos')
    expect(wrapper.findAll('tbody tr')).toHaveLength(3)

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(7)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(3)

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
    expect(smokeDom).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
    expect(smokeDom).not.toMatch(/\b\d{8}[A-Z]\b/i)
  })

  it('shows the empty state for unsupported fixture filters', async () => {
    const wrapper = await mountRecibosView()
    await settleRecibosView()

    await wrapper.get('#recibos-filter-situacion').setValue('Anulado')
    await wrapper.get('#recibos-filter-vencimiento').setValue('2026-06-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleRecibosView()

    expect(wrapper.get('.summary-header').text()).toContain('0 recibos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay recibos fixture para los filtros actuales.')
  })
})
