import { apiClient } from '@/services/apiClient'
import { assertRuntimeConfigReady } from '@/services/runtimeConfig'

import { agendaFixture } from './fixtures'
import type { AgendaEstado, AgendaFilters, AgendaListItem, AgendaPrioridad } from './types'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  total: number
}

export interface AgendaSearchParams extends Partial<AgendaFilters> {
  page?: number
  pageSize?: number
  sort?: string
}

export interface AgendaCreatePayload {
  referencia: string
  titulo: string
  inicio: string
  fin?: string
  prioridad?: string
  objetoRelacionadoTipo?: string
}

export interface AgendaUpdatePayload {
  titulo?: string
  inicio?: string
  fin?: string
  prioridad?: string
  objetoRelacionadoTipo?: string
}

export interface AgendaCreateResult {
  id: string
}

interface BackendAgendaListItem {
  id: string
  referencia: string
  titulo: string
  inicio: string
  fin: string | null
  estado: string
  prioridad: string
  origen: string
  objetoRelacionadoTipo: string
}

function toDatePart(value: string | null | undefined) {
  if (!value) {
    return ''
  }

  return value.slice(0, 10)
}

function toTimePart(value: string | null | undefined) {
  if (!value) {
    return ''
  }

  const timePart = value.includes('T') ? value.split('T')[1] : value.split(' ')[1]
  return timePart ? timePart.slice(0, 5) : ''
}

function toAgendaEstado(value: string): AgendaEstado {
  return value === 'Cerrado' || value === 'Programado' ? value : 'Pendiente'
}

function toAgendaPrioridad(value: string): AgendaPrioridad {
  return value === 'Alta' || value === 'Baja' ? value : 'Media'
}

function mapBackendItem(item: BackendAgendaListItem): AgendaListItem {
  return {
    id: item.id,
    referencia: item.referencia,
    asunto: item.titulo,
    estado: toAgendaEstado(item.estado),
    prioridad: toAgendaPrioridad(item.prioridad),
    fechaInicio: toDatePart(item.inicio),
    horaInicio: toTimePart(item.inicio),
    fechaFin: toDatePart(item.fin),
    horaFin: toTimePart(item.fin),
    objetoRelacionado: item.objetoRelacionadoTipo,
    origen: item.origen,
  }
}

function searchFixture(params: AgendaSearchParams): PagedResult<AgendaListItem> {
  const texto = params.texto?.trim().toLocaleLowerCase('es-ES') ?? ''
  const filteredItems = agendaFixture.filter((item) => {
    const matchesText =
      !texto ||
      item.referencia.toLocaleLowerCase('es-ES').includes(texto) ||
      item.asunto.toLocaleLowerCase('es-ES').includes(texto) ||
      item.objetoRelacionado.toLocaleLowerCase('es-ES').includes(texto)
    const matchesEstado = !params.estado || item.estado === params.estado
    const matchesPrioridad = !params.prioridad || item.prioridad === params.prioridad
    const matchesFecha = !params.fechaDesde || item.fechaInicio >= params.fechaDesde
    return matchesText && matchesEstado && matchesPrioridad && matchesFecha
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

export async function searchAgenda(
  params: AgendaSearchParams,
): Promise<PagedResult<AgendaListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    assertRuntimeConfigReady()
    const response = await apiClient.get<PagedResult<BackendAgendaListItem>>('/api/agenda', {
      params: {
        texto: params.texto || undefined,
        estado: params.estado || undefined,
        prioridad: params.prioridad || undefined,
        fechaDesde: params.fechaDesde || undefined,
        page: params.page,
        pageSize: params.pageSize,
        sort: params.sort,
      },
    })

    return {
      ...response.data,
      items: response.data.items.map(mapBackendItem),
    }
  }

  return searchFixture(params)
}

function assertBackendWritesReady() {
  if (import.meta.env.VITE_USE_BACKEND !== 'true') {
    throw new Error('Las escrituras de agenda requieren backend BBDD habilitado.')
  }

  assertRuntimeConfigReady()
}

export async function createAgendaEvent(payload: AgendaCreatePayload): Promise<AgendaCreateResult> {
  assertBackendWritesReady()
  const response = await apiClient.post<AgendaCreateResult>('/api/agenda', payload)
  return response.data
}

export async function updateAgendaEvent(id: string, payload: AgendaUpdatePayload): Promise<void> {
  assertBackendWritesReady()
  await apiClient.put(`/api/agenda/${encodeURIComponent(id)}`, payload)
}

export async function deleteAgendaEvent(id: string): Promise<void> {
  assertBackendWritesReady()
  await apiClient.delete(`/api/agenda/${encodeURIComponent(id)}`)
}
