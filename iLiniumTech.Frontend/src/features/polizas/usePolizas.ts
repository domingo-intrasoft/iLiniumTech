import { computed, onMounted, reactive, ref } from 'vue'

import { getPolizasMetadata, searchPolizas } from './polizasApi'
import type { PolizaListItem, PolizasComponentMetadata } from './polizasTypes'

export function usePolizas() {
  const metadata = ref<PolizasComponentMetadata | null>(null)
  const items = ref<PolizaListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filters = reactive({
    numero: '',
    cliente: '',
    estado: '',
  })

  const visibleFields = computed(() =>
    (metadata.value?.fields ?? [])
      .filter((field) => field.visible)
      .sort((a, b) => a.order - b.order),
  )

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      metadata.value = await getPolizasMetadata()
      const result = await searchPolizas({
        numero: filters.numero || undefined,
        cliente: filters.cliente || undefined,
        estado: filters.estado || undefined,
        page: 1,
        pageSize: 25,
        sort: 'fechaEfecto:desc',
      })
      items.value = result.items
      total.value = result.total
    } catch {
      error.value = 'No se pudo cargar el componente de pólizas.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  onMounted(refresh)

  return {
    metadata,
    visibleFields,
    items,
    total,
    loading,
    error,
    filters,
    refresh,
  }
}
