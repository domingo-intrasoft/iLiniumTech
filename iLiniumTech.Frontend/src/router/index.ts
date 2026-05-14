import { createRouter, createWebHistory } from 'vue-router'

import PolizaDetailView from '@/features/polizas/PolizaDetailView.vue'
import PolizasView from '@/features/polizas/PolizasView.vue'

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/polizas',
    },
    {
      path: '/polizas',
      name: 'polizas',
      component: PolizasView,
    },
    {
      path: '/polizas/:id',
      name: 'poliza-detail',
      component: PolizaDetailView,
    },
  ],
})
