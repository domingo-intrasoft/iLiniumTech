<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'Liquidaciones',
  title: 'Liquidaciones de colaborador',
  subtitle:
    'MVP estatico para ordenar el alcance read-only de liquidaciones de colaborador sin consultar datos reales.',
  status: 'MVP estatico sin API real',
  source: 'Fuente documental sanitizada: liq-col/README.md.',
  actions: [
    { label: 'Consultar', icon: 'pi pi-search' },
    { label: 'Abrir detalle', icon: 'pi pi-eye' },
    { label: 'Ver conceptos', icon: 'pi pi-list-check' },
    { label: 'Exportar informe', icon: 'pi pi-download' },
  ],
  metrics: [
    { label: 'Pantalla', value: 'Preparada', tone: 'ready' },
    { label: 'API', value: 'No conectada', tone: 'pending' },
    { label: 'Permisos', value: 'Pendientes', tone: 'pending' },
    { label: 'Comisiones', value: 'Protegidas', tone: 'risk' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Placeholder read-only para liquidaciones de colaborador vinculadas con recibos, reparto, tesoreria y reporting.',
      'Listado minimizado: colaborador, fecha, cierre, estado y resumen financiero condicionado a permisos.',
      'Areas candidatas: recibos asociados, conceptos, exclusiones y relacion con liquidacion de compania.',
      'El menu y la ruta real deben seguir bloqueados hasta existir SDD, permiso y API explicita.',
    ],
  },
  nextSteps: {
    title: 'Para desbloquear',
    items: [
      'Confirmar si es modulo independiente o vista/reporting desde colaboradores, recibos o Liq.Cia.',
      'Definir permisos liqCol.read, liqCol.detail, liqCol.financial y liqCol.commissions.',
      'Clasificar documento de colaborador, comisiones, retenciones, liquido y motivos libres.',
      'Aprobar filtros diarios: fecha, colaborador, oficina, estado, poliza, recibo, ramo y compania.',
    ],
  },
  risks: {
    title: 'Riesgos controlados',
    items: [
      'Documento de colaborador y nombres de cliente, mediador o colaborador no deben salir por defecto.',
      'Comisiones, retenciones y liquido requieren permiso funcional y minimizacion.',
      'Motivos libres de exclusion o validacion pueden contener PII y deben revisarse.',
      'Generacion, cierre, recalculo, anulacion y validacion permanecen bloqueados.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
