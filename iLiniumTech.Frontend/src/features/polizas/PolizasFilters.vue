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
}>()

const emit = defineEmits<{
  search: [criteria: PolizasSearchCriteria]
  clear: []
}>()

const searchValues = reactive(createEmptyPolizasFilterForm())

function fieldStyle(field: PolizasSearchField) {
  return { gridColumn: `span ${field.span}` }
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
        <button type="button" class="primary-action" @click="executeSearch">
          <i class="pi pi-search"></i>
          Buscar
        </button>
        <button type="button" @click="clearFilters">
          <i class="pi pi-trash"></i>
          Limpiar Filtros
        </button>
        <button type="button"><i class="pi pi-times"></i>Cerrar Pestanas</button>
        <button type="button"><i class="pi pi-save"></i>Guardar busqueda</button>
        <button type="button"><i class="pi pi-arrow-up"></i>Avanzada</button>
        <button type="button" class="primary-action compact-action">
          <i class="pi pi-search"></i>
          Simple
        </button>
        <button type="button" class="primary-action icon-only" aria-label="Agregar filtro">
          <i class="pi pi-plus"></i>
        </button>
      </div>
      <i class="pi pi-chevron-up"></i>
    </header>

    <div class="criteria-card">
      <div class="criteria-select">
        <button type="button">Seleccione... <i class="pi pi-chevron-down"></i></button>
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
            :style="fieldStyle(field)"
          >
            <span>{{ field.label }}</span>
            <span class="field-control">
              <select v-if="field.control === 'select'" v-model="searchValues[field.key]">
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
                v-model="searchValues[field.key]"
                :type="field.control === 'date' ? 'date' : 'search'"
              />
              <button type="button" aria-label="Filtro de campo">
                <i class="pi pi-filter"></i>
              </button>
            </span>
          </label>
        </div>
      </section>
    </div>
  </section>
</template>
