# Evidencia Polizas CRUD BBDD MVP

Fecha: 2026-05-18
Rama: `codex/polizas-crud-bbdd`
SDD: `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`

## Objetivo vigente

El objetivo MVP activo queda centrado en evolucionar la pantalla `Polizas` desde read-only hacia CRUD contra BBDD local autorizada.

Reglas de alcance:

- La UI sigue siendo Vue/TypeScript estatico, no metadata AppBuilder runtime.
- El backend debe exponer API explicita y permisos propios.
- Las escrituras deben estar desactivadas por defecto y solo habilitarse en entorno local/demo con configuracion explicita.
- No se deben versionar connection strings, credenciales, nombres de servidores, dumps ni datos personales reales.
- El resto de paginas del menu se abordara despues de cerrar Polizas CRUD, con agentes por pagina, SDD propia y evidencia independiente.

## Hallazgo tecnico que desbloquea CRUD

Analisis local de solo lectura sobre BBDD de pruebas:

- `dbo.Pantalla_Polizas` es una vista.
- El campo visible `Poliza` no es unico y puede repetirse.
- `dbo.Poliza.Id` es el identificador estable que debe usarse para detalle y CRUD.
- `Poliza.Poliza` debe seguir mostrandose como numero visible, pero no debe usarse como recurso de escritura.

Decision aplicada en backend:

- El listado SQL proyecta `CAST([Id] AS nvarchar(100)) AS [Id]`.
- El detalle SQL proyecta `CAST([Id] AS nvarchar(100)) AS [Id]`.
- El detalle filtra por `[Id] = TRY_CONVERT(int, @id)`.
- El numero visible sigue proyectandose como `Numero`.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/Sql/PolizasSqlQueryBuilder.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasSqlQueryBuilderTests.cs`

## Preparacion de permisos y gate de escritura

Incremento aplicado despues del id estable:

- Se registran permisos activos `polizas.create`, `polizas.update` y `polizas.delete`.
- Los permisos de escritura no se conceden por defecto a `DemoSession`.
- La compatibilidad MVP de API key en `Development` queda limitada a lectura: `catalogs`, `read` y `detail`.
- Se exponen endpoints placeholder `POST /api/polizas`, `PUT /api/polizas/{id}` y `DELETE /api/polizas/{id}`.
- Los endpoints de escritura exigen permiso especifico y `Polizas:WritesEnabled=true`.
- Si el gate no esta activo, devuelven `POLIZAS_WRITES_DISABLED`.
- Si el gate esta activo pero aun no existe implementacion CRUD real, devuelven `POLIZAS_CRUD_NOT_IMPLEMENTED` con HTTP `501`.
- Fuera de `Development`, `Polizas:WritesEnabled=true` requiere `Polizas:WritesEnabledDemoOptIn` con el valor demo opt-in ya usado por el proyecto.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Security/PolizasAuthorization.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasApiTests.cs`

## Validaciones ejecutadas

Backend:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter PolizasSqlQueryBuilderTests
```

Resultado: `11/11` tests OK.

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `102/102` tests OK.

Permisos/gate CRUD:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter PolizasApiTests
```

Resultado: `79/79` tests OK.

Suite backend completa tras permisos/gate:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `115/115` tests OK.

Pendiente aun para fases CRUD:

- Prueba local create -> read -> update -> read -> delete con registro marcado `IdSistemaOrigen = 'origen-iLiniumTech-MVP'`.
- Implementacion real de comandos SQL create/update/delete tras los endpoints placeholder.
- Tests de validacion de DTOs de escritura.
- Tests SQL/transaccionales de create/update/delete y borrado bloqueado para registros no MVP.

## Riesgos residuales

- Las escrituras sobre `dbo.Poliza` pueden disparar triggers heredados no cubiertos por tests unitarios.
- El delete fisico no debe aplicarse a polizas existentes; la primera version debe limitarlo a registros creados por el MVP.
- `DemoSession`, API key MVP y headers locales no son seguridad productiva.
- Los campos sensibles de cliente, direccion, banco, riesgo y contacto siguen fuera de alcance hasta SDD/UAT especifica.
- Aun falta validar el flujo CRUD completo con limpieza verificable en BBDD local.

## Siguiente paso obligatorio

Antes de desarrollar el resto de paginas del menu, continuar Polizas CRUD BBDD en este orden:

1. Hecho: anadir permisos backend `polizas.create`, `polizas.update` y `polizas.delete`, sin concederlos por defecto.
2. Hecho: anadir gate `Polizas:WritesEnabled` para bloquear escrituras por defecto.
3. Pendiente: crear DTOs y validaciones de create/update.
4. Pendiente: implementar comandos SQL parametrizados y transaccionales sobre `dbo.Poliza`.
5. Pendiente: probar create/read/update/delete contra BBDD local sin dejar datos residuales sensibles.
6. Pendiente: activar UI CRUD solo cuando `/api/me` indique permisos y contexto valido.
7. Pendiente: cuando Polizas CRUD este cerrado con evidencia, crear agentes por pagina del menu para evolucionar las siguientes superficies.
