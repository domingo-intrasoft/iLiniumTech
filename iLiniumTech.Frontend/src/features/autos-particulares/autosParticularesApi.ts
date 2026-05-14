import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import {
  autosParticularesCatalogsFixture,
  autosParticularesFixture,
} from './autosParticularesFixture'
import type {
  AutoParticularPolizaListItem,
  AutosParticularesCatalogs,
  PagedResult,
} from './autosParticularesTypes'

export interface AutosParticularesSearchParams {
  numero?: string
  cliente?: string
  estado?: string
  compania?: string
  fechaEfectoDesde?: string
  fechaEfectoHasta?: string
  page?: number
  pageSize?: number
  sort?: string
}

function includesText(value: string, query: string) {
  return value.toLowerCase().includes(query)
}

export async function searchAutosParticulares(
  params: AutosParticularesSearchParams,
): Promise<PagedResult<AutoParticularPolizaListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<AutoParticularPolizaListItem>>(
      '/api/autos-particulares/polizas',
      { params },
    )
    return response.data
  }

  const numero = params.numero?.toLowerCase() ?? ''
  const cliente = params.cliente?.toLowerCase() ?? ''
  const estado = params.estado?.toLowerCase() ?? ''
  const compania = params.compania?.toLowerCase() ?? ''
  const fechaEfectoDesde = params.fechaEfectoDesde ?? ''
  const fechaEfectoHasta = params.fechaEfectoHasta ?? ''

  const filteredItems = autosParticularesFixture.items.filter((item) => {
    const matchesNumero = !numero || includesText(item.numero, numero)
    const matchesCliente = !cliente || includesText(item.clienteNombre, cliente)
    const matchesEstado = !estado || includesText(item.estado, estado)
    const matchesCompania = !compania || includesText(item.compania, compania)
    const matchesFechaDesde = !fechaEfectoDesde || item.fechaEfecto >= fechaEfectoDesde
    const matchesFechaHasta = !fechaEfectoHasta || item.fechaEfecto <= fechaEfectoHasta

    return (
      matchesNumero &&
      matchesCliente &&
      matchesEstado &&
      matchesCompania &&
      matchesFechaDesde &&
      matchesFechaHasta
    )
  })

  const page = Math.max(1, params.page ?? autosParticularesFixture.page)
  const pageSize = Math.max(1, params.pageSize ?? autosParticularesFixture.pageSize)
  const start = (page - 1) * pageSize

  return {
    ...autosParticularesFixture,
    page,
    pageSize,
    total: filteredItems.length,
    items: filteredItems.slice(start, start + pageSize),
  }
}

export async function getAutosParticularesCatalogs(): Promise<AutosParticularesCatalogs> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<AutosParticularesCatalogs>(
      '/api/autos-particulares/catalogs',
    )
    return response.data
  }

  return autosParticularesCatalogsFixture
}
