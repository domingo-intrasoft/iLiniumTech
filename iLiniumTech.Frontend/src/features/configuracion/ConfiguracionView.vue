<script setup lang="ts">
import { computed, reactive } from 'vue'
import { useRouter } from 'vue-router'

import { useAuthSession } from '@/features/auth/authSession'
import AppShell from '@/layout/AppShell.vue'

type ConfigEstado = 'Visible demo' | 'Redactado' | 'Bloqueado'
type ConfigRiesgo = 'Alto' | 'Medio' | 'Bajo'
type ConfigArea = 'General' | 'Seguridad' | 'Integraciones' | 'Preferencias'

interface ConfiguracionItem {
  id: string
  referencia: string
  area: ConfigArea
  estado: ConfigEstado
  riesgo: ConfigRiesgo
  valorDemo: string
  alcance: string
  bloqueo: string
}

interface ConfiguracionFilters {
  texto: string
  estado: '' | ConfigEstado
  riesgo: '' | ConfigRiesgo
  area: '' | ConfigArea
}

const configuracionFixture: ConfiguracionItem[] = [
  {
    id: 'CFG-MVP-1001',
    referencia: 'CFG-DEMO-GENERAL',
    area: 'General',
    estado: 'Visible demo',
    riesgo: 'Bajo',
    valorDemo: 'Parametro demo no sensible',
    alcance: 'Resumen estatico de configuracion funcional futura',
    bloqueo: 'Sin valores de entorno ni rutas internas',
  },
  {
    id: 'CFG-MVP-1002',
    referencia: 'CFG-DEMO-SEGURIDAD',
    area: 'Seguridad',
    estado: 'Redactado',
    riesgo: 'Alto',
    valorDemo: 'Configurado: valor oculto',
    alcance: 'Indicador sanitizado de politica de seguridad',
    bloqueo: 'Claves, tokens y secretos bloqueados',
  },
  {
    id: 'CFG-MVP-1003',
    referencia: 'CFG-DEMO-INTEGRACIONES',
    area: 'Integraciones',
    estado: 'Bloqueado',
    riesgo: 'Alto',
    valorDemo: 'No operativo',
    alcance: 'Inventario demo sin endpoints ni credenciales',
    bloqueo: 'URLs internas, proveedores y credenciales bloqueados',
  },
  {
    id: 'CFG-MVP-1004',
    referencia: 'CFG-DEMO-PREFERENCIAS',
    area: 'Preferencias',
    estado: 'Visible demo',
    riesgo: 'Medio',
    valorDemo: 'Preferencia demo read-only',
    alcance: 'Preferencias locales no persistidas',
    bloqueo: 'Cambios de configuracion deshabilitados',
  },
]

const pageSizeOptions = [2, 4]
const topBadges = ['Read-only', 'Fixture']
const moduleActions = [
  { label: 'Editar configuracion demo', icon: 'pi pi-pencil' },
  { label: 'Rotar secreto demo', icon: 'pi pi-key' },
  { label: 'Validar integracion demo', icon: 'pi pi-check-circle' },
]

const filters = reactive<ConfiguracionFilters>({
  texto: '',
  estado: '',
  riesgo: '',
  area: '',
})

const draftFilters = reactive<ConfiguracionFilters>({
  texto: '',
  estado: '',
  riesgo: '',
  area: '',
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

  return configuracionFixture.filter((item) => {
    const matchesText =
      !texto ||
      normalizeText(item.referencia).includes(texto) ||
      normalizeText(item.valorDemo).includes(texto) ||
      normalizeText(item.alcance).includes(texto)
    const matchesEstado = !filters.estado || item.estado === filters.estado
    const matchesRiesgo = !filters.riesgo || item.riesgo === filters.riesgo
    const matchesArea = !filters.area || item.area === filters.area

    return matchesText && matchesEstado && matchesRiesgo && matchesArea
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
const resultLabel = computed(() => (total.value === 1 ? 'registro' : 'registros'))
const canGoPrevious = computed(() => pagination.page > 1)
const canGoNext = computed(() => pagination.page < totalPages.value)

function normalizeText(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function copyFilters(target: ConfiguracionFilters, source: ConfiguracionFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.riesgo = source.riesgo
  target.area = source.area
}

function searchConfiguracion() {
  copyFilters(filters, draftFilters)
  pagination.page = 1
}

function clearFilters() {
  copyFilters(draftFilters, {
    texto: '',
    estado: '',
    riesgo: '',
    area: '',
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
    content-id="configuracion-content"
    section-title="Configuracion"
    :session-label="sessionLabel"
    :session-needs-attention="sessionNeedsAttention"
    :top-badges="topBadges"
    :user-label="userLabel"
    show-sign-out
    @sign-out="signOut"
  >
    <div id="configuracion-content" class="polizas-toolbar">
      <div>
        <p class="section-kicker">MVP read-only</p>
        <h1>Configuracion</h1>
      </div>

      <div class="toolbar-groups">
        <div class="action-group">
          <button
            v-for="action in moduleActions"
            :key="action.label"
            class="square-action"
            type="button"
            :aria-label="action.label"
            title="Cambio de configuracion no operativo en MVP"
            disabled
          >
            <i :class="action.icon" aria-hidden="true"></i>
          </button>
        </div>
      </div>
    </div>

    <section class="runtime-strip" aria-label="Contexto de configuracion">
      <span><i class="pi pi-lock" aria-hidden="true"></i> Superficie sensible</span>
      <span><i class="pi pi-database" aria-hidden="true"></i> Fixture local sin API</span>
      <span><i class="pi pi-ban" aria-hidden="true"></i> Sin metadata runtime</span>
      <span
        ><i class="pi pi-shield" aria-hidden="true"></i> Secretos y valores de entorno
        bloqueados</span
      >
      <span
        ><i class="pi pi-link" aria-hidden="true"></i> URLs internas y credenciales bloqueadas</span
      >
    </section>

    <section class="search-panel" aria-label="Filtros locales de configuracion">
      <header class="search-actions">
        <div class="search-action-buttons">
          <button type="button" class="primary-action" @click="searchConfiguracion">
            <i class="pi pi-search" aria-hidden="true"></i>
            Buscar
          </button>
          <button type="button" @click="clearFilters">
            <i class="pi pi-trash" aria-hidden="true"></i>
            Limpiar Filtros
          </button>
          <button type="button" disabled>
            <i class="pi pi-save" aria-hidden="true"></i>
            Guardar cambios
          </button>
          <button type="button" disabled>
            <i class="pi pi-key" aria-hidden="true"></i>
            Rotar secretos
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
            <label
              class="filter-field"
              for="configuracion-filter-texto"
              style="grid-column: span 4"
            >
              <span>Texto o referencia demo</span>
              <span class="field-control">
                <input
                  id="configuracion-filter-texto"
                  v-model="draftFilters.texto"
                  type="search"
                  aria-label="Texto o referencia demo"
                />
                <button type="button" aria-label="Opciones de texto" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="configuracion-filter-estado"
              style="grid-column: span 2"
            >
              <span>Estado</span>
              <span class="field-control">
                <select
                  id="configuracion-filter-estado"
                  v-model="draftFilters.estado"
                  aria-label="Estado"
                >
                  <option value=""></option>
                  <option>Visible demo</option>
                  <option>Redactado</option>
                  <option>Bloqueado</option>
                </select>
                <button type="button" aria-label="Opciones de estado" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label
              class="filter-field"
              for="configuracion-filter-riesgo"
              style="grid-column: span 2"
            >
              <span>Riesgo</span>
              <span class="field-control">
                <select
                  id="configuracion-filter-riesgo"
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
            <label class="filter-field" for="configuracion-filter-area" style="grid-column: span 3">
              <span>Area</span>
              <span class="field-control">
                <select
                  id="configuracion-filter-area"
                  v-model="draftFilters.area"
                  aria-label="Area"
                >
                  <option value=""></option>
                  <option>General</option>
                  <option>Seguridad</option>
                  <option>Integraciones</option>
                  <option>Preferencias</option>
                </select>
                <button type="button" aria-label="Opciones de area" disabled>
                  <i class="pi pi-filter" aria-hidden="true"></i>
                </button>
              </span>
            </label>

            <label class="filter-field unsupported-filter" style="grid-column: span 5">
              <span>Conexiones, claves, proveedores y valores reales</span>
              <span class="field-control">
                <input
                  type="search"
                  value="Bloqueado: no se muestran secretos ni configuracion real"
                  disabled
                  aria-label="Valores de configuracion reales bloqueados"
                />
                <button
                  type="button"
                  aria-label="Valores de configuracion reales bloqueados"
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

    <section
      class="results-summary"
      aria-labelledby="configuracion-results-title"
      aria-live="polite"
    >
      <div class="summary-header">
        <h2 id="configuracion-results-title">Resultado</h2>
        <span>
          <strong>{{ total }}</strong> {{ resultLabel }}
        </span>
        <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
      </div>

      <div v-if="pagedItems.length === 0" class="state state-box empty" role="status">
        <i class="pi pi-inbox" aria-hidden="true"></i>
        <div>
          <strong>Sin resultados</strong>
          <p>No hay registros de configuracion fixture para los filtros actuales.</p>
        </div>
      </div>

      <div v-else class="table-scroll">
        <table>
          <thead>
            <tr>
              <th scope="col">Acciones</th>
              <th scope="col">Referencia</th>
              <th scope="col">Area</th>
              <th scope="col">Estado</th>
              <th scope="col">Riesgo</th>
              <th scope="col">Valor demo</th>
              <th scope="col">Alcance</th>
              <th scope="col">Bloqueo</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in pagedItems" :key="item.id">
              <td>
                <button
                  class="table-icon-action"
                  type="button"
                  :aria-label="`Cambio de configuracion bloqueado para ${item.referencia}`"
                  title="Cambio de configuracion no operativo"
                  disabled
                >
                  <i class="pi pi-eye-slash" aria-hidden="true"></i>
                </button>
              </td>
              <td>
                <strong class="table-strong">{{ item.referencia }}</strong>
              </td>
              <td>{{ item.area }}</td>
              <td>{{ item.estado }}</td>
              <td>{{ item.riesgo }}</td>
              <td>{{ item.valorDemo }}</td>
              <td>{{ item.alcance }}</td>
              <td>{{ item.bloqueo }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <footer class="pagination-bar" aria-label="Paginacion de configuracion">
        <div class="page-size-control">
          <label for="configuracion-page-size">Filas</label>
          <select
            id="configuracion-page-size"
            :value="pagination.pageSize"
            @change="changePageSize"
          >
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
