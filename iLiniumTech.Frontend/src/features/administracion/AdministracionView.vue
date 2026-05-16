<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type AdminEstado = 'Pendiente SDD' | 'Bloqueado' | 'Read-only'
type AdminRiesgo = 'Alto' | 'Medio' | 'Bajo'
type AdminArea = 'Accesos' | 'Auditoria' | 'Catalogos' | 'Operacion'

interface AdministracionItem {
  id: string
  referencia: string
  area: AdminArea
  estado: AdminEstado
  riesgo: AdminRiesgo
  responsableDemo: string
  alcance: string
  bloqueo: string
}

interface AdministracionFilters {
  texto: string
  estado: '' | AdminEstado
  riesgo: '' | AdminRiesgo
  area: '' | AdminArea
}

const administracionFixture: AdministracionItem[] = [
  {
    id: 'ADM-MVP-1001',
    referencia: 'ADM-DEMO-ACCESOS',
    area: 'Accesos',
    estado: 'Bloqueado',
    riesgo: 'Alto',
    responsableDemo: 'Responsable demo A',
    alcance: 'Inventario anonimo de accesos de producto',
    bloqueo: 'Usuarios reales y permisos reales bloqueados',
  },
  {
    id: 'ADM-MVP-1002',
    referencia: 'ADM-DEMO-AUDITORIA',
    area: 'Auditoria',
    estado: 'Read-only',
    riesgo: 'Medio',
    responsableDemo: 'Responsable demo B',
    alcance: 'Vista demo de eventos administrativos sanitizados',
    bloqueo: 'Sin trazas reales ni identificadores personales',
  },
  {
    id: 'ADM-MVP-1003',
    referencia: 'ADM-DEMO-CATALOGOS',
    area: 'Catalogos',
    estado: 'Pendiente SDD',
    riesgo: 'Medio',
    responsableDemo: 'Responsable demo C',
    alcance: 'Revision futura de catalogos administrativos no sensibles',
    bloqueo: 'Altas, bajas y edicion deshabilitadas',
  },
  {
    id: 'ADM-MVP-1004',
    referencia: 'ADM-DEMO-OPERACION',
    area: 'Operacion',
    estado: 'Bloqueado',
    riesgo: 'Alto',
    responsableDemo: 'Responsable demo D',
    alcance: 'Checklist operativo sin datos internos',
    bloqueo: 'Conexiones, rutas internas y secretos bloqueados',
  },
]

const pageSizeOptions = [2, 4]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Crear usuario demo', icon: 'pi pi-user-plus' },
  { label: 'Editar permisos demo', icon: 'pi pi-lock' },
  { label: 'Exportar auditoria demo', icon: 'pi pi-download' },
]

const filters = reactive<AdministracionFilters>({
  texto: '',
  estado: '',
  riesgo: '',
  area: '',
})

const draftFilters = reactive<AdministracionFilters>({
  texto: '',
  estado: '',
  riesgo: '',
  area: '',
})

const pagination = reactive({
  page: 1,
  pageSize: 2,
})

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)

const filteredItems = computed(() => {
  const texto = normalizeText(filters.texto)

  return administracionFixture.filter((item) => {
    const matchesText =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.responsableDemo).includes(texto) ||
      normalizeText(item.alcance).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesRiesgo = !filters.riesgo || item.riesgo === filters.riesgo
    const matchesArea = !filters.area || item.area === filters.area

    return matchesText && matchesEstado && matchesRiesgo && matchesArea
  })
})

const total = computed(() => filteredItems.value.length)
const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pagination.pageSize)))
const pagedItems = computed(() => {
  const start = (pagination.page - 1) * pagination.pageSize
  return filteredItems.value.slice(start, start + pagination.pageSize)
})
const firstVisible = computed(() =>
  total.value === 0 ? 0 : (pagination.page - 1) * pagination.pageSize + 1,
)
const lastVisible = computed(() => Math.min(pagination.page * pagination.pageSize, total.value))
const resultLabel = computed(() => (total.value === 1 ? 'registro' : 'registros'))
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: AdministracionFilters, source: AdministracionFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.riesgo = source.riesgo
  target.area = source.area
}

function searchAdministracion() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    estado: '',
    riesgo: '',
    area: '',
  })
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function changePage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    pagination.page = page
  }
}

function changePageSize(event: Event) {
  pagination.pageSize = Number((event.target as HTMLSelectElement).value)
  pagination.page = 1
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}
</script>

<template>
  <AppShell
    content-id="administracion-content"
    section-title="Administracion"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="administracion-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Administracion</h1>
      </div>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            type="button"
            :aria-label="action.label"
            title="Accion administrativa no operativa en MVP"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de administracion">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Superficie sensible</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin metadata runtime</span>
      <span
        ><i class="pi pi-shield" aria-hidden="true"></i> Secretos y URLs internas bloqueados</span
      >
      <span
        ><i class="pi pi-users" aria-hidden="true"></i> Usuarios reales y permisos reales
        bloqueados</span
      >
    </section>

    <section class="search-panel" aria-label="Filtros locales de administracion">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchAdministracion">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar vista
          </button>
          <button type="button" disabled>
            <i class="pi pi-user-edit" aria-hidden="true"></i>
            Cambiar permisos
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
              for="administracion-filter-texto"
              style="grid-column: span 4"
            >
              <span>Texto o referencia demo</span>
              <span class="field-control">
                <input
                  id="administracion-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto o referencia demo"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="administracion-filter-estado"
              style="grid-column: span 2"
            >
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="administracion-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Pendiente SDD</option>
                  <option>Bloqueado</option>
                  <option>Read-only</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="administracion-filter-riesgo"
              style="grid-column: span 2"
            >
              <span>Riesgo</span>
              <span class="field-control">
                <select
                  id="administracion-filter-riesgo"
                  v-model="draftFilters.riesgo"
                  aria-label="Riesgo"
                >
                  <option value=""></option>
                  <option>Alto</option>
                  <option>Medio</option>
                  <option>Bajo</option>
                </select>
                <button type="button" aria-label="Opciones de riesgo" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label
              class="filter-field"
              for="administracion-filter-area"
              style="grid-column: span 3"
            >
              <span>Area</span>
              <span class="field-control">
                <select
                  id="administracion-filter-area"
                  v-model="draftFilters.area"
                  aria-label="Area"
                >
                  <option value=""></option>
                  <option>Accesos</option>
                  <option>Auditoria</option>
                  <option>Catalogos</option>
                  <option>Operacion</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field unsupported-filter" style="grid-column: span 5">
              <span>Usuarios, permisos, conexiones y claves reales</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: requiere SDD, API explicita y permisos productivos"
                  disabled
                  aria-label="Datos administrativos reales bloqueados"
                />
                <button type="button" aria-label="Datos administrativos reales bloqueados" disabled>
                  <i class="pi pi-lock" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>
        </section>
      </div>
    </section>

    <section
      class="results-summary"
      aria-labelledby="administracion-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="administracion-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay registros administrativos fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Area</th>
              <th scope="col">Estado</th>
              <th scope="col">Riesgo</th>
              <th scope="col">Responsable</th>
              <th scope="col">Alcance</th>
              <th scope="col">Bloqueo</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Accion administrativa bloqueada para ${item.referencia}`"
                  title="Accion administrativa no operativa"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.area }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.riesgo }}</td>
              <td>{{ item.responsableDemo }}</td>
              <td>{{ item.alcance }}</td>
              <td>{{ item.bloqueo }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de administracion">
        <div class="page-size-control">
          <label for="administracion-page-size">Filas</label>
          <select
            id="administracion-page-size"
            :value="pagination.pageSize"
            @change="changePageSize"
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
