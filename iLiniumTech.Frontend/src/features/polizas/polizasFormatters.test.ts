import { describe, expect, it } from 'vitest'

import { POLIZA_EMPTY_VALUE, formatPolizaValue } from './polizasFormatters'

describe('polizas formatters', () => {
  it('formats missing values honestly instead of hiding them behind a dash', () => {
    expect(formatPolizaValue('', 'string')).toBe(POLIZA_EMPTY_VALUE)
    expect(formatPolizaValue(null, 'date')).toBe(POLIZA_EMPTY_VALUE)
    expect(formatPolizaValue('0001-01-01', 'date')).toBe(POLIZA_EMPTY_VALUE)
    expect(formatPolizaValue('1900-01-01', 'date')).toBe(POLIZA_EMPTY_VALUE)
  })

  it('formats dates and money with the Spanish locale', () => {
    expect(formatPolizaValue('2026-02-15', 'date')).toBe('15/2/2026')
    expect(formatPolizaValue(315.75, 'money', 'EUR')).toBe('315,75\u00a0\u20ac')
  })
})
