<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type ReciboSituacion = 'Pendiente' | 'Cobrado' | 'Anulado'
type ReciboTipo = 'Prima' | 'Extorno' | 'Regularizacion'

interface ReciboListItem {
  id: string
  recibo: string
  poliza: string
  cliente: string
  compania: string
  tipo: ReciboTipo
  situacion: ReciboSituacion
  efecto: string
  vencimiento: string
  cobro: string
  canal: string
  importeDemo: string
}

interface RecibosFilters {
  recibo: string
  poliza: string
  situacion: '' | ReciboSituacion
  tipo: '' | ReciboTipo
  vencimientoDesde: string
}

const recibosFixture: ReciboListItem[] = [
  {
    id: 'REC-MVP-1001',
    recibo: 'REC-2026-0001',
    poliza: 'POL-2026-0001',
    cliente: 'Cliente anonimo 1',
    compania: 'Compania demo norte',
    tipo: 'Prima',
    situacion: 'Pendiente',
    efecto: '2026-01-01',
    vencimiento: '2026-02-01',
    cobro: 'No operativo',
    canal: 'Canal demo',
    importeDemo: 'Importe demo A',
  },
  {
    id: 'REC-MVP-1002',
    recibo: 'REC-2026-0002',
    poliza: 'POL-2026-0002',
    cliente: 'Cliente anonimo 2',
    compania: 'Compania demo sur',
    tipo: 'Regularizacion',
    situacion: 'Cobrado',
    efecto: '2026-02-15',
    vencimiento: '2026-03-15',
    cobro: 'Cobro demo confirmado',
    canal: 'Canal demo mediador',
    importeDemo: 'Importe demo B',
  },
  {
    id: 'REC-MVP-1003',
    recibo: 'REC-2026-0003',
    poliza: 'POL-2026-0003',
    cliente: 'Cliente anonimo 3',
    compania: 'Compania demo este',
    tipo: 'Extorno',
    situacion: 'Anulado',
    efecto: '2026-04-01',
    vencimiento: '2026-05-01',
    cobro: 'No operativo',
    canal: 'Canal demo compania',
    importeDemo: 'Importe demo C',
  },
]

const pageSizeOptions = [10, 25, 50]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar recibos', icon: 'pi pi-search', active: true },
  { label: 'Detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]
const blockedActionsDescription =
  'Acciones de recibos bloqueadas en el MVP read-only hasta SDD, contrato API, permisos, UAT y decision de cobros/remesas/datos bancarios.'

const filters = reactive<RecibosFilters>({
  recibo: '',
  poliza: '',
  situacion: '',
  tipo: '',
  vencimientoDesde: '',
})

const draftFilters = reactive<RecibosFilters>({
  recibo: '',
  poliza: '',
  situacion: '',
  tipo: '',
  vencimientoDesde: '',
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
  const recibo = normalizeText(filters.recibo)
  const poliza = normalizeText(filters.poliza)

  return recibosFixture.filter((item) => {
    const matchesRecibo =
      !recibo ||
      normalizeText(item.recibo).includes(recibo) ||
      normalizeText(item.cliente).includes(recibo)
    const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
    const matchesSituacion = !filters.situacion || item.situacion === filters.situacion
    const matchesTipo = !filters.tipo || item.tipo === filters.tipo
    const matchesVencimiento =
      !filters.vencimientoDesde || item.vencimiento >= filters.vencimientoDesde

    return matchesRecibo && matchesPoliza && matchesSituacion && matchesTipo && matchesVencimiento
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
const resultLabel = computed(() => (total.value === 1 ? 'recibo' : 'recibos'))
const tableCaption = computed(
  () =>
    `Recibos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados con importes demo anonimizados y sin API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: RecibosFilters, source: RecibosFilters) {
  target.recibo = source.recibo
  target.poliza = source.poliza
  target.situacion = source.situacion
  target.tipo = source.tipo
  target.vencimientoDesde = source.vencimientoDesde
}

function searchRecibos() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    recibo: '',
    poliza: '',
    situacion: '',
    tipo: '',
    vencimientoDesde: '',
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
    content-id="recibos-content"
    section-title="Recibos"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="recibos-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Recibos</h1>
      </div>

      <p id="recibos-blocked-actions" class="sr-only" v-text="blockedActionsDescription"></p>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            :class="{ active: action.active }"
            type="button"
            :aria-label="action.label"
            aria-describedby="recibos-blocked-actions"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de recibos">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos sanitizados</span>
      <span
        ><i class="pi pi-ban" aria-hidden="true"></i> Detalle, exportacion y cobro pendientes</span
      >
      <span><i class="pi pi-euro" aria-hidden="true"></i> Importes demo anonimizados</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de recibos">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros principales de recibos">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchRecibos">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" aria-describedby="recibos-blocked-actions" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar busqueda
          </button>
          <button type="button" aria-describedby="recibos-blocked-actions" disabled>
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
            <label class="filter-field" for="recibos-filter-recibo" style="grid-column: span 3">
              <span>Recibo o cliente</span>
              <span class="field-control">
                <input
                  id="recibos-filter-recibo"
                  v-model="draftFilters.recibo"
                  type="search"
                  aria-label="Recibo o cliente"
                />
                <button
                  type="button"
                  aria-label="Opciones de recibo"
                  aria-describedby="recibos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="recibos-filter-poliza" style="grid-column: span 3">
              <span>Poliza</span>
              <span class="field-control">
                <input
                  id="recibos-filter-poliza"
                  v-model="draftFilters.poliza"
                  type="search"
                  aria-label="Poliza"
                />
                <button
                  type="button"
                  aria-label="Opciones de poliza"
                  aria-describedby="recibos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="recibos-filter-situacion" style="grid-column: span 2">
              <span>Situacion</span>
              <span class="field-control">
                <select
                  id="recibos-filter-situacion"
                  v-model="draftFilters.situacion"
                  aria-label="Situacion"
                >
                  <option value=""></option>
                  <option>Pendiente</option>
                  <option>Cobrado</option>
                  <option>Anulado</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de situacion"
                  aria-describedby="recibos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="recibos-filter-tipo" style="grid-column: span 2">
              <span>Tipo</span>
              <span class="field-control">
                <select id="recibos-filter-tipo" v-model="draftFilters.tipo" aria-label="Tipo">
                  <option value=""></option>
                  <option>Prima</option>
                  <option>Extorno</option>
                  <option>Regularizacion</option>
                </select>
                <button
                  type="button"
                  aria-label="Opciones de tipo"
                  aria-describedby="recibos-blocked-actions"
                  disabled
                >
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="recibos-filter-vencimiento"
              style="grid-column: span 2"
            >
              <span>Vencimiento desde</span>
              <span class="field-control">
                <input
                  id="recibos-filter-vencimiento"
                  v-model="draftFilters.vencimientoDesde"
                  type="date"
                  aria-label="Vencimiento desde"
                />
                <button
                  type="button"
                  aria-label="Opciones de vencimiento"
                  aria-describedby="recibos-blocked-actions"
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
              <span>Cobro, remesas, documentos y datos bancarios</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Campos financieros sensibles bloqueados"
                  aria-describedby="recibos-blocked-actions"
                />
                <button
                  type="button"
                  aria-label="Campos financieros sensibles bloqueados"
                  aria-describedby="recibos-blocked-actions"
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

    <section class="results-summary" aria-labelledby="recibos-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="recibos-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay recibos fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Recibo</th>
              <th scope="col">Poliza</th>
              <th scope="col">Cliente</th>
              <th scope="col">Compania</th>
              <th scope="col">Tipo</th>
              <th scope="col">Situacion</th>
              <th scope="col">Efecto</th>
              <th scope="col">Vencimiento</th>
              <th scope="col">Cobro</th>
              <th scope="col">Canal</th>
              <th scope="col">Importe</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para recibo ${item.recibo}`"
                  aria-describedby="recibos-blocked-actions"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.recibo }}</strong>
              </td>
              <td>{{ item.poliza }}</td>
              <td>{{ item.cliente }}</td>
              <td>{{ item.compania }}</td>
              <td>{{ item.tipo }}</td>
              <td>{{ item.situacion }}</td>
              <td>{{ formatDate(item.efecto) }}</td>
              <td>{{ formatDate(item.vencimiento) }}</td>
              <td>{{ item.cobro }}</td>
              <td>{{ item.canal }}</td>
              <td>{{ item.importeDemo }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de recibos">
        <div class="page-size-control">
          <label for="recibos-page-size">Filas</label>
          <select id="recibos-page-size" :value="pagination.pageSize" @change="changePageSize">
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
