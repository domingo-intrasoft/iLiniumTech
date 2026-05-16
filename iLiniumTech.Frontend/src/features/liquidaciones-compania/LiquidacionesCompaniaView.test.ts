import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'

import LiquidacionesCompaniaView from './LiquidacionesCompaniaView.vue'

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

describe('LiquidacionesCompaniaView', () => {
  it('renders the static company settlements MVP without runtime markers', () => {
    const wrapper = mount(LiquidacionesCompaniaView, {
      global: {
        stubs: {
          MvpPageShell: MvpPageShellStub,
        },
      },
    })

    expect(wrapper.text()).toContain('Liquidaciones de compania')
    expect(wrapper.text()).toContain('liqCia.read')
    expect(wrapper.text()).toContain('conciliacion')
    expect(wrapper.text()).toContain('Importes, comisiones, facturas')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(4)
    expect(wrapper.html()).not.toMatch(forbiddenRuntimeMarkers)
  })
})
