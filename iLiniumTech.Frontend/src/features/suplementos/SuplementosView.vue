<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type SuplementoTipo = 'Alta de riesgo' | 'Regularizacion' | 'Domiciliacion' | 'Renovacion'
type SuplementoSituacion = 'Pendiente' | 'En revision' | 'Validado' | 'Bloqueado'

interface SuplementoListItem {
  id: string
  referencia: string
  poliza: string
  tipo: SuplementoTipo
  situacion: SuplementoSituacion
  fechaEfecto: string
  concepto: string
  resumen: string
  origen: string
}

interface SuplementosFilters {
  texto: string
  poliza: string
  tipo: '' | SuplementoTipo
  situacion: '' | SuplementoSituacion
  fechaDesde: string
}

const suplementosFixture: SuplementoListItem[] = [
  {
    id: 'SUP-MVP-1001',
    referencia: 'SUP-2026-0001',
    poliza: 'POL-2026-0001',
    tipo: 'Alta de riesgo',
    situacion: 'Pendiente',
    fechaEfecto: '2026-02-01',
    concepto: 'Incorporacion de cobertura',
    resumen: 'Cambio operativo pendiente de contrato API',
    origen: 'Fixture sanitizado A',
  },
  {
    id: 'SUP-MVP-1002',
    referencia: 'SUP-2026-0002',
    poliza: 'POL-2026-0002',
    tipo: 'Regularizacion',
    situacion: 'En revision',
    fechaEfecto: '2026-03-15',
    concepto: 'Revision de condiciones',
    resumen: 'Movimiento read-only sin importes ni adjuntos',
    origen: 'Fixture sanitizado B',
  },
  {
    id: 'SUP-MVP-1003',
    referencia: 'SUP-2026-0003',
    poliza: 'POL-2026-0003',
    tipo: 'Domiciliacion',
    situacion: 'Bloqueado',
    fechaEfecto: '2026-01-20',
    concepto: 'Cambio administrativo',
    resumen: 'Datos restringidos ocultos hasta SDD',
    origen: 'Fixture sanitizado C',
  },
  {
    id: 'SUP-MVP-1004',
    referencia: 'SUP-2026-0004',
    poliza: 'POL-2026-0004',
    tipo: 'Renovacion',
    situacion: 'Validado',
    fechaEfecto: '2026-04-05',
    concepto: 'Actualizacion de vigencia',
    resumen: 'Lectura de muestra sin workflows',
    origen: 'Fixture sanitizado D',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar suplementos', icon: 'pi pi-search', active: true },
  { label: 'Detalle pendiente', icon: 'pi pi-eye' },
  { label: 'Exportacion pendiente', icon: 'pi pi-download' },
]

const filters = reactive<SuplementosFilters>({
  texto: '',
  poliza: '',
  tipo: '',
  situacion: '',
  fechaDesde: '',
})

const draftFilters = reactive<SuplementosFilters>({
  texto: '',
  poliza: '',
  tipo: '',
  situacion: '',
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
  const poliza = normalizeText(filters.poliza)

  return suplementosFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.concepto).includes(texto) ||
      normalizeText(item.resumen).includes(texto)
    const matchesPoliza = !poliza || normalizeText(item.poliza).includes(poliza)
    const matchesTipo = !filters.tipo || item.tipo === filters.tipo
    const matchesSituacion = !filters.situacion || item.situacion === filters.situacion
    const matchesFecha = !filters.fechaDesde || item.fechaEfecto >= filters.fechaDesde

    return matchesTexto && matchesPoliza && matchesTipo && matchesSituacion && matchesFecha
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
const resultLabel = computed(() => (total.value === 1 ? 'suplemento' : 'suplementos'))
const tableCaption = computed(
  () =>
    `Suplementos fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos sanitizados sin workflows ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(target: SuplementosFilters, source: SuplementosFilters) {
  target.texto = source.texto
  target.poliza = source.poliza
  target.tipo = source.tipo
  target.situacion = source.situacion
  target.fechaDesde = source.fechaDesde
}

function searchSuplementos() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    poliza: '',
    tipo: '',
    situacion: '',
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
    content-id="suplementos-content"
    section-title="Suplementos"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="suplementos-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Suplementos</h1>
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

    <section class="runtime-strip" aria-label="Contexto de suplementos">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin workflows ni escrituras</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Tabla de suplementos">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de suplementos">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchSuplementos">
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
            <label class="filter-field" for="suplementos-filter-texto" style="grid-column: span 3">
              <span>Referencia o concepto</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Referencia o concepto"
                />
                <button type="button" aria-label="Opciones de referencia" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-poliza" style="grid-column: span 3">
              <span>Poliza</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-poliza"
                  v-model="draftFilters.poliza"
                  type="search"
                  aria-label="Poliza"
                />
                <button type="button" aria-label="Opciones de poliza" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-tipo" style="grid-column: span 2">
              <span>Tipo</span>
              <span class="field-control">
                <select id="suplementos-filter-tipo" v-model="draftFilters.tipo" aria-label="Tipo">
                  <option value=""></option>
                  <option>Alta de riesgo</option>
                  <option>Regularizacion</option>
                  <option>Domiciliacion</option>
                  <option>Renovacion</option>
                </select>
                <button type="button" aria-label="Opciones de tipo" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label
              class="filter-field"
              for="suplementos-filter-situacion"
              style="grid-column: span 2"
            >
              <span>Situacion</span>
              <span class="field-control">
                <select
                  id="suplementos-filter-situacion"
                  v-model="draftFilters.situacion"
                  aria-label="Situacion"
                >
                  <option value=""></option>
                  <option>Pendiente</option>
                  <option>En revision</option>
                  <option>Validado</option>
                  <option>Bloqueado</option>
                </select>
                <button type="button" aria-label="Opciones de situacion" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="suplementos-filter-fecha" style="grid-column: span 2">
              <span>Fecha efecto desde</span>
              <span class="field-control">
                <input
                  id="suplementos-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha efecto desde"
                />
                <button type="button" aria-label="Opciones de fecha" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 4"
              title="Pendiente de SDD/API"
            >
              <span>Campos restringidos</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Campos restringidos bloqueados"
                />
                <button type="button" aria-label="Campos restringidos bloqueados" disabled>
                  <i class="pi pi-lock" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>
        </section>
      </div>
    </section>

    <section class="results-summary" aria-labelledby="suplementos-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="suplementos-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay suplementos fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Poliza</th>
              <th scope="col">Tipo</th>
              <th scope="col">Situacion</th>
              <th scope="col">Fecha efecto</th>
              <th scope="col">Concepto</th>
              <th scope="col">Resumen</th>
              <th scope="col">Origen</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para suplemento ${item.referencia}`"
                  title="Detalle pendiente de SDD/API"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.poliza }}</td>
              <td>{{ item.tipo }}</td>
              <td>{{ item.situacion }}</td>
              <td>{{ formatDate(item.fechaEfecto) }}</td>
              <td>{{ item.concepto }}</td>
              <td>{{ item.resumen }}</td>
              <td>{{ item.origen }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de suplementos">
        <div class="page-size-control">
          <label for="suplementos-page-size">Filas</label>
          <select id="suplementos-page-size" :value="pagination.pageSize" @change="changePageSize">
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
