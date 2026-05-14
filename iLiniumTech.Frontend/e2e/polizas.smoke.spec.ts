import { expect, test } from '@playwright/test'

test('shows local polizas and opens a policy detail', async ({ page }) => {
  await page.goto('/polizas')

  await expect(page.getByRole('main')).toBeVisible()
  await expect(page.getByText(/Resultado:\s*2\s*polizas/)).toBeVisible()

  const table = page.getByRole('table')
  await expect(table).toBeVisible()
  await expect(table.getByRole('columnheader', { name: 'Poliza' })).toBeVisible()
  await expect(table.getByRole('link', { name: 'POL-2026-0001' })).toBeVisible()
  await expect(table.getByText('Cliente anonimo 1')).toBeVisible()

  await table.getByRole('link', { name: 'POL-2026-0001' }).click()

  await expect(page).toHaveURL(/\/polizas\/POL-1001$/)
  await expect(page.getByRole('heading', { level: 1, name: 'POL-2026-0001' })).toBeVisible()
  await expect(page.locator('.detail-hero').getByText('Cliente anonimo 1')).toBeVisible()
  await expect(page.locator('.detail-premium').getByText('Prima anual')).toBeVisible()
})

test('filters local polizas and clears criteria', async ({ page }) => {
  await page.goto('/polizas')

  const polizaFilter = page
    .locator('.filter-field')
    .filter({ has: page.locator('span', { hasText: /^Poliza$/ }) })
    .locator('input')

  await polizaFilter.fill('0002')
  await page.getByRole('button', { name: 'Buscar' }).click()

  await expect(page.getByText(/Resultado:\s*1\s*polizas/)).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002' })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0001' })).toBeHidden()

  await page.getByRole('button', { name: /Limpiar Filtros/ }).click()

  await expect(page.getByText(/Resultado:\s*2\s*polizas/)).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0001' })).toBeVisible()
  await expect(page.getByRole('link', { name: 'POL-2026-0002' })).toBeVisible()
})

test('validates pagination controls and safe not-found state', async ({ page }) => {
  await page.goto('/polizas')

  await page.locator('.page-size-control select').selectOption('10')

  await expect(page.getByText('1-2 visibles')).toBeVisible()
  await expect(page.getByRole('button', { name: /Anterior/ })).toBeDisabled()
  await expect(page.getByRole('button', { name: /Siguiente/ })).toBeDisabled()

  await page.goto('/polizas/NOPE')

  const error = page.getByText('Poliza no encontrada.')
  await expect(error).toBeVisible()
  await expect(page.getByText(/SELECT|ConnectionString|Data Source|Server=|C:\\/i)).toHaveCount(0)
})
