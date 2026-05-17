import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { clearAuthSession, hasAuthSession } from './authSession'
import LoginView from './LoginView.vue'

function createTestRouter() {
  return createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: LoginView },
      { path: '/polizas', name: 'polizas', component: { template: '<div>Polizas</div>' } },
    ],
  })
}

async function mountLoginView(redirect = '/polizas', reason?: string) {
  const router = createTestRouter()
  await router.push({
    name: 'login',
    query: {
      redirect,
      ...(reason ? { reason } : {}),
    },
  })
  await router.isReady()

  return {
    router,
    wrapper: mount(LoginView, {
      attachTo: document.body,
      global: {
        plugins: [router],
      },
    }),
  }
}

describe('LoginView', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_BROKER_ID', '')
    document.body.innerHTML = ''
    clearAuthSession()
  })

  it('starts a demo session and redirects to the requested app route', async () => {
    const { router, wrapper } = await mountLoginView('/polizas')

    await wrapper.get('#auth-username').setValue('domingo')
    await wrapper.get('#auth-password').setValue('demo')
    await wrapper.get('form').trigger('submit')
    await flushPromises()

    expect(hasAuthSession()).toBe(true)
    expect(router.currentRoute.value.fullPath).toBe('/polizas')
  })

  it('shows validation feedback when credentials are empty', async () => {
    const { wrapper } = await mountLoginView()
    await flushPromises()

    expect(document.activeElement).toBe(wrapper.get('#auth-username').element)

    await wrapper.get('form').trigger('submit')
    await flushPromises()

    expect(wrapper.get('[role="alert"]').text()).toBe('Introduce usuario y contrasena.')
    expect(wrapper.get('form').attributes('aria-describedby')).toBe('login-error-message')
    expect(wrapper.get('#auth-username').attributes('aria-invalid')).toBe('true')
    expect(wrapper.get('#auth-password').attributes('aria-invalid')).toBe('true')
    expect(hasAuthSession()).toBe(false)
  })

  it('shows generic feedback when the backend session cannot be confirmed', async () => {
    const { wrapper } = await mountLoginView('/polizas', 'session-check-failed')

    expect(wrapper.get('[role="status"]').text()).toBe(
      'No hemos podido confirmar tu sesion. Vuelve a iniciar sesion para continuar con seguridad.',
    )
    expect(wrapper.get('form').attributes('aria-describedby')).toBe('login-session-notice')
  })
})
