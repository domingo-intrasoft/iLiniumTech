import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import AutosParticularesFilters from './AutosParticularesFilters.vue'
import { autosParticularesCatalogsFixture } from './autosParticularesFixture'

function mountFilters(overrides = {}) {
  return mount(AutosParticularesFilters, {
    props: {
      catalogs: autosParticularesCatalogsFixture,
      loading: false,
      error: null,
      ...overrides,
    },
  })
}

describe('AutosParticularesFilters', () => {
  it('emits reviewed static criteria for Autos Particulares', async () => {
    const wrapper = mountFilters()

    await wrapper.get('#autos-filter-numero').setValue('0002')
    await wrapper.get('#autos-filter-cliente').setValue('Cliente auto')
    await wrapper.get('#autos-filter-estado').setValue('Pendiente')
    await wrapper.get('#autos-filter-compania').setValue('Compania demo')
    await wrapper.get('#autos-filter-fechaEfectoDesde').setValue('2026-02-01')
    await wrapper.get('#autos-filter-fechaEfectoHasta').setValue('2026-02-28')
    await wrapper.get('.primary-action').trigger('click')

    expect(wrapper.emitted('search')?.[0]).toEqual([
      {
        numero: '0002',
        cliente: 'Cliente auto',
        estado: 'Pendiente',
        compania: 'Compania demo',
        fechaEfectoDesde: '2026-02-01',
        fechaEfectoHasta: '2026-02-28',
      },
    ])
  })

  it('keeps text criteria usable while catalog selects are loading', () => {
    const wrapper = mountFilters({ loading: true })

    expect(wrapper.attributes('aria-busy')).toBe('true')
    expect(wrapper.find('[role="status"]').text()).toContain('Cargando catalogos')
    expect(wrapper.get('#autos-filter-numero').attributes('disabled')).toBeUndefined()
    expect(wrapper.get('#autos-filter-estado').attributes('disabled')).toBeDefined()
  })
})
