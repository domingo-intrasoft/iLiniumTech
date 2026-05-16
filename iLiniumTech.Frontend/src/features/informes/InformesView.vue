<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type InformeArea = 'Polizas' | 'Recibos' | 'Riesgos' | 'Liquidaciones'
type InformeEstado = 'Sin aprobar' | 'Bloqueado por evidencia' | 'Requiere SDD'
type InformeRiesgo = 'Alto' | 'Medio'

interface InformeCatalogItem {
  id: string
  categoria: string
  area: InformeArea
  estado: InformeEstado
  riesgo: InformeRiesgo
  resultado: string
  evidencia: string
  bloqueo: string
}

interface InformesFilters {
  texto: string
  area: '' | InformeArea
  estado: '' | InformeEstado
  riesgo: '' | InformeRiesgo
}

const informesFixture: InformeCatalogItem[] = [
  {
    id: 'INF-MVP-1001',
    categoria: 'Polizas en vigor',
    area: 'Polizas',
    estado: 'Bloqueado por evidencia',
    riesgo: 'Alto',
    resultado: 'Candidato tecnico no confirmado',
    evidencia: 'Existen indicios transversales, pero no una pantalla aprobada de Informes.',
    bloqueo: 'Sin catalogo funcional, permisos ni parametros validados.',
  },
  {
    id: 'INF-MVP-1002',
    categoria: 'Recibos y remesas',
    area: 'Recibos',
    estado: 'Requiere SDD',
    riesgo: 'Alto',
    resultado: 'No ejecutable en MVP',
    evidencia: 'Area candidata por trazas documentales; no equivale a informe real.',
    bloqueo: 'Sin contrato de descarga, auditoria ni minimizacion de datos financieros.',
  },
  {
    id: 'INF-MVP-1003',
    categoria: 'Riesgos asociados a poliza',
    area: 'Riesgos',
    estado: 'Sin aprobar',
    riesgo: 'Medio',
    resultado: 'Pendiente de propietario funcional',
    evidencia: 'Nombre de area candidato; filtros y columnas no estan aprobados.',
    bloqueo: 'Sin UAT, permisos ni definicion de campos sensibles.',
  },
  {
    id: 'INF-MVP-1004',
    categoria: 'Liquidaciones',
    area: 'Liquidaciones',
    estado: 'Requiere SDD',
    riesgo: 'Alto',
    resultado: 'Bloqueado para descarga',
    evidencia: 'Puede contener importes, comisiones o facturacion; solo se lista como riesgo.',
    bloqueo: 'Sin revision de seguridad documental, retencion ni autorizacion por informe.',
  },
]

const pageSizeOptions = [2, 10, 25]
const topBadges = ['Read-only', 'Fixture', 'Sin API']
const moduleActions = [
  { label: 'Buscar catalogo', icon: 'pi pi-search', active: true },
  { label: 'Ejecutar bloqueado', icon: 'pi pi-play' },
  { label: 'Descargar bloqueado', icon: 'pi pi-download' },
  { label: 'Historial pendiente', icon: 'pi pi-history' },
]

const filters = reactive<InformesFilters>({
  texto: '',
  area: '',
  estado: '',
  riesgo: '',
})

const draftFilters = reactive<InformesFilters>({
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

  return informesFixture.filter((item) => {
    const matchesTexto =
      !texto ||
      normalizeText(item.categoria).includes(texto) ||
      normalizeText(item.resultado).includes(texto) ||
      normalizeText(item.evidencia).includes(texto) ||
      normalizeText(item.bloqueo).includes(texto)
    const matchesArea = !filters.area || item.area === filters.area
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesRiesgo = !filters.riesgo || item.riesgo === filters.riesgo

    return matchesTexto && matchesArea && matchesEstado && matchesRiesgo
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
  total.value === 1 ? 'categoria candidata' : 'categorias candidatas',
)
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: InformesFilters, source: InformesFilters) {
  target.texto = source.texto
  target.area = source.area
  target.estado = source.estado
  target.riesgo = source.riesgo
}

function searchInformes() {
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
    content-id="informes-content"
    section-title="Informes"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="informes-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Informes</h1>
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

    <section class="runtime-strip" aria-label="Contexto de informes">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin ejecucion ni descarga</span>
      <span><i class="pi pi-shield" aria-hidden="true"></i> Sin informes aprobados todavia</span>
      <span
        ><i class="pi pi-info-circle" aria-hidden="true"></i> Evidencia AppBuilder
        insuficiente</span
      >
    </section>

    <div class="tab-strip">
      <button type="button" class="tab active" aria-label="Filtros">
        <i class="pi pi-search" aria-hidden="true"></i>
      </button>
      <button type="button" class="tab" aria-label="Catalogo de informes">
        <i class="pi pi-table" aria-hidden="true"></i>
      </button>
      <strong>({{ total }})</strong>
    </div>

    <section class="search-panel" aria-label="Filtros locales de informes">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchInformes">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-play" aria-hidden="true"></i>
            Ejecutar
          </button>
          <button type="button" disabled>
            <i class="pi pi-download" aria-hidden="true"></i>
            Descargar
          </button>
        </div>
        <i class="pi pi-chevron-up" aria-hidden="true"></i>
      </header>

      <div class="criteria-card">
        <section class="filter-section">
          <div class="filter-section-title">
            <h2>Catalogo de riesgo</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="informes-filter-texto" style="grid-column: span 3">
              <span>Texto</span>
              <span class="field-control">
                <input
                  id="informes-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="informes-filter-area" style="grid-column: span 2">
              <span>Area</span>
              <span class="field-control">
                <select id="informes-filter-area" v-model="draftFilters.area" aria-label="Area">
                  <option value=""></option>
                  <option>Polizas</option>
                  <option>Recibos</option>
                  <option>Riesgos</option>
                  <option>Liquidaciones</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field" for="informes-filter-estado" style="grid-column: span 3">
              <span>Estado de aprobacion</span>
              <span class="field-control">
                <select
                  id="informes-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado de aprobacion"
                >
                  <option value=""></option>
                  <option>Sin aprobar</option>
                  <option>Bloqueado por evidencia</option>
                  <option>Requiere SDD</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>

          <div class="filter-row">
            <label class="filter-field" for="informes-filter-riesgo" style="grid-column: span 2">
              <span>Riesgo</span>
              <span class="field-control">
                <select
                  id="informes-filter-riesgo"
                  v-model="draftFilters.riesgo"
                  aria-label="Riesgo"
                >
                  <option value=""></option>
                  <option>Alto</option>
                  <option>Medio</option>
                </select>
                <button type="button" aria-label="Opciones de riesgo" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field unsupported-filter"
              style="grid-column: span 6"
              title="Pendiente de SDD/API"
            >
              <span>Ejecucion, descarga, historial y programacion</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: no hay API ni informes aprobados"
                  disabled
                  aria-label="Ejecucion y descarga bloqueadas"
                />
                <button type="button" aria-label="Ejecucion y descarga bloqueadas" disabled>
                  <i class="pi pi-lock" aria-hidden="true"></i>
                </button>
              </span>
            </label>
          </div>
        </section>
      </div>
    </section>

    <section class="results-summary" aria-labelledby="informes-results-title" aria-live="polite">
      <div class="summary-header">
        <h2 id="informes-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay categorias candidatas para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Categoria candidata</th>
              <th scope="col">Area</th>
              <th scope="col">Estado</th>
              <th scope="col">Riesgo</th>
              <th scope="col">Resultado</th>
              <th scope="col">Evidencia</th>
              <th scope="col">Bloqueo</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Ejecucion y descarga bloqueadas para ${item.categoria}`"
                  title="Sin API, sin descarga y sin ejecucion aprobada"
                  disabled
                >
                  <i class="pi pi-ban" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.categoria }}</strong>
              </td>
              <td>{{ item.area }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.riesgo }}</td>
              <td>{{ item.resultado }}</td>
              <td>{{ item.evidencia }}</td>
              <td>{{ item.bloqueo }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de informes">
        <div class="page-size-control">
          <label for="informes-page-size">Filas</label>
          <select id="informes-page-size" :value="pagination.pageSize" @change="changePageSize">
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
