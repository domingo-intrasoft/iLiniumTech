import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import ConectividadView from './ConectividadView.vue'

const unsafeRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|metadata heredada/i
const secretOrRealDataMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|bearer|api[-_ ]?key|token|https?:\/\/|@|\b\d{8}[A-Z]\b/i

async function mountConectividadView() {
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
      { path: '/conectividad', component: ConectividadView },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/conectividad')
  await router.isReady()

  return mount(ConectividadView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleView() {
  await flushPromises()
  await flushPromises()
}

describe('ConectividadView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only connectivity MVP with sanitized fixture data and pagination', async () => {
    const wrapper = await mountConectividadView()
    await settleView()

    expect(wrapper.text()).toContain('Conectividad')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Secretos, logs, payloads y enlaces externos bloqueados')
    expect(wrapper.text()).toContain('Pruebas externas deshabilitadas')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 entradas candidatas')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('CON-DEMO-001')
    expect(wrapper.text()).toContain('CON-DEMO-002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleView()

    expect(wrapper.text()).toContain('CON-DEMO-003')
    expect(wrapper.text()).toContain('CON-DEMO-004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters connectivity candidates locally by text, area, state, and risk', async () => {
    const wrapper = await mountConectividadView()
    await settleView()

    await wrapper.get('#conectividad-filter-text').setValue('allowlist')
    await wrapper.get('#conectividad-filter-area').setValue('Integraciones')
    await wrapper.get('#conectividad-filter-state').setValue('Bloqueado')
    await wrapper.get('#conectividad-filter-risk').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('1 entrada candidata')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('CON-DEMO-004')
    expect(wrapper.text()).toContain('URL libre no permitida')
    expect(wrapper.text()).not.toContain('CON-DEMO-001')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountConectividadView()
    await settleView()

    await wrapper.get('#conectividad-filter-text').setValue('servicio aprobado')
    await wrapper.get('#conectividad-filter-state').setValue('Observado')
    await wrapper.get('#conectividad-filter-risk').setValue('Bajo')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('0 entradas candidatas')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain(
      'No hay entradas fixture de conectividad para los filtros actuales.',
    )
  })

  it('keeps external tests, detail, and export disabled without real endpoints or secrets', async () => {
    const wrapper = await mountConectividadView()
    await settleView()

    const testButton = wrapper.findAll('button').find((button) => button.text().includes('Probar'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))

    expect(testButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Pruebas y endpoints bloqueados"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: sin API, sin URLs reales, sin llamadas externas')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(unsafeRuntimeMarkers)
    expect(smokeDom).not.toMatch(secretOrRealDataMarkers)
  })
})
