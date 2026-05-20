import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { suplementoSituaciones, suplementoTipos, suplementosFixture } from './fixtures'
import type { SuplementoListItem, SuplementosFilters } from './types'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface SuplementosCatalogs {
  tipos: string[]
  situaciones: string[]
}

export interface SuplementosSearchParams extends Partial<SuplementosFilters> {
  page?: number
  pageSize?: number
  sort?: string
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function searchFixture(params: SuplementosSearchParams): PagedResult<SuplementoListItem> {
  const texto = normalizeText(params.texto ?? '')
  const poliza = normalizeText(params.poliza ?? '')
  const filteredItems = suplementosFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.concepto).includes(texto) ||
      normalizeText(item.resumen).includes(texto)
    const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
    const matchesTipo = !params.tipo || item.tipo === params.tipo
    const matchesSituacion = !params.situacion || item.situacion === params.situacion
    const matchesFecha = !params.fechaDesde || item.fechaEfecto >= params.fechaDesde

    return matchesTexto && matchesPoliza && matchesTipo && matchesSituacion && matchesFecha
  })
  const page = Math.max(1, params.page ?? 1)
  const pageSize = Math.max(1, params.pageSize ?? 2)
  const start = (page - 1) * pageSize

  return {
    items: filteredItems.slice(start, start + pageSize),
    page,
    pageSize,
    total: filteredItems.length,
  }
}

export async function getSuplementosCatalogs(): Promise<SuplementosCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<SuplementosCatalogs>('/api/suplementos/catalogs')
    return response.data
  }

  return {
    tipos: [...suplementoTipos],
    situaciones: [...suplementoSituaciones],
  }
}

export async function searchSuplementos(
  params: SuplementosSearchParams,
): Promise<PagedResult<SuplementoListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<SuplementoListItem>>('/api/suplementos', {
      params: {
        referencia: params.texto || undefined,
        poliza: params.poliza || undefined,
        tipo: params.tipo || undefined,
        situacion: params.situacion || undefined,
        fechaEfectoDesde: params.fechaDesde || undefined,
        page: params.page,
        pageSize: params.pageSize,
        sort: params.sort,
      },
    })

    return response.data
  }

  return searchFixture(params)
}
