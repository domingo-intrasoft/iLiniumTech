import { mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { describe, expect, it } from 'vitest'

import AppSideMenu from './AppSideMenu.vue'

function createTestRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/polizas', component: { template: '<div />' } },
      { path: '/polizas/:id', component: { template: '<div />' } },
      { path: '/autos-particulares', component: { template: '<div />' } },
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
  it('exposes static navigation to existing screens without runtime metadata', async () => {
    const wrapper = await mountMenu()

    expect(wrapper.get('.side-nav a[href="/polizas"]').text()).toContain('Polizas')
    expect(wrapper.get('.side-nav a[href="/autos-particulares"]').text()).toContain(
      'Autos Particulares',
    )
    expect(wrapper.get('.side-nav [aria-current="page"]').text()).toContain('Polizas')
    expect(wrapper.text()).not.toMatch(/IAP_|QueryStatic|ComponentDataSource|metadata/i)
  })

  it('marks nested Autos Particulares route as current', async () => {
    const wrapper = await mountMenu('/autos-particulares')

    expect(wrapper.get('[aria-current="page"]').text()).toContain('Autos Particulares')
    expect(wrapper.get('.has-children').classes()).toContain('active')
  })

  it('keeps Polizas active in detail routes', async () => {
    const wrapper = await mountMenu('/polizas/POL-1001')

    expect(wrapper.get('.has-children').classes()).toContain('active')
    expect(wrapper.find('[aria-current="page"]').exists()).toBe(false)
  })
})
