import { expect, test, type Page } from '@playwright/test'

import { loginDemo } from './auth'
import { blockBackendRequests, expectNoBackendRequests } from './networkGuards'

const forbiddenRuntimeLeaks =
  /connectionString|SELECT \*|QueryStatic|ComponentDataSource|IAP_|Pantalla_|metadata|appsettings/i
const forbiddenSecretLeaks =
  /BEGIN (RSA|OPENSSH|PRIVATE) KEY|password=|pwd=|secret|token|IBAN|[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i
const forbiddenRealAmounts = /\b\d{1,3}(?:[.,]\d{3})*[.,]\d{2}\s?(?:EUR|€)|\b\d+[.,]\d{2}\b/

interface DomainSmokeCase {
  path: string
  heading: string
  filterSelector: string
  searchText: string
  expectedAfterSearch: string
  initialMarker: string
  summaryAfterSearch: string
}

const domainSmokeCases: DomainSmokeCase[] = [
  {
    path: '/agenda',
    heading: 'Agenda',
    filterSelector: '#agenda-filter-texto',
    searchText: 'SIN-DEMO-0002',
    expectedAfterSearch: 'AGE-2026-0002',
    initialMarker: 'AGE-2026-0001',
    summaryAfterSearch: '1 evento',
  },
  {
    path: '/propuestas',
    heading: 'Propuestas',
    filterSelector: '#propuestas-filter-referencia',
    searchText: '0002',
    expectedAfterSearch: 'PROP-2026-0002',
    initialMarker: 'PROP-2026-0001',
    summaryAfterSearch: '1 propuesta',
  },
  {
    path: '/recibos',
    heading: 'Recibos',
    filterSelector: '#recibos-filter-recibo',
    searchText: '0002',
    expectedAfterSearch: 'REC-2026-0002',
    initialMarker: 'REC-2026-0001',
    summaryAfterSearch: '1 recibo',
  },
  {
    path: '/suplementos',
    heading: 'Suplementos',
    filterSelector: '#suplementos-filter-texto',
    searchText: '0002',
    expectedAfterSearch: 'SUP-2026-0002',
    initialMarker: 'SUP-2026-0001',
    summaryAfterSearch: '1 suplemento',
  },
  {
    path: '/siniestros',
    heading: 'Siniestros',
    filterSelector: '#siniestros-filter-referencia',
    searchText: '0002',
    expectedAfterSearch: 'SIN-2026-0002',
    initialMarker: 'SIN-2026-0001',
    summaryAfterSearch: '1 siniestro',
  },
  {
    path: '/liq-cia',
    heading: 'Liquidaciones de compania',
    filterSelector: '#liq-cia-filter-texto',
    searchText: '0002',
    expectedAfterSearch: 'LCIA-2026-0002',
    initialMarker: 'LCIA-2026-0001',
    summaryAfterSearch: '1 liquidacion',
  },
  {
    path: '/liq-col',
    heading: 'Liquidaciones de colaborador',
    filterSelector: '#liq-col-filter-texto',
    searchText: 'anonimo c',
    expectedAfterSearch: 'LC-2026-0003',
    initialMarker: 'LC-2026-0001',
    summaryAfterSearch: '1 liquidacion',
  },
]

async function assertFixtureDomainPage(page: Page, smokeCase: DomainSmokeCase) {
  await page.goto(smokeCase.path)
  await expect(page).toHaveURL(new RegExp(`${smokeCase.path}$`))
  await expect(page.getByRole('heading', { name: smokeCase.heading })).toBeVisible()
  await expect(page.getByText('Solo lectura')).toBeVisible()
  await expect(page.getByText('Fixture local sin API', { exact: true })).toBeVisible()
  await expect(page.getByRole('table')).toBeVisible()
  await expect(page.getByText(smokeCase.initialMarker)).toBeVisible()

  const textFilter = page.locator(smokeCase.filterSelector)
  await textFilter.fill(smokeCase.searchText)
  await page.locator('.search-action-buttons .primary-action').click()

  await expect(page.getByText(smokeCase.summaryAfterSearch)).toBeVisible()
  await expect(page.getByText(smokeCase.expectedAfterSearch)).toBeVisible()
  await expect(page.getByText(smokeCase.initialMarker)).toHaveCount(0)

  await page.getByRole('button', { name: 'Limpiar Filtros' }).click()

  await expect(textFilter).toHaveValue('')
  await expect(page.getByText(smokeCase.initialMarker)).toBeVisible()
  await expect(page.locator('body')).not.toContainText(forbiddenRuntimeLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenSecretLeaks)
  await expect(page.locator('body')).not.toContainText(forbiddenRealAmounts)
}

test('domain fixture MVP pages stay read-only, filter locally, and avoid backend calls', async ({
  page,
}) => {
  const backendRequests = await blockBackendRequests(page)

  await loginDemo(page, domainSmokeCases[0]!.path)

  for (const smokeCase of domainSmokeCases) {
    await assertFixtureDomainPage(page, smokeCase)
  }

  expectNoBackendRequests(backendRequests)
})
