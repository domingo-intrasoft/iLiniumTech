import { expect, test } from '@playwright/test'

import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|AppBuilder|QueryStatic|Pantalla_Polizas|metadata/i
const forbiddenVehicleLeaks = /\b\d{4}\s?[A-Z]{3}\b/

test('autos particulares smoke uses local fixture and blocks backend calls', async ({ page }) => {
  const backendRequests = await blockBackendRequests(page)

  await loginDemo(page, '/autos-particulares')

  await expect(page.getByRole('heading', { name: 'Autos Particulares' })).toBeVisible()
  await expect(page.getByText('Solo lectura')).toBeVisible()
  await expect(page.getByText('Fixture local autos')).toBeVisible()
  await expect(page.getByText('Ramo Autos')).toBeVisible()
  await expect(page.getByText('Particulares pendiente UAT')).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()
  await expect(page.getByText('AUTO-2026-0001')).toBeVisible()
  await expect(page.getByText('AUTO-2026-0002')).toBeVisible()

  const numeroFilter = page.getByRole('searchbox', { name: 'Poliza auto', exact: true })

  await numeroFilter.fill('0002')
  await page.getByRole('button', { name: 'Buscar' }).click()

  await expect(page.getByText('AUTO-2026-0002')).toBeVisible()
  await expect(page.getByText('AUTO-2026-0001')).toHaveCount(0)
  await expect(page.getByText('1 auto particular')).toBeVisible()

  await page.getByRole('button', { name: 'Limpiar Filtros' }).click()

  await expect(numeroFilter).toHaveValue('')
  await expect(page.getByText('AUTO-2026-0001')).toBeVisible()
  await expect(page.getByText('AUTO-2026-0002')).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenVehicleLeaks)
  expectNoBackendRequests(backendRequests)
})
