# Componente Clientes: polizas del cliente

Fecha: 2026-05-16

Estado: `pendiente tecnico`, bloqueado para implementacion hasta SDD.

## Rol del componente

Pestana o panel candidato dentro de la ficha de cliente para listar polizas relacionadas.

## Evidencia AppBuilder

Fuentes:

- `NombreTablasConst.ts`: `VW_CLIENTEPOLIZAS = "vw_ClientePolizas"`
- `ModeloDbContext.cs`: mapeo de `VwClientePoliza` a `vw_ClientePolizas`

Campos detectados:

- `Id`
- `Poliza`
- `Ramo`
- `Subramo`
- `Situacion`
- `CiaDgs`
- `CiaRazonSocial`
- `FEfecto`
- `FEfectoPrimero`
- `FVencimiento`
- `FAnulacion`
- `Motivoanulacion`
- `ObservacionesAnulacion`
- `Riesgo`
- `Etiqueta`
- `Gestor`
- `Colaboradores`
- `CanalCobro`
- `IdRamo`, `IdSubRamo`, `IdSituacion`, `IdGestor`, `IdCanalCobro`
- `NumDocumento`
- `RazonSocial`

## Evidencia ausente

- No consta si `vw_ClientePolizas` es una pestana real de `Clientes` o una vista auxiliar.
- No consta filtro por cliente exacto ni nombre del campo de relacion en la vista.
- No consta columnas visibles ni orden por defecto.

## Propuesta Vue/API estatica

Vue:

- `ClientePolizasTab.vue`

API:

- `GET /api/clientes/{clienteId}/polizas?page=&pageSize=&sort=`

Integracion con modulo Polizas:

- reutilizar tipos/conceptos solo si no rompe el boundary de Clientes;
- al pulsar una poliza, navegar a `/polizas/{id}` si existe permiso `polizas.detail`;
- conservar vuelta a `/clientes/{clienteId}`.

## Permisos candidatos

- `clientes.polizas.read`
- `polizas.detail` para navegar al detalle de poliza.

## Riesgos PII

- `NumDocumento` y `RazonSocial` no deben exponerse completos en esta pestana si ya se esta dentro del cliente.
- `Riesgo` puede contener matricula, direccion, bien asegurado u otra informacion sensible.
- `ObservacionesAnulacion` puede contener texto libre.

## Pruebas obligatorias

- broker cruzado no revela polizas;
- no se proyecta `NumDocumento` completo;
- sort por whitelist;
- paginacion;
- estados loading/empty/error;
- navegacion a detalle solo con permiso.

## Bloqueo

Confirmar con producto si esta pestana entra en el primer corte de Clientes o si se delega al modulo Polizas.
