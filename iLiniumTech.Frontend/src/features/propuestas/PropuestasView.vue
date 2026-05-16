<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'
import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const propuestasPage: MvpPageDefinition = {
  sectionTitle: 'Propuestas',
  title: 'Propuestas',
  subtitle:
    'Pagina MVP estatica para hacer visible el alcance futuro sin confundir Propuestas con Solicitudes.',
  status: 'Bloqueado externo: sin metadata concreta ni origen de datos confirmado',
  source:
    'Fuente: docs/appbuilder/pages/propuestas/README.md, usada solo como evidencia documental.',
  actions: [
    { label: 'Listar propuestas', icon: 'pi pi-list' },
    { label: 'Crear propuesta', icon: 'pi pi-plus' },
    { label: 'Convertir a poliza', icon: 'pi pi-send' },
  ],
  metrics: [
    { label: 'Origen datos', value: 'No confirmado', tone: 'risk' },
    { label: 'Escrituras', value: 'Fuera de alcance', tone: 'ready' },
    { label: 'Permisos', value: 'Pendientes', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Recoge una propuesta read-only futura: listado, filtros y detalle solo si SDD confirma el caso de uso.',
      'No asume que Propuestas sea Solicitudes ni que exista una base concreta sin validacion.',
      'Creacion, edicion, conversion, tarificacion, documentos y workflows quedan bloqueados.',
      'Las acciones visibles permanecen deshabilitadas hasta permisos efectivos, UAT y contrato API.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Obtener menu/componente sanitizado o especificacion funcional equivalente para Propuestas.',
      'Crear SDD con origen de datos, filtros, estados, permisos y UAT owner.',
      'Definir API explicita de lectura y matriz de permisos antes de exponer acciones.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Confundir nombre de menu con flujo de emision puede generar una pantalla inventada.',
      'PII y estados de propuesta requieren minimizacion y permisos antes de detalle.',
      'Resolver conexion o ejecutar flujos heredados sin broker validado abriria riesgo multi-tenant.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="propuestasPage" />
</template>
