import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import PropuestasView from './PropuestasView.vue'

const runtimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata/i
const secretOrSensitiveMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|secret|token|iban|cuenta bancaria|direccion|telefono|email|@/i

async function mountPropuestasView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: { template: '<div />' } },
      { path: '/agenda', component: { template: '<div />' } },
      { path: '/clientes', component: { template: '<div />' } },
      { path: '/propuestas', component: PropuestasView },
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
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/propuestas')
  await router.isReady()

  return mount(PropuestasView, {
    global: {
      plugins: [router],
    },
  })
}

async function settlePropuestasView() {
  await flushPromises()
  await flushPromises()
}

describe('PropuestasView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the read-only fixture shell with sanitized data and disabled actions', async () => {
    const wrapper = await mountPropuestasView()
    await settlePropuestasView()

    expect(wrapper.text()).toContain('Propuestas')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Datos minimizados y sanitizados')
    expect(wrapper.text()).toContain('Sin emision ni conversion a poliza')
    expect(wrapper.text()).toContain('Origen y SDD pendientes')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 propuestas')
    expect(wrapper.get('caption').text()).toContain(
      'Propuestas fixture read-only: 1-4 de 4 propuestas',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(4)
    expect(wrapper.text()).toContain('PROP-2026-0001')
    expect(wrapper.text()).toContain('Solicitante anonimo 2')
    expect(wrapper.text()).toContain('Importe demo D')

    const disabledActions = wrapper.findAll('button[disabled]')
    expect(disabledActions.length).toBeGreaterThanOrEqual(12)
    expect(wrapper.findAll('button.table-icon-action[disabled]')).toHaveLength(4)
    expect(wrapper.text()).toContain('Crear')
    expect(wrapper.text()).toContain('Convertir')
    expect(wrapper.text()).toContain('Documentos')
    expect(wrapper.text()).toContain('Exportar')
  })

  it('filters locally by reference, state, branch/type, and date from', async () => {
    const wrapper = await mountPropuestasView()
    await settlePropuestasView()

    await wrapper.get('#propuestas-filter-referencia').setValue('0002')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settlePropuestasView()

    expect(wrapper.get('.summary-header').text()).toContain('1 propuesta')
    expect(wrapper.get('caption').text()).toContain(
      'Propuestas fixture read-only: 1-1 de 1 propuesta',
    )
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('PROP-2026-0002')
    expect(wrapper.text()).toContain('Hogar demo')
    expect(wrapper.text()).not.toContain('PROP-2026-0001')

    const clearButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Limpiar Filtros'))
    expect(clearButton).toBeDefined()
    await clearButton!.trigger('click')
    await settlePropuestasView()

    await wrapper.get('#propuestas-filter-estado').setValue('Bloqueada demo')
    await wrapper.get('#propuestas-filter-ramo').setValue('Salud demo')
    await wrapper.get('#propuestas-filter-fecha').setValue('2026-04-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settlePropuestasView()

    expect(wrapper.get('.summary-header').text()).toContain('1 propuesta')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('PROP-2026-0004')
    expect(wrapper.text()).toContain('Documentos no conectados')
  })

  it('shows the empty state for filters without fixture matches', async () => {
    const wrapper = await mountPropuestasView()
    await settlePropuestasView()

    await wrapper.get('#propuestas-filter-estado').setValue('Caducada demo')
    await wrapper.get('#propuestas-filter-fecha').setValue('2026-04-01')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settlePropuestasView()

    expect(wrapper.get('.summary-header').text()).toContain('0 propuestas')
    expect(wrapper.find('tbody tr').exists()).toBe(false)
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay propuestas fixture para los filtros actuales.')
  })

  it('does not expose runtime metadata, secrets, personal identifiers, or operative amounts', async () => {
    const wrapper = await mountPropuestasView()
    await settlePropuestasView()

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(runtimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
    expect(smokeDom).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
    expect(smokeDom).not.toMatch(/\b\d{8}[A-Z]\b/i)
    expect(smokeDom).not.toMatch(/\b\d{1,3}(?:\.\d{3})*,\d{2}\s?EUR\b/i)
  })
})
