import { beforeEach, describe, expect, it, vi } from 'vitest'

const mocks = vi.hoisted(() => ({
  get: vi.fn(),
  assertRuntimeConfigReady: vi.fn(),
}))

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    get: mocks.get,
  },
}))

vi.mock('@/services/runtimeConfig', () => ({
  assertRuntimeConfigReady: mocks.assertRuntimeConfigReady,
}))

import { getSiniestrosCatalogs, searchSiniestros } from './siniestrosApi'

describe('siniestros API adapter', () => {
  beforeEach(() => {
    vi.unstubAllEnvs()
    mocks.get.mockReset()
    mocks.assertRuntimeConfigReady.mockReset()
  })

  it('uses the sanitized fixture only when backend mode is disabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchSiniestros({ referencia: '0002', page: 1, pageSize: 25 })

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(result.total).toBe(1)
    expect(result.items[0]?.referencia).toBe('SIN-2026-0002')
    expect(JSON.stringify(result)).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
  })

  it('exposes local catalogs without calling the backend in fixture mode', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const catalogs = await getSiniestrosCatalogs()

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(catalogs.estados).toEqual(['En revision', 'Abierto', 'Cerrado'])
    expect(catalogs.prioridades).toEqual(['Alta', 'Media', 'Baja'])
  })

  it('calls the Siniestros catalogs endpoint when backend mode is enabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        estados: ['Abierto'],
        prioridades: ['Alta'],
      },
    })

    await getSiniestrosCatalogs()

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/siniestros/catalogs')
  })

  it('calls the Siniestros search endpoint with the minimized read-only params', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        items: [],
        page: 2,
        pageSize: 10,
        total: 0,
      },
    })

    await searchSiniestros({
      referencia: 'SIN',
      poliza: 'POL',
      estado: 'Abierto',
      prioridad: 'Alta',
      fechaDesde: '2026-01-01',
      page: 2,
      pageSize: 10,
      sort: 'fechaSiniestro:desc',
    })

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/siniestros', {
      params: {
        referencia: 'SIN',
        poliza: 'POL',
        estado: 'Abierto',
        prioridad: 'Alta',
        fechaSiniestroDesde: '2026-01-01',
        page: 2,
        pageSize: 10,
        sort: 'fechaSiniestro:desc',
      },
    })
  })
})
