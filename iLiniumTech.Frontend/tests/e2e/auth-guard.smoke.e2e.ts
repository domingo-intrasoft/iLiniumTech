import { expect, test } from '@playwright/test'

import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

async function expectLoginRedirect(pageUrl: string, expectedRedirect: string) {
  const url = new URL(pageUrl)

  expect(url.pathname).toBe('/login')
  expect(url.searchParams.get('redirect')).toBe(expectedRedirect)
}

test('protected MVP routes require demo login and logout clears access', async ({ page }) => {
  const backendRequests = await blockBackendRequests(page)

  await page.goto('/clientes')

  await expect(page.getByRole('heading', { name: 'Iniciar sesion' })).toBeVisible()
  await expectLoginRedirect(page.url(), '/clientes')

  await page.getByLabel('Usuario', { exact: true }).fill('e2e')
  await page.getByLabel('Contrasena', { exact: true }).fill('demo')
  await page.getByRole('button', { name: 'Entrar', exact: true }).click()

  await expect(page).toHaveURL(/\/clientes$/)
  await expect(page.getByRole('heading', { name: 'Clientes' })).toBeVisible()
  await expect(page.getByRole('complementary', { name: 'Menu principal' })).toBeVisible()

  await page.getByRole('button', { name: 'Salir' }).click()

  await expect(page).toHaveURL(/\/login$/)
  await expect(page.getByRole('heading', { name: 'Iniciar sesion' })).toBeVisible()

  const authStorageKeys = await page.evaluate(() =>
    Object.keys(window.sessionStorage).filter((key) => key.includes('iliniumtech.auth')),
  )
  expect(authStorageKeys).toEqual([])

  await page.goto('/polizas')

  await expect(page.getByRole('heading', { name: 'Iniciar sesion' })).toBeVisible()
  await expectLoginRedirect(page.url(), '/polizas')
  await expect(page.getByRole('heading', { name: 'Polizas' })).toHaveCount(0)

  expectNoBackendRequests(backendRequests)
})
