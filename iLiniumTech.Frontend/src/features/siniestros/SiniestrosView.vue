<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import { siniestroEstados, siniestroPrioridades } from './siniestrosTypes'
import { useSiniestrosFixture } from './useSiniestrosFixture'

const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar siniestros', icon: 'pi pi-search', active: true },
  { label: 'Ver detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]
const blockedActionsDescription =
  'Acciones de siniestros bloqueadas en el MVP read-only hasta SDD, contrato API, permisos y UAT.'

const {
  canGoNext,
  canGoPrevious,
  changePage,
  changePageSize,
  clearFilters,
  draftFilters,
  firstVisible,
  lastVisible,
  pageSizeOptions,
  pagedItems,
  pagination,
  resultLabel,
  searchSiniestros,
  tableCaption,
  total,
  totalPages,
} = useSiniestrosFixture()

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function changePageSizeFromEvent(event: Event) {
  changePageSize(Number((event.target as HTMLSelectElement).value))
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="siniestros-content"
    section-title="Siniestros"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="siniestros-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Siniestros</h1>
      </div>

      <p id="siniestros-blocked-actions" class="sr-only">
        {{ blockedActionsDescription }}
      </p>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            aria-describedby="siniestros-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de siniestros">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Detalle y exportacion pendientes</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de siniestros">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros principales de siniestros">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchSiniestros">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" aria-describedby="siniestros-blocked-actions" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar busqueda
          </button>
          <button type="button" aria-describedby="siniestros-blocked-actions" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Busqueda read-only</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label
              class="filter-field"
              for="siniestros-filter-referencia"
              style="grid-column: span 3"
            >
              <span>Referencia o cliente</span>
              <span class="field-control">
                <input
                  id="siniestros-filter-referencia"
                  v-model="draftFilters.referencia"
                  type="search"
                  aria-label="Referencia o cliente"
                />
                <button
                  type="button"
                  aria-label="Opciones de referencia"
                  aria-describedby="siniestros-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="siniestros-filter-poliza" style="grid-column: span 3">
              <span>Poliza</span>
              <span class="field-control">
                <input
                  id="siniestros-filter-poliza"
                  v-model="draftFilters.poliza"
                  type="search"
                  aria-label="Poliza"
                />
                <button
                  type="button"
                  aria-label="Opciones de poliza"
                  aria-describedby="siniestros-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="siniestros-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="siniestros-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option v-for="estado in siniestroEstados" :key="estado" :value="estado">
                    {{ estado }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de estado"
                  aria-describedby="siniestros-blocked-actions"
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
              for="siniestros-filter-prioridad"
              style="grid-column: span 2"
            >
              <span>Prioridad</span>
              <span class="field-control">
                <select
                  id="siniestros-filter-prioridad"
                  v-model="draftFilters.prioridad"
                  aria-label="Prioridad"
                >
                  <option value=""></option>
                  <option
                    v-for="prioridad in siniestroPrioridades"
                    :key="prioridad"
                    :value="prioridad"
                  >
                    {{ prioridad }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de prioridad"
                  aria-describedby="siniestros-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="siniestros-filter-fecha" style="grid-column: span 2">
              <span>Fecha siniestro desde</span>
              <span class="field-control">
                <input
                  id="siniestros-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha siniestro desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de fecha"
                  aria-describedby="siniestros-blocked-actions"
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
              <span>Intervinientes, importes y observaciones</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Campos sensibles bloqueados"
                  aria-describedby="siniestros-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Campos sensibles bloqueados"
                  aria-describedby="siniestros-blocked-actions"
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

    <section class="results-summary" aria-labelledby="siniestros-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="siniestros-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay siniestros fixture para los filtros actuales.</p>
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
              <th scope="col">Cliente</th>
              <th scope="col">Compania</th>
              <th scope="col">Situacion</th>
              <th scope="col">Estado</th>
              <th scope="col">Prioridad</th>
              <th scope="col">Fecha siniestro</th>
              <th scope="col">Fecha parte</th>
              <th scope="col">Tramitador</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para siniestro ${item.referencia}`"
                  aria-describedby="siniestros-blocked-actions"
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
              <td>{{ item.cliente }}</td>
              <td>{{ item.compania }}</td>
              <td>{{ item.situacion }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.prioridad }}</td>
              <td>{{ formatDate(item.fechaSiniestro) }}</td>
              <td>{{ formatDate(item.fechaParte) }}</td>
              <td>{{ item.tramitador }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de siniestros">
        <div class="page-size-control">
          <label for="siniestros-page-size">Filas</label>
          <select
            id="siniestros-page-size"
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
