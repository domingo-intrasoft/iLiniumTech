import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import ByAunnaView from './ByAunnaView.vue'

const unsafeRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|metadata heredada/i
const secretOrRealDataMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|bearer|api[-_ ]?key|token|https?:\/\/|@|\b\d{8}[A-Z]\b/i

async function mountByAunnaView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: { template: '<div />' } },
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: { template: '<div />' } },
      { path: '/propuestas', component: { template: '<div />' } },
      { path: '/polizas', component: { template: '<div />' } },
      { path: '/autos-particulares', component: { template: '<div />' } },
      { path: '/polizas/flotas', component: { template: '<div />' } },
      { path: '/polizas/colectivas', component: { template: '<div />' } },
      { path: '/recibos', component: { template: '<div />' } },
      { path: '/suplementos', component: { template: '<div />' } },
      { path: '/siniestros', component: { template: '<div />' } },
      { path: '/liq-cia', component: { template: '<div />' } },
      { path: '/liq-col', component: { template: '<div />' } },
      { path: '/informes', component: { template: '<div />' } },
      { path: '/controles', component: { template: '<div />' } },
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: ByAunnaView },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/by-aunna')
  await router.isReady()

  return mount(ByAunnaView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleView() {
  await flushPromises()
  await flushPromises()
}

describe('ByAunnaView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only By Aunna MVP with sanitized fixture data and pagination', async () => {
    const wrapper = await mountByAunnaView()
    await settleView()

    expect(wrapper.text()).toContain('By Aunna')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Contenido demo sanitizado')
    expect(wrapper.text()).toContain('Secretos, logs, payloads y enlaces externos bloqueados')
    expect(wrapper.text()).toContain('Publicacion y detalle deshabilitados')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 elementos candidatos')
    expect(wrapper.get('caption').text()).toContain(
      'By Aunna fixture read-only: 1-2 de 4 elementos candidatos',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('AUN-DEMO-001')
    expect(wrapper.text()).toContain('AUN-DEMO-002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleView()

    expect(wrapper.text()).toContain('AUN-DEMO-003')
    expect(wrapper.text()).toContain('AUN-DEMO-004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters By Aunna candidates locally by text, area, state, and risk', async () => {
    const wrapper = await mountByAunnaView()
    await settleView()

    await wrapper.get('#by-aunna-filter-text').setValue('recursos')
    await wrapper.get('#by-aunna-filter-area').setValue('Contenido')
    await wrapper.get('#by-aunna-filter-state').setValue('Bloqueado')
    await wrapper.get('#by-aunna-filter-risk').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('1 elemento candidato')
    expect(wrapper.get('caption').text()).toContain(
      'By Aunna fixture read-only: 1-1 de 1 elemento candidato',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('AUN-DEMO-003')
    expect(wrapper.text()).toContain('Descarga y detalle no operativos')
    expect(wrapper.text()).not.toContain('AUN-DEMO-001')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountByAunnaView()
    await settleView()

    await wrapper.get('#by-aunna-filter-text').setValue('enlace publicado')
    await wrapper.get('#by-aunna-filter-area').setValue('Marca')
    await wrapper.get('#by-aunna-filter-state').setValue('Bloqueado')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleView()

    expect(wrapper.get('.summary-header').text()).toContain('0 elementos candidatos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain(
      'No hay elementos fixture de By Aunna para los filtros actuales.',
    )
  })

  it('keeps publish, link, and detail actions disabled without real external links', async () => {
    const wrapper = await mountByAunnaView()
    await settleView()

    const publishButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Publicar'))
    const linkButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Abrir enlace'))

    expect(publishButton?.attributes('disabled')).toBeDefined()
    expect(linkButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Enlaces y publicacion bloqueados"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: sin API, sin enlaces reales, sin publicacion')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(unsafeRuntimeMarkers)
    expect(smokeDom).not.toMatch(secretOrRealDataMarkers)
  })
})
