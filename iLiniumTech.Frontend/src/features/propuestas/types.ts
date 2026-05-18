export type PropuestaEstado =
  | 'Borrador demo'
  | 'En revision demo'
  | 'Caducada demo'
  | 'Bloqueada demo'

export type PropuestaRamo = 'Autos demo' | 'Hogar demo' | 'Comercio demo' | 'Salud demo'

export interface PropuestaListItem {
  id: string
  referencia: string
  estado: PropuestaEstado
  ramo: PropuestaRamo
  fechaAlta: string
  vigencia: string
  solicitante: string
  canal: string
  resultado: string
  importeDemo: string
}

export interface PropuestasFilters {
  referencia: string
  estado: '' | PropuestaEstado
  ramo: '' | PropuestaRamo
  fechaDesde: string
}

export interface PropuestasModuleAction {
  label: string
  icon: string
  active?: boolean
}
