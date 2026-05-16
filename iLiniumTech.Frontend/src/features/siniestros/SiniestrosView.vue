<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Siniestros',
  title: 'Siniestros',
  subtitle:
    'Pagina MVP estatica para preparar un seguimiento read-only de siniestros con datos minimizados.',
  status: 'MVP bloqueado hasta SDD/API',
  source: 'Fuente documental sanitizada: README de Siniestros.',
  actions: [
    { label: 'Buscar siniestros', icon: 'pi pi-search' },
    { label: 'Ver detalle', icon: 'pi pi-eye' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Estado', value: 'Bloqueado externo', tone: 'risk' },
    { label: 'Sensibilidad', value: 'Alta', tone: 'risk' },
    { label: 'Modo', value: 'Solo lectura', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Vista candidata para listado read-only de siniestros.',
      'Campos candidatos: referencia, poliza, situacion, fechas principales, tramitador y compania.',
      'Intervinientes, lesionados, observaciones, vehiculos e importes quedan fuera sin permiso especifico.',
    ],
  },
  nextSteps: {
    title: 'Desbloqueo necesario',
    items: [
      'Aprobar SDD con alcance, columnas, filtros, permisos sensibles y UAT owner.',
      'Confirmar con DBA la fuente autorizada y la regla multi-broker.',
      'Separar EIAC, aduana, agenda y pagos si entran en un incremento posterior.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Exposicion de PII, salud, vehiculos, textos libres o importes.',
      'Mezclar seguimiento, EIAC, aduana y reporting sin frontera funcional.',
      'Revelar existencia de siniestros cruzados por errores 403 o 404 mal definidos.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
