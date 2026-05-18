export type ReciboSituacion = 'Pendiente' | 'Cobrado' | 'Anulado'
export type ReciboTipo = 'Prima' | 'Extorno' | 'Regularizacion'

export interface ReciboListItem {
  id: string
  recibo: string
  poliza: string
  cliente: string
  compania: string
  tipo: ReciboTipo
  situacion: ReciboSituacion
  efecto: string
  vencimiento: string
  cobro: string
  canal: string
  importeDemo: string
}

export interface RecibosFilters {
  recibo: string
  poliza: string
  situacion: '' | ReciboSituacion
  tipo: '' | ReciboTipo
  vencimientoDesde: string
}
