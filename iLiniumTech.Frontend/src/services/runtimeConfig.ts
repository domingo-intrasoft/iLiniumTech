export interface RuntimeConfigIssue {
  code:
    | 'ILINIUMTECH_API_KEY_MISSING'
    | 'ILINIUMTECH_API_BASE_URL_INVALID'
    | 'ILINIUMTECH_BROKER_ID_INVALID'
  message: string
}

export interface RuntimeConfig {
  backendEnabled: boolean
  apiBaseUrl: string
  apiKey: string | null
  brokerId: string | null
  issues: RuntimeConfigIssue[]
}

export class RuntimeConfigError extends Error {
  constructor(message: string) {
    super(message)
    this.name = 'RuntimeConfigError'
  }
}

const DEFAULT_API_BASE_URL = 'http://localhost:5146'
const API_KEY_PLACEHOLDERS = new Set([
  '__SET_IN_ENVIRONMENT__',
  'CHANGE_ME',
  'CHANGEME',
  'TODO',
  'REPLACE_ME',
])

function cleanValue(value: string | undefined) {
  const cleaned = value?.trim()
  return cleaned ? cleaned : null
}

function isValidHttpUrl(value: string) {
  try {
    const url = new URL(value)
    return url.protocol === 'http:' || url.protocol === 'https:'
  } catch {
    return false
  }
}

function readBrokerId(value: string | undefined, issues: RuntimeConfigIssue[]) {
  const brokerId = cleanValue(value)

  if (!brokerId) {
    return null
  }

  if (!/^[1-9]\d*$/.test(brokerId)) {
    issues.push({
      code: 'ILINIUMTECH_BROKER_ID_INVALID',
      message: 'VITE_BROKER_ID debe ser un entero positivo cuando se envia al backend.',
    })
    return null
  }

  return brokerId
}

export function getRuntimeConfig(): RuntimeConfig {
  const issues: RuntimeConfigIssue[] = []
  const backendEnabled = import.meta.env.VITE_USE_BACKEND === 'true'
  const rawApiBaseUrl = cleanValue(import.meta.env.VITE_API_BASE_URL) ?? DEFAULT_API_BASE_URL
  const configuredApiKey = cleanValue(import.meta.env.VITE_ILINIUMTECH_API_KEY)
  const apiKeyIsPlaceholder = configuredApiKey
    ? API_KEY_PLACEHOLDERS.has(configuredApiKey.toUpperCase())
    : false
  const apiKey = configuredApiKey && !apiKeyIsPlaceholder ? configuredApiKey : null
  const brokerId = readBrokerId(import.meta.env.VITE_BROKER_ID, issues)

  if (backendEnabled && !isValidHttpUrl(rawApiBaseUrl)) {
    issues.push({
      code: 'ILINIUMTECH_API_BASE_URL_INVALID',
      message: 'VITE_API_BASE_URL debe ser una URL absoluta http/https.',
    })
  }

  if (backendEnabled && !apiKey) {
    issues.push({
      code: 'ILINIUMTECH_API_KEY_MISSING',
      message: 'VITE_ILINIUMTECH_API_KEY debe configurarse fuera de Git para usar la API.',
    })
  }

  return {
    backendEnabled,
    apiBaseUrl: isValidHttpUrl(rawApiBaseUrl) ? rawApiBaseUrl : DEFAULT_API_BASE_URL,
    apiKey,
    brokerId,
    issues,
  }
}

export function getApiHeaders(config = getRuntimeConfig()) {
  return {
    ...(config.apiKey ? { 'X-ILiniumTech-Api-Key': config.apiKey } : {}),
    ...(config.brokerId ? { 'X-Broker-Id': config.brokerId } : {}),
  }
}

export function getBlockingRuntimeConfigMessage(config = getRuntimeConfig()) {
  if (!config.backendEnabled || config.issues.length === 0) {
    return null
  }

  return config.issues.map((issue) => issue.message).join(' ')
}

export function assertRuntimeConfigReady(config = getRuntimeConfig()) {
  const message = getBlockingRuntimeConfigMessage(config)

  if (message) {
    throw new RuntimeConfigError(message)
  }
}
