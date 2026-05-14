<script setup lang="ts">
import { reactive } from 'vue'

import {
  createEmptyPolizasFilterForm,
  polizasSearchSections,
  type PolizasSearchCriteria,
  type PolizasSearchField,
} from './polizasConstants'
import type { PolizasCatalogs } from './polizasTypes'

const props = defineProps<{
  catalogs: PolizasCatalogs
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  search: [criteria: PolizasSearchCriteria]
  clear: []
}>()

const searchValues = reactive(createEmptyPolizasFilterForm())

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

function executeSearch() {
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
</script>

<template>
  <section class="search-panel" aria-label="Busqueda avanzada de polizas">
    <header class="search-actions">
      <div class="search-action-buttons">
        <button
          type="button"
          class="primary-action"
          :disabled="props.loading"
          @click="executeSearch"
        >
          <i class="pi pi-search" aria-hidden="true"></i>
          Buscar
        </button>
        <button type="button" :disabled="props.loading" @click="clearFilters">
          <i class="pi pi-trash" aria-hidden="true"></i>
          Limpiar Filtros
        </button>
        <button type="button" disabled>
          <i class="pi pi-times" aria-hidden="true"></i>Cerrar Pestanas
        </button>
        <button type="button" disabled>
          <i class="pi pi-save" aria-hidden="true"></i>Guardar busqueda
        </button>
        <button type="button" disabled>
          <i class="pi pi-arrow-up" aria-hidden="true"></i>Avanzada
        </button>
        <button type="button" class="primary-action compact-action" disabled>
          <i class="pi pi-search" aria-hidden="true"></i>
          Simple
        </button>
        <button type="button" class="primary-action icon-only" aria-label="Agregar filtro" disabled>
          <i class="pi pi-plus" aria-hidden="true"></i>
        </button>
      </div>
      <i class="pi pi-chevron-up" aria-hidden="true"></i>
    </header>

    <p v-if="props.error" class="filter-warning" role="alert">
      {{ props.error }}
    </p>

    <div class="criteria-card">
      <div class="criteria-select">
        <button type="button" disabled>
          Seleccione... <i class="pi pi-chevron-down" aria-hidden="true"></i>
        </button>
      </div>

      <section v-for="section in polizasSearchSections" :key="section.title" class="filter-section">
        <div class="filter-section-title">
          <h2>{{ section.title }}</h2>
          <i class="pi pi-minus"></i>
        </div>

        <div v-for="(row, rowIndex) in section.rows" :key="rowIndex" class="filter-row">
          <label
            v-for="field in row"
            :key="field.key"
            class="filter-field"
            :for="fieldControlId(field)"
            :style="fieldStyle(field)"
          >
            <span>{{ field.label }}</span>
            <span class="field-control">
              <select
                v-if="field.control === 'select'"
                :id="fieldControlId(field)"
                v-model="searchValues[field.key]"
                :aria-label="field.label"
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
              />
              <button type="button" :aria-label="`Opciones de filtro para ${field.label}`" disabled>
                <i class="pi pi-filter" aria-hidden="true"></i>
              </button>
            </span>
          </label>
        </div>
      </section>
    </div>
  </section>
</template>
