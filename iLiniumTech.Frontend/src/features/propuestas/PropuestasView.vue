<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import {
  propuestasBlockedActionsDescription,
  propuestasEstados,
  propuestasModuleActions,
  propuestasRamos,
  propuestasTopBadges,
} from './fixtures'
import { formatPropuestaDate, usePropuestasFixture } from './usePropuestasFixture'

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()
const {
  canGoNext,
  canGoPrevious,
  changePage,
  changePageSize: changeFixturePageSize,
  clearFilters,
  draftFilters,
  firstVisible,
  lastVisible,
  pageSizeOptions,
  pagedItems,
  pagination,
  resultLabel,
  searchPropuestas,
  tableCaption,
  total,
  totalPages,
} = usePropuestasFixture()

const estados = propuestasEstados
const ramos = propuestasRamos
const topBadges = propuestasTopBadges
const moduleActions = propuestasModuleActions
const blockedActionsDescription = propuestasBlockedActionsDescription

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)

function formatDate(value: string) {
  return formatPropuestaDate(value)
}

function changePageSize(event: Event) {
  changeFixturePageSize(Number((event.target as HTMLSelectElement).value))
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="propuestas-content"
    section-title="Propuestas"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <p id="propuestas-blocked-actions" class="sr-only" v-text="blockedActionsDescription"></p>

    <div id="propuestas-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Propuestas</h1>
      </div>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            aria-describedby="propuestas-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de propuestas">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos minimizados y sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin emision ni conversion a poliza</span>
      <span><i class="pi pi-file-edit" aria-hidden="true"></i> Origen y SDD pendientes</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de propuestas">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de propuestas">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchPropuestas">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" aria-describedby="propuestas-blocked-actions" disabled>
            <i class="pi pi-plus" aria-hidden="true"></i>
            Crear
          </button>
          <button type="button" aria-describedby="propuestas-blocked-actions" disabled>
            <i class="pi pi-send" aria-hidden="true"></i>
            Convertir
          </button>
          <button type="button" aria-describedby="propuestas-blocked-actions" disabled>
            <i class="pi pi-folder" aria-hidden="true"></i>
            Documentos
          </button>
          <button type="button" aria-describedby="propuestas-blocked-actions" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Busqueda local read-only</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label
              class="filter-field"
              for="propuestas-filter-referencia"
              style="grid-column: span 3"
            >
              <span>Referencia o solicitante</span>
              <span class="field-control">
                <input
                  id="propuestas-filter-referencia"
                  v-model="draftFilters.referencia"
                  type="search"
                  aria-label="Referencia o solicitante"
                />
                <button
                  type="button"
                  aria-label="Opciones de referencia"
                  aria-describedby="propuestas-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="propuestas-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="propuestas-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option v-for="estado in estados" :key="estado">{{ estado }}</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de estado"
                  aria-describedby="propuestas-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="propuestas-filter-ramo" style="grid-column: span 2">
              <span>Ramo / tipo</span>
              <span class="field-control">
                <select id="propuestas-filter-ramo" v-model="draftFilters.ramo" aria-label="Ramo">
                  <option value=""></option>
                  <option v-for="ramo in ramos" :key="ramo">{{ ramo }}</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de ramo"
                  aria-describedby="propuestas-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="propuestas-filter-fecha" style="grid-column: span 2">
              <span>Fecha desde</span>
              <span class="field-control">
                <input
                  id="propuestas-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de fecha"
                  aria-describedby="propuestas-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 7"
              title="Pendiente de SDD/API"
            >
              <span>Emision, conversion, documentos, detalle y origen real</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only: requiere SDD, API y UAT"
                  disabled
                  aria-label="Funciones de propuesta bloqueadas"
                  aria-describedby="propuestas-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Funciones de propuesta bloqueadas"
                  aria-describedby="propuestas-blocked-actions"
                  disabled
                >
                  <i class="pi pi-lock" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>
        </section>
      </div>
    </section>

    <section class="results-summary" aria-labelledby="propuestas-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="propuestas-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay propuestas fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Estado</th>
              <th scope="col">Ramo / tipo</th>
              <th scope="col">Fecha alta</th>
              <th scope="col">Vigencia</th>
              <th scope="col">Solicitante</th>
              <th scope="col">Canal</th>
              <th scope="col">Resultado</th>
              <th scope="col">Importe</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para propuesta ${item.referencia}`"
                  aria-describedby="propuestas-blocked-actions"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.estado }}</td>
              <td>{{ item.ramo }}</td>
              <td>{{ formatDate(item.fechaAlta) }}</td>
              <td>{{ item.vigencia }}</td>
              <td>{{ item.solicitante }}</td>
              <td>{{ item.canal }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.importeDemo }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de propuestas">
        <div class="page-size-control">
          <label for="propuestas-page-size">Filas</label>
          <select id="propuestas-page-size" :value="pagination.pageSize" @change="changePageSize">
            <option v-for="option in pageSizeOptions" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
        </div>

        <div class="page-controls">
          <button type="button" :disabled="!canGoPrevious" @click="changePage(pagination.page - 1)">
            <i class="pi pi-chevron-left" aria-hidden="true"></i>
            Anterior
          </button>
          <span>Pagina {{ pagination.page }} de {{ totalPages }}</span>
          <button type="button" :disabled="!canGoNext" @click="changePage(pagination.page + 1)">
            Siguiente
            <i class="pi pi-chevron-right" aria-hidden="true"></i>
          </button>
        </div>
      </footer>
    </section>
  </AppShell>
</template>
