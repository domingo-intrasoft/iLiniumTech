<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Administracion',
  title: 'Administracion',
  subtitle:
    'Superficie sensible aparcada: no replica Builder/runtime ni habilita cambios administrativos.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de Administracion.',
  actions: [
    { label: 'Revisar permisos', icon: 'pi pi-lock' },
    { label: 'Consultar auditoria', icon: 'pi pi-shield' },
  ],
  metrics: [
    { label: 'Superficie', value: 'Sensible', tone: 'risk' },
    { label: 'Builder/runtime', value: 'No replicado', tone: 'ready' },
    { label: 'Acciones', value: 'Deshabilitadas', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Landing estatica de bloqueo para una futura administracion de producto.',
      'No se migra la consola tecnica heredada ni un editor generico de menus.',
      'No se exponen usuarios, permisos, configuracion, conexiones ni procesos.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Definir con producto si la administracion sera funcional, tecnica o auditoria.',
      'Cerrar auth productiva, permisos y threat review antes de cualquier dato real.',
      'Crear SDD y endpoints explicitos por caso de uso aprobado.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Exponer configuracion, usuarios o permisos sin matriz funcional.',
      'Usar estado administrador o cabeceras demo como autoridad productiva.',
      'Reintroducir Builder/runtime mediante pantallas administrativas genericas.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
