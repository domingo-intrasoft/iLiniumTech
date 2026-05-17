<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type ControlArea = 'Producto' | 'Operaciones' | 'Seguridad' | 'Datos'
type ControlEstado = 'Candidato' | 'Pendiente SDD' | 'Pendiente UAT'
type ControlRiesgo = 'Alto' | 'Medio' | 'Bajo'

interface ControlCandidate {
  id: string
  referencia: string
  nombre: string
  area: ControlArea
  categoria: string
  estado: ControlEstado
  riesgo: ControlRiesgo
  resultado: string
  alcance: string
  propietario: string
}

interface ControlFilters {
  texto: string
  area: '' | ControlArea
  estado: '' | ControlEstado
  riesgo: '' | ControlRiesgo
}

const controlesFixture: ControlCandidate[] = [
  {
    id: 'CTRL-MVP-1001',
    referencia: 'CTRL-2026-0001',
    nombre: 'Revision de alcance de polizas',
    area: 'Producto',
    categoria: 'Categoria candidata',
    estado: 'Pendiente SDD',
    riesgo: 'Medio',
    resultado: 'No operativo: control pendiente de definicion funcional.',
    alcance: 'Lectura y UAT pendientes',
    propietario: 'Owner funcional pendiente',
  },
  {
    id: 'CTRL-MVP-1002',
    referencia: 'CTRL-2026-0002',
    nombre: 'Validacion de datos sanitizados',
    area: 'Datos',
    categoria: 'Categoria candidata',
    estado: 'Candidato',
    riesgo: 'Alto',
    resultado: 'No operativo: no valida datos reales ni dispara procesos.',
    alcance: 'Fixture local sin API',
    propietario: 'DBA/UAT pendiente',
  },
  {
    id: 'CTRL-MVP-1003',
    referencia: 'CTRL-2026-0003',
    nombre: 'Revision de permisos de lectura',
    area: 'Seguridad',
    categoria: 'Categoria candidata',
    estado: 'Pendiente UAT',
    riesgo: 'Alto',
    resultado: 'No operativo: permisos reales no confirmados.',
    alcance: 'Sin ejecucion ni auditoria real',
    propietario: 'Seguridad pendiente',
  },
  {
    id: 'CTRL-MVP-1004',
    referencia: 'CTRL-2026-0004',
    nombre: 'Seguimiento operativo candidato',
    area: 'Operaciones',
    categoria: 'Categoria candidata',
    estado: 'Candidato',
    riesgo: 'Bajo',
    resultado: 'No operativo: listado read-only para conversacion de alcance.',
    alcance: 'Pendiente SDD/UAT',
    propietario: 'Operaciones pendiente',
  },
]

const pageSizeOptions = [2, 4]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Validar alcance', icon: 'pi pi-check-circle' },
  { label: 'Refrescar', icon: 'pi pi-refresh' },
  { label: 'Exportar', icon: 'pi pi-download' },
  { label: 'Abrir detalle', icon: 'pi pi-eye' },
]

const filters = reactive<ControlFilters>({
  texto: '',
  area: '',
  estado: '',
  riesgo: '',
})

const draftFilters = reactive<ControlFilters>({
  texto: '',
  area: '',
  estado: '',
  riesgo: '',
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

  return controlesFixture.filter((item) => {
    const matchesText =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.nombre).includes(texto) ||
      normalizeText(item.categoria).includes(texto) ||
      normalizeText(item.alcance).includes(texto)
    const matchesArea = !filters.area || item.area === filters.area
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesRiesgo = !filters.riesgo || item.riesgo === filters.riesgo

    return matchesText && matchesArea && matchesEstado && matchesRiesgo
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
  total.value === 1 ? 'control candidato' : 'controles candidatos',
)
const tableCaption = computed(
  () =>
    `Controles fixture read-only: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}. Sin ejecucion real, auditoria real ni API backend.`,
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: ControlFilters, source: ControlFilters) {
  target.texto = source.texto
  target.area = source.area
  target.estado = source.estado
  target.riesgo = source.riesgo
}

function searchControles() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    area: '',
    estado: '',
    riesgo: '',
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
    content-id="controles-content"
    section-title="Controles"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="controles-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Controles</h1>
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

    <section class="runtime-strip" aria-label="Contexto de controles">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Datos sanitizados</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Controles no confirmados</span>
      <span><i class="pi pi-flag" aria-hidden="true"></i> SDD/UAT pendiente</span>
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Listado de controles">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de controles">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchControles">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-check-circle" aria-hidden="true"></i>
            Validar alcance
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
            <label class="filter-field" for="controles-filter-texto" style="grid-column: span 3">
              <span>Texto</span>
              <span class="field-control">
                <input
                  id="controles-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto de control"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="controles-filter-area" style="grid-column: span 2">
              <span>Area</span>
              <span class="field-control">
                <select id="controles-filter-area" v-model="draftFilters.area" aria-label="Area">
                  <option value=""></option>
                  <option>Producto</option>
                  <option>Operaciones</option>
                  <option>Seguridad</option>
                  <option>Datos</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="controles-filter-estado" style="grid-column: span 2">
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="controles-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Candidato</option>
                  <option>Pendiente SDD</option>
                  <option>Pendiente UAT</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="controles-filter-riesgo" style="grid-column: span 2">
              <span>Riesgo</span>
              <span class="field-control">
                <select
                  id="controles-filter-riesgo"
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
              class="filter-field unsupported-filter"
              style="grid-column: span 5"
              title="Pendiente de SDD/API"
            >
              <span>Ejecucion, validacion real y auditoria</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: no hay controles funcionales aprobados"
                  disabled
                  aria-label="Ejecucion de controles bloqueada"
                />
                <button type="button" aria-label="Ejecucion de controles bloqueada" disabled>
                  <i class="pi pi-lock" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>
        </section>
      </div>
    </section>

    <section class="results-summary" aria-labelledby="controles-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="controles-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay controles candidatos para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <caption class="sr-only" v-text="tableCaption"></caption>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Control candidato</th>
              <th scope="col">Area</th>
              <th scope="col">Categoria</th>
              <th scope="col">Estado</th>
              <th scope="col">Riesgo</th>
              <th scope="col">Resultado</th>
              <th scope="col">Alcance</th>
              <th scope="col">Propietario</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Detalle pendiente para ${item.referencia}`"
                  title="Detalle pendiente de SDD/UAT"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.nombre }}</td>
              <td>{{ item.area }}</td>
              <td>{{ item.categoria }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.riesgo }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.alcance }}</td>
              <td>{{ item.propietario }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de controles">
        <div class="page-size-control">
          <label for="controles-page-size">Filas</label>
          <select id="controles-page-size" :value="pagination.pageSize" @change="changePageSize">
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
