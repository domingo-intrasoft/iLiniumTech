import type {
  PropuestaEstado,
  PropuestaListItem,
  PropuestaRamo,
  PropuestasModuleAction,
} from './types'

export const propuestasFixture: PropuestaListItem[] = [
  {
    id: 'PROP-MVP-1001',
    referencia: 'PROP-2026-0001',
    estado: 'Borrador demo',
    ramo: 'Autos demo',
    fechaAlta: '2026-01-12',
    vigencia: 'Pendiente de SDD',
    solicitante: 'Solicitante anonimo 1',
    canal: 'Canal demo mediador',
    resultado: 'Sin tarificacion operativa',
    importeDemo: 'Importe demo A',
  },
  {
    id: 'PROP-MVP-1002',
    referencia: 'PROP-2026-0002',
    estado: 'En revision demo',
    ramo: 'Hogar demo',
    fechaAlta: '2026-02-20',
    vigencia: 'Pendiente de origen',
    solicitante: 'Solicitante anonimo 2',
    canal: 'Canal demo oficina',
    resultado: 'Revision funcional pendiente',
    importeDemo: 'Importe demo B',
  },
  {
    id: 'PROP-MVP-1003',
    referencia: 'PROP-2026-0003',
    estado: 'Caducada demo',
    ramo: 'Comercio demo',
    fechaAlta: '2026-03-18',
    vigencia: 'No operativa',
    solicitante: 'Solicitante anonimo 3',
    canal: 'Canal demo interno',
    resultado: 'Conversion bloqueada',
    importeDemo: 'Importe demo C',
  },
  {
    id: 'PROP-MVP-1004',
    referencia: 'PROP-2026-0004',
    estado: 'Bloqueada demo',
    ramo: 'Salud demo',
    fechaAlta: '2026-04-05',
    vigencia: 'Pendiente de API',
    solicitante: 'Solicitante anonimo 4',
    canal: 'Canal demo compania',
    resultado: 'Documentos no conectados',
    importeDemo: 'Importe demo D',
  },
]

export const propuestasEstados: PropuestaEstado[] = [
  'Borrador demo',
  'En revision demo',
  'Caducada demo',
  'Bloqueada demo',
]

export const propuestasRamos: PropuestaRamo[] = [
  'Autos demo',
  'Hogar demo',
  'Comercio demo',
  'Salud demo',
]

export const propuestasTopBadges = ['Read-only', 'Fixture']

export const propuestasModuleActions: PropuestasModuleAction[] = [
  { label: 'Buscar propuestas', icon: 'pi pi-search', active: true },
  { label: 'Crear propuesta pendiente', icon: 'pi pi-plus' },
  { label: 'Convertir a poliza pendiente', icon: 'pi pi-send' },
  { label: 'Documentos pendientes', icon: 'pi pi-folder' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]

export const propuestasBlockedActionsDescription =
  'Acciones de propuestas bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de emision/conversion/documentos.'
