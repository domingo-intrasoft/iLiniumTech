<script setup lang="ts">
import { reactive } from 'vue'

import {
  autosSearchSections,
  createEmptyAutosFilterForm,
  type AutosSearchCriteria,
  type AutosSearchField,
} from './autosParticularesConstants'
import type { AutosParticularesCatalogs } from './autosParticularesTypes'

const props = defineProps<{
  catalogs: AutosParticularesCatalogs
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  search: [criteria: AutosSearchCriteria]
  clear: []
}>()

const searchValues = reactive(createEmptyAutosFilterForm())

function fieldStyle(field: AutosSearchField) {
  return { gridColumn: `span ${field.span}` }
}

function fieldControlId(field: AutosSearchField) {
  return `autos-filter-${field.key}`
}

function catalogOptions(field: AutosSearchField) {
  if (!field.catalogKey) {
    return []
  }

  return props.catalogs[field.catalogKey]
}

function fieldDisabled(field: AutosSearchField) {
  return props.loading === true && field.control === 'select'
}

function executeSearch() {
  emit('search', { ...searchValues })
}

function clearFilters() {
  Object.keys(searchValues).forEach((key) => {
    searchValues[key as keyof typeof searchValues] = ''
  })
  emit('clear')
}
</script>

<template>
  <section class="search-panel" aria-label="Busqueda de autos particulares" :aria-busy="loading">
    <header class="search-actions">
      <div class="search-action-buttons">
        <button type="button" class="primary-action" :disabled="loading" @click="executeSearch">
          <i class="pi pi-search" aria-hidden="true"></i>
          Buscar
        </button>
        <button type="button" :disabled="loading" @click="clearFilters">
          <i class="pi pi-trash" aria-hidden="true"></i>
          Limpiar Filtros
        </button>
        <button type="button" disabled>
          <i class="pi pi-save" aria-hidden="true"></i>
          Guardar busqueda
        </button>
        <button type="button" class="primary-action icon-only" aria-label="Agregar filtro" disabled>
          <i class="pi pi-plus" aria-hidden="true"></i>
        </button>
      </div>
      <i class="pi pi-chevron-up" aria-hidden="true"></i>
    </header>

    <p v-if="loading" class="sr-only" role="status">Cargando catalogos de autos.</p>

    <p v-if="error" id="autos-catalog-error" class="filter-warning" role="alert">
      {{ error }}
    </p>

    <div class="criteria-card autos-criteria-card">
      <section v-for="section in autosSearchSections" :key="section.title" class="filter-section">
        <div class="filter-section-title">
          <h2>{{ section.title }}</h2>
          <i class="pi pi-minus" aria-hidden="true"></i>
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
                :aria-describedby="error ? 'autos-catalog-error' : undefined"
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
