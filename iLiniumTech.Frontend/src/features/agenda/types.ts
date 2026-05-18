export type AgendaEstado = 'Pendiente' | 'Programado' | 'Cerrado'
export type AgendaPrioridad = 'Alta' | 'Media' | 'Baja'

export interface AgendaListItem {
  id: string
  referencia: string
  asunto: string
  estado: AgendaEstado
  prioridad: AgendaPrioridad
  fechaInicio: string
  horaInicio: string
  fechaFin: string
  horaFin: string
  objetoRelacionado: string
  origen: string
}

export interface AgendaFilters {
  texto: string
  estado: '' | AgendaEstado
  prioridad: '' | AgendaPrioridad
  fechaDesde: string
}
