<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRoute } from 'vue-router'

import { appNavigation, isNavigationItemActive, type AppNavigationItem } from './appNavigation'

const route = useRoute()

const activePath = computed(() => route.path)

function itemTitle(item: AppNavigationItem) {
  return item.disabled ? `${item.label} no disponible en el MVP` : item.label
}
</script>

<template>
  <aside class="il-sidebar" aria-label="Menu principal">
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
          <RouterLink
            v-if="item.to"
            :to="item.to"
            :title="itemTitle(item)"
            :aria-current="item.to === activePath ? 'page' : undefined"
          >
            <i :class="item.icon" aria-hidden="true"></i>
            <span>{{ item.label }}</span>
          </RouterLink>
          <span v-else class="side-nav-disabled" :title="itemTitle(item)" aria-disabled="true">
            <i :class="item.icon" aria-hidden="true"></i>
            <span>{{ item.label }}</span>
          </span>

          <ul v-if="item.children?.length" class="side-subnav">
            <li v-for="child in item.children" :key="child.label">
              <RouterLink
                v-if="child.to"
                :to="child.to"
                :title="itemTitle(child)"
                :aria-current="child.to === activePath ? 'page' : undefined"
              >
                <i :class="child.icon" aria-hidden="true"></i>
                <span>{{ child.label }}</span>
              </RouterLink>
              <span v-else class="side-nav-disabled" :title="itemTitle(child)" aria-disabled="true">
                <i :class="child.icon" aria-hidden="true"></i>
                <span>{{ child.label }}</span>
              </span>
            </li>
          </ul>
        </li>
      </ul>
    </nav>
  </aside>
</template>
