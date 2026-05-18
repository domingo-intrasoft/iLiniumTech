export const siniestroEstados = ['En revision', 'Abierto', 'Cerrado'] as const
export const siniestroPrioridades = ['Alta', 'Media', 'Baja'] as const

export type SiniestroEstado = (typeof siniestroEstados)[number]
export type SiniestroPrioridad = (typeof siniestroPrioridades)[number]

export interface SiniestroListItem {
  id: string
  referencia: string
  poliza: string
  cliente: string
  compania: string
  situacion: string
  estado: SiniestroEstado
  prioridad: SiniestroPrioridad
  fechaSiniestro: string
  fechaParte: string
  tramitador: string
}

export interface SiniestrosFilters {
  referencia: string
  poliza: string
  estado: '' | SiniestroEstado
  prioridad: '' | SiniestroPrioridad
  fechaDesde: string
}
