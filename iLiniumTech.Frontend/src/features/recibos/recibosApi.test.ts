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

import { getRecibosCatalogs, searchRecibos } from './recibosApi'

describe('recibos API adapter', () => {
  beforeEach(() => {
    vi.unstubAllEnvs()
    mocks.get.mockReset()
    mocks.assertRuntimeConfigReady.mockReset()
  })

  it('uses the sanitized fixture only when backend mode is disabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchRecibos({ recibo: '0002', page: 1, pageSize: 25 })

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(result.total).toBe(1)
    expect(result.items[0]?.recibo).toBe('REC-2026-0002')
    expect(JSON.stringify(result)).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
    expect(JSON.stringify(result)).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
  })

  it('exposes local catalogs without calling the backend in fixture mode', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const catalogs = await getRecibosCatalogs()

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(catalogs.situaciones).toEqual(['Pendiente', 'Cobrado', 'Anulado'])
    expect(catalogs.tipos).toEqual(['Prima', 'Extorno', 'Regularizacion'])
    expect(catalogs.canales).toEqual(['Canal demo', 'Canal demo mediador', 'Canal demo compania'])
  })

  it('calls the Recibos catalogs endpoint when backend mode is enabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        situaciones: ['Pendiente'],
        tipos: ['Prima'],
        canales: ['No operativo'],
      },
    })

    await getRecibosCatalogs()

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/recibos/catalogs')
  })

  it('calls the Recibos search endpoint with minimized read-only params', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        items: [],
        page: 2,
        pageSize: 10,
        total: 0,
      },
    })

    await searchRecibos({
      recibo: 'REC',
      poliza: 'POL',
      situacion: 'Pendiente',
      tipo: 'Prima',
      vencimientoDesde: '2026-01-01',
      page: 2,
      pageSize: 10,
      sort: 'fechaVencimiento:desc',
    })

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/recibos', {
      params: {
        recibo: 'REC',
        poliza: 'POL',
        situacion: 'Pendiente',
        tipo: 'Prima',
        fechaVencimientoDesde: '2026-01-01',
        page: 2,
        pageSize: 10,
        sort: 'fechaVencimiento:desc',
      },
    })
  })
})
