import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import ControlesView from './ControlesView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|appsettings|datasource/i
const realDataMarkers =
  /[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}|\b\d{8}[A-Z]\b|\b\d{9}\b|\b\d{3}[-.\s]?\d{3}[-.\s]?\d{3}\b|IBAN|ES\d{22}|password=|pwd=/i

async function mountControlesView() {
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
      { path: '/controles', component: ControlesView },
      { path: '/estadisticas', component: { template: '<div />' } },
      { path: '/administracion', component: { template: '<div />' } },
      { path: '/configuracion', component: { template: '<div />' } },
      { path: '/conectividad', component: { template: '<div />' } },
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/controles')
  await router.isReady()

  return mount(ControlesView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleControlesView() {
  await flushPromises()
  await flushPromises()
}

describe('ControlesView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only controls MVP with local fixtures and pagination', async () => {
    const wrapper = await mountControlesView()
    await settleControlesView()

    expect(wrapper.text()).toContain('Controles')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Controles no confirmados')
    expect(wrapper.text()).toContain('SDD/UAT pendiente')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 controles candidatos')
    expect(wrapper.get('caption').text()).toContain(
      'Controles fixture read-only: 1-2 de 4 controles candidatos',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('CTRL-2026-0001')
    expect(wrapper.text()).toContain('CTRL-2026-0002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleControlesView()

    expect(wrapper.text()).toContain('CTRL-2026-0003')
    expect(wrapper.text()).toContain('CTRL-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters candidate controls locally by text, area, state, and risk', async () => {
    const wrapper = await mountControlesView()
    await settleControlesView()

    await wrapper.get('#controles-filter-texto').setValue('datos')
    await wrapper.get('#controles-filter-area').setValue('Datos')
    await wrapper.get('#controles-filter-estado').setValue('Candidato')
    await wrapper.get('#controles-filter-riesgo').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleControlesView()

    expect(wrapper.get('.summary-header').text()).toContain('1 control candidato')
    expect(wrapper.get('caption').text()).toContain(
      'Controles fixture read-only: 1-1 de 1 control candidato',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('CTRL-2026-0002')
    expect(wrapper.text()).toContain('Validacion de datos sanitizados')
    expect(wrapper.text()).not.toContain('CTRL-2026-0001')
  })

  it('shows an empty state for filters without candidate matches', async () => {
    const wrapper = await mountControlesView()
    await settleControlesView()

    await wrapper.get('#controles-filter-texto').setValue('control real aprobado')
    await wrapper.get('#controles-filter-area').setValue('Operaciones')
    await wrapper.get('#controles-filter-riesgo').setValue('Alto')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleControlesView()

    expect(wrapper.get('.summary-header').text()).toContain('0 controles candidatos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay controles candidatos para los filtros actuales.')
  })

  it('keeps validation/export/detail actions disabled and exposes no real data', async () => {
    const wrapper = await mountControlesView()
    await settleControlesView()

    const validateButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Validar alcance'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))

    expect(validateButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(2)
    expect(
      (
        wrapper.get('input[aria-label="Ejecucion de controles bloqueada"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: no hay controles funcionales aprobados')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(realDataMarkers)
  })
})
