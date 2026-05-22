import { createRouter, createWebHistory } from 'vue-router'

import PolizasView from '@/features/polizas/PolizasView.vue'
import RecibosView from '@/features/recibos/RecibosView.vue'
import RiesgosView from '@/features/riesgos/RiesgosView.vue'

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
      path: '/recibos',
      name: 'recibos',
      component: RecibosView,
    },
    {
      path: '/riesgos',
      name: 'riesgos',
      component: RiesgosView,
    },
  ],
})
