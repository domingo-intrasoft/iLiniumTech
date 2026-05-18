<script setup lang="ts">
import { computed, reactive, watch } from 'vue'

import {
  createEmptyPolizasFilterForm,
  polizasSupportedFilterKeys,
  polizasSearchSections,
  type PolizasSearchCriteria,
  type PolizasSearchField,
} from './polizasConstants'
import type { PolizasCatalogs } from './polizasTypes'

const props = withDefaults(
  defineProps<{
    catalogs: PolizasCatalogs
    criteria?: PolizasSearchCriteria
    loading?: boolean
    error?: string | null
    searchDisabled?: boolean
    blockedMessage?: string | null
  }>(),
  {
    criteria: undefined,
    loading: false,
    error: null,
    searchDisabled: false,
    blockedMessage: null,
  },
)

const emit = defineEmits<{
  search: [criteria: PolizasSearchCriteria]
  clear: []
}>()

const blockedFilterActionsDescription =
  'Las acciones heredadas de filtros permanecen bloqueadas hasta tener SDD, contrato API, permisos y UAT.'
const filterOptionActionDescription =
  'Opciones avanzadas de filtro pendientes de SDD y contrato API de polizas.'
const unsupportedFilterDescription = 'Pendiente de contrato API de polizas.'

const searchValues = reactive(createEmptyPolizasFilterForm())
const canSearch = computed(() => !props.loading && !props.searchDisabled)

function applyCriteria(criteria: PolizasSearchCriteria | undefined) {
  Object.keys(searchValues).forEach((key) => {
    searchValues[key as keyof typeof searchValues] = ''
  })

  if (!criteria) {
    return
  }

  searchValues.poliza = criteria.numero
  searchValues.nombreCompleto = criteria.cliente
  searchValues.tipoPoliza = criteria.estado
  searchValues.cia = criteria.compania
  searchValues.ramo = criteria.ramo
  searchValues.efectoInicial = criteria.fechaEfectoDesde
  searchValues.efectoFinal = criteria.fechaEfectoHasta
}

function fieldStyle(field: PolizasSearchField) {
  return { gridColumn: `span ${field.span}` }
}

function fieldControlId(field: PolizasSearchField) {
  return `polizas-filter-${field.key}`
}

function searchValue(key: keyof typeof searchValues) {
  return searchValues[key] ?? ''
}

function catalogOptions(field: PolizasSearchField) {
  if (!field.catalogKey) {
    return []
  }

  return props.catalogs[field.catalogKey]
}

function fieldIsSupported(field: PolizasSearchField) {
  return polizasSupportedFilterKeys.includes(field.key)
}

function fieldDisabled(field: PolizasSearchField) {
  return (
    props.searchDisabled ||
    !fieldIsSupported(field) ||
    (props.loading && field.control === 'select')
  )
}

function fieldTitle(field: PolizasSearchField) {
  if (props.searchDisabled && props.blockedMessage) {
    return props.blockedMessage
  }

  return fieldIsSupported(field) ? undefined : unsupportedFilterDescription
}

function fieldDescriptionIds(field: PolizasSearchField) {
  const ids = [
    props.error && field.control === 'select' ? 'polizas-catalog-error' : undefined,
    props.searchDisabled && props.blockedMessage ? 'polizas-search-blocked' : undefined,
    !fieldIsSupported(field) ? 'polizas-filter-unsupported' : undefined,
  ]
    .filter(Boolean)
    .join(' ')

  return ids || undefined
}

function searchDescriptionId() {
  return props.searchDisabled && props.blockedMessage ? 'polizas-search-blocked' : undefined
}

function executeSearch() {
  if (!canSearch.value) {
    return
  }

  emit('search', {
    numero: searchValue('poliza'),
    cliente: searchValue('nombreCompleto') || searchValue('nombre') || searchValue('documento'),
    estado: searchValue('tipoPoliza'),
    compania: searchValue('cia'),
    ramo: searchValue('ramo'),
    fechaEfectoDesde: searchValue('efectoInicial'),
    fechaEfectoHasta: searchValue('efectoFinal'),
  })
}

function clearFilters() {
  Object.keys(searchValues).forEach((key) => {
    searchValues[key as keyof typeof searchValues] = ''
  })
  emit('clear')
}

watch(() => props.criteria, applyCriteria, { immediate: true, deep: true })
</script>

<template>
  <section
    class="search-panel polizas-search-panel"
    aria-label="Busqueda avanzada de polizas"
    :aria-busy="props.loading"
  >
    <header class="search-actions polizas-grid-search">
      <p id="polizas-filter-blocked-actions" class="sr-only">
        {{ blockedFilterActionsDescription }}
      </p>
      <p
        v-if="props.searchDisabled && props.blockedMessage"
        id="polizas-search-blocked"
        class="sr-only"
      >
        {{ props.blockedMessage }}
      </p>
      <p id="polizas-filter-unsupported" class="sr-only">
        {{ unsupportedFilterDescription }}
      </p>
      <label class="polizas-main-search" for="polizas-filter-poliza">
        <i class="pi pi-search" aria-hidden="true"></i>
        <span class="sr-only">Buscar poliza</span>
        <input
          id="polizas-filter-poliza"
          v-model="searchValues.poliza"
          type="search"
          placeholder="Buscar..."
          aria-label="Poliza"
          :aria-describedby="searchDescriptionId()"
          :disabled="props.searchDisabled"
          @keydown.enter.prevent="executeSearch"
        />
      </label>
      <div class="search-action-buttons">
        <button
          type="button"
          class="icon-only"
          aria-label="Opciones de filtro para Poliza no disponibles en el MVP"
          :title="filterOptionActionDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-cog" aria-hidden="true"></i>
        </button>
        <button
          type="button"
          class="primary-action"
          :disabled="!canSearch"
          :title="props.searchDisabled ? (props.blockedMessage ?? undefined) : undefined"
          :aria-describedby="searchDescriptionId()"
          @click="executeSearch"
        >
          <i class="pi pi-search" aria-hidden="true"></i>
          Buscar
        </button>
        <button type="button" :disabled="props.loading" @click="clearFilters">
          <i class="pi pi-trash" aria-hidden="true"></i>
          Limpiar Filtros
        </button>
        <button
          type="button"
          :title="blockedFilterActionsDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-times" aria-hidden="true"></i>Cerrar Pestanas
        </button>
        <button
          type="button"
          :title="blockedFilterActionsDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-save" aria-hidden="true"></i>Guardar busqueda
        </button>
        <button
          type="button"
          :title="blockedFilterActionsDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-arrow-up" aria-hidden="true"></i>Avanzada
        </button>
        <button
          type="button"
          class="primary-action compact-action"
          :title="blockedFilterActionsDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-search" aria-hidden="true"></i>
          Simple
        </button>
        <button
          type="button"
          class="primary-action icon-only"
          aria-label="Agregar filtro no disponible en el MVP"
          :title="blockedFilterActionsDescription"
          aria-describedby="polizas-filter-blocked-actions"
          disabled
        >
          <i class="pi pi-plus" aria-hidden="true"></i>
        </button>
      </div>
      <i class="pi pi-chevron-up" aria-hidden="true"></i>
    </header>

    <p v-if="props.loading" id="polizas-catalog-loading" class="sr-only" role="status">
      Cargando catalogos de polizas.
    </p>

    <p v-if="props.error" id="polizas-catalog-error" class="filter-warning" role="alert">
      {{ props.error }}
    </p>

    <details class="advanced-filter-panel">
      <summary>Filtros detallados</summary>
      <div class="criteria-card">
        <div class="criteria-select">
          <button
            type="button"
            :title="blockedFilterActionsDescription"
            aria-describedby="polizas-filter-blocked-actions"
            disabled
          >
            Seleccione... <i class="pi pi-chevron-down" aria-hidden="true"></i>
          </button>
        </div>

        <section
          v-for="section in polizasSearchSections"
          :key="section.title"
          class="filter-section"
        >
          <div class="filter-section-title">
            <h2>{{ section.title }}</h2>
            <i class="pi pi-minus" aria-hidden="true"></i>
          </div>

          <div v-for="(row, rowIndex) in section.rows" :key="rowIndex" class="filter-row">
            <template v-for="field in row" :key="field.key">
              <label
                v-if="field.key !== 'poliza'"
                class="filter-field"
                :class="{ 'unsupported-filter': !fieldIsSupported(field) }"
                :for="fieldControlId(field)"
                :style="fieldStyle(field)"
                :title="fieldTitle(field)"
              >
                <span>{{ field.label }}</span>
                <span class="field-control">
                  <select
                    v-if="field.control === 'select'"
                    :id="fieldControlId(field)"
                    v-model="searchValues[field.key]"
                    :aria-label="field.label"
                    :aria-describedby="fieldDescriptionIds(field)"
                    :disabled="fieldDisabled(field)"
                  >
                    <option value=""></option>
                    <option
                      v-for="option in catalogOptions(field)"
                      :key="option.value"
                      :value="option.value"
                    >
                      {{ option.label }}
                    </option>
                  </select>
                  <input
                    v-else
                    :id="fieldControlId(field)"
                    v-model="searchValues[field.key]"
                    :type="field.control === 'date' ? 'date' : 'search'"
                    :aria-label="field.label"
                    :aria-describedby="fieldDescriptionIds(field)"
                    :disabled="fieldDisabled(field)"
                  />
                  <button
                    type="button"
                    :aria-label="`Opciones de filtro para ${field.label} no disponibles en el MVP`"
                    :title="filterOptionActionDescription"
                    aria-describedby="polizas-filter-blocked-actions"
                    disabled
                  >
                    <i class="pi pi-filter" aria-hidden="true"></i>
                  </button>
                </span>
              </label>
            </template>
          </div>
        </section>
      </div>
    </details>
  </section>
</template>
