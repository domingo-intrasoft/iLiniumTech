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
  { label: 'Agenda', icon: 'pi pi-calendar', to: '/agenda' },
  { label: 'Clientes', icon: 'pi pi-user', to: '/clientes' },
  { label: 'Propuestas', icon: 'pi pi-folder-open', to: '/propuestas' },
  {
    label: 'Polizas',
    icon: 'pi pi-briefcase',
    to: '/polizas',
    requiredPermission: 'polizas.read',
    children: [
      { label: 'Autos Particulares', icon: 'pi pi-car', to: '/autos-particulares', disabled: true },
      { label: 'Flotas', icon: 'pi pi-truck', to: '/polizas/flotas' },
      { label: 'Colectivas', icon: 'pi pi-users', to: '/polizas/colectivas' },
    ],
  },
  { label: 'Recibos', icon: 'pi pi-money-bill', to: '/recibos' },
  { label: 'Suplementos', icon: 'pi pi-link', to: '/suplementos' },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle', to: '/siniestros' },
  { label: 'Liq.Cia', icon: 'pi pi-list', to: '/liq-cia' },
  { label: 'Liq.Col', icon: 'pi pi-list-check', to: '/liq-col' },
  { label: 'Informes', icon: 'pi pi-file', to: '/informes' },
  { label: 'Controles', icon: 'pi pi-home', to: '/controles' },
  { label: 'Estadisticas', icon: 'pi pi-chart-bar', to: '/estadisticas' },
  { label: 'Administracion', icon: 'pi pi-table', to: '/administracion' },
  { label: 'Configuracion', icon: 'pi pi-cog', to: '/configuracion' },
  { label: 'Conectividad', icon: 'pi pi-code', to: '/conectividad' },
  { label: 'By Aunna', icon: 'pi pi-sitemap', to: '/by-aunna' },
  { label: 'Logs', icon: 'pi pi-database', to: '/logs' },
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
