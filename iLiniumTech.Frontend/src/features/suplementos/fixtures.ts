import type { SuplementoListItem } from './types'

export const suplementosFixture: readonly SuplementoListItem[] = [
  {
    id: 'SUP-MVP-1001',
    referencia: 'SUP-2026-0001',
    poliza: 'POL-2026-0001',
    tipo: 'Alta de riesgo',
    situacion: 'Pendiente',
    fechaEfecto: '2026-02-01',
    concepto: 'Incorporacion de cobertura',
    resumen: 'Cambio operativo pendiente de contrato API',
    origen: 'Fixture sanitizado A',
  },
  {
    id: 'SUP-MVP-1002',
    referencia: 'SUP-2026-0002',
    poliza: 'POL-2026-0002',
    tipo: 'Regularizacion',
    situacion: 'En revision',
    fechaEfecto: '2026-03-15',
    concepto: 'Revision de condiciones',
    resumen: 'Movimiento read-only sin importes ni adjuntos',
    origen: 'Fixture sanitizado B',
  },
  {
    id: 'SUP-MVP-1003',
    referencia: 'SUP-2026-0003',
    poliza: 'POL-2026-0003',
    tipo: 'Domiciliacion',
    situacion: 'Bloqueado',
    fechaEfecto: '2026-01-20',
    concepto: 'Cambio administrativo',
    resumen: 'Datos restringidos ocultos hasta SDD',
    origen: 'Fixture sanitizado C',
  },
  {
    id: 'SUP-MVP-1004',
    referencia: 'SUP-2026-0004',
    poliza: 'POL-2026-0004',
    tipo: 'Renovacion',
    situacion: 'Validado',
    fechaEfecto: '2026-04-05',
    concepto: 'Actualizacion de vigencia',
    resumen: 'Lectura de muestra sin workflows',
    origen: 'Fixture sanitizado D',
  },
]

export const pageSizeOptions = [2, 10, 25]
export const topBadges = ['Read-only', 'Fixture']
export const moduleActions = [
  { label: 'Buscar suplementos', icon: 'pi pi-search', active: true },
  { label: 'Detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]
export const blockedActionsDescription =
  'Acciones de suplementos bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de workflows/adjuntos/datos restringidos.'
