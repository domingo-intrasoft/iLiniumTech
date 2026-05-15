import {
  createRouter,
  createWebHistory,
  type Router,
  type RouterHistory,
  type RouteRecordRaw,
} from 'vue-router'

import AutosParticularesView from '@/features/autos-particulares/AutosParticularesView.vue'
import LoginView from '@/features/auth/LoginView.vue'
import PolizaDetailView from '@/features/polizas/PolizaDetailView.vue'
import PolizasView from '@/features/polizas/PolizasView.vue'
import { hasAuthSession } from '@/features/auth/authSession'

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: '/polizas',
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { public: true },
  },
  {
    path: '/polizas',
    name: 'polizas',
    component: PolizasView,
    meta: { requiresAuth: true },
  },
  {
    path: '/autos-particulares',
    name: 'autos-particulares',
    component: AutosParticularesView,
    meta: { requiresAuth: true },
  },
  {
    path: '/polizas/:id',
    name: 'poliza-detail',
    component: PolizaDetailView,
    meta: { requiresAuth: true },
  },
]

export function registerAuthGuard(router: Router) {
  router.beforeEach((to) => {
    const authenticated = hasAuthSession()

    if (to.name === 'login' && authenticated) {
      const redirect = to.query.redirect
      const target = Array.isArray(redirect) ? redirect[0] : redirect
      return target?.startsWith('/') && !target.startsWith('//') ? target : { name: 'polizas' }
    }

    if (to.meta.requiresAuth && !authenticated) {
      return {
        name: 'login',
        query: { redirect: to.fullPath },
      }
    }

    return true
  })
}

export function createIliniumRouter(history: RouterHistory = createWebHistory()) {
  const router = createRouter({
    history,
    routes,
  })

  registerAuthGuard(router)
  return router
}

export const router = createIliniumRouter()
