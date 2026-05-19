import { computed, onMounted, reactive, ref } from 'vue'

import { getRecibosCatalogs, searchRecibos as searchRecibosApi } from './recibosApi'
import type { ReciboListItem, RecibosFilters } from './types'

export const recibosPageSizeOptions = [10, 25, 50]

function createEmptyRecibosFilters(): RecibosFilters {
  return {
    recibo: '',
    poliza: '',
    situacion: '',
    tipo: '',
    vencimientoDesde: '',
  }
}

function copyFilters(target: RecibosFilters, source: RecibosFilters) {
  target.recibo = source.recibo
  target.poliza = source.poliza
  target.situacion = source.situacion
  target.tipo = source.tipo
  target.vencimientoDesde = source.vencimientoDesde
}

export function formatRecibosDate(value: string) {
  if (!value) {
    return ''
  }

  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value.slice(0, 10)}T00:00:00`))
}

export function useRecibos() {
  const filters = reactive<RecibosFilters>(createEmptyRecibosFilters())
  const draftFilters = reactive<RecibosFilters>(createEmptyRecibosFilters())
  const items = ref<ReciboListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const situacionOptions = ref<string[]>([])
  const tipoOptions = ref<string[]>([])
  const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'

  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pagination.pageSize)))
  const pagedItems = computed(() => items.value)
  const firstVisible = computed(() =>
    total.value === 0 ? 0 : (pagination.page - 1) * pagination.pageSize + 1,
  )
  const lastVisible = computed(() => Math.min(pagination.page * pagination.pageSize, total.value))
  const resultLabel = computed(() => {
    return total.value === 1 ? 'recibo' : 'recibos'
  })
  const tableCaption = computed(() => {
    if (isBackendMode) {
      return `Recibos BBDD local/API: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Listado read-only minimizado sin importes reales, banco ni cobro.`
    }

    return `Recibos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin API backend ni datos reales.`
  })
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  async function loadCatalogs() {
    const catalogs = await getRecibosCatalogs()
    situacionOptions.value = catalogs.situaciones
    tipoOptions.value = catalogs.tipos
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchRecibosApi({
        recibo: filters.recibo || undefined,
        poliza: filters.poliza || undefined,
        situacion: filters.situacion || undefined,
        tipo: filters.tipo || undefined,
        vencimientoDesde: filters.vencimientoDesde || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaVencimiento:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudo cargar Recibos.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function searchRecibos() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function clearFilters() {
    copyFilters(draftFilters, createEmptyRecibosFilters())
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function changePage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      pagination.page = page
      await refresh()
    }
  }

  async function changePageSize(pageSize: number) {
    pagination.pageSize = pageSize
    pagination.page = 1
    await refresh()
  }

  onMounted(async () => {
    try {
      await loadCatalogs()
    } catch {
      error.value = 'No se pudieron cargar los catalogos de Recibos.'
    }

    await refresh()
  })

  return {
    canGoNext,
    canGoPrevious,
    changePage,
    changePageSize,
    clearFilters,
    draftFilters,
    error,
    firstVisible,
    formatDate: formatRecibosDate,
    isBackendMode,
    lastVisible,
    loading,
    pageSizeOptions: recibosPageSizeOptions,
    pagedItems,
    pagination,
    resultLabel,
    searchRecibos,
    situacionOptions,
    tableCaption,
    tipoOptions,
    total,
    totalPages,
  }
}
