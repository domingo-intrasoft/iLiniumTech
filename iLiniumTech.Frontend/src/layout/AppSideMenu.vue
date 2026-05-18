<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'

import { useSession } from '@/services/session'

import {
  appNavigation,
  appNavigationStatusLegend,
  getNavigationStatusDescription,
  getNavigationStatusLabel,
  getNavigationStatusShortLabel,
  getNavigationUnavailableReason,
  isNavigationItemActive,
  type AppNavigationItem,
} from './appNavigation'

const route = useRoute()
const { session } = useSession()

const props = withDefaults(
  defineProps<{
    appbuilderChrome?: boolean
  }>(),
  {
    appbuilderChrome: false,
  },
)

const activePath = computed(() => route.path)

function itemUnavailable(item: AppNavigationItem) {
  return getNavigationUnavailableReason(item, session.value) !== null
}

function childUnavailable(parent: AppNavigationItem, child: AppNavigationItem) {
  return itemUnavailable(parent) || itemUnavailable(child)
}

function itemTitle(item: AppNavigationItem) {
  const unavailableReason = getNavigationUnavailableReason(item, session.value)
  const statusDescription = getNavigationStatusDescription(item)

  if (unavailableReason === 'disabled') {
    return `${item.label} no disponible en el MVP. ${statusDescription}`
  }

  if (unavailableReason === 'permission') {
    return `${item.label} no disponible para la sesion actual. ${statusDescription}`
  }

  return `${item.label}. ${statusDescription}`
}

function childTitle(parent: AppNavigationItem, child: AppNavigationItem) {
  const parentReason = getNavigationUnavailableReason(parent, session.value)

  if (parentReason === 'disabled') {
    return `${child.label} no disponible en el MVP`
  }

  if (parentReason === 'permission') {
    return `${child.label} no disponible para la sesion actual`
  }

  return itemTitle(child)
}

function accessibleStatusDescription(item: AppNavigationItem) {
  return getNavigationStatusDescription(item).replace(
    'fixture local',
    'datos de demostracion local',
  )
}

function itemAccessibleDescription(item: AppNavigationItem) {
  const unavailableReason = getNavigationUnavailableReason(item, session.value)

  if (unavailableReason === 'disabled') {
    return `${item.label} no disponible en el MVP. ${accessibleStatusDescription(item)}`
  }

  if (unavailableReason === 'permission') {
    return `${item.label} no disponible para la sesion actual. ${accessibleStatusDescription(item)}`
  }

  return `${item.label}. ${accessibleStatusDescription(item)}`
}

function childAccessibleDescription(parent: AppNavigationItem, child: AppNavigationItem) {
  const parentReason = getNavigationUnavailableReason(parent, session.value)

  if (parentReason === 'disabled') {
    return `${child.label} no disponible en el MVP`
  }

  if (parentReason === 'permission') {
    return `${child.label} no disponible para la sesion actual`
  }

  return itemAccessibleDescription(child)
}

function navigationDescriptionId(item: AppNavigationItem, parent?: AppNavigationItem) {
  const key = [parent?.label, item.label]
    .filter(Boolean)
    .join('-')
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/(^-|-$)/g, '')

  return `nav-description-${key}`
}

function statusLegendItem(status: (typeof appNavigationStatusLegend)[number]): AppNavigationItem {
  return { label: status, icon: '', status }
}

function statusLegendTitle(status: (typeof appNavigationStatusLegend)[number]) {
  const item = statusLegendItem(status)
  return `${getNavigationStatusLabel(item)}: ${getNavigationStatusDescription(item)}`
}
</script>

<template>
  <aside
    class="il-sidebar"
    :class="{ 'appbuilder-side-menu': props.appbuilderChrome }"
    aria-label="Menu principal"
  >
    <RouterLink class="broker-logo" to="/polizas" aria-label="Ir a Polizas">
      <span aria-hidden="true"></span>
      AUXFISE
    </RouterLink>

    <nav class="side-nav" aria-label="Navegacion principal">
      <ul>
        <li
          v-for="item in appNavigation"
          :key="item.label"
          :class="{
            'has-children': item.children?.length,
            active: isNavigationItemActive(item, activePath),
          }"
        >
          <span :id="navigationDescriptionId(item)" hidden>{{
            itemAccessibleDescription(item)
          }}</span>
          <RouterLink
            v-if="item.to && !itemUnavailable(item)"
            :to="item.to"
            :title="itemTitle(item)"
            :aria-current="item.to === activePath ? 'page' : undefined"
            :aria-describedby="navigationDescriptionId(item)"
          >
            <i :class="item.icon" aria-hidden="true"></i>
            <span class="side-nav-label">{{ item.label }}</span>
            <span
              class="nav-status-dot"
              :class="`status-${item.status}`"
              :title="getNavigationStatusLabel(item)"
              aria-hidden="true"
            ></span>
          </RouterLink>
          <span
            v-else
            class="side-nav-disabled"
            :title="itemTitle(item)"
            :aria-disabled="itemUnavailable(item) ? 'true' : undefined"
            :aria-describedby="navigationDescriptionId(item)"
          >
            <i :class="item.icon" aria-hidden="true"></i>
            <span class="side-nav-label">{{ item.label }}</span>
            <span
              class="nav-status-dot"
              :class="`status-${item.status}`"
              :title="getNavigationStatusLabel(item)"
              aria-hidden="true"
            ></span>
          </span>

          <ul v-if="item.children?.length && !props.appbuilderChrome" class="side-subnav">
            <li v-for="child in item.children" :key="child.label">
              <span :id="navigationDescriptionId(child, item)" hidden>{{
                childAccessibleDescription(item, child)
              }}</span>
              <RouterLink
                v-if="child.to && !childUnavailable(item, child)"
                :to="child.to"
                :title="childTitle(item, child)"
                :aria-current="child.to === activePath ? 'page' : undefined"
                :aria-describedby="navigationDescriptionId(child, item)"
              >
                <i :class="child.icon" aria-hidden="true"></i>
                <span class="side-nav-label">{{ child.label }}</span>
                <span
                  class="nav-status-dot"
                  :class="`status-${child.status}`"
                  :title="getNavigationStatusLabel(child)"
                  aria-hidden="true"
                ></span>
              </RouterLink>
              <span
                v-else
                class="side-nav-disabled"
                :title="childTitle(item, child)"
                :aria-disabled="childUnavailable(item, child) ? 'true' : undefined"
                :aria-describedby="navigationDescriptionId(child, item)"
              >
                <i :class="child.icon" aria-hidden="true"></i>
                <span class="side-nav-label">{{ child.label }}</span>
                <span
                  class="nav-status-dot"
                  :class="`status-${child.status}`"
                  :title="getNavigationStatusLabel(child)"
                  aria-hidden="true"
                ></span>
              </span>
            </li>
          </ul>
        </li>
      </ul>
    </nav>

    <div
      v-if="!props.appbuilderChrome"
      class="side-status-legend"
      aria-label="Leyenda de estados del menu"
    >
      <span class="side-status-legend-title">Estado</span>
      <span
        v-for="status in appNavigationStatusLegend"
        :key="status"
        class="nav-status-dot"
        :class="`status-${status}`"
        :title="statusLegendTitle(status)"
        :aria-label="statusLegendTitle(status)"
        role="img"
      >
        <span aria-hidden="true">{{
          getNavigationStatusShortLabel(statusLegendItem(status))
        }}</span>
      </span>
    </div>
  </aside>
</template>
