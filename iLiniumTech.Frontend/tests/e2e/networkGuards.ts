import { expect, type Page } from '@playwright/test'

const backendPatterns = ['**/api/**', 'http://localhost:5146/**', 'http://127.0.0.1:5146/**']

export async function blockBackendRequests(page: Page) {
  const backendRequests: string[] = []

  for (const pattern of backendPatterns) {
    await page.route(pattern, async (route) => {
      backendRequests.push(route.request().url())
      await route.abort()
    })
  }

  return backendRequests
}

export function expectNoBackendRequests(backendRequests: readonly string[]) {
  expect(backendRequests).toEqual([])
}
