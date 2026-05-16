import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import PropuestasView from './PropuestasView.vue'

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

describe('PropuestasView', () => {
  it('renders static MVP content with disabled actions and no runtime markers', () => {
    const wrapper = mount(PropuestasView, {
      global: {
        stubs: {
          MvpPageShell: mvpPageShellStub,
        },
      },
    })

    expect(wrapper.get('h1').text()).toBe('Propuestas')
    expect(wrapper.text()).toContain('sin metadata concreta')
    expect(wrapper.text()).toContain('Propuestas sea Solicitudes')
    expect(wrapper.text()).toContain('read-only futura')
    expect(wrapper.text()).toContain('multi-tenant')
    expect(wrapper.findAll('button[disabled]').map((button) => button.text())).toEqual([
      'Listar propuestas',
      'Crear propuesta',
      'Convertir a poliza',
    ])
    expect(wrapper.html()).not.toMatch(/IAP_|QueryStatic|ComponentDataSource/)
  })
})
