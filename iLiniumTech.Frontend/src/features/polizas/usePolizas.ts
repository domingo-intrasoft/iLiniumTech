import { onMounted, reactive, ref } from 'vue'

import { getPolizasCatalogs, searchPolizas } from './polizasApi'
import { polizasCatalogsFixture } from './polizasFixture'
import type { PolizaListItem, PolizasCatalogs, PolizasQueryFilters } from './polizasTypes'

export function usePolizas() {
  const catalogs = ref<PolizasCatalogs>(polizasCatalogsFixture)
  const items = ref<PolizaListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filters = reactive<PolizasQueryFilters>({
    numero: '',
    cliente: '',
    estado: '',
    compania: '',
    ramo: '',
  })

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchPolizas({
        numero: filters.numero || undefined,
        cliente: filters.cliente || undefined,
        estado: filters.estado || undefined,
        compania: filters.compania || undefined,
        ramo: filters.ramo || undefined,
        page: 1,
        pageSize: 25,
        sort: 'fechaEfecto:desc',
      })
      items.value = result.items
      total.value = result.total
    } catch {
      error.value = 'No se pudieron cargar las polizas.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function loadCatalogs() {
    catalogs.value = await getPolizasCatalogs()
  }

  onMounted(() => {
    void loadCatalogs()
    void refresh()
  })

  return {
    catalogs,
    items,
    total,
    loading,
    error,
    filters,
    refresh,
  }
}
