import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import PolizasFilters from './PolizasFilters.vue'
import { polizasCatalogsFixture } from './polizasFixture'

function mountFilters(overrides = {}) {
  return mount(PolizasFilters, {
    props: {
      catalogs: polizasCatalogsFixture,
      loading: false,
      error: null,
      ...overrides,
    },
  })
}

describe('PolizasFilters', () => {
  it('emits the reviewed static search criteria', async () => {
    const wrapper = mountFilters()

    await wrapper.get('#polizas-filter-poliza').setValue('0002')
    await wrapper.get('#polizas-filter-nombreCompleto').setValue('Cliente anonimo')
    await wrapper.get('#polizas-filter-tipoPoliza').setValue('Vigor')
    await wrapper.get('#polizas-filter-cia').setValue('Compania demo')
    await wrapper.get('#polizas-filter-ramo').setValue('Autos')
    await wrapper.get('.primary-action').trigger('click')

    expect(wrapper.emitted('search')?.[0]).toEqual([
      {
        numero: '0002',
        cliente: 'Cliente anonimo',
        estado: 'Vigor',
        compania: 'Compania demo',
        ramo: 'Autos',
        fechaEfectoDesde: '',
        fechaEfectoHasta: '',
      },
    ])
  })

  it('exposes catalog loading errors without blocking text search', () => {
    const wrapper = mountFilters({ error: 'No se pudieron cargar los catalogos de polizas.' })

    expect(wrapper.find('[role="alert"]').text()).toContain('catalogos')
    expect(wrapper.get('#polizas-filter-poliza').attributes('aria-label')).toBe('Poliza')
  })
})
