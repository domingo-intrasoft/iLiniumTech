export interface PolizaListItem {
  id: string
  numero: string
  aplicacion: string
  estado: string
  ramo: string
  clienteId: string
  clienteNombre: string
  compania: string
  fechaEfecto: string
  fechaVencimiento: string
  primaAnual: number
  moneda: string
}

export const POLIZA_MVP_PREFIX = 'ILMVP-'
export const POLIZA_MVP_DELETED_PREFIX = 'ILMVP-DELETED-'

export interface PolizaCreatePayload {
  numero: string
  aplicacion: string
  ciaId: number
  clienteId: number
  estado: string
  ramo: string
  tipoPoliza: string
  fechaEfecto: string
  fechaVencimiento: string | null
  primaAnual: number
}

export interface PolizaUpdatePayload {
  numero?: string
  aplicacion?: string
  estado?: string
  ramo?: string
  tipoPoliza?: string
  fechaEfecto?: string
  fechaVencimiento?: string | null
  primaAnual?: number
}

export interface PolizaCreateResult {
  id: string
}

export interface PolizaDetail extends PolizaListItem {
  certificado: string
  tipoPoliza: string
  riesgo: string
  oficina: string
  division: string
  colaborador1: string
  administrativo: string
  comercial: string
  siniestros: string
  gestor: string
  canalCobro: string
  fraccionPago: string
  ccaa: string
  documento: string
  apellido1: string
  apellido2: string
  nombre: string
  sexo: string
  fechaNacimiento: string
  edad: number
  estadoCivil: string
  hijos: number
  regimenLaboral: string
  profesion: string
  email: string
  telefono: string
}

export interface PolizasCatalogOption {
  value: string
  label: string
}

export interface PolizasCatalogs {
  tipoPoliza: PolizasCatalogOption[]
  compania: PolizasCatalogOption[]
  ramo: PolizasCatalogOption[]
  oficina: PolizasCatalogOption[]
  division: PolizasCatalogOption[]
  colaborador1: PolizasCatalogOption[]
  administrativo: PolizasCatalogOption[]
  comercial: PolizasCatalogOption[]
  siniestros: PolizasCatalogOption[]
  gestor: PolizasCatalogOption[]
  canalCobro: PolizasCatalogOption[]
  fraccionPago: PolizasCatalogOption[]
  ccaa: PolizasCatalogOption[]
  sexo: PolizasCatalogOption[]
  estadoCivil: PolizasCatalogOption[]
  regimenLaboral: PolizasCatalogOption[]
  profesion: PolizasCatalogOption[]
}

export interface PolizasQueryFilters {
  numero: string
  cliente: string
  estado: string
  compania: string
  ramo: string
  fechaEfectoDesde: string
  fechaEfectoHasta: string
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}
