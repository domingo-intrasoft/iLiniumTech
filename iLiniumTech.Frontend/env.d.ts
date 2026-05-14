/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_USE_BACKEND?: string
  readonly VITE_API_BASE_URL?: string
  readonly VITE_ILINIUMTECH_API_KEY?: string
  readonly VITE_BROKER_ID?: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
