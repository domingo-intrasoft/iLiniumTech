import type { AgendaListItem } from './types'

export const agendaFixture: readonly AgendaListItem[] = [
  {
    id: 'AGE-MVP-1001',
    referencia: 'AGE-2026-0001',
    asunto: 'Revision demo de documentacion',
    estado: 'Pendiente',
    prioridad: 'Alta',
    fechaInicio: '2026-05-18',
    horaInicio: '09:30',
    fechaFin: '2026-05-18',
    horaFin: '10:00',
    objetoRelacionado: 'POL-DEMO-0001',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1002',
    referencia: 'AGE-2026-0002',
    asunto: 'Seguimiento demo de tramite',
    estado: 'Programado',
    prioridad: 'Media',
    fechaInicio: '2026-05-21',
    horaInicio: '12:00',
    fechaFin: '2026-05-21',
    horaFin: '12:30',
    objetoRelacionado: 'SIN-DEMO-0002',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1003',
    referencia: 'AGE-2026-0003',
    asunto: 'Cierre demo de tarea interna',
    estado: 'Cerrado',
    prioridad: 'Baja',
    fechaInicio: '2026-05-24',
    horaInicio: '16:00',
    fechaFin: '2026-05-24',
    horaFin: '16:20',
    objetoRelacionado: 'REC-DEMO-0003',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1004',
    referencia: 'AGE-2026-0004',
    asunto: 'Control demo de agenda semanal',
    estado: 'Programado',
    prioridad: 'Alta',
    fechaInicio: '2026-06-02',
    horaInicio: '11:15',
    fechaFin: '2026-06-02',
    horaFin: '11:45',
    objetoRelacionado: 'GEN-DEMO-0004',
    origen: 'Fixture local',
  },
]

export const pageSizeOptions = [2, 10, 25]
export const topBadges = ['Read-only', 'Fixture']
export const moduleActions = [
  { label: 'Buscar agenda', icon: 'pi pi-search', active: true },
  { label: 'Crear evento bloqueado', icon: 'pi pi-plus' },
  { label: 'Reprogramar bloqueado', icon: 'pi pi-calendar-times' },
  { label: 'Exportar bloqueado', icon: 'pi pi-download' },
]
export const blockedActionsDescription =
  'Acciones de agenda bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de minimizacion de PII/asuntos sensibles.'
