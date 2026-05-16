import { mount } from '@vue/test-utils'
import { describe, expect, it } from 'vitest'
import type { Component } from 'vue'

import PolizasColectivasView from './PolizasColectivasView.vue'
import PolizasFlotasView from './PolizasFlotasView.vue'

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

function mountScope(component: Component) {
  return mount(component, {
    global: {
      stubs: {
        MvpPageShell: shellStub,
      },
    },
  })
}

describe('Polizas scope MVP views', () => {
  it('renders Flotas as a static blocked scope with disabled actions and no runtime markers', () => {
    const wrapper = mountScope(PolizasFlotasView)

    expect(wrapper.get('h1').text()).toBe('Polizas Flotas')
    expect(wrapper.text()).toContain('Scope Flotas')
    expect(wrapper.text()).toContain('polizas.scope.flotas.read')
    expect(wrapper.text()).toContain('no confirmados')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(3)
    expect(wrapper.html()).not.toMatch(runtimeMarkers)
  })

  it('renders Colectivas as a static blocked scope with disabled actions and no runtime markers', () => {
    const wrapper = mountScope(PolizasColectivasView)

    expect(wrapper.get('h1').text()).toBe('Polizas Colectivas')
    expect(wrapper.text()).toContain('Scope Colectivas')
    expect(wrapper.text()).toContain('polizas.scope.colectivas.read')
    expect(wrapper.text()).toContain('no confirmados')
    expect(wrapper.findAll('button[disabled]')).toHaveLength(3)
    expect(wrapper.html()).not.toMatch(runtimeMarkers)
  })
})
