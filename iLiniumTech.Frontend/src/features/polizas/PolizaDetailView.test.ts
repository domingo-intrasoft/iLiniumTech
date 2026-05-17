import { flushPromises, mount } from '@vue/test-utils'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { clearAuthSession, hasAuthSession, loginDemo } from '@/features/auth/authSession'
import type { SessionContext } from '@/services/session'

import { polizasDetailFixture } from './polizasFixture'

const mocks = vi.hoisted(() => ({
  route: {
    path: '/polizas/POL-1001',
    params: { id: 'POL-1001' as string | string[] },
    query: {} as Record<string, string>,
  },
  router: { replace: vi.fn() },
  session: { value: null as SessionContext | null },
  sessionLoading: { value: false },
  sessionError: { value: null as string | null },
  sessionErrorKind: { value: null as 'unauthenticated' | null },
  loadSession: vi.fn(),
  switchSessionBroker: vi.fn(),
  getPolizaById: vi.fn(),
}))

vi.mock('vue-router', () => ({
  useRoute: () => mocks.route,
  useRouter: () => mocks.router,
  RouterLink: {
    props: ['to'],
    template:
      '<a href="#" :data-to-name="to.name" :data-query-numero="to.query?.numero ?? \'\'" :data-query-page="to.query?.page ?? \'\'" :data-query-page-size="to.query?.pageSize ?? \'\'"><slot /></a>',
  },
}))

vi.mock('@/services/session', () => ({
  clearSessionContext: vi.fn(() => {
    mocks.session.value = null
    mocks.sessionError.value = null
    mocks.sessionErrorKind.value = null
  }),
  useSession: () => ({
    session: mocks.session,
    loading: mocks.sessionLoading,
    error: mocks.sessionError,
    errorKind: mocks.sessionErrorKind,
    loadSession: mocks.loadSession,
  }),
  switchSessionBroker: mocks.switchSessionBroker,
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

function backendSession(overrides: Partial<SessionContext> = {}): SessionContext {
  return {
    brokerId: 42,
    entityMainId: 42,
    userId: 7,
    profileId: 9,
    profileTypeId: 'mvp-profile',
    isAdmin: false,
    headerExecutionContextEnabled: true,
    polizasExecutionContextRequired: true,
    permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
    ...overrides,
  }
}

function enableBackendDemoSessionMode() {
  vi.stubEnv('VITE_USE_BACKEND', 'true')
  vi.stubEnv('VITE_AUTH_MODE', 'demo-session')
}

describe('PolizaDetailView', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    mocks.route.path = '/polizas/POL-1001'
    mocks.route.params = { id: 'POL-1001' }
    mocks.route.query = {}
    mocks.router.replace.mockReset()
    mocks.session.value = null
    mocks.sessionLoading.value = false
    mocks.sessionError.value = null
    mocks.sessionErrorKind.value = null
    mocks.loadSession.mockReset()
    mocks.switchSessionBroker.mockReset()
    mocks.getPolizaById.mockReset()
    clearAuthSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
  })

  it('renders an honest read-only detail from local data', async () => {
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[0])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(wrapper.get('h1').text()).toBe('POL-2026-0001')
    expect(wrapper.text()).toContain('Inicio')
    expect(wrapper.get('.detail-breadcrumb').attributes('aria-label')).toBe('Ruta de poliza')
    expect(wrapper.get('.detail-breadcrumb').text()).toBe('Polizas/Detalle/POL-2026-0001')
    expect(wrapper.findAll('.detail-breadcrumb [aria-hidden="true"]')).toHaveLength(2)
    expect(wrapper.get('.detail-breadcrumb [aria-current="page"]').text()).toBe('POL-2026-0001')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local')
    expect(wrapper.text()).toContain('Sin workflows heredados')
  })

  it('preserves the original polizas query in the back link', async () => {
    mocks.route.query = { numero: '0002', page: '2', pageSize: '10' }
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[1])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    const backLink = wrapper.get('.detail-back')
    expect(backLink.attributes('data-to-name')).toBe('polizas')
    expect(backLink.attributes('data-query-numero')).toBe('0002')
    expect(backLink.attributes('data-query-page')).toBe('2')
    expect(backLink.attributes('data-query-page-size')).toBe('10')
    expect(backLink.attributes('aria-label')).toBe('Volver al listado de polizas')
    expect(backLink.attributes('aria-describedby')).toBe('poliza-detail-back-context')
    expect(backLink.attributes('title')).toContain('conservando filtros')
    expect(wrapper.get('#poliza-detail-back-context').text()).toContain(
      'Conserva los filtros y la paginacion de origen',
    )
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
    enableBackendDemoSessionMode()
    mocks.loadSession.mockResolvedValueOnce(backendSession())
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

  it('does not call detail API when backend session cannot be validated', async () => {
    enableBackendDemoSessionMode()
    loginDemo({ username: 'domingo', password: 'demo' })
    mocks.sessionError.value =
      'La sesion no esta autorizada para consultar polizas. Inicia sesion de nuevo si el problema continua.'
    mocks.sessionErrorKind.value = 'unauthenticated'
    mocks.loadSession.mockResolvedValueOnce(null)

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(mocks.getPolizaById).not.toHaveBeenCalled()
    expect(hasAuthSession()).toBe(false)
    expect(wrapper.find('[role="alert"]').text()).toContain(
      'La sesion no esta autorizada para consultar polizas.',
    )
  })

  it('does not call detail API when backend session is missing broker context', async () => {
    enableBackendDemoSessionMode()
    mocks.session.value = backendSession({ brokerId: null, entityMainId: null })
    mocks.loadSession.mockResolvedValueOnce(mocks.session.value)

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(mocks.getPolizaById).not.toHaveBeenCalled()
    expect(wrapper.get('.environment-badge').text()).toBe('Broker requerido')
    expect(wrapper.get('.environment-badge').classes()).toContain('warning')
    expect(wrapper.find('[role="alert"]').text()).toContain(
      'Configura un broker para consultar polizas.',
    )
  })

  it('does not call detail API when backend session lacks polizas.detail permission', async () => {
    enableBackendDemoSessionMode()
    mocks.session.value = backendSession({ permissions: ['polizas.read'] })
    mocks.loadSession.mockResolvedValueOnce(mocks.session.value)

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(mocks.getPolizaById).not.toHaveBeenCalled()
    expect(wrapper.get('.environment-badge').text()).toBe('Broker 42')
    expect(wrapper.get('.environment-badge').classes()).toContain('warning')
    expect(wrapper.find('[role="alert"]').text()).toContain(
      'La sesion actual no tiene permiso para consultar el detalle de polizas.',
    )
  })

  it('shows the active backend broker in the detail shell when the session is valid', async () => {
    enableBackendDemoSessionMode()
    mocks.session.value = backendSession({ brokerId: 84, entityMainId: 84 })
    mocks.loadSession.mockResolvedValueOnce(mocks.session.value)
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[0])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    expect(wrapper.get('.environment-badge').text()).toBe('Broker 84')
    expect(wrapper.get('.environment-badge').classes()).not.toContain('warning')
  })

  it('redirects to the polizas list after switching broker from detail', async () => {
    enableBackendDemoSessionMode()
    loginDemo({ username: 'domingo', password: 'demo' })
    const initialSession = backendSession({
      allowedBrokerIds: [42, 84],
      authMode: 'DemoSession',
    })
    const switchedSession = backendSession({
      brokerId: 84,
      entityMainId: 84,
      allowedBrokerIds: [42, 84],
      authMode: 'DemoSession',
    })
    mocks.session.value = initialSession
    mocks.loadSession.mockResolvedValue(initialSession)
    mocks.switchSessionBroker.mockResolvedValueOnce(switchedSession)
    mocks.getPolizaById.mockResolvedValueOnce(polizasDetailFixture[0])

    const wrapper = mount(PolizaDetailView)
    await flushPromises()

    await wrapper.get('select[aria-label="Broker activo"]').setValue('84')
    await flushPromises()

    expect(mocks.switchSessionBroker).toHaveBeenCalledWith(84)
    expect(mocks.router.replace).toHaveBeenCalledWith({ name: 'polizas' })
  })
})
