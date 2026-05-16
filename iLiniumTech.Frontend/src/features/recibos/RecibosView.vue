<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Recibos',
  title: 'Recibos',
  subtitle:
    'Pagina MVP estatica para ordenar el futuro listado de recibos sin conectar datos heredados en ejecucion.',
  status: 'MVP bloqueado hasta SDD/API',
  source: 'Fuente documental sanitizada: README de Recibos.',
  actions: [
    { label: 'Buscar recibos', icon: 'pi pi-search' },
    { label: 'Ver detalle', icon: 'pi pi-eye' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Estado', value: 'Bloqueado externo', tone: 'risk' },
    { label: 'Datos', value: 'Sin API', tone: 'pending' },
    { label: 'Modo', value: 'Solo lectura', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Hueco estatico para listado y detalle read-only de recibos.',
      'Campos candidatos: poliza, cliente minimizado, situacion, fechas, cobro e importes revisados.',
      'Datos bancarios, documentos, emails, telefonos y direcciones quedan fuera del primer corte.',
    ],
  },
  nextSteps: {
    title: 'Desbloqueo necesario',
    items: [
      'Aprobar una SDD de recibos con columnas, filtros, permisos y UAT owner.',
      'Confirmar con DBA la vista o tabla autorizada y la regla por broker.',
      'Definir catalogos y contrato API antes de conectar datos reales.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Exposicion de PII, datos bancarios o importes sin permiso especifico.',
      'Confundir la busqueda generica heredada con un contrato de producto.',
      'Lectura cruzada entre brokers si el backend no valida contexto y permisos.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
