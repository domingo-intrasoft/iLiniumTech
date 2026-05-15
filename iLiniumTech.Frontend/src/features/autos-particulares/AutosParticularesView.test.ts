import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import AutosParticularesView from './AutosParticularesView.vue'
import { autosParticularesFixture } from './autosParticularesFixture'

async function mountAutosView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/polizas', component: { template: '<div />' } },
      { path: '/autos-particulares', component: AutosParticularesView },
    ],
  })
  await router.push('/autos-particulares')
  await router.isReady()

  return mount(AutosParticularesView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleAutosView() {
  await flushPromises()
  await flushPromises()
}

describe('AutosParticularesView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the Autos read-only shell, filters fixture rows, and exposes no unsafe runtime details', async () => {
    const wrapper = await mountAutosView()
    await settleAutosView()

    expect(wrapper.text()).toContain('Autos Particulares')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local autos')
    expect(wrapper.text()).toContain('Ramo Autos')
    expect(wrapper.text()).toContain('Particulares pendiente UAT')
    expect(wrapper.text()).toContain('Modo local')
    expect(wrapper.get('.summary-header').text()).toContain(
      `${autosParticularesFixture.total} autos particulares`,
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(autosParticularesFixture.total)
    expect(wrapper.text()).toContain('AUTO-2026-0001')
    expect(wrapper.text()).toContain('AUTO-2026-0002')

    await wrapper.get('#autos-filter-numero').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleAutosView()

    expect(wrapper.get('.summary-header').text()).toContain('1 auto particular')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('AUTO-2026-0002')
    expect(wrapper.text()).toContain('SUV familiar')
    expect(wrapper.text()).not.toContain('AUTO-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settleAutosView()

    expect(wrapper.get('.summary-header').text()).toContain(
      `${autosParticularesFixture.total} autos particulares`,
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(autosParticularesFixture.total)

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toContain('connectionString')
    expect(smokeDom).not.toContain('SELECT *')
    expect(smokeDom).not.toContain('AppBuilder')
    expect(smokeDom).not.toMatch(/\b\d{4}\s?[A-Z]{3}\b/)
    expect(smokeDom).not.toMatch(/IAP_|QueryStatic|ComponentDataSource|Pantalla_Polizas|metadata/i)
  })
})
