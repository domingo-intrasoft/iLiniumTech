<script setup lang="ts">
import { reactive } from 'vue'

import { usePolizas } from './usePolizas'

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
    return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
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
  filters.cliente =
    searchValue('nombreCompleto') || searchValue('nombre') || searchValue('documento')
  filters.estado = searchValue('tipoPoliza')
  await refresh()
}

function clearFilters() {
  Object.keys(searchValues).forEach((key) => {
    searchValues[key] = ''
  })
  filters.numero = ''
  filters.cliente = ''
  filters.estado = ''
}
</script>

<template>
  <main class="ilinium-shell">
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

    <section class="workspace">
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

      <div class="tab-strip">
        <button type="button" class="tab active"><i class="pi pi-search"></i></button>
        <button type="button" class="tab"><i class="pi pi-table"></i></button>
        <strong>({{ total }})</strong>
      </div>

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

      <section class="results-summary" aria-live="polite">
        <div class="summary-header">
          <span>
            Resultado MVP: <strong>{{ total }}</strong> pólizas
          </span>
          <span v-if="metadata">
            AppBuilder {{ metadata.appBuilder.rootComponentId }} /
            {{ metadata.appBuilder.crudComponentId }} / DS {{ metadata.appBuilder.dataSourceId }}
          </span>
        </div>

        <p v-if="loading" class="state">Cargando pólizas...</p>
        <p v-else-if="error" class="state error">{{ error }}</p>
        <p v-else-if="items.length === 0" class="state">
          No hay pólizas para los filtros actuales.
        </p>

        <div v-else class="table-scroll">
          <table>
            <thead>
              <tr>
                <th v-for="field in visibleFields" :key="field.name" scope="col">
                  {{ field.label }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in items" :key="item.id">
                <td v-for="field in visibleFields" :key="field.name">
                  {{ formatValue(item[field.name as keyof typeof item], field.type) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </section>
  </main>
</template>
