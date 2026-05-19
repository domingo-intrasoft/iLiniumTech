import { flushPromises, mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'

import { useSession } from '@/services/session'

import AgendaView from './AgendaView.vue'

const unsafeRuntimeMarkers =
  /IAP_|QueryStatic|ComponentDataSource|connectionString|SELECT \*|Pantalla_|AppBuilder|appsettings|metadata|FullCalendar|drag-and-drop/i
const secretOrSensitiveMarkers =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|secret|token|iban|cuenta bancaria|direccion|telefono|email|@|dni|nif|salud|lesion/i

async function mountAgendaView() {
  const router = createRouter({
    history: createMemoryHistory(),
    routes: [
      { path: '/login', name: 'login', component: { template: '<div />' } },
      { path: '/agenda', component: AgendaView },
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
      { path: '/by-aunna', component: { template: '<div />' } },
      { path: '/logs', component: { template: '<div />' } },
    ],
  })
  await router.push('/agenda')
  await router.isReady()

  return mount(AgendaView, {
    global: {
      plugins: [router],
    },
  })
}

async function settleAgendaView() {
  await flushPromises()
  await flushPromises()
}

describe('AgendaView smoke', () => {
  beforeEach(() => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')
    vi.stubEnv('VITE_BROKER_ID', '')
    useSession().resetSession()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    useSession().resetSession()
  })

  it('renders the static read-only Agenda MVP with fixture data and pagination', async () => {
    const wrapper = await mountAgendaView()
    await settleAgendaView()

    expect(wrapper.text()).toContain('Agenda')
    expect(wrapper.text()).toContain('Solo lectura')
    expect(wrapper.text()).toContain('Fixture local sin API')
    expect(wrapper.text()).toContain('Sin calendario dinamico')
    expect(wrapper.text()).toContain('PII/asuntos sensibles bloqueados')
    expect(wrapper.text()).toContain('Crear, reprogramar y exportar bloqueados')
    expect(wrapper.text()).toContain('Modo fixture')
    expect(wrapper.get('.summary-header').text()).toContain('4 eventos')
    expect(wrapper.get('caption').text()).toContain('Agenda fixture read-only: 1-2 de 4 eventos')
    expect(wrapper.findAll('tbody tr')).toHaveLength(2)
    expect(wrapper.text()).toContain('AGE-2026-0001')
    expect(wrapper.text()).toContain('AGE-2026-0002')
    expect(wrapper.text()).toContain('Pagina 1 de 2')

    await wrapper
      .findAll('button')
      .find((button) => button.text().includes('Siguiente'))!
      .trigger('click')
    await settleAgendaView()

    expect(wrapper.text()).toContain('AGE-2026-0003')
    expect(wrapper.text()).toContain('AGE-2026-0004')
    expect(wrapper.text()).toContain('Pagina 2 de 2')
  })

  it('filters Agenda events locally by text, state, priority, and start date', async () => {
    const wrapper = await mountAgendaView()
    await settleAgendaView()

    await wrapper.get('#agenda-filter-texto').setValue('SIN-DEMO-0002')
    await wrapper.get('#agenda-filter-estado').setValue('Programado')
    await wrapper.get('#agenda-filter-prioridad').setValue('Media')
    await wrapper.get('#agenda-filter-fecha').setValue('2026-05-20')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleAgendaView()

    expect(wrapper.get('.summary-header').text()).toContain('1 evento')
    expect(wrapper.get('caption').text()).toContain('Agenda fixture read-only: 1-1 de 1 evento')
    expect(wrapper.findAll('tbody tr')).toHaveLength(1)
    expect(wrapper.text()).toContain('AGE-2026-0002')
    expect(wrapper.text()).toContain('Seguimiento demo de tramite')
    expect(wrapper.text()).not.toContain('AGE-2026-0001')
  })

  it('shows an empty state when local filters have no fixture matches', async () => {
    const wrapper = await mountAgendaView()
    await settleAgendaView()

    await wrapper.get('#agenda-filter-texto').setValue('referencia inexistente')
    await wrapper.get('#agenda-filter-estado').setValue('Cerrado')
    await wrapper.get('#agenda-filter-fecha').setValue('2026-06-30')
    await wrapper.get('.search-action-buttons .primary-action').trigger('click')
    await settleAgendaView()

    expect(wrapper.get('.summary-header').text()).toContain('0 eventos')
    expect(wrapper.text()).toContain('Sin resultados')
    expect(wrapper.text()).toContain('No hay eventos fixture para los filtros actuales.')
  })

  it('keeps write/detail actions disabled and exposes no unsafe or sensitive data', async () => {
    const wrapper = await mountAgendaView()
    await settleAgendaView()

    const createButton = wrapper.findAll('button').find((button) => button.text().includes('Crear'))
    const rescheduleButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Reprogramar'))
    const exportButton = wrapper
      .findAll('button')
      .find((button) => button.text().includes('Exportar'))

    expect(wrapper.get('#agenda-blocked-actions').text()).toContain(
      'Acciones de agenda bloqueadas en el MVP read-only',
    )
    expect(createButton?.attributes('disabled')).toBeDefined()
    expect(createButton?.attributes('aria-describedby')).toBe('agenda-blocked-actions')
    expect(rescheduleButton?.attributes('disabled')).toBeDefined()
    expect(rescheduleButton?.attributes('aria-describedby')).toBe('agenda-blocked-actions')
    expect(exportButton?.attributes('disabled')).toBeDefined()
    expect(exportButton?.attributes('aria-describedby')).toBe('agenda-blocked-actions')
    expect(
      wrapper
        .get('button[aria-label="Opciones de texto o referencia"]')
        .attributes('aria-describedby'),
    ).toBe('agenda-blocked-actions')
    const tableActions = wrapper.findAll('button.table-icon-action[disabled]')
    expect(tableActions).toHaveLength(4)
    expect(tableActions[0]?.attributes('aria-describedby')).toBe('agenda-blocked-actions')
    expect(
      (
        wrapper.get('input[aria-label="Campos y acciones sensibles bloqueados"]')
          .element as HTMLInputElement
      ).value,
    ).toBe('Bloqueado: PII/asuntos sensibles, sin API ni escrituras')
    expect(
      wrapper
        .get('input[aria-label="Campos y acciones sensibles bloqueados"]')
        .attributes('aria-describedby'),
    ).toBe('agenda-blocked-actions')

    const smokeDom = wrapper.html()
    expect(smokeDom).not.toMatch(unsafeRuntimeMarkers)
    expect(smokeDom).not.toMatch(secretOrSensitiveMarkers)
    expect(smokeDom).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
    expect(smokeDom).not.toMatch(/\b\d{8}[A-Z]\b/i)
  })
})
