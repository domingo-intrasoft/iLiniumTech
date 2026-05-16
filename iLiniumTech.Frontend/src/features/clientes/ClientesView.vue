<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type ClienteEstado = 'Activo demo' | 'En revision' | 'Bloqueado PII'
type ClienteSegmento = 'Particular demo' | 'Empresa demo' | 'Colectivo demo'

interface ClienteListItem {
  id: string
  referencia: string
  alias: string
  estado: ClienteEstado
  segmento: ClienteSegmento
  fechaAlta: string
  resultado: string
  datos: string
  relacionadas: string
}

interface ClientesFilters {
  texto: string
  estado: '' | ClienteEstado
  segmento: '' | ClienteSegmento
  fechaAltaDesde: string
}

const clientesFixture: ClienteListItem[] = [
  {
    id: 'CLI-MVP-1001',
    referencia: 'CLI-2026-0001',
    alias: 'Alias anonimo A',
    estado: 'Activo demo',
    segmento: 'Particular demo',
    fechaAlta: '2026-01-12',
    resultado: 'Listado minimizado',
    datos: 'PII bloqueada',
    relacionadas: 'Tabs relacionadas pendientes',
  },
  {
    id: 'CLI-MVP-1002',
    referencia: 'CLI-2026-0002',
    alias: 'Alias anonimo B',
    estado: 'En revision',
    segmento: 'Empresa demo',
    fechaAlta: '2026-02-18',
    resultado: 'Pendiente de SDD',
    datos: 'Datos personales no incluidos',
    relacionadas: 'Polizas y recibos no operativos',
  },
  {
    id: 'CLI-MVP-1003',
    referencia: 'CLI-2026-0003',
    alias: 'Alias anonimo C',
    estado: 'Bloqueado PII',
    segmento: 'Colectivo demo',
    fechaAlta: '2026-03-05',
    resultado: 'Solo trazabilidad demo',
    datos: 'Contacto y bancarios bloqueados',
    relacionadas: 'Riesgos, siniestros y suplementos pendientes',
  },
  {
    id: 'CLI-MVP-1004',
    referencia: 'CLI-2026-0004',
    alias: 'Alias anonimo D',
    estado: 'Activo demo',
    segmento: 'Particular demo',
    fechaAlta: '2026-04-21',
    resultado: 'Fixture local',
    datos: 'Documento legal bloqueado',
    relacionadas: 'Ficha no operativa',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture', 'PII bloqueada']
const moduleActions = [
  { label: 'Buscar clientes demo', icon: 'pi pi-search', active: true },
  { label: 'Abrir ficha bloqueado', icon: 'pi pi-id-card' },
  { label: 'Exportar bloqueado', icon: 'pi pi-download' },
  { label: 'Desglose bloqueado', icon: 'pi pi-sitemap' },
]

const filters = reactive<ClientesFilters>({
  texto: '',
  estado: '',
  segmento: '',
  fechaAltaDesde: '',
})

const draftFilters = reactive<ClientesFilters>({
  texto: '',
  estado: '',
  segmento: '',
  fechaAltaDesde: '',
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

  return clientesFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.alias).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesSegmento = !filters.segmento || item.segmento === filters.segmento
    const matchesFechaAlta = !filters.fechaAltaDesde || item.fechaAlta >= filters.fechaAltaDesde

    return matchesTexto && matchesEstado && matchesSegmento && matchesFechaAlta
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
const resultLabel = computed(() => (total.value === 1 ? 'cliente demo' : 'clientes demo'))
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: ClientesFilters, source: ClientesFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.segmento = source.segmento
  target.fechaAltaDesde = source.fechaAltaDesde
}

function searchClientes() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    estado: '',
    segmento: '',
    fechaAltaDesde: '',
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
    content-id="clientes-content"
    section-title="Clientes"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="clientes-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Clientes</h1>
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

    <section class="runtime-strip" aria-label="Contexto de clientes">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
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
          <button type="button" disabled>
            <i class="pi pi-id-card" aria-hidden="true"></i>
            Abrir ficha
          </button>
          <button type="button" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Exportar
          </button>
          <button type="button" disabled>
            <i class="pi pi-sitemap" aria-hidden="true"></i>
            Desglose
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Busqueda local minimizada</h2>
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
                <button type="button" aria-label="Opciones de referencia" disabled>
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
                  <option>Activo demo</option>
                  <option>En revision</option>
                  <option>Bloqueado PII</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
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
                  <option>Particular demo</option>
                  <option>Empresa demo</option>
                  <option>Colectivo demo</option>
                </select>
                <button type="button" aria-label="Opciones de segmento" disabled>
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
                <button type="button" aria-label="Opciones de fecha alta" disabled>
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
                />
                <button type="button" aria-label="Campos PII bloqueados" disabled>
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

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay clientes fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
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
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Ficha, exportacion y desglose bloqueados para ${item.referencia}`"
                  title="Sin API, sin permisos efectivos y sin SDD de ficha"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
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
