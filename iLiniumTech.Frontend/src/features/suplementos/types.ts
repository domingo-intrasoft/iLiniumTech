export type SuplementoTipo = 'Alta de riesgo' | 'Regularizacion' | 'Domiciliacion' | 'Renovacion'
export type SuplementoSituacion = 'Pendiente' | 'En revision' | 'Validado' | 'Bloqueado'

export interface SuplementoListItem {
  id: string
  referencia: string
  poliza: string
  tipo: SuplementoTipo
  situacion: SuplementoSituacion
  fechaEfecto: string
  concepto: string
  resumen: string
  origen: string
}

export interface SuplementosFilters {
  texto: string
  poliza: string
  tipo: '' | SuplementoTipo
  situacion: '' | SuplementoSituacion
  fechaDesde: string
}
