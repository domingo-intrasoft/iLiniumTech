import { computed, onMounted, reactive, ref } from 'vue'

import { pageSizeOptions as suplementosPageSizeOptions } from './fixtures'
import { getSuplementosCatalogs, searchSuplementos as searchSuplementosApi } from './suplementosApi'
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

function copyFilters(target: SuplementosFilters, source: SuplementosFilters) {
  target.texto = source.texto
  target.poliza = source.poliza
  target.tipo = source.tipo
  target.situacion = source.situacion
  target.fechaDesde = source.fechaDesde
}

export function formatSuplementosDate(value: string) {
  if (!value) {
    return ''
  }

  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value.slice(0, 10)}T00:00:00`))
}

export function useSuplementos() {
  const filters = reactive<SuplementosFilters>(createEmptySuplementosFilters())
  const draftFilters = reactive<SuplementosFilters>(createEmptySuplementosFilters())
  const items = ref<SuplementoListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const tipoOptions = ref<string[]>([])
  const situacionOptions = ref<string[]>([])
  const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'

  const pagination = reactive({
    page: 1,
    pageSize: 2,
  })

  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pagination.pageSize)))
  const pagedItems = computed(() => items.value)
  const firstVisible = computed(() =>
    total.value === 0 ? 0 : (pagination.page - 1) * pagination.pageSize + 1,
  )
  const lastVisible = computed(() => Math.min(pagination.page * pagination.pageSize, total.value))
  const resultLabel = computed(() => (total.value === 1 ? 'suplemento' : 'suplementos'))
  const tableCaption = computed(() => {
    if (isBackendMode) {
      return `Suplementos BBDD local/API: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Listado read-only minimizado sin workflows, adjuntos, importes, banco ni datos personales.`
    }

    return `Suplementos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin workflows ni API backend.`
  })
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  async function loadCatalogs() {
    const catalogs = await getSuplementosCatalogs()
    tipoOptions.value = catalogs.tipos
    situacionOptions.value = catalogs.situaciones
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchSuplementosApi({
        texto: filters.texto || undefined,
        poliza: filters.poliza || undefined,
        tipo: filters.tipo || undefined,
        situacion: filters.situacion || undefined,
        fechaDesde: filters.fechaDesde || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaEfecto:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudo cargar Suplementos.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function searchSuplementos() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function clearFilters() {
    copyFilters(draftFilters, createEmptySuplementosFilters())
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
      error.value = 'No se pudieron cargar los catalogos de Suplementos.'
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
    formatDate: formatSuplementosDate,
    isBackendMode,
    lastVisible,
    loading,
    pageSizeOptions: suplementosPageSizeOptions,
    pagedItems,
    pagination,
    resultLabel,
    searchSuplementos,
    situacionOptions,
    tableCaption,
    tipoOptions,
    total,
    totalPages,
  }
}
