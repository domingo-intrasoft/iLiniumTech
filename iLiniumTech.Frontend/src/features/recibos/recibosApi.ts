import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { reciboCanales, reciboSituaciones, reciboTipos, recibosFixture } from './fixtures'
import type { ReciboListItem, RecibosFilters } from './types'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface RecibosCatalogs {
  situaciones: string[]
  tipos: string[]
  canales: string[]
}

export interface RecibosSearchParams extends Partial<RecibosFilters> {
  page?: number
  pageSize?: number
  sort?: string
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function searchFixture(params: RecibosSearchParams): PagedResult<ReciboListItem> {
  const recibo = normalizeText(params.recibo ?? '')
  const poliza = normalizeText(params.poliza ?? '')
  const filteredItems = recibosFixture.filter((item) => {
    const matchesRecibo =
      !recibo ||
      normalizeText(item.recibo).includes(recibo) ||
      normalizeText(item.cliente).includes(recibo)
    const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
    const matchesSituacion = !params.situacion || item.situacion === params.situacion
    const matchesTipo = !params.tipo || item.tipo === params.tipo
    const matchesVencimiento =
      !params.vencimientoDesde || item.fechaVencimiento >= params.vencimientoDesde

    return matchesRecibo && matchesPoliza && matchesSituacion && matchesTipo && matchesVencimiento
  })
  const page = Math.max(1, params.page ?? 1)
  const pageSize = Math.max(1, params.pageSize ?? 25)
  const start = (page - 1) * pageSize

  return {
    items: filteredItems.slice(start, start + pageSize),
    page,
    pageSize,
    total: filteredItems.length,
  }
}

export async function getRecibosCatalogs(): Promise<RecibosCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<RecibosCatalogs>('/api/recibos/catalogs')
    return response.data
  }

  return {
    situaciones: [...reciboSituaciones],
    tipos: [...reciboTipos],
    canales: [...reciboCanales],
  }
}

export async function searchRecibos(
  params: RecibosSearchParams,
): Promise<PagedResult<ReciboListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<ReciboListItem>>('/api/recibos', {
      params: {
        recibo: params.recibo || undefined,
        poliza: params.poliza || undefined,
        situacion: params.situacion || undefined,
        tipo: params.tipo || undefined,
        fechaVencimientoDesde: params.vencimientoDesde || undefined,
        page: params.page,
        pageSize: params.pageSize,
        sort: params.sort,
      },
    })

    return response.data
  }

  return searchFixture(params)
}
