import { computed, onMounted, reactive, ref } from 'vue'

import {
  createAgendaEvent,
  deleteAgendaEvent,
  searchAgenda as searchAgendaApi,
  updateAgendaEvent,
} from './agendaApi'
import type { AgendaFilters, AgendaListItem } from './types'

function createEmptyFilters(): AgendaFilters {
  return {
    texto: '',
    estado: '',
    prioridad: '',
    fechaDesde: '',
  }
}

function copyFilters(target: AgendaFilters, source: AgendaFilters) {
  target.texto = source.texto
  target.estado = source.estado
  target.prioridad = source.prioridad
  target.fechaDesde = source.fechaDesde
}

export function formatAgendaDate(value: string) {
  if (!value) {
    return ''
  }

  return new Intl.DateTimeFormat('es-ES').format(new Date(`${value}T00:00:00`))
}

function toIsoWithoutMilliseconds(value: Date) {
  return value.toISOString().replace(/\.\d{3}Z$/, '')
}

export function useAgenda() {
  const filters = reactive<AgendaFilters>(createEmptyFilters())
  const draftFilters = reactive<AgendaFilters>(createEmptyFilters())
  const items = ref<AgendaListItem[]>([])
  const total = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const isBackendMode = import.meta.env.VITE_USE_BACKEND === 'true'

  const pagination = reactive({
    page: 1,
    pageSize: 2,
  })

  const totalPages = computed(() => Math.max(1, Math.ceil(total.value / pagination.pageSize)))
  const pagedItems = computed(() => items.value)
  const firstVisible = computed(() =>
    total.value === 0 ? 0 : (pagination.page - 1) * pagination.pageSize + 1,
  )
  const lastVisible = computed(() => Math.min(pagination.page * pagination.pageSize, total.value))
  const resultLabel = computed(() => (total.value === 1 ? 'evento' : 'eventos'))
  const tableCaption = computed(() => {
    const mode = isBackendMode ? 'BBDD local/API' : 'fixture read-only'
    return `Agenda ${mode}: ${firstVisible.value}-${lastVisible.value} de ${total.value} ${resultLabel.value}.`
  })
  const canGoPrevious = computed(() => pagination.page > 1)
  const canGoNext = computed(() => pagination.page < totalPages.value)

  async function refresh() {
    loading.value = true
    error.value = null

    try {
      const result = await searchAgendaApi({
        texto: filters.texto || undefined,
        estado: filters.estado || undefined,
        prioridad: filters.prioridad || undefined,
        fechaDesde: filters.fechaDesde || undefined,
        page: pagination.page,
        pageSize: pagination.pageSize,
        sort: 'inicio:desc',
      })
      items.value = result.items
      total.value = result.total
      pagination.page = result.page
      pagination.pageSize = result.pageSize
    } catch {
      error.value = 'No se pudo cargar Agenda.'
      items.value = []
      total.value = 0
    } finally {
      loading.value = false
    }
  }

  async function searchAgenda() {
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function clearFilters() {
    copyFilters(draftFilters, createEmptyFilters())
    copyFilters(filters, draftFilters)
    pagination.page = 1
    await refresh()
  }

  async function changePage(page: number) {
    if (page >= 1 && page <= totalPages.value) {
      pagination.page = page
      await refresh()
    }
  }

  async function changePageSize(event: Event) {
    pagination.pageSize = Number((event.target as HTMLSelectElement).value)
    pagination.page = 1
    await refresh()
  }

  async function createMvpEvent() {
    try {
      const start = new Date()
      start.setDate(start.getDate() + 1)
      start.setHours(9, 0, 0, 0)
      const end = new Date(start)
      end.setMinutes(end.getMinutes() + 30)
      await createAgendaEvent({
        referencia: `ILMVP-AGE-${Date.now()}`,
        titulo: 'Evento creado desde iLiniumTech',
        inicio: toIsoWithoutMilliseconds(start),
        fin: toIsoWithoutMilliseconds(end),
        prioridad: 'Media',
        objetoRelacionadoTipo: 'Generico',
      })
      await refresh()
    } catch {
      error.value = 'No se pudo crear el evento de Agenda.'
    }
  }

  async function updateMvpEvent(item: AgendaListItem) {
    try {
      await updateAgendaEvent(item.id, {
        titulo: 'Evento actualizado desde iLiniumTech',
        prioridad: item.prioridad === 'Alta' ? 'Media' : 'Alta',
        objetoRelacionadoTipo: item.objetoRelacionado || 'Generico',
      })
      await refresh()
    } catch {
      error.value = 'No se pudo actualizar el evento de Agenda.'
    }
  }

  async function deleteMvpEvent(item: AgendaListItem) {
    try {
      await deleteAgendaEvent(item.id)
      await refresh()
    } catch {
      error.value = 'No se pudo eliminar el evento de Agenda.'
    }
  }

  onMounted(() => {
    void refresh()
  })

  return {
    canGoNext,
    canGoPrevious,
    changePage,
    changePageSize,
    clearFilters,
    createMvpEvent,
    deleteMvpEvent,
    draftFilters,
    error,
    firstVisible,
    formatDate: formatAgendaDate,
    isBackendMode,
    lastVisible,
    loading,
    pagedItems,
    pagination,
    resultLabel,
    searchAgenda,
    tableCaption,
    total,
    totalPages,
    updateMvpEvent,
  }
}
