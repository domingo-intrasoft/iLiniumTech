<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Polizas',
  title: 'Polizas Colectivas',
  subtitle:
    'Vista MVP estatica para el scope Colectivas, pendiente de regla funcional, permiso y contrato API.',
  status: 'Scope aparcado hasta SDD',
  source: 'Fuente documental sanitizada: submenus de Polizas.',
  actions: [
    { label: 'Filtrar colectivas', icon: 'pi pi-filter' },
    { label: 'Ver poliza', icon: 'pi pi-eye' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Scope', value: 'Colectivas', tone: 'pending' },
    { label: 'Regla', value: 'Pendiente', tone: 'risk' },
    { label: 'Modo', value: 'Solo lectura', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Pagina estatica para reservar el alcance de polizas colectivas.',
      'No infiere filtros por ramo, division, tipo o etiqueta hasta validacion funcional.',
      'No consulta API ni activa navegacion hasta tener permiso de producto.',
    ],
  },
  nextSteps: {
    title: 'Desbloqueo necesario',
    items: [
      'Confirmar si Colectivas sera pagina, filtro de polizas o vertical independiente.',
      'Definir enum backend cerrado, permiso polizas.scope.colectivas.read y pruebas.',
      'Validar muestras con producto y DBA sin guardar datos reales.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Usar un nombre de menu como regla de base de datos puede falsear resultados.',
      'Filtrar por campos no confirmados puede mezclar polizas de otros alcances.',
      'Activar exportaciones o detalle sin permisos ampliaria exposicion de PII.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
