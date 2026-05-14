export interface AutoParticularPolizaListItem {
  id: string
  numero: string
  aplicacion?: string
  estado: string
  ramo?: string
  compania: string
  clienteNombre: string
  vehiculoResumen?: string
  fechaEfecto: string
  fechaVencimiento: string
  primaAnual: number
  moneda: string
}

export interface AutosParticularesCatalogOption {
  value: string
  label: string
}

export interface AutosParticularesCatalogs {
  estado: AutosParticularesCatalogOption[]
  compania: AutosParticularesCatalogOption[]
  scope?: AutosParticularesScope
}

export interface AutosParticularesQueryFilters {
  numero: string
  cliente: string
  estado: string
  compania: string
  fechaEfectoDesde: string
  fechaEfectoHasta: string
}

export interface AutosParticularesScope {
  ramo: string
  divisionObjetivo: string
  divisionFiltroAplicado: boolean
  divisionPendienteUat: boolean
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
  scope?: AutosParticularesScope
}
