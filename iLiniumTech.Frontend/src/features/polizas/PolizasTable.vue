<script setup lang="ts">
import { RouterLink } from 'vue-router'

import {
  polizasTableColumns,
  type PolizaTableColumn,
  type PolizaValueType,
} from './polizasConstants'
import type { PolizaListItem } from './polizasTypes'

defineProps<{
  items: PolizaListItem[]
  total: number
  loading: boolean
  error: string | null
}>()

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
</script>

<template>
  <section class="results-summary" aria-live="polite">
    <div class="summary-header">
      <span>
        Resultado: <strong>{{ total }}</strong> polizas
      </span>
      <span>{{ items.length }} visibles</span>
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
  </section>
</template>
