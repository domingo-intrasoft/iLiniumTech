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

import { getAutosParticularesCatalogs, searchAutosParticulares } from './autosParticularesApi'
import { autosTableColumns } from './autosParticularesConstants'

describe('autos particulares static frontend contract', () => {
  beforeEach(() => {
    vi.unstubAllEnvs()
    mocks.get.mockReset()
    mocks.assertRuntimeConfigReady.mockReset()
  })

  it('keeps table columns declared locally for the Autos MVP', () => {
    expect(autosTableColumns.map((column) => column.key)).toEqual([
      'compania',
      'numero',
      'estado',
      'clienteNombre',
      'vehiculoResumen',
      'fechaEfecto',
      'fechaVencimiento',
      'primaAnual',
    ])
  })

  it('filters local fixture rows by supported backend-aligned criteria', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchAutosParticulares({ numero: '0002', compania: 'Compania demo' })

    expect(result.total).toBe(1)
    expect(result.items[0]?.numero).toBe('AUTO-2026-0002')
    expect(result.items[0]?.vehiculoResumen).toBe('SUV familiar')
    expect(JSON.stringify(result)).not.toMatch(/\b\d{4}\s?[A-Z]{3}\b/)
  })

  it('filters local fixture rows by Autos-specific catalog values', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchAutosParticulares({
      estado: 'Vigor',
      compania: 'Aseguradora ejemplo',
    })

    expect(result.total).toBe(1)
    expect(result.items[0]?.numero).toBe('AUTO-2026-0003')
  })

  it('filters local fixture rows by fecha efecto range', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchAutosParticulares({
      fechaEfectoDesde: '2026-02-01',
      fechaEfectoHasta: '2026-02-28',
    })

    expect(result.total).toBe(1)
    expect(result.items[0]?.fechaEfecto).toBe('2026-02-01')
  })

  it('paginates local fixture rows with the backend page contract', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchAutosParticulares({ page: 2, pageSize: 2 })

    expect(result.page).toBe(2)
    expect(result.pageSize).toBe(2)
    expect(result.total).toBe(4)
    expect(result.items).toHaveLength(2)
    expect(result.items[0]?.numero).toBe('AUTO-2026-0003')
  })

  it('calls the probable backend endpoint when backend mode is enabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        items: [],
        page: 1,
        pageSize: 25,
        total: 0,
        scope: {
          ramo: 'Autos',
          divisionObjetivo: 'Particulares',
          divisionFiltroAplicado: false,
          divisionPendienteUat: true,
        },
      },
    })

    await searchAutosParticulares({ numero: '0001', page: 1, pageSize: 25 })

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/autos-particulares/polizas', {
      params: { numero: '0001', page: 1, pageSize: 25 },
    })
  })

  it('exposes local filter catalogs without unsafe runtime fields', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const catalogs = await getAutosParticularesCatalogs()

    expect(catalogs.estado.map((option) => option.value)).toContain('Vigor')
    expect(catalogs.scope?.divisionPendienteUat).toBe(true)
    expect('appBuilder' in catalogs).toBe(false)
    expect('connectionString' in catalogs).toBe(false)
  })

  it('calls the Autos-specific catalogs endpoint in backend mode', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        estado: [],
        compania: [],
        scope: {
          ramo: 'Autos',
          divisionObjetivo: 'Particulares',
          divisionFiltroAplicado: false,
          divisionPendienteUat: true,
        },
      },
    })

    await getAutosParticularesCatalogs()

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/autos-particulares/catalogs')
  })
})
