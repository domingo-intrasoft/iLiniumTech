<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, type LocationQueryRaw } from 'vue-router'

import { polizasTableColumns, type PolizaTableColumn } from './polizasConstants'
import { POLIZA_EMPTY_VALUE, formatPolizaValue } from './polizasFormatters'
import { POLIZA_MVP_PREFIX, type PolizaListItem } from './polizasTypes'

const props = withDefaults(
  defineProps<{
    items: PolizaListItem[]
    total: number
    loading: boolean
    error: string | null
    page: number
    pageSize: number
    detailQuery?: LocationQueryRaw
    canOpenDetail?: boolean
    detailUnavailableMessage?: string | null
    canUpdateMvp?: boolean
    canDeleteMvp?: boolean
    writeBusyId?: string | null
  }>(),
  {
    detailQuery: undefined,
    canOpenDetail: true,
    detailUnavailableMessage: null,
    canUpdateMvp: false,
    canDeleteMvp: false,
    writeBusyId: null,
  },
)

const emit = defineEmits<{
  'page-change': [page: number]
  'page-size-change': [pageSize: number]
  edit: [item: PolizaListItem]
  delete: [item: PolizaListItem]
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
const canOpenDetail = computed(() => props.canOpenDetail ?? true)
const detailUnavailableMessageId = 'polizas-detail-unavailable-message'
const detailUnavailableMessage = computed(() =>
  canOpenDetail.value ? null : props.detailUnavailableMessage,
)
const writeUnavailableMessageId = 'polizas-write-unavailable-message'
const writesAllowed = computed(() => props.canUpdateMvp || props.canDeleteMvp)
const tableCaption = computed(
  () =>
    `Listado de polizas. Mostrando ${firstVisible.value}-${lastVisible.value} de ${props.total}.`,
)

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

function detailRoute(item: PolizaListItem) {
  return {
    name: 'poliza-detail',
    params: { id: item.id },
    query: props.detailQuery,
  }
}

function hasStableNumericId(item: PolizaListItem) {
  return /^[1-9]\d*$/.test(item.id)
}

function isMvpEditable(item: PolizaListItem) {
  return item.numero.startsWith(POLIZA_MVP_PREFIX) && hasStableNumericId(item)
}

function canEditItem(item: PolizaListItem) {
  return props.canUpdateMvp && isMvpEditable(item) && props.writeBusyId === null
}

function canDeleteItem(item: PolizaListItem) {
  return props.canDeleteMvp && isMvpEditable(item) && props.writeBusyId === null
}

function writeActionTitle(item: PolizaListItem, action: 'edit' | 'delete') {
  if (props.writeBusyId === item.id) {
    return 'Operacion en curso'
  }

  if (!isMvpEditable(item)) {
    return `Solo registros ${POLIZA_MVP_PREFIX} con id estable pueden modificarse.`
  }

  if (action === 'edit' && !props.canUpdateMvp) {
    return 'La sesion no tiene permiso de edicion.'
  }

  if (action === 'delete' && !props.canDeleteMvp) {
    return 'La sesion no tiene permiso de borrado.'
  }

  return action === 'edit' ? 'Editar poliza MVP' : 'Eliminar poliza MVP'
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

    <p
      v-if="detailUnavailableMessage"
      :id="detailUnavailableMessageId"
      class="results-permission-warning"
      role="status"
    >
      <i class="pi pi-lock" aria-hidden="true"></i>
      {{ detailUnavailableMessage }}
    </p>

    <p v-if="writesAllowed" :id="writeUnavailableMessageId" class="results-permission-warning">
      <i class="pi pi-shield" aria-hidden="true"></i>
      Escritura limitada a registros {{ POLIZA_MVP_PREFIX }}.
    </p>

    <div v-if="loading" class="table-scroll" role="status" aria-label="Cargando polizas">
      <span class="sr-only">Cargando polizas.</span>
      <table>
        <caption class="sr-only">
          Cargando listado de polizas.
        </caption>
        <thead>
          <tr>
            <th scope="col">Acciones</th>
            <th v-for="column in polizasTableColumns" :key="column.key" scope="col">
              {{ column.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in loadingRows" :key="row" class="skeleton-row">
            <td>
              <span class="skeleton-cell compact">{{ POLIZA_EMPTY_VALUE }}</span>
            </td>
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
        <caption class="sr-only" v-text="tableCaption"></caption>
        <thead>
          <tr>
            <th scope="col">Acciones</th>
            <th v-for="column in polizasTableColumns" :key="column.key" scope="col">
              {{ column.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td>
              <RouterLink
                v-if="canOpenDetail"
                class="table-icon-action"
                :to="detailRoute(item)"
                :aria-label="`Ver detalle de poliza ${item.numero}`"
                title="Ver detalle"
              >
                <i class="pi pi-eye" aria-hidden="true"></i>
              </RouterLink>
              <button
                v-else
                class="table-icon-action"
                type="button"
                :aria-label="`Detalle no disponible para poliza ${item.numero}`"
                :aria-describedby="
                  detailUnavailableMessage ? detailUnavailableMessageId : undefined
                "
                title="Detalle no disponible"
                disabled
              >
                <i class="pi pi-eye-slash" aria-hidden="true"></i>
              </button>
              <button
                class="table-icon-action write-action"
                type="button"
                :aria-label="`Editar poliza ${item.numero}`"
                :aria-describedby="
                  writesAllowed && !canEditItem(item) ? writeUnavailableMessageId : undefined
                "
                :title="writeActionTitle(item, 'edit')"
                :disabled="!canEditItem(item)"
                @click="emit('edit', item)"
              >
                <i class="pi pi-pencil" aria-hidden="true"></i>
              </button>
              <button
                class="table-icon-action write-action danger"
                type="button"
                :aria-label="`Eliminar poliza ${item.numero}`"
                :aria-describedby="
                  writesAllowed && !canDeleteItem(item) ? writeUnavailableMessageId : undefined
                "
                :title="writeActionTitle(item, 'delete')"
                :disabled="!canDeleteItem(item)"
                @click="emit('delete', item)"
              >
                <i class="pi pi-trash" aria-hidden="true"></i>
              </button>
            </td>
            <td v-for="column in polizasTableColumns" :key="column.key">
              <RouterLink
                v-if="column.key === 'numero' && canOpenDetail"
                class="table-link"
                :to="detailRoute(item)"
              >
                {{ formatPolizaValue(tableCellValue(item, column), column.type, item.moneda) }}
              </RouterLink>
              <span v-else-if="column.key === 'numero'">
                {{ formatPolizaValue(tableCellValue(item, column), column.type, item.moneda) }}
              </span>
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
