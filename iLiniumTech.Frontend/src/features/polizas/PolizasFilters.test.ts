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
    expect(wrapper.get('#polizas-filter-tipoPoliza').attributes('aria-describedby')).toBe(
      'polizas-catalog-error',
    )
  })

  it('keeps text criteria usable while catalog selects are loading', () => {
    const wrapper = mountFilters({ loading: true })

    expect(wrapper.attributes('aria-busy')).toBe('true')
    expect(wrapper.find('[role="status"]').text()).toContain('Cargando catalogos')
    expect(wrapper.get('#polizas-filter-poliza').attributes('disabled')).toBeUndefined()
    expect(wrapper.get('#polizas-filter-tipoPoliza').attributes('disabled')).toBeDefined()
  })

  it('marks filters outside the current API contract as unavailable', () => {
    const wrapper = mountFilters()

    expect(wrapper.get('#polizas-filter-poliza').attributes('disabled')).toBeUndefined()
    expect(wrapper.get('#polizas-filter-riesgo').attributes('disabled')).toBeDefined()
    expect(wrapper.get('#polizas-filter-riesgo').attributes('aria-describedby')).toBe(
      'polizas-filter-unsupported',
    )
    expect(wrapper.get('#polizas-filter-unsupported').text()).toContain('contrato API')
    expect(wrapper.get('label[for="polizas-filter-riesgo"]').attributes('title')).toContain(
      'Pendiente de contrato API',
    )
  })

  it('explains inherited blocked filter actions', () => {
    const wrapper = mountFilters()
    const buttons = wrapper.findAll('button')
    const saveSearchButton = buttons.find((button) => button.text().includes('Guardar busqueda'))
    const advancedButton = buttons.find((button) => button.text().includes('Avanzada'))

    expect(wrapper.get('#polizas-filter-blocked-actions').text()).toContain('SDD')
    expect(saveSearchButton?.attributes('title')).toContain('contrato API')
    expect(saveSearchButton?.attributes('aria-describedby')).toBe('polizas-filter-blocked-actions')
    expect(advancedButton?.attributes('title')).toContain('UAT')
    expect(
      wrapper
        .get('button[aria-label="Agregar filtro no disponible en el MVP"]')
        .attributes('title'),
    ).toContain('permisos')
    expect(
      wrapper
        .get('button[aria-label="Opciones de filtro para Poliza no disponibles en el MVP"]')
        .attributes('title'),
    ).toContain('Opciones avanzadas')
  })

  it('blocks search actions when the execution context is not ready', async () => {
    const wrapper = mountFilters({
      searchDisabled: true,
      blockedMessage: 'Configura un broker para consultar polizas.',
    })

    expect(wrapper.get('#polizas-filter-poliza').attributes('disabled')).toBeDefined()
    expect(wrapper.get('.primary-action').attributes('disabled')).toBeDefined()
    expect(wrapper.get('.primary-action').attributes('aria-describedby')).toBe(
      'polizas-search-blocked',
    )
    expect(wrapper.get('#polizas-search-blocked').text()).toContain('Configura un broker')

    await wrapper.get('.primary-action').trigger('click')

    expect(wrapper.emitted('search')).toBeUndefined()
  })
})
