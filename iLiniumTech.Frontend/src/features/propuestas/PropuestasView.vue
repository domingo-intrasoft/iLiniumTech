<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type PropuestaEstado = 'Borrador demo' | 'En revision demo' | 'Caducada demo' | 'Bloqueada demo'
type PropuestaRamo = 'Autos demo' | 'Hogar demo' | 'Comercio demo' | 'Salud demo'

interface PropuestaListItem {
  id: string
  referencia: string
  estado: PropuestaEstado
  ramo: PropuestaRamo
  fechaAlta: string
  vigencia: string
  solicitante: string
  canal: string
  resultado: string
  importeDemo: string
}

interface PropuestasFilters {
  referencia: string
  estado: '' | PropuestaEstado
  ramo: '' | PropuestaRamo
  fechaDesde: string
}

const propuestasFixture: PropuestaListItem[] = [
  {
    id: 'PROP-MVP-1001',
    referencia: 'PROP-2026-0001',
    estado: 'Borrador demo',
    ramo: 'Autos demo',
    fechaAlta: '2026-01-12',
    vigencia: 'Pendiente de SDD',
    solicitante: 'Solicitante anonimo 1',
    canal: 'Canal demo mediador',
    resultado: 'Sin tarificacion operativa',
    importeDemo: 'Importe demo A',
  },
  {
    id: 'PROP-MVP-1002',
    referencia: 'PROP-2026-0002',
    estado: 'En revision demo',
    ramo: 'Hogar demo',
    fechaAlta: '2026-02-20',
    vigencia: 'Pendiente de origen',
    solicitante: 'Solicitante anonimo 2',
    canal: 'Canal demo oficina',
    resultado: 'Revision funcional pendiente',
    importeDemo: 'Importe demo B',
  },
  {
    id: 'PROP-MVP-1003',
    referencia: 'PROP-2026-0003',
    estado: 'Caducada demo',
    ramo: 'Comercio demo',
    fechaAlta: '2026-03-18',
    vigencia: 'No operativa',
    solicitante: 'Solicitante anonimo 3',
    canal: 'Canal demo interno',
    resultado: 'Conversion bloqueada',
    importeDemo: 'Importe demo C',
  },
  {
    id: 'PROP-MVP-1004',
    referencia: 'PROP-2026-0004',
    estado: 'Bloqueada demo',
    ramo: 'Salud demo',
    fechaAlta: '2026-04-05',
    vigencia: 'Pendiente de API',
    solicitante: 'Solicitante anonimo 4',
    canal: 'Canal demo compania',
    resultado: 'Documentos no conectados',
    importeDemo: 'Importe demo D',
  },
]

const estados: PropuestaEstado[] = [
  'Borrador demo',
  'En revision demo',
  'Caducada demo',
  'Bloqueada demo',
]
const ramos: PropuestaRamo[] = ['Autos demo', 'Hogar demo', 'Comercio demo', 'Salud demo']
const pageSizeOptions = [10, 25, 50]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar propuestas', icon: 'pi pi-search', active: true },
  { label: 'Crear propuesta pendiente', icon: 'pi pi-plus' },
  { label: 'Convertir a poliza pendiente', icon: 'pi pi-send' },
  { label: 'Documentos pendientes', icon: 'pi pi-folder' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]

const filters = reactive<PropuestasFilters>({
  referencia: '',
  estado: '',
  ramo: '',
  fechaDesde: '',
})

const draftFilters = reactive<PropuestasFilters>({
  referencia: '',
  estado: '',
  ramo: '',
  fechaDesde: '',
})

const pagination = reactive({
  page: 1,
  pageSize: 25,
})

const router = useRouter()
const { session, userLabel, logout } = useAuthSession()

const sessionLabel = computed(() => {
  return session.value?.currentBrokerId ? `Broker ${session.value.currentBrokerId}` : 'Modo fixture'
})

const sessionNeedsAttention = computed(() => false)

const filteredItems = computed(() => {
  const referencia = normalizeText(filters.referencia)

  return propuestasFixture.filter((item) => {
    const matchesReferencia =
      !referencia ||
      normalizeText(item.referencia).includes(referencia) ||
      normalizeText(item.solicitante).includes(referencia)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesRamo = !filters.ramo || item.ramo === filters.ramo
    const matchesFecha = !filters.fechaDesde || item.fechaAlta >= filters.fechaDesde

    return matchesReferencia && matchesEstado && matchesRamo && matchesFecha
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
const resultLabel = computed(() => (total.value === 1 ? 'propuesta' : 'propuestas'))
const tableCaption = computed(
  () =>
    `Propuestas fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin emision, conversion ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: PropuestasFilters, source: PropuestasFilters) {
  target.referencia = source.referencia
  target.estado = source.estado
  target.ramo = source.ramo
  target.fechaDesde = source.fechaDesde
}

function searchPropuestas() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    referencia: '',
    estado: '',
    ramo: '',
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
    content-id="propuestas-content"
    section-title="Propuestas"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
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
          <button type="button" disabled>
            <i class="pi pi-plus" aria-hidden="true"></i>
            Crear
          </button>
          <button type="button" disabled>
            <i class="pi pi-send" aria-hidden="true"></i>
            Convertir
          </button>
          <button type="button" disabled>
            <i class="pi pi-folder" aria-hidden="true"></i>
            Documentos
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
                <button type="button" aria-label="Opciones de referencia" disabled>
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
                <button type="button" aria-label="Opciones de estado" disabled>
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
                <button type="button" aria-label="Opciones de ramo" disabled>
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
                <button type="button" aria-label="Opciones de fecha" disabled>
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
                />
                <button type="button" aria-label="Funciones de propuesta bloqueadas" disabled>
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
