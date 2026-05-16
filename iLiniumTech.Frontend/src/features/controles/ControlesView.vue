<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Controles',
  title: 'Controles',
  subtitle:
    'Pagina MVP estatica para reservar la superficie sin inventar una pantalla funcional ni replicar Builder/runtime.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de Controles.',
  actions: [
    { label: 'Validar alcance', icon: 'pi pi-check-circle' },
    { label: 'Revisar evidencia', icon: 'pi pi-search' },
  ],
  metrics: [
    { label: 'Evidencia funcional', value: 'No confirmada', tone: 'risk' },
    { label: 'Contrato API', value: 'Pendiente', tone: 'pending' },
    { label: 'Acciones', value: 'Deshabilitadas', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Entrada futura del menu, todavia sin ruta ni permiso de producto.',
      'No hay pantalla heredada confirmada con raiz, hijos, filtros o datos.',
      'No se convierte el catalogo tecnico de controles en producto final.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Confirmar con producto que significa Controles en iLiniumTech.',
      'Obtener evidencia sanitizada de menu y arbol si existe una pantalla real.',
      'Crear SDD y contrato API explicito antes de activar lectura de datos.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Confundir un placeholder de navegacion con funcionalidad aprobada.',
      'Mezclar controles tecnicos heredados con una pagina de negocio.',
      'Reintroducir Builder/runtime mediante componentes genericos.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
