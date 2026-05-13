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

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}
