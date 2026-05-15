import { computed, readonly, ref } from 'vue'

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
  sessionId: string
  createdAt: string
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

  return {
    mode: 'demo',
    sessionId: candidate.sessionId,
    createdAt: candidate.createdAt,
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
}

export function readStoredSession(): AuthMvpSession | null {
  try {
    const rawSession = storage()?.getItem(SESSION_STORAGE_KEY)
    return rawSession ? normalizeStoredSession(JSON.parse(rawSession)) : null
  } catch {
    return null
  }
}

export function hasAuthSession() {
  return readStoredSession() !== null
}

export function loginDemo(credentials: LoginCredentials): AuthMvpSession {
  const username = credentials.username.trim()
  if (!username || !credentials.password) {
    throw new Error('Introduce usuario y contrasena.')
  }

  const nextSession: AuthMvpSession = {
    mode: 'demo',
    sessionId: createSessionId(),
    createdAt: new Date().toISOString(),
    user: {
      id: `demo:${username.toLowerCase()}`,
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

export function clearAuthSession() {
  storage()?.removeItem(SESSION_STORAGE_KEY)
  session.value = null
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
    login: loginDemo,
    logout: clearAuthSession,
    reloadSession,
  }
}
