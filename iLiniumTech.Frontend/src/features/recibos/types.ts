export interface ReciboListItem {
  id: string
  recibo: string
  poliza: string
  cliente: string
  compania: string
  tipo: string
  situacion: string
  fechaEfecto: string
  fechaVencimiento: string
  estadoCobro: string
  canal: string
}

export interface RecibosFilters {
  recibo: string
  poliza: string
  situacion: string
  tipo: string
  vencimientoDesde: string
}
