import { describe, expect, it } from 'vitest'

import { getPolizaById, getPolizasCatalogs, searchPolizas } from './polizasApi'
import { polizasTableColumns } from './polizasConstants'

describe('polizas static frontend contract', () => {
  it('keeps table columns declared locally', () => {
    const columnNames = polizasTableColumns.map((column) => column.key)

    expect(columnNames).toEqual([
      'compania',
      'numero',
      'aplicacion',
      'estado',
      'ramo',
      'documento',
      'riesgo',
      'fechaEfecto',
      'fechaVencimiento',
      'primaAnual',
      'clienteNombre',
    ])
    expect(columnNames).not.toContain('connectionString')
  })

  it('filters fixture rows by policy number', async () => {
    const result = await searchPolizas({ numero: '0002' })

    expect(result.total).toBe(1)
    expect(result.items[0]?.numero).toBe('POL-2026-0002')
  })

  it('filters fixture rows by local catalog-backed values', async () => {
    const result = await searchPolizas({ estado: 'Vigor', ramo: 'Autos' })

    expect(result.total).toBe(1)
    expect(result.items[0]?.estado).toBe('Vigor')
    expect(result.items[0]?.ramo).toBe('Autos')
  })

  it('filters fixture rows by fecha efecto range', async () => {
    const result = await searchPolizas({
      fechaEfectoDesde: '2026-02-01',
      fechaEfectoHasta: '2026-02-28',
    })

    expect(result.total).toBe(1)
    expect(result.items[0]?.fechaEfecto).toBe('2026-02-15')
  })

  it('paginates fixture rows with the backend page contract', async () => {
    const result = await searchPolizas({ page: 2, pageSize: 1 })

    expect(result.page).toBe(2)
    expect(result.pageSize).toBe(1)
    expect(result.total).toBe(2)
    expect(result.items).toHaveLength(1)
    expect(result.items[0]?.numero).toBe('POL-2026-0002')
  })

  it('exposes local catalogs without metadata', async () => {
    const catalogs = await getPolizasCatalogs()

    expect(catalogs.tipoPoliza.map((option) => option.value)).toContain('Vigor')
    expect(catalogs.ramo.map((option) => option.value)).toContain('Autos')
    expect('appBuilder' in catalogs).toBe(false)
  })

  it('loads a policy detail from the local fixture', async () => {
    const detail = await getPolizaById('POL-1001')

    expect(detail?.numero).toBe('POL-2026-0001')
    expect(detail?.clienteNombre).toBe('Cliente anonimo 1')
  })
})
