# Componente Clientes: recibos del cliente

Fecha: 2026-05-16

Estado: `pendiente tecnico`, bloqueado para implementacion hasta SDD.

## Rol del componente

Pestana o panel candidato de recibos asociados a un cliente.

## Evidencia AppBuilder

Fuentes:

- `NombreTablasConst.ts`: `VW_CLIENTERECIBOS = "vw_ClienteRecibos"`
- `ModeloDbContext.cs`: mapeo de `VwClienteRecibo` a `vw_ClienteRecibos`

Campos detectados:

- `ReciboCia`
- `Poliza`
- `Cliente`
- `CiaDgs`
- `CiaRazonSocial`
- `Ramo`
- `Subramo`
- `SituacionPoliza`
- `SituacionRecibo`
- `TipoReciboCia`
- `TipoCanalCobro`
- `FEfecto`
- `FVencimiento`
- `FCobro`
- `PrimaNeta`
- `Primatotal`
- `Gestor`
- `Tecnico`
- `Colaboradores`
- `Aplicacion`
- `UltimaAnotacion`
- `AnotacionDevolucion`

## Evidencia ausente

- No consta si es pestana real en pantalla Clientes.
- No consta reglas de importe, recibo pendiente/cobrado/devuelto ni filtros finales.
- No consta si se permite navegar a un futuro modulo Recibos.

## Propuesta Vue/API estatica

Vue:

- `ClienteRecibosTab.vue`

API:

- `GET /api/clientes/{clienteId}/recibos?page=&pageSize=&situacion=&fechaDesde=&fechaHasta=&sort=`

Columnas candidatas:

- recibo;
- poliza;
- compania;
- situacion;
- vencimiento/cobro;
- prima total minimizada si permiso;
- canal de cobro.

## Permisos candidatos

- `clientes.recibos.read`
- permiso futuro `recibos.detail` si se implementa navegacion.
- permiso especifico para importes si producto lo exige.

## Riesgos

- Importes y situacion de cobro pueden ser informacion sensible.
- `UltimaAnotacion` puede contener texto libre o datos personales.
- Mostrar `Cliente` dentro de ficha puede duplicar PII.

## Pruebas obligatorias

- no mostrar anotaciones libres por defecto;
- importes ocultos si no hay permiso;
- filtros por fecha parametrizados;
- sort por whitelist;
- empty state para cliente sin recibos;
- broker cruzado bloqueado.

## Bloqueo

Confirmar alcance de recibos y tratamiento de importes/anotaciones.
