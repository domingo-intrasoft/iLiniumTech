import { computed, onMounted, reactive, ref } from 'vue'

import { getClientesCatalogs, searchClientes as searchClientesApi } from './clientesApi'
import type { ClienteListItem, ClientesFilters } from './types'

function createEmptyClientesFilters(): ClientesFilters {
  return {
    texto: '',
    estado: '',
    segmento: '',
    fechaAltaDesde: '',
  }
}

function copyFilters(target: ClientesFilters, source: ClientesFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.segmento = source.segmento
  target.fechaAltaDesde = source.fechaAltaDesde
}

export function formatClienteDate(value: string) {
  if (!value) {
    return ''
  }

  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value.slice(0, 10)}T00:00:00`))
}

export function useClientes() {
  const filters = reactive<ClientesFilters>(createEmptyClientesFilters())
  const draftFilters = reactive<ClientesFilters>(createEmptyClientesFilters())
  const items = ref<ClienteListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const estadoOptions = ref<string[]>([])
  const segmentoOptions = ref<string[]>([])
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
  const resultLabel = computed(() => {
    if (isBackendMode) {
      return total.value === 1 ? 'cliente' : 'clientes'
    }

    return total.value === 1 ? 'cliente demo' : 'clientes demo'
  })
  const tableCaption = computed(() => {
    if (isBackendMode) {
      return `Clientes BBDD local/API: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Listado read-only minimizado.`
    }

    return `Clientes fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin PII real ni API backend.`
  })
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  async function loadCatalogs() {
    const catalogs = await getClientesCatalogs()
    estadoOptions.value = catalogs.estados
    segmentoOptions.value = catalogs.segmentos
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchClientesApi({
        texto: filters.texto || undefined,
        estado: filters.estado || undefined,
        segmento: filters.segmento || undefined,
        fechaAltaDesde: filters.fechaAltaDesde || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaAlta:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudo cargar Clientes.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function searchClientes() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function clearFilters() {
    copyFilters(draftFilters, createEmptyClientesFilters())
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

  async function changePageSize(event: Event) {
    pagination.pageSize = Number((event.target as HTMLSelectElement).value)
    pagination.page = 1
    await refresh()
  }

  onMounted(async () => {
    try {
      await loadCatalogs()
    } catch {
      error.value = 'No se pudieron cargar los catalogos de Clientes.'
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
    filters,
    firstVisible,
    formatDate: formatClienteDate,
    isBackendMode,
    lastVisible,
    loading,
    pagedItems,
    pagination,
    resultLabel,
    searchClientes,
    segmentoOptions,
    tableCaption,
    total,
    totalPages,
  }
}
