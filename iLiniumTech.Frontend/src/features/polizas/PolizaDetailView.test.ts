import { flushPromises, mount } from '@vue/test-utils'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { polizasDetailFixture } from './polizasFixture'

const mocks = vi.hoisted(() => ({
  route: { params: { id: 'POL-1001' as string | string[] } },
  getPolizaById: vi.fn(),
}))

vi.mock('vue-router', () => ({
  useRoute: () => mocks.route,
  RouterLink: {
    props: ['to'],
    template: '<a href="#"><slot /></a>',
  },
}))

vi.mock('./polizasApi', () => ({
  getPolizaById: mocks.getPolizaById,
}))

import PolizaDetailView from './PolizaDetailView.vue'

describe('PolizaDetailView', () => {
  beforeEach(() => {
    mocks.route.params = { id: 'POL-1001' }
    mocks.getPolizaById.mockReset()
  })

  it('renders an honest read-only detail from local data', async () => {
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[0])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(wrapper.get('h1').text()).toBe('POL-2026-0001')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local')
    expect(wrapper.text()).toContain('Sin workflows heredados')
  })

  it('shows a loading state while the detail request is pending', () => {
    mocks.getPolizaById.mockReturnValueOnce(new Promise(() => undefined))

    const wrapper = mount(PolizaDetailView)

    expect(wrapper.find('[aria-busy="true"]').exists()).toBe(true)
    expect(wrapper.find('[role="status"]').attributes('aria-label')).toBe(
      'Cargando detalle de poliza',
    )
  })

  it('sanitizes backend failures in the detail state', async () => {
    mocks.getPolizaById.mockRejectedValueOnce(new Error('SELECT * FROM Pantalla_Polizas'))

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(wrapper.find('[role="alert"]').text()).toContain('No se pudo cargar la poliza.')
    expect(wrapper.text()).not.toContain('SELECT *')
  })
})
