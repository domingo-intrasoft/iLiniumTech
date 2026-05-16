<script setup lang="ts">
import { computed } from 'vue'

import AppShell from '@/layout/AppShell.vue'

import type { MvpPageDefinition, MvpPageMetric } from './mvpPageTypes'

const props = defineProps<{
  page: MvpPageDefinition
}>()

const contentId = computed(
  () => `${props.page.sectionTitle.toLowerCase().replace(/\s+/g, '-')}-content`,
)

function metricToneClass(metric: MvpPageMetric) {
  return metric.tone ? `tone-${metric.tone}` : 'tone-ready'
}
</script>

<template>
  <AppShell
    :content-id="contentId"
    :section-title="page.sectionTitle"
    session-label="MVP estatico"
    session-needs-attention
  >
    <main :id="contentId" class="mvp-module-page" :aria-labelledby="`${page.sectionTitle}-title`">
      <header class="mvp-module-header">
        <div>
          <p class="section-kicker">{{ page.status }}</p>
          <h1 :id="`${page.sectionTitle}-title`">{{ page.title }}</h1>
          <p>{{ page.subtitle }}</p>
        </div>

        <div class="mvp-module-actions" aria-label="Acciones del modulo">
          <button v-for="action in page.actions" :key="action.label" type="button" disabled>
            <i :class="action.icon" aria-hidden="true"></i>
            <span>{{ action.label }}</span>
          </button>
        </div>
      </header>

      <section class="mvp-module-band" aria-label="Estado del modulo">
        <article
          v-for="metric in page.metrics"
          :key="metric.label"
          class="mvp-module-metric"
          :class="metricToneClass(metric)"
        >
          <span>{{ metric.label }}</span>
          <strong>{{ metric.value }}</strong>
        </article>
      </section>

      <section class="mvp-module-grid">
        <article>
          <h2>{{ page.scope.title }}</h2>
          <ul>
            <li v-for="item in page.scope.items" :key="item">{{ item }}</li>
          </ul>
        </article>

        <article>
          <h2>{{ page.nextSteps.title }}</h2>
          <ul>
            <li v-for="item in page.nextSteps.items" :key="item">{{ item }}</li>
          </ul>
        </article>

        <article>
          <h2>{{ page.risks.title }}</h2>
          <ul>
            <li v-for="item in page.risks.items" :key="item">{{ item }}</li>
          </ul>
        </article>
      </section>

      <p class="mvp-module-source">{{ page.source }}</p>
    </main>
  </AppShell>
</template>
