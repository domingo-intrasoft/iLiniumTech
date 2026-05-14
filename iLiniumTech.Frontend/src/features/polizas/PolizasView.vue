<script setup lang="ts">
import { computed } from 'vue'

import PolizasFilters from './PolizasFilters.vue'
import PolizasTable from './PolizasTable.vue'
import {
  moduleActions,
  sideItems,
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
  items,
  total,
  loading,
  error,
  filters,
  pagination,
  refresh,
  setPage,
  setPageSize,
} = usePolizas()

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
</script>

<template>
  <main class="ilinium-shell">
    <aside class="il-sidebar" aria-label="Menu principal">
      <div class="broker-logo"><span></span>AUXFISE</div>
      <nav class="side-nav">
        <a
          v-for="item in sideItems"
          :key="item.label"
          href="#"
          :class="{ active: item.active }"
          :aria-current="item.active ? 'page' : undefined"
        >
          <i :class="item.icon"></i>
          <span>{{ item.label }}</span>
        </a>
      </nav>
    </aside>

    <section class="workspace">
      <header class="workspace-topbar">
        <div class="breadcrumb-line">
          <button class="icon-button" type="button" aria-label="Menu">
            <i class="pi pi-bars"></i>
          </button>
          <strong>Inicio</strong>
          <span>/</span>
          <strong>Polizas</strong>
          <span>/</span>
        </div>

        <span class="environment-badge" :class="{ warning: sessionNeedsAttention }">
          {{ sessionLabel }}
        </span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <span v-for="badge in topBadges" :key="badge" class="round-badge">{{ badge }}</span>
          <span class="user-name">Domingo () (-1)</span>
          <button class="icon-button ghost" type="button" aria-label="Notificaciones">
            <i class="pi pi-bell"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Configuracion">
            <i class="pi pi-cog"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Salir">
            <i class="pi pi-sign-out"></i>
          </button>
        </div>
      </header>

      <div class="polizas-toolbar">
        <div class="toolbar-groups">
          <div class="action-group">
            <button
              v-for="action in moduleActions"
              :key="action.label"
              class="square-action"
              :class="{ active: action.active }"
              type="button"
              :aria-label="action.label"
            >
              <i :class="action.icon"></i>
            </button>
          </div>
          <div class="action-group compact">
            <button
              v-for="action in statusActions"
              :key="action.label"
              class="square-action dark"
              type="button"
              :aria-label="action.label"
            >
              <i :class="action.icon"></i>
            </button>
          </div>
        </div>

        <div class="context-buttons">
          <button type="button">Polizas de flota</button>
          <button type="button">Polizas colectivas</button>
          <button type="button">Polizas Externas</button>
        </div>
      </div>

      <div class="tab-strip">
        <button type="button" class="tab active"><i class="pi pi-search"></i></button>
        <button type="button" class="tab"><i class="pi pi-table"></i></button>
        <strong>({{ total }})</strong>
      </div>

      <PolizasFilters :catalogs="catalogs" @search="executeSearch" @clear="clearFilters" />
      <PolizasTable
        :items="items"
        :total="total"
        :loading="loading"
        :error="error"
        :page="pagination.page"
        :page-size="pagination.pageSize"
        @page-change="setPage"
        @page-size-change="setPageSize"
      />
    </section>
  </main>
</template>
