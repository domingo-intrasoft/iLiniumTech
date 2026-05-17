import { expect, test } from '@playwright/test'

import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks = /connectionString|SELECT \*|AppBuilder|QueryStatic|Pantalla_Polizas/i

test('polizas smoke uses local fixture without leaking AppBuilder runtime details', async ({
  page,
}) => {
  const backendRequests = await blockBackendRequests(page)

  await loginDemo(page, '/polizas')

  await expect(page.getByText('Solo lectura')).toBeVisible()
  await expect(page.getByText('Fixture local')).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()
  await expect(page.getByRole('link', { name: 'Polizas', exact: true })).toBeVisible()
  await expect(page.getByText('Autos Particulares', { exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Autos Particulares', exact: true })).toHaveCount(0)
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(
    page.getByRole('link', { name: 'Ver detalle de poliza POL-2026-0001', exact: true }),
  ).toBeVisible()
  await expect(page.getByRole('link', { name: 'Polizas de flota', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'Polizas colectivas', exact: true })).toBeVisible()
  await expect(page.getByRole('button', { name: 'Polizas Externas', exact: true })).toBeDisabled()

  await page.getByRole('link', { name: 'Polizas de flota', exact: true }).click()
  await expect(page).toHaveURL(/\/polizas\/flotas$/)
  await expect(page.getByRole('heading', { name: 'Polizas Flotas' })).toBeVisible()
  await expect(page.getByText('Scope aparcado hasta SDD')).toBeVisible()
  await page.goto('/polizas')

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
  await expect(page).toHaveURL(/numero=0002/)

  await page.getByRole('link', { name: 'Ver detalle de poliza POL-2026-0002', exact: true }).click()
  await expect(page.getByRole('heading', { name: 'POL-2026-0002' })).toBeVisible()
  await expect(page).toHaveURL(/numero=0002/)
  await page.locator('#poliza-detail-content').getByRole('link', { name: 'Polizas' }).click()
  await expect(page).toHaveURL(/numero=0002/)
  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toHaveCount(0)

  await page.getByRole('button', { name: 'Limpiar Filtros' }).click()

  await expect(polizaFilter).toHaveValue('')
  await expect(page.getByRole('link', { name: 'POL-2026-0001', exact: true })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002', exact: true })).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  expectNoBackendRequests(backendRequests)
})
