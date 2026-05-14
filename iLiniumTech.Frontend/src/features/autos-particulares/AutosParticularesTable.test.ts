import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import AutosParticularesTable from './AutosParticularesTable.vue'
import { autosParticularesFixture } from './autosParticularesFixture'

function mountTable(overrides = {}) {
  return mount(AutosParticularesTable, {
    props: {
      items: autosParticularesFixture.items,
      total: autosParticularesFixture.total,
      loading: false,
      error: null,
      page: 1,
      pageSize: 25,
      ...overrides,
    },
  })
}

describe('AutosParticularesTable', () => {
  it('renders Autos rows with accessible result context', () => {
    const wrapper = mountTable()

    expect(wrapper.find('#autos-results-title').text()).toBe('Resultado')
    expect(wrapper.text()).toContain('4 autos particulares')
    expect(wrapper.findAll('tbody tr')).toHaveLength(4)
    expect(wrapper.text()).toContain('AUTO-2026-0001')
    expect(wrapper.text()).toContain('Turismo compacto')
    expect(wrapper.text()).not.toMatch(/\b\d{4}\s?[A-Z]{3}\b/)
  })

  it('renders an empty state for filters without matches', () => {
    const wrapper = mountTable({ items: [], total: 0 })

    expect(wrapper.find('[role="status"]').text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay autos particulares')
  })

  it('renders an error state and emits retry', async () => {
    const wrapper = mountTable({
      items: [],
      total: 0,
      error: 'No se pudieron cargar los autos particulares.',
    })

    expect(wrapper.find('[role="alert"]').text()).toContain('No se pudo obtener el listado')
    await wrapper.get('.state-action').trigger('click')

    expect(wrapper.emitted('retry')).toHaveLength(1)
  })
})
