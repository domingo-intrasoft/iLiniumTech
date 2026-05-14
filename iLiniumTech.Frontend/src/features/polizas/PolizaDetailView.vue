<script setup lang="ts">
import { ref, watch } from 'vue'
import { RouterLink, useRoute } from 'vue-router'

import { getPolizaById } from './polizasApi'
import {
  polizaDetailSections,
  type PolizaDetailField,
  type PolizaValueType,
} from './polizasConstants'
import type { PolizaDetail } from './polizasTypes'

const route = useRoute()
const poliza = ref<PolizaDetail | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
const premiumField: PolizaDetailField = { key: 'primaAnual', label: 'Prima anual', type: 'money' }

function currentPolizaId() {
  const id = route.params.id
  return Array.isArray(id) ? (id[0] ?? '') : (id ?? '')
}

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

function detailValue(item: PolizaDetail, field: PolizaDetailField) {
  return formatPolizaValue(item[field.key], field.type, item.moneda)
}

async function loadPoliza() {
  loading.value = true
  error.value = null

  try {
    const id = currentPolizaId()
    poliza.value = id ? await getPolizaById(id) : null

    if (!poliza.value) {
      error.value = 'Poliza no encontrada.'
    }
  } catch {
    error.value = 'No se pudo cargar la poliza.'
    poliza.value = null
  } finally {
    loading.value = false
  }
}

watch(() => route.params.id, loadPoliza, { immediate: true })
</script>

<template>
  <main class="poliza-detail-page">
    <header class="detail-topbar">
      <RouterLink class="detail-back" :to="{ name: 'polizas' }">
        <i class="pi pi-arrow-left"></i>
        Polizas
      </RouterLink>
    </header>

    <p v-if="loading" class="state">Cargando poliza...</p>
    <p v-else-if="error" class="state error">{{ error }}</p>

    <template v-else-if="poliza">
      <section class="detail-hero">
        <div>
          <span class="detail-kicker">{{ poliza.estado }}</span>
          <h1>{{ poliza.numero }}</h1>
          <p>{{ poliza.clienteNombre }}</p>
        </div>
        <div class="detail-premium">
          <span>Prima anual</span>
          <strong>{{ detailValue(poliza, premiumField) }}</strong>
        </div>
      </section>

      <section v-for="section in polizaDetailSections" :key="section.title" class="detail-section">
        <h2>{{ section.title }}</h2>
        <dl class="detail-grid">
          <div v-for="field in section.fields" :key="field.key" class="detail-field">
            <dt>{{ field.label }}</dt>
            <dd>{{ detailValue(poliza, field) }}</dd>
          </div>
        </dl>
      </section>
    </template>
  </main>
</template>
