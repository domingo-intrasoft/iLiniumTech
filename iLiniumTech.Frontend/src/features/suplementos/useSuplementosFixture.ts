import { computed, reactive } from 'vue'

import { suplementosFixture } from './fixtures'
import type { SuplementoListItem, SuplementosFilters } from './types'

function createEmptySuplementosFilters(): SuplementosFilters {
  return {
    texto: '',
    poliza: '',
    tipo: '',
    situacion: '',
    fechaDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: SuplementosFilters, source: SuplementosFilters) {
  target.texto = source.texto
  target.poliza = source.poliza
  target.tipo = source.tipo
  target.situacion = source.situacion
  target.fechaDesde = source.fechaDesde
}

function matchesSuplementosFilters(item: SuplementoListItem, filters: SuplementosFilters) {
  const texto = normalizeText(filters.texto)
  const poliza = normalizeText(filters.poliza)
  const matchesTexto =
    !texto ||
    normalizeText(item.referencia).includes(texto) ||
    normalizeText(item.concepto).includes(texto) ||
    normalizeText(item.resumen).includes(texto)
  const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
  const matchesTipo = !filters.tipo || item.tipo === filters.tipo
  const matchesSituacion = !filters.situacion || item.situacion === filters.situacion
  const matchesFecha = !filters.fechaDesde || item.fechaEfecto >= filters.fechaDesde

  return matchesTexto && matchesPoliza && matchesTipo && matchesSituacion && matchesFecha
}

export function formatSuplementosDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

export function useSuplementosFixture() {
  const filters = reactive<SuplementosFilters>(createEmptySuplementosFilters())
  const draftFilters = reactive<SuplementosFilters>(createEmptySuplementosFilters())
  const pagination = reactive({
    page: 1,
    pageSize: 2,
  })

  const filteredItems = computed(() => {
    return suplementosFixture.filter((item) => matchesSuplementosFilters(item, filters))
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
  const resultLabel = computed(() => (total.value === 1 ? 'suplemento' : 'suplementos'))
  const tableCaption = computed(
    () =>
      `Suplementos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin workflows ni API backend.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchSuplementos() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function clearFilters() {
    copyFilters(draftFilters, createEmptySuplementosFilters())
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
    canGoNext,
    canGoPrevious,
    changePage,
    changePageSize,
    clearFilters,
    draftFilters,
    filteredItems,
    filters,
    firstVisible,
    formatDate: formatSuplementosDate,
    lastVisible,
    pagedItems,
    pagination,
    resultLabel,
    searchSuplementos,
    tableCaption,
    total,
    totalPages,
  }
}
