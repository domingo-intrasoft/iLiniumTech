import { apiClient } from '@/services/apiClient'

import { polizasFixture, polizasMetadata } from './polizasFixture'
import type { PagedResult, PolizaListItem, PolizasComponentMetadata, PolizaDetail } from './polizasTypes'

export interface PolizasSearchParams {
  numero?: string
  cliente?: string
  estado?: string
  compania?: string
  ramo?: string
  documento?: string
  fechaEfectoDesde?: string
  fechaEfectoHasta?: string
  page?: number
  pageSize?: number
  sort?: string
}

export async function getPolizasMetadata(): Promise<PolizasComponentMetadata> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PolizasComponentMetadata>('/api/polizas/metadata')
    return response.data
  }

  return polizasMetadata
}

export async function searchPolizas(
  params: PolizasSearchParams,
): Promise<PagedResult<PolizaListItem>> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PagedResult<PolizaListItem>>('/api/polizas', { params })
    return response.data
  }

  const numero = params.numero?.toLowerCase() ?? ''
  const cliente = params.cliente?.toLowerCase() ?? ''
  const estado = params.estado?.toLowerCase() ?? ''
  const items = polizasFixture.items.filter((item) => {
    const matchesNumero = !numero || item.numero.toLowerCase().includes(numero)
    const matchesCliente = !cliente || item.clienteNombre.toLowerCase().includes(cliente)
    const matchesEstado = !estado || item.estado.toLowerCase().includes(estado)
    return matchesNumero && matchesCliente && matchesEstado
  })

  return {
    ...polizasFixture,
    total: items.length,
    items,
  }
}

export async function getPolizaById(id: string): Promise<PolizaDetail> {
  if (import.meta.env.VITE_USE_BACKEND === 'true') {
    const response = await apiClient.get<PolizaDetail>(`/api/polizas/${id}`)
    return response.data
  }

  // Fallback to mock detailed item
  return {
    id: id,
    numero: '4105925343142',
    aplicacion: 'Autos',
    estado: 'Vigor',
    ramo: 'Autos',
    compania: 'Reale Vida',
    cliente: {
      id: 'CLI-588788',
      nombre: 'Hernandez Gila, Lara',
      documento: '52017408X',
    },
    producto: {
      nombre: 'Reale Seguros Generales, S.A. - Vida Tem',
      modalidad: 'Individual',
    },
    vigencia: {
      fechaInicio: '2026-01-01',
      fechaVencimiento: '2027-01-01',
      renovacion: 'Anual',
    },
    financiero: {
      primaAnual: 609.61,
      moneda: 'EUR',
    },
    riesgos: [
      { id: 'R-001', descripcion: 'Vehiculo Automovil ', tipoRiesgo: 'Autos Cat.1', fechaAlta: '2026-01-01', fechaBaja: undefined }
    ],
    recibos: [
      {
        id: 'REC-251436851',
        numero: '251436851',
        estado: 'Cobrado',
        estadoCia: 'Liquidado',
        estadoColab: 'Liquidado',
        tipo: 'Producción',
        gestor: 'Compañía',
        primaTotal: 609.61,
        fechaEfecto: '2026-01-01',
        fechaVencimiento: '2027-01-01'
      }
    ]
  }
}
