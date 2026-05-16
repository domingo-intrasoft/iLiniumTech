import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import EstadisticasView from './EstadisticasView.vue'

const forbiddenRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|SELECT\s+\*|connectionString|metadata|AppBuilder|vw_rpt_|IapMenu|datasource/i

const MvpPageShellStub = {
  props: ['page'],
  template: `
    <main>
      <h1>{{ page.title }}</h1>
      <p>{{ page.subtitle }}</p>
      <button v-for="action in page.actions" :key="action.label" disabled>
        {{ action.label }}
      </button>
      <section>{{ page.scope.items.join(' ') }}</section>
      <section>{{ page.nextSteps.items.join(' ') }}</section>
      <section>{{ page.risks.items.join(' ') }}</section>
      <footer>{{ page.source }}</footer>
    </main>
  `,
}

describe('EstadisticasView', () => {
  it('renders the static statistics MVP without runtime markers', () => {
    const wrapper = mount(EstadisticasView, {
      global: {
        stubs: {
          MvpPageShell: MvpPageShellStub,
        },
      },
    })

    expect(wrapper.text()).toContain('Estadisticas')
    expect(wrapper.text()).toContain('estadisticas.read')
    expect(wrapper.text()).toContain('reidentificacion')
    expect(wrapper.text()).toContain('aislamiento por broker')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(4)
    expect(wrapper.html()).not.toMatch(forbiddenRuntimeMarkers)
  })
})
