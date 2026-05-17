import { expect, test, type Page } from '@playwright/test'

import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|QueryStatic|ComponentDataSource|IAP_|Pantalla_|appsettings/i
const forbiddenSecretLeaks =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|client_secret|access_token|Bearer\s+[A-Za-z0-9]|api[-_ ]?key|https?:\/\/|[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i
const forbiddenRealAmounts = /\b\d{1,3}(?:[.,]\d{3})*[.,]\d{2}\s?(?:EUR|€)|\b\d+[.,]\d{2}\b/

interface BlockedPageSmokeCase {
  path: string
  heading: string
  filterSelector: string
  searchText: string
  initialMarker: string
  expectedAfterSearch: string
  summaryAfterSearch: string
  safetyMarker?: string
}

const blockedPageSmokeCases: BlockedPageSmokeCase[] = [
  {
    path: '/administracion',
    heading: 'Administracion',
    filterSelector: '#administracion-filter-texto',
    searchText: 'auditoria',
    initialMarker: 'ADM-DEMO-ACCESOS',
    expectedAfterSearch: 'ADM-DEMO-AUDITORIA',
    summaryAfterSearch: '1 registro',
    safetyMarker: 'Superficie sensible',
  },
  {
    path: '/configuracion',
    heading: 'Configuracion',
    filterSelector: '#configuracion-filter-texto',
    searchText: 'seguridad',
    initialMarker: 'CFG-DEMO-GENERAL',
    expectedAfterSearch: 'CFG-DEMO-SEGURIDAD',
    summaryAfterSearch: '1 registro',
    safetyMarker: 'Superficie sensible',
  },
  {
    path: '/conectividad',
    heading: 'Conectividad',
    filterSelector: '#conectividad-filter-text',
    searchText: 'allowlist',
    initialMarker: 'CON-DEMO-001',
    expectedAfterSearch: 'CON-DEMO-004',
    summaryAfterSearch: '1 entrada candidata',
  },
  {
    path: '/logs',
    heading: 'Logs',
    filterSelector: '#logs-filter-text',
    searchText: 'request',
    initialMarker: 'LOG-DEMO-001',
    expectedAfterSearch: 'LOG-DEMO-003',
    summaryAfterSearch: '1 log demo',
  },
  {
    path: '/by-aunna',
    heading: 'By Aunna',
    filterSelector: '#by-aunna-filter-text',
    searchText: 'recursos',
    initialMarker: 'AUN-DEMO-001',
    expectedAfterSearch: 'AUN-DEMO-003',
    summaryAfterSearch: '1 elemento candidato',
  },
  {
    path: '/controles',
    heading: 'Controles',
    filterSelector: '#controles-filter-texto',
    searchText: 'datos',
    initialMarker: 'CTRL-2026-0001',
    expectedAfterSearch: 'CTRL-2026-0002',
    summaryAfterSearch: '1 control candidato',
  },
  {
    path: '/estadisticas',
    heading: 'Estadisticas',
    filterSelector: '#estadisticas-filter-texto',
    searchText: 'recibos',
    initialMarker: 'EST-2026-0001',
    expectedAfterSearch: 'EST-2026-0002',
    summaryAfterSearch: '1 KPI candidato',
  },
  {
    path: '/informes',
    heading: 'Informes',
    filterSelector: '#informes-filter-texto',
    searchText: 'recibos',
    initialMarker: 'Polizas en vigor',
    expectedAfterSearch: 'Recibos y remesas',
    summaryAfterSearch: '1 categoria candidata',
  },
]

async function assertBlockedPage(page: Page, smokeCase: BlockedPageSmokeCase) {
  await page.goto(smokeCase.path)
  await expect(page).toHaveURL(new RegExp(`${smokeCase.path}$`))
  await expect(page.getByRole('heading', { name: smokeCase.heading })).toBeVisible()
  await expect(
    page.getByText(smokeCase.safetyMarker ?? 'Solo lectura', { exact: true }),
  ).toBeVisible()
  await expect(page.getByText('Fixture local sin API', { exact: true }).first()).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()
  await expect(page.getByText(smokeCase.initialMarker)).toBeVisible()

  const textFilter = page.locator(smokeCase.filterSelector)
  await textFilter.fill(smokeCase.searchText)
  await page.locator('.search-action-buttons .primary-action').click()

  await expect(page.getByText(smokeCase.summaryAfterSearch, { exact: true })).toBeVisible()
  await expect(page.getByText(smokeCase.expectedAfterSearch)).toBeVisible()
  await expect(page.getByText(smokeCase.initialMarker)).toHaveCount(0)

  await page.getByRole('button', { name: 'Limpiar Filtros' }).click()

  await expect(textFilter).toHaveValue('')
  await expect(page.getByText(smokeCase.initialMarker)).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenSecretLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenRealAmounts)
}

test('blocked technical MVP pages remain read-only and do not expose secrets or backend calls', async ({
  page,
}) => {
  const backendRequests = await blockBackendRequests(page)

  await loginDemo(page, blockedPageSmokeCases[0]!.path)

  for (const smokeCase of blockedPageSmokeCases) {
    await assertBlockedPage(page, smokeCase)
  }

  expectNoBackendRequests(backendRequests)
})
