import { computed, reactive } from 'vue'

import { clientesFixture } from './fixtures'
import type { ClienteListItem, ClientesFilters } from './types'

function createEmptyClientesFilters(): ClientesFilters {
  return {
    texto: '',
    estado: '',
    segmento: '',
    fechaAltaDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: ClientesFilters, source: ClientesFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.segmento = source.segmento
  target.fechaAltaDesde = source.fechaAltaDesde
}

function matchesClientesFilters(item: ClienteListItem, filters: ClientesFilters) {
  const texto = normalizeText(filters.texto)
  const matchesTexto =
    !texto ||
    normalizeText(item.referencia).includes(texto) ||
    normalizeText(item.alias).includes(texto)
  const matchesEstado = !filters.estado || item.estado === filters.estado
  const matchesSegmento = !filters.segmento || item.segmento === filters.segmento
  const matchesFechaAlta = !filters.fechaAltaDesde || item.fechaAlta >= filters.fechaAltaDesde

  return matchesTexto && matchesEstado && matchesSegmento && matchesFechaAlta
}

export function formatClienteDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

export function useClientesFixture() {
  const filters = reactive<ClientesFilters>(createEmptyClientesFilters())
  const draftFilters = reactive<ClientesFilters>(createEmptyClientesFilters())
  const pagination = reactive({
    page: 1,
    pageSize: 2,
  })

  const filteredItems = computed(() => {
    return clientesFixture.filter((item) => matchesClientesFilters(item, filters))
  })

  const total = computed(() => filteredItems.value.length)
  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pagination.pageSize)))
  const pagedItems = computed(() => {
    const start = (pagination.page - 1) * pagination.pageSize
    return filteredItems.value.slice(start, start + pagination.pageSize)
  })
  const firstVisible = computed(() =>
    total.value === 0 ? 0 : (pagination.page - 1) * pagination.pageSize + 1,
  )
  const lastVisible = computed(() => Math.min(pagination.page * pagination.pageSize, total.value))
  const resultLabel = computed(() => (total.value === 1 ? 'cliente demo' : 'clientes demo'))
  const tableCaption = computed(
    () =>
      `Clientes fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin PII real ni API backend.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchClientes() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function clearFilters() {
    copyFilters(draftFilters, createEmptyClientesFilters())
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function changePage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      pagination.page = page
    }
  }

  function changePageSize(event: Event) {
    pagination.pageSize = Number((event.target as HTMLSelectElement).value)
    pagination.page = 1
  }

  return {
    filters,
    draftFilters,
    pagination,
    filteredItems,
    total,
    totalPages,
    pagedItems,
    firstVisible,
    lastVisible,
    resultLabel,
    tableCaption,
    canGoPrevious,
    canGoNext,
    searchClientes,
    clearFilters,
    changePage,
    changePageSize,
    formatDate: formatClienteDate,
  }
}
