import { apiClient } from '@/services/apiClient'

import { polizasFixture, polizasMetadata } from './polizasFixture'
import type { PagedResult, PolizaListItem, PolizasComponentMetadata } from './polizasTypes'

export interface PolizasSearchParams {
  numero?: string
  cliente?: string
  estado?: string
  page?: number
  pageSize?: number
  sort?: string
}

export async function getPolizasMetadata(): Promise<PolizasComponentMetadata> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PolizasComponentMetadata>('/api/polizas/metadata')
    return response.data
  }

  return polizasMetadata
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
  const items = polizasFixture.items.filter((item) => {
    const matchesNumero = !numero || item.numero.toLowerCase().includes(numero)
    const matchesCliente = !cliente || item.clienteNombre.toLowerCase().includes(cliente)
    const matchesEstado = !estado || item.estado.toLowerCase().includes(estado)
    return matchesNumero && matchesCliente && matchesEstado
  })

  return {
    ...polizasFixture,
    total: items.length,
    items,
  }
}
