import { expect, test } from '@playwright/test'

import { loginDemo } from './auth'

const forbiddenRuntimeLeaks = /connectionString|SELECT \*|AppBuilder|QueryStatic|Pantalla_Polizas/i

test('polizas smoke uses local fixture without leaking AppBuilder runtime details', async ({
  page,
}) => {
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

  await loginDemo(page, '/polizas')

  await expect(page.getByText('Solo lectura')).toBeVisible()
  await expect(page.getByText('Fixture local')).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()
  await expect(page.getByRole('link', { name: 'Polizas', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Autos Particulares', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(
    page.getByRole('link', { name: 'Ver detalle de poliza POL-2026-0001', exact: true }),
  ).toBeVisible()

  await page.getByRole('link', { name: 'Ver detalle de poliza POL-2026-0001', exact: true }).click()
  await expect(page.getByRole('heading', { name: 'POL-2026-0001' })).toBeVisible()
  await expect(page.getByText('Detalle / POL-2026-0001')).toBeVisible()
  await page.goto('/polizas')

  const polizaFilter = page.getByRole('searchbox', { name: 'Poliza', exact: true })

  await polizaFilter.fill('0002')
  await page.getByRole('button', { name: 'Buscar' }).click()

  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toHaveCount(0)
  await expect(page.getByText('1 poliza')).toBeVisible()

  await page.getByRole('button', { name: 'Limpiar Filtros' }).click()

  await expect(polizaFilter).toHaveValue('')
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  expect(backendRequests).toEqual([])
})
