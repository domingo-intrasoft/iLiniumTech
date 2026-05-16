<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type ConnectivityArea = 'Integraciones' | 'Plataforma' | 'Auditoria'
type ConnectivityState = 'Observado' | 'Bloqueado' | 'Pendiente'
type ConnectivityRisk = 'Alto' | 'Medio' | 'Bajo'

interface ConnectivityItem {
  id: string
  name: string
  area: ConnectivityArea
  category: string
  state: ConnectivityState
  risk: ConnectivityRisk
  result: string
  lastReview: string
  guardrail: string
}

interface ConnectivityFilters {
  text: string
  area: '' | ConnectivityArea
  state: '' | ConnectivityState
  risk: '' | ConnectivityRisk
}

const connectivityFixture: ConnectivityItem[] = [
  {
    id: 'CON-DEMO-001',
    name: 'Canal demo de servicios',
    area: 'Integraciones',
    category: 'REST/SOAP candidato',
    state: 'Bloqueado',
    risk: 'Alto',
    result: 'Prueba externa no operativa',
    lastReview: '2026-05-16',
    guardrail: 'Secretos, logs, payloads y enlaces externos bloqueados',
  },
  {
    id: 'CON-DEMO-002',
    name: 'Resolucion demo por entorno',
    area: 'Plataforma',
    category: 'Configuracion candidata',
    state: 'Observado',
    risk: 'Medio',
    result: 'Solo inventario sanitizado',
    lastReview: '2026-05-16',
    guardrail: 'Sin endpoints reales ni URLs internas',
  },
  {
    id: 'CON-DEMO-003',
    name: 'Auditoria demo de prueba',
    area: 'Auditoria',
    category: 'Trazabilidad candidata',
    state: 'Pendiente',
    risk: 'Bajo',
    result: 'Sin ejecucion ni detalle',
    lastReview: '2026-05-16',
    guardrail: 'Detalle y exportacion deshabilitados',
  },
  {
    id: 'CON-DEMO-004',
    name: 'Allowlist demo de integracion',
    area: 'Integraciones',
    category: 'Gobierno candidato',
    state: 'Bloqueado',
    risk: 'Alto',
    result: 'URL libre no permitida',
    lastReview: '2026-05-16',
    guardrail: 'Pruebas remotas bloqueadas hasta SDD/API',
  },
]

const pageSizeOptions = [2, 4]
const topBadges = ['Read-only', 'Fixture', 'Sin API']
const moduleActions = [
  { label: 'Filtrar conectividad', icon: 'pi pi-search', active: true },
  { label: 'Probar integracion bloqueado', icon: 'pi pi-bolt' },
  { label: 'Exportar bloqueado', icon: 'pi pi-download' },
]

const filters = reactive<ConnectivityFilters>({
  text: '',
  area: '',
  state: '',
  risk: '',
})

const draftFilters = reactive<ConnectivityFilters>({
  text: '',
  area: '',
  state: '',
  risk: '',
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
  const text = normalizeText(filters.text)

  return connectivityFixture.filter((item) => {
    const matchesText =
      !text ||
      normalizeText(item.id).includes(text) ||
      normalizeText(item.name).includes(text) ||
      normalizeText(item.category).includes(text) ||
      normalizeText(item.result).includes(text)
    const matchesArea = !filters.area || item.area === filters.area
    const matchesState = !filters.state || item.state === filters.state
    const matchesRisk = !filters.risk || item.risk === filters.risk

    return matchesText && matchesArea && matchesState && matchesRisk
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
const resultLabel = computed(() =>
  total.value === 1 ? 'entrada candidata' : 'entradas candidatas',
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: ConnectivityFilters, source: ConnectivityFilters) {
  target.text = source.text
  target.area = source.area
  target.state = source.state
  target.risk = source.risk
}

function searchConnectivity() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    text: '',
    area: '',
    state: '',
    risk: '',
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
    content-id="conectividad-content"
    section-title="Conectividad"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="conectividad-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Conectividad</h1>
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
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de conectividad">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos demo sanitizados</span>
      <span
        ><i class="pi pi-ban" aria-hidden="true"></i> Secretos, logs, payloads y enlaces externos
        bloqueados</span
      >
      <span><i class="pi pi-bolt" aria-hidden="true"></i> Pruebas externas deshabilitadas</span>
    </section>

    <section class="search-panel" aria-label="Filtros locales de conectividad">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchConnectivity">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-bolt" aria-hidden="true"></i>
            Probar
          </button>
          <button type="button" disabled>
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
            <label class="filter-field" for="conectividad-filter-text" style="grid-column: span 3">
              <span>Texto</span>
              <span class="field-control">
                <input
                  id="conectividad-filter-text"
                  v-model="draftFilters.text"
                  type="search"
                  aria-label="Texto de conectividad"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="conectividad-filter-area" style="grid-column: span 2">
              <span>Area</span>
              <span class="field-control">
                <select id="conectividad-filter-area" v-model="draftFilters.area" aria-label="Area">
                  <option value=""></option>
                  <option>Integraciones</option>
                  <option>Plataforma</option>
                  <option>Auditoria</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="conectividad-filter-state" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="conectividad-filter-state"
                  v-model="draftFilters.state"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Observado</option>
                  <option>Bloqueado</option>
                  <option>Pendiente</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="conectividad-filter-risk" style="grid-column: span 2">
              <span>Riesgo</span>
              <span class="field-control">
                <select
                  id="conectividad-filter-risk"
                  v-model="draftFilters.risk"
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
            <label class="filter-field unsupported-filter" style="grid-column: span 5">
              <span>Prueba, detalle, endpoints y credenciales</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: sin API, sin URLs reales, sin llamadas externas"
                  disabled
                  aria-label="Pruebas y endpoints bloqueados"
                />
                <button type="button" aria-label="Pruebas y endpoints bloqueados" disabled>
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
      aria-labelledby="conectividad-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="conectividad-results-title">Resultado</h2>
        <span
          ><strong>{{ total }}</strong> {{ resultLabel }}</span
        >
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay entradas fixture de conectividad para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Id demo</th>
              <th scope="col">Nombre</th>
              <th scope="col">Area</th>
              <th scope="col">Categoria</th>
              <th scope="col">Estado</th>
              <th scope="col">Riesgo</th>
              <th scope="col">Resultado</th>
              <th scope="col">Revision</th>
              <th scope="col">Guardrail</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle bloqueado para ${item.id}`"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.id }}</strong>
              </td>
              <td>{{ item.name }}</td>
              <td>{{ item.area }}</td>
              <td>{{ item.category }}</td>
              <td>{{ item.state }}</td>
              <td>{{ item.risk }}</td>
              <td>{{ item.result }}</td>
              <td>{{ formatDate(item.lastReview) }}</td>
              <td>{{ item.guardrail }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de conectividad">
        <div class="page-size-control">
          <label for="conectividad-page-size">Filas</label>
          <select id="conectividad-page-size" :value="pagination.pageSize" @change="changePageSize">
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
