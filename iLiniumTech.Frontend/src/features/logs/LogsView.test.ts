import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import LogsView from './LogsView.vue'

const unsafeRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|metadata heredada/i
const secretOrRealDataMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|bearer|api[-_ ]?key|token|https?:\/\/|@|\b\d{8}[A-Z]\b/i

async function mountLogsView() {
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
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: LogsView },
    ],
  })
  await router.push('/logs')
  await router.isReady()

  return mount(LogsView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleView() {
  await flushPromises()
  await flushPromises()
}

describe('LogsView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only logs MVP with redacted fixture data and pagination', async () => {
    const wrapper = await mountLogsView()
    await settleView()

    expect(wrapper.text()).toContain('Logs')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos redactados y sanitizados')
    expect(wrapper.text()).toContain('Secretos, logs, payloads y enlaces externos bloqueados')
    expect(wrapper.text()).toContain('Detalle y exportacion deshabilitados')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 logs demo')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('LOG-DEMO-001')
    expect(wrapper.text()).toContain('LOG-DEMO-002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleView()

    expect(wrapper.text()).toContain('LOG-DEMO-003')
    expect(wrapper.text()).toContain('LOG-DEMO-004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters logs locally by text, area, state, and risk', async () => {
    const wrapper = await mountLogsView()
    await settleView()

    await wrapper.get('#logs-filter-text').setValue('request')
    await wrapper.get('#logs-filter-area').setValue('Servicios')
    await wrapper.get('#logs-filter-state').setValue('Pendiente')
    await wrapper.get('#logs-filter-risk').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('1 log demo')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('LOG-DEMO-003')
    expect(wrapper.text()).toContain('Request y response no expuestos')
    expect(wrapper.text()).not.toContain('LOG-DEMO-001')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountLogsView()
    await settleView()

    await wrapper.get('#logs-filter-text').setValue('descarga aprobada')
    await wrapper.get('#logs-filter-area').setValue('Auditoria')
    await wrapper.get('#logs-filter-risk').setValue('Bajo')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('0 logs demo')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay logs fixture para los filtros actuales.')
  })

  it('keeps detail and export disabled without real payloads, traces, or identities', async () => {
    const wrapper = await mountLogsView()
    await settleView()

    const detailButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Detalle'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))

    expect(detailButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Detalle y exportacion bloqueados"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: sin API, sin datos personales, sin exportacion')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(unsafeRuntimeMarkers)
    expect(smokeDom).not.toMatch(secretOrRealDataMarkers)
  })
})
