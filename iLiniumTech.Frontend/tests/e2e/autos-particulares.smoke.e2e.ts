import { expect, test } from '@playwright/test'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|AppBuilder|QueryStatic|Pantalla_Polizas|metadata/i
const forbiddenVehicleLeaks = /\b\d{4}\s?[A-Z]{3}\b/

test('autos particulares smoke uses local fixture and blocks backend calls', async ({ page }) => {
  const backendRequests: string[] = []

  await page.route('**/api/**', async (route) => {
    backendRequests.push(route.request().url())
    await route.abort()
  })
  await page.route('http://localhost:5146/**', async (route) => {
    backendRequests.push(route.request().url())
    await route.abort()
  })
  await page.route('http://127.0.0.1:5146/**', async (route) => {
    backendRequests.push(route.request().url())
    await route.abort()
  })

  await page.goto('/autos-particulares')

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
  expect(backendRequests).toEqual([])
})
