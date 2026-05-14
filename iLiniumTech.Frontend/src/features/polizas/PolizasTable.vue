<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink } from 'vue-router'

import { polizasTableColumns, type PolizaTableColumn } from './polizasConstants'
import { POLIZA_EMPTY_VALUE, formatPolizaValue } from './polizasFormatters'
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
  retry: []
}>()

const pageSizeOptions = [10, 25, 50]
const loadingRows = Array.from({ length: 5 }, (_, index) => index)
const totalPages = computed(() => Math.max(1, Math.ceil(props.total / props.pageSize)))
const firstVisible = computed(() => (props.total === 0 ? 0 : (props.page - 1) * props.pageSize + 1))
const lastVisible = computed(() => Math.min(props.page * props.pageSize, props.total))
const canGoPrevious = computed(() => props.page > 1 && !props.loading)
const canGoNext = computed(() => props.page < totalPages.value && !props.loading)
const resultLabel = computed(() => (props.total === 1 ? 'poliza' : 'polizas'))

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

function retrySearch() {
  if (!props.loading) {
    emit('retry')
  }
}
</script>

<template>
  <section
    class="results-summary"
    aria-labelledby="polizas-results-title"
    :aria-busy="loading"
    aria-live="polite"
  >
    <div class="summary-header">
      <h2 id="polizas-results-title">Resultado</h2>
      <span>
        <strong>{{ total }}</strong> {{ resultLabel }}
      </span>
      <span>{{ firstVisible }}-{{ lastVisible }} visibles</span>
    </div>

    <div v-if="loading" class="table-scroll" role="status" aria-label="Cargando polizas">
      <table>
        <thead>
          <tr>
            <th v-for="column in polizasTableColumns" :key="column.key" scope="col">
              {{ column.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in loadingRows" :key="row" class="skeleton-row">
            <td v-for="column in polizasTableColumns" :key="column.key">
              <span class="skeleton-cell">{{ POLIZA_EMPTY_VALUE }}</span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div v-else-if="error" class="state state-box error" role="alert">
      <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
      <div>
        <strong>No se pudo obtener el listado</strong>
        <p>{{ error }}</p>
      </div>
      <button type="button" class="state-action" @click="retrySearch">
        <i class="pi pi-refresh" aria-hidden="true"></i>
        Reintentar
      </button>
    </div>

    <div v-else-if="items.length === 0" class="state state-box empty" role="status">
      <i class="pi pi-inbox" aria-hidden="true"></i>
      <div>
        <strong>Sin resultados</strong>
        <p>No hay polizas para los filtros actuales.</p>
      </div>
    </div>

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
        <label for="polizas-page-size">Filas</label>
        <select
          id="polizas-page-size"
          :value="pageSize"
          :disabled="loading"
          @change="changePageSize"
        >
          <option v-for="option in pageSizeOptions" :key="option" :value="option">
            {{ option }}
          </option>
        </select>
      </div>

      <div class="page-controls">
        <button type="button" :disabled="!canGoPrevious" @click="changePage(page - 1)">
          <i class="pi pi-chevron-left" aria-hidden="true"></i>
          Anterior
        </button>
        <span>Pagina {{ page }} de {{ totalPages }}</span>
        <button type="button" :disabled="!canGoNext" @click="changePage(page + 1)">
          Siguiente
          <i class="pi pi-chevron-right" aria-hidden="true"></i>
        </button>
      </div>
    </footer>
  </section>
</template>
