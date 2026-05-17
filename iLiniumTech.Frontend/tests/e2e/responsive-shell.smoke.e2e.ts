import { expect, test, type Locator, type Page } from '@playwright/test'

import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const mobileViewport = { width: 390, height: 844 }

async function expectNoGlobalHorizontalOverflow(page: Page) {
  const overflow = await page.evaluate(() => {
    const documentWidth = Math.max(document.documentElement.scrollWidth, document.body.scrollWidth)

    return documentWidth - window.innerWidth
  })

  expect(overflow).toBeLessThanOrEqual(4)
}

async function expectWithinViewport(page: Page, locator: Locator) {
  const box = await locator.boundingBox()
  const viewport = page.viewportSize()

  expect(box).not.toBeNull()
  expect(viewport).not.toBeNull()

  expect(box!.x).toBeGreaterThanOrEqual(0)
  expect(box!.x + box!.width).toBeLessThanOrEqual(viewport!.width + 1)
}

test('login, side menu, and fixture pages remain usable on mobile viewport', async ({ page }) => {
  await page.setViewportSize(mobileViewport)
  const backendRequests = await blockBackendRequests(page)

  await page.goto('/login?redirect=/polizas')

  await expect(page.getByRole('heading', { name: 'iLiniumTech' })).toBeVisible()
  await expect(page.getByRole('heading', { name: 'Iniciar sesion' })).toBeVisible()
  await expect(page.getByLabel('Usuario', { exact: true })).toBeVisible()
  await expect(page.getByLabel('Contrasena', { exact: true })).toBeVisible()
  await expectWithinViewport(page, page.locator('.login-panel'))
  await expectNoGlobalHorizontalOverflow(page)

  await page.getByLabel('Usuario', { exact: true }).fill('e2e')
  await page.getByLabel('Contrasena', { exact: true }).fill('demo')
  await page.getByRole('button', { name: 'Entrar', exact: true }).click()

  await expect(page).toHaveURL(/\/polizas(?:\?.*)?$/)
  await expect(page.getByRole('heading', { name: 'Polizas' })).toBeVisible()
  await expect(page.getByRole('complementary', { name: 'Menu principal' })).toBeVisible()
  await expect(page.getByRole('button', { name: 'Ocultar menu' })).toHaveAttribute(
    'aria-expanded',
    'true',
  )
  await expect(page.locator('#app-side-menu')).not.toHaveAttribute('inert', /.*/)
  await expectNoGlobalHorizontalOverflow(page)

  const polizasTableScroll = page.locator('.table-scroll').first()
  await expect(polizasTableScroll).toBeVisible()

  const tableMetrics = await polizasTableScroll.evaluate((element) => ({
    clientWidth: element.clientWidth,
    scrollWidth: element.scrollWidth,
  }))

  expect(tableMetrics.clientWidth).toBeLessThanOrEqual(mobileViewport.width)
  expect(tableMetrics.scrollWidth).toBeGreaterThan(tableMetrics.clientWidth)

  await page.getByRole('button', { name: 'Ocultar menu' }).click()

  await expect(page.getByRole('button', { name: 'Mostrar menu' })).toHaveAttribute(
    'aria-expanded',
    'false',
  )
  await expect(page.locator('#app-side-menu')).toHaveAttribute('aria-hidden', 'true')
  await expect(page.locator('#app-side-menu')).toHaveAttribute('inert', /.*/)
  await expectNoGlobalHorizontalOverflow(page)

  await page.getByRole('button', { name: 'Mostrar menu' }).click()
  await expect(page.locator('#app-side-menu')).not.toHaveAttribute('inert', /.*/)
  await page.getByRole('link', { name: 'Clientes', exact: true }).click()

  await expect(page).toHaveURL(/\/clientes$/)
  await expect(page.getByRole('heading', { name: 'Clientes' })).toBeVisible()
  await expect(page.getByText('Fixture local sin API', { exact: true })).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()

  const firstFilterField = page.locator('.filter-row input, .filter-row select').first()
  await expect(firstFilterField).toBeVisible()
  await expectWithinViewport(page, firstFilterField)
  await expectNoGlobalHorizontalOverflow(page)

  expectNoBackendRequests(backendRequests)
})
