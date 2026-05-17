export interface AppNavigationItem {
  label: string
  icon: string
  to?: string
  disabled?: boolean
  status: AppNavigationStatus
  requiredPermission?: string
  children?: AppNavigationItem[]
}

export type AppNavigationStatus = 'operational' | 'fixture' | 'parked' | 'blockedSdd'

export interface AppNavigationSessionState {
  authMode?: string
  permissions?: readonly string[]
}

const navigationStatusLabels: Record<AppNavigationStatus, string> = {
  operational: 'Operativo',
  fixture: 'Fixture',
  parked: 'Aparcado',
  blockedSdd: 'Bloqueado SDD',
}

const navigationStatusShortLabels: Record<AppNavigationStatus, string> = {
  operational: 'OK',
  fixture: 'FIC',
  parked: 'PA',
  blockedSdd: 'SDD',
}

const navigationStatusDescriptions: Record<AppNavigationStatus, string> = {
  operational: 'MVP read-only con contrato iLiniumTech explicito',
  fixture: 'MVP estatico con fixture local sin API real',
  parked: 'Aparcado por decision de producto',
  blockedSdd: 'Bloqueado hasta SDD, regla funcional, permisos y UAT',
}

export const appNavigationStatusLegend: AppNavigationStatus[] = [
  'operational',
  'fixture',
  'parked',
  'blockedSdd',
]

export const appNavigation: AppNavigationItem[] = [
  { label: 'Agenda', icon: 'pi pi-calendar', to: '/agenda', status: 'fixture' },
  { label: 'Clientes', icon: 'pi pi-user', to: '/clientes', status: 'fixture' },
  { label: 'Propuestas', icon: 'pi pi-folder-open', to: '/propuestas', status: 'fixture' },
  {
    label: 'Polizas',
    icon: 'pi pi-briefcase',
    to: '/polizas',
    status: 'operational',
    requiredPermission: 'polizas.read',
    children: [
      {
        label: 'Autos Particulares',
        icon: 'pi pi-car',
        to: '/autos-particulares',
        status: 'parked',
        disabled: true,
      },
      { label: 'Flotas', icon: 'pi pi-truck', to: '/polizas/flotas', status: 'blockedSdd' },
      {
        label: 'Colectivas',
        icon: 'pi pi-users',
        to: '/polizas/colectivas',
        status: 'blockedSdd',
      },
    ],
  },
  { label: 'Recibos', icon: 'pi pi-money-bill', to: '/recibos', status: 'fixture' },
  { label: 'Suplementos', icon: 'pi pi-link', to: '/suplementos', status: 'fixture' },
  {
    label: 'Siniestros',
    icon: 'pi pi-exclamation-triangle',
    to: '/siniestros',
    status: 'fixture',
  },
  { label: 'Liq.Cia', icon: 'pi pi-list', to: '/liq-cia', status: 'fixture' },
  { label: 'Liq.Col', icon: 'pi pi-list-check', to: '/liq-col', status: 'fixture' },
  { label: 'Informes', icon: 'pi pi-file', to: '/informes', status: 'fixture' },
  { label: 'Controles', icon: 'pi pi-home', to: '/controles', status: 'blockedSdd' },
  { label: 'Estadisticas', icon: 'pi pi-chart-bar', to: '/estadisticas', status: 'fixture' },
  { label: 'Administracion', icon: 'pi pi-table', to: '/administracion', status: 'blockedSdd' },
  { label: 'Configuracion', icon: 'pi pi-cog', to: '/configuracion', status: 'blockedSdd' },
  { label: 'Conectividad', icon: 'pi pi-code', to: '/conectividad', status: 'blockedSdd' },
  { label: 'By Aunna', icon: 'pi pi-sitemap', to: '/by-aunna', status: 'blockedSdd' },
  { label: 'Logs', icon: 'pi pi-database', to: '/logs', status: 'blockedSdd' },
]

export function getNavigationStatusLabel(item: AppNavigationItem) {
  return navigationStatusLabels[item.status]
}

export function getNavigationStatusShortLabel(item: AppNavigationItem) {
  return navigationStatusShortLabels[item.status]
}

export function getNavigationStatusDescription(item: AppNavigationItem) {
  return navigationStatusDescriptions[item.status]
}

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
