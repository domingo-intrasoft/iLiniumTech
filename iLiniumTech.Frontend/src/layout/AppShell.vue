<script setup lang="ts">
import AppSideMenu from './AppSideMenu.vue'

withDefaults(
  defineProps<{
    contentId: string
    sectionTitle: string
    sessionLabel: string
    sessionNeedsAttention?: boolean
    topBadges?: string[]
    userLabel?: string
    showSignOut?: boolean
  }>(),
  {
    sessionNeedsAttention: false,
    topBadges: () => [],
    userLabel: 'MVP iLiniumTech',
    showSignOut: false,
  },
)
</script>

<template>
  <main class="ilinium-shell">
    <a class="skip-link" :href="`#${contentId}`">Saltar al contenido</a>
    <AppSideMenu />

    <section class="workspace">
      <header class="workspace-topbar">
        <div class="breadcrumb-line">
          <button class="icon-button" type="button" aria-label="Menu">
            <i class="pi pi-bars" aria-hidden="true"></i>
          </button>
          <strong>Inicio</strong>
          <span>/</span>
          <strong>{{ sectionTitle }}</strong>
          <span>/</span>
        </div>

        <span class="environment-badge" :class="{ warning: sessionNeedsAttention }">
          {{ sessionLabel }}
        </span>

        <div class="top-actions" aria-label="Acciones de usuario">
          <span v-for="badge in topBadges" :key="badge" class="round-badge">{{ badge }}</span>
          <span class="user-name">{{ userLabel }}</span>
          <button class="icon-button ghost" type="button" aria-label="Notificaciones">
            <i class="pi pi-bell" aria-hidden="true"></i>
          </button>
          <button class="icon-button ghost" type="button" aria-label="Configuracion">
            <i class="pi pi-cog" aria-hidden="true"></i>
          </button>
          <button v-if="showSignOut" class="icon-button ghost" type="button" aria-label="Salir">
            <i class="pi pi-sign-out" aria-hidden="true"></i>
          </button>
        </div>
      </header>

      <slot />
    </section>
  </main>
</template>
