import { computed, reactive } from 'vue'

import { recibosFixture } from './fixtures'
import type { RecibosFilters } from './types'

function createEmptyFilters(): RecibosFilters {
  return {
    recibo: '',
    poliza: '',
    situacion: '',
    tipo: '',
    vencimientoDesde: '',
  }
}

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: RecibosFilters, source: RecibosFilters) {
  target.recibo = source.recibo
  target.poliza = source.poliza
  target.situacion = source.situacion
  target.tipo = source.tipo
  target.vencimientoDesde = source.vencimientoDesde
}

export function formatRecibosDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

export function useRecibosFixture() {
  const filters = reactive<RecibosFilters>(createEmptyFilters())
  const draftFilters = reactive<RecibosFilters>(createEmptyFilters())

  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  const filteredItems = computed(() => {
    const recibo = normalizeText(filters.recibo)
    const poliza = normalizeText(filters.poliza)

    return recibosFixture.filter((item) => {
      const matchesRecibo =
        !recibo ||
        normalizeText(item.recibo).includes(recibo) ||
        normalizeText(item.cliente).includes(recibo)
      const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
      const matchesSituacion = !filters.situacion || item.situacion === filters.situacion
      const matchesTipo = !filters.tipo || item.tipo === filters.tipo
      const matchesVencimiento =
        !filters.vencimientoDesde || item.vencimiento >= filters.vencimientoDesde

      return matchesRecibo && matchesPoliza && matchesSituacion && matchesTipo && matchesVencimiento
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
  const resultLabel = computed(() => (total.value === 1 ? 'recibo' : 'recibos'))
  const tableCaption = computed(
    () =>
      `Recibos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados con importes demo anonimizados y sin API backend.`,
  )
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  function searchRecibos() {
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
    filters,
    firstVisible,
    formatDate: formatRecibosDate,
    lastVisible,
    pagedItems,
    pagination,
    resultLabel,
    searchRecibos,
    tableCaption,
    total,
    totalPages,
  }
}
