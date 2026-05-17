import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import MvpPageShell from './MvpPageShell.vue'
import type { MvpPageDefinition } from './mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Clientes',
  title: 'Clientes',
  subtitle: 'MVP estatico para iniciar desarrollo sin metadata runtime.',
  status: 'MVP inicial',
  source: 'Fuente: documentacion AppBuilder sanitizada.',
  actions: [{ label: 'Buscar', icon: 'pi pi-search' }],
  metrics: [
    { label: 'Pantalla', value: 'Estatica', tone: 'ready' },
    { label: 'API', value: 'Pendiente', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance',
    items: ['Ruta protegida y visible en menu.'],
  },
  nextSteps: {
    title: 'Siguiente paso',
    items: ['Cerrar SDD antes de conectar datos reales.'],
  },
  risks: {
    title: 'Riesgos',
    items: ['No consumir metadata AppBuilder en runtime.'],
  },
}

describe('MvpPageShell', () => {
  it('renders static page content without AppBuilder runtime markers', () => {
    const wrapper = mount(MvpPageShell, {
      props: { page },
      global: {
        stubs: {
          AppShell: { template: '<div><slot /></div>', props: ['sectionTitle'] },
        },
      },
    })

    expect(wrapper.get('h1').text()).toBe('Clientes')
    expect(wrapper.text()).toContain('MVP estatico')
    expect(wrapper.get('section.mvp-module-page').attributes('aria-labelledby')).toBe(
      'Clientes-title',
    )
    expect(wrapper.find('main.mvp-module-page').exists()).toBe(false)
    expect(wrapper.findAll('button[disabled]')).toHaveLength(1)
    expect(wrapper.text()).not.toMatch(/IAP_|QueryStatic|ComponentDataSource/)
  })
})
