# Componente Clientes: siniestros del cliente

Fecha: 2026-05-16

Estado: `pendiente tecnico`, bloqueado para implementacion hasta SDD.

## Rol del componente

Pestana o panel candidato para siniestros relacionados con el cliente.

## Evidencia AppBuilder

Fuentes:

- `NombreTablasConst.ts`: `VW_CLIENTESINIESTROS = "vw_ClienteSiniestros"`
- `ModeloDbContext.cs`: mapeo de `VwClienteSiniestro` a `vw_ClienteSiniestros`

Campos detectados:

- `Id`
- `Poliza`
- `Cliente`
- `CiaDgs`
- `CiaRazonSocial`
- `ReferenciaCia`
- `ReferenciaMediador`
- `Ramo`
- `Situacion`
- `Estado`
- `Prioridad`
- `Tecnico`
- `Riesgo`
- `Danos`
- `Descripcion`
- `Etiquetas`
- `Reserva`
- `FParte`
- `FSiniestro`
- `FCierre`
- `FRevision`

## Evidencia ausente

- No consta si es pestana real de Clientes.
- No consta permisos de siniestros ni si reserva/danos/descripcion deben mostrarse.
- No consta navegacion al modulo Siniestros.

## Propuesta Vue/API estatica

Vue:

- `ClienteSiniestrosTab.vue`

API:

- `GET /api/clientes/{clienteId}/siniestros?page=&pageSize=&estado=&fechaDesde=&fechaHasta=&sort=`

Columnas candidatas para primer corte:

- referencia interna;
- poliza;
- compania;
- situacion/estado;
- fecha de siniestro;
- fecha de cierre;
- tecnico.

Ocultar inicialmente:

- `Danos`;
- `Descripcion`;
- `Riesgo`;
- `Reserva`;
- `Etiquetas`;
- referencias externas si producto las clasifica como sensibles.

## Permisos candidatos

- `clientes.siniestros.read`
- permiso futuro `siniestros.detail`
- permiso especifico para reserva/importes.

## Riesgos

- Siniestros puede contener informacion especialmente sensible.
- `Descripcion` y `Danos` son texto libre con alto riesgo PII.
- `Reserva` es informacion economica sensible.

## Pruebas obligatorias

- campos sensibles no aparecen por defecto;
- permisos especificos para importes/detalle;
- broker cruzado bloqueado;
- 404/403 no revela existencia de siniestro;
- logs sin descripcion, danos ni referencias completas.

## Bloqueo

Necesaria decision de producto/seguridad antes de incluir siniestros en ficha de cliente.
