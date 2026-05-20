export interface SuplementoListItem {
  id: string
  referencia: string
  poliza: string
  tipo: string
  situacion: string
  fechaEfecto: string
  concepto: string
  resumen: string
}

export interface SuplementosFilters {
  texto: string
  poliza: string
  tipo: string
  situacion: string
  fechaDesde: string
}
