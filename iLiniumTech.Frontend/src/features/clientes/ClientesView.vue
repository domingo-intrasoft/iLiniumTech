<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'
import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const clientesPage: MvpPageDefinition = {
  sectionTitle: 'Clientes',
  title: 'Clientes',
  subtitle:
    'Pagina MVP estatica para iniciar Clientes como codigo Vue mantenido, sin renderer dinamico heredado.',
  status: 'Bloqueado externo para desarrollo final: evidencia tecnica parcial',
  source: 'Fuente: docs/appbuilder/pages/clientes/README.md, usada solo como evidencia documental.',
  actions: [
    { label: 'Buscar clientes', icon: 'pi pi-search' },
    { label: 'Abrir ficha', icon: 'pi pi-id-card' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'PII', value: 'Alta', tone: 'risk' },
    { label: 'API', value: 'Pendiente', tone: 'pending' },
    { label: 'Alcance', value: 'Read-only propuesto', tone: 'ready' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Resume un futuro listado read-only minimizado de clientes, no una ficha completa ni tabs dinamicas.',
      'Polizas, recibos, riesgos, siniestros y suplementos quedan como areas candidatas pendientes de SDD.',
      'Documento, contacto, direccion y datos bancarios no deben mostrarse sin permisos y reglas PII.',
      'Las acciones visibles permanecen deshabilitadas hasta permisos efectivos, UAT y contrato API.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Crear SDD Clientes read-only y decidir si la primera entrega sera listado, ficha resumen o tabs relacionadas.',
      'Validar vistas vw_Cliente autorizadas, broker efectivo y claves de contexto con DBA.',
      'Definir permisos clientes.read, clientes.detail y permisos por areas relacionadas.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Clientes concentra PII alta y cualquier fuga entre brokers tendria impacto critico.',
      'Las vistas candidatas no sustituyen metadata de pantalla ni UAT funcional.',
      'Un CRUD generico o tabs desde metadata recrearia el runtime heredado.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="clientesPage" />
</template>
