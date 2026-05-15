export interface AppNavigationItem {
  label: string
  icon: string
  to?: string
  disabled?: boolean
  children?: AppNavigationItem[]
}

export const appNavigation: AppNavigationItem[] = [
  { label: 'Agenda', icon: 'pi pi-calendar', disabled: true },
  { label: 'Clientes', icon: 'pi pi-user', disabled: true },
  { label: 'Propuestas', icon: 'pi pi-folder-open', disabled: true },
  {
    label: 'Polizas',
    icon: 'pi pi-briefcase',
    to: '/polizas',
    children: [
      { label: 'Autos Particulares', icon: 'pi pi-car', to: '/autos-particulares' },
      { label: 'Flotas', icon: 'pi pi-truck', disabled: true },
      { label: 'Colectivas', icon: 'pi pi-users', disabled: true },
    ],
  },
  { label: 'Recibos', icon: 'pi pi-money-bill', disabled: true },
  { label: 'Suplementos', icon: 'pi pi-link', disabled: true },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle', disabled: true },
  { label: 'Liq.Cia', icon: 'pi pi-list', disabled: true },
  { label: 'Liq.Col', icon: 'pi pi-list-check', disabled: true },
  { label: 'Informes', icon: 'pi pi-file', disabled: true },
  { label: 'Controles', icon: 'pi pi-home', disabled: true },
  { label: 'Estadisticas', icon: 'pi pi-chart-bar', disabled: true },
  { label: 'Administracion', icon: 'pi pi-table', disabled: true },
  { label: 'Configuracion', icon: 'pi pi-cog', disabled: true },
  { label: 'Conectividad', icon: 'pi pi-code', disabled: true },
  { label: 'By Aunna', icon: 'pi pi-sitemap', disabled: true },
  { label: 'Logs', icon: 'pi pi-database', disabled: true },
]

export function isNavigationItemActive(item: AppNavigationItem, path: string): boolean {
  if (item.to && (item.to === path || path.startsWith(`${item.to}/`))) {
    return true
  }

  return item.children?.some((child) => isNavigationItemActive(child, path)) ?? false
}
