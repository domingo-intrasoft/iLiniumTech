<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Reporting',
  title: 'Informes',
  subtitle:
    'MVP estatico para inventariar reporting aprobado sin ejecutar informes, abrir descargas ni llamar a API real.',
  status: 'MVP estatico sin API real',
  source: 'Fuente documental sanitizada: informes/README.md.',
  actions: [
    { label: 'Ver catalogo', icon: 'pi pi-list' },
    { label: 'Ejecutar informe', icon: 'pi pi-play' },
    { label: 'Descargar', icon: 'pi pi-download' },
    { label: 'Ver historial', icon: 'pi pi-history' },
  ],
  metrics: [
    { label: 'Catalogo', value: 'Pendiente', tone: 'pending' },
    { label: 'Ejecucion', value: 'Bloqueada', tone: 'risk' },
    { label: 'Descargas', value: 'Bloqueadas', tone: 'risk' },
    { label: 'Permisos', value: 'Por informe', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Catalogo estatico de capacidades de reporting, sin ejecucion ni descarga.',
      'Areas candidatas: polizas, recibos, riesgos, liquidaciones, facturas, SEPA, remesas y cliente-polizas.',
      'Parametros reales por informe no confirmados; fechas, broker, oficina, compania, colaborador y formato son hipotesis.',
      'Cada informe futuro debe tener DTO, parametros, permisos, auditoria y minimizacion propios.',
    ],
  },
  nextSteps: {
    title: 'Para desbloquear',
    items: [
      'Aprobar inventario de informes, propietarios funcionales, formatos, campos y permisos.',
      'Definir permisos informes.read, informes.execute, informes.download e informes.admin.',
      'Disenar una API explicita por catalogo y ejecuciones autorizadas, con errores sanitizados.',
      'Revisar seguridad de documentos generados, retencion, cache, destinatarios y auditoria.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Exportaciones PDF o Excel pueden filtrar PII de forma masiva.',
      'Importes, comisiones, liquidaciones, facturas, datos bancarios y SEPA requieren permisos.',
      'Launcher externo, enlaces o codigos no aprobados pueden abrir exfiltracion u open redirect.',
      'Nombres tecnicos de vistas no equivalen a informes de negocio aprobados.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
