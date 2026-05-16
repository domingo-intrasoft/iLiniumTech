import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import SiniestrosView from './SiniestrosView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings/i

const shellStub = {
  props: ['page'],
  template: `
    <section data-test="mvp-shell">
      <h1>{{ page.title }}</h1>
      <p>{{ page.subtitle }}</p>
      <p>{{ page.status }}</p>
      <button v-for="action in page.actions" :key="action.label" type="button" disabled>
        {{ action.label }}
      </button>
      <article v-for="metric in page.metrics" :key="metric.label">
        {{ metric.label }} {{ metric.value }}
      </article>
      <section>
        <h2>{{ page.scope.title }}</h2>
        <p v-for="item in page.scope.items" :key="item">{{ item }}</p>
      </section>
      <section>
        <h2>{{ page.nextSteps.title }}</h2>
        <p v-for="item in page.nextSteps.items" :key="item">{{ item }}</p>
      </section>
      <section>
        <h2>{{ page.risks.title }}</h2>
        <p v-for="item in page.risks.items" :key="item">{{ item }}</p>
      </section>
      <footer>{{ page.source }}</footer>
    </section>
  `,
}

describe('SiniestrosView', () => {
  it('renders a static blocked MVP page with disabled actions and no runtime markers', () => {
    const wrapper = mount(SiniestrosView, {
      global: {
        stubs: {
          MvpPageShell: shellStub,
        },
      },
    })

    expect(wrapper.get('h1').text()).toBe('Siniestros')
    expect(wrapper.text()).toContain('Sensibilidad Alta')
    expect(wrapper.text()).toContain('Intervinientes')
    expect(wrapper.text()).toContain('EIAC')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(3)
    expect(wrapper.html()).not.toMatch(runtimeMarkers)
  })
})
