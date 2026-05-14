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
