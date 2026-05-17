<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type LiquidacionEstado = 'Pendiente' | 'En revision' | 'Cerrada' | 'Bloqueada'

interface LiquidacionCompaniaListItem {
  id: string
  referencia: string
  compania: string
  estado: LiquidacionEstado
  oficina: string
  fecha: string
  periodo: string
  resultado: string
  origen: string
}

interface LiquidacionesCompaniaFilters {
  texto: string
  estado: '' | LiquidacionEstado
  oficina: string
  fechaDesde: string
}

const liquidacionesFixture: LiquidacionCompaniaListItem[] = [
  {
    id: 'LCIA-MVP-1001',
    referencia: 'LCIA-2026-0001',
    compania: 'Compania demo norte',
    estado: 'Pendiente',
    oficina: 'Oficina demo central',
    fecha: '2026-01-15',
    periodo: 'Enero demo',
    resultado: 'Resultado demo A',
    origen: 'Fixture sanitizado A',
  },
  {
    id: 'LCIA-MVP-1002',
    referencia: 'LCIA-2026-0002',
    compania: 'Compania demo sur',
    estado: 'En revision',
    oficina: 'Oficina demo costa',
    fecha: '2026-02-12',
    periodo: 'Febrero demo',
    resultado: 'Resultado demo B',
    origen: 'Fixture sanitizado B',
  },
  {
    id: 'LCIA-MVP-1003',
    referencia: 'LCIA-2026-0003',
    compania: 'Compania demo este',
    estado: 'Cerrada',
    oficina: 'Oficina demo interior',
    fecha: '2026-03-20',
    periodo: 'Marzo demo',
    resultado: 'Resultado demo C',
    origen: 'Fixture sanitizado C',
  },
  {
    id: 'LCIA-MVP-1004',
    referencia: 'LCIA-2026-0004',
    compania: 'Compania demo oeste',
    estado: 'Bloqueada',
    oficina: 'Oficina demo central',
    fecha: '2026-04-08',
    periodo: 'Abril demo',
    resultado: 'Resultado restringido',
    origen: 'Fixture sanitizado D',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar liquidaciones', icon: 'pi pi-search', active: true },
  { label: 'Detalle bloqueado', icon: 'pi pi-eye' },
  { label: 'Desglose bloqueado', icon: 'pi pi-list' },
  { label: 'Exportacion bloqueada', icon: 'pi pi-download' },
]

const filters = reactive<LiquidacionesCompaniaFilters>({
  texto: '',
  estado: '',
  oficina: '',
  fechaDesde: '',
})

const draftFilters = reactive<LiquidacionesCompaniaFilters>({
  texto: '',
  estado: '',
  oficina: '',
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
  const oficina = normalizeText(filters.oficina)

  return liquidacionesFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.compania).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesOficina = !oficina || normalizeText(item.oficina).includes(oficina)
    const matchesFecha = !filters.fechaDesde || item.fecha >= filters.fechaDesde

    return matchesTexto && matchesEstado && matchesOficina && matchesFecha
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
const resultLabel = computed(() => (total.value === 1 ? 'liquidacion' : 'liquidaciones'))
const tableCaption = computed(
  () =>
    `Liquidaciones de compania fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin importes reales ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: LiquidacionesCompaniaFilters, source: LiquidacionesCompaniaFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.oficina = source.oficina
  target.fechaDesde = source.fechaDesde
}

function searchLiquidaciones() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    estado: '',
    oficina: '',
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
    content-id="liquidaciones-compania-content"
    section-title="Liquidaciones"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="liquidaciones-compania-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Liquidaciones de compania</h1>
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

    <section class="runtime-strip" aria-label="Contexto de liquidaciones de compania">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos minimizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Operaciones financieras bloqueadas</span>
      <span><i class="pi pi-eye-slash" aria-hidden="true"></i> Sin importes reales</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de liquidaciones">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de liquidaciones de compania">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchLiquidaciones">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar busqueda
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
            <h2>Busqueda local</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="liq-cia-filter-texto" style="grid-column: span 3">
              <span>Referencia o compania</span>
              <span class="field-control">
                <input
                  id="liq-cia-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Referencia o compania"
                />
                <button type="button" aria-label="Opciones de referencia o compania" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="liq-cia-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="liq-cia-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Pendiente</option>
                  <option>En revision</option>
                  <option>Cerrada</option>
                  <option>Bloqueada</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="liq-cia-filter-oficina" style="grid-column: span 3">
              <span>Oficina</span>
              <span class="field-control">
                <input
                  id="liq-cia-filter-oficina"
                  v-model="draftFilters.oficina"
                  type="search"
                  aria-label="Oficina"
                />
                <button type="button" aria-label="Opciones de oficina" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="liq-cia-filter-fecha" style="grid-column: span 2">
              <span>Fecha desde</span>
              <span class="field-control">
                <input
                  id="liq-cia-filter-fecha"
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
              style="grid-column: span 6"
              title="Pendiente de SDD/API"
            >
              <span>Detalle, desglose, banco, facturas y exportacion</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Operaciones financieras bloqueadas"
                />
                <button type="button" aria-label="Operaciones financieras bloqueadas" disabled>
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
      aria-labelledby="liquidaciones-compania-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="liquidaciones-compania-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay liquidaciones de compania fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Compania</th>
              <th scope="col">Estado</th>
              <th scope="col">Oficina</th>
              <th scope="col">Fecha</th>
              <th scope="col">Periodo</th>
              <th scope="col">Resultado</th>
              <th scope="col">Origen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle bloqueado para liquidacion ${item.referencia}`"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.compania }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.oficina }}</td>
              <td>{{ formatDate(item.fecha) }}</td>
              <td>{{ item.periodo }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.origen }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de liquidaciones de compania">
        <div class="page-size-control">
          <label for="liq-cia-page-size">Filas</label>
          <select id="liq-cia-page-size" :value="pagination.pageSize" @change="changePageSize">
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
