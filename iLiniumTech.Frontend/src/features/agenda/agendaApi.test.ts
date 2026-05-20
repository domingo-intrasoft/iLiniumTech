import { beforeEach, describe, expect, it, vi } from 'vitest'

const mocks = vi.hoisted(() => ({
  get: vi.fn(),
  post: vi.fn(),
  put: vi.fn(),
  delete: vi.fn(),
  assertRuntimeConfigReady: vi.fn(),
}))

vi.mock('@/services/apiClient', () => ({
  apiClient: {
    get: mocks.get,
    post: mocks.post,
    put: mocks.put,
    delete: mocks.delete,
  },
}))

vi.mock('@/services/runtimeConfig', () => ({
  assertRuntimeConfigReady: mocks.assertRuntimeConfigReady,
}))

import { createAgendaEvent, deleteAgendaEvent, searchAgenda, updateAgendaEvent } from './agendaApi'

describe('agenda API adapter', () => {
  beforeEach(() => {
    vi.unstubAllEnvs()
    mocks.get.mockReset()
    mocks.post.mockReset()
    mocks.put.mockReset()
    mocks.delete.mockReset()
    mocks.assertRuntimeConfigReady.mockReset()
  })

  it('uses the sanitized fixture only when backend mode is disabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    const result = await searchAgenda({ texto: '0002', page: 1, pageSize: 25 })

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.get).not.toHaveBeenCalled()
    expect(result.total).toBe(1)
    expect(result.items[0]?.referencia).toBe('AGE-2026-0002')
    expect(JSON.stringify(result)).not.toMatch(/[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}/i)
    expect(JSON.stringify(result)).not.toMatch(/[A-Z]{2}\d{2}[A-Z0-9]{11,30}/)
  })

  it('calls the Agenda search endpoint and maps minimized backend fields', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.get.mockResolvedValue({
      data: {
        items: [
          {
            id: '42',
            referencia: 'ILMVP-AGE-42',
            titulo: 'Evento backend minimizado',
            inicio: '2026-05-20T09:00:00',
            fin: '2026-05-20T09:30:00',
            estado: 'Programado',
            prioridad: 'Alta',
            origen: 'BBDD local',
            objetoRelacionadoTipo: 'Generico',
          },
        ],
        page: 2,
        pageSize: 10,
        total: 1,
      },
    })

    const result = await searchAgenda({
      texto: 'ILMVP',
      estado: 'Programado',
      prioridad: 'Alta',
      fechaDesde: '2026-05-01',
      page: 2,
      pageSize: 10,
      sort: 'inicio:desc',
    })

    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledOnce()
    expect(mocks.get).toHaveBeenCalledWith('/api/agenda', {
      params: {
        texto: 'ILMVP',
        estado: 'Programado',
        prioridad: 'Alta',
        fechaDesde: '2026-05-01',
        page: 2,
        pageSize: 10,
        sort: 'inicio:desc',
      },
    })
    expect(result.items[0]).toMatchObject({
      id: '42',
      referencia: 'ILMVP-AGE-42',
      asunto: 'Evento backend minimizado',
      estado: 'Programado',
      prioridad: 'Alta',
      fechaInicio: '2026-05-20',
      horaInicio: '09:00',
      fechaFin: '2026-05-20',
      horaFin: '09:30',
      objetoRelacionado: 'Generico',
      origen: 'BBDD local',
    })
  })

  it('blocks Agenda writes when backend mode is disabled', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'false')

    await expect(
      createAgendaEvent({
        referencia: 'ILMVP-AGE-LOCAL',
        titulo: 'Evento MVP',
        inicio: '2026-05-20T09:00:00',
      }),
    ).rejects.toThrow('Las escrituras de agenda requieren backend BBDD habilitado.')

    expect(mocks.assertRuntimeConfigReady).not.toHaveBeenCalled()
    expect(mocks.post).not.toHaveBeenCalled()
    expect(mocks.put).not.toHaveBeenCalled()
    expect(mocks.delete).not.toHaveBeenCalled()
  })

  it('calls explicit Agenda CRUD endpoints in backend mode without changing flags locally', async () => {
    vi.stubEnv('VITE_USE_BACKEND', 'true')
    mocks.post.mockResolvedValue({ data: { id: '123' } })
    mocks.put.mockResolvedValue({})
    mocks.delete.mockResolvedValue({})

    const createResult = await createAgendaEvent({
      referencia: 'ILMVP-AGE-123',
      titulo: 'Evento MVP',
      inicio: '2026-05-20T09:00:00',
      fin: '2026-05-20T09:30:00',
      prioridad: 'Media',
      objetoRelacionadoTipo: 'Generico',
    })
    await updateAgendaEvent('123', { titulo: 'Evento MVP actualizado', prioridad: 'Alta' })
    await deleteAgendaEvent('123')

    expect(createResult.id).toBe('123')
    expect(mocks.assertRuntimeConfigReady).toHaveBeenCalledTimes(3)
    expect(mocks.post).toHaveBeenCalledWith('/api/agenda', {
      referencia: 'ILMVP-AGE-123',
      titulo: 'Evento MVP',
      inicio: '2026-05-20T09:00:00',
      fin: '2026-05-20T09:30:00',
      prioridad: 'Media',
      objetoRelacionadoTipo: 'Generico',
    })
    expect(mocks.put).toHaveBeenCalledWith('/api/agenda/123', {
      titulo: 'Evento MVP actualizado',
      prioridad: 'Alta',
    })
    expect(mocks.delete).toHaveBeenCalledWith('/api/agenda/123')
  })
})
