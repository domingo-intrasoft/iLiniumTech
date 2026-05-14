import type { PolizaDetail, PolizaListItem, PolizasCatalogs } from './polizasTypes'

export type PolizaValueType = 'string' | 'date' | 'money' | 'number'
export type PolizasFilterControl = 'text' | 'select' | 'date'
export type PolizasCatalogKey = keyof PolizasCatalogs

export interface ShellAction {
  label: string
  icon: string
  active?: boolean
}

export interface PolizaTableColumn {
  key: keyof PolizaListItem
  label: string
  type: PolizaValueType
}

export interface PolizaDetailField {
  key: keyof PolizaDetail
  label: string
  type: PolizaValueType
}

export interface PolizaDetailSection {
  title: string
  fields: PolizaDetailField[]
}

export const sideItems: ShellAction[] = [
  { label: 'Agenda', icon: 'pi pi-calendar' },
  { label: 'Clientes', icon: 'pi pi-user' },
  { label: 'Propuestas', icon: 'pi pi-folder-open' },
  { label: 'Polizas', icon: 'pi pi-briefcase', active: true },
  { label: 'Recibos', icon: 'pi pi-money-bill' },
  { label: 'Suplementos', icon: 'pi pi-link' },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle' },
  { label: 'Liq.Cia', icon: 'pi pi-list' },
  { label: 'Liq.Col', icon: 'pi pi-list-check' },
  { label: 'Informes', icon: 'pi pi-file' },
  { label: 'Controles', icon: 'pi pi-home' },
  { label: 'Estadisticas', icon: 'pi pi-chart-bar' },
  { label: 'Administracion', icon: 'pi pi-table' },
  { label: 'Configuracion', icon: 'pi pi-cog' },
  { label: 'Conectividad', icon: 'pi pi-code' },
  { label: 'By Aunna', icon: 'pi pi-sitemap' },
  { label: 'Logs', icon: 'pi pi-database' },
]

export const moduleActions: ShellAction[] = [
  { label: 'Autos', icon: 'pi pi-car', active: true },
  { label: 'Gestion', icon: 'pi pi-id-card' },
  { label: 'Favoritos', icon: 'pi pi-heart-fill' },
  { label: 'Servicios', icon: 'pi pi-plus' },
  { label: 'Riesgos', icon: 'pi pi-truck' },
]

export const statusActions: ShellAction[] = [
  { label: 'Validar', icon: 'pi pi-check-circle' },
  { label: 'Pausar', icon: 'pi pi-pause-circle' },
  { label: 'Cerrar', icon: 'pi pi-times-circle' },
]

export const topBadges = ['F', '?', 'A', 'L', 'E', 'RH', 'WP', 'AU']

export const polizasTableColumns: PolizaTableColumn[] = [
  { key: 'compania', label: 'Compania', type: 'string' },
  { key: 'numero', label: 'Poliza', type: 'string' },
  { key: 'aplicacion', label: 'Aplicacion', type: 'string' },
  { key: 'estado', label: 'Situacion', type: 'string' },
  { key: 'ramo', label: 'Ramo', type: 'string' },
  { key: 'fechaEfecto', label: 'Fecha efecto', type: 'date' },
  { key: 'fechaVencimiento', label: 'Fecha vencimiento', type: 'date' },
  { key: 'primaAnual', label: 'Prima anual', type: 'money' },
  { key: 'clienteNombre', label: 'Cliente', type: 'string' },
]

export const polizaDetailSections: PolizaDetailSection[] = [
  {
    title: 'Datos de poliza',
    fields: [
      { key: 'numero', label: 'Poliza', type: 'string' },
      { key: 'certificado', label: 'Certificado', type: 'string' },
      { key: 'tipoPoliza', label: 'Tipo poliza', type: 'string' },
      { key: 'estado', label: 'Situacion', type: 'string' },
      { key: 'compania', label: 'Compania', type: 'string' },
      { key: 'ramo', label: 'Ramo', type: 'string' },
      { key: 'riesgo', label: 'Riesgo', type: 'string' },
      { key: 'primaAnual', label: 'Prima anual', type: 'money' },
    ],
  },
  {
    title: 'Gestion',
    fields: [
      { key: 'fechaEfecto', label: 'Fecha efecto', type: 'date' },
      { key: 'fechaVencimiento', label: 'Fecha vencimiento', type: 'date' },
      { key: 'oficina', label: 'Oficina', type: 'string' },
      { key: 'division', label: 'Division', type: 'string' },
      { key: 'colaborador1', label: 'Colaborador 1', type: 'string' },
      { key: 'administrativo', label: 'Administrativo', type: 'string' },
      { key: 'comercial', label: 'Comercial', type: 'string' },
      { key: 'gestor', label: 'Gestor', type: 'string' },
      { key: 'canalCobro', label: 'Canal cobro', type: 'string' },
      { key: 'fraccionPago', label: 'Fraccion pago', type: 'string' },
    ],
  },
  {
    title: 'Tomador',
    fields: [
      { key: 'clienteNombre', label: 'Nombre completo', type: 'string' },
      { key: 'documento', label: 'Documento', type: 'string' },
      { key: 'apellido1', label: 'Apellido 1', type: 'string' },
      { key: 'apellido2', label: 'Apellido 2', type: 'string' },
      { key: 'nombre', label: 'Nombre', type: 'string' },
      { key: 'sexo', label: 'Sexo', type: 'string' },
      { key: 'fechaNacimiento', label: 'Fecha nacimiento', type: 'date' },
      { key: 'edad', label: 'Edad', type: 'number' },
      { key: 'estadoCivil', label: 'Estado civil', type: 'string' },
      { key: 'hijos', label: 'Hijos', type: 'number' },
      { key: 'regimenLaboral', label: 'Regimen laboral', type: 'string' },
      { key: 'profesion', label: 'Profesion', type: 'string' },
      { key: 'email', label: 'Email', type: 'string' },
      { key: 'telefono', label: 'Telefono', type: 'string' },
    ],
  },
]

export const polizasFilterKeys = [
  'poliza',
  'certif',
  'tipoPoliza',
  'cia',
  'ramo',
  'riesgo',
  'efectoInicial',
  'efectoFinal',
  'vencimiento',
  'anulacion',
  'oficina',
  'division',
  'colaborador1',
  'administrativo',
  'comercial',
  'siniestros',
  'gestor',
  'canalCobro',
  'fraccionPago',
  'ccaa',
  'nombreCompleto',
  'documento',
  'apellido1',
  'apellido2',
  'nombre',
  'sexo',
  'nacimiento',
  'edad',
  'fallecimiento',
  'estadoCivil',
  'hijos',
  'regimenLaboral',
  'profesion',
] as const

export type PolizasFilterKey = (typeof polizasFilterKeys)[number]
export type PolizasFilterForm = Record<PolizasFilterKey, string>

export interface PolizasSearchField {
  key: PolizasFilterKey
  label: string
  control?: PolizasFilterControl
  catalogKey?: PolizasCatalogKey
  span: number
}

export interface PolizasSearchSection {
  title: string
  rows: PolizasSearchField[][]
}

export interface PolizasSearchCriteria {
  numero: string
  cliente: string
  estado: string
  compania: string
  ramo: string
  fechaEfectoDesde: string
  fechaEfectoHasta: string
}

export const polizasSearchSections: PolizasSearchSection[] = [
  {
    title: 'Datos de la poliza',
    rows: [
      [
        { key: 'poliza', label: 'Poliza', span: 2 },
        { key: 'certif', label: 'Certif.', span: 2 },
        {
          key: 'tipoPoliza',
          label: 'Tipo poliza',
          control: 'select',
          catalogKey: 'tipoPoliza',
          span: 2,
        },
        { key: 'cia', label: 'Cia.', control: 'select', catalogKey: 'compania', span: 4 },
        { key: 'ramo', label: 'Ramo', control: 'select', catalogKey: 'ramo', span: 2 },
      ],
      [{ key: 'riesgo', label: 'Riesgo/Matric.', span: 4 }],
    ],
  },
  {
    title: 'Datos de gestion',
    rows: [
      [
        { key: 'efectoInicial', label: 'F_EfectoInicial', control: 'date', span: 2 },
        { key: 'efectoFinal', label: 'F_EfectoHasta', control: 'date', span: 2 },
        { key: 'vencimiento', label: 'F. Vencimiento', control: 'date', span: 2 },
        { key: 'anulacion', label: 'F. Anulacion', control: 'date', span: 2 },
        { key: 'oficina', label: 'Oficina', control: 'select', catalogKey: 'oficina', span: 2 },
        { key: 'division', label: 'Division', control: 'select', catalogKey: 'division', span: 2 },
      ],
      [
        {
          key: 'colaborador1',
          label: 'Colaborador 1',
          control: 'select',
          catalogKey: 'colaborador1',
          span: 2,
        },
        {
          key: 'administrativo',
          label: 'Administrativo',
          control: 'select',
          catalogKey: 'administrativo',
          span: 2,
        },
        {
          key: 'comercial',
          label: 'Comercial',
          control: 'select',
          catalogKey: 'comercial',
          span: 2,
        },
        {
          key: 'siniestros',
          label: 'Siniestros',
          control: 'select',
          catalogKey: 'siniestros',
          span: 2,
        },
      ],
      [
        { key: 'gestor', label: 'Gestor', control: 'select', catalogKey: 'gestor', span: 2 },
        {
          key: 'canalCobro',
          label: 'Canal cobro',
          control: 'select',
          catalogKey: 'canalCobro',
          span: 4,
        },
        {
          key: 'fraccionPago',
          label: 'Fraccion pago',
          control: 'select',
          catalogKey: 'fraccionPago',
          span: 2,
        },
        { key: 'ccaa', label: 'CC AA', control: 'select', catalogKey: 'ccaa', span: 2 },
      ],
    ],
  },
  {
    title: 'Datos del tomador',
    rows: [
      [
        { key: 'nombreCompleto', label: 'N. Completo', span: 4 },
        { key: 'documento', label: 'N. Documento', span: 2 },
      ],
      [
        { key: 'apellido1', label: 'Apellido 1', span: 2 },
        { key: 'apellido2', label: 'Apellido 2', span: 2 },
        { key: 'nombre', label: 'Nombre', span: 2 },
      ],
      [
        { key: 'sexo', label: 'Sexo', control: 'select', catalogKey: 'sexo', span: 1 },
        { key: 'nacimiento', label: 'F. Nacimiento', control: 'date', span: 2 },
        { key: 'edad', label: 'Edad', span: 1 },
        { key: 'fallecimiento', label: 'F. Fallecimiento', control: 'date', span: 2 },
        {
          key: 'estadoCivil',
          label: 'Edo. Civil',
          control: 'select',
          catalogKey: 'estadoCivil',
          span: 2,
        },
        { key: 'hijos', label: 'N. Hijos', span: 1 },
      ],
      [
        {
          key: 'regimenLaboral',
          label: 'Reg. Laboral',
          control: 'select',
          catalogKey: 'regimenLaboral',
          span: 4,
        },
        {
          key: 'profesion',
          label: 'Profesion',
          control: 'select',
          catalogKey: 'profesion',
          span: 4,
        },
      ],
    ],
  },
]

export function createEmptyPolizasFilterForm(): PolizasFilterForm {
  return Object.fromEntries(polizasFilterKeys.map((key) => [key, ''])) as PolizasFilterForm
}
