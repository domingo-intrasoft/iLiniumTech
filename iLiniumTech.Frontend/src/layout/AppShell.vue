<script setup lang="ts">
import { ref } from 'vue'

import AppSideMenu from './AppSideMenu.vue'

const emit = defineEmits<{
  signOut: []
  brokerChange: [brokerId: number]
}>()

const sideMenuOpen = ref(true)
const placeholderTopbarActionDescription =
  'Acciones de cabecera no disponibles en el MVP hasta SDD y contrato funcional.'

const props = withDefaults(
  defineProps<{
    contentId: string
    sectionTitle: string
    sessionLabel: string
    sessionNeedsAttention?: boolean
    topBadges?: string[]
    userLabel?: string
    showSignOut?: boolean
    brokerOptions?: number[]
    activeBrokerId?: number | null
    brokerChanging?: boolean
    brokerError?: string | null
    appbuilderChrome?: boolean
  }>(),
  {
    sessionNeedsAttention: false,
    topBadges: () => [],
    userLabel: 'MVP iLiniumTech',
    showSignOut: false,
    brokerOptions: () => [],
    activeBrokerId: null,
    brokerChanging: false,
    brokerError: null,
    appbuilderChrome: false,
  },
)

function onBrokerSelection(event: Event) {
  const selectedBrokerId = Number((event.target as HTMLSelectElement).value)
  if (Number.isInteger(selectedBrokerId) && selectedBrokerId > 0) {
    emit('brokerChange', selectedBrokerId)
  }
}

function toggleSideMenu() {
  sideMenuOpen.value = !sideMenuOpen.value
}

function focusMainContent(event: MouseEvent) {
  const content = document.getElementById(props.contentId)

  if (!content) {
    return
  }

  event.preventDefault()

  if (!content.hasAttribute('tabindex')) {
    content.setAttribute('tabindex', '-1')
  }

  content.focus({ preventScroll: false })
}
</script>

<template>
  <main
    class="ilinium-shell"
    :class="{ 'menu-collapsed': !sideMenuOpen, 'appbuilder-chrome': appbuilderChrome }"
  >
    <a class="skip-link" :href="`#${contentId}`" @click="focusMainContent">Saltar al contenido</a>
    <div v-if="appbuilderChrome" class="demo-environment-strip">
      <i class="pi pi-exclamation-triangle" aria-hidden="true"></i>
      ENTORNO DEMO - Los cambios NO se reflejan en produccion
    </div>
    <AppSideMenu
      id="app-side-menu"
      :appbuilder-chrome="appbuilderChrome"
      :aria-hidden="!sideMenuOpen"
      :inert="sideMenuOpen ? undefined : true"
    />

    <section class="workspace">
      <header class="workspace-topbar">
        <nav class="breadcrumb-line" aria-label="Ruta actual">
          <button
            class="icon-button"
            type="button"
            :aria-label="sideMenuOpen ? 'Ocultar menu' : 'Mostrar menu'"
            aria-controls="app-side-menu"
            :aria-expanded="sideMenuOpen"
            :title="sideMenuOpen ? 'Ocultar menu' : 'Mostrar menu'"
            @click="toggleSideMenu"
          >
            <i class="pi pi-bars" aria-hidden="true"></i>
          </button>
          <strong>Inicio</strong>
          <span>/</span>
          <strong aria-current="page">{{ sectionTitle }}</strong>
          <span v-if="appbuilderChrome">/</span>
        </nav>

        <span class="environment-badge" :class="{ warning: sessionNeedsAttention }">
          {{ appbuilderChrome ? 'AunnaTech | Portal (DEMO)' : sessionLabel }}
        </span>
        <span v-if="appbuilderChrome" class="sr-only">{{ sessionLabel }}</span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <p id="app-shell-placeholder-actions" class="sr-only">
            {{ placeholderTopbarActionDescription }}
          </p>
          <label v-if="brokerOptions.length > 0" class="broker-selector">
            <span class="sr-only">Broker activo</span>
            <i class="pi pi-building" aria-hidden="true"></i>
            <select
              :value="activeBrokerId ?? ''"
              :disabled="brokerChanging"
              :aria-invalid="brokerError ? 'true' : 'false'"
              :aria-describedby="brokerError ? 'app-shell-broker-error' : undefined"
              aria-label="Broker activo"
              @change="onBrokerSelection"
            >
              <option v-for="brokerId in brokerOptions" :key="brokerId" :value="brokerId">
                Broker {{ brokerId }}
              </option>
            </select>
            <span v-if="brokerError" id="app-shell-broker-error" class="sr-only">
              {{ brokerError }}
            </span>
          </label>
          <span v-for="badge in topBadges" :key="badge" class="round-badge">{{ badge }}</span>
          <span class="user-name">{{ userLabel }}</span>
          <button
            class="icon-button ghost"
            type="button"
            aria-label="Notificaciones no disponibles en el MVP"
            title="Notificaciones pendientes de SDD"
            aria-describedby="app-shell-placeholder-actions"
            disabled
          >
            <i class="pi pi-bell" aria-hidden="true"></i>
          </button>
          <button
            class="icon-button ghost"
            type="button"
            aria-label="Configuracion no disponible desde esta accion"
            title="Usa el menu lateral; la configuracion funcional sigue bloqueada por SDD"
            aria-describedby="app-shell-placeholder-actions"
            disabled
          >
            <i class="pi pi-cog" aria-hidden="true"></i>
          </button>
          <button
            v-if="showSignOut"
            class="icon-button ghost"
            type="button"
            aria-label="Salir"
            title="Cerrar sesion"
            @click="emit('signOut')"
          >
            <i class="pi pi-sign-out" aria-hidden="true"></i>
          </button>
        </div>
      </header>

      <slot />
    </section>
  </main>
</template>
