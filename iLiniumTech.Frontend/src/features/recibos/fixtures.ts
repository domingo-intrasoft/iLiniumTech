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
    efecto: '2026-01-01',
    vencimiento: '2026-02-01',
    cobro: 'No operativo',
    canal: 'Canal demo',
    importeDemo: 'Importe demo A',
  },
  {
    id: 'REC-MVP-1002',
    recibo: 'REC-2026-0002',
    poliza: 'POL-2026-0002',
    cliente: 'Cliente anonimo 2',
    compania: 'Compania demo sur',
    tipo: 'Regularizacion',
    situacion: 'Cobrado',
    efecto: '2026-02-15',
    vencimiento: '2026-03-15',
    cobro: 'Cobro demo confirmado',
    canal: 'Canal demo mediador',
    importeDemo: 'Importe demo B',
  },
  {
    id: 'REC-MVP-1003',
    recibo: 'REC-2026-0003',
    poliza: 'POL-2026-0003',
    cliente: 'Cliente anonimo 3',
    compania: 'Compania demo este',
    tipo: 'Extorno',
    situacion: 'Anulado',
    efecto: '2026-04-01',
    vencimiento: '2026-05-01',
    cobro: 'No operativo',
    canal: 'Canal demo compania',
    importeDemo: 'Importe demo C',
  },
]

export const pageSizeOptions = [10, 25, 50]
export const topBadges = ['Read-only', 'Fixture']
export const moduleActions = [
  { label: 'Buscar recibos', icon: 'pi pi-search', active: true },
  { label: 'Detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]
export const blockedActionsDescription =
  'Acciones de recibos bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de cobros/remesas/datos bancarios.'
