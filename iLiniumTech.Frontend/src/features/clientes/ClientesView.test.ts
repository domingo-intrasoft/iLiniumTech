import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import ClientesView from './ClientesView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|IapMenu|datasource/i
const personalDataMarkers =
  /[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}|\b\d{8}[A-Z]\b|\b\d{9}\b|\b\d{3}[-.\s]?\d{3}[-.\s]?\d{3}\b|IBAN|ES\d{22}|password=|pwd=/i

async function mountClientesView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: { template: '<div />' } },
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: ClientesView },
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
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/clientes')
  await router.isReady()

  return mount(ClientesView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleClientesView() {
  await flushPromises()
  await flushPromises()
}

describe('ClientesView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only clientes MVP with local fixture rows and pagination', async () => {
    const wrapper = await mountClientesView()
    await settleClientesView()

    expect(wrapper.text()).toContain('Clientes')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos minimizados')
    expect(wrapper.text()).toContain('PII bloqueada')
    expect(wrapper.text()).toContain('Tabs relacionadas pendientes')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 clientes demo')
    expect(wrapper.get('caption').text()).toContain(
      'Clientes fixture read-only: 1-2 de 4 clientes demo',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('CLI-2026-0001')
    expect(wrapper.text()).toContain('CLI-2026-0002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleClientesView()

    expect(wrapper.text()).toContain('CLI-2026-0003')
    expect(wrapper.text()).toContain('CLI-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters fixture rows locally by reference, state, segment, and signup date', async () => {
    const wrapper = await mountClientesView()
    await settleClientesView()

    await wrapper.get('#clientes-filter-texto').setValue('0004')
    await wrapper.get('#clientes-filter-estado').setValue('Activo demo')
    await wrapper.get('#clientes-filter-segmento').setValue('Particular demo')
    await wrapper.get('#clientes-filter-fecha').setValue('2026-04-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleClientesView()

    expect(wrapper.get('.summary-header').text()).toContain('1 cliente demo')
    expect(wrapper.get('caption').text()).toContain(
      'Clientes fixture read-only: 1-1 de 1 cliente demo',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('CLI-2026-0004')
    expect(wrapper.text()).toContain('Alias anonimo D')
    expect(wrapper.text()).not.toContain('CLI-2026-0001')
  })

  it('shows an empty state for filters without fixture matches', async () => {
    const wrapper = await mountClientesView()
    await settleClientesView()

    await wrapper.get('#clientes-filter-texto').setValue('CLI-2026-9999')
    await wrapper.get('#clientes-filter-segmento').setValue('Empresa demo')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleClientesView()

    expect(wrapper.get('.summary-header').text()).toContain('0 clientes demo')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay clientes fixture para los filtros actuales.')
  })

  it('keeps real actions disabled and exposes no real PII or runtime details', async () => {
    const wrapper = await mountClientesView()
    await settleClientesView()

    const openButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Abrir ficha'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))
    const breakdownButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Desglose'))

    expect(wrapper.get('#clientes-blocked-actions').text()).toContain(
      'Acciones de clientes bloqueadas en el MVP read-only',
    )
    expect(openButton?.attributes('disabled')).toBeDefined()
    expect(openButton?.attributes('aria-describedby')).toBe('clientes-blocked-actions')
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('aria-describedby')).toBe('clientes-blocked-actions')
    expect(breakdownButton?.attributes('disabled')).toBeDefined()
    expect(breakdownButton?.attributes('aria-describedby')).toBe('clientes-blocked-actions')
    expect(
      wrapper.get('button[aria-label="Opciones de referencia"]').attributes('aria-describedby'),
    ).toBe('clientes-blocked-actions')
    const tableActions = wrapper.findAll('button.table-icon-action[disabled]')
    expect(tableActions).toHaveLength(4)
    expect(tableActions[0]?.attributes('aria-describedby')).toBe('clientes-blocked-actions')
    expect(
      (wrapper.get('input[aria-label="Campos PII bloqueados"]').element as HTMLInputElement).value,
    ).toBe('Bloqueado: no se captura ni muestra PII en este MVP')
    expect(
      wrapper.get('input[aria-label="Campos PII bloqueados"]').attributes('aria-describedby'),
    ).toBe('clientes-blocked-actions')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(personalDataMarkers)
  })
})
