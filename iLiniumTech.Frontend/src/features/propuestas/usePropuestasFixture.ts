import { computed, reactive } from 'vue'

import { propuestasFixture } from './fixtures'
import type { PropuestasFilters } from './types'

export const propuestasPageSizeOptions = [10, 25, 50]

function createEmptyPropuestasFilters(): PropuestasFilters {
  return {
    referencia: '',
    estado: '',
    ramo: '',
    fechaDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: PropuestasFilters, source: PropuestasFilters) {
  target.referencia = source.referencia
  target.estado = source.estado
  target.ramo = source.ramo
  target.fechaDesde = source.fechaDesde
}

export function formatPropuestaDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

export function usePropuestasFixture() {
  const filters = reactive<PropuestasFilters>(createEmptyPropuestasFilters())
  const draftFilters = reactive<PropuestasFilters>(createEmptyPropuestasFilters())
  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  const filteredItems = computed(() => {
    const referencia = normalizeText(filters.referencia)

    return propuestasFixture.filter((item) => {
      const matchesReferencia =
        !referencia ||
        normalizeText(item.referencia).includes(referencia) ||
        normalizeText(item.solicitante).includes(referencia)
      const matchesEstado = !filters.estado || item.estado === filters.estado
      const matchesRamo = !filters.ramo || item.ramo === filters.ramo
      const matchesFecha = !filters.fechaDesde || item.fechaAlta >= filters.fechaDesde

      return matchesReferencia && matchesEstado && matchesRamo && matchesFecha
    })
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
  const resultLabel = computed(() => (total.value === 1 ? 'propuesta' : 'propuestas'))
  const tableCaption = computed(
    () =>
      `Propuestas fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin emision, conversion ni API backend.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchPropuestas() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function clearFilters() {
    copyFilters(draftFilters, createEmptyPropuestasFilters())
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function changePage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      pagination.page = page
    }
  }

  function changePageSize(pageSize: number) {
    pagination.pageSize = pageSize
    pagination.page = 1
  }

  return {
    canGoNext,
    canGoPrevious,
    changePage,
    changePageSize,
    clearFilters,
    draftFilters,
    firstVisible,
    lastVisible,
    pageSizeOptions: propuestasPageSizeOptions,
    pagedItems,
    pagination,
    resultLabel,
    searchPropuestas,
    tableCaption,
    total,
    totalPages,
  }
}
