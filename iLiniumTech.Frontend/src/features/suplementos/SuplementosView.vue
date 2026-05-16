<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Suplementos',
  title: 'Suplementos',
  subtitle:
    'Pagina MVP estatica para preparar el futuro seguimiento de suplementos vinculados a polizas.',
  status: 'MVP bloqueado hasta SDD/API',
  source: 'Fuente documental sanitizada: README de Suplementos.',
  actions: [
    { label: 'Buscar suplementos', icon: 'pi pi-search' },
    { label: 'Ver detalle', icon: 'pi pi-eye' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Pantalla', value: 'Estatica', tone: 'ready' },
    { label: 'API', value: 'Pendiente', tone: 'pending' },
    { label: 'UAT', value: 'Bloqueado', tone: 'risk' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Vista read-only candidata para suplementos relacionados con una poliza.',
      'Campos candidatos: referencia, tipo, situacion, fecha de efecto, concepto y resumen del cambio.',
      'Tomador, beneficiarios, documentos, emails, telefonos, direcciones e IBAN quedan fuera por defecto.',
    ],
  },
  nextSteps: {
    title: 'Desbloqueo necesario',
    items: [
      'Localizar metadata sanitizada o aprobar SDD funcional de suplementos.',
      'Confirmar si el modulo es pantalla independiente, subflujo de polizas o ambos.',
      'Definir contrato API, permisos y minimizacion antes de conectar datos.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Confundir tablas especializadas con pestanas reales de producto.',
      'Activar altas, modificaciones, importaciones o comunicaciones sin SDD.',
      'Exponer PII, IBAN o importes por reutilizar campos heredados sin clasificacion.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
