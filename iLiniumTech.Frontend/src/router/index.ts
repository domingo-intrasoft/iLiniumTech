import { createRouter, createWebHistory } from 'vue-router'

import AutosParticularesView from '@/features/autos-particulares/AutosParticularesView.vue'
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
      path: '/autos-particulares',
      name: 'autos-particulares',
      component: AutosParticularesView,
    },
    {
      path: '/polizas/:id',
      name: 'poliza-detail',
      component: PolizaDetailView,
    },
  ],
})
