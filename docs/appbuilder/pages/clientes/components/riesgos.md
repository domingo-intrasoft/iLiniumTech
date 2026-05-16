# Componente Clientes: riesgos del cliente

Fecha: 2026-05-16

Estado: `pendiente tecnico`, bloqueado para implementacion hasta SDD.

## Rol del componente

Pestana o panel candidato para riesgos asegurados asociados al cliente.

## Evidencia AppBuilder

Fuentes:

- `NombreTablasConst.ts`: `VW_CLIENTERIESGOS = "vw_ClienteRiesgos"`
- `ModeloDbContext.cs`: mapeo de `VwClienteRiesgo` a `vw_ClienteRiesgos`

Campos detectados:

- `Cliente`
- `Poliza`
- `Riesgo`
- `Situacion`
- `FechaAlta`
- `FechaBaja`
- `Aplicacion`

## Evidencia ausente

- No consta si esta vista se usa en Clientes como pestana visible.
- No consta tipo de riesgo ni mascara necesaria por ramo.
- No consta si `Riesgo` contiene texto estructurado o libre.

## Propuesta Vue/API estatica

Vue:

- `ClienteRiesgosTab.vue`

API:

- `GET /api/clientes/{clienteId}/riesgos?page=&pageSize=&situacion=&sort=`

Columnas candidatas:

- poliza;
- descripcion de riesgo minimizada;
- situacion;
- fecha alta;
- fecha baja.

## Permisos candidatos

- `clientes.riesgos.read`

Permisos futuros por tipo de riesgo si se exponen vehiculos, inmuebles, salud u otros bienes sensibles.

## Riesgos

- `Riesgo` puede incluir matricula, bastidor, direccion de inmueble, persona asegurada o texto libre.
- La sensibilidad varia por ramo; no debe tratarse como campo inocuo.

## Pruebas obligatorias

- mascara por tipo de riesgo si se identifica vehiculo/inmueble;
- no exponer riesgo completo sin decision PII;
- filtros y sort por whitelist;
- broker cruzado bloqueado.

## Bloqueo

Clasificar contenido de `Riesgo` con producto/DBA antes de mostrarlo.
