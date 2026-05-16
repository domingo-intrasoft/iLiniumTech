import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import InformesView from './InformesView.vue'

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

describe('InformesView', () => {
  it('renders the static reports MVP without runtime markers', () => {
    const wrapper = mount(InformesView, {
      global: {
        stubs: {
          MvpPageShell: MvpPageShellStub,
        },
      },
    })

    expect(wrapper.text()).toContain('Informes')
    expect(wrapper.text()).toContain('informes.execute')
    expect(wrapper.text()).toContain('PDF o Excel')
    expect(wrapper.text()).toContain('open redirect')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(4)
    expect(wrapper.html()).not.toMatch(forbiddenRuntimeMarkers)
  })
})
