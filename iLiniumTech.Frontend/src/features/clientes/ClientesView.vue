<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

import {
  clientesBlockedActionsDescription,
  clientesModuleActions,
  clientesPageSizeOptions,
  clientesTopBadges,
} from './fixtures'
import type { ClienteListItem } from './types'
import { useClientes } from './useClientes'

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()
const {
  draftFilters,
  pagination,
  total,
  totalPages,
  pagedItems,
  firstVisible,
  lastVisible,
  resultLabel,
  tableCaption,
  canGoPrevious,
  canGoNext,
  error,
  estadoOptions,
  isBackendMode,
  loading,
  searchClientes,
  clearFilters,
  changePage,
  changePageSize,
  createMvpCliente,
  updateMvpCliente,
  deleteMvpCliente,
  formatDate,
  isMvpOwned,
  segmentoOptions,
} = useClientes()

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)
const topBadges = computed(() =>
  isBackendMode ? ['CRUD local', 'BBDD local', 'PII minimizada'] : clientesTopBadges,
)
const permissionSet = computed(() => new Set(session.value?.permissions ?? []))
const canCreateClientes = computed(
  () =>
    isBackendMode &&
    permissionSet.value.has('clientes.read') &&
    permissionSet.value.has('clientes.create') &&
    !loading.value,
)
const canUpdateClientes = computed(
  () =>
    isBackendMode &&
    permissionSet.value.has('clientes.read') &&
    permissionSet.value.has('clientes.update') &&
    !loading.value,
)
const canDeleteClientes = computed(
  () =>
    isBackendMode &&
    permissionSet.value.has('clientes.read') &&
    permissionSet.value.has('clientes.delete') &&
    !loading.value,
)

function canUpdateCliente(item: ClienteListItem) {
  return canUpdateClientes.value && isMvpOwned(item)
}

function canDeleteCliente(item: ClienteListItem) {
  return canDeleteClientes.value && isMvpOwned(item)
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="clientes-content"
    section-title="Clientes"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <p id="clientes-blocked-actions" class="sr-only" v-text="clientesBlockedActionsDescription"></p>

    <div id="clientes-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">{{ isBackendMode ? 'MVP CRUD local' : 'MVP read-only' }}</p>
        <h1>Clientes</h1>
      </div>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in clientesModuleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            aria-describedby="clientes-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de clientes">
      <span
        ><i :class="isBackendMode ? 'pi pi-unlock' : 'pi pi-lock'" aria-hidden="true"></i>
        {{ isBackendMode ? 'CRUD local guardado' : 'Solo lectura' }}</span
      >
      <span>
        <i class="pi pi-database" aria-hidden="true"></i>
        {{ isBackendMode ? 'BBDD local/API' : 'Fixture local sin API' }}
      </span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos minimizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> PII bloqueada</span>
      <span><i class="pi pi-sitemap" aria-hidden="true"></i> Tabs relacionadas pendientes</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de clientes">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de clientes">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchClientes">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button
            type="button"
            :aria-describedby="canCreateClientes ? undefined : 'clientes-blocked-actions'"
            :disabled="!canCreateClientes"
            @click="createMvpCliente"
          >
            <i class="pi pi-plus" aria-hidden="true"></i>
            Crear
          </button>
          <button type="button" aria-describedby="clientes-blocked-actions" disabled>
            <i class="pi pi-id-card" aria-hidden="true"></i>
            Abrir ficha
          </button>
          <button type="button" aria-describedby="clientes-blocked-actions" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
          <button type="button" aria-describedby="clientes-blocked-actions" disabled>
            <i class="pi pi-sitemap" aria-hidden="true"></i>
            Desglose
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Busqueda minimizada</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="clientes-filter-texto" style="grid-column: span 3">
              <span>Referencia o alias anonimo</span>
              <span class="field-control">
                <input
                  id="clientes-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Referencia o alias anonimo"
                />
                <button
                  type="button"
                  aria-label="Opciones de referencia"
                  aria-describedby="clientes-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="clientes-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="clientes-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option v-for="estado in estadoOptions" :key="estado">
                    {{ estado }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de estado"
                  aria-describedby="clientes-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="clientes-filter-segmento" style="grid-column: span 2">
              <span>Segmento</span>
              <span class="field-control">
                <select
                  id="clientes-filter-segmento"
                  v-model="draftFilters.segmento"
                  aria-label="Segmento"
                >
                  <option value=""></option>
                  <option v-for="segmento in segmentoOptions" :key="segmento">
                    {{ segmento }}
                  </option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de segmento"
                  aria-describedby="clientes-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="clientes-filter-fecha" style="grid-column: span 2">
              <span>Fecha alta desde</span>
              <span class="field-control">
                <input
                  id="clientes-filter-fecha"
                  v-model="draftFilters.fechaAltaDesde"
                  type="date"
                  aria-label="Fecha alta desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de fecha alta"
                  aria-describedby="clientes-blocked-actions"
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
              style="grid-column: span 8"
              title="Pendiente de SDD/API y decision PII"
            >
              <span>Documento, contacto, direccion, bancarios y anotaciones</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: no se captura ni muestra PII en este MVP"
                  disabled
                  aria-label="Campos PII bloqueados"
                  aria-describedby="clientes-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Campos PII bloqueados"
                  aria-describedby="clientes-blocked-actions"
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

    <section class="results-summary" aria-labelledby="clientes-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="clientes-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="loading" class="state state-box empty" role="status">
        <i class="pi pi-spin pi-spinner" aria-hidden="true"></i>
        <div>
          <strong>Cargando</strong>
          <p>Consultando clientes.</p>
        </div>
      </div>

      <div v-else-if="error" class="state state-box empty" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        <div>
          <strong>Error</strong>
          <p>{{ error }}</p>
        </div>
      </div>

      <div v-else-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>
            {{
              isBackendMode
                ? 'No hay clientes para los filtros actuales.'
                : 'No hay clientes fixture para los filtros actuales.'
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
              <th scope="col">Alias anonimo</th>
              <th scope="col">Estado</th>
              <th scope="col">Segmento</th>
              <th scope="col">Fecha alta</th>
              <th scope="col">Resultado</th>
              <th scope="col">Datos</th>
              <th scope="col">Relacionadas</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <div class="row-actions">
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Editar cliente ${item.referencia}`"
                    :aria-describedby="
                      canUpdateCliente(item) ? undefined : 'clientes-blocked-actions'
                    "
                    :title="
                      canUpdateCliente(item)
                        ? 'Actualizar cliente MVP'
                        : 'Solo clientes creados por iLiniumTech MVP con permiso clientes.update'
                    "
                    :disabled="!canUpdateCliente(item)"
                    @click="updateMvpCliente(item)"
                  >
                    <i class="pi pi-pencil" aria-hidden="true"></i>
                  </button>
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Eliminar cliente ${item.referencia}`"
                    :aria-describedby="
                      canDeleteCliente(item) ? undefined : 'clientes-blocked-actions'
                    "
                    :title="
                      canDeleteCliente(item)
                        ? 'Baja logica de cliente MVP'
                        : 'Solo clientes creados por iLiniumTech MVP con permiso clientes.delete'
                    "
                    :disabled="!canDeleteCliente(item)"
                    @click="deleteMvpCliente(item)"
                  >
                    <i class="pi pi-trash" aria-hidden="true"></i>
                  </button>
                </div>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.alias }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.segmento }}</td>
              <td>{{ formatDate(item.fechaAlta) }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.datos }}</td>
              <td>{{ item.relacionadas }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de clientes">
        <div class="page-size-control">
          <label for="clientes-page-size">Filas</label>
          <select id="clientes-page-size" :value="pagination.pageSize" @change="changePageSize">
            <option v-for="option in clientesPageSizeOptions" :key="option" :value="option">
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
