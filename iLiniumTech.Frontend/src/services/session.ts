import { computed, readonly, ref } from 'vue'

import { toPolizasUserMessage } from './apiErrors'
import { apiClient } from './apiClient'
import { assertRuntimeConfigReady } from './runtimeConfig'

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
let pendingRequest: Promise<SessionContext> | null = null

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

export function useSession() {
  const brokerId = computed(() => session.value?.brokerId ?? null)
  const hasBrokerContext = computed(() => brokerId.value !== null)

  async function loadSession(options: { force?: boolean } = {}) {
    if (session.value !== null && !options.force) {
      return session.value
    }

    loading.value = true
    error.value = null

    pendingRequest ??= getSessionContext().finally(() => {
      pendingRequest = null
    })

    try {
      session.value = await pendingRequest
      return session.value
    } catch (exception) {
      session.value = null
      error.value = toPolizasUserMessage(exception, 'No se pudo cargar la sesion.')
      return null
    } finally {
      loading.value = false
    }
  }

  function resetSession() {
    session.value = null
    error.value = null
    pendingRequest = null
  }

  return {
    session: readonly(session),
    brokerId,
    hasBrokerContext,
    loading: readonly(loading),
    error: readonly(error),
    loadSession,
    resetSession,
  }
}
