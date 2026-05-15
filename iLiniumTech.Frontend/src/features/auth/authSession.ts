import { computed, readonly, ref } from 'vue'
import axios from 'axios'

import { apiClient } from '@/services/apiClient'
import { getRuntimeConfig } from '@/services/runtimeConfig'

export interface AuthUser {
  id: string
  displayName: string
}

export interface AuthApplication {
  key: string
  name: string
}

export interface AuthMvpSession {
  mode: 'demo'
  source: 'local' | 'backend'
  sessionId: string
  createdAt: string
  expiresAt: string | null
  user: AuthUser
  application: AuthApplication
  currentBrokerId: number | null
  permissions: string[]
}

export interface LoginCredentials {
  username: string
  password: string
}

const SESSION_STORAGE_KEY = 'iliniumtech.auth.mvp-session'
const DEMO_PERMISSIONS = ['polizas.catalogs', 'polizas.read', 'polizas.detail']
const session = ref<AuthMvpSession | null>(readStoredSession())

interface BackendLoginResponse {
  session: {
    mode: 'demo'
    expiresAt: string
  }
  user: AuthUser
  application: AuthApplication
  currentBrokerId: number | null
  permissions: string[]
}

interface BackendAuthErrorResponse {
  error?: {
    code?: string
    message?: string
  }
}

function readPositiveInteger(value: string | undefined): number | null {
  if (!value) {
    return null
  }

  const parsed = Number(value)
  return Number.isInteger(parsed) && parsed > 0 ? parsed : null
}

function storage(): Storage | null {
  return typeof window === 'undefined' ? null : window.sessionStorage
}

function createSessionId() {
  return typeof crypto !== 'undefined' && 'randomUUID' in crypto
    ? crypto.randomUUID()
    : `mvp-${Date.now()}`
}

function displayNameFromUsername(username: string) {
  const name = username.trim()
  return name.includes('@') ? (name.split('@')[0] ?? name) : name
}

function normalizeStoredSession(value: unknown): AuthMvpSession | null {
  if (!value || typeof value !== 'object') {
    return null
  }

  const candidate = value as Partial<AuthMvpSession>
  if (
    candidate.mode !== 'demo' ||
    !candidate.sessionId ||
    !candidate.createdAt ||
    !candidate.user?.id ||
    !candidate.user.displayName ||
    candidate.application?.key !== 'iliniumtech'
  ) {
    return null
  }

  const normalized: AuthMvpSession = {
    mode: 'demo',
    source: candidate.source === 'backend' ? 'backend' : 'local',
    sessionId: candidate.sessionId,
    createdAt: candidate.createdAt,
    expiresAt: candidate.expiresAt ?? null,
    user: {
      id: candidate.user.id,
      displayName: candidate.user.displayName,
    },
    application: {
      key: 'iliniumtech',
      name: candidate.application.name || 'iLiniumTech',
    },
    currentBrokerId: candidate.currentBrokerId ?? null,
    permissions: Array.isArray(candidate.permissions) ? candidate.permissions : DEMO_PERMISSIONS,
  }

  return isAuthSessionExpired(normalized) ? null : normalized
}

export function isAuthSessionExpired(candidate: Pick<AuthMvpSession, 'expiresAt'>) {
  if (!candidate.expiresAt) {
    return false
  }

  const expiresAt = Date.parse(candidate.expiresAt)
  return !Number.isFinite(expiresAt) || expiresAt <= Date.now()
}

export function readStoredSession(): AuthMvpSession | null {
  const store = storage()
  try {
    const rawSession = store?.getItem(SESSION_STORAGE_KEY)
    if (!rawSession) {
      return null
    }

    const storedSession = normalizeStoredSession(JSON.parse(rawSession))
    if (!storedSession) {
      store?.removeItem(SESSION_STORAGE_KEY)
    }

    return storedSession
  } catch {
    store?.removeItem(SESSION_STORAGE_KEY)
    return null
  }
}

export function hasAuthSession() {
  session.value = readStoredSession()
  return session.value !== null
}

export function loginDemo(credentials: LoginCredentials): AuthMvpSession {
  const username = credentials.username.trim()
  if (!username || !credentials.password) {
    throw new Error('Introduce usuario y contrasena.')
  }

  const nextSession: AuthMvpSession = {
    mode: 'demo',
    source: 'local',
    sessionId: createSessionId(),
    createdAt: new Date().toISOString(),
    expiresAt: null,
    user: {
      id: `demo:${displayNameFromUsername(username).toLowerCase()}`,
      displayName: displayNameFromUsername(username),
    },
    application: {
      key: 'iliniumtech',
      name: 'iLiniumTech',
    },
    currentBrokerId: readPositiveInteger(import.meta.env.VITE_BROKER_ID),
    permissions: DEMO_PERMISSIONS,
  }

  storage()?.setItem(SESSION_STORAGE_KEY, JSON.stringify(nextSession))
  session.value = nextSession
  return nextSession
}

function toBackendSession(data: BackendLoginResponse): AuthMvpSession {
  return {
    mode: 'demo',
    source: 'backend',
    sessionId: `backend-${Date.now()}`,
    createdAt: new Date().toISOString(),
    expiresAt: data.session.expiresAt,
    user: data.user,
    application: data.application,
    currentBrokerId: data.currentBrokerId,
    permissions: data.permissions,
  }
}

function toBackendLoginMessage(error: unknown) {
  if (!axios.isAxiosError<BackendAuthErrorResponse>(error)) {
    return 'No se pudo iniciar sesion.'
  }

  const status = error.response?.status
  const code = error.response?.data?.error?.code

  if (status === 400 || code === 'AUTH_VALIDATION_ERROR') {
    return 'Introduce usuario y contrasena.'
  }

  if (status === 401 || code === 'AUTH_INVALID_CREDENTIALS') {
    return 'Credenciales no validas.'
  }

  if (status === 403 || code === 'AUTH_DEMO_DISABLED') {
    return 'El login demo backend no esta habilitado en este entorno.'
  }

  return 'No se pudo iniciar sesion.'
}

export async function loginWithBackendDemo(credentials: LoginCredentials): Promise<AuthMvpSession> {
  const username = credentials.username.trim()
  if (!username || !credentials.password) {
    throw new Error('Introduce usuario y contrasena.')
  }

  const brokerId = readPositiveInteger(import.meta.env.VITE_BROKER_ID)
  let response
  try {
    response = await apiClient.post<BackendLoginResponse>('/api/auth/login', {
      username,
      password: credentials.password,
      brokerId,
    })
  } catch (error) {
    const translatedError = new Error(toBackendLoginMessage(error)) as Error & { cause?: unknown }
    translatedError.cause = error
    throw translatedError
  }

  const nextSession = toBackendSession(response.data)

  storage()?.setItem(SESSION_STORAGE_KEY, JSON.stringify(nextSession))
  session.value = nextSession
  return nextSession
}

export async function loginAuthSession(credentials: LoginCredentials): Promise<AuthMvpSession> {
  const runtimeConfig = getRuntimeConfig()
  return runtimeConfig.backendEnabled && runtimeConfig.authMode === 'demo-session'
    ? loginWithBackendDemo(credentials)
    : loginDemo(credentials)
}

export function clearAuthSession() {
  storage()?.removeItem(SESSION_STORAGE_KEY)
  session.value = null
}

export async function logoutAuthSession() {
  const runtimeConfig = getRuntimeConfig()
  clearAuthSession()

  if (runtimeConfig.backendEnabled && runtimeConfig.authMode === 'demo-session') {
    try {
      await apiClient.post('/api/auth/logout')
    } catch {
      // Local logout must not keep the user trapped if the backend endpoint is unavailable.
    }
  }
}

export function useAuthSession() {
  const isAuthenticated = computed(() => session.value !== null)
  const userLabel = computed(() => session.value?.user.displayName ?? 'Usuario MVP')

  function reloadSession() {
    session.value = readStoredSession()
    return session.value
  }

  return {
    session: readonly(session),
    isAuthenticated,
    userLabel,
    login: loginAuthSession,
    logout: logoutAuthSession,
    reloadSession,
  }
}
