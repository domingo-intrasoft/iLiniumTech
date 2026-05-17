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

  it('marks placeholder topbar actions as unavailable in the MVP', () => {
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

    const notifications = wrapper.get(
      'button[aria-label="Notificaciones no disponibles en el MVP"]',
    )
    const configuration = wrapper.get(
      'button[aria-label="Configuracion no disponible desde esta accion"]',
    )

    expect(notifications.attributes('disabled')).toBeDefined()
    expect(notifications.attributes('title')).toContain('pendientes de SDD')
    expect(configuration.attributes('disabled')).toBeDefined()
    expect(configuration.attributes('title')).toContain('bloqueada por SDD')
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

  it('moves focus to the target content when the skip link is activated', async () => {
    const host = document.createElement('div')
    document.body.appendChild(host)

    const wrapper = mount(AppShell, {
      attachTo: host,
      props: {
        contentId: 'content',
        sectionTitle: 'Polizas',
        sessionLabel: 'Modo local',
      },
      slots: {
        default: '<section id="content">Contenido principal</section>',
      },
      global: {
        stubs: {
          AppSideMenu: true,
        },
      },
    })

    await wrapper.get('.skip-link').trigger('click')

    const content = host.querySelector<HTMLElement>('#content')
    expect(content).not.toBeNull()
    expect(content?.getAttribute('tabindex')).toBe('-1')
    expect(document.activeElement).toBe(content)

    wrapper.unmount()
    host.remove()
  })
})
