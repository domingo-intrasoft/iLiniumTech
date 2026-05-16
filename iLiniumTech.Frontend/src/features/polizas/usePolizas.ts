import { onMounted, reactive, ref } from 'vue'

import { clearAuthSession } from '@/features/auth/authSession'
import { isPolizasAccessError, toPolizasUserError } from '@/services/apiErrors'
import { getBlockingRuntimeConfigMessage } from '@/services/runtimeConfig'
import { type SessionContext, useSession } from '@/services/session'

import { getPolizasCatalogs, searchPolizas } from './polizasApi'
import { polizasCatalogsFixture } from './polizasFixture'
import type { PolizaListItem, PolizasCatalogs, PolizasQueryFilters } from './polizasTypes'

const POLIZAS_READ_PERMISSION = 'polizas.read'
const POLIZAS_CATALOGS_PERMISSION = 'polizas.catalogs'
const POLIZAS_ACCESS_DENIED_MESSAGE = 'La sesion actual no tiene permiso para consultar polizas.'

function permissionIsAllowed(context: SessionContext, permission: string) {
  return (
    context.authMode === 'ApiKey' ||
    !Array.isArray(context.permissions) ||
    context.permissions.includes(permission)
  )
}

export function usePolizas() {
  const {
    session,
    loading: sessionLoading,
    error: sessionError,
    errorKind: sessionErrorKind,
    loadSession,
  } = useSession()
  const catalogs = ref<PolizasCatalogs>(polizasCatalogsFixture)
  const catalogsLoading = ref(false)
  const catalogsError = ref<string | null>(null)
  const items = ref<PolizaListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const runtimeError = ref<string | null>(getBlockingRuntimeConfigMessage())
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
    const backendEnabled = import.meta.env.VITE_USE_BACKEND === 'true'
    runtimeError.value = getBlockingRuntimeConfigMessage()

    if (runtimeError.value) {
      error.value = runtimeError.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    const currentSession = await loadSession()
    if (!backendEnabled) {
      contextBlocked.value = false
      runtimeError.value = null
      return true
    }

    if (!currentSession) {
      const validationError =
        sessionError.value ?? 'No se pudo validar la sesion antes de consultar polizas.'

      if (sessionErrorKind.value === 'unauthenticated') {
        clearAuthSession()
      }

      error.value = validationError
      runtimeError.value = error.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    if (currentSession.polizasExecutionContextRequired && currentSession.brokerId === null) {
      error.value = 'Configura un broker para consultar polizas.'
      runtimeError.value = error.value
      contextBlocked.value = true
      items.value = []
      total.value = 0
      return false
    }

    if (!permissionIsAllowed(currentSession, POLIZAS_READ_PERMISSION)) {
      error.value = POLIZAS_ACCESS_DENIED_MESSAGE
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
    } catch (exception) {
      const userError = toPolizasUserError(exception, 'No se pudieron cargar las polizas.')
      error.value = userError.message
      if (userError.kind === 'unauthenticated') {
        clearAuthSession()
      }
      if (isPolizasAccessError(userError)) {
        runtimeError.value = userError.message
        contextBlocked.value = true
      }
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
      const runtimeConfigError = getBlockingRuntimeConfigMessage()
      if (runtimeConfigError) {
        catalogsError.value = runtimeConfigError
        return
      }

      if (import.meta.env.VITE_USE_BACKEND === 'true') {
        const currentSession = await loadSession()
        if (!currentSession) {
          catalogsError.value =
            sessionError.value ?? 'No se pudo validar la sesion antes de cargar catalogos.'
          return
        }

        if (!permissionIsAllowed(currentSession, POLIZAS_CATALOGS_PERMISSION)) {
          catalogsError.value = POLIZAS_ACCESS_DENIED_MESSAGE
          return
        }
      }

      catalogs.value = await getPolizasCatalogs()
    } catch (exception) {
      const userError = toPolizasUserError(
        exception,
        'No se pudieron cargar los catalogos de polizas.',
      )
      catalogsError.value = userError.message
      if (userError.kind === 'unauthenticated') {
        clearAuthSession()
        runtimeError.value = userError.message
        contextBlocked.value = true
      }
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
    runtimeError,
    contextBlocked,
    filters,
    pagination,
    refresh,
    setPage,
    setPageSize,
  }
}
