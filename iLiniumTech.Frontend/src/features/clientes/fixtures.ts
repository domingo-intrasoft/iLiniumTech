import type { ClienteEstado, ClienteListItem, ClienteSegmento, ClientesModuleAction } from './types'

export const clientesFixture: ClienteListItem[] = [
  {
    id: 'CLI-MVP-1001',
    referencia: 'CLI-2026-0001',
    alias: 'Alias anonimo A',
    estado: 'Activo demo',
    segmento: 'Particular demo',
    fechaAlta: '2026-01-12',
    resultado: 'Listado minimizado',
    datos: 'PII bloqueada',
    relacionadas: 'Tabs relacionadas pendientes',
  },
  {
    id: 'CLI-MVP-1002',
    referencia: 'CLI-2026-0002',
    alias: 'Alias anonimo B',
    estado: 'En revision',
    segmento: 'Empresa demo',
    fechaAlta: '2026-02-18',
    resultado: 'Pendiente de SDD',
    datos: 'Datos personales no incluidos',
    relacionadas: 'Polizas y recibos no operativos',
  },
  {
    id: 'CLI-MVP-1003',
    referencia: 'CLI-2026-0003',
    alias: 'Alias anonimo C',
    estado: 'Bloqueado PII',
    segmento: 'Colectivo demo',
    fechaAlta: '2026-03-05',
    resultado: 'Solo trazabilidad demo',
    datos: 'Contacto y bancarios bloqueados',
    relacionadas: 'Riesgos, siniestros y suplementos pendientes',
  },
  {
    id: 'CLI-MVP-1004',
    referencia: 'CLI-2026-0004',
    alias: 'Alias anonimo D',
    estado: 'Activo demo',
    segmento: 'Particular demo',
    fechaAlta: '2026-04-21',
    resultado: 'Fixture local',
    datos: 'Identidad legal bloqueada',
    relacionadas: 'Ficha no operativa',
  },
]

export const clienteEstadoOptions: ClienteEstado[] = ['Activo demo', 'En revision', 'Bloqueado PII']

export const clienteSegmentoOptions: ClienteSegmento[] = [
  'Particular demo',
  'Empresa demo',
  'Colectivo demo',
]

export const clientesPageSizeOptions = [2, 10, 25]

export const clientesTopBadges = ['Read-only', 'Fixture', 'PII bloqueada']

export const clientesModuleActions: ClientesModuleAction[] = [
  { label: 'Buscar clientes demo', icon: 'pi pi-search', active: true },
  { label: 'Abrir ficha bloqueado', icon: 'pi pi-id-card' },
  { label: 'Exportar bloqueado', icon: 'pi pi-download' },
  { label: 'Desglose bloqueado', icon: 'pi pi-sitemap' },
]

export const clientesBlockedActionsDescription =
  'Acciones de clientes bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de minimizacion PII.'
