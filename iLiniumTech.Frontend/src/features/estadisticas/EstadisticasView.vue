<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Reporting',
  title: 'Estadisticas',
  subtitle:
    'MVP estatico para preparar estadisticas agregadas sin KPIs reales, graficos conectados ni API abierta.',
  status: 'MVP estatico sin API real',
  source: 'Fuente documental sanitizada: estadisticas/README.md.',
  actions: [
    { label: 'Refrescar', icon: 'pi pi-refresh' },
    { label: 'Cambiar periodo', icon: 'pi pi-calendar' },
    { label: 'Exportar', icon: 'pi pi-download' },
    { label: 'Drilldown', icon: 'pi pi-chart-line' },
  ],
  metrics: [
    { label: 'KPIs', value: 'No confirmados', tone: 'pending' },
    { label: 'Graficos', value: 'Sin datos', tone: 'pending' },
    { label: 'Permisos', value: 'Pendientes', tone: 'pending' },
    { label: 'Privacidad', value: 'Riesgo agregado', tone: 'risk' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Panel placeholder de estadisticas agregadas, sin KPIs reales ni graficos conectados.',
      'Hipotesis pendientes: periodo, broker, ramo, compania, oficina, gestor, estado y comparativa anual.',
      'Metricas candidatas: polizas activas, nuevas polizas, anulaciones, recibos, primas, siniestros y suplementos.',
      'Los graficos futuros deben ser componentes Vue estaticos con contratos de API explicitos.',
    ],
  },
  nextSteps: {
    title: 'Para desbloquear',
    items: [
      'Nombrar owner funcional para metricas, filtros, criterios UAT y supresion de grupos pequenos.',
      'Definir permisos estadisticas.read, estadisticas.export y estadisticas.drilldown.',
      'Crear API explicita para catalogos, resumen, series y distribuciones autorizadas.',
      'Validar origen de datos, reglas de agregacion, rendimiento y aislamiento por broker.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Agregados con pocos registros pueden permitir reidentificacion.',
      'Drilldown con primas, documentos, clientes o matriculas requiere permiso y minimizacion.',
      'Cache o filtros mal aislados pueden mezclar brokers, oficinas, gestores o perfiles.',
      'Un dashboard generico inventaria metricas no aprobadas y dificulta UAT.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
