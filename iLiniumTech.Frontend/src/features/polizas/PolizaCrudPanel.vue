<script setup lang="ts">
import { computed, reactive, watch } from 'vue'

import {
  POLIZA_MVP_DELETED_PREFIX,
  POLIZA_MVP_PREFIX,
  type PolizaCreatePayload,
  type PolizaListItem,
  type PolizasCatalogs,
  type PolizaUpdatePayload,
} from './polizasTypes'

const props = defineProps<{
  mode: 'create' | 'edit'
  open: boolean
  catalogs: PolizasCatalogs
  item: PolizaListItem | null
  saving: boolean
  error: string | null
}>()

const emit = defineEmits<{
  close: []
  submit: [payload: PolizaCreatePayload | PolizaUpdatePayload]
}>()

const form = reactive({
  numero: POLIZA_MVP_PREFIX,
  aplicacion: 'MVP',
  ciaId: '1',
  clienteId: '1',
  estado: 'Vigor',
  ramo: 'Autos',
  tipoPoliza: 'Cartera',
  fechaEfecto: '2026-01-01',
  fechaVencimiento: '2026-12-31',
  primaAnual: '0',
})

const panelTitle = computed(() =>
  props.mode === 'create' ? 'Nueva poliza MVP' : `Editar ${props.item?.numero ?? 'poliza MVP'}`,
)

const numeroIsMvp = computed(() => form.numero.trim().startsWith(POLIZA_MVP_PREFIX))
const numeroUsesReservedDeletedPrefix = computed(() =>
  form.numero.trim().startsWith(POLIZA_MVP_DELETED_PREFIX),
)
const ciaIdNumber = computed(() => Number(form.ciaId))
const clienteIdNumber = computed(() => Number(form.clienteId))
const primaAnualNumber = computed(() => Number(form.primaAnual))

const validationError = computed(() => {
  if (!numeroIsMvp.value) {
    return `El numero debe empezar por ${POLIZA_MVP_PREFIX}.`
  }

  if (numeroUsesReservedDeletedPrefix.value) {
    return `El prefijo ${POLIZA_MVP_DELETED_PREFIX} esta reservado para bajas MVP.`
  }

  if (!form.aplicacion.trim()) {
    return 'Aplicacion es obligatoria.'
  }

  if (props.mode === 'create') {
    if (!Number.isInteger(ciaIdNumber.value) || ciaIdNumber.value === 0) {
      return 'CiaId debe ser un entero distinto de cero.'
    }

    if (!Number.isInteger(clienteIdNumber.value) || clienteIdNumber.value === 0) {
      return 'ClienteId debe ser un entero distinto de cero.'
    }
  }

  if (!form.estado || !form.ramo || !form.fechaEfecto) {
    return 'Completa situacion, ramo y fecha de efecto.'
  }

  if (props.mode === 'create' && !form.tipoPoliza.trim()) {
    return 'Tipo es obligatorio en altas.'
  }

  if (form.fechaVencimiento && form.fechaEfecto && form.fechaVencimiento < form.fechaEfecto) {
    return 'La fecha de vencimiento no puede ser anterior al efecto.'
  }

  if (!Number.isFinite(primaAnualNumber.value) || primaAnualNumber.value < 0) {
    return 'Prima anual debe ser un numero no negativo.'
  }

  return null
})

const submitDisabled = computed(() => props.saving || validationError.value !== null)

function resetForm() {
  if (props.mode === 'edit' && props.item) {
    const editableItem = props.item as PolizaListItem & { tipoPoliza?: string }
    form.numero = props.item.numero
    form.aplicacion = props.item.aplicacion || 'MVP'
    form.estado = props.item.estado || 'Vigor'
    form.ramo = props.item.ramo || 'Autos'
    form.tipoPoliza = editableItem.tipoPoliza ?? ''
    form.fechaEfecto = props.item.fechaEfecto || '2026-01-01'
    form.fechaVencimiento = props.item.fechaVencimiento || ''
    form.primaAnual = String(props.item.primaAnual ?? 0)
    return
  }

  form.numero = POLIZA_MVP_PREFIX
  form.aplicacion = 'MVP'
  form.ciaId = '1'
  form.clienteId = '1'
  form.estado = props.catalogs.tipoPoliza[0]?.value ?? 'Vigor'
  form.ramo = props.catalogs.ramo[0]?.value ?? 'Autos'
  form.tipoPoliza = 'Cartera'
  form.fechaEfecto = '2026-01-01'
  form.fechaVencimiento = '2026-12-31'
  form.primaAnual = '0'
}

function submitForm() {
  if (submitDisabled.value) {
    return
  }

  const basePayload = {
    numero: form.numero.trim(),
    aplicacion: form.aplicacion.trim(),
    estado: form.estado,
    ramo: form.ramo,
    fechaEfecto: form.fechaEfecto,
    fechaVencimiento: form.fechaVencimiento || null,
    primaAnual: primaAnualNumber.value,
  }

  if (props.mode === 'create') {
    emit('submit', {
      ...basePayload,
      tipoPoliza: form.tipoPoliza.trim(),
      ciaId: ciaIdNumber.value,
      clienteId: clienteIdNumber.value,
    })
    return
  }

  emit('submit', {
    ...basePayload,
    ...(form.tipoPoliza.trim() ? { tipoPoliza: form.tipoPoliza.trim() } : {}),
  })
}

watch(
  () => [props.open, props.mode, props.item?.id],
  () => {
    if (props.open) {
      resetForm()
    }
  },
  { immediate: true },
)
</script>

<template>
  <section v-if="open" class="poliza-crud-panel" aria-labelledby="poliza-crud-title">
    <header class="poliza-crud-header">
      <div>
        <h2 id="poliza-crud-title">{{ panelTitle }}</h2>
        <span>{{ mode === 'create' ? 'Alta controlada' : 'Registro MVP editable' }}</span>
      </div>
      <button
        class="table-icon-action"
        type="button"
        aria-label="Cerrar formulario de poliza"
        title="Cerrar"
        :disabled="saving"
        @click="emit('close')"
      >
        <i class="pi pi-times" aria-hidden="true"></i>
      </button>
    </header>

    <form class="poliza-crud-form" @submit.prevent="submitForm">
      <label>
        Poliza
        <input v-model.trim="form.numero" type="text" maxlength="50" required />
      </label>
      <label>
        Aplicacion
        <input v-model.trim="form.aplicacion" type="text" maxlength="30" required />
      </label>
      <label v-if="mode === 'create'">
        CiaId
        <input v-model="form.ciaId" type="number" step="1" required />
      </label>
      <label v-if="mode === 'create'">
        ClienteId
        <input v-model="form.clienteId" type="number" step="1" required />
      </label>
      <label>
        Situacion
        <input v-model.trim="form.estado" type="text" list="poliza-estado-options" required />
        <datalist id="poliza-estado-options">
          <option v-for="option in catalogs.tipoPoliza" :key="option.value" :value="option.value">
            {{ option.label }}
          </option>
        </datalist>
      </label>
      <label>
        Ramo
        <input v-model.trim="form.ramo" type="text" list="poliza-ramo-options" required />
        <datalist id="poliza-ramo-options">
          <option v-for="option in catalogs.ramo" :key="option.value" :value="option.value">
            {{ option.label }}
          </option>
        </datalist>
      </label>
      <label v-if="mode === 'create' || form.tipoPoliza">
        Tipo
        <input v-model.trim="form.tipoPoliza" type="text" maxlength="50" required />
      </label>
      <label>
        Efecto
        <input v-model="form.fechaEfecto" type="date" required />
      </label>
      <label>
        Vencimiento
        <input v-model="form.fechaVencimiento" type="date" />
      </label>
      <label>
        Prima anual
        <input v-model="form.primaAnual" type="number" min="0" step="0.01" required />
      </label>

      <p v-if="validationError" class="form-state warning" role="status">
        <i class="pi pi-lock" aria-hidden="true"></i>
        {{ validationError }}
      </p>
      <p v-if="error" class="form-state error" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        {{ error }}
      </p>

      <footer class="poliza-crud-actions">
        <button
          type="button"
          class="state-action secondary"
          :disabled="saving"
          @click="emit('close')"
        >
          Cancelar
        </button>
        <button type="submit" class="state-action" :disabled="submitDisabled">
          <i class="pi pi-save" aria-hidden="true"></i>
          {{ saving ? 'Guardando' : 'Guardar' }}
        </button>
      </footer>
    </form>
  </section>
</template>
