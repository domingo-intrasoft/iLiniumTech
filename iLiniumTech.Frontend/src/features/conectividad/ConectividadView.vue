<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Conectividad',
  title: 'Conectividad',
  subtitle:
    'Superficie sensible aparcada: no replica Builder/runtime ni ejecuta pruebas de integracion.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de Conectividad.',
  actions: [
    { label: 'Probar integracion', icon: 'pi pi-bolt' },
    { label: 'Revisar auditoria', icon: 'pi pi-shield' },
  ],
  metrics: [
    { label: 'Superficie', value: 'Sensible', tone: 'risk' },
    { label: 'Pruebas externas', value: 'Deshabilitadas', tone: 'pending' },
    { label: 'Secretos', value: 'No expuestos', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Vista estatica para bloquear una futura consola segura de integraciones.',
      'No edita conectores, no acepta URLs libres y no muestra tokens o credenciales.',
      'No ejecuta REST, SOAP, preview ni importaciones desde configuracion heredada.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Confirmar si el caso de uso es salud operativa, soporte o administracion tecnica.',
      'Definir allowlists, secret store, timeouts, auditoria y permisos.',
      'Crear endpoints explicitos antes de cualquier prueba real desde backend.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'SSRF o llamadas externas no autorizadas si se permite URL libre.',
      'Fuga de tokens, payloads, endpoints internos o respuestas con datos sensibles.',
      'Reintroducir Builder/runtime mediante un editor generico de integraciones.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
