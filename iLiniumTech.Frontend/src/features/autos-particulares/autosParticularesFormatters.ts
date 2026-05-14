export const AUTOS_EMPTY_VALUE = '-'

export type AutosValueType = 'string' | 'date' | 'money'

export function formatAutosParticularesValue(
  value: string | number | null | undefined,
  type: AutosValueType,
  currency = 'EUR',
) {
  if (value === null || value === undefined || value === '') {
    return AUTOS_EMPTY_VALUE
  }

  if (type === 'date') {
    const date = new Date(`${value}T00:00:00`)
    return Number.isNaN(date.getTime())
      ? AUTOS_EMPTY_VALUE
      : new Intl.DateTimeFormat('es-ES').format(date)
  }

  if (type === 'money') {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency,
      maximumFractionDigits: 2,
    }).format(Number(value))
  }

  return String(value)
}
