import { describe, expect, it } from 'vitest'

import {
  appNavigation,
  getNavigationStatusDescription,
  getNavigationStatusLabel,
  getNavigationUnavailableReason,
  hasNavigationPermission,
} from './appNavigation'

describe('appNavigation static permission state', () => {
  it('keeps Polizas as static source navigation with an iLiniumTech permission', () => {
    const polizas = appNavigation.find((item) => item.label === 'Polizas')

    expect(polizas?.to).toBe('/polizas')
    expect(polizas && getNavigationStatusLabel(polizas)).toBe('Operativo')
    expect(polizas?.requiredPermission).toBe('polizas.read')
    expect(polizas?.children?.find((item) => item.label === 'Autos Particulares')?.disabled).toBe(
      true,
    )
    expect(polizas?.children?.find((item) => item.label === 'Autos Particulares')?.status).toBe(
      'parked',
    )
    expect(polizas?.children?.find((item) => item.label === 'Flotas')?.to).toBe('/polizas/flotas')
    expect(polizas?.children?.find((item) => item.label === 'Flotas')?.status).toBe('blockedSdd')
    expect(polizas?.children?.find((item) => item.label === 'Colectivas')?.to).toBe(
      '/polizas/colectivas',
    )
    expect(JSON.stringify(appNavigation)).not.toMatch(/IAP_|QueryStatic|metadata/i)
  })

  it('opens the non-polizas MVP pages as static navigation entries', () => {
    const expectedRoutes = [
      '/agenda',
      '/clientes',
      '/propuestas',
      '/recibos',
      '/suplementos',
      '/siniestros',
      '/liq-cia',
      '/liq-col',
      '/informes',
      '/controles',
      '/estadisticas',
      '/administracion',
      '/configuracion',
      '/conectividad',
      '/by-aunna',
      '/logs',
    ]

    const routes = appNavigation.flatMap((item) => [
      item.to,
      ...(item.children ?? []).map((child) => child.to),
    ])

    for (const route of expectedRoutes) {
      expect(routes).toContain(route)
    }

    expect(appNavigation.filter((item) => item.disabled)).toHaveLength(0)
  })

  it('classifies visible routes by MVP maturity without activating backend features', () => {
    const clientes = appNavigation.find((item) => item.label === 'Clientes')!
    const logs = appNavigation.find((item) => item.label === 'Logs')!

    expect(getNavigationStatusLabel(clientes)).toBe('Fixture')
    expect(getNavigationStatusDescription(clientes)).toContain('fixture local sin API real')
    expect(getNavigationStatusLabel(logs)).toBe('Bloqueado SDD')
    expect(getNavigationStatusDescription(logs)).toContain('SDD')
  })

  it('does not disable legacy API-key mode when /api/me cannot declare permissions yet', () => {
    const polizas = appNavigation.find((item) => item.label === 'Polizas')!

    expect(hasNavigationPermission(polizas, { authMode: 'ApiKey', permissions: [] })).toBe(true)
    expect(getNavigationUnavailableReason(polizas, { authMode: 'ApiKey', permissions: [] })).toBe(
      null,
    )
  })

  it('marks Polizas unavailable when explicit backend permissions omit polizas.read', () => {
    const polizas = appNavigation.find((item) => item.label === 'Polizas')!

    expect(hasNavigationPermission(polizas, { permissions: ['polizas.catalogs'] })).toBe(false)
    expect(getNavigationUnavailableReason(polizas, { permissions: ['polizas.catalogs'] })).toBe(
      'permission',
    )
  })
})
