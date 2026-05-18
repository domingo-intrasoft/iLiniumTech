<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import {
  RouterLink,
  useRoute,
  useRouter,
  type LocationQuery,
  type LocationQueryRaw,
} from 'vue-router'

import { clearAuthSession, switchAuthBroker, useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'
import { toPolizasUserError } from '@/services/apiErrors'

import PolizaCrudPanel from './PolizaCrudPanel.vue'
import PolizasFilters from './PolizasFilters.vue'
import PolizasTable from './PolizasTable.vue'
import { createPoliza, deletePoliza, updatePoliza } from './polizasApi'
import {
  moduleActions,
  statusActions,
  topBadges,
  type PolizasCatalogKey,
  type PolizasSearchCriteria,
} from './polizasConstants'
import { usePolizas } from './usePolizas'
import {
  POLIZA_MVP_PREFIX,
  type PolizaCreatePayload,
  type PolizaListItem,
  type PolizasQueryFilters,
  type PolizaUpdatePayload,
} from './polizasTypes'

const DEFAULT_PAGE = 1
const DEFAULT_PAGE_SIZE = 25
const POLIZAS_PAGE_SIZES = new Set([10, 25, 50])

const {
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
  loadCatalogs,
} = usePolizas()

const route = useRoute()
const router = useRouter()
const { userLabel, logout } = useAuthSession()
const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'
const dataOriginLabel = computed(() => (isBackendMode ? 'API polizas' : 'Fixture local'))
const blockedToolbarActionDescription =
  'Accion bloqueada en el MVP read-only hasta SDD, contrato API, permisos y UAT.'
const blockedScopeActionDescription =
  'Scope de Polizas aparcado: no carga datos ni activa permisos hasta SDD, contrato API y UAT.'
const POLIZAS_DETAIL_PERMISSION = 'polizas.detail'
const POLIZAS_CREATE_PERMISSION = 'polizas.create'
const POLIZAS_UPDATE_PERMISSION = 'polizas.update'
const POLIZAS_DELETE_PERMISSION = 'polizas.delete'
const brokerChanging = ref(false)
const brokerError = ref<string | null>(null)
const crudMode = ref<'create' | 'edit'>('create')
const crudOpen = ref(false)
const crudItem = ref<PolizaListItem | null>(null)
const crudSaving = ref(false)
const crudError = ref<string | null>(null)
const crudSuccess = ref<string | null>(null)
const writeBusyId = ref<string | null>(null)

interface PolizasRouteState {
  filters: PolizasQueryFilters
  page: number
  pageSize: number
}

interface ActivePolizasFilterDefinition {
  key: keyof PolizasQueryFilters
  label: string
  catalogKey?: PolizasCatalogKey
}

const activeFilterDefinitions: ActivePolizasFilterDefinition[] = [
  { key: 'numero', label: 'Poliza' },
  { key: 'cliente', label: 'Cliente' },
  { key: 'estado', label: 'Tipo poliza', catalogKey: 'tipoPoliza' },
  { key: 'compania', label: 'Compania', catalogKey: 'compania' },
  { key: 'ramo', label: 'Ramo', catalogKey: 'ramo' },
  { key: 'fechaEfectoDesde', label: 'Efecto desde' },
  { key: 'fechaEfectoHasta', label: 'Efecto hasta' },
]

function firstQueryValue(value: LocationQuery[string] | undefined) {
  const item = Array.isArray(value) ? value[0] : value
  return typeof item === 'string' ? item.trim() : ''
}

function readPositiveInteger(
  query: LocationQuery,
  key: string,
  fallback: number,
  allowedValues?: Set<number>,
) {
  const rawValue = firstQueryValue(query[key])
  const parsed = Number(rawValue)
  const allowed = !allowedValues || allowedValues.has(parsed)
  return Number.isInteger(parsed) && parsed > 0 && allowed ? parsed : fallback
}

function readIsoDateQueryValue(value: LocationQuery[string] | undefined) {
  const rawValue = firstQueryValue(value)
  const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(rawValue)
  if (!match) {
    return ''
  }

  const year = Number(match[1])
  const month = Number(match[2])
  const day = Number(match[3])
  const candidate = new Date(Date.UTC(year, month - 1, day))
  return candidate.getUTCFullYear() === year &&
    candidate.getUTCMonth() === month - 1 &&
    candidate.getUTCDate() === day
    ? rawValue
    : ''
}

function parsePolizasRouteQuery(query: LocationQuery): PolizasRouteState {
  return {
    filters: {
      numero: firstQueryValue(query.numero),
      cliente: firstQueryValue(query.cliente),
      estado: firstQueryValue(query.estado),
      compania: firstQueryValue(query.compania),
      ramo: firstQueryValue(query.ramo),
      fechaEfectoDesde: readIsoDateQueryValue(query.fechaEfectoDesde),
      fechaEfectoHasta: readIsoDateQueryValue(query.fechaEfectoHasta),
    },
    page: readPositiveInteger(query, 'page', DEFAULT_PAGE),
    pageSize: readPositiveInteger(query, 'pageSize', DEFAULT_PAGE_SIZE, POLIZAS_PAGE_SIZES),
  }
}

function addQueryValue(query: LocationQueryRaw, key: string, value: string) {
  const cleanValue = value.trim()
  if (cleanValue) {
    query[key] = cleanValue
  }
}

function buildPolizasRouteQuery(
  currentFilters: PolizasQueryFilters,
  currentPagination: { page: number; pageSize: number },
): LocationQueryRaw {
  const query: LocationQueryRaw = {
    page: String(currentPagination.page),
    pageSize: String(currentPagination.pageSize),
  }

  addQueryValue(query, 'numero', currentFilters.numero)
  addQueryValue(query, 'cliente', currentFilters.cliente)
  addQueryValue(query, 'estado', currentFilters.estado)
  addQueryValue(query, 'compania', currentFilters.compania)
  addQueryValue(query, 'ramo', currentFilters.ramo)
  addQueryValue(query, 'fechaEfectoDesde', currentFilters.fechaEfectoDesde)
  addQueryValue(query, 'fechaEfectoHasta', currentFilters.fechaEfectoHasta)

  return query
}

function queryFingerprint(query: LocationQuery | LocationQueryRaw) {
  return Object.entries(query)
    .flatMap(([key, value]) => {
      const values = Array.isArray(value) ? value : [value]
      return values
        .filter((item) => item !== null && item !== undefined)
        .map((item) => [key, String(item)] as const)
    })
    .sort(([leftKey, leftValue], [rightKey, rightValue]) =>
      leftKey === rightKey ? leftValue.localeCompare(rightValue) : leftKey.localeCompare(rightKey),
    )
    .map(([key, value]) => `${key}=${value}`)
    .join('&')
}

function applyRouteState(state: PolizasRouteState) {
  filters.numero = state.filters.numero
  filters.cliente = state.filters.cliente
  filters.estado = state.filters.estado
  filters.compania = state.filters.compania
  filters.ramo = state.filters.ramo
  filters.fechaEfectoDesde = state.filters.fechaEfectoDesde
  filters.fechaEfectoHasta = state.filters.fechaEfectoHasta
  pagination.page = state.page
  pagination.pageSize = state.pageSize
}

function stateMatchesCurrentPolizas(state: PolizasRouteState) {
  return (
    filters.numero === state.filters.numero &&
    filters.cliente === state.filters.cliente &&
    filters.estado === state.filters.estado &&
    filters.compania === state.filters.compania &&
    filters.ramo === state.filters.ramo &&
    filters.fechaEfectoDesde === state.filters.fechaEfectoDesde &&
    filters.fechaEfectoHasta === state.filters.fechaEfectoHasta &&
    pagination.page === state.page &&
    pagination.pageSize === state.pageSize
  )
}

async function replacePolizasRouteQuery() {
  const query = buildPolizasRouteQuery(filters, pagination)

  if (queryFingerprint(route.query) !== queryFingerprint(query)) {
    await router.replace({ name: 'polizas', query })
  }
}

async function refreshWithRouteState() {
  await replacePolizasRouteQuery()
  await refresh()
  await replacePolizasRouteQuery()
}

applyRouteState(parsePolizasRouteQuery(route.query))

const sessionLabel = computed(() => {
  if (sessionLoading.value) {
    return 'Contexto...'
  }

  if (sessionError.value) {
    return 'Sesion no disponible'
  }

  if (session.value?.brokerId) {
    return `Broker ${session.value.brokerId}`
  }

  return session.value?.polizasExecutionContextRequired ? 'Broker requerido' : 'Modo local'
})

const sessionNeedsAttention = computed(
  () =>
    Boolean(sessionError.value) ||
    Boolean(brokerError.value) ||
    Boolean(session.value?.polizasExecutionContextRequired && !session.value.brokerId),
)

const brokerOptions = computed(() => {
  if (!isBackendMode || session.value?.authMode !== 'DemoSession') {
    return []
  }

  const brokerIds = session.value.allowedBrokerIds ?? []
  const uniqueBrokerIds = Array.from(
    new Set(brokerIds.filter((brokerId) => Number.isInteger(brokerId) && brokerId > 0)),
  )

  return uniqueBrokerIds.length > 1 ? uniqueBrokerIds : []
})

const filterCriteria = computed<PolizasSearchCriteria>(() => ({
  numero: filters.numero,
  cliente: filters.cliente,
  estado: filters.estado,
  compania: filters.compania,
  ramo: filters.ramo,
  fechaEfectoDesde: filters.fechaEfectoDesde,
  fechaEfectoHasta: filters.fechaEfectoHasta,
}))

function catalogValueLabel(catalogKey: PolizasCatalogKey, value: string) {
  return catalogs.value[catalogKey].find((option) => option.value === value)?.label ?? value
}

const activeFilterChips = computed(() =>
  activeFilterDefinitions.flatMap((definition) => {
    const rawValue = filters[definition.key].trim()

    if (!rawValue) {
      return []
    }

    return [
      {
        key: definition.key,
        label: definition.label,
        value: definition.catalogKey
          ? catalogValueLabel(definition.catalogKey, rawValue)
          : rawValue,
      },
    ]
  }),
)

const detailQuery = computed(() => buildPolizasRouteQuery(filters, pagination))
const canOpenPolizaDetail = computed(() => {
  if (!isBackendMode) {
    return true
  }

  const currentSession = session.value
  if (!currentSession) {
    return false
  }

  return (
    currentSession.authMode === 'ApiKey' ||
    !Array.isArray(currentSession.permissions) ||
    currentSession.permissions.includes(POLIZAS_DETAIL_PERMISSION)
  )
})
const detailUnavailableMessage = computed(() =>
  canOpenPolizaDetail.value
    ? null
    : 'La sesion actual no tiene permiso para abrir el detalle de polizas.',
)
const hasBackendWriteContext = computed(() => {
  const currentSession = session.value

  return (
    isBackendMode &&
    !contextBlocked.value &&
    currentSession !== null &&
    currentSession.authMode !== 'ApiKey' &&
    (!currentSession.polizasExecutionContextRequired || currentSession.brokerId !== null)
  )
})
const canCreatePoliza = computed(
  () => hasBackendWriteContext.value && hasExplicitPermission(POLIZAS_CREATE_PERMISSION),
)
const canUpdatePoliza = computed(
  () => hasBackendWriteContext.value && hasExplicitPermission(POLIZAS_UPDATE_PERMISSION),
)
const canDeletePoliza = computed(
  () => hasBackendWriteContext.value && hasExplicitPermission(POLIZAS_DELETE_PERMISSION),
)
const crudStateLabel = computed(() =>
  canCreatePoliza.value || canUpdatePoliza.value || canDeletePoliza.value
    ? 'CRUD BBDD limitado'
    : 'Solo lectura',
)

function hasExplicitPermission(permission: string) {
  const permissions = session.value?.permissions
  return Array.isArray(permissions) && permissions.includes(permission)
}

function isPolizaMvpEditable(item: PolizaListItem) {
  return item.numero.startsWith(POLIZA_MVP_PREFIX) && /^[1-9]\d*$/.test(item.id)
}

async function executeSearch(criteria: PolizasSearchCriteria) {
  filters.numero = criteria.numero
  filters.cliente = criteria.cliente
  filters.estado = criteria.estado
  filters.compania = criteria.compania
  filters.ramo = criteria.ramo
  filters.fechaEfectoDesde = criteria.fechaEfectoDesde
  filters.fechaEfectoHasta = criteria.fechaEfectoHasta
  pagination.page = 1
  await refreshWithRouteState()
}

async function clearFilters() {
  filters.numero = ''
  filters.cliente = ''
  filters.estado = ''
  filters.compania = ''
  filters.ramo = ''
  filters.fechaEfectoDesde = ''
  filters.fechaEfectoHasta = ''
  pagination.page = 1
  await refreshWithRouteState()
}

async function changePage(page: number) {
  if (page === pagination.page || loading.value) {
    return
  }

  pagination.page = page
  await refreshWithRouteState()
}

async function changePageSize(pageSize: number) {
  if (pageSize === pagination.pageSize || loading.value) {
    return
  }

  pagination.page = 1
  pagination.pageSize = pageSize
  await refreshWithRouteState()
}

async function changeBroker(brokerId: number) {
  if (brokerChanging.value || brokerId === session.value?.brokerId) {
    return
  }

  if (!brokerOptions.value.includes(brokerId)) {
    brokerError.value = 'El broker seleccionado no esta disponible para la sesion actual.'
    return
  }

  brokerChanging.value = true
  brokerError.value = null

  try {
    await switchAuthBroker(brokerId)
    pagination.page = 1
    await refreshWithRouteState()
    await loadCatalogs()
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo cambiar el broker activo.')
    brokerError.value = userError.message
    if (userError.kind === 'unauthenticated') {
      clearAuthSession()
      await router.replace({ name: 'login' })
    }
  } finally {
    brokerChanging.value = false
  }
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}

function openCreatePanel() {
  if (!canCreatePoliza.value) {
    crudError.value = 'La sesion actual no tiene permiso para crear polizas MVP.'
    return
  }

  crudMode.value = 'create'
  crudItem.value = null
  crudError.value = null
  crudSuccess.value = null
  crudOpen.value = true
}

function openEditPanel(item: PolizaListItem) {
  if (!canUpdatePoliza.value || !isPolizaMvpEditable(item)) {
    crudError.value = `Solo se pueden editar registros ${POLIZA_MVP_PREFIX} con id estable.`
    return
  }

  crudMode.value = 'edit'
  crudItem.value = item
  crudError.value = null
  crudSuccess.value = null
  crudOpen.value = true
}

function closeCrudPanel() {
  if (crudSaving.value) {
    return
  }

  crudOpen.value = false
  crudItem.value = null
  crudError.value = null
}

async function submitCrud(payload: PolizaCreatePayload | PolizaUpdatePayload) {
  crudSaving.value = true
  crudError.value = null
  crudSuccess.value = null

  try {
    if (crudMode.value === 'create') {
      await createPoliza(payload as PolizaCreatePayload)
      crudSuccess.value = 'Poliza MVP creada.'
    } else if (crudItem.value) {
      await updatePoliza(crudItem.value.id, payload as PolizaUpdatePayload)
      crudSuccess.value = 'Poliza MVP actualizada.'
    }

    crudOpen.value = false
    crudItem.value = null
    await refreshWithRouteState()
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo guardar la poliza MVP.')
    crudError.value = userError.message
    if (userError.kind === 'unauthenticated') {
      clearAuthSession()
      await router.replace({ name: 'login' })
    }
  } finally {
    crudSaving.value = false
  }
}

async function deleteMvpPoliza(item: PolizaListItem) {
  if (!canDeletePoliza.value || !isPolizaMvpEditable(item)) {
    crudError.value = `Solo se pueden eliminar registros ${POLIZA_MVP_PREFIX} con id estable.`
    return
  }

  const confirmed = window.confirm(`Eliminar la poliza MVP ${item.numero}?`)
  if (!confirmed) {
    return
  }

  writeBusyId.value = item.id
  crudError.value = null
  crudSuccess.value = null

  try {
    await deletePoliza(item.id)
    crudSuccess.value = 'Poliza MVP eliminada.'
    if (crudItem.value?.id === item.id) {
      closeCrudPanel()
    }
    await refreshWithRouteState()
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo eliminar la poliza MVP.')
    crudError.value = userError.message
    if (userError.kind === 'unauthenticated') {
      clearAuthSession()
      await router.replace({ name: 'login' })
    }
  } finally {
    writeBusyId.value = null
  }
}

watch(
  () => route.query,
  async (query) => {
    const routeState = parsePolizasRouteQuery(query)

    if (stateMatchesCurrentPolizas(routeState)) {
      return
    }

    applyRouteState(routeState)
    await refresh()
    await replacePolizasRouteQuery()
  },
)

onMounted(() => {
  void replacePolizasRouteQuery()
})
</script>

<template>
  <AppShell
    content-id="polizas-content"
    section-title="Polizas"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    :broker-options="brokerOptions"
    :active-broker-id="session?.brokerId ?? null"
    :broker-changing="brokerChanging"
    :broker-error="brokerError"
    appbuilder-chrome
    show-sign-out
    @broker-change="changeBroker"
    @sign-out="signOut"
  >
    <div id="polizas-content" class="polizas-toolbar">
      <h1 class="sr-only">Polizas</h1>
      <p id="polizas-toolbar-blocked-actions" class="sr-only">
        {{ blockedToolbarActionDescription }}
      </p>
      <div class="toolbar-groups">
        <div class="action-group">
          <button
            class="square-action"
            :class="{ active: canCreatePoliza }"
            type="button"
            aria-label="Nueva poliza MVP"
            title="Nueva poliza MVP"
            :disabled="!canCreatePoliza || crudSaving"
            @click="openCreatePanel"
          >
            <i class="pi pi-plus" aria-hidden="true"></i>
          </button>
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            :title="blockedToolbarActionDescription"
            aria-describedby="polizas-toolbar-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
        <div class="action-group compact">
          <button
            v-for="action in statusActions"
            :key="action.label"
            class="square-action dark"
            type="button"
            :aria-label="action.label"
            :title="blockedToolbarActionDescription"
            aria-describedby="polizas-toolbar-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>

      <div class="context-buttons">
        <p id="polizas-scope-blocked-actions" class="sr-only">
          {{ blockedScopeActionDescription }}
        </p>
        <RouterLink
          to="/polizas/flotas"
          title="Scope estatico bloqueado hasta SDD, regla funcional, permisos y UAT"
          aria-describedby="polizas-scope-blocked-actions"
        >
          Polizas de flota
        </RouterLink>
        <RouterLink
          to="/polizas/colectivas"
          title="Scope estatico bloqueado hasta SDD, regla funcional, permisos y UAT"
          aria-describedby="polizas-scope-blocked-actions"
        >
          Polizas colectivas
        </RouterLink>
        <button
          type="button"
          title="Pendiente de SDD, regla funcional, permisos y contrato API"
          aria-describedby="polizas-scope-blocked-actions"
          disabled
        >
          Polizas Externas
        </button>
      </div>
    </div>

    <section
      class="runtime-strip"
      :class="{
        'runtime-strip-compact': !crudSuccess && !crudError && !contextBlocked && !brokerError,
      }"
      aria-label="Contexto de polizas"
    >
      <span><i class="pi pi-lock" aria-hidden="true"></i> {{ crudStateLabel }}</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> {{ dataOriginLabel }}</span>
      <span v-if="crudSuccess" class="success" role="status">
        <i class="pi pi-check-circle" aria-hidden="true"></i>
        {{ crudSuccess }}
      </span>
      <span v-if="crudError" class="warning" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        {{ crudError }}
      </span>
      <span v-if="contextBlocked" class="warning" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        {{ runtimeError ?? 'Broker requerido' }}
      </span>
      <span v-if="brokerError" class="warning" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        {{ brokerError }}
      </span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de resultados">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <PolizasFilters
      :catalogs="catalogs"
      :criteria="filterCriteria"
      :loading="catalogsLoading"
      :error="catalogsError"
      :search-disabled="contextBlocked"
      :blocked-message="runtimeError"
      @search="executeSearch"
      @clear="clearFilters"
    />
    <PolizaCrudPanel
      :mode="crudMode"
      :open="crudOpen"
      :catalogs="catalogs"
      :item="crudItem"
      :saving="crudSaving"
      :error="crudError"
      @close="closeCrudPanel"
      @submit="submitCrud"
    />
    <section
      v-if="activeFilterChips.length > 0"
      class="active-filter-summary"
      aria-label="Filtros activos de polizas"
    >
      <span class="active-filter-summary-title">Filtros activos</span>
      <span v-for="filter in activeFilterChips" :key="filter.key" class="active-filter-chip">
        <strong>{{ filter.label }}</strong>
        {{ filter.value }}
      </span>
    </section>
    <PolizasTable
      :items="items"
      :total="total"
      :loading="loading"
      :error="error"
      :page="pagination.page"
      :page-size="pagination.pageSize"
      :detail-query="detailQuery"
      :can-open-detail="canOpenPolizaDetail"
      :detail-unavailable-message="detailUnavailableMessage"
      :can-update-mvp="canUpdatePoliza"
      :can-delete-mvp="canDeletePoliza"
      :write-busy-id="writeBusyId"
      @page-change="changePage"
      @page-size-change="changePageSize"
      @edit="openEditPanel"
      @delete="deleteMvpPoliza"
      @retry="refresh"
    />
  </AppShell>
</template>
