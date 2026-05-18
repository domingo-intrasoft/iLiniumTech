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

    expect(wrapper.find('#polizas-results-title').text()).toBe('Polizas')
    expect(wrapper.text()).toContain('2 polizas')
    expect(wrapper.get('caption').text()).toBe('Listado de polizas. Mostrando 1-2 de 2.')
    expect(wrapper.get('table').classes()).toContain('appbuilder-table')
    expect(wrapper.findAll('thead th').map((header) => header.text())).toEqual([
      'Seleccion',
      'Acciones',
      'Cia.',
      'Poliza',
      'Certif.',
      'N. Documento',
      'Cliente',
      'Situacion',
      'Ramo',
      'Riesgo/Matric.',
    ])
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.find('.policy-status-badge').text()).toBe('En Vigor')
    expect(wrapper.find('.policy-number-link').text()).toBe('POL-2026-0001')
    expect(wrapper.findAll('.pending-data-cell')).toHaveLength(6)
  })

  it('renders a loading table without losing column structure', () => {
    const wrapper = mountTable({ items: [], total: 0, loading: true })

    expect(wrapper.attributes('aria-busy')).toBe('true')
    expect(wrapper.find('[role="status"]').exists()).toBe(true)
    expect(wrapper.get('caption').text()).toBe('Cargando listado de polizas.')
    expect(wrapper.findAll('thead th')).toHaveLength(10)
    expect(wrapper.findAll('tbody tr')).toHaveLength(5)
  })

  it('renders explicit detail actions for each row', () => {
    const wrapper = mountTable()

    const detailActions = wrapper.findAll('a.table-icon-action')

    expect(detailActions).toHaveLength(polizasFixture.items.length)
    expect(detailActions[0].attributes('aria-label')).toContain('Ver detalle de poliza')
  })

  it('enables write actions only for MVP rows with stable numeric ids', async () => {
    const wrapper = mountTable({
      items: [
        { ...polizasFixture.items[0], id: '101', numero: 'ILMVP-0001' },
        polizasFixture.items[1],
      ],
      canUpdateMvp: true,
      canDeleteMvp: true,
    })

    const editActions = wrapper.findAll('button[aria-label^="Editar poliza"]')
    const deleteActions = wrapper.findAll('button[aria-label^="Eliminar poliza"]')

    expect(wrapper.text()).toContain('Escritura limitada a registros ILMVP-.')
    expect(editActions[0].attributes('disabled')).toBeUndefined()
    expect(deleteActions[0].attributes('disabled')).toBeUndefined()
    expect(editActions[1].attributes('disabled')).toBeDefined()
    expect(deleteActions[1].attributes('disabled')).toBeDefined()

    await editActions[0].trigger('click')
    await deleteActions[0].trigger('click')

    expect(wrapper.emitted('edit')?.[0]?.[0]).toMatchObject({ id: '101', numero: 'ILMVP-0001' })
    expect(wrapper.emitted('delete')?.[0]?.[0]).toMatchObject({ id: '101', numero: 'ILMVP-0001' })
  })

  it('renders disabled detail actions with the access reason when detail permission is denied', () => {
    const wrapper = mountTable({
      canOpenDetail: false,
      detailUnavailableMessage: 'La sesion actual no tiene permiso para abrir el detalle.',
    })

    const disabledActions = wrapper.findAll('button[aria-label^="Detalle no disponible"]')

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
