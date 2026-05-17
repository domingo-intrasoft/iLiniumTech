import {
  createRouter,
  createWebHistory,
  type Router,
  type RouterHistory,
  type RouteRecordRaw,
} from 'vue-router'

import AdministracionView from '@/features/administracion/AdministracionView.vue'
import AgendaView from '@/features/agenda/AgendaView.vue'
import AutosParticularesView from '@/features/autos-particulares/AutosParticularesView.vue'
import LoginView from '@/features/auth/LoginView.vue'
import ByAunnaView from '@/features/by-aunna/ByAunnaView.vue'
import ClientesView from '@/features/clientes/ClientesView.vue'
import ConectividadView from '@/features/conectividad/ConectividadView.vue'
import ConfiguracionView from '@/features/configuracion/ConfiguracionView.vue'
import ControlesView from '@/features/controles/ControlesView.vue'
import EstadisticasView from '@/features/estadisticas/EstadisticasView.vue'
import InformesView from '@/features/informes/InformesView.vue'
import LiquidacionesColaboradorView from '@/features/liquidaciones-colaborador/LiquidacionesColaboradorView.vue'
import LiquidacionesCompaniaView from '@/features/liquidaciones-compania/LiquidacionesCompaniaView.vue'
import LogsView from '@/features/logs/LogsView.vue'
import PolizaDetailView from '@/features/polizas/PolizaDetailView.vue'
import PolizasColectivasView from '@/features/polizas-scopes/PolizasColectivasView.vue'
import PolizasFlotasView from '@/features/polizas-scopes/PolizasFlotasView.vue'
import PolizasView from '@/features/polizas/PolizasView.vue'
import PropuestasView from '@/features/propuestas/PropuestasView.vue'
import RecibosView from '@/features/recibos/RecibosView.vue'
import SiniestrosView from '@/features/siniestros/SiniestrosView.vue'
import SuplementosView from '@/features/suplementos/SuplementosView.vue'
import { hasAuthSession, validateAuthSession } from '@/features/auth/authSession'

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
    path: '/agenda',
    name: 'agenda',
    component: AgendaView,
    meta: { requiresAuth: true },
  },
  {
    path: '/clientes',
    name: 'clientes',
    component: ClientesView,
    meta: { requiresAuth: true },
  },
  {
    path: '/propuestas',
    name: 'propuestas',
    component: PropuestasView,
    meta: { requiresAuth: true },
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
    path: '/polizas/flotas',
    name: 'polizas-flotas',
    component: PolizasFlotasView,
    meta: { requiresAuth: true },
  },
  {
    path: '/polizas/colectivas',
    name: 'polizas-colectivas',
    component: PolizasColectivasView,
    meta: { requiresAuth: true },
  },
  {
    path: '/polizas/:id',
    name: 'poliza-detail',
    component: PolizaDetailView,
    meta: { requiresAuth: true },
  },
  {
    path: '/recibos',
    name: 'recibos',
    component: RecibosView,
    meta: { requiresAuth: true },
  },
  {
    path: '/suplementos',
    name: 'suplementos',
    component: SuplementosView,
    meta: { requiresAuth: true },
  },
  {
    path: '/siniestros',
    name: 'siniestros',
    component: SiniestrosView,
    meta: { requiresAuth: true },
  },
  {
    path: '/liq-cia',
    name: 'liq-cia',
    component: LiquidacionesCompaniaView,
    meta: { requiresAuth: true },
  },
  {
    path: '/liq-col',
    name: 'liq-col',
    component: LiquidacionesColaboradorView,
    meta: { requiresAuth: true },
  },
  {
    path: '/informes',
    name: 'informes',
    component: InformesView,
    meta: { requiresAuth: true },
  },
  {
    path: '/controles',
    name: 'controles',
    component: ControlesView,
    meta: { requiresAuth: true },
  },
  {
    path: '/estadisticas',
    name: 'estadisticas',
    component: EstadisticasView,
    meta: { requiresAuth: true },
  },
  {
    path: '/administracion',
    name: 'administracion',
    component: AdministracionView,
    meta: { requiresAuth: true },
  },
  {
    path: '/configuracion',
    name: 'configuracion',
    component: ConfiguracionView,
    meta: { requiresAuth: true },
  },
  {
    path: '/conectividad',
    name: 'conectividad',
    component: ConectividadView,
    meta: { requiresAuth: true },
  },
  {
    path: '/by-aunna',
    name: 'by-aunna',
    component: ByAunnaView,
    meta: { requiresAuth: true },
  },
  {
    path: '/logs',
    name: 'logs',
    component: LogsView,
    meta: { requiresAuth: true },
  },
]

export function registerAuthGuard(router: Router) {
  router.beforeEach(async (to) => {
    const requiresAuth = to.matched.some((route) => route.meta.requiresAuth)
    const hadSessionBeforeValidation = requiresAuth ? hasAuthSession() : false
    const authenticated = await validateAuthSession({
      requireBackendConfirmation: requiresAuth || to.name === 'login',
    })

    if (to.name === 'login' && authenticated) {
      const redirect = to.query.redirect
      const target = Array.isArray(redirect) ? redirect[0] : redirect
      return target?.startsWith('/') && !target.startsWith('//') ? target : { name: 'polizas' }
    }

    if (requiresAuth && !authenticated) {
      return {
        name: 'login',
        query: {
          redirect: to.fullPath,
          ...(hadSessionBeforeValidation ? { reason: 'session-check-failed' } : {}),
        },
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
