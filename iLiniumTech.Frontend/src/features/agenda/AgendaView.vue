<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import { blockedActionsDescription, moduleActions, pageSizeOptions, topBadges } from './fixtures'
import { useAgenda } from './useAgenda'

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()
const {
  canGoNext,
  canGoPrevious,
  changePage,
  changePageSize,
  clearFilters,
  createMvpEvent,
  deleteMvpEvent,
  draftFilters,
  error,
  firstVisible,
  formatDate,
  isBackendMode,
  lastVisible,
  loading,
  pagedItems,
  pagination,
  resultLabel,
  searchAgenda,
  tableCaption,
  total,
  totalPages,
  updateMvpEvent,
} = useAgenda()

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="agenda-content"
    section-title="Agenda"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <p id="agenda-blocked-actions" class="sr-only" v-text="blockedActionsDescription"></p>

    <div id="agenda-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Agenda</h1>
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
            aria-describedby="agenda-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de agenda">
      <span
        ><i :class="isBackendMode ? 'pi pi-unlock' : 'pi pi-lock'" aria-hidden="true"></i>
        {{ isBackendMode ? 'CRUD local' : 'Solo lectura' }}</span
      >
      <span
        ><i class="pi pi-database" aria-hidden="true"></i>
        {{ isBackendMode ? 'API/BBDD local' : 'Fixture local sin API' }}</span
      >
      <span><i class="pi pi-calendar" aria-hidden="true"></i> Sin calendario dinamico</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> PII/asuntos sensibles bloqueados</span>
      <span
        ><i class="pi pi-ban" aria-hidden="true"></i>
        {{
          isBackendMode
            ? 'Detalle, exportar y workflows bloqueados'
            : 'Crear, reprogramar y exportar bloqueados'
        }}</span
      >
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Listado de agenda">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de agenda">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchAgenda">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button
            type="button"
            :aria-describedby="isBackendMode ? undefined : 'agenda-blocked-actions'"
            :disabled="!isBackendMode || loading"
            @click="createMvpEvent"
          >
            <i class="pi pi-plus" aria-hidden="true"></i>
            Crear
          </button>
          <button type="button" aria-describedby="agenda-blocked-actions" disabled>
            <i class="pi pi-calendar-times" aria-hidden="true"></i>
            Reprogramar
          </button>
          <button type="button" aria-describedby="agenda-blocked-actions" disabled>
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
            <label class="filter-field" for="agenda-filter-texto" style="grid-column: span 3">
              <span>Texto o referencia</span>
              <span class="field-control">
                <input
                  id="agenda-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto o referencia"
                />
                <button
                  type="button"
                  aria-label="Opciones de texto o referencia"
                  aria-describedby="agenda-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="agenda-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select id="agenda-filter-estado" v-model="draftFilters.estado" aria-label="Estado">
                  <option value=""></option>
                  <option>Pendiente</option>
                  <option>Programado</option>
                  <option>Cerrado</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de estado"
                  aria-describedby="agenda-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="agenda-filter-prioridad" style="grid-column: span 2">
              <span>Prioridad</span>
              <span class="field-control">
                <select
                  id="agenda-filter-prioridad"
                  v-model="draftFilters.prioridad"
                  aria-label="Prioridad"
                >
                  <option value=""></option>
                  <option>Alta</option>
                  <option>Media</option>
                  <option>Baja</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de prioridad"
                  aria-describedby="agenda-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="agenda-filter-fecha" style="grid-column: span 2">
              <span>Fecha desde</span>
              <span class="field-control">
                <input
                  id="agenda-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de fecha"
                  aria-describedby="agenda-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 5"
              title="Pendiente de SDD/API"
            >
              <span>Descripcion, participantes, calendario dinamico y workflows</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: PII/asuntos sensibles, sin API ni escrituras"
                  disabled
                  aria-label="Campos y acciones sensibles bloqueados"
                  aria-describedby="agenda-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Campos y acciones sensibles bloqueados"
                  aria-describedby="agenda-blocked-actions"
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

    <section class="results-summary" aria-labelledby="agenda-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="agenda-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="error" class="state state-box empty" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        <div>
          <strong>No disponible</strong>
          <p>{{ error }}</p>
        </div>
      </div>

      <div v-else-if="loading" class="state state-box empty" role="status">
        <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
        <div>
          <strong>Cargando</strong>
          <p>Consultando Agenda.</p>
        </div>
      </div>

      <div v-else-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay eventos fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Asunto sanitizado</th>
              <th scope="col">Estado</th>
              <th scope="col">Prioridad</th>
              <th scope="col">Inicio</th>
              <th scope="col">Fin</th>
              <th scope="col">Objeto relacionado</th>
              <th scope="col">Origen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <div class="row-actions">
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Editar agenda ${item.referencia}`"
                    :aria-describedby="isBackendMode ? undefined : 'agenda-blocked-actions'"
                    :title="
                      isBackendMode ? 'Actualizar evento MVP' : 'Edicion pendiente de backend'
                    "
                    :disabled="!isBackendMode || loading"
                    @click="updateMvpEvent(item)"
                  >
                    <i class="pi pi-pencil" aria-hidden="true"></i>
                  </button>
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Eliminar agenda ${item.referencia}`"
                    :aria-describedby="isBackendMode ? undefined : 'agenda-blocked-actions'"
                    :title="isBackendMode ? 'Eliminar evento MVP' : 'Baja pendiente de backend'"
                    :disabled="!isBackendMode || loading"
                    @click="deleteMvpEvent(item)"
                  >
                    <i class="pi pi-trash" aria-hidden="true"></i>
                  </button>
                </div>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.asunto }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.prioridad }}</td>
              <td>{{ formatDate(item.fechaInicio) }} {{ item.horaInicio }}</td>
              <td>{{ formatDate(item.fechaFin) }} {{ item.horaFin }}</td>
              <td>{{ item.objetoRelacionado }}</td>
              <td>{{ item.origen }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de agenda">
        <div class="page-size-control">
          <label for="agenda-page-size">Filas</label>
          <select id="agenda-page-size" :value="pagination.pageSize" @change="changePageSize">
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
