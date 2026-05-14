import type { PolizaValueType } from './polizasConstants'

export const POLIZA_EMPTY_VALUE = 'No informado'

export function formatPolizaValue(value: unknown, type: PolizaValueType, currency = 'EUR') {
  if (value === null || value === undefined || value === '') {
    return POLIZA_EMPTY_VALUE
  }

  if (type === 'date' && typeof value === 'string' && isSentinelDate(value)) {
    return POLIZA_EMPTY_VALUE
  }

  if (type === 'money' && typeof value === 'number') {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency,
      maximumFractionDigits: 2,
    }).format(value)
  }

  if (type === 'date' && typeof value === 'string') {
    return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
  }

  return String(value)
}

function isSentinelDate(value: string) {
  return value === '0001-01-01' || value === '1900-01-01' || value === '19000101'
}
