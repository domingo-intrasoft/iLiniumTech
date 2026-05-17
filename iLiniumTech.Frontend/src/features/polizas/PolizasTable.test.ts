import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import PolizasTable from './PolizasTable.vue'
import { polizasFixture } from './polizasFixture'

function mountTable(overrides = {}) {
  return mount(PolizasTable, {
    props: {
      items: polizasFixture.items,
      total: polizasFixture.total,
      loading: false,
      error: null,
      page: 1,
      pageSize: 25,
      ...overrides,
    },
    global: {
      stubs: {
        RouterLink: {
          props: ['to'],
          template: '<a href="#"><slot /></a>',
        },
      },
    },
  })
}

describe('PolizasTable', () => {
  it('renders rows with accessible result context', () => {
    const wrapper = mountTable()

    expect(wrapper.find('#polizas-results-title').text()).toBe('Resultado')
    expect(wrapper.text()).toContain('2 polizas')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
  })

  it('renders a loading table without losing column structure', () => {
    const wrapper = mountTable({ items: [], total: 0, loading: true })

    expect(wrapper.attributes('aria-busy')).toBe('true')
    expect(wrapper.find('[role="status"]').exists()).toBe(true)
    expect(wrapper.findAll('thead th')).toHaveLength(10)
    expect(wrapper.findAll('tbody tr')).toHaveLength(5)
  })

  it('renders explicit detail actions for each row', () => {
    const wrapper = mountTable()

    const detailActions = wrapper.findAll('.table-icon-action')

    expect(detailActions).toHaveLength(polizasFixture.items.length)
    expect(detailActions[0].attributes('aria-label')).toContain('Ver detalle de poliza')
  })

  it('renders disabled detail actions with the access reason when detail permission is denied', () => {
    const wrapper = mountTable({
      canOpenDetail: false,
      detailUnavailableMessage: 'La sesion actual no tiene permiso para abrir el detalle.',
    })

    const disabledActions = wrapper.findAll('button.table-icon-action')

    expect(wrapper.get('#polizas-detail-unavailable-message').text()).toContain(
      'La sesion actual no tiene permiso para abrir el detalle.',
    )
    expect(disabledActions).toHaveLength(polizasFixture.items.length)
    expect(disabledActions[0].attributes('disabled')).toBeDefined()
    expect(disabledActions[0].attributes('aria-label')).toContain('Detalle no disponible')
    expect(disabledActions[0].attributes('aria-describedby')).toBe(
      'polizas-detail-unavailable-message',
    )
    expect(wrapper.find('a.table-icon-action').exists()).toBe(false)
    expect(wrapper.find('a.table-link').exists()).toBe(false)
  })

  it('renders an error state and emits retry', async () => {
    const wrapper = mountTable({ items: [], total: 0, error: 'No se pudieron cargar las polizas.' })

    expect(wrapper.find('[role="alert"]').text()).toContain('No se pudo obtener el listado')

    await wrapper.get('.state-action').trigger('click')

    expect(wrapper.emitted('retry')).toHaveLength(1)
  })

  it('renders an empty state for filters without matches', () => {
    const wrapper = mountTable({ items: [], total: 0 })

    expect(wrapper.find('[role="status"]').text()).toContain('Sin resultados')
  })
})
