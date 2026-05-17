import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import InformesView from './InformesView.vue'

const unsafeRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|vw_rpt_|IapMenu|datasource/i
const secretOrSensitiveMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|token|IBAN|cuenta bancaria|telefono|email|@/i

async function mountInformesView() {
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
      { path: '/informes', component: InformesView },
      { path: '/controles', component: { template: '<div />' } },
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/informes')
  await router.isReady()

  return mount(InformesView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleInformesView() {
  await flushPromises()
  await flushPromises()
}

describe('InformesView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only catalog/risk MVP with honest blocking copy', async () => {
    const wrapper = await mountInformesView()
    await settleInformesView()

    expect(wrapper.text()).toContain('Informes')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Sin ejecucion ni descarga')
    expect(wrapper.text()).toContain('Sin informes aprobados todavia')
    expect(wrapper.text()).toContain('Evidencia AppBuilder insuficiente')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 categorias candidatas')
    expect(wrapper.get('caption').text()).toContain(
      'Informes fixture read-only: 1-2 de 4 categorias candidatas',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('Polizas en vigor')
    expect(wrapper.text()).toContain('Recibos y remesas')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleInformesView()

    expect(wrapper.text()).toContain('Riesgos asociados a poliza')
    expect(wrapper.text()).toContain('Liquidaciones')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters candidate reports locally by text, area, approval state, and risk', async () => {
    const wrapper = await mountInformesView()
    await settleInformesView()

    await wrapper.get('#informes-filter-texto').setValue('recibos')
    await wrapper.get('#informes-filter-area').setValue('Recibos')
    await wrapper.get('#informes-filter-estado').setValue('Requiere SDD')
    await wrapper.get('#informes-filter-riesgo').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleInformesView()

    expect(wrapper.get('.summary-header').text()).toContain('1 categoria candidata')
    expect(wrapper.get('caption').text()).toContain(
      'Informes fixture read-only: 1-1 de 1 categoria candidata',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('Recibos y remesas')
    expect(wrapper.text()).toContain('No ejecutable en MVP')
    expect(wrapper.text()).not.toContain('Polizas en vigor')
  })

  it('shows an empty state for filters without candidate matches', async () => {
    const wrapper = await mountInformesView()
    await settleInformesView()

    await wrapper.get('#informes-filter-texto').setValue('excel aprobado')
    await wrapper.get('#informes-filter-estado').setValue('Sin aprobar')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleInformesView()

    expect(wrapper.get('.summary-header').text()).toContain('0 categorias candidatas')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay categorias candidatas para los filtros actuales.')
  })

  it('keeps execution and download actions disabled and exposes no unsafe runtime details', async () => {
    const wrapper = await mountInformesView()
    await settleInformesView()

    const executeButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Ejecutar'))
    const downloadButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Descargar'))

    expect(executeButton?.attributes('disabled')).toBeDefined()
    expect(downloadButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Ejecucion y descarga bloqueadas"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: no hay API ni informes aprobados')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(unsafeRuntimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
  })
})
