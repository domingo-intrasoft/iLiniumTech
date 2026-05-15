import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import PolizasView from './PolizasView.vue'
import { polizasFixture } from './polizasFixture'

async function mountPolizasView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/polizas', component: PolizasView },
      { path: '/autos-particulares', component: { template: '<div />' } },
      { path: '/polizas/:id', name: 'poliza-detail', component: { template: '<div />' } },
    ],
  })
  await router.push('/polizas')
  await router.isReady()

  return mount(PolizasView, {
    global: {
      plugins: [router],
    },
  })
}

async function settlePolizasView() {
  await flushPromises()
  await flushPromises()
}

describe('PolizasView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the local read-only shell, filters fixture rows, and exposes no runtime metadata', async () => {
    const wrapper = await mountPolizasView()
    await settlePolizasView()

    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local')
    expect(wrapper.text()).toContain('Modo local')
    expect(wrapper.get('.summary-header').text()).toContain(`${polizasFixture.total} polizas`)
    expect(wrapper.findAll('tbody tr')).toHaveLength(polizasFixture.total)
    expect(wrapper.text()).toContain('POL-2026-0001')
    expect(wrapper.text()).toContain('POL-2026-0002')

    await wrapper.get('#polizas-filter-poliza').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settlePolizasView()

    expect(wrapper.get('.summary-header').text()).toContain('1 poliza')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('POL-2026-0002')
    expect(wrapper.text()).toContain('Cliente anonimo 2')
    expect(wrapper.text()).not.toContain('POL-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settlePolizasView()

    expect(wrapper.get('.summary-header').text()).toContain(`${polizasFixture.total} polizas`)
    expect(wrapper.findAll('tbody tr')).toHaveLength(polizasFixture.total)
    expect(wrapper.text()).toContain('POL-2026-0001')
    expect(wrapper.text()).toContain('POL-2026-0002')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toContain('connectionString')
    expect(smokeDom).not.toContain('SELECT *')
    expect(smokeDom).not.toContain('AppBuilder')
    expect(smokeDom).not.toMatch(/IAP_|QueryStatic|ComponentDataSource|Pantalla_Polizas/)
  })
})
