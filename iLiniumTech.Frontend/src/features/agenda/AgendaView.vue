<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type AgendaEstado = 'Pendiente' | 'Programado' | 'Cerrado'
type AgendaPrioridad = 'Alta' | 'Media' | 'Baja'

interface AgendaListItem {
  id: string
  referencia: string
  asunto: string
  estado: AgendaEstado
  prioridad: AgendaPrioridad
  fechaInicio: string
  horaInicio: string
  fechaFin: string
  horaFin: string
  objetoRelacionado: string
  origen: string
}

interface AgendaFilters {
  texto: string
  estado: '' | AgendaEstado
  prioridad: '' | AgendaPrioridad
  fechaDesde: string
}

const agendaFixture: AgendaListItem[] = [
  {
    id: 'AGE-MVP-1001',
    referencia: 'AGE-2026-0001',
    asunto: 'Revision demo de documentacion',
    estado: 'Pendiente',
    prioridad: 'Alta',
    fechaInicio: '2026-05-18',
    horaInicio: '09:30',
    fechaFin: '2026-05-18',
    horaFin: '10:00',
    objetoRelacionado: 'POL-DEMO-0001',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1002',
    referencia: 'AGE-2026-0002',
    asunto: 'Seguimiento demo de tramite',
    estado: 'Programado',
    prioridad: 'Media',
    fechaInicio: '2026-05-21',
    horaInicio: '12:00',
    fechaFin: '2026-05-21',
    horaFin: '12:30',
    objetoRelacionado: 'SIN-DEMO-0002',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1003',
    referencia: 'AGE-2026-0003',
    asunto: 'Cierre demo de tarea interna',
    estado: 'Cerrado',
    prioridad: 'Baja',
    fechaInicio: '2026-05-24',
    horaInicio: '16:00',
    fechaFin: '2026-05-24',
    horaFin: '16:20',
    objetoRelacionado: 'REC-DEMO-0003',
    origen: 'Fixture local',
  },
  {
    id: 'AGE-MVP-1004',
    referencia: 'AGE-2026-0004',
    asunto: 'Control demo de agenda semanal',
    estado: 'Programado',
    prioridad: 'Alta',
    fechaInicio: '2026-06-02',
    horaInicio: '11:15',
    fechaFin: '2026-06-02',
    horaFin: '11:45',
    objetoRelacionado: 'GEN-DEMO-0004',
    origen: 'Fixture local',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar agenda', icon: 'pi pi-search', active: true },
  { label: 'Crear evento bloqueado', icon: 'pi pi-plus' },
  { label: 'Reprogramar bloqueado', icon: 'pi pi-calendar-times' },
  { label: 'Exportar bloqueado', icon: 'pi pi-download' },
]

const filters = reactive<AgendaFilters>({
  texto: '',
  estado: '',
  prioridad: '',
  fechaDesde: '',
})

const draftFilters = reactive<AgendaFilters>({
  texto: '',
  estado: '',
  prioridad: '',
  fechaDesde: '',
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

  return agendaFixture.filter((item) => {
    const matchesText =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.asunto).includes(texto) ||
      normalizeText(item.objetoRelacionado).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesPrioridad = !filters.prioridad || item.prioridad === filters.prioridad
    const matchesFecha = !filters.fechaDesde || item.fechaInicio >= filters.fechaDesde

    return matchesText && matchesEstado && matchesPrioridad && matchesFecha
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
const resultLabel = computed(() => (total.value === 1 ? 'evento' : 'eventos'))
const tableCaption = computed(
  () =>
    `Agenda fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin calendario dinamico ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: AgendaFilters, source: AgendaFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.prioridad = source.prioridad
  target.fechaDesde = source.fechaDesde
}

function searchAgenda() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    estado: '',
    prioridad: '',
    fechaDesde: '',
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
    content-id="agenda-content"
    section-title="Agenda"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
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
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de agenda">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-calendar" aria-hidden="true"></i> Sin calendario dinamico</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> PII/asuntos sensibles bloqueados</span>
      <span
        ><i class="pi pi-ban" aria-hidden="true"></i> Crear, reprogramar y exportar bloqueados</span
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
          <button type="button" disabled>
            <i class="pi pi-plus" aria-hidden="true"></i>
            Crear
          </button>
          <button type="button" disabled>
            <i class="pi pi-calendar-times" aria-hidden="true"></i>
            Reprogramar
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
            <label class="filter-field" for="agenda-filter-texto" style="grid-column: span 3">
              <span>Texto o referencia</span>
              <span class="field-control">
                <input
                  id="agenda-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto o referencia"
                />
                <button type="button" aria-label="Opciones de texto o referencia" disabled>
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
                <button type="button" aria-label="Opciones de estado" disabled>
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
                <button type="button" aria-label="Opciones de prioridad" disabled>
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
                <button type="button" aria-label="Opciones de fecha" disabled>
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
                />
                <button type="button" aria-label="Campos y acciones sensibles bloqueados" disabled>
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

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
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
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Desglose bloqueado para agenda ${item.referencia}`"
                  title="Desglose pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
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
