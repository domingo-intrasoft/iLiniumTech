import type {
  AutoParticularPolizaListItem,
  AutosParticularesCatalogs,
  AutosParticularesScope,
  PagedResult,
} from './autosParticularesTypes'

export const autosParticularesScopeFixture: AutosParticularesScope = {
  ramo: 'Autos',
  divisionObjetivo: 'Particulares',
  divisionFiltroAplicado: false,
  divisionPendienteUat: true,
}

export const autosParticularesCatalogsFixture: AutosParticularesCatalogs = {
  estado: [
    { value: 'Vigor', label: 'En vigor' },
    { value: 'Pendiente', label: 'Pendiente' },
    { value: 'Anulada', label: 'Anulada' },
  ],
  compania: [
    { value: 'Compania demo', label: 'Compania demo' },
    { value: 'Aseguradora ejemplo', label: 'Aseguradora ejemplo' },
  ],
  scope: autosParticularesScopeFixture,
}

export const autosParticularesFixture: PagedResult<AutoParticularPolizaListItem> = {
  page: 1,
  pageSize: 25,
  total: 4,
  scope: autosParticularesScopeFixture,
  items: [
    {
      id: 'AUTO-1001',
      numero: 'AUTO-2026-0001',
      aplicacion: 'Auto',
      estado: 'Vigor',
      ramo: 'Autos',
      compania: 'Compania demo',
      clienteNombre: 'Cliente auto anonimo 1',
      vehiculoResumen: 'Turismo compacto',
      fechaEfecto: '2026-01-10',
      fechaVencimiento: '2027-01-09',
      primaAnual: 386.25,
      moneda: 'EUR',
    },
    {
      id: 'AUTO-1002',
      numero: 'AUTO-2026-0002',
      aplicacion: 'Auto',
      estado: 'Pendiente',
      ramo: 'Autos',
      compania: 'Compania demo',
      clienteNombre: 'Cliente auto anonimo 2',
      vehiculoResumen: 'SUV familiar',
      fechaEfecto: '2026-02-01',
      fechaVencimiento: '2027-01-31',
      primaAnual: 612.4,
      moneda: 'EUR',
    },
    {
      id: 'AUTO-1003',
      numero: 'AUTO-2026-0003',
      aplicacion: 'Auto',
      estado: 'Vigor',
      ramo: 'Autos',
      compania: 'Aseguradora ejemplo',
      clienteNombre: 'Cliente auto anonimo 3',
      vehiculoResumen: 'Utilitario electrico',
      fechaEfecto: '2026-03-18',
      fechaVencimiento: '2027-03-17',
      primaAnual: 540.0,
      moneda: 'EUR',
    },
    {
      id: 'AUTO-1004',
      numero: 'AUTO-2026-0004',
      aplicacion: 'Auto',
      estado: 'Anulada',
      ramo: 'Autos',
      compania: 'Aseguradora ejemplo',
      clienteNombre: 'Cliente auto anonimo 4',
      vehiculoResumen: 'Moto particular',
      fechaEfecto: '2025-11-20',
      fechaVencimiento: '2026-11-19',
      primaAnual: 198.9,
      moneda: 'EUR',
    },
  ],
}
