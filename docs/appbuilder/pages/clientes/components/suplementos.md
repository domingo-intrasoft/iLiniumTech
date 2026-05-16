# Componente Clientes: suplementos del cliente

Fecha: 2026-05-16

Estado: `pendiente tecnico`, bloqueado para implementacion hasta SDD.

## Rol del componente

Pestana o panel candidato para suplementos relacionados con polizas del cliente.

## Evidencia AppBuilder

Fuentes:

- `NombreTablasConst.ts`: `VW_CLIENTESSUPLEMENTOS = "vw_ClienteSuplementos"`
- `ModeloDbContext.cs`: mapeo de `VwClienteSuplemento` a `vw_ClienteSuplementos`

Campos detectados:

- `Poliza`
- `Polizaid`
- `Cliente`
- `Tipo`
- `Situacion`
- `Referencia`
- `ReferenciaCia`
- `FechaCreacion`
- `Aplicacion`
- `Declaraciones`
- `Recibos`
- `UltimaAnotacion`

## Evidencia ausente

- No consta si es pestana real de Clientes.
- No consta si declaraciones/recibos/anotaciones deben mostrarse o solo contar/resumir.
- No consta navegacion a modulo Suplementos.

## Propuesta Vue/API estatica

Vue:

- `ClienteSuplementosTab.vue`

API:

- `GET /api/clientes/{clienteId}/suplementos?page=&pageSize=&tipo=&situacion=&sort=`

Columnas candidatas:

- referencia;
- poliza;
- tipo;
- situacion;
- fecha de creacion;
- referencia compania si se aprueba.

Ocultar inicialmente:

- `Declaraciones`;
- `Recibos`;
- `UltimaAnotacion`.

## Permisos candidatos

- `clientes.suplementos.read`
- permiso futuro `suplementos.detail`.

## Riesgos

- Declaraciones y anotaciones pueden contener texto libre.
- Puede haber datos de cambios contractuales, recibos y decisiones sensibles.
- Relacion con Polizas debe respetar permisos de ambos modulos.

## Pruebas obligatorias

- no exponer texto libre por defecto;
- sort/filtros por whitelist;
- broker cruzado bloqueado;
- navegacion a poliza/suplemento solo con permiso.

## Bloqueo

Confirmar alcance de suplementos y minimizacion de texto libre.
