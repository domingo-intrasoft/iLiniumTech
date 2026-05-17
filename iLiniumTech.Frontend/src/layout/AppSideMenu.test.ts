import { mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { beforeEach, describe, expect, it } from 'vitest'

import { useSession } from '@/services/session'

import AppSideMenu from './AppSideMenu.vue'

function createTestRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: { template: '<div />' } },
      { path: '/propuestas', component: { template: '<div />' } },
      { path: '/polizas', component: { template: '<div />' } },
      { path: '/polizas/:id', component: { template: '<div />' } },
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
}

async function mountMenu(path = '/polizas') {
  const router = createTestRouter()
  await router.push(path)
  await router.isReady()

  return mount(AppSideMenu, {
    global: {
      plugins: [router],
    },
  })
}

describe('AppSideMenu', () => {
  beforeEach(() => {
    useSession().resetSession()
  })

  it('exposes static navigation to existing screens without runtime metadata', async () => {
    const wrapper = await mountMenu()

    expect(wrapper.get('.side-nav a[href="/polizas"]').text()).toContain('Polizas')
    expect(wrapper.find('.side-nav a[href="/autos-particulares"]').exists()).toBe(false)
    expect(wrapper.get('.side-subnav .side-nav-disabled').text()).toContain('Autos Particulares')
    expect(wrapper.get('.side-nav a[href="/clientes"]').text()).toContain('Clientes')
    expect(wrapper.get('.side-nav a[href="/recibos"]').text()).toContain('Recibos')
    expect(wrapper.get('.side-subnav a[href="/polizas/flotas"]').text()).toContain('Flotas')
    expect(wrapper.get('.side-nav a[href="/polizas"] .nav-status-dot').classes()).toContain(
      'status-operational',
    )
    expect(wrapper.get('.side-nav a[href="/clientes"] .nav-status-dot').classes()).toContain(
      'status-fixture',
    )
    expect(
      wrapper.get('.side-subnav a[href="/polizas/flotas"] .nav-status-dot').classes(),
    ).toContain('status-blockedSdd')
    expect(wrapper.get('.side-status-legend').attributes('aria-label')).toBe(
      'Leyenda de estados del menu',
    )
    expect(wrapper.findAll('.side-status-legend .nav-status-dot')).toHaveLength(4)
    expect(wrapper.get('.side-status-legend .status-operational').attributes('title')).toContain(
      'Operativo',
    )
    expect(wrapper.get('.side-status-legend .status-fixture').attributes('title')).toContain(
      'Fixture',
    )
    expect(wrapper.get('.side-status-legend').text()).toContain('OK')
    expect(wrapper.get('.side-status-legend').text()).toContain('FIC')
    expect(wrapper.get('.side-status-legend').text()).toContain('PA')
    expect(wrapper.get('.side-status-legend').text()).toContain('SDD')
    expect(wrapper.get('.side-nav [aria-current="page"]').text()).toContain('Polizas')
    expect(wrapper.text()).not.toMatch(/IAP_|QueryStatic|ComponentDataSource|metadata/i)
  })

  it('keeps parked Autos Particulares disabled even when the direct route is open', async () => {
    const wrapper = await mountMenu('/autos-particulares')

    expect(wrapper.find('[aria-current="page"]').exists()).toBe(false)
    expect(wrapper.get('.side-subnav .side-nav-disabled').text()).toContain('Autos Particulares')
    expect(wrapper.get('.side-subnav .side-nav-disabled').attributes('title')).toContain(
      'Aparcado por decision de producto',
    )
    expect(wrapper.get('.side-subnav .side-nav-disabled .nav-status-dot').classes()).toContain(
      'status-parked',
    )
    expect(wrapper.get('.has-children').classes()).toContain('active')
  })

  it('keeps Polizas active in detail routes', async () => {
    const wrapper = await mountMenu('/polizas/POL-1001')

    expect(wrapper.get('.has-children').classes()).toContain('active')
    expect(wrapper.find('[aria-current="page"]').exists()).toBe(false)
  })

  it('marks MVP pages active from the static menu', async () => {
    const wrapper = await mountMenu('/clientes')

    expect(wrapper.get('.side-nav [aria-current="page"]').text()).toContain('Clientes')
  })
})
