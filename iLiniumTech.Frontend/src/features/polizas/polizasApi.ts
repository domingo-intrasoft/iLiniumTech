import { apiClient } from '@/services/apiClient'

import { polizasCatalogsFixture, polizasDetailFixture, polizasFixture } from './polizasFixture'
import type { PagedResult, PolizaDetail, PolizaListItem, PolizasCatalogs } from './polizasTypes'

export interface PolizasSearchParams {
  numero?: string
  cliente?: string
  estado?: string
  compania?: string
  ramo?: string
  page?: number
  pageSize?: number
  sort?: string
}

export async function searchPolizas(
  params: PolizasSearchParams,
): Promise<PagedResult<PolizaListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PagedResult<PolizaListItem>>('/api/polizas', { params })
    return response.data
  }

  const numero = params.numero?.toLowerCase() ?? ''
  const cliente = params.cliente?.toLowerCase() ?? ''
  const estado = params.estado?.toLowerCase() ?? ''
  const compania = params.compania?.toLowerCase() ?? ''
  const ramo = params.ramo?.toLowerCase() ?? ''
  const items = polizasFixture.items.filter((item) => {
    const matchesNumero = !numero || item.numero.toLowerCase().includes(numero)
    const matchesCliente = !cliente || item.clienteNombre.toLowerCase().includes(cliente)
    const matchesEstado = !estado || item.estado.toLowerCase().includes(estado)
    const matchesCompania = !compania || item.compania.toLowerCase().includes(compania)
    const matchesRamo = !ramo || item.ramo.toLowerCase().includes(ramo)
    return matchesNumero && matchesCliente && matchesEstado && matchesCompania && matchesRamo
  })

  return {
    ...polizasFixture,
    total: items.length,
    items,
  }
}

export async function getPolizasCatalogs(): Promise<PolizasCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PolizasCatalogs>('/api/polizas/catalogs')
    return response.data
  }

  return polizasCatalogsFixture
}

export async function getPolizaById(id: string): Promise<PolizaDetail | null> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PolizaDetail>(`/api/polizas/${encodeURIComponent(id)}`)
    return response.data
  }

  return polizasDetailFixture.find((item) => item.id === id) ?? null
}
