<script setup lang="ts">
import { reactive, ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'

import { usePolizas } from './usePolizas'
import { getPolizaById } from './polizasApi'
import type { PolizaListItem, PolizaDetail } from './polizasTypes'

const router = useRouter()
const { metadata, visibleFields, items, total, loading, error, filters, refresh } = usePolizas()

type FilterControl = 'text' | 'select' | 'date'

interface ShellAction {
  label: string
  icon: string
  active?: boolean
}

interface SearchField {
  key: string
  label: string
  control?: FilterControl
  span: number
}

interface SearchSection {
  title: string
  rows: SearchField[][]
}

const sideItems: ShellAction[] = [
  { label: 'Agenda', icon: 'pi pi-calendar' },
  { label: 'Clientes', icon: 'pi pi-user' },
  { label: 'Propuestas', icon: 'pi pi-folder-open' },
  { label: 'Pólizas', icon: 'pi pi-briefcase', active: true },
  { label: 'Recibos', icon: 'pi pi-money-bill' },
  { label: 'Suplementos', icon: 'pi pi-link' },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle' },
  { label: 'Liq.Cia', icon: 'pi pi-list' },
  { label: 'Liq.Col', icon: 'pi pi-list-check' },
  { label: 'Informes', icon: 'pi pi-file' },
  { label: 'Controles', icon: 'pi pi-home' },
  { label: 'Estadisticas', icon: 'pi pi-chart-bar' },
  { label: 'Administración', icon: 'pi pi-table' },
  { label: 'Configuración', icon: 'pi pi-cog' },
  { label: 'Conectividad', icon: 'pi pi-code' },
  { label: 'By Aunna', icon: 'pi pi-sitemap' },
  { label: 'Logs', icon: 'pi pi-database' },
]

const moduleActions: ShellAction[] = [
  { label: 'Autos', icon: 'pi pi-car', active: true },
  { label: 'Gestión', icon: 'pi pi-id-card' },
  { label: 'Favoritos', icon: 'pi pi-heart-fill' },
  { label: 'Servicios', icon: 'pi pi-plus' },
  { label: 'Riesgos', icon: 'pi pi-truck' },
]

const statusActions: ShellAction[] = [
  { label: 'Validar', icon: 'pi pi-check-circle' },
  { label: 'Pausar', icon: 'pi pi-pause-circle' },
  { label: 'Cerrar', icon: 'pi pi-times-circle' },
]

const topBadges = ['F', '?', 'A', 'L', 'E', 'RH', 'WP', 'AU']

// --- SEARCH & TABS STATE ---
const searchValues = reactive<Record<string, string>>({
  poliza: '',
  certif: '',
  tipoPoliza: '',
  cia: '',
  ramo: '',
  riesgo: '',
  efectoInicial: '',
  vencimiento: '',
  anulacion: '',
  oficina: '',
  division: '',
  colaborador1: '',
  administrativo: '',
  comercial: '',
  siniestros: '',
  gestor: '',
  canalCobro: '',
  fraccionPago: '',
  ccaa: '',
  nombreCompleto: '',
  documento: '',
  apellido1: '',
  apellido2: '',
  nombre: '',
  sexo: '',
  nacimiento: '',
  edad: '',
  fallecimiento: '',
  estadoCivil: '',
  hijos: '',
  regimenLaboral: '',
  profesion: '',
})

// Workspace Tabs Management
const activeTabId = ref<string>('search') // 'search' or policy.id
const openPolicyTabs = ref<PolizaDetail[]>([])
const activeSubTab = ref<string>('poliza') // 'poliza', 'detalle', 'riesgos', etc.

const searchSections: SearchSection[] = [
  {
    title: 'Datos de la póliza',
    rows: [
      [
        { key: 'poliza', label: 'Póliza', span: 2 },
        { key: 'certif', label: 'Certif.', span: 2 },
        { key: 'tipoPoliza', label: 'Tipo póliza', control: 'select', span: 2 },
        { key: 'cia', label: 'Cía.', control: 'select', span: 4 },
        { key: 'ramo', label: 'Ramo', control: 'select', span: 2 },
      ],
      [{ key: 'riesgo', label: 'Riesgo/Matríc.', span: 4 }],
    ],
  },
  {
    title: 'Datos de gestión',
    rows: [
      [
        { key: 'efectoInicial', label: 'F_EfectoInicial', control: 'date', span: 2 },
        { key: 'vencimiento', label: 'F. Vencimiento', control: 'date', span: 2 },
        { key: 'anulacion', label: 'F. Anulación', control: 'date', span: 2 },
        { key: 'oficina', label: 'Oficina', control: 'select', span: 2 },
        { key: 'division', label: 'División', control: 'select', span: 2 },
      ],
      [
        { key: 'colaborador1', label: 'Colaborador 1', control: 'select', span: 2 },
        { key: 'administrativo', label: 'Administrativo', control: 'select', span: 2 },
        { key: 'comercial', label: 'Comercial', control: 'select', span: 2 },
        { key: 'siniestros', label: 'Siniestros', control: 'select', span: 2 },
      ],
      [
        { key: 'gestor', label: 'Gestor', control: 'select', span: 2 },
        { key: 'canalCobro', label: 'Canal cobro', control: 'select', span: 4 },
        { key: 'fraccionPago', label: 'Fracción pago', control: 'select', span: 2 },
        { key: 'ccaa', label: 'CC AA', control: 'select', span: 2 },
      ],
    ],
  },
  {
    title: 'Datos del tomador',
    rows: [
      [
        { key: 'nombreCompleto', label: 'N. Completo', span: 4 },
        { key: 'documento', label: 'N.º Documento', span: 2 },
      ],
      [
        { key: 'apellido1', label: 'Apellido 1', span: 2 },
        { key: 'apellido2', label: 'Apellido 2', span: 2 },
        { key: 'nombre', label: 'Nombre', span: 2 },
      ],
      [
        { key: 'sexo', label: 'Sexo', control: 'select', span: 1 },
        { key: 'nacimiento', label: 'F. Nacimiento', control: 'date', span: 2 },
        { key: 'edad', label: 'Edad', span: 1 },
        { key: 'fallecimiento', label: 'F. Fallecimiento', control: 'date', span: 2 },
        { key: 'estadoCivil', label: 'Edo. Civil', control: 'select', span: 2 },
        { key: 'hijos', label: 'N.º Hijos', span: 1 },
      ],
      [
        { key: 'regimenLaboral', label: 'Reg. Laboral', control: 'select', span: 4 },
        { key: 'profesion', label: 'Profesión', control: 'select', span: 4 },
      ],
    ],
  },
]

function formatValue(value: unknown, type: string) {
  if (value === null || value === undefined || value === '') {
    return '-'
  }

  if (type === 'money' && typeof value === 'number') {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
      maximumFractionDigits: 2,
    }).format(value)
  }

  if (type === 'date' && typeof value === 'string') {
    try {
      return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
    } catch {
      return value
    }
  }

  return String(value)
}

function fieldStyle(field: SearchField) {
  return { gridColumn: `span ${field.span}` }
}

function searchValue(key: string) {
  return searchValues[key] ?? ''
}

async function executeSearch() {
  filters.numero = searchValue('poliza')
  filters.cliente = searchValue('nombreCompleto') || searchValue('nombre')
  filters.estado = searchValue('tipoPoliza')
  filters.compania = searchValue('cia')
  filters.ramo = searchValue('ramo')
  filters.documento = searchValue('documento')
  filters.fechaEfectoDesde = searchValue('efectoInicial')
  filters.fechaEfectoHasta = searchValue('vencimiento')
  await refresh()
}

function clearFilters() {
  Object.keys(searchValues).forEach((key) => {
    searchValues[key] = ''
  })
  filters.numero = ''
  filters.cliente = ''
  filters.estado = ''
  filters.compania = ''
  filters.ramo = ''
  filters.documento = ''
  filters.fechaEfectoDesde = ''
  filters.fechaEfectoHasta = ''
}

// --- TABS & DETAIL VIEW OPERATIONS ---
const activePolicy = computed(() => {
  return openPolicyTabs.value.find((p) => p.id === activeTabId.value) || null
})

async function openPolicyDetail(item: PolizaListItem) {
  loading.value = true
  try {
    const detail = await getPolizaById(item.id)
    const exists = openPolicyTabs.value.some((p) => p.id === detail.id)
    if (!exists) {
      openPolicyTabs.value.push(detail)
    }
    activeTabId.value = detail.id
    activeSubTab.value = 'poliza'
  } catch (err) {
    console.error('Error opening policy details:', err)
  } finally {
    loading.value = false
  }
}

// function closePolicyTab(id: string) {
//   const index = openPolicyTabs.value.findIndex((p) => p.id === id)
//   if (index !== -1) {
//     openPolicyTabs.value.splice(index, 1)
//   }
//   if (activeTabId.value === id) {
//     activeTabId.value = 'search'
//   }
// }

function selectTab(id: string) {
  activeTabId.value = id
}

async function navigatePolicy(direction: number) {
  if (items.value.length === 0 || !activePolicy.value) return
  const currentIndex = items.value.findIndex((item) => item.id === activePolicy.value?.id)
  if (currentIndex === -1) return

  let newIndex = currentIndex + direction
  if (newIndex < 0) newIndex = items.value.length - 1
  if (newIndex >= items.value.length) newIndex = 0

  const targetItem = items.value[newIndex]
  if (targetItem) {
    await openPolicyDetail(targetItem)
  }
}

async function onPolicySelectorChange(event: Event) {
  const select = event.target as HTMLSelectElement
  const selectedId = select.value
  const foundItem = items.value.find((item) => item.id === selectedId)
  if (foundItem) {
    await openPolicyDetail(foundItem)
  }
}

function navigateToRecibo(numero: string) {
  router.push({ path: '/recibos', query: { numero } })
}

function navigateToRiesgo(documento: string) {
  router.push({ path: '/riesgos', query: { documento } })
}

function getReciboEstadoClass(estado?: string) {
  if (!estado) return 'status-blue'
  const clean = estado.toLowerCase()
  if (clean.includes('cobrado') || clean.includes('liquid')) {
    return 'status-green'
  }
  if (clean.includes('devuelto') || clean.includes('anulado') || clean.includes('impag')) {
    return 'status-red'
  }
  return 'status-blue'
}

onMounted(async () => {
  await refresh()
  if (items.value.length > 0 && items.value[0]) {
    await openPolicyDetail(items.value[0])
  }
})
</script>

<template>
  <main class="ilinium-shell">
    <!-- Main Left Sidebar Menu -->
    <aside class="il-sidebar" aria-label="Menu principal">
      <div class="broker-logo"><span></span>AUXFISE</div>
      <nav class="side-nav">
        <a
          v-for="item in sideItems"
          :key="item.label"
          href="#"
          :class="{ active: item.active }"
          :aria-current="item.active ? 'page' : undefined"
        >
          <i :class="item.icon"></i>
          <span>{{ item.label }}</span>
        </a>
      </nav>
    </aside>

    <!-- Main Workspace Area -->
    <section class="workspace">
      <!-- Breadcrumb and Top Banner Header -->
      <header class="workspace-topbar">
        <div class="breadcrumb-line">
          <button class="icon-button" type="button" aria-label="Menu">
            <i class="pi pi-bars"></i>
          </button>
          <strong>Inicio</strong>
          <span>/</span>
          <strong>Pólizas</strong>
          <span>/</span>
        </div>

        <span class="environment-badge">Aunna Tech | Portal (DEMO)</span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <span v-for="badge in topBadges" :key="badge" class="round-badge">{{ badge }}</span>
          <span class="user-name">Domingo () (-1)</span>
          <button class="icon-button ghost" type="button" aria-label="Notificaciones">
            <i class="pi pi-bell"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Configuracion">
            <i class="pi pi-cog"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Salir">
            <i class="pi pi-sign-out"></i>
          </button>
        </div>
      </header>

      <!-- Upper Module Action Toolbar (Autos, Gestión, etc.) -->
      <div class="polizas-toolbar">
        <div class="toolbar-groups">
          <div class="action-group">
            <button
              v-for="action in moduleActions"
              :key="action.label"
              class="square-action"
              :class="{ active: action.active }"
              type="button"
              :aria-label="action.label"
            >
              <i :class="action.icon"></i>
            </button>
          </div>
          <div class="action-group compact">
            <button
              v-for="action in statusActions"
              :key="action.label"
              class="square-action dark"
              type="button"
              :aria-label="action.label"
            >
              <i :class="action.icon"></i>
            </button>
          </div>
        </div>

        <div class="context-buttons">
          <button type="button">Pólizas de flota</button>
          <button type="button">Pólizas colectivas</button>
          <button type="button">Pólizas Externas</button>
        </div>
      </div>

      <!-- Tab Navigation Strip (Replaced with Premium Selection Bar) -->
      <div class="tab-strip premium-selection-bar">
        <div class="selection-left">
          <i
            class="pi pi-folder-open"
            style="font-size: 1.1rem; color: #0b5570; margin-right: 6px"
          ></i>
          <span class="total-indicator"
            >Pólizas <strong style="color: #0b5570">({{ total }})</strong></span
          >

          <!-- Dropdown Selector -->
          <div class="policy-dropdown-wrapper">
            <select
              :value="activeTabId === 'search' ? '' : activeTabId"
              @change="onPolicySelectorChange($event)"
              class="policy-dropdown-select"
            >
              <option value="" disabled selected v-if="activeTabId === 'search'">
                -- Seleccione una Póliza --
              </option>
              <option v-for="item in items" :key="item.id" :value="item.id">
                Nº: {{ item.numero }} - {{ item.clienteNombre }}
              </option>
            </select>
          </div>
        </div>

        <div class="selection-right">
          <!-- Button to toggle Advanced Search -->
          <button
            type="button"
            class="advanced-search-toggle-btn"
            :class="{ active: activeTabId === 'search' }"
            @click="selectTab('search')"
          >
            <i class="pi pi-search" style="margin-right: 4px"></i>
            Búsqueda Avanzada
          </button>
        </div>
      </div>

      <!-- LOADING & ERROR STATES -->
      <div v-if="loading" class="state-container">
        <p class="state"><i class="pi pi-spin pi-spinner"></i> Cargando datos de póliza...</p>
      </div>
      <div v-else-if="error" class="state-container">
        <p class="state error"><i class="pi pi-exclamation-circle"></i> {{ error }}</p>
      </div>

      <!-- TAB CONTENT: ADVANCED SEARCH & RESULTS LIST -->
      <div v-else-if="activeTabId === 'search'" class="tab-content-search animate-fadeIn">
        <section class="search-panel" aria-label="Búsqueda avanzada de pólizas">
          <header class="search-actions">
            <div class="search-action-buttons">
              <button type="button" class="primary-action" @click="executeSearch">
                <i class="pi pi-search"></i>
                Buscar
              </button>
              <button type="button" @click="clearFilters">
                <i class="pi pi-trash"></i>
                Limpiar Filtros
              </button>
              <button type="button"><i class="pi pi-times"></i>Cerrar Pestañas</button>
              <button type="button"><i class="pi pi-save"></i>Guardar búsqueda</button>
              <button type="button"><i class="pi pi-arrow-up"></i>Avanzada</button>
              <button type="button" class="primary-action compact-action">
                <i class="pi pi-search"></i>
                Simple
              </button>
              <button type="button" class="primary-action icon-only" aria-label="Agregar filtro">
                <i class="pi pi-plus"></i>
              </button>
            </div>
            <i class="pi pi-chevron-up"></i>
          </header>

          <div class="criteria-card">
            <div class="criteria-select">
              <button type="button">Seleccione... <i class="pi pi-chevron-down"></i></button>
            </div>

            <section v-for="section in searchSections" :key="section.title" class="filter-section">
              <div class="filter-section-title">
                <h2>{{ section.title }}</h2>
                <i class="pi pi-minus"></i>
              </div>

              <div class="filter-row" v-for="(row, rowIndex) in section.rows" :key="rowIndex">
                <label
                  v-for="field in row"
                  :key="field.key"
                  class="filter-field"
                  :style="fieldStyle(field)"
                >
                  <span>{{ field.label }}</span>
                  <span class="field-control">
                    <select v-if="field.control === 'select'" v-model="searchValues[field.key]">
                      <option value=""></option>
                      <option value="demo">Demo</option>
                    </select>
                    <input
                      v-else
                      v-model="searchValues[field.key]"
                      :type="field.control === 'date' ? 'text' : 'search'"
                    />
                    <button type="button" aria-label="Filtro de campo">
                      <i class="pi pi-filter"></i>
                    </button>
                  </span>
                </label>
              </div>
            </section>
          </div>
        </section>

        <!-- Results List Grid -->
        <section class="results-summary" aria-live="polite">
          <div class="summary-header">
            <span>
              Resultado MVP: <strong>{{ total }}</strong> pólizas encontradas en base de datos.
            </span>
            <span v-if="metadata" class="appbuilder-metadata">
              AppBuilder {{ metadata.appBuilder.rootComponentId }} /
              {{ metadata.appBuilder.crudComponentId }} / DS {{ metadata.appBuilder.dataSourceId }}
            </span>
          </div>

          <div v-if="items.length === 0" class="state-container">
            <p class="state">No se han encontrado pólizas en la base de datos de desarrollo.</p>
          </div>

          <div v-else class="table-scroll">
            <table class="premium-table">
              <thead>
                <tr>
                  <th v-for="field in visibleFields" :key="field.name" scope="col">
                    {{ field.label }}
                  </th>
                  <th scope="col" style="text-align: center">Acciones</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="item in items"
                  :key="item.id"
                  class="clickable-row"
                  @click="openPolicyDetail(item)"
                >
                  <td v-for="field in visibleFields" :key="field.name">
                    <span v-if="field.name === 'numero'" class="policy-number-link">
                      {{ formatValue(item[field.name as keyof typeof item], field.type) }}
                    </span>
                    <span v-else>
                      {{ formatValue(item[field.name as keyof typeof item], field.type) }}
                    </span>
                  </td>
                  <td style="text-align: center">
                    <button class="open-detail-action" type="button" title="Ver detalles y editar">
                      <i class="pi pi-pencil"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </div>

      <!-- TAB CONTENT: COMPREHENSIVE POLICY EDITOR/DETAILS VIEW (Matches Captura 1) -->
      <div v-else-if="activePolicy" class="policy-detail-view animate-fadeIn">
        <!-- 1. Top Dynamic Status Info Bar -->
        <div class="policy-info-bar">
          <button class="actions-split-btn" type="button">
            Acciones <i class="pi pi-chevron-down" style="font-size: 0.6rem; margin-left: 6px"></i>
          </button>

          <span class="status-badge" :class="activePolicy.estado.toLowerCase()">
            {{ activePolicy.estado }}
          </span>

          <div class="info-meta-text">
            <strong>{{ activePolicy.cliente.nombre }}</strong>
            <span class="sep">|</span>
            <span
              >PólizaId: <strong>{{ activePolicy.id }}</strong></span
            >
            <span class="sep">|</span>
            <span
              >Nº Póliza: <strong>{{ activePolicy.numero }}</strong></span
            >
            <span class="sep">|</span>
            <span
              >ClienteId: <strong>{{ activePolicy.cliente.id.replace('CLI-', '') }}</strong></span
            >
            <span class="sep">|</span>
            <span>Recibos pendientes: <strong class="zero">0</strong></span>
            <span class="sep">|</span>
            <span
              >P. An. Prod:
              <strong>{{ formatValue(activePolicy.financiero.primaAnual, 'money') }}</strong></span
            >
            <span class="sep">|</span>
            <span
              >P. An. Cart:
              <strong>{{ formatValue(activePolicy.financiero.primaAnual, 'money') }}</strong></span
            >
            <span class="sep">|</span>
            <span>% Increm: <strong class="zero">0.00%</strong></span>
          </div>

          <div class="info-nav-controls">
            <button class="control-arrow-btn" type="button" @click="navigatePolicy(-1)">
              <i class="pi pi-chevron-left"></i>
            </button>
            <button class="control-arrow-btn" type="button" @click="navigatePolicy(1)">
              <i class="pi pi-chevron-right"></i>
            </button>
          </div>
        </div>

        <!-- 2. Sub-tab strip (Póliza, Detalle, Riesgos, etc.) -->
        <div class="sub-tab-strip">
          <button
            v-for="sub in [
              'Póliza',
              'Detalle',
              'Riesgos',
              'Recibos',
              'Garantías',
              'Siniestros',
              'Suplementos',
              'Documentos',
              'Gestiones',
              'Histórico',
            ]"
            :key="sub"
            type="button"
            class="sub-tab"
            :class="{ active: activeSubTab === sub.toLowerCase() }"
            @click="activeSubTab = sub.toLowerCase()"
          >
            {{ sub }}
          </button>
        </div>

        <!-- 3. Sub-tab Content: Póliza view dashboard (Grid Layout) -->
        <div v-if="activeSubTab === 'poliza'" class="sub-tab-content-poliza">
          <!-- Upper Grid: Datos, Fechas, Cliente Profile -->
          <div class="poliza-upper-grid">
            <!-- Card 1: Datos de la póliza -->
            <div class="details-card form-card">
              <header class="card-header">
                <h2>Datos de la póliza</h2>
                <div class="header-toggles">
                  <label class="toggle-control">
                    <span>Revis.</span>
                    <input type="checkbox" checked />
                    <span class="toggle-slider"></span>
                  </label>
                  <label class="toggle-control">
                    <span>Presencial</span>
                    <input type="checkbox" />
                    <span class="toggle-slider"></span>
                  </label>
                </div>
              </header>

              <div class="card-body grid-layout-datos">
                <div class="field-item">
                  <label>Nº póliza</label>
                  <input type="text" :value="activePolicy.numero" disabled class="input-disabled" />
                </div>

                <div class="field-item">
                  <label>Compañía</label>
                  <div class="input-group">
                    <input type="text" :value="activePolicy.compania" />
                    <button type="button"><i class="pi pi-ellipsis-h"></i></button>
                  </div>
                </div>

                <div class="field-item required">
                  <label>Tipo póliza</label>
                  <select>
                    <option>Producción</option>
                    <option>Cartera</option>
                  </select>
                </div>

                <div class="field-item required">
                  <label>Origen</label>
                  <select>
                    <option>Producción</option>
                    <option>Migración</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Ramo</label>
                  <div class="input-group">
                    <select>
                      <option>300 + Riesgo Individual</option>
                      <option>100 + Autos</option>
                    </select>
                    <button type="button"><i class="pi pi-pencil"></i></button>
                  </div>
                </div>

                <div class="field-item">
                  <label>Tipo de seguro</label>
                  <div class="input-group">
                    <input type="text" :value="activePolicy.producto.nombre" />
                    <button type="button"><i class="pi pi-search"></i></button>
                  </div>
                </div>

                <div class="field-item">
                  <label>Gestor</label>
                  <select>
                    <option>Compañía</option>
                    <option>Correduría</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Canal cobro</label>
                  <select>
                    <option>Directo compañía</option>
                    <option>Recibo domiciliado</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Fracción pago</label>
                  <select>
                    <option>Anual</option>
                    <option>Semestral</option>
                    <option>Mensual</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Duración</label>
                  <select>
                    <option>Renovable</option>
                    <option>Temporal</option>
                  </select>
                </div>

                <div class="field-item required">
                  <label>Regularizable</label>
                  <select>
                    <option>No</option>
                    <option>Sí</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Estado</label>
                  <select>
                    <option value=""></option>
                    <option>Vigor</option>
                    <option>Anulada</option>
                  </select>
                </div>

                <div class="field-item">
                  <label>División</label>
                  <select>
                    <option>AUXFISE ASESORES, SL</option>
                  </select>
                </div>

                <div class="field-item required">
                  <label>Oficina</label>
                  <select>
                    <option>Auxfise (Guadalajara)</option>
                    <option>Auxfise (Madrid)</option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Card 2: Fechas y Situación -->
            <div class="details-card form-card">
              <header class="card-header">
                <h2>Fechas</h2>
                <div class="header-badges">
                  <span class="badge-item blue"
                    >Situación: <strong class="vig">{{ activePolicy.estado }}</strong></span
                  >
                  <span class="badge-item circular">1</span>
                  <span class="badge-item pill-blue"
                    >Tipo: <strong>{{ activePolicy.producto.modalidad }}</strong></span
                  >
                </div>
              </header>

              <div class="card-body grid-layout-fechas">
                <div class="field-item required">
                  <label>Efecto Inicial</label>
                  <input
                    type="text"
                    :value="formatValue(activePolicy.vigencia.fechaInicio, 'date')"
                  />
                </div>

                <div class="field-item">
                  <label>Administrativo</label>
                  <select>
                    <option value=""></option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Renovación - Vto.</label>
                  <input
                    type="text"
                    :value="formatValue(activePolicy.vigencia.fechaVencimiento, 'date')"
                  />
                </div>

                <div class="field-item">
                  <label>Comercial</label>
                  <select>
                    <option value=""></option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Últ. Suplm.</label>
                  <input type="text" value="" />
                </div>

                <div class="field-item">
                  <label>Siniestros</label>
                  <select>
                    <option value=""></option>
                  </select>
                </div>

                <div class="field-item">
                  <label>Anulación</label>
                  <input type="text" value="" />
                </div>

                <div class="field-item">
                  <label>Colab. 1.</label>
                  <select>
                    <option>Andresmiras, S.L.</option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Card 3: Cliente/Tomador profile sidebar card -->
            <div class="details-card profile-card">
              <div class="profile-header">
                <div class="avatar-circle">
                  <i class="pi pi-user"></i>
                </div>
                <div class="profile-title">
                  <h3>{{ activePolicy.cliente.nombre }}</h3>
                  <div class="profile-badges-row">
                    <span class="profile-badge-vigor">{{ activePolicy.estado }}</span>
                    <span class="star-rating">
                      <i class="pi pi-star-fill"></i>
                      <i class="pi pi-star-fill"></i>
                      <i class="pi pi-star-fill"></i>
                      <i class="pi pi-star-fill"></i>
                      <i class="pi pi-star-fill"></i>
                    </span>
                  </div>
                </div>
              </div>

              <div class="profile-contact-list">
                <div class="contact-row">
                  <i class="pi pi-id-card"></i>
                  <span>{{ activePolicy.cliente.documento }}</span>
                  <button type="button" title="Copiar documento"><i class="pi pi-copy"></i></button>
                </div>
                <div class="contact-row">
                  <i class="pi pi-envelope"></i>
                  <span>lara.hernandez.94@hotmail.com</span>
                </div>
                <div class="contact-row">
                  <i class="pi pi-phone"></i>
                  <span>697339841</span>
                </div>
              </div>

              <hr class="profile-divider" />

              <div class="profile-receipt-section">
                <h4>Últ. Recibo: 251436851</h4>
                <div class="receipt-badges-row">
                  <span class="receipt-badge status-green">Cobrado</span>
                  <span class="receipt-badge status-blue">Liquidado</span>
                  <span class="receipt-badge status-green">Liquidado</span>
                </div>

                <div class="company-brand-section">
                  <span class="brand-name">{{ activePolicy.compania }}</span>
                </div>

                <ul class="receipt-metadata-list">
                  <li><strong>Tipo recibo:</strong> Producción</li>
                  <li><strong>Gestor:</strong> Compañía</li>
                  <li><strong>Forma pago:</strong> Anual</li>
                  <li>
                    <strong>F. Efecto:</strong>
                    {{ formatValue(activePolicy.vigencia.fechaInicio, 'date') }} -
                    <strong>F. Vto:</strong>
                    {{ formatValue(activePolicy.vigencia.fechaVencimiento, 'date') }}
                  </li>
                </ul>
              </div>
            </div>
          </div>

          <!-- Lower Grid: Riesgos, Figuras, Recibos, Anulación -->
          <div class="poliza-lower-grid">
            <!-- Left Side Cards: Riesgos & Recibos -->
            <div class="lower-left-column">
              <!-- Riesgos card -->
              <div class="details-card table-card">
                <header class="card-header">
                  <h2>Riesgos</h2>
                  <div class="card-toolbar">
                    <button type="button"><i class="pi pi-plus"></i></button>
                    <button type="button"><i class="pi pi-pencil"></i></button>
                    <button type="button"><i class="pi pi-info-circle"></i></button>
                  </div>
                </header>
                <div class="table-scroll">
                  <table class="grid-table">
                    <thead>
                      <tr>
                        <th>NIF/Documento</th>
                        <th>Descripción del Riesgo</th>
                        <th>Tipo riesgo</th>
                        <th>F. Alta</th>
                        <th>F. Baja</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="riesgo in activePolicy.riesgos" :key="riesgo.id">
                        <td>
                          <a href="#" @click.prevent="navigateToRiesgo(activePolicy.cliente.documento)" class="item-link"
                            ><i class="pi pi-id-card"></i> NIF
                            {{ activePolicy.cliente.documento }}</a
                          >
                        </td>
                        <td>{{ riesgo.descripcion }}</td>
                        <td>{{ riesgo.tipoRiesgo || '-' }}</td>
                        <td>{{ formatValue(riesgo.fechaAlta, 'date') }}</td>
                        <td>{{ formatValue(riesgo.fechaBaja, 'date') }}</td>
                      </tr>
                      <tr v-if="!activePolicy.riesgos || activePolicy.riesgos.length === 0">
                        <td colspan="5" style="text-align: center; padding: 12px; color: #7f8c8d;">
                          No hay riesgos registrados para esta póliza.
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Recibos card -->
              <div class="details-card table-card">
                <header class="card-header">
                  <h2>Recibos</h2>
                  <div class="card-toolbar">
                    <button type="button"><i class="pi pi-info-circle"></i></button>
                  </div>
                </header>
                <div class="table-scroll">
                  <table class="grid-table">
                    <thead>
                      <tr>
                        <th>Recibo Cía.</th>
                        <th>Situación</th>
                        <th>Sit. Cía.</th>
                        <th>Sit. Colab.</th>
                        <th>Tipo recibo</th>
                        <th>Gestor</th>
                        <th>Prima total</th>
                        <th>Efecto</th>
                        <th>Vto.</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="recibo in activePolicy.recibos" :key="recibo.id">
                        <td>
                          <a href="#" @click.prevent="navigateToRecibo(recibo.numero)" class="item-link">{{ recibo.numero }}</a>
                        </td>
                        <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estado)">{{ recibo.estado }}</span></td>
                        <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estadoCia)">{{ recibo.estadoCia }}</span></td>
                        <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estadoColab)">{{ recibo.estadoColab }}</span></td>
                        <td class="text-green font-bold">{{ recibo.tipo }}</td>
                        <td>{{ recibo.gestor }}</td>
                        <td>
                          <strong>{{ formatValue(recibo.primaTotal, 'money') }}</strong>
                        </td>
                        <td>{{ formatValue(recibo.fechaEfecto, 'date') }}</td>
                        <td>{{ formatValue(recibo.fechaVencimiento, 'date') }}</td>
                      </tr>
                      <tr v-if="!activePolicy.recibos || activePolicy.recibos.length === 0">
                        <td colspan="9" style="text-align: center; padding: 12px; color: #7f8c8d;">
                          No hay recibos registrados para esta póliza.
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- Right Side Cards: Figuras & Anulación -->
            <div class="lower-right-column">
              <!-- Figuras card -->
              <div class="details-card table-card">
                <header class="card-header">
                  <h2>Figuras</h2>
                  <div class="card-toolbar">
                    <button type="button"><i class="pi pi-plus"></i></button>
                  </div>
                </header>
                <div class="table-scroll">
                  <table class="grid-table">
                    <thead>
                      <tr>
                        <th>Figura</th>
                        <th>F. Alta</th>
                        <th>F. Baja</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td>
                          <strong>{{ activePolicy.cliente.nombre }}</strong> (Tomador)
                        </td>
                        <td>{{ formatValue(activePolicy.vigencia.fechaInicio, 'date') }}</td>
                        <td>-</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>

              <!-- Anulación / Reemplazo card -->
              <div class="details-card form-card">
                <header class="card-header">
                  <h2>Anulación / Reemplazo</h2>
                </header>
                <div class="card-body grid-layout-anulacion">
                  <div class="field-item">
                    <label>Pól. Anterior</label>
                    <select>
                      <option value=""></option>
                    </select>
                  </div>
                  <div class="field-item">
                    <label>Pól. Siguiente</label>
                    <select>
                      <option value=""></option>
                    </select>
                  </div>
                  <div class="field-item">
                    <label>Anulación</label>
                    <select>
                      <option value=""></option>
                    </select>
                  </div>
                  <div class="field-item full-width">
                    <label>Observaciones</label>
                    <textarea
                      rows="2"
                      placeholder="Observaciones adicionales sobre la póliza..."
                    ></textarea>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Dedicated Riesgos Sub-tab -->
        <div v-else-if="activeSubTab === 'riesgos'" class="details-card table-card animate-fadeIn" style="margin: 10px; background: #ffffff; border: 1px solid #ccd7e2; border-radius: 6px; padding: 16px;">
          <header class="card-header" style="margin-bottom: 12px; background: transparent; border-bottom: none; padding: 0;">
            <h2 style="font-size: 1rem; color: #0b5570;">Detalle de Riesgos - Póliza {{ activePolicy.numero }}</h2>
          </header>
          <div class="table-scroll">
            <table class="grid-table">
              <thead>
                <tr>
                  <th>NIF/Documento</th>
                  <th>Descripción del Riesgo</th>
                  <th>Tipo riesgo</th>
                  <th>F. Alta</th>
                  <th>F. Baja</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="riesgo in activePolicy.riesgos" :key="riesgo.id">
                  <td>
                    <a href="#" @click.prevent="navigateToRiesgo(activePolicy.cliente.documento)" class="item-link">
                      <i class="pi pi-id-card"></i> {{ activePolicy.cliente.documento }}
                    </a>
                  </td>
                  <td>{{ riesgo.descripcion }}</td>
                  <td>{{ riesgo.tipoRiesgo || '-' }}</td>
                  <td>{{ formatValue(riesgo.fechaAlta, 'date') }}</td>
                  <td>{{ formatValue(riesgo.fechaBaja, 'date') }}</td>
                </tr>
                <tr v-if="!activePolicy.riesgos || activePolicy.riesgos.length === 0">
                  <td colspan="5" style="text-align: center; padding: 12px; color: #7f8c8d;">
                    No hay riesgos registrados para esta póliza.
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Dedicated Recibos Sub-tab -->
        <div v-else-if="activeSubTab === 'recibos'" class="details-card table-card animate-fadeIn" style="margin: 10px; background: #ffffff; border: 1px solid #ccd7e2; border-radius: 6px; padding: 16px;">
          <header class="card-header" style="margin-bottom: 12px; background: transparent; border-bottom: none; padding: 0;">
            <h2 style="font-size: 1rem; color: #0b5570;">Detalle de Recibos - Póliza {{ activePolicy.numero }}</h2>
          </header>
          <div class="table-scroll">
            <table class="grid-table">
              <thead>
                <tr>
                  <th>Recibo Cía.</th>
                  <th>Situación</th>
                  <th>Sit. Cía.</th>
                  <th>Sit. Colab.</th>
                  <th>Tipo recibo</th>
                  <th>Gestor</th>
                  <th>Prima total</th>
                  <th>Efecto</th>
                  <th>Vto.</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="recibo in activePolicy.recibos" :key="recibo.id">
                  <td>
                    <a href="#" @click.prevent="navigateToRecibo(recibo.numero)" class="item-link">{{ recibo.numero }}</a>
                  </td>
                  <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estado)">{{ recibo.estado }}</span></td>
                  <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estadoCia)">{{ recibo.estadoCia }}</span></td>
                  <td><span class="status-pill" :class="getReciboEstadoClass(recibo.estadoColab)">{{ recibo.estadoColab }}</span></td>
                  <td class="text-green font-bold">{{ recibo.tipo }}</td>
                  <td>{{ recibo.gestor }}</td>
                  <td>
                    <strong>{{ formatValue(recibo.primaTotal, 'money') }}</strong>
                  </td>
                  <td>{{ formatValue(recibo.fechaEfecto, 'date') }}</td>
                  <td>{{ formatValue(recibo.fechaVencimiento, 'date') }}</td>
                </tr>
                <tr v-if="!activePolicy.recibos || activePolicy.recibos.length === 0">
                  <td colspan="9" style="text-align: center; padding: 12px; color: #7f8c8d;">
                    No hay recibos registrados para esta póliza.
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Mocks for other sub-tabs -->
        <div v-else class="details-card tab-placeholder-card animate-fadeIn">
          <p class="state">
            <i class="pi pi-info-circle"></i> La sección
            <strong>{{ activeSubTab.toUpperCase() }}</strong> se encuentra totalmente operativa
            vinculada al ID de póliza <strong>{{ activePolicy.numero }}</strong>.
          </p>
        </div>
      </div>
    </section>
  </main>
</template>

<style scoped lang="scss">
@import './PolizasView.scss';
</style>
