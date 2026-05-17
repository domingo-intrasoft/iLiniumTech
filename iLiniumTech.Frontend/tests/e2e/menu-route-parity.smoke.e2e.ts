import { expect, test, type Page } from '@playwright/test'

import { appNavigation, type AppNavigationItem } from '../../src/layout/appNavigation'
import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|QueryStatic|ComponentDataSource|IAP_|Pantalla_|appsettings/i
const forbiddenSecretLeaks =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|client_secret|access_token|Bearer\s+[A-Za-z0-9]|api[-_ ]?key|[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i

interface MenuRouteExpectation {
  path: string
  label: string
  heading: string
  status: AppNavigationItem['status']
  marker: string
  parentLabel?: string
}

function flattenNavigation(items: readonly AppNavigationItem[], parentLabel?: string) {
  return items.flatMap((item) => [
    { ...item, parentLabel },
    ...flattenNavigation(item.children ?? [], item.label),
  ])
}

const menuRouteExpectations: MenuRouteExpectation[] = [
  {
    path: '/agenda',
    label: 'Agenda',
    heading: 'Agenda',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/clientes',
    label: 'Clientes',
    heading: 'Clientes',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/propuestas',
    label: 'Propuestas',
    heading: 'Propuestas',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/polizas',
    label: 'Polizas',
    heading: 'Polizas',
    status: 'operational',
    marker: 'Fixture local',
  },
  {
    path: '/polizas/flotas',
    label: 'Flotas',
    heading: 'Polizas Flotas',
    status: 'blockedSdd',
    marker: 'Scope aparcado hasta SDD',
    parentLabel: 'Polizas',
  },
  {
    path: '/polizas/colectivas',
    label: 'Colectivas',
    heading: 'Polizas Colectivas',
    status: 'blockedSdd',
    marker: 'Scope aparcado hasta SDD',
    parentLabel: 'Polizas',
  },
  {
    path: '/recibos',
    label: 'Recibos',
    heading: 'Recibos',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/suplementos',
    label: 'Suplementos',
    heading: 'Suplementos',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/siniestros',
    label: 'Siniestros',
    heading: 'Siniestros',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/liq-cia',
    label: 'Liq.Cia',
    heading: 'Liquidaciones de compania',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/liq-col',
    label: 'Liq.Col',
    heading: 'Liquidaciones de colaborador',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/informes',
    label: 'Informes',
    heading: 'Informes',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/controles',
    label: 'Controles',
    heading: 'Controles',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
  {
    path: '/estadisticas',
    label: 'Estadisticas',
    heading: 'Estadisticas',
    status: 'fixture',
    marker: 'Fixture local sin API',
  },
  {
    path: '/administracion',
    label: 'Administracion',
    heading: 'Administracion',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
  {
    path: '/configuracion',
    label: 'Configuracion',
    heading: 'Configuracion',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
  {
    path: '/conectividad',
    label: 'Conectividad',
    heading: 'Conectividad',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
  {
    path: '/by-aunna',
    label: 'By Aunna',
    heading: 'By Aunna',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
  {
    path: '/logs',
    label: 'Logs',
    heading: 'Logs',
    status: 'blockedSdd',
    marker: 'Fixture local sin API',
  },
]

const expectedRoutePaths = new Set(menuRouteExpectations.map((route) => route.path))
const navigableMenuItems = flattenNavigation(appNavigation).filter(
  (item) => item.to && !item.disabled,
)

function routePath(item: AppNavigationItem & { parentLabel?: string }) {
  return item.to ?? ''
}

async function assertMenuLinkParity(page: Page, route: MenuRouteExpectation) {
  const sideMenu = page.getByRole('complementary', { name: 'Menu principal' })

  if (route.parentLabel) {
    await sideMenu.getByRole('link', { name: route.parentLabel, exact: true }).hover()
  }

  const link = sideMenu.getByRole('link', { name: route.label, exact: true })
  await expect(link).toBeVisible()
  await expect(sideMenu.locator(`a[href="${route.path}"] .nav-status-dot`)).toHaveClass(
    new RegExp(`status-${route.status}`),
  )
}

async function assertProtectedRoute(page: Page, route: MenuRouteExpectation) {
  await page.goto(route.path)
  await expect(page).toHaveURL(new RegExp(`${route.path}(\\?.*)?$`))
  await expect(page.getByRole('heading', { name: route.heading })).toBeVisible()
  await expect(page.getByText(route.marker, { exact: true }).first()).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenSecretLeaks)
}

test('static menu routes match appNavigation and render without backend calls', async ({
  page,
}) => {
  const backendRequests = await blockBackendRequests(page)

  expect(navigableMenuItems.map(routePath).sort()).toEqual([...expectedRoutePaths].sort())

  await loginDemo(page, '/polizas')

  const parkedAutos = flattenNavigation(appNavigation).find(
    (item) => item.to === '/autos-particulares',
  )
  expect(parkedAutos?.disabled).toBe(true)
  expect(parkedAutos?.status).toBe('parked')
  await expect(page.locator('.side-nav-disabled:has-text("Autos Particulares")')).toHaveCount(1)
  await expect(page.getByRole('link', { name: 'Autos Particulares', exact: true })).toHaveCount(0)

  for (const route of menuRouteExpectations) {
    await assertMenuLinkParity(page, route)
    await assertProtectedRoute(page, route)
  }

  expectNoBackendRequests(backendRequests)
})
