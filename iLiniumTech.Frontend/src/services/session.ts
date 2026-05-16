import { computed, readonly, ref } from 'vue'

import { toPolizasUserError, type PolizasUserErrorKind } from './apiErrors'
import { apiClient } from './apiClient'
import { assertRuntimeConfigReady, getRuntimeConfig, RuntimeConfigError } from './runtimeConfig'

export interface SessionContext {
  brokerId: number | null
  entityMainId: number | null
  userId: number | null
  profileId: number | null
  profileTypeId: string | null
  isAdmin: boolean | null
  headerExecutionContextEnabled: boolean
  polizasExecutionContextRequired: boolean
  user?: {
    id: string
    displayName: string
  } | null
  application?: {
    key: string
    name: string
  } | null
  allowedBrokerIds?: number[]
  permissions?: string[]
  authMode?: string
}

const session = ref<SessionContext | null>(null)
const loading = ref(false)
const error = ref<string | null>(null)
const errorKind = ref<PolizasUserErrorKind | null>(null)
let pendingRequest: Promise<SessionContext> | null = null

export class BrokerSwitchVerificationError extends Error {
  readonly cause: unknown

  constructor(cause: unknown) {
    super('No se pudo confirmar el broker activo tras cambiarlo.')
    this.name = 'BrokerSwitchVerificationError'
    this.cause = cause
  }
}

function readPositiveInteger(value: string | undefined): number | null {
  if (!value) {
    return null
  }

  const parsed = Number(value)
  return Number.isInteger(parsed) && parsed > 0 ? parsed : null
}

function normalizeSessionContext(data: SessionContext): SessionContext {
  return {
    brokerId: data.brokerId ?? null,
    entityMainId: data.entityMainId ?? null,
    userId: data.userId ?? null,
    profileId: data.profileId ?? null,
    profileTypeId: data.profileTypeId ?? null,
    isAdmin: data.isAdmin ?? null,
    headerExecutionContextEnabled: data.headerExecutionContextEnabled,
    polizasExecutionContextRequired: data.polizasExecutionContextRequired,
    user: data.user ?? null,
    application: data.application ?? null,
    allowedBrokerIds: data.allowedBrokerIds ?? [],
    permissions: Array.isArray(data.permissions) ? data.permissions : undefined,
    authMode: data.authMode,
  }
}

export async function getSessionContext(): Promise<SessionContext> {
  if (import.meta.env.VITE_USE_BACKEND !== 'true') {
    const brokerId = readPositiveInteger(import.meta.env.VITE_BROKER_ID)
    return normalizeSessionContext({
      brokerId,
      entityMainId: brokerId,
      userId: null,
      profileId: null,
      profileTypeId: null,
      isAdmin: null,
      headerExecutionContextEnabled: false,
      polizasExecutionContextRequired: false,
      user: null,
      application: { key: 'iliniumtech', name: 'iLiniumTech' },
      allowedBrokerIds: brokerId ? [brokerId] : [],
      permissions: ['polizas.catalogs', 'polizas.read', 'polizas.detail'],
      authMode: 'local',
    })
  }

  assertRuntimeConfigReady()
  const response = await apiClient.get<SessionContext>('/api/me')
  return normalizeSessionContext(response.data)
}

export async function switchSessionBroker(brokerId: number): Promise<SessionContext> {
  if (!Number.isInteger(brokerId) || brokerId <= 0) {
    throw new RuntimeConfigError('Selecciona un broker valido para continuar.')
  }

  const runtimeConfig = getRuntimeConfig()
  if (!runtimeConfig.backendEnabled || runtimeConfig.authMode !== 'demo-session') {
    throw new RuntimeConfigError(
      'El cambio de broker requiere una sesion demo validada por backend.',
    )
  }

  assertRuntimeConfigReady(runtimeConfig)
  loading.value = true
  error.value = null
  errorKind.value = null
  let brokerSwitchCommitted = false

  try {
    await apiClient.post('/api/auth/broker', { brokerId })
    brokerSwitchCommitted = true
    clearSessionContext()
    const nextSession = await getSessionContext()
    session.value = nextSession
    return nextSession
  } catch (exception) {
    const userError = toPolizasUserError(exception, 'No se pudo cambiar el broker activo.')
    error.value = userError.message
    errorKind.value = userError.kind
    if (brokerSwitchCommitted) {
      throw new BrokerSwitchVerificationError(exception)
    }
    throw exception
  } finally {
    loading.value = false
  }
}

export function useSession() {
  const brokerId = computed(() => session.value?.brokerId ?? null)
  const hasBrokerContext = computed(() => brokerId.value !== null)

  async function loadSession(options: { force?: boolean } = {}) {
    if (session.value !== null && !options.force) {
      return session.value
    }

    loading.value = true
    error.value = null
    errorKind.value = null

    pendingRequest ??= getSessionContext().finally(() => {
      pendingRequest = null
    })

    try {
      session.value = await pendingRequest
      return session.value
    } catch (exception) {
      const userError = toPolizasUserError(exception, 'No se pudo cargar la sesion.')
      session.value = null
      error.value = userError.message
      errorKind.value = userError.kind
      return null
    } finally {
      loading.value = false
    }
  }

  function resetSession() {
    clearSessionContext()
  }

  return {
    session: readonly(session),
    brokerId,
    hasBrokerContext,
    loading: readonly(loading),
    error: readonly(error),
    errorKind: readonly(errorKind),
    loadSession,
    resetSession,
  }
}

export function clearSessionContext() {
  session.value = null
  error.value = null
  errorKind.value = null
  pendingRequest = null
}
