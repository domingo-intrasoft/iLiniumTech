import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { clienteEstadoOptions, clienteSegmentoOptions, clientesFixture } from './fixtures'
import type {
  ClienteCreatePayload,
  ClienteCreateResult,
  ClienteListItem,
  ClienteUpdatePayload,
  ClientesFilters,
} from './types'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface ClientesCatalogs {
  estados: string[]
  segmentos: string[]
}

export interface ClientesSearchParams extends Partial<ClientesFilters> {
  page?: number
  pageSize?: number
  sort?: string
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function searchFixture(params: ClientesSearchParams): PagedResult<ClienteListItem> {
  const texto = normalizeText(params.texto ?? '')
  const filteredItems = clientesFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.alias).includes(texto)
    const matchesEstado = !params.estado || item.estado === params.estado
    const matchesSegmento = !params.segmento || item.segmento === params.segmento
    const matchesFechaAlta = !params.fechaAltaDesde || item.fechaAlta >= params.fechaAltaDesde

    return matchesTexto && matchesEstado && matchesSegmento && matchesFechaAlta
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

export async function getClientesCatalogs(): Promise<ClientesCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<ClientesCatalogs>('/api/clientes/catalogs')
    return response.data
  }

  return {
    estados: clienteEstadoOptions,
    segmentos: clienteSegmentoOptions,
  }
}

export async function searchClientes(
  params: ClientesSearchParams,
): Promise<PagedResult<ClienteListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<ClienteListItem>>('/api/clientes', {
      params: {
        texto: params.texto || undefined,
        estado: params.estado || undefined,
        segmento: params.segmento || undefined,
        fechaAltaDesde: params.fechaAltaDesde || undefined,
        page: params.page,
        pageSize: params.pageSize,
        sort: params.sort,
      },
    })

    return response.data
  }

  return searchFixture(params)
}

function assertBackendWritesReady() {
  if (import.meta.env.VITE_USE_BACKEND !== 'true') {
    throw new Error('Las escrituras de clientes requieren backend BBDD habilitado.')
  }

  assertRuntimeConfigReady()
}

export async function createCliente(payload: ClienteCreatePayload): Promise<ClienteCreateResult> {
  assertBackendWritesReady()
  const response = await apiClient.post<ClienteCreateResult>('/api/clientes', payload)
  return response.data
}

export async function updateCliente(id: string, payload: ClienteUpdatePayload): Promise<void> {
  assertBackendWritesReady()
  await apiClient.put(`/api/clientes/${encodeURIComponent(id)}`, payload)
}

export async function deleteCliente(id: string): Promise<void> {
  assertBackendWritesReady()
  await apiClient.delete(`/api/clientes/${encodeURIComponent(id)}`)
}
