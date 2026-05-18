import { computed, reactive } from 'vue'

import { siniestrosFixture } from './siniestrosFixture'
import type { SiniestrosFilters } from './siniestrosTypes'

export const siniestrosPageSizeOptions = [10, 25, 50]

function createEmptySiniestrosFilters(): SiniestrosFilters {
  return {
    referencia: '',
    poliza: '',
    estado: '',
    prioridad: '',
    fechaDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: SiniestrosFilters, source: SiniestrosFilters) {
  target.referencia = source.referencia
  target.poliza = source.poliza
  target.estado = source.estado
  target.prioridad = source.prioridad
  target.fechaDesde = source.fechaDesde
}

export function useSiniestrosFixture() {
  const filters = reactive<SiniestrosFilters>(createEmptySiniestrosFilters())
  const draftFilters = reactive<SiniestrosFilters>(createEmptySiniestrosFilters())
  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  const filteredItems = computed(() => {
    const referencia = normalizeText(filters.referencia)
    const poliza = normalizeText(filters.poliza)

    return siniestrosFixture.filter((item) => {
      const matchesReferencia =
        !referencia ||
        normalizeText(item.referencia).includes(referencia) ||
        normalizeText(item.cliente).includes(referencia)
      const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
      const matchesEstado = !filters.estado || item.estado === filters.estado
      const matchesPrioridad = !filters.prioridad || item.prioridad === filters.prioridad
      const matchesFecha = !filters.fechaDesde || item.fechaSiniestro >= filters.fechaDesde

      return matchesReferencia && matchesPoliza && matchesEstado && matchesPrioridad && matchesFecha
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
  const resultLabel = computed(() => (total.value === 1 ? 'siniestro' : 'siniestros'))
  const tableCaption = computed(
    () =>
      `Siniestros fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin API backend ni datos reales.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchSiniestros() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function clearFilters() {
    copyFilters(draftFilters, createEmptySiniestrosFilters())
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
    filteredItems,
    firstVisible,
    lastVisible,
    pageSizeOptions: siniestrosPageSizeOptions,
    pagedItems,
    pagination,
    resultLabel,
    searchSiniestros,
    tableCaption,
    total,
    totalPages,
  }
}
