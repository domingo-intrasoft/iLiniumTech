import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import ConfiguracionView from './ConfiguracionView.vue'

const runtimeMarkers = /AppBuilder|IAP_|QueryStatic|ComponentDataSource|SELECT \*/i
const realDataMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|Bearer\s+[A-Za-z0-9]|[A-Z]{2}\d{2}[A-Z0-9]{11,30}|\b\d{8}[A-Z]\b|https?:\/\/|@/i

async function mountConfiguracionView() {
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
      { path: '/configuracion', component: ConfiguracionView },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/configuracion')
  await router.isReady()

  return mount(ConfiguracionView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleView() {
  await flushPromises()
  await flushPromises()
}

describe('ConfiguracionView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only fixture shell with redacted configuration rows', async () => {
    const wrapper = await mountConfiguracionView()
    await settleView()

    expect(wrapper.text()).toContain('Configuracion')
    expect(wrapper.text()).toContain('Superficie sensible')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Secretos y valores de entorno bloqueados')
    expect(wrapper.get('.summary-header').text()).toContain('4 registros')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('CFG-DEMO-GENERAL')
    expect(wrapper.text()).toContain('CFG-DEMO-SEGURIDAD')

    const dom = wrapper.html()
    expect(dom).not.toMatch(runtimeMarkers)
    expect(dom).not.toMatch(realDataMarkers)
  })

  it('filters by local text, area, status, and risk', async () => {
    const wrapper = await mountConfiguracionView()
    await settleView()

    await wrapper.get('#configuracion-filter-texto').setValue('integraciones')
    await wrapper.get('#configuracion-filter-area').setValue('Integraciones')
    await wrapper.get('#configuracion-filter-estado').setValue('Bloqueado')
    await wrapper.get('#configuracion-filter-riesgo').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('1 registro')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('CFG-DEMO-INTEGRACIONES')
    expect(wrapper.text()).not.toContain('CFG-DEMO-GENERAL')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountConfiguracionView()
    await settleView()

    await wrapper.get('#configuracion-filter-texto').setValue('sin coincidencias')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('0 registros')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain(
      'No hay registros de configuracion fixture para los filtros actuales.',
    )
  })

  it('keeps configuration actions disabled and non operational', async () => {
    const wrapper = await mountConfiguracionView()
    await settleView()

    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      wrapper.findAll('button[aria-label="Editar configuracion demo"][disabled]'),
    ).toHaveLength(1)
    expect(wrapper.findAll('button[aria-label="Rotar secreto demo"][disabled]')).toHaveLength(1)
    expect(
      wrapper.findAll('button[aria-label="Valores de configuracion reales bloqueados"][disabled]'),
    ).toHaveLength(1)
    expect(wrapper.find('a.table-icon-action').exists()).toBe(false)
  })
})
