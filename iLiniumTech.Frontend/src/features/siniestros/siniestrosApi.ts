import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { siniestrosFixture } from './siniestrosFixture'
import {
  siniestroEstados,
  siniestroPrioridades,
  type SiniestroListItem,
  type SiniestrosFilters,
} from './siniestrosTypes'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface SiniestrosCatalogs {
  estados: string[]
  prioridades: string[]
}

export interface SiniestrosSearchParams extends Partial<SiniestrosFilters> {
  page?: number
  pageSize?: number
  sort?: string
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function searchFixture(params: SiniestrosSearchParams): PagedResult<SiniestroListItem> {
  const referencia = normalizeText(params.referencia ?? '')
  const poliza = normalizeText(params.poliza ?? '')
  const filteredItems = siniestrosFixture.filter((item) => {
    const matchesReferencia =
      !referencia ||
      normalizeText(item.referencia).includes(referencia) ||
      normalizeText(item.cliente).includes(referencia)
    const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
    const matchesEstado = !params.estado || item.estado === params.estado
    const matchesPrioridad = !params.prioridad || item.prioridad === params.prioridad
    const matchesFecha = !params.fechaDesde || item.fechaSiniestro >= params.fechaDesde

    return matchesReferencia && matchesPoliza && matchesEstado && matchesPrioridad && matchesFecha
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

export async function getSiniestrosCatalogs(): Promise<SiniestrosCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<SiniestrosCatalogs>('/api/siniestros/catalogs')
    return response.data
  }

  return {
    estados: [...siniestroEstados],
    prioridades: [...siniestroPrioridades],
  }
}

export async function searchSiniestros(
  params: SiniestrosSearchParams,
): Promise<PagedResult<SiniestroListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<SiniestroListItem>>('/api/siniestros', {
      params: {
        referencia: params.referencia || undefined,
        poliza: params.poliza || undefined,
        estado: params.estado || undefined,
        prioridad: params.prioridad || undefined,
        fechaSiniestroDesde: params.fechaDesde || undefined,
        page: params.page,
        pageSize: params.pageSize,
        sort: params.sort,
      },
    })

    return response.data
  }

  return searchFixture(params)
}
