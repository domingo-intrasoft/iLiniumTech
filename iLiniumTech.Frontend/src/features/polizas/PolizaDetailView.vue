<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'

import { clearAuthSession, switchAuthBroker, useAuthSession } from '@/features/auth/authSession'
import { toPolizasUserError } from '@/services/apiErrors'
import { getBlockingRuntimeConfigMessage } from '@/services/runtimeConfig'
import { type SessionContext, useSession } from '@/services/session'

import AppShell from '@/layout/AppShell.vue'

import { getPolizaById } from './polizasApi'
import { polizaDetailSections, type PolizaDetailField } from './polizasConstants'
import { POLIZA_EMPTY_VALUE, formatPolizaValue } from './polizasFormatters'
import type { PolizaDetail } from './polizasTypes'

const route = useRoute()
const router = useRouter()
const { userLabel, logout } = useAuthSession()
const { session, error: sessionError, errorKind: sessionErrorKind, loadSession } = useSession()
const poliza = ref<PolizaDetail | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
const brokerChanging = ref(false)
const brokerError = ref<string | null>(null)
const premiumField: PolizaDetailField = { key: 'primaAnual', label: 'Prima anual', type: 'money' }
const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'
const dataOriginLabel = computed(() => (isBackendMode ? 'API polizas' : 'Fixture local'))
const shellStatusLabel = computed(() => (isBackendMode ? 'API polizas' : 'Modo local'))
const detailTitle = computed(() => poliza.value?.numero ?? 'Detalle de poliza')
const polizasBackTarget = computed(() => ({ name: 'polizas', query: route.query }))
const POLIZAS_DETAIL_PERMISSION = 'polizas.detail'
const POLIZAS_DETAIL_ACCESS_DENIED_MESSAGE =
  'La sesion actual no tiene permiso para consultar el detalle de polizas.'

const brokerOptions = computed(() => {
  if (!isBackendMode || session.value?.authMode !== 'DemoSession') {
    return []
  }

  const brokerIds = session.value.allowedBrokerIds ?? []
  const uniqueBrokerIds = Array.from(
    new Set(brokerIds.filter((brokerId) => Number.isInteger(brokerId) && brokerId > 0)),
  )

  return uniqueBrokerIds.length > 1 ? uniqueBrokerIds : []
})

function currentPolizaId() {
  const id = route.params.id
  return Array.isArray(id) ? (id[0] ?? '') : (id ?? '')
}

function detailValue(item: PolizaDetail, field: PolizaDetailField) {
  return formatPolizaValue(item[field.key], field.type, item.moneda)
}

function isMissingDetailValue(item: PolizaDetail, field: PolizaDetailField) {
  return detailValue(item, field) === POLIZA_EMPTY_VALUE
}

function permissionIsAllowed(context: SessionContext, permission: string) {
  return (
    context.authMode === 'ApiKey' ||
    !Array.isArray(context.permissions) ||
    context.permissions.includes(permission)
  )
}

async function ensureBackendDetailContext() {
  const runtimeConfigError = getBlockingRuntimeConfigMessage()
  if (runtimeConfigError) {
    error.value = runtimeConfigError
    poliza.value = null
    return false
  }

  if (!isBackendMode) {
    return true
  }

  const currentSession = await loadSession()
  if (!currentSession) {
    const validationError =
      sessionError.value ?? 'No se pudo validar la sesion antes de consultar polizas.'

    if (sessionErrorKind.value === 'unauthenticated') {
      clearAuthSession()
    }

    error.value = validationError
    poliza.value = null
    return false
  }

  if (currentSession.polizasExecutionContextRequired && currentSession.brokerId === null) {
    error.value = 'Configura un broker para consultar polizas.'
    poliza.value = null
    return false
  }

  if (!permissionIsAllowed(currentSession, POLIZAS_DETAIL_PERMISSION)) {
    error.value = POLIZAS_DETAIL_ACCESS_DENIED_MESSAGE
    poliza.value = null
    return false
  }

  return true
}

async function loadPoliza() {
  loading.value = true
  error.value = null

  try {
    if (!(await ensureBackendDetailContext())) {
      return
    }

    const id = currentPolizaId()
    poliza.value = id ? await getPolizaById(id) : null

    if (!poliza.value) {
      error.value = 'Poliza no encontrada.'
    }
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo cargar la poliza.')
    if (userError.kind === 'unauthenticated') {
      clearAuthSession()
    }
    error.value = userError.message
    poliza.value = null
  } finally {
    loading.value = false
  }
}

async function signOut() {
  await logout()
  await router.replace({ name: 'login' })
}

async function changeBroker(brokerId: number) {
  if (brokerChanging.value || brokerId === session.value?.brokerId) {
    return
  }

  if (!brokerOptions.value.includes(brokerId)) {
    brokerError.value = 'El broker seleccionado no esta disponible para la sesion actual.'
    error.value = brokerError.value
    return
  }

  brokerChanging.value = true
  brokerError.value = null

  try {
    await switchAuthBroker(brokerId)
    await router.replace({ name: 'polizas' })
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo cambiar el broker activo.')
    brokerError.value = userError.message
    error.value = brokerError.value
    if (userError.kind === 'unauthenticated') {
      clearAuthSession()
      await router.replace({ name: 'login' })
    }
  } finally {
    brokerChanging.value = false
  }
}

watch(() => route.params.id, loadPoliza, { immediate: true })
</script>

<template>
  <AppShell
    content-id="poliza-detail-content"
    section-title="Polizas"
    :session-label="shellStatusLabel"
    :user-label="userLabel"
    :broker-options="brokerOptions"
    :active-broker-id="session?.brokerId ?? null"
    :broker-changing="brokerChanging"
    :broker-error="brokerError"
    show-sign-out
    @broker-change="changeBroker"
    @sign-out="signOut"
  >
    <div id="poliza-detail-content" class="poliza-detail-page">
      <header class="detail-topbar">
        <RouterLink
          class="detail-back"
          :to="polizasBackTarget"
          aria-label="Volver al listado de polizas"
        >
          <i class="pi pi-arrow-left" aria-hidden="true"></i>
          Volver a Polizas
        </RouterLink>
        <nav class="detail-breadcrumb" aria-label="Ruta de poliza">
          <span>Polizas / Detalle / </span>
          <span aria-current="page">{{ detailTitle }}</span>
        </nav>
      </header>

      <section
        v-if="loading"
        class="detail-loading"
        role="status"
        aria-label="Cargando detalle de poliza"
        aria-busy="true"
      >
        <div class="detail-hero skeleton-panel">
          <div>
            <span class="skeleton-line short"></span>
            <span class="skeleton-line title"></span>
            <span class="skeleton-line medium"></span>
          </div>
          <div class="detail-premium">
            <span class="skeleton-line short"></span>
            <span class="skeleton-line medium"></span>
          </div>
        </div>
      </section>

      <div v-else-if="error" class="state state-box error" role="alert">
        <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
        <div>
          <strong>No se pudo abrir el detalle</strong>
          <p>{{ error }}</p>
        </div>
        <button type="button" class="state-action" @click="loadPoliza">
          <i class="pi pi-refresh" aria-hidden="true"></i>
          Reintentar
        </button>
      </div>

      <template v-else-if="poliza">
        <section class="detail-hero" :aria-labelledby="'poliza-detail-title'">
          <div>
            <span class="detail-kicker">{{ poliza.estado }}</span>
            <h1 id="poliza-detail-title">{{ detailTitle }}</h1>
            <p>{{ poliza.clienteNombre }}</p>
          </div>
          <div class="detail-premium">
            <span>Prima anual</span>
            <strong>{{ detailValue(poliza, premiumField) }}</strong>
          </div>
        </section>

        <section class="detail-notes" aria-label="Contexto del detalle">
          <span><i class="pi pi-lock" aria-hidden="true"></i> Solo lectura</span>
          <span><i class="pi pi-database" aria-hidden="true"></i> {{ dataOriginLabel }}</span>
          <span><i class="pi pi-shield" aria-hidden="true"></i> Sin workflows heredados</span>
        </section>

        <section
          v-for="section in polizaDetailSections"
          :key="section.title"
          class="detail-section"
        >
          <h2>{{ section.title }}</h2>
          <dl class="detail-grid">
            <div v-for="field in section.fields" :key="field.key" class="detail-field">
              <dt>{{ field.label }}</dt>
              <dd :class="{ missing: isMissingDetailValue(poliza, field) }">
                {{ detailValue(poliza, field) }}
              </dd>
            </div>
          </dl>
        </section>
      </template>
    </div>
  </AppShell>
</template>
