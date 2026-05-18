import { computed, reactive } from 'vue'

import { agendaFixture } from './fixtures'
import type { AgendaFilters } from './types'

function createEmptyFilters(): AgendaFilters {
  return {
    texto: '',
    estado: '',
    prioridad: '',
    fechaDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: AgendaFilters, source: AgendaFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.prioridad = source.prioridad
  target.fechaDesde = source.fechaDesde
}

export function formatAgendaDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

export function useAgendaFixture() {
  const filters = reactive<AgendaFilters>(createEmptyFilters())
  const draftFilters = reactive<AgendaFilters>(createEmptyFilters())

  const pagination = reactive({
    page: 1,
    pageSize: 2,
  })

  const filteredItems = computed(() => {
    const texto = normalizeText(filters.texto)

    return agendaFixture.filter((item) => {
      const matchesText =
        !texto ||
        normalizeText(item.referencia).includes(texto) ||
        normalizeText(item.asunto).includes(texto) ||
        normalizeText(item.objetoRelacionado).includes(texto)
      const matchesEstado = !filters.estado || item.estado === filters.estado
      const matchesPrioridad = !filters.prioridad || item.prioridad === filters.prioridad
      const matchesFecha = !filters.fechaDesde || item.fechaInicio >= filters.fechaDesde

      return matchesText && matchesEstado && matchesPrioridad && matchesFecha
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
  const resultLabel = computed(() => (total.value === 1 ? 'evento' : 'eventos'))
  const tableCaption = computed(
    () =>
      `Agenda fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin calendario dinamico ni API backend.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchAgenda() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
  }

  function clearFilters() {
    copyFilters(draftFilters, createEmptyFilters())
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
    firstVisible,
    formatDate: formatAgendaDate,
    lastVisible,
    pagedItems,
    pagination,
    resultLabel,
    searchAgenda,
    tableCaption,
    total,
    totalPages,
  }
}
