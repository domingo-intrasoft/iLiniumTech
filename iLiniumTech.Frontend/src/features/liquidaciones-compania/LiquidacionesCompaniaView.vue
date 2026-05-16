<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Liquidaciones',
  title: 'Liquidaciones de compania',
  subtitle:
    'MVP estatico para revisar el alcance read-only de liquidaciones de compania sin abrir datos reales ni API.',
  status: 'MVP estatico sin API real',
  source: 'Fuente documental sanitizada: liq-cia/README.md.',
  actions: [
    { label: 'Buscar liquidaciones', icon: 'pi pi-search' },
    { label: 'Abrir detalle', icon: 'pi pi-eye' },
    { label: 'Ver desglose', icon: 'pi pi-list' },
    { label: 'Exportar', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Pantalla', value: 'Preparada', tone: 'ready' },
    { label: 'API', value: 'No conectada', tone: 'pending' },
    { label: 'Permisos', value: 'Por definir', tone: 'pending' },
    { label: 'Datos', value: 'Minimizados', tone: 'risk' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Listado read-only como placeholder revisable para liquidaciones de compania.',
      'Campos candidatos: compania, fechas, cierre, total minimizado, oficina y estado funcional pendiente.',
      'Detalle, recibos, conceptos y desglose financiero quedan como areas candidatas sin datos reales.',
      'El broker debera venir del contexto autorizado cuando exista una API explicita.',
    ],
  },
  nextSteps: {
    title: 'Para desbloquear',
    items: [
      'Cerrar SDD/UAT con columnas, filtros diarios y frontera entre consulta, conciliacion y cierre.',
      'Definir permisos liqCia.read, liqCia.detail y liqCia.financial antes de abrir importes.',
      'Validar fuente read-only y minimizacion de banco, facturas, comentarios e importes.',
      'Crear endpoints explicitos solo cuando el contrato funcional y de seguridad este aprobado.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Importes, comisiones, facturas y datos bancarios no deben mostrarse por defecto.',
      'PII posible en cliente, poliza, nombre completo y observaciones requiere mascara o permiso.',
      'Cierre, validacion, importacion, conciliacion y facturacion permanecen bloqueados.',
      'Errores 403/404 no deben revelar liquidaciones de otro broker.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
