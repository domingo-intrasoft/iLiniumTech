export interface PolizaFieldMetadata {
  name: string
  sourceField: string
  label: string
  type: 'string' | 'date' | 'money'
  filterable: boolean
  sortable: boolean
  visible: boolean
  order: number
}

export interface PolizasComponentMetadata {
  resource: 'polizas'
  version: number
  appBuilder: {
    applicationId: number
    applicationVersion: number
    menuId: number
    rootComponentId: number
    crudComponentId: number
    componentDataSourceId: number
    dataSourceId: number
    dataSourceName: string
    modelObject: string
  }
  fields: PolizaFieldMetadata[]
}

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

export interface PolizaCliente {
  id: string
  nombre: string
  documento: string
}

export interface PolizaProducto {
  nombre: string
  modalidad: string
}

export interface PolizaVigencia {
  fechaInicio: string
  fechaVencimiento: string
  renovacion: string
}

export interface PolizaFinanciero {
  primaAnual: number
  moneda: string
}

export interface PolizaRiesgo {
  id: string
  descripcion: string
  tipoRiesgo?: string
  fechaAlta?: string
  fechaBaja?: string
}

export interface PolizaRecibo {
  id: string
  numero: string
  estado: string
  estadoCia: string
  estadoColab: string
  tipo: string
  gestor: string
  primaTotal: number
  fechaEfecto: string
  fechaVencimiento: string
}

export interface PolizaDetail {
  id: string
  numero: string
  aplicacion: string
  estado: string
  ramo: string
  compania: string
  cliente: PolizaCliente
  producto: PolizaProducto
  vigencia: PolizaVigencia
  financiero: PolizaFinanciero
  riesgos: PolizaRiesgo[]
  recibos: PolizaRecibo[]
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}
