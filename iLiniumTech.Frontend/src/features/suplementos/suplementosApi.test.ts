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

import { getSuplementosCatalogs, searchSuplementos } from './suplementosApi'

describe('suplementos API adapter', () => {
  beforeEach(() => {
    vi.unstubAllEnvs()
    mocks.get.mockReset()
    mocks.assertRuntimeConfigReady.mockReset()
  })

  it('uses the sanitized fixture only when backend mode is disabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchSuplementos({ texto: '0002', page: 1, pageSize: 25 })

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(result.total).toBe(1)
    expect(result.items[0]?.referencia).toBe('SUP-2026-0002')
    expect(JSON.stringify(result)).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
    expect(JSON.stringify(result)).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
  })

  it('exposes local catalogs without calling the backend in fixture mode', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const catalogs = await getSuplementosCatalogs()

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(catalogs.tipos).toEqual([
      'Alta de riesgo',
      'Regularizacion',
      'Domiciliacion',
      'Renovacion',
    ])
    expect(catalogs.situaciones).toEqual(['Pendiente', 'En revision', 'Validado', 'Bloqueado'])
  })

  it('calls the Suplementos catalogs endpoint when backend mode is enabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        tipos: ['No informado'],
        situaciones: ['No informado'],
      },
    })

    await getSuplementosCatalogs()

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/suplementos/catalogs')
  })

  it('calls the Suplementos search endpoint with minimized read-only params', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        items: [],
        page: 2,
        pageSize: 10,
        total: 0,
      },
    })

    await searchSuplementos({
      texto: 'SUP',
      poliza: 'POL',
      tipo: 'Regularizacion',
      situacion: 'Validado',
      fechaDesde: '2026-01-01',
      page: 2,
      pageSize: 10,
      sort: 'fechaEfecto:desc',
    })

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/suplementos', {
      params: {
        referencia: 'SUP',
        poliza: 'POL',
        tipo: 'Regularizacion',
        situacion: 'Validado',
        fechaEfectoDesde: '2026-01-01',
        page: 2,
        pageSize: 10,
        sort: 'fechaEfecto:desc',
      },
    })
  })
})
