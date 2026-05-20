<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import { blockedActionsDescription, moduleActions } from './fixtures'
import { useSuplementos } from './useSuplementos'

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()
const {
  canGoNext,
  canGoPrevious,
  changePage,
  changePageSize,
  clearFilters,
  draftFilters,
  error,
  firstVisible,
  formatDate,
  isBackendMode,
  lastVisible,
  loading,
  pageSizeOptions,
  pagedItems,
  pagination,
  resultLabel,
  searchSuplementos,
  situacionOptions,
  tableCaption,
  tipoOptions,
  total,
  totalPages,
} = useSuplementos()

const sessionLabel = computed(() => {
  if (session.value?.currentBrokerId) {
    return `Broker ${session.value.currentBrokerId}`
  }

  return isBackendMode ? 'Broker pendiente' : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => isBackendMode && !session.value?.currentBrokerId)
const topBadges = computed(() =>
  isBackendMode ? ['Read-only', 'BBDD local'] : ['Read-only', 'Fixture'],
)

async function changePageSizeFromEvent(event: Event) {
  await changePageSize(Number((event.target as HTMLSelectElement).value))
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="suplementos-content"
    section-title="Suplementos"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="suplementos-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Suplementos</h1>
      </div>

      <p id="suplementos-blocked-actions" class="sr-only" v-text="blockedActionsDescription"></p>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            aria-describedby="suplementos-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de suplementos">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span>
        <i class="pi pi-database" aria-hidden="true"></i>
        {{ isBackendMode ? 'BBDD local/API' : 'Fixture local sin API' }}
      </span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin workflows ni escrituras</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de suplementos">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de suplementos">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button
            type="button"
            class="primary-action"
            :disabled="loading"
            @click="searchSuplementos"
          >
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" aria-describedby="suplementos-blocked-actions" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar busqueda
          </button>
          <button type="button" aria-describedby="suplementos-blocked-actions" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Busqueda local</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="suplementos-filter-texto" style="grid-column: span 3">
              <span>Referencia</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Referencia"
                />
                <button
                  type="button"
                  aria-label="Opciones de referencia"
                  aria-describedby="suplementos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-poliza" style="grid-column: span 3">
              <span>Poliza</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-poliza"
                  v-model="draftFilters.poliza"
                  type="search"
                  aria-label="Poliza"
                />
                <button
                  type="button"
                  aria-label="Opciones de poliza"
                  aria-describedby="suplementos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-tipo" style="grid-column: span 2">
              <span>Tipo</span>
              <span class="field-control">
                <select id="suplementos-filter-tipo" v-model="draftFilters.tipo" aria-label="Tipo">
                  <option value=""></option>
                  <option v-for="option in tipoOptions" :key="option" :value="option">
                    {{ option }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de tipo"
                  aria-describedby="suplementos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label
              class="filter-field"
              for="suplementos-filter-situacion"
              style="grid-column: span 2"
            >
              <span>Situacion</span>
              <span class="field-control">
                <select
                  id="suplementos-filter-situacion"
                  v-model="draftFilters.situacion"
                  aria-label="Situacion"
                >
                  <option value=""></option>
                  <option v-for="option in situacionOptions" :key="option" :value="option">
                    {{ option }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de situacion"
                  aria-describedby="suplementos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-fecha" style="grid-column: span 2">
              <span>Fecha efecto desde</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha efecto desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de fecha"
                  aria-describedby="suplementos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 4"
              title="Pendiente de SDD/API"
            >
              <span>Campos restringidos</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Campos restringidos bloqueados"
                  aria-describedby="suplementos-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Campos restringidos bloqueados"
                  aria-describedby="suplementos-blocked-actions"
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

    <section class="results-summary" aria-labelledby="suplementos-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="suplementos-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="error" class="state state-box error" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        <div>
          <strong>Error de carga</strong>
          <p>{{ error }}</p>
        </div>
      </div>

      <div v-else-if="loading" class="state state-box" role="status">
        <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
        <div>
          <strong>Cargando</strong>
          <p>Consultando suplementos.</p>
        </div>
      </div>

      <div v-else-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>
            {{
              isBackendMode
                ? 'No hay suplementos en la BBDD local para los filtros actuales.'
                : 'No hay suplementos fixture para los filtros actuales.'
            }}
          </p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Poliza</th>
              <th scope="col">Tipo</th>
              <th scope="col">Situacion</th>
              <th scope="col">Fecha efecto</th>
              <th scope="col">Concepto</th>
              <th scope="col">Resumen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para suplemento ${item.referencia}`"
                  aria-describedby="suplementos-blocked-actions"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.poliza }}</td>
              <td>{{ item.tipo }}</td>
              <td>{{ item.situacion }}</td>
              <td>{{ formatDate(item.fechaEfecto) }}</td>
              <td>{{ item.concepto }}</td>
              <td>{{ item.resumen }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de suplementos">
        <div class="page-size-control">
          <label for="suplementos-page-size">Filas</label>
          <select
            id="suplementos-page-size"
            :value="pagination.pageSize"
            @change="changePageSizeFromEvent"
          >
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
