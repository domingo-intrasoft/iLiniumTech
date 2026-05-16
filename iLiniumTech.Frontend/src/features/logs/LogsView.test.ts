import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import LogsView from './LogsView.vue'

const runtimeMarkers = /AppBuilder|IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*/i

function mountView() {
  return mount(LogsView, {
    global: {
      stubs: {
        MvpPageShell: {
          props: ['page'],
          template: `
            <section>
              <h1>{{ page.title }}</h1>
              <p>{{ page.status }}</p>
              <p>{{ page.subtitle }}</p>
              <button v-for="action in page.actions" :key="action.label" type="button" disabled>
                {{ action.label }}
              </button>
              <span v-for="metric in page.metrics" :key="metric.label">
                {{ metric.label }} {{ metric.value }}
              </span>
              <ul>
                <li v-for="item in page.scope.items" :key="item">{{ item }}</li>
                <li v-for="item in page.nextSteps.items" :key="item">{{ item }}</li>
                <li v-for="item in page.risks.items" :key="item">{{ item }}</li>
              </ul>
            </section>
          `,
        },
      },
    },
  })
}

describe('LogsView', () => {
  it('renders the sensitive static Logs surface without runtime markers', () => {
    const wrapper = mountView()

    expect(wrapper.get('h1').text()).toBe('Logs')
    expect(wrapper.text()).toContain('payloads, trazas o datos personales')
    expect(wrapper.text()).toContain('Datos Redaccion obligatoria')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(2)
    expect(wrapper.text()).not.toMatch(runtimeMarkers)
  })
})
