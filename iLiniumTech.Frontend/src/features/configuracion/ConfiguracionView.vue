<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Configuracion',
  title: 'Configuracion',
  subtitle:
    'Superficie sensible aparcada: no replica Builder/runtime ni muestra valores de entorno o secretos.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de Configuracion.',
  actions: [
    { label: 'Revisar politica', icon: 'pi pi-lock' },
    { label: 'Ver auditoria', icon: 'pi pi-history' },
  ],
  metrics: [
    { label: 'Superficie', value: 'Sensible', tone: 'risk' },
    { label: 'Valores secretos', value: 'No expuestos', tone: 'ready' },
    { label: 'Acciones', value: 'Deshabilitadas', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Vista estatica para separar preferencias de usuario y configuracion administrativa futura.',
      'No lista claves heredadas, valores por entorno, proveedores, conexiones ni secretos.',
      'No permite editar branding, seguridad, integraciones ni parametros tecnicos.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Decidir si Configuracion sera personal, administrativa o ambas con areas separadas.',
      'Definir valores permitidos, valores prohibidos, redaccion y auditoria.',
      'Crear API explicita con permisos antes de cualquier lectura o escritura.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Fuga de secretos, URLs internas, proveedores de login o configuracion por entorno.',
      'Cambios de seguridad o integraciones sin rollback ni auditoria.',
      'Reintroducir Builder/runtime mediante claves heredadas como contrato de UI.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
