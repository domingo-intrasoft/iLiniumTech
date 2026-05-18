export type ClienteEstado = 'Activo demo' | 'En revision' | 'Bloqueado PII'

export type ClienteSegmento = 'Particular demo' | 'Empresa demo' | 'Colectivo demo'

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
