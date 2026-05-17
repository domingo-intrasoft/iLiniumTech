import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import AppShell from './AppShell.vue'

describe('AppShell', () => {
  it('renders broker selector only when backend exposes useful broker options', async () => {
    const wrapper = mount(AppShell, {
      props: {
        contentId: 'content',
        sectionTitle: 'Polizas',
        sessionLabel: 'Broker 42',
        brokerOptions: [42, 84],
        activeBrokerId: 42,
      },
      global: {
        stubs: {
          AppSideMenu: true,
        },
      },
    })

    const selector = wrapper.get('select[aria-label="Broker activo"]')

    expect(selector.element.value).toBe('42')
    await selector.setValue('84')

    expect(wrapper.emitted('brokerChange')).toEqual([[84]])
  })

  it('keeps broker selector hidden when there are no useful allowed brokers', () => {
    const wrapper = mount(AppShell, {
      props: {
        contentId: 'content',
        sectionTitle: 'Polizas',
        sessionLabel: 'Modo local',
      },
      global: {
        stubs: {
          AppSideMenu: true,
        },
      },
    })

    expect(wrapper.find('select[aria-label="Broker activo"]').exists()).toBe(false)
  })

  it('toggles the static side menu from the topbar control', async () => {
    const wrapper = mount(AppShell, {
      props: {
        contentId: 'content',
        sectionTitle: 'Polizas',
        sessionLabel: 'Modo local',
      },
      global: {
        stubs: {
          AppSideMenu: true,
        },
      },
    })

    const menuButton = wrapper.get('button[aria-controls="app-side-menu"]')
    const sideMenu = wrapper.get('#app-side-menu')

    expect(wrapper.get('main').classes()).not.toContain('menu-collapsed')
    expect(sideMenu.attributes('aria-hidden')).toBe('false')
    expect(sideMenu.attributes('inert')).toBeUndefined()
    expect(menuButton.attributes('aria-expanded')).toBe('true')
    expect(menuButton.attributes('aria-label')).toBe('Ocultar menu')

    await menuButton.trigger('click')

    expect(wrapper.get('main').classes()).toContain('menu-collapsed')
    expect(sideMenu.attributes('aria-hidden')).toBe('true')
    expect(sideMenu.attributes('inert')).toBeDefined()
    expect(menuButton.attributes('aria-expanded')).toBe('false')
    expect(menuButton.attributes('aria-label')).toBe('Mostrar menu')

    await menuButton.trigger('click')

    expect(wrapper.get('main').classes()).not.toContain('menu-collapsed')
    expect(sideMenu.attributes('aria-hidden')).toBe('false')
    expect(sideMenu.attributes('inert')).toBeUndefined()
    expect(menuButton.attributes('aria-expanded')).toBe('true')
  })
})
