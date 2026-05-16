<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute, useRouter, type LocationQuery, type LocationQueryRaw } from 'vue-router'

import { clearAuthSession, switchAuthBroker, useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'
import { toPolizasUserError } from '@/services/apiErrors'

import PolizasFilters from './PolizasFilters.vue'
import PolizasTable from './PolizasTable.vue'
import {
  moduleActions,
  statusActions,
  topBadges,
  type PolizasSearchCriteria,
} from './polizasConstants'
import { usePolizas } from './usePolizas'
import type { PolizasQueryFilters } from './polizasTypes'

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
const POLIZAS_DETAIL_PERMISSION = 'polizas.detail'
const brokerChanging = ref(false)
const brokerError = ref<string | null>(null)

interface PolizasRouteState {
  filters: PolizasQueryFilters
  page: number
  pageSize: number
}

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
    show-sign-out
    @broker-change="changeBroker"
    @sign-out="signOut"
  >
    <div id="polizas-content" class="polizas-toolbar">
      <h1 class="sr-only">Polizas</h1>
      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
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
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>

      <div class="context-buttons">
        <button type="button" disabled>Polizas de flota</button>
        <button type="button" disabled>Polizas colectivas</button>
        <button type="button" disabled>Polizas Externas</button>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de polizas">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> {{ dataOriginLabel }}</span>
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
    <PolizasTable
      :items="items"
      :total="total"
      :loading="loading"
      :error="error"
      :page="pagination.page"
      :page-size="pagination.pageSize"
      :detail-query="detailQuery"
      :can-open-detail="canOpenPolizaDetail"
      @page-change="changePage"
      @page-size-change="changePageSize"
      @retry="refresh"
    />
  </AppShell>
</template>
