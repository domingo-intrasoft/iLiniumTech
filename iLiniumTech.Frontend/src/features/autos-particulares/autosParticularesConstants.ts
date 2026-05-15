import type {
  AutoParticularPolizaListItem,
  AutosParticularesCatalogs,
} from './autosParticularesTypes'
import type { AutosValueType } from './autosParticularesFormatters'

export type AutosFilterControl = 'text' | 'select' | 'date'
export type AutosCatalogKey = Exclude<keyof AutosParticularesCatalogs, 'scope'>

export interface ShellAction {
  label: string
  icon: string
  active?: boolean
}

export interface AutosTableColumn {
  key: keyof AutoParticularPolizaListItem
  label: string
  type: AutosValueType
}

export interface AutosSearchCriteria {
  numero: string
  cliente: string
  estado: string
  compania: string
  fechaEfectoDesde: string
  fechaEfectoHasta: string
}

export interface AutosSearchField {
  key: AutosFilterKey
  label: string
  control?: AutosFilterControl
  catalogKey?: AutosCatalogKey
  span: number
}

export interface AutosSearchSection {
  title: string
  rows: AutosSearchField[][]
}

export const autosModuleActions: ShellAction[] = [
  { label: 'Autos Particulares', icon: 'pi pi-car', active: true },
  { label: 'Flotas', icon: 'pi pi-truck' },
  { label: 'Colectivos', icon: 'pi pi-users' },
  { label: 'Recibos', icon: 'pi pi-money-bill' },
]

export const autosTableColumns: AutosTableColumn[] = [
  { key: 'compania', label: 'Compania', type: 'string' },
  { key: 'numero', label: 'Poliza auto', type: 'string' },
  { key: 'estado', label: 'Situacion', type: 'string' },
  { key: 'clienteNombre', label: 'Tomador', type: 'string' },
  { key: 'vehiculoResumen', label: 'Vehiculo', type: 'string' },
  { key: 'fechaEfecto', label: 'Fecha efecto', type: 'date' },
  { key: 'fechaVencimiento', label: 'Fecha vencimiento', type: 'date' },
  { key: 'primaAnual', label: 'Prima anual', type: 'money' },
]

export const autosFilterKeys = [
  'numero',
  'cliente',
  'estado',
  'compania',
  'fechaEfectoDesde',
  'fechaEfectoHasta',
] as const

export type AutosFilterKey = (typeof autosFilterKeys)[number]
export type AutosFilterForm = Record<AutosFilterKey, string>

export const autosSearchSections: AutosSearchSection[] = [
  {
    title: 'Poliza y vehiculo',
    rows: [
      [
        { key: 'numero', label: 'Poliza auto', span: 3 },
        { key: 'estado', label: 'Situacion', control: 'select', catalogKey: 'estado', span: 2 },
        { key: 'compania', label: 'Compania', control: 'select', catalogKey: 'compania', span: 3 },
      ],
    ],
  },
  {
    title: 'Tomador y vigencia',
    rows: [
      [
        { key: 'cliente', label: 'Tomador', span: 4 },
        { key: 'fechaEfectoDesde', label: 'F. efecto desde', control: 'date', span: 2 },
        { key: 'fechaEfectoHasta', label: 'F. efecto hasta', control: 'date', span: 2 },
      ],
    ],
  },
]

export function createEmptyAutosFilterForm(): AutosFilterForm {
  return Object.fromEntries(autosFilterKeys.map((key) => [key, ''])) as AutosFilterForm
}
