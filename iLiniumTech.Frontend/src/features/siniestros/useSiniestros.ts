import { computed, onMounted, reactive, ref } from 'vue'

import { getSiniestrosCatalogs, searchSiniestros as searchSiniestrosApi } from './siniestrosApi'
import type { SiniestroListItem, SiniestrosFilters } from './siniestrosTypes'

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

function copyFilters(target: SiniestrosFilters, source: SiniestrosFilters) {
  target.referencia = source.referencia
  target.poliza = source.poliza
  target.estado = source.estado
  target.prioridad = source.prioridad
  target.fechaDesde = source.fechaDesde
}

export function formatSiniestroDate(value: string) {
  if (!value) {
    return ''
  }

  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value.slice(0, 10)}T00:00:00`))
}

export function useSiniestros() {
  const filters = reactive<SiniestrosFilters>(createEmptySiniestrosFilters())
  const draftFilters = reactive<SiniestrosFilters>(createEmptySiniestrosFilters())
  const items = ref<SiniestroListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const estadoOptions = ref<string[]>([])
  const prioridadOptions = ref<string[]>([])
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
    return total.value === 1 ? 'siniestro' : 'siniestros'
  })
  const tableCaption = computed(() => {
    if (isBackendMode) {
      return `Siniestros BBDD local/API: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Listado read-only minimizado sin detalle ni exportacion.`
    }

    return `Siniestros fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin API backend ni datos reales.`
  })
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  async function loadCatalogs() {
    const catalogs = await getSiniestrosCatalogs()
    estadoOptions.value = catalogs.estados
    prioridadOptions.value = catalogs.prioridades
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchSiniestrosApi({
        referencia: filters.referencia || undefined,
        poliza: filters.poliza || undefined,
        estado: filters.estado || undefined,
        prioridad: filters.prioridad || undefined,
        fechaDesde: filters.fechaDesde || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaSiniestro:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudo cargar Siniestros.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function searchSiniestros() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function clearFilters() {
    copyFilters(draftFilters, createEmptySiniestrosFilters())
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
      error.value = 'No se pudieron cargar los catalogos de Siniestros.'
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
    estadoOptions,
    firstVisible,
    formatDate: formatSiniestroDate,
    isBackendMode,
    lastVisible,
    loading,
    pageSizeOptions: siniestrosPageSizeOptions,
    pagedItems,
    pagination,
    prioridadOptions,
    resultLabel,
    searchSiniestros,
    tableCaption,
    total,
    totalPages,
  }
}
