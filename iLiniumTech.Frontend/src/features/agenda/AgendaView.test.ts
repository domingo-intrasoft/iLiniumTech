import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import AgendaView from './AgendaView.vue'

const mvpPageShellStub = {
  props: ['page'],
  template: `
    <section data-test="mvp-page-shell">
      <h1>{{ page.title }}</h1>
      <p>{{ page.status }}</p>
      <p>{{ page.subtitle }}</p>
      <p>{{ page.source }}</p>

      <div>
        <button v-for="action in page.actions" :key="action.label" type="button" disabled>
          {{ action.label }}
        </button>
      </div>

      <article v-for="metric in page.metrics" :key="metric.label">
        {{ metric.label }} {{ metric.value }}
      </article>

      <section>
        <h2>{{ page.scope.title }}</h2>
        <ul>
          <li v-for="item in page.scope.items" :key="item">{{ item }}</li>
        </ul>
      </section>

      <section>
        <h2>{{ page.nextSteps.title }}</h2>
        <ul>
          <li v-for="item in page.nextSteps.items" :key="item">{{ item }}</li>
        </ul>
      </section>

      <section>
        <h2>{{ page.risks.title }}</h2>
        <ul>
          <li v-for="item in page.risks.items" :key="item">{{ item }}</li>
        </ul>
      </section>
    </section>
  `,
}

describe('AgendaView', () => {
  it('renders static MVP content with disabled actions and no runtime markers', () => {
    const wrapper = mount(AgendaView, {
      global: {
        stubs: {
          MvpPageShell: mvpPageShellStub,
        },
      },
    })

    expect(wrapper.get('h1').text()).toBe('Agenda')
    expect(wrapper.text()).toContain('Bloqueado externo')
    expect(wrapper.text()).toContain('Agenda read-only')
    expect(wrapper.text()).toContain('drag-and-drop')
    expect(wrapper.text()).toContain('BrokerIntegracionId')
    expect(wrapper.findAll('button[disabled]').map((button) => button.text())).toEqual([
      'Consultar eventos',
      'Crear evento',
      'Reprogramar',
    ])
    expect(wrapper.html()).not.toMatch(/IAP_|QueryStatic|ComponentDataSource/)
  })
})
