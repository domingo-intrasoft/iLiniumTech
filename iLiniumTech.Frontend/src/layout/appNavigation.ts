export interface AppNavigationItem {
  label: string
  icon: string
  to?: string
  disabled?: boolean
  requiredPermission?: string
  children?: AppNavigationItem[]
}

export interface AppNavigationSessionState {
  authMode?: string
  permissions?: readonly string[]
}

export const appNavigation: AppNavigationItem[] = [
  { label: 'Agenda', icon: 'pi pi-calendar', disabled: true },
  { label: 'Clientes', icon: 'pi pi-user', disabled: true },
  { label: 'Propuestas', icon: 'pi pi-folder-open', disabled: true },
  {
    label: 'Polizas',
    icon: 'pi pi-briefcase',
    to: '/polizas',
    requiredPermission: 'polizas.read',
    children: [
      { label: 'Autos Particulares', icon: 'pi pi-car', to: '/autos-particulares', disabled: true },
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

export function hasNavigationPermission(
  item: AppNavigationItem,
  session?: AppNavigationSessionState | null,
) {
  if (!item.requiredPermission) {
    return true
  }

  if (session?.authMode === 'ApiKey' || !Array.isArray(session?.permissions)) {
    return true
  }

  return session.permissions.includes(item.requiredPermission)
}

export function getNavigationUnavailableReason(
  item: AppNavigationItem,
  session?: AppNavigationSessionState | null,
) {
  if (item.disabled) {
    return 'disabled'
  }

  return hasNavigationPermission(item, session) ? null : 'permission'
}
