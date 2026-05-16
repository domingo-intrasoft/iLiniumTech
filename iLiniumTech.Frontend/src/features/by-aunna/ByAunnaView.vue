<script setup lang="ts">
import MvpPageShell from '@/features/mvp-pages/MvpPageShell.vue'

import type { MvpPageDefinition } from '@/features/mvp-pages/mvpPageTypes'

const page: MvpPageDefinition = {
  sectionTitle: 'By Aunna',
  title: 'By Aunna',
  subtitle:
    'Pagina MVP estatica para reservar el area de marca sin asumir contenido, datos o rutas funcionales.',
  status: 'Bloqueado externo',
  source: 'Fuente: inventario documental sanitizado de By Aunna.',
  actions: [
    { label: 'Validar contenido', icon: 'pi pi-check-circle' },
    { label: 'Revisar enlaces', icon: 'pi pi-link' },
  ],
  metrics: [
    { label: 'Evidencia funcional', value: 'No confirmada', tone: 'risk' },
    { label: 'Origen de datos', value: 'Pendiente', tone: 'pending' },
    { label: 'Acciones', value: 'Deshabilitadas', tone: 'pending' },
  ],
  scope: {
    title: 'Alcance MVP',
    items: [
      'Entrada futura relacionada con marca o tenant, todavia sin pantalla verificada.',
      'No usa tema visual heredado como contrato funcional.',
      'No publica contenido, documentos, enlaces externos ni administracion de marca.',
    ],
  },
  nextSteps: {
    title: 'Siguientes pasos',
    items: [
      'Confirmar con producto si By Aunna es contenido, branding, enlaces o modulo nuevo.',
      'Obtener evidencia sanitizada si existe una pantalla heredada real.',
      'Crear SDD, permisos y API explicita antes de activar la pagina.',
    ],
  },
  risks: {
    title: 'Riesgos',
    items: [
      'Confundir branding con una pagina de negocio aprobada.',
      'Publicar contenido o enlaces sin owner funcional ni UAT.',
      'Reintroducir Builder/runtime mediante menus o temas heredados.',
    ],
  },
}
</script>

<template>
  <MvpPageShell :page="page" />
</template>
