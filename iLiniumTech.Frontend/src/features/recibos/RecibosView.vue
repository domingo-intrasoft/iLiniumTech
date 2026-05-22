<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const queryNumero = computed(() => (route.query.numero as string) || '')

interface ShellAction {
  label: string
  icon: string
  active?: boolean
}

const sideItems: ShellAction[] = [
  { label: 'Agenda', icon: 'pi pi-calendar' },
  { label: 'Clientes', icon: 'pi pi-user' },
  { label: 'Propuestas', icon: 'pi pi-folder-open' },
  { label: 'Pólizas', icon: 'pi pi-briefcase' },
  { label: 'Recibos', icon: 'pi pi-money-bill', active: true },
  { label: 'Suplementos', icon: 'pi pi-link' },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle' },
  { label: 'Riesgos', icon: 'pi pi-truck' },
  { label: 'Liq.Cia', icon: 'pi pi-list' },
  { label: 'Liq.Col', icon: 'pi pi-list-check' },
  { label: 'Informes', icon: 'pi pi-file' },
]

interface ReciboData {
  id: string
  numero: string
  polizaId: string
  polizaNumero: string
  clienteNombre: string
  estado: string
  estadoCia: string
  estadoColab: string
  tipo: string
  gestor: string
  primaTotal: number
  fechaEfecto: string
  fechaVencimiento: string
}

const allRecibos = ref<ReciboData[]>([
  {
    id: '72143',
    numero: '251436851',
    polizaId: '24',
    polizaNumero: '26331455',
    clienteNombre: 'Ramos Fraga, Juan Carlos',
    estado: 'Cobrado',
    estadoCia: 'Liquidado',
    estadoColab: 'Liquidado',
    tipo: 'Cartera',
    gestor: 'Compañía',
    primaTotal: 609.61,
    fechaEfecto: '2020-01-02',
    fechaVencimiento: '2021-01-02',
  },
  {
    id: '8769',
    numero: '109283741',
    polizaId: '24',
    polizaNumero: '26331455',
    clienteNombre: 'Ramos Fraga, Juan Carlos',
    estado: 'Cobrado',
    estadoCia: 'Pendiente',
    estadoColab: 'Pendiente',
    tipo: 'Nueva Producción',
    gestor: 'Compañía',
    primaTotal: 450.0,
    fechaEfecto: '2014-01-02',
    fechaVencimiento: '2015-01-02',
  },
  {
    id: '4413',
    numero: '982736151',
    polizaId: '3',
    polizaNumero: '41059253',
    clienteNombre: 'Hernandez Gila, Lara',
    estado: 'Cobrado',
    estadoCia: 'Liquidado',
    estadoColab: 'Liquidado',
    tipo: 'Cartera',
    gestor: 'Correduría',
    primaTotal: 609.61,
    fechaEfecto: '2023-01-01',
    fechaVencimiento: '2024-01-01',
  },
  {
    id: '92134',
    numero: '887263541',
    polizaId: '35',
    polizaNumero: '55667788',
    clienteNombre: 'Domingo Cabeza',
    estado: 'Pendiente',
    estadoCia: 'Pendiente',
    estadoColab: 'Pendiente',
    tipo: 'Suplemento',
    gestor: 'Compañía',
    primaTotal: 125.5,
    fechaEfecto: '2022-01-02',
    fechaVencimiento: '2023-01-02',
  }
])

const filterNumero = ref('')

onMounted(() => {
  if (queryNumero.value) {
    filterNumero.value = queryNumero.value
  }
})

const filteredRecibos = computed(() => {
  if (!filterNumero.value.trim()) {
    return allRecibos.value
  }
  const query = filterNumero.value.toLowerCase()
  return allRecibos.value.filter(
    (r) => r.numero.toLowerCase().includes(query) || r.clienteNombre.toLowerCase().includes(query)
  )
})

function clearFilter() {
  filterNumero.value = ''
  router.replace({ query: {} })
}

function navigateToPolicy(polizaId: string) {
  router.push(`/polizas?id=${polizaId}`)
}

function formatValue(value: number, type: string) {
  if (type === 'money') {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency: 'EUR',
    }).format(value)
  }
  return String(value)
}

function getReciboEstadoClass(estado: string) {
  if (!estado) return ''
  const est = estado.toLowerCase()
  if (est.includes('cobrado') || est.includes('liquidado')) {
    return 'status-green'
  }
  if (est.includes('devuelto') || est.includes('anulado')) {
    return 'status-red'
  }
  if (est.includes('pendiente')) {
    return 'status-blue'
  }
  return ''
}
</script>

<template>
  <main class="ilinium-shell">
    <!-- Main Left Sidebar Menu -->
    <aside class="il-sidebar" aria-label="Menu principal">
      <div class="broker-logo"><span></span>AUXFISE</div>
      <nav class="side-nav">
        <router-link
          v-for="item in sideItems"
          :key="item.label"
          :to="item.label === 'Pólizas' ? '/polizas' : item.label === 'Recibos' ? '/recibos' : item.label === 'Riesgos' ? '/riesgos' : '#'"
          :class="{ active: item.active }"
        >
          <i :class="item.icon"></i>
          <span>{{ item.label }}</span>
        </router-link>
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
          <strong>Recibos</strong>
          <span>/</span>
        </div>

        <span class="environment-badge">Aunna Tech | Portal (DEMO)</span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <span class="user-name">Domingo () (-1)</span>
          <button class="icon-button ghost" type="button" aria-label="Salir">
            <i class="pi pi-sign-out"></i>
          </button>
        </div>
      </header>

      <div class="module-title-bar animate-fadeIn">
        <h1>Búsqueda y Gestión de Recibos</h1>
        <button class="primary-action" @click="router.push('/polizas')">
          <i class="pi pi-arrow-left"></i> Volver a Pólizas
        </button>
      </div>

      <div class="tab-content-search animate-fadeIn">
        <section class="search-panel" aria-label="Búsqueda de recibos">
          <div class="criteria-card">
            <section class="filter-section">
              <div class="filter-section-title">
                <h2>Filtrar Recibo</h2>
              </div>
              <div class="filter-row">
                <label class="filter-field" style="grid-column: span 6">
                  <span>Número de Recibo Cía. / Nombre Tomador</span>
                  <span class="field-control">
                    <input v-model="filterNumero" type="search" placeholder="Escriba número o tomador..." />
                    <button type="button" v-if="filterNumero" @click="clearFilter" title="Limpiar filtro">
                      <i class="pi pi-times"></i>
                    </button>
                  </span>
                </label>
              </div>
            </section>
          </div>
        </section>

        <!-- Results List Grid -->
        <section class="results-summary">
          <div class="summary-header">
            <span>
              Resultado: <strong>{{ filteredRecibos.length }}</strong> recibos asociados.
            </span>
          </div>

          <div v-if="filteredRecibos.length === 0" class="state-container">
            <p class="state">No se han encontrado recibos con este criterio.</p>
          </div>

          <div v-else class="table-scroll">
            <table class="premium-table">
              <thead>
                <tr>
                  <th>Nº Recibo Cía.</th>
                  <th>Tomador</th>
                  <th>Nº Póliza</th>
                  <th>Situación</th>
                  <th>Sit. Cía.</th>
                  <th>Sit. Colab.</th>
                  <th>Tipo</th>
                  <th>Gestor</th>
                  <th>Prima Total</th>
                  <th>F. Efecto</th>
                  <th>F. Vencimiento</th>
                  <th style="text-align: center">Ver Póliza</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="recibo in filteredRecibos"
                  :key="recibo.id"
                  :class="{ 'highlighted-row': queryNumero === recibo.numero }"
                >
                  <td class="font-bold text-blue">{{ recibo.numero }}</td>
                  <td>{{ recibo.clienteNombre }}</td>
                  <td>
                    <button class="poliza-link-btn" @click="navigateToPolicy(recibo.polizaId)">
                      Nº {{ recibo.polizaNumero }} <i class="pi pi-external-link"></i>
                    </button>
                  </td>
                  <td>
                    <span class="status-pill" :class="getReciboEstadoClass(recibo.estado)">{{ recibo.estado }}</span>
                  </td>
                  <td>
                    <span class="status-pill" :class="getReciboEstadoClass(recibo.estadoCia)">{{ recibo.estadoCia }}</span>
                  </td>
                  <td>
                    <span class="status-pill" :class="getReciboEstadoClass(recibo.estadoColab)">{{ recibo.estadoColab }}</span>
                  </td>
                  <td :class="recibo.tipo.includes('Cartera') ? 'text-blue font-bold' : 'text-green font-bold'">
                    {{ recibo.tipo }}
                  </td>
                  <td>{{ recibo.gestor }}</td>
                  <td class="font-bold">{{ formatValue(recibo.primaTotal, 'money') }}</td>
                  <td>{{ recibo.fechaEfecto }}</td>
                  <td>{{ recibo.fechaVencimiento }}</td>
                  <td style="text-align: center">
                    <button class="open-detail-action" type="button" @click="navigateToPolicy(recibo.polizaId)" title="Ver póliza asociada">
                      <i class="pi pi-briefcase"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </div>
    </section>
  </main>
</template>

<style scoped lang="scss">
@import '../polizas/PolizasView.scss';

.module-title-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 20px;
  background: #f0f4f8;
  border-bottom: 1px solid #c9d7e6;

  h1 {
    font-size: 1.25rem;
    font-weight: 800;
    color: #0b5570;
    margin: 0;
  }
}

.poliza-link-btn {
  background: transparent;
  border: 0;
  color: #0b5570;
  font-weight: 700;
  text-decoration: underline;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 4px;

  &:hover {
    color: #ff6b00;
  }
}

.highlighted-row {
  background: #edf8ff !important;
  border: 1px solid #99d2ff;
}
</style>
