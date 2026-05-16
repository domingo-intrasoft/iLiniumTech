<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'
import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const agendaPage: MvpPageDefinition = {
  sectionTitle: 'Agenda',
  title: 'Agenda',
  subtitle:
    'Vista MVP estatica para abrir el modulo sin activar calendario dinamico ni metadata heredada.',
  status: 'Bloqueado externo: evidencia parcial, sin SDD funcional',
  source: 'Fuente: docs/appbuilder/pages/agenda/README.md, usada solo como evidencia documental.',
  actions: [
    { label: 'Consultar eventos', icon: 'pi pi-calendar' },
    { label: 'Crear evento', icon: 'pi pi-plus' },
    { label: 'Reprogramar', icon: 'pi pi-arrows-alt' },
  ],
  metrics: [
    { label: 'Pantalla', value: 'Placeholder estatico', tone: 'pending' },
    { label: 'Datos', value: 'No conectado', tone: 'risk' },
    { label: 'Escritura', value: 'Deshabilitada', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Resumen read-only del modulo Agenda para iniciar la pagina sin router ni API nuevos.',
      'Eventos, asunto, fechas, prioridad y objeto relacionado quedan como hipotesis pendiente de SDD.',
      'Alta, edicion, borrado, drag-and-drop y workflows quedan fuera del MVP inicial.',
      'Las acciones visibles permanecen deshabilitadas hasta permisos efectivos, UAT y contrato API.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Obtener metadata sanitizada o validacion funcional sobre la pantalla Agenda real.',
      'Crear SDD Agenda read-only con rango de fechas, permisos agenda.read/agenda.detail y minimizacion PII.',
      'Confirmar con DBA Agenda, vw_Agenda, zonas horarias y trigger de siniestros antes de conectar datos.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Asunto y descripcion pueden contener PII o informacion sensible.',
      'BrokerIntegracionId obliga a validar aislamiento multi-tenant antes de consultar datos.',
      'La evidencia del calendario generico no confirma la composicion real de la pagina.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="agendaPage" />
</template>
