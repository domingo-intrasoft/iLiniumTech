import { onMounted, reactive, ref } from 'vue'

import { toPolizasUserMessage } from '@/services/apiErrors'
import { getBlockingRuntimeConfigMessage } from '@/services/runtimeConfig'
import { useSession } from '@/services/session'

import { getAutosParticularesCatalogs, searchAutosParticulares } from './autosParticularesApi'
import {
  autosParticularesCatalogsFixture,
  autosParticularesScopeFixture,
} from './autosParticularesFixture'
import type {
  AutoParticularPolizaListItem,
  AutosParticularesCatalogs,
  AutosParticularesQueryFilters,
  AutosParticularesScope,
} from './autosParticularesTypes'

export function useAutosParticulares() {
  const { session, loading: sessionLoading, error: sessionError, loadSession } = useSession()
  const catalogs = ref<AutosParticularesCatalogs>(autosParticularesCatalogsFixture)
  const catalogsLoading = ref(false)
  const catalogsError = ref<string | null>(null)
  const items = ref<AutoParticularPolizaListItem[]>([])
  const scope = ref<AutosParticularesScope>(autosParticularesScopeFixture)
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const runtimeError = ref<string | null>(getBlockingRuntimeConfigMessage())
  const contextBlocked = ref(false)
  const filters = reactive<AutosParticularesQueryFilters>({
    numero: '',
    cliente: '',
    estado: '',
    compania: '',
    fechaEfectoDesde: '',
    fechaEfectoHasta: '',
  })
  const pagination = reactive({
    page: 1,
    pageSize: 25,
  })

  async function ensureBackendContext() {
    runtimeError.value = getBlockingRuntimeConfigMessage()

    if (runtimeError.value) {
      error.value = runtimeError.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    const currentSession = await loadSession()
    if (import.meta.env.VITE_USE_BACKEND === 'true' && !currentSession) {
      error.value = sessionError.value ?? 'No se pudo validar la sesion antes de consultar autos.'
      runtimeError.value = error.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    if (
      import.meta.env.VITE_USE_BACKEND === 'true' &&
      currentSession?.polizasExecutionContextRequired &&
      currentSession.brokerId === null
    ) {
      error.value = 'Configura un broker para consultar autos particulares.'
      runtimeError.value = error.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    contextBlocked.value = false
    runtimeError.value = null
    return true
  }

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      if (!(await ensureBackendContext())) {
        return
      }

      const result = await searchAutosParticulares({
        numero: filters.numero || undefined,
        cliente: filters.cliente || undefined,
        estado: filters.estado || undefined,
        compania: filters.compania || undefined,
        fechaEfectoDesde: filters.fechaEfectoDesde || undefined,
        fechaEfectoHasta: filters.fechaEfectoHasta || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'fechaEfecto:desc',
      })
      items.value = result.items
      scope.value = result.scope ?? autosParticularesScopeFixture
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch (exception) {
      error.value = toPolizasUserMessage(exception, 'No se pudieron cargar los autos particulares.')
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
      catalogs.value = await getAutosParticularesCatalogs()
    } catch (exception) {
      catalogsError.value = toPolizasUserMessage(
        exception,
        'No se pudieron cargar los catalogos de autos.',
      )
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
    scope,
    total,
    loading,
    error,
    runtimeError,
    contextBlocked,
    filters,
    pagination,
    refresh,
    setPage,
    setPageSize,
  }
}
