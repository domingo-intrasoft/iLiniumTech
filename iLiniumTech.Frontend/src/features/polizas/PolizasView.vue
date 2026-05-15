<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import PolizasFilters from './PolizasFilters.vue'
import PolizasTable from './PolizasTable.vue'
import {
  moduleActions,
  statusActions,
  topBadges,
  type PolizasSearchCriteria,
} from './polizasConstants'
import { usePolizas } from './usePolizas'

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
  setPage,
  setPageSize,
} = usePolizas()

const router = useRouter()
const { userLabel, logout } = useAuthSession()
const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'
const dataOriginLabel = computed(() => (isBackendMode ? 'API polizas' : 'Fixture local'))

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
    Boolean(session.value?.polizasExecutionContextRequired && !session.value.brokerId),
)

async function executeSearch(criteria: PolizasSearchCriteria) {
  filters.numero = criteria.numero
  filters.cliente = criteria.cliente
  filters.estado = criteria.estado
  filters.compania = criteria.compania
  filters.ramo = criteria.ramo
  filters.fechaEfectoDesde = criteria.fechaEfectoDesde
  filters.fechaEfectoHasta = criteria.fechaEfectoHasta
  pagination.page = 1
  await refresh()
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
  await refresh()
}

async function signOut() {
  logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="polizas-content"
    section-title="Polizas"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
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
      @page-change="setPage"
      @page-size-change="setPageSize"
      @retry="refresh"
    />
  </AppShell>
</template>
