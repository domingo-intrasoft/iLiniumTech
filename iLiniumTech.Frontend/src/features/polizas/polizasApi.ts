import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { polizasCatalogsFixture, polizasDetailFixture, polizasFixture } from './polizasFixture'
import type {
  PagedResult,
  PolizaCreatePayload,
  PolizaCreateResult,
  PolizaDetail,
  PolizaListItem,
  PolizasCatalogs,
  PolizaUpdatePayload,
} from './polizasTypes'

export interface PolizasSearchParams {
  numero?: string
  cliente?: string
  estado?: string
  compania?: string
  ramo?: string
  fechaEfectoDesde?: string
  fechaEfectoHasta?: string
  page?: number
  pageSize?: number
  sort?: string
}

export async function searchPolizas(
  params: PolizasSearchParams,
): Promise<PagedResult<PolizaListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<PolizaListItem>>('/api/polizas', { params })
    return response.data
  }

  const numero = params.numero?.toLowerCase() ?? ''
  const cliente = params.cliente?.toLowerCase() ?? ''
  const estado = params.estado?.toLowerCase() ?? ''
  const compania = params.compania?.toLowerCase() ?? ''
  const ramo = params.ramo?.toLowerCase() ?? ''
  const fechaEfectoDesde = params.fechaEfectoDesde ?? ''
  const fechaEfectoHasta = params.fechaEfectoHasta ?? ''
  const filteredItems = polizasFixture.items.filter((item) => {
    const matchesNumero = !numero || item.numero.toLowerCase().includes(numero)
    const matchesCliente = !cliente || item.clienteNombre.toLowerCase().includes(cliente)
    const matchesEstado = !estado || item.estado.toLowerCase().includes(estado)
    const matchesCompania = !compania || item.compania.toLowerCase().includes(compania)
    const matchesRamo = !ramo || item.ramo.toLowerCase().includes(ramo)
    const matchesFechaDesde = !fechaEfectoDesde || item.fechaEfecto >= fechaEfectoDesde
    const matchesFechaHasta = !fechaEfectoHasta || item.fechaEfecto <= fechaEfectoHasta
    return (
      matchesNumero &&
      matchesCliente &&
      matchesEstado &&
      matchesCompania &&
      matchesRamo &&
      matchesFechaDesde &&
      matchesFechaHasta
    )
  })
  const page = Math.max(1, params.page ?? polizasFixture.page)
  const pageSize = Math.max(1, params.pageSize ?? polizasFixture.pageSize)
  const start = (page - 1) * pageSize
  const items = filteredItems.slice(start, start + pageSize)

  return {
    ...polizasFixture,
    page,
    pageSize,
    total: filteredItems.length,
    items,
  }
}

export async function getPolizasCatalogs(): Promise<PolizasCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PolizasCatalogs>('/api/polizas/catalogs')
    return response.data
  }

  return polizasCatalogsFixture
}

export async function getPolizaById(id: string): Promise<PolizaDetail | null> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PolizaDetail>(`/api/polizas/${encodeURIComponent(id)}`)
    return response.data
  }

  return polizasDetailFixture.find((item) => item.id === id) ?? null
}

function assertBackendWritesReady() {
  if (import.meta.env.VITE_USE_BACKEND !== 'true') {
    throw new Error('Las escrituras de polizas requieren backend BBDD habilitado.')
  }

  assertRuntimeConfigReady()
}

export async function createPoliza(payload: PolizaCreatePayload): Promise<PolizaCreateResult> {
  assertBackendWritesReady()
  const response = await apiClient.post<PolizaCreateResult>('/api/polizas', payload)
  return response.data
}

export async function updatePoliza(id: string, payload: PolizaUpdatePayload): Promise<void> {
  assertBackendWritesReady()
  await apiClient.put(`/api/polizas/${encodeURIComponent(id)}`, payload)
}

export async function deletePoliza(id: string): Promise<void> {
  assertBackendWritesReady()
  await apiClient.delete(`/api/polizas/${encodeURIComponent(id)}`)
}
