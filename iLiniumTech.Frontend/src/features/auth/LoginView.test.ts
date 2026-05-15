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

async function mountLoginView(redirect = '/polizas') {
  const router = createTestRouter()
  await router.push({ name: 'login', query: { redirect } })
  await router.isReady()

  return {
    router,
    wrapper: mount(LoginView, {
      global: {
        plugins: [router],
      },
    }),
  }
}

describe('LoginView', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_BROKER_ID', '')
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

    await wrapper.get('form').trigger('submit')
    await flushPromises()

    expect(wrapper.get('[role="alert"]').text()).toBe('Introduce usuario y contrasena.')
    expect(hasAuthSession()).toBe(false)
  })
})
