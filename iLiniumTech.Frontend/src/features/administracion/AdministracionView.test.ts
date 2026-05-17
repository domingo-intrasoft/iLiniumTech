import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import AdministracionView from './AdministracionView.vue'

const runtimeMarkers = /AppBuilder|IAP_|QueryStatic|ComponentDataSource|SELECT \*/i
const realDataMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|Bearer\s+[A-Za-z0-9]|[A-Z]{2}\d{2}[A-Z0-9]{11,30}|\b\d{8}[A-Z]\b|@/i

async function mountAdministracionView() {
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
      { path: '/administracion', component: AdministracionView },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/administracion')
  await router.isReady()

  return mount(AdministracionView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleView() {
  await flushPromises()
  await flushPromises()
}

describe('AdministracionView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only fixture shell with sanitized administrative rows', async () => {
    const wrapper = await mountAdministracionView()
    await settleView()

    expect(wrapper.text()).toContain('Administracion')
    expect(wrapper.text()).toContain('Superficie sensible')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Usuarios reales y permisos reales bloqueados')
    expect(wrapper.get('.summary-header').text()).toContain('4 registros')
    expect(wrapper.get('caption').text()).toContain(
      'Administracion fixture read-only: 1-2 de 4 registros',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('ADM-DEMO-ACCESOS')
    expect(wrapper.text()).toContain('ADM-DEMO-AUDITORIA')

    const dom = wrapper.html()
    expect(dom).not.toMatch(runtimeMarkers)
    expect(dom).not.toMatch(realDataMarkers)
  })

  it('filters by local text, area, status, and risk', async () => {
    const wrapper = await mountAdministracionView()
    await settleView()

    await wrapper.get('#administracion-filter-texto').setValue('auditoria')
    await wrapper.get('#administracion-filter-area').setValue('Auditoria')
    await wrapper.get('#administracion-filter-estado').setValue('Read-only')
    await wrapper.get('#administracion-filter-riesgo').setValue('Medio')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('1 registro')
    expect(wrapper.get('caption').text()).toContain(
      'Administracion fixture read-only: 1-1 de 1 registro',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('ADM-DEMO-AUDITORIA')
    expect(wrapper.text()).not.toContain('ADM-DEMO-ACCESOS')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountAdministracionView()
    await settleView()

    await wrapper.get('#administracion-filter-texto').setValue('sin coincidencias')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('0 registros')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain(
      'No hay registros administrativos fixture para los filtros actuales.',
    )
  })

  it('keeps administrative actions disabled and non operational', async () => {
    const wrapper = await mountAdministracionView()
    await settleView()

    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(wrapper.findAll('button[aria-label="Crear usuario demo"][disabled]')).toHaveLength(1)
    expect(wrapper.findAll('button[aria-label="Editar permisos demo"][disabled]')).toHaveLength(1)
    expect(
      wrapper.findAll('button[aria-label="Datos administrativos reales bloqueados"][disabled]'),
    ).toHaveLength(1)
    expect(wrapper.find('a.table-icon-action').exists()).toBe(false)
  })
})
