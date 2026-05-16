<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Logs',
  title: 'Logs',
  subtitle:
    'Superficie sensible aparcada: no replica Builder/runtime ni muestra payloads, trazas o datos personales.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de Logs.',
  actions: [
    { label: 'Filtrar logs', icon: 'pi pi-filter' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Superficie', value: 'Sensible', tone: 'risk' },
    { label: 'Datos', value: 'Redaccion obligatoria', tone: 'risk' },
    { label: 'Acciones', value: 'Deshabilitadas', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Vista estatica para bloquear una futura consulta segura de auditoria y operacion.',
      'No consulta tablas de logs, cuerpos de mensajes, payloads, tokens ni trazas.',
      'No permite detalle, descarga, reintento, borrado ni exportacion real.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Definir owner, fuente autorizada, retencion, rango maximo y minimizacion.',
      'Crear matriz de permisos para auditoria, errores, servicios, mail y SMS.',
      'Disenar API explicita con filtros parametrizados y errores sanitizados.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Fuga de PII, secretos, endpoints internos, cookies o detalles de infraestructura.',
      'Enumeracion masiva de actividad sin rango temporal ni paginacion estricta.',
      'Reintroducir Builder/runtime mediante lectura generica de logs heredados.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
