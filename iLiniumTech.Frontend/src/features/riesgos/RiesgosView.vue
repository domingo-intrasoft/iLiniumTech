<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const queryDocumento = computed(() => (route.query.documento as string) || '')

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
  { label: 'Recibos', icon: 'pi pi-money-bill' },
  { label: 'Suplementos', icon: 'pi pi-link' },
  { label: 'Siniestros', icon: 'pi pi-exclamation-triangle' },
  { label: 'Riesgos', icon: 'pi pi-truck', active: true },
  { label: 'Liq.Cia', icon: 'pi pi-list' },
  { label: 'Liq.Col', icon: 'pi pi-list-check' },
  { label: 'Informes', icon: 'pi pi-file' },
]

interface RiesgoData {
  id: string
  polizaId: string
  polizaNumero: string
  clienteNombre: string
  documento: string
  descripcion: string
  tipo: string
  fechaAlta: string
  estado: string
}

const allRiesgos = ref<RiesgoData[]>([
  {
    id: 'R-1',
    polizaId: '24',
    polizaNumero: '26331455',
    clienteNombre: 'Ramos Fraga, Juan Carlos',
    documento: '52017408X',
    descripcion: 'Turismo Seat Ibiza 1.6 TDI',
    tipo: 'Autos Cat.1 (Turismos)',
    fechaAlta: '2020-01-02',
    estado: 'Vigor',
  },
  {
    id: 'R-2',
    polizaId: '3',
    polizaNumero: '41059253',
    clienteNombre: 'Hernandez Gila, Lara',
    documento: '52017408X',
    descripcion: 'Vivienda Principal Guadalajara',
    tipo: 'Multirriesgos (Hogar)',
    fechaAlta: '2023-01-01',
    estado: 'Vigor',
  },
  {
    id: 'R-3',
    polizaId: '45',
    polizaNumero: '98273611',
    clienteNombre: 'Domingo Cabeza',
    documento: '12345678A',
    descripcion: 'Veh. Comercial Furgón Ford Transit',
    tipo: 'Autos Cat.1 (Furgoneta)',
    fechaAlta: '2021-06-15',
    estado: 'Vigor',
  },
  {
    id: 'R-4',
    polizaId: '12',
    polizaNumero: '10928374',
    clienteNombre: 'Jiménez López, Pedro',
    documento: '98765432B',
    descripcion: 'Seguro de Vida - Cobertura Principal',
    tipo: 'Vida P.P. (Asegurado)',
    fechaAlta: '2019-11-01',
    estado: 'Vigor',
  }
])

const filterDocumento = ref('')

onMounted(() => {
  if (queryDocumento.value) {
    filterDocumento.value = queryDocumento.value
  }
})

const filteredRiesgos = computed(() => {
  if (!filterDocumento.value.trim()) {
    return allRiesgos.value
  }
  const query = filterDocumento.value.toLowerCase()
  return allRiesgos.value.filter(
    (r) => r.documento.toLowerCase().includes(query) || r.clienteNombre.toLowerCase().includes(query)
  )
})

function clearFilter() {
  filterDocumento.value = ''
  router.replace({ query: {} })
}

function navigateToPolicy(polizaId: string) {
  router.push(`/polizas?id=${polizaId}`)
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
          <strong>Riesgos</strong>
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
        <h1>Búsqueda y Gestión de Riesgos</h1>
        <button class="primary-action" @click="router.push('/polizas')">
          <i class="pi pi-arrow-left"></i> Volver a Pólizas
        </button>
      </div>

      <div class="tab-content-search animate-fadeIn">
        <section class="search-panel" aria-label="Búsqueda de riesgos">
          <div class="criteria-card">
            <section class="filter-section">
              <div class="filter-section-title">
                <h2>Filtrar Riesgo</h2>
              </div>
              <div class="filter-row">
                <label class="filter-field" style="grid-column: span 6">
                  <span>Documento tomador / NIF / CIF</span>
                  <span class="field-control">
                    <input v-model="filterDocumento" type="search" placeholder="Escriba documento o NIF..." />
                    <button type="button" v-if="filterDocumento" @click="clearFilter" title="Limpiar filtro">
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
              Resultado: <strong>{{ filteredRiesgos.length }}</strong> riesgos asociados.
            </span>
          </div>

          <div v-if="filteredRiesgos.length === 0" class="state-container">
            <p class="state">No se han encontrado riesgos con este documento.</p>
          </div>

          <div v-else class="table-scroll">
            <table class="premium-table">
              <thead>
                <tr>
                  <th>NIF/CIF</th>
                  <th>Tomador</th>
                  <th>Descripción del Riesgo</th>
                  <th>Tipo Riesgo</th>
                  <th>Nº Póliza</th>
                  <th>F. Alta</th>
                  <th>Estado</th>
                  <th style="text-align: center">Acciones</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="riesgo in filteredRiesgos"
                  :key="riesgo.id"
                  :class="{ 'highlighted-row': queryDocumento === riesgo.documento }"
                >
                  <td class="font-bold">{{ riesgo.documento }}</td>
                  <td>{{ riesgo.clienteNombre }}</td>
                  <td>
                    <span class="risk-desc-tag">
                      <i class="pi pi-car" v-if="riesgo.tipo.includes('Autos')"></i>
                      <i class="pi pi-home" v-else-if="riesgo.tipo.includes('Hogar')"></i>
                      <i class="pi pi-user" v-else></i>
                      {{ riesgo.descripcion }}
                    </span>
                  </td>
                  <td>{{ riesgo.tipo }}</td>
                  <td>
                    <button class="poliza-link-btn" @click="navigateToPolicy(riesgo.polizaId)">
                      Nº {{ riesgo.polizaNumero }} <i class="pi pi-external-link"></i>
                    </button>
                  </td>
                  <td>{{ riesgo.fechaAlta }}</td>
                  <td>
                    <span class="status-pill status-green">{{ riesgo.estado }}</span>
                  </td>
                  <td style="text-align: center">
                    <button class="open-detail-action" type="button" @click="navigateToPolicy(riesgo.polizaId)" title="Ver póliza asociada">
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

.risk-desc-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;

  i {
    color: #0b5570;
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
  background: #fff9e6 !important;
  border: 1px solid #ffe899;
}
</style>
