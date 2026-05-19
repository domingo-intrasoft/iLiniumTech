import type { ReciboListItem } from './types'

export const recibosFixture: readonly ReciboListItem[] = [
  {
    id: 'REC-MVP-1001',
    recibo: 'REC-2026-0001',
    poliza: 'POL-2026-0001',
    cliente: 'Cliente anonimo 1',
    compania: 'Compania demo norte',
    tipo: 'Prima',
    situacion: 'Pendiente',
    fechaEfecto: '2026-01-01',
    fechaVencimiento: '2026-02-01',
    estadoCobro: 'No operativo',
    canal: 'Canal demo',
  },
  {
    id: 'REC-MVP-1002',
    recibo: 'REC-2026-0002',
    poliza: 'POL-2026-0002',
    cliente: 'Cliente anonimo 2',
    compania: 'Compania demo sur',
    tipo: 'Regularizacion',
    situacion: 'Cobrado',
    fechaEfecto: '2026-02-15',
    fechaVencimiento: '2026-03-15',
    estadoCobro: 'Cobro demo confirmado',
    canal: 'Canal demo mediador',
  },
  {
    id: 'REC-MVP-1003',
    recibo: 'REC-2026-0003',
    poliza: 'POL-2026-0003',
    cliente: 'Cliente anonimo 3',
    compania: 'Compania demo este',
    tipo: 'Extorno',
    situacion: 'Anulado',
    fechaEfecto: '2026-04-01',
    fechaVencimiento: '2026-05-01',
    estadoCobro: 'No operativo',
    canal: 'Canal demo compania',
  },
]

export const reciboSituaciones = ['Pendiente', 'Cobrado', 'Anulado']
export const reciboTipos = ['Prima', 'Extorno', 'Regularizacion']
export const reciboCanales = ['Canal demo', 'Canal demo mediador', 'Canal demo compania']
export const pageSizeOptions = [10, 25, 50]
export const moduleActions = [
  { label: 'Buscar recibos', icon: 'pi pi-search', active: true },
  { label: 'Detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]
export const blockedActionsDescription =
  'Acciones de recibos bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de cobros/remesas/datos bancarios.'
