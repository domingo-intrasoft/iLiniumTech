import { onMounted, reactive, ref } from 'vue'

import { useSession } from '@/services/session'

import { getPolizasCatalogs, searchPolizas } from './polizasApi'
import { polizasCatalogsFixture } from './polizasFixture'
import type { PolizaListItem, PolizasCatalogs, PolizasQueryFilters } from './polizasTypes'

export function usePolizas() {
  const { session, loading: sessionLoading, error: sessionError, loadSession } = useSession()
  const catalogs = ref<PolizasCatalogs>(polizasCatalogsFixture)
  const catalogsLoading = ref(false)
  const catalogsError = ref<string | null>(null)
  const items = ref<PolizaListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const contextBlocked = ref(false)
  const filters = reactive<PolizasQueryFilters>({
    numero: '',
    cliente: '',
    estado: '',
    compania: '',
    ramo: '',
    fechaEfectoDesde: '',
    fechaEfectoHasta: '',
  })
  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  async function ensureBackendContext() {
    const currentSession = await loadSession()
    if (
      import.meta.env.VITE_USE_BACKEND === 'true' &&
      currentSession?.polizasExecutionContextRequired &&
      currentSession.brokerId === null
    ) {
      error.value = 'Configura un broker para consultar polizas.'
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    contextBlocked.value = false
    return true
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      if (!(await ensureBackendContext())) {
        return
      }

      const result = await searchPolizas({
        numero: filters.numero || undefined,
        cliente: filters.cliente || undefined,
        estado: filters.estado || undefined,
        compania: filters.compania || undefined,
        ramo: filters.ramo || undefined,
        fechaEfectoDesde: filters.fechaEfectoDesde || undefined,
        fechaEfectoHasta: filters.fechaEfectoHasta || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaEfecto:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudieron cargar las polizas.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function setPage(page: number) {
    if (page === pagination.page || loading.value) {
      return
    }

    pagination.page = page
    await refresh()
  }

  async function setPageSize(pageSize: number) {
    if (pageSize === pagination.pageSize || loading.value) {
      return
    }

    pagination.page = 1
    pagination.pageSize = pageSize
    await refresh()
  }

  async function loadCatalogs() {
    catalogsLoading.value = true
    catalogsError.value = null

    try {
      catalogs.value = await getPolizasCatalogs()
    } catch {
      catalogsError.value = 'No se pudieron cargar los catalogos de polizas.'
    } finally {
      catalogsLoading.value = false
    }
  }

  onMounted(() => {
    void refresh()
    void loadCatalogs()
  })

  return {
    session,
    sessionLoading,
    sessionError,
    catalogs,
    catalogsLoading,
    catalogsError,
    items,
    total,
    loading,
    error,
    contextBlocked,
    filters,
    pagination,
    refresh,
    setPage,
    setPageSize,
  }
}
