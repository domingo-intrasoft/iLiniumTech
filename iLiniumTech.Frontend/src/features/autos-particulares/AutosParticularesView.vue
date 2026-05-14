<script setup lang="ts">
import { computed } from 'vue'

import AutosParticularesFilters from './AutosParticularesFilters.vue'
import AutosParticularesTable from './AutosParticularesTable.vue'
import {
  autosModuleActions,
  autosParticularesSideItems,
  type AutosSearchCriteria,
} from './autosParticularesConstants'
import { useAutosParticulares } from './useAutosParticulares'

const {
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
} = useAutosParticulares()

const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'
const dataOriginLabel = computed(() =>
  isBackendMode ? 'API autos particulares' : 'Fixture local autos',
)

const divisionScopeLabel = computed(() =>
  scope.value.divisionPendienteUat
    ? `${scope.value.divisionObjetivo} pendiente UAT`
    : `Division ${scope.value.divisionObjetivo}`,
)

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

async function executeSearch(criteria: AutosSearchCriteria) {
  filters.numero = criteria.numero
  filters.cliente = criteria.cliente
  filters.estado = criteria.estado
  filters.compania = criteria.compania
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
  filters.fechaEfectoDesde = ''
  filters.fechaEfectoHasta = ''
  pagination.page = 1
  await refresh()
}
</script>

<template>
  <main class="ilinium-shell">
    <a class="skip-link" href="#autos-content">Saltar al contenido</a>
    <aside class="il-sidebar" aria-label="Menu principal">
      <div class="broker-logo"><span></span>AUXFISE</div>
      <nav class="side-nav">
        <a
          v-for="item in autosParticularesSideItems"
          :key="item.label"
          href="#"
          :class="{ active: item.active }"
          :aria-current="item.active ? 'page' : undefined"
        >
          <i :class="item.icon" aria-hidden="true"></i>
          <span>{{ item.label }}</span>
        </a>
      </nav>
    </aside>

    <section class="workspace">
      <header class="workspace-topbar">
        <div class="breadcrumb-line">
          <button class="icon-button" type="button" aria-label="Menu">
            <i class="pi pi-bars" aria-hidden="true"></i>
          </button>
          <strong>Inicio</strong>
          <span>/</span>
          <strong>Autos Particulares</strong>
          <span>/</span>
        </div>

        <span class="environment-badge" :class="{ warning: sessionNeedsAttention }">
          {{ sessionLabel }}
        </span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <span class="user-name">MVP Autos Particulares</span>
          <button class="icon-button ghost" type="button" aria-label="Notificaciones">
            <i class="pi pi-bell" aria-hidden="true"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Configuracion">
            <i class="pi pi-cog" aria-hidden="true"></i>
          </button>
        </div>
      </header>

      <div id="autos-content" class="polizas-toolbar autos-toolbar">
        <div>
          <p class="section-kicker">MVP read-only</p>
          <h1>Autos Particulares</h1>
        </div>

        <div class="toolbar-groups">
          <div class="action-group">
            <button
              v-for="action in autosModuleActions"
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
        </div>
      </div>

      <section class="runtime-strip" aria-label="Contexto de autos particulares">
        <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
        <span><i class="pi pi-database" aria-hidden="true"></i> {{ dataOriginLabel }}</span>
        <span><i class="pi pi-car" aria-hidden="true"></i> Ramo {{ scope.ramo }}</span>
        <span><i class="pi pi-shield" aria-hidden="true"></i> {{ divisionScopeLabel }}</span>
        <span v-if="contextBlocked" class="warning" role="alert">
          <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
          {{ runtimeError ?? 'Broker requerido' }}
        </span>
      </section>

      <div class="tab-strip">
        <button type="button" class="tab active" aria-label="Filtros">
          <i class="pi pi-search" aria-hidden="true"></i>
        </button>
        <button type="button" class="tab" aria-label="Tabla de autos particulares">
          <i class="pi pi-table" aria-hidden="true"></i>
        </button>
        <strong>({{ total }})</strong>
      </div>

      <AutosParticularesFilters
        :catalogs="catalogs"
        :loading="catalogsLoading"
        :error="catalogsError"
        @search="executeSearch"
        @clear="clearFilters"
      />
      <AutosParticularesTable
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
    </section>
  </main>
</template>
