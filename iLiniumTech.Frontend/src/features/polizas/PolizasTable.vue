<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'

import {
  polizasTableColumns,
  type PolizaTableColumn,
  type PolizaValueType,
} from './polizasConstants'
import type { PolizaListItem } from './polizasTypes'

const props = defineProps<{
  items: PolizaListItem[]
  total: number
  loading: boolean
  error: string | null
  page: number
  pageSize: number
}>()

const emit = defineEmits<{
  'page-change': [page: number]
  'page-size-change': [pageSize: number]
}>()

const pageSizeOptions = [10, 25, 50]
const totalPages = computed(() => Math.max(1, Math.ceil(props.total / props.pageSize)))
const firstVisible = computed(() => (props.total === 0 ? 0 : (props.page - 1) * props.pageSize + 1))
const lastVisible = computed(() => Math.min(props.page * props.pageSize, props.total))
const canGoPrevious = computed(() => props.page > 1 && !props.loading)
const canGoNext = computed(() => props.page < totalPages.value && !props.loading)

function formatPolizaValue(value: unknown, type: PolizaValueType, currency = 'EUR') {
  if (value === null || value === undefined || value === '') {
    return '-'
  }

  if (type === 'money' && typeof value === 'number') {
    return new Intl.NumberFormat('es-ES', {
      style: 'currency',
      currency,
      maximumFractionDigits: 2,
    }).format(value)
  }

  if (type === 'date' && typeof value === 'string') {
    return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
  }

  return String(value)
}

function tableCellValue(item: PolizaListItem, column: PolizaTableColumn) {
  return item[column.key]
}

function changePage(page: number) {
  if (page >= 1 && page <= totalPages.value) {
    emit('page-change', page)
  }
}

function changePageSize(event: Event) {
  emit('page-size-change', Number((event.target as HTMLSelectElement).value))
}
</script>

<template>
  <section class="results-summary" aria-live="polite">
    <div class="summary-header">
      <span>
        Resultado: <strong>{{ total }}</strong> polizas
      </span>
      <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
    </div>

    <p v-if="loading" class="state">Cargando polizas...</p>
    <p v-else-if="error" class="state error">{{ error }}</p>
    <p v-else-if="items.length === 0" class="state">No hay polizas para los filtros actuales.</p>

    <div v-else class="table-scroll">
      <table>
        <thead>
          <tr>
            <th v-for="column in polizasTableColumns" :key="column.key" scope="col">
              {{ column.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td v-for="column in polizasTableColumns" :key="column.key">
              <RouterLink
                v-if="column.key === 'numero'"
                class="table-link"
                :to="{ name: 'poliza-detail', params: { id: item.id } }"
              >
                {{ formatPolizaValue(tableCellValue(item, column), column.type, item.moneda) }}
              </RouterLink>
              <span v-else>
                {{ formatPolizaValue(tableCellValue(item, column), column.type, item.moneda) }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <footer class="pagination-bar" aria-label="Paginacion de polizas">
      <div class="page-size-control">
        <span>Filas</span>
        <select :value="pageSize" :disabled="loading" @change="changePageSize">
          <option v-for="option in pageSizeOptions" :key="option" :value="option">
            {{ option }}
          </option>
        </select>
      </div>

      <div class="page-controls">
        <button type="button" :disabled="!canGoPrevious" @click="changePage(page - 1)">
          <i class="pi pi-chevron-left"></i>
          Anterior
        </button>
        <span>Pagina {{ page }} de {{ totalPages }}</span>
        <button type="button" :disabled="!canGoNext" @click="changePage(page + 1)">
          Siguiente
          <i class="pi pi-chevron-right"></i>
        </button>
      </div>
    </footer>
  </section>
</template>
