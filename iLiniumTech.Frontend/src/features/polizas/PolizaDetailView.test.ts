import { flushPromises, mount } from '@vue/test-utils'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { clearAuthSession, hasAuthSession, loginDemo } from '@/features/auth/authSession'

import { polizasDetailFixture } from './polizasFixture'

const mocks = vi.hoisted(() => ({
  route: { path: '/polizas/POL-1001', params: { id: 'POL-1001' as string | string[] } },
  router: { replace: vi.fn() },
  getPolizaById: vi.fn(),
}))

vi.mock('vue-router', () => ({
  useRoute: () => mocks.route,
  useRouter: () => mocks.router,
  RouterLink: {
    props: ['to'],
    template: '<a href="#"><slot /></a>',
  },
}))

vi.mock('./polizasApi', () => ({
  getPolizaById: mocks.getPolizaById,
}))

import PolizaDetailView from './PolizaDetailView.vue'

function axiosError(status: number, data: unknown) {
  return {
    isAxiosError: true,
    response: {
      status,
      data,
    },
  }
}

describe('PolizaDetailView', () => {
  beforeEach(() => {
    mocks.route.path = '/polizas/POL-1001'
    mocks.route.params = { id: 'POL-1001' }
    mocks.router.replace.mockReset()
    mocks.getPolizaById.mockReset()
    clearAuthSession()
  })

  it('renders an honest read-only detail from local data', async () => {
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[0])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(wrapper.get('h1').text()).toBe('POL-2026-0001')
    expect(wrapper.text()).toContain('Inicio')
    expect(wrapper.text()).toContain('Detalle / POL-2026-0001')
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

  it('clears the MVP session when the backend rejects detail with 401', async () => {
    loginDemo({ username: 'domingo', password: 'demo' })
    mocks.getPolizaById.mockRejectedValueOnce(
      axiosError(401, {
        error: {
          message: 'Expired demo-session cookie.',
          correlationId: 'auth-401',
        },
      }),
    )

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(hasAuthSession()).toBe(false)
    expect(wrapper.find('[role="alert"]').text()).toContain(
      'La sesion no esta autorizada para consultar polizas.',
    )
  })
})
