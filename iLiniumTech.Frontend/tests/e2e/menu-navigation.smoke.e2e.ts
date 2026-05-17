import { expect, test } from '@playwright/test'

import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|AppBuilder|QueryStatic|ComponentDataSource|IAP_|Pantalla_|metadata/i

test('side menu exposes maturity states and navigates MVP routes without backend calls', async ({
  page,
}) => {
  const backendRequests = await blockBackendRequests(page)

  await loginDemo(page, '/polizas')

  const sideMenu = page.getByRole('complementary', { name: 'Menu principal' })
  await expect(sideMenu).toBeVisible()
  await expect(sideMenu.locator('.side-status-legend .status-operational')).toHaveAttribute(
    'title',
    /Operativo/,
  )
  await expect(sideMenu.locator('.side-status-legend .status-fixture')).toHaveAttribute(
    'title',
    /Fixture/,
  )
  await expect(sideMenu.locator('.side-status-legend .status-parked')).toHaveAttribute(
    'title',
    /Aparcado/,
  )
  await expect(sideMenu.locator('.side-status-legend .status-blockedSdd')).toHaveAttribute(
    'title',
    /Bloqueado SDD/,
  )
  await expect(sideMenu.locator('a[href="/polizas"] .nav-status-dot')).toHaveClass(
    /status-operational/,
  )
  await expect(sideMenu.locator('a[href="/clientes"] .nav-status-dot')).toHaveClass(
    /status-fixture/,
  )
  await expect(sideMenu.locator('.side-nav-disabled:has-text("Autos Particulares")')).toHaveCount(1)
  await expect(page.getByRole('link', { name: 'Autos Particulares', exact: true })).toHaveCount(0)

  await page.getByRole('link', { name: 'Clientes', exact: true }).click()
  await expect(page).toHaveURL(/\/clientes$/)
  await expect(page.getByRole('heading', { name: 'Clientes' })).toBeVisible()
  await expect(page.getByText('Fixture local sin API', { exact: true })).toBeVisible()

  await page.getByRole('link', { name: 'Siniestros', exact: true }).click()
  await expect(page).toHaveURL(/\/siniestros$/)
  await expect(page.getByRole('heading', { name: 'Siniestros' })).toBeVisible()
  await expect(page.getByText('Detalle y exportacion pendientes')).toBeVisible()

  await page.getByRole('link', { name: 'Polizas', exact: true }).hover()
  await page.getByRole('link', { name: 'Flotas', exact: true }).click()
  await expect(page).toHaveURL(/\/polizas\/flotas$/)
  await expect(page.getByRole('heading', { name: 'Polizas Flotas' })).toBeVisible()
  await expect(page.getByText('Scope aparcado hasta SDD')).toBeVisible()

  await page.getByRole('link', { name: 'Polizas', exact: true }).hover()
  await page.getByRole('link', { name: 'Colectivas', exact: true }).click()
  await expect(page).toHaveURL(/\/polizas\/colectivas$/)
  await expect(page.getByRole('heading', { name: 'Polizas Colectivas' })).toBeVisible()
  await expect(page.getByText('Scope aparcado hasta SDD')).toBeVisible()

  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  expectNoBackendRequests(backendRequests)
})
