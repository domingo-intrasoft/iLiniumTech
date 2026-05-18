<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, type LocationQueryRaw } from 'vue-router'

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
const appBuilderColumns = [
  { key: 'selector', label: '', kind: 'selector' },
  { key: 'actions', label: '', kind: 'actions' },
  { key: 'compania', label: 'Cia.', kind: 'field' },
  { key: 'numero', label: 'Poliza', kind: 'poliza' },
  { key: 'certificado', label: 'Certif.', kind: 'missing' },
  { key: 'documento', label: 'N. Documento', kind: 'field' },
  { key: 'clienteNombre', label: 'Cliente', kind: 'field' },
  { key: 'estado', label: 'Situacion', kind: 'status' },
  { key: 'ramo', label: 'Ramo', kind: 'field' },
  { key: 'riesgo', label: 'Riesgo/Matric.', kind: 'field' },
] as const
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

function appBuilderCellValue(item: PolizaListItem, key: string) {
  if (key === 'compania') {
    return item.compania
  }

  if (key === 'numero') {
    return item.numero
  }

  if (key === 'clienteNombre') {
    return item.clienteNombre
  }

  if (key === 'documento') {
    return item.documento
  }

  if (key === 'estado') {
    return item.estado
  }

  if (key === 'ramo') {
    return item.ramo
  }

  if (key === 'riesgo') {
    return item.riesgo
  }

  return ''
}

function normalizedStatus(value: string) {
  return value.trim().toLocaleLowerCase('es-ES')
}

function isActiveStatus(value: string) {
  const normalized = normalizedStatus(value)

  return (
    normalized === 'vigor' ||
    normalized === 'en vigor' ||
    normalized === 'ev' ||
    normalized === 'situacionpoliza-ev' ||
    normalized.endsWith('-ev')
  )
}

function statusLabel(value: string) {
  if (isActiveStatus(value)) {
    return 'En Vigor'
  }

  return formatPolizaValue(value, 'string')
}

function statusClass(value: string) {
  return {
    'policy-status-badge': true,
    'status-active': isActiveStatus(value),
    'status-muted': !isActiveStatus(value),
  }
}

function formattedCellValue(item: PolizaListItem, key: string) {
  return formatPolizaValue(appBuilderCellValue(item, key), 'string', item.moneda)
}

function isPendingCellValue(item: PolizaListItem, key: string) {
  return formattedCellValue(item, key) === POLIZA_EMPTY_VALUE
}

function companyLogoLabel(item: PolizaListItem) {
  const value = formattedCellValue(item, 'compania')

  if (value === POLIZA_EMPTY_VALUE) {
    return 'CIA'
  }

  const normalized = value.trim()

  if (/^-?\d+$/.test(normalized)) {
    return `CIA ${normalized.replace('-', '')}`
  }

  return normalized.length > 12 ? normalized.slice(0, 12) : normalized
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

function showEditAction(item: PolizaListItem) {
  return canEditItem(item) || props.writeBusyId === item.id
}

function showDeleteAction(item: PolizaListItem) {
  return canDeleteItem(item) || props.writeBusyId === item.id
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
    class="results-summary polizas-results-grid"
    aria-labelledby="polizas-results-title"
    :aria-busy="loading"
    aria-live="polite"
  >
    <div class="summary-header">
      <h2 id="polizas-results-title">Polizas</h2>
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

    <p
      v-if="writesAllowed"
      :id="writeUnavailableMessageId"
      class="results-permission-warning sr-only"
    >
      <i class="pi pi-shield" aria-hidden="true"></i>
      Escritura limitada a registros {{ POLIZA_MVP_PREFIX }}.
    </p>

    <div v-if="loading" class="table-scroll" role="status" aria-label="Cargando polizas">
      <span class="sr-only">Cargando polizas.</span>
      <table class="appbuilder-table">
        <caption class="sr-only">
          Cargando listado de polizas.
        </caption>
        <thead>
          <tr>
            <th v-for="column in appBuilderColumns" :key="column.key" scope="col">
              <span v-if="column.label" class="column-heading">
                <span>{{ column.label }}</span>
                <i class="pi pi-filter" aria-hidden="true"></i>
              </span>
              <span v-else class="sr-only">
                {{ column.kind === 'selector' ? 'Seleccion' : 'Acciones' }}
              </span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in loadingRows" :key="row" class="skeleton-row">
            <td v-for="column in appBuilderColumns" :key="column.key">
              <span
                class="skeleton-cell"
                :class="{ compact: column.kind === 'selector' || column.kind === 'actions' }"
              >
                {{ POLIZA_EMPTY_VALUE }}
              </span>
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
      <table class="appbuilder-table">
        <caption class="sr-only" v-text="tableCaption"></caption>
        <thead>
          <tr>
            <th v-for="column in appBuilderColumns" :key="column.key" scope="col">
              <span v-if="column.label" class="column-heading">
                <span>{{ column.label }}</span>
                <i class="pi pi-filter" aria-hidden="true"></i>
              </span>
              <span v-else class="sr-only">
                {{ column.kind === 'selector' ? 'Seleccion' : 'Acciones' }}
              </span>
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in items" :key="item.id">
            <td v-for="column in appBuilderColumns" :key="column.key">
              <span
                v-if="column.kind === 'selector'"
                class="row-selector-dot"
                aria-hidden="true"
              ></span>
              <span v-else-if="column.kind === 'actions'" class="row-action-cluster">
                <button
                  class="row-menu-action"
                  type="button"
                  :aria-label="`Menu contextual no disponible para poliza ${item.numero}`"
                  title="Menu contextual pendiente de SDD"
                  disabled
                >
                  <i class="pi pi-ellipsis-v" aria-hidden="true"></i>
                </button>
                <button
                  v-if="!canOpenDetail"
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
                  v-if="showEditAction(item)"
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
                  v-if="showDeleteAction(item)"
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
              </span>
              <span v-else-if="column.key === 'compania'" class="company-logo-cell">
                <span>{{ companyLogoLabel(item) }}</span>
              </span>
              <span
                v-else-if="column.kind === 'field'"
                class="dense-cell"
                :class="{ 'pending-data-cell': isPendingCellValue(item, column.key) }"
                :aria-label="isPendingCellValue(item, column.key) ? POLIZA_EMPTY_VALUE : undefined"
                :title="
                  isPendingCellValue(item, column.key)
                    ? 'Dato no entregado por la API actual'
                    : undefined
                "
              >
                <template v-if="!isPendingCellValue(item, column.key)">
                  {{ formattedCellValue(item, column.key) }}
                </template>
              </span>
              <RouterLink
                v-else-if="column.kind === 'poliza' && canOpenDetail"
                class="table-link policy-number-link"
                :to="detailRoute(item)"
              >
                {{ formattedCellValue(item, column.key) }}
              </RouterLink>
              <span v-else-if="column.kind === 'poliza'" class="policy-number-link readonly">
                {{ formattedCellValue(item, column.key) }}
              </span>
              <span v-else-if="column.kind === 'status'" :class="statusClass(item.estado)">
                {{ statusLabel(item.estado) }}
              </span>
              <span
                v-else
                class="pending-data-cell"
                :aria-label="POLIZA_EMPTY_VALUE"
                title="Dato no entregado por la API actual"
              >
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
