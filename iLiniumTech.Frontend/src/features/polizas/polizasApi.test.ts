import { describe, expect, it } from 'vitest'

import { getPolizasMetadata, searchPolizas } from './polizasApi'

describe('polizas feature contract', () => {
  it('exposes the AppBuilder component reference', async () => {
    const metadata = await getPolizasMetadata()

    expect(metadata.appBuilder.rootComponentId).toBe(2824)
    expect(metadata.appBuilder.crudComponentId).toBe(2825)
    expect(metadata.appBuilder.dataSourceId).toBe(146)
  })

  it('filters fixture rows by policy number', async () => {
    const result = await searchPolizas({ numero: '0002' })

    expect(result.total).toBe(1)
    expect(result.items[0]?.numero).toBe('POL-2026-0002')
  })

  it('keeps visible fields inside the MVP whitelist', async () => {
    const metadata = await getPolizasMetadata()
    const visibleNames = metadata.fields.filter((field) => field.visible).map((field) => field.name)

    expect(visibleNames).toContain('numero')
    expect(visibleNames).toContain('clienteNombre')
    expect(visibleNames).not.toContain('connectionString')
  })
})
