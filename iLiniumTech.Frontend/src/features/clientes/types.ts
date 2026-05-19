export type ClienteEstado = string

export type ClienteSegmento = string

export interface ClienteListItem {
  id: string
  referencia: string
  alias: string
  estado: ClienteEstado
  segmento: ClienteSegmento
  fechaAlta: string
  resultado: string
  datos: string
  relacionadas: string
}

export interface ClienteCreatePayload {
  nombreMostrable: string
  tipoCliente?: 'particular' | 'empresa'
}

export interface ClienteUpdatePayload {
  nombreMostrable?: string
  tipoCliente?: 'particular' | 'empresa'
}

export interface ClienteCreateResult {
  id: string
}

export interface ClientesFilters {
  texto: string
  estado: '' | ClienteEstado
  segmento: '' | ClienteSegmento
  fechaAltaDesde: string
}

export interface ClientesModuleAction {
  label: string
  icon: string
  active?: boolean
}
