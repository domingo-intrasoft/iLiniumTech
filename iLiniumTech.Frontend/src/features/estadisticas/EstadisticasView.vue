<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type EstadisticaArea = 'Polizas' | 'Recibos' | 'Siniestros' | 'Operaciones'
type EstadisticaEstado = 'Candidata' | 'Pendiente SDD' | 'Pendiente UAT'
type EstadisticaPeriodo = 'Mensual' | 'Trimestral' | 'Anual'

interface EstadisticaCandidate {
  id: string
  referencia: string
  indicador: string
  area: EstadisticaArea
  periodo: EstadisticaPeriodo
  estado: EstadisticaEstado
  categoria: string
  resultado: string
  lectura: string
  privacidad: string
}

interface EstadisticasFilters {
  texto: string
  area: '' | EstadisticaArea
  estado: '' | EstadisticaEstado
  periodo: '' | EstadisticaPeriodo
}

const estadisticasFixture: EstadisticaCandidate[] = [
  {
    id: 'STAT-MVP-1001',
    referencia: 'EST-2026-0001',
    indicador: 'Volumen agregado de polizas',
    area: 'Polizas',
    periodo: 'Mensual',
    estado: 'Pendiente SDD',
    categoria: 'KPI candidato',
    resultado: 'Sin cifra real: metrica no confirmada.',
    lectura: 'Solo sirve para validar forma visual.',
    privacidad: 'Agregado pendiente de regla de supresion.',
  },
  {
    id: 'STAT-MVP-1002',
    referencia: 'EST-2026-0002',
    indicador: 'Distribucion candidata de recibos',
    area: 'Recibos',
    periodo: 'Trimestral',
    estado: 'Candidata',
    categoria: 'KPI candidato',
    resultado: 'Sin cifra real: origen de datos no aprobado.',
    lectura: 'No hay drilldown ni exportacion.',
    privacidad: 'Datos financieros fuera de alcance.',
  },
  {
    id: 'STAT-MVP-1003',
    referencia: 'EST-2026-0003',
    indicador: 'Evolucion candidata de siniestros',
    area: 'Siniestros',
    periodo: 'Anual',
    estado: 'Pendiente UAT',
    categoria: 'KPI candidato',
    resultado: 'Sin cifra real: pendiente de owner funcional.',
    lectura: 'No compara contra fuente autorizada.',
    privacidad: 'Detalle sensible bloqueado.',
  },
  {
    id: 'STAT-MVP-1004',
    referencia: 'EST-2026-0004',
    indicador: 'Resumen operativo candidato',
    area: 'Operaciones',
    periodo: 'Mensual',
    estado: 'Candidata',
    categoria: 'KPI candidato',
    resultado: 'Sin cifra real: no representa actividad real.',
    lectura: 'Pendiente de SDD/UAT.',
    privacidad: 'Sin datos personales.',
  },
]

const pageSizeOptions = [2, 4]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Refrescar', icon: 'pi pi-refresh' },
  { label: 'Exportar', icon: 'pi pi-download' },
  { label: 'Drilldown', icon: 'pi pi-chart-line' },
  { label: 'Cambiar periodo', icon: 'pi pi-calendar' },
]

const filters = reactive<EstadisticasFilters>({
  texto: '',
  area: '',
  estado: '',
  periodo: '',
})

const draftFilters = reactive<EstadisticasFilters>({
  texto: '',
  area: '',
  estado: '',
  periodo: '',
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

  return estadisticasFixture.filter((item) => {
    const matchesText =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.indicador).includes(texto) ||
      normalizeText(item.categoria).includes(texto) ||
      normalizeText(item.resultado).includes(texto)
    const matchesArea = !filters.area || item.area === filters.area
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesPeriodo = !filters.periodo || item.periodo === filters.periodo

    return matchesText && matchesArea && matchesEstado && matchesPeriodo
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
const resultLabel = computed(() => (total.value === 1 ? 'KPI candidato' : 'KPIs candidatos'))
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: EstadisticasFilters, source: EstadisticasFilters) {
  target.texto = source.texto
  target.area = source.area
  target.estado = source.estado
  target.periodo = source.periodo
}

function searchEstadisticas() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    area: '',
    estado: '',
    periodo: '',
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
    content-id="estadisticas-content"
    section-title="Estadisticas"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="estadisticas-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Estadisticas</h1>
      </div>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            type="button"
            :aria-label="`${action.label} no operativo`"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de estadisticas">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-chart-bar" aria-hidden="true"></i> KPIs no confirmados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin drilldown ni exportacion</span>
      <span><i class="pi pi-flag" aria-hidden="true"></i> SDD/UAT pendiente</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Listado de estadisticas">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de estadisticas">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchEstadisticas">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-refresh" aria-hidden="true"></i>
            Refrescar
          </button>
          <button type="button" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
          <button type="button" disabled>
            <i class="pi pi-chart-line" aria-hidden="true"></i>
            Drilldown
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
            <label class="filter-field" for="estadisticas-filter-texto" style="grid-column: span 3">
              <span>Texto</span>
              <span class="field-control">
                <input
                  id="estadisticas-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto de estadistica"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="estadisticas-filter-area" style="grid-column: span 2">
              <span>Area</span>
              <span class="field-control">
                <select id="estadisticas-filter-area" v-model="draftFilters.area" aria-label="Area">
                  <option value=""></option>
                  <option>Polizas</option>
                  <option>Recibos</option>
                  <option>Siniestros</option>
                  <option>Operaciones</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="estadisticas-filter-estado"
              style="grid-column: span 2"
            >
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="estadisticas-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Candidata</option>
                  <option>Pendiente SDD</option>
                  <option>Pendiente UAT</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="estadisticas-filter-periodo"
              style="grid-column: span 2"
            >
              <span>Periodo</span>
              <span class="field-control">
                <select
                  id="estadisticas-filter-periodo"
                  v-model="draftFilters.periodo"
                  aria-label="Periodo"
                >
                  <option value=""></option>
                  <option>Mensual</option>
                  <option>Trimestral</option>
                  <option>Anual</option>
                </select>
                <button type="button" aria-label="Opciones de periodo" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 5"
              title="Pendiente de SDD/API"
            >
              <span>Refresco, exportacion y navegacion a detalle</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: no hay KPIs aprobados ni API"
                  disabled
                  aria-label="Acciones de estadisticas bloqueadas"
                />
                <button type="button" aria-label="Acciones de estadisticas bloqueadas" disabled>
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
      aria-labelledby="estadisticas-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="estadisticas-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay KPIs candidatos para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Indicador candidato</th>
              <th scope="col">Area</th>
              <th scope="col">Periodo</th>
              <th scope="col">Estado</th>
              <th scope="col">Categoria</th>
              <th scope="col">Resultado</th>
              <th scope="col">Lectura</th>
              <th scope="col">Privacidad</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Drilldown pendiente para ${item.referencia}`"
                  title="Drilldown pendiente de SDD/UAT"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.indicador }}</td>
              <td>{{ item.area }}</td>
              <td>{{ item.periodo }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.categoria }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.lectura }}</td>
              <td>{{ item.privacidad }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de estadisticas">
        <div class="page-size-control">
          <label for="estadisticas-page-size">Filas</label>
          <select id="estadisticas-page-size" :value="pagination.pageSize" @change="changePageSize">
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
