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

## Contrato y validacion de escritura

Incremento aplicado despues del gate:

- Se crean requests explicitos `PolizaCreateRequest` y `PolizaUpdateRequest`.
- Se crea `PolizasWriteValidator` en Application para validar sin depender de ASP.NET ni SQL.
- `POST /api/polizas` valida payload minimo antes del placeholder.
- `PUT /api/polizas/{id}` valida id tecnico numerico y payload parcial.
- `DELETE /api/polizas/{id}` valida id tecnico numerico.
- Los ids de escritura deben ser enteros positivos, alineados con `dbo.Poliza.Id`; `Poliza.Poliza` no vale como id de escritura.
- La validacion inicial solo contempla campos aprobados por SDD: numero, aplicacion, ciaId, clienteId, estado, ramo, tipoPoliza, fechaEfecto, fechaVencimiento y primaAnual.
- No se incorporan documento, telefono, email, direccion, banco, riesgo completo, matricula, bastidor ni campos de cobro/gestores.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Domain/Polizas/PolizasWriteRequests.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Application/Polizas/PolizasWriteValidator.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasApiTests.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasWriteValidatorTests.cs`

## Builder SQL de comandos

Incremento aplicado despues del contrato:

- Se crea `PolizasSqlCommandBuilder`.
- `BuildCreateCommand` genera `INSERT` parametrizado sobre `dbo.Poliza`.
- El alta escribe siempre `IdSistemaOrigen = 'origen-iLiniumTech-MVP'`.
- `BuildUpdateCommand` genera `UPDATE` parametrizado por `dbo.Poliza.Id`.
- La actualizacion inicial queda restringida a registros con marcador MVP para mantener el CRUD local reversible.
- `BuildDeleteCommand` genera `DELETE` parametrizado por `dbo.Poliza.Id`.
- El borrado fisico inicial queda restringido a registros con marcador MVP.
- Los comandos de update/delete reciben id entero validado y usan `@id` con tipo `Int`.
- El origen MVP se pasa como `@idSistemaOrigen`, no concatenado como literal de SQL.
- Los tests verifican que valores de usuario no se concatenan en `CommandText`.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/Sql/PolizasSqlCommandBuilder.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasSqlCommandBuilderTests.cs`

## Integracion backend de escritura

Incremento aplicado despues del builder:

- Se crea `IPolizasWriteRepository`, separado del repositorio read-only.
- `PolizasService` valida y delega `CreateAsync`, `UpdateAsync` y `DeleteAsync`.
- Los endpoints `POST`, `PUT` y `DELETE` dejan de devolver placeholder `501`.
- `POST /api/polizas` devuelve `201 Created` con id creado.
- `PUT /api/polizas/{id}` devuelve `204 NoContent` o `POLIZAS_NOT_FOUND_OR_NOT_WRITABLE`.
- `DELETE /api/polizas/{id}` devuelve `204 NoContent` o `POLIZAS_NOT_FOUND_OR_NOT_WRITABLE`.
- `SqlPolizasWriteRepository` abre conexion, aplica `SESSION_CONTEXT`, ejecuta en transaccion y usa comandos parametrizados.
- `InMemoryPolizasRepository` soporta un flujo minimo create/update/delete para pruebas locales sin BBDD; update/delete solo aceptan ids creados por el propio MVP.
- `DependencyInjection` registra repositorio de escritura SQL cuando `Polizas:Repository=Sql`.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Application/Polizas/IPolizasWriteRepository.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Application/Polizas/IPolizasService.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Application/Polizas/PolizasService.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Domain/Polizas/PolizasWriteResults.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/DependencyInjection.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/InMemoryPolizasRepository.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/SqlPolizasWriteRepository.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasApiTests.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasInfrastructureTests.cs`

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

Contrato y validacion de escritura:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasApiTests|PolizasWriteValidatorTests"
```

Resultado: `98/98` tests OK.

Suite backend completa tras contrato/validacion:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `134/134` tests OK.

Builder SQL de comandos:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter PolizasSqlCommandBuilderTests
```

Resultado: `5/5` tests OK.

Builder SQL junto a API y validadores:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasSqlCommandBuilderTests|PolizasWriteValidatorTests|PolizasApiTests"
```

Resultado: `103/103` tests OK.

Suite backend completa tras builder SQL:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `139/139` tests OK.

Integracion backend de escritura:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter PolizasApiTests
```

Resultado: `81/81` tests OK.

Suite backend completa tras integracion:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `139/139` tests OK.

Pendiente aun para fases CRUD:

- Prueba local create -> read -> update -> read -> delete con registro marcado `IdSistemaOrigen = 'origen-iLiniumTech-MVP'`.
- Probar endpoints reales create/update/delete contra BBDD local autorizada con limpieza verificable.
- Confirmar con DBA/UAT defaults/triggers de `dbo.Poliza` antes de considerar el CRUD SQL cerrado.
- Activar UI CRUD solo cuando permisos y gate esten presentes.

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
3. Hecho: crear DTOs y validaciones de create/update.
4. Hecho: crear builder de comandos SQL parametrizados e integracion transaccional sobre `dbo.Poliza`.
5. Pendiente: probar create/read/update/delete contra BBDD local sin dejar datos residuales sensibles.
6. Pendiente: activar UI CRUD solo cuando `/api/me` indique permisos y contexto valido.
7. Pendiente: cuando Polizas CRUD este cerrado con evidencia, crear agentes por pagina del menu para evolucionar las siguientes superficies.
