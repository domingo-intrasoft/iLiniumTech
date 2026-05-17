<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type LiquidacionEstado = 'Pendiente' | 'En revision' | 'Cerrada' | 'Bloqueada'
type LiquidacionOficina = 'Oficina demo norte' | 'Oficina demo sur' | 'Oficina demo este'

interface LiquidacionColaboradorListItem {
  id: string
  referencia: string
  colaborador: string
  oficina: LiquidacionOficina
  estado: LiquidacionEstado
  fechaLiquidacion: string
  cierre: string
  recibosResumen: string
  resultado: string
  alcance: string
}

interface LiquidacionesColaboradorFilters {
  texto: string
  estado: '' | LiquidacionEstado
  oficina: '' | LiquidacionOficina
  fechaDesde: string
}

const liquidacionesFixture: LiquidacionColaboradorListItem[] = [
  {
    id: 'LC-MVP-1001',
    referencia: 'LC-2026-0001',
    colaborador: 'Colaborador anonimo A',
    oficina: 'Oficina demo norte',
    estado: 'Pendiente',
    fechaLiquidacion: '2026-01-15',
    cierre: 'Sin cierre operativo',
    recibosResumen: 'Lote demo 01',
    resultado: 'Resultado minimizado A',
    alcance: 'Comisiones y liquidos bloqueados',
  },
  {
    id: 'LC-MVP-1002',
    referencia: 'LC-2026-0002',
    colaborador: 'Colaborador anonimo B',
    oficina: 'Oficina demo sur',
    estado: 'En revision',
    fechaLiquidacion: '2026-02-20',
    cierre: 'Revision read-only',
    recibosResumen: 'Lote demo 02',
    resultado: 'Resultado minimizado B',
    alcance: 'Conceptos no operativos',
  },
  {
    id: 'LC-MVP-1003',
    referencia: 'LC-2026-0003',
    colaborador: 'Colaborador anonimo C',
    oficina: 'Oficina demo este',
    estado: 'Bloqueada',
    fechaLiquidacion: '2026-03-10',
    cierre: 'Bloqueo por SDD pendiente',
    recibosResumen: 'Lote demo 03',
    resultado: 'Resultado minimizado C',
    alcance: 'Detalle financiero oculto',
  },
  {
    id: 'LC-MVP-1004',
    referencia: 'LC-2026-0004',
    colaborador: 'Colaborador anonimo D',
    oficina: 'Oficina demo norte',
    estado: 'Cerrada',
    fechaLiquidacion: '2026-04-05',
    cierre: 'Cierre de muestra',
    recibosResumen: 'Lote demo 04',
    resultado: 'Resultado minimizado D',
    alcance: 'Exportacion bloqueada',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Buscar liquidaciones', icon: 'pi pi-search', active: true },
  { label: 'Detalle bloqueado', icon: 'pi pi-eye' },
  { label: 'Conceptos bloqueados', icon: 'pi pi-list-check' },
  { label: 'Exportacion bloqueada', icon: 'pi pi-download' },
]

const filters = reactive<LiquidacionesColaboradorFilters>({
  texto: '',
  estado: '',
  oficina: '',
  fechaDesde: '',
})

const draftFilters = reactive<LiquidacionesColaboradorFilters>({
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

  return liquidacionesFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.colaborador).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesOficina = !filters.oficina || item.oficina === filters.oficina
    const matchesFecha = !filters.fechaDesde || item.fechaLiquidacion >= filters.fechaDesde

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
    `Liquidaciones de colaborador fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Datos minimizados sin comisiones/liquidos reales ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function copyFilters(
  target: LiquidacionesColaboradorFilters,
  source: LiquidacionesColaboradorFilters,
) {
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
    content-id="liquidaciones-colaborador-content"
    section-title="Liquidaciones"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="liquidaciones-colaborador-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Liquidaciones de colaborador</h1>
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

    <section class="runtime-strip" aria-label="Contexto de liquidaciones de colaborador">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos minimizados y sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Detalle y conceptos no operativos</span>
      <span><i class="pi pi-euro" aria-hidden="true"></i> Comisiones y liquidos bloqueados</span>
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

    <section class="search-panel" aria-label="Filtros locales de liquidaciones de colaborador">
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
            <h2>Busqueda read-only</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="liq-col-filter-texto" style="grid-column: span 4">
              <span>Referencia o colaborador</span>
              <span class="field-control">
                <input
                  id="liq-col-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Referencia o colaborador"
                />
                <button type="button" aria-label="Opciones de referencia" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="liq-col-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="liq-col-filter-estado"
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

            <label class="filter-field" for="liq-col-filter-oficina" style="grid-column: span 2">
              <span>Oficina</span>
              <span class="field-control">
                <select
                  id="liq-col-filter-oficina"
                  v-model="draftFilters.oficina"
                  aria-label="Oficina"
                >
                  <option value=""></option>
                  <option>Oficina demo norte</option>
                  <option>Oficina demo sur</option>
                  <option>Oficina demo este</option>
                </select>
                <button type="button" aria-label="Opciones de oficina" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="liq-col-filter-fecha" style="grid-column: span 2">
              <span>Fecha liquidacion desde</span>
              <span class="field-control">
                <input
                  id="liq-col-filter-fecha"
                  v-model="draftFilters.fechaDesde"
                  type="date"
                  aria-label="Fecha liquidacion desde"
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
              <span>Documentos, datos bancarios, comisiones y liquidos</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Fuera del MVP read-only"
                  disabled
                  aria-label="Campos financieros sensibles bloqueados"
                />
                <button type="button" aria-label="Campos financieros sensibles bloqueados" disabled>
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
      aria-labelledby="liquidaciones-colaborador-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="liquidaciones-colaborador-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay liquidaciones fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Colaborador</th>
              <th scope="col">Oficina</th>
              <th scope="col">Estado</th>
              <th scope="col">Fecha</th>
              <th scope="col">Cierre</th>
              <th scope="col">Recibos</th>
              <th scope="col">Resultado</th>
              <th scope="col">Alcance</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <div class="action-group">
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Detalle bloqueado para liquidacion ${item.referencia}`"
                    title="Detalle pendiente de SDD/API"
                    disabled
                  >
                    <i class="pi pi-eye-slash" aria-hidden="true"></i>
                  </button>
                  <button
                    class="table-icon-action"
                    type="button"
                    :aria-label="`Conceptos bloqueados para liquidacion ${item.referencia}`"
                    title="Conceptos pendientes de SDD/API"
                    disabled
                  >
                    <i class="pi pi-list-check" aria-hidden="true"></i>
                  </button>
                </div>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.colaborador }}</td>
              <td>{{ item.oficina }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ formatDate(item.fechaLiquidacion) }}</td>
              <td>{{ item.cierre }}</td>
              <td>{{ item.recibosResumen }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.alcance }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de liquidaciones de colaborador">
        <div class="page-size-control">
          <label for="liq-col-page-size">Filas</label>
          <select id="liq-col-page-size" :value="pagination.pageSize" @change="changePageSize">
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
