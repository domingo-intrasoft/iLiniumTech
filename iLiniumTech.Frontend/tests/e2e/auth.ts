import { expect, type Page } from '@playwright/test'

export async function loginDemo(page: Page, path: string) {
  await page.goto(path)
  await expect(page.getByRole('heading', { name: 'Iniciar sesion' })).toBeVisible()
  await page.getByLabel('Usuario', { exact: true }).fill('e2e')
  await page.getByLabel('Contrasena', { exact: true }).fill('demo')
  await page.getByRole('button', { name: 'Entrar', exact: true }).click()
  await expect(page).toHaveURL(new RegExp(`${path}(?:\\?.*)?$`))
}
