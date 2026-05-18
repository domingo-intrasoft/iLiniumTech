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
- La validacion inicial contempla campos aprobados por SDD: numero, aplicacion, ciaId, clienteId, estado, ramo, tipoPoliza, fechaEfecto y fechaVencimiento.
- `primaAnual` queda read-only en CRUD hasta que DBA/UAT confirme una columna persistible en el modelo real usado para smoke.
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
- El alta escribe `IdSistemaOrigen` con un valor ya catalogado en la BBDD local y exige numero tecnico con prefijo `ILMVP-`.
- La BBDD local no contiene aun el catalogo propio `origen-iLiniumTech-MVP`; queda como decision DBA/UAT si se quiere crear ese origen mas adelante.
- El alta incorpora defaults catalogados para fraccion de pago y gestor, necesarios para superar constraints heredadas.
- `BuildUpdateCommand` genera `UPDATE` parametrizado por `dbo.Poliza.Id`.
- La actualizacion inicial queda restringida a registros con marcador MVP para mantener el CRUD local reversible.
- `BuildDeleteCommand` no hace borrado fisico: actualiza el numero a `ILMVP-DELETED-<Id>` y lo oculta del listado/detalle.
- La baja tecnica inicial queda restringida a registros con prefijo `ILMVP-` no dados de baja previamente.
- Los comandos de update/delete reciben id entero validado y usan `@id` con tipo `Int`.
- Al comprobar BBDD local, el origen propio `origen-iLiniumTech-MVP` no existe en catalogo; la defensa temporal usa origen catalogado y prefijo tecnico `ILMVP-` para update/delete.
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

## Broker real AppBuilder

Hallazgo durante preparacion de smoke local contra BBDD:

- Las conexiones modelo activas en el maestro local pueden usar `IdentityId` negativo.
- La primera regla MVP de login/contexto rechazaba `brokerId <= 0`, lo que impedia probar con `CurrentBrokerId` real de AppBuilder.
- La regla se ajusta a `brokerId != 0` para broker y allowed brokers.
- `UserId` y `ProfileId` mantienen validacion positiva cuando aplica.
- Se anaden tests de login demo y contexto por headers/configuracion con broker negativo.

Archivos modificados:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Security/HeaderPolizasExecutionContextAccessor.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/Connections/ConfiguredPolizasExecutionContextAccessor.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasApiTests.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/HeaderPolizasExecutionContextAccessorTests.cs`

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

Broker negativo AppBuilder:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasApiTests|HeaderPolizasExecutionContextAccessorTests|PolizasInfrastructureTests"
```

Resultado: `98/98` tests OK.

Suite backend completa tras broker real AppBuilder:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `142/142` tests OK.

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

CRUD BBDD, baja tecnica y UI:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasSqlCommandBuilderTests|PolizasSqlQueryBuilderTests|PolizasWriteValidatorTests|PolizasApiTests|HeaderPolizasExecutionContextAccessorTests"
```

Resultado: `127/127` tests OK.

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: `144/144` tests OK.

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
```

Resultado: format/lint/build OK; unit tests `198/198` OK.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
git diff --check
```

Resultado: secret scan limpio, baseline documental OK, dependency audit `0` findings, CORS audit OK y diff check sin errores.

Smoke navegador:

- `http://127.0.0.1:5174/polizas` carga tras login demo local.
- En modo fixture local la pantalla queda en `Solo lectura`, muestra `Fixture local` y el boton `Nueva poliza MVP` permanece deshabilitado, como corresponde sin backend BBDD/permisos de escritura.

Validacion local BBDD autorizada:

- Prueba SQL transaccional con rollback sobre BBDD local: `INSERT` OK, `UPDATE` OK y baja tecnica `ILMVP-DELETED-` OK.
- Prueba transaccional ejecutando el `PolizasSqlCommandBuilder` actual: `BuilderInsert=OK`, `BuilderUpdateRows=1`, `BuilderSoftDeleteRows=1`.
- La tabla `dbo.Poliza` tiene triggers activos; el intento de borrado fisico genero conflicto de FK con datos dependientes creados por triggers.
- Conteo posterior en conexion nueva: `0` filas `ILMVP-%` residuales.

Smoke API/UI real contra backend SQL local:

- Resolucion local sanitizada del maestro AppBuilder: OK, sin imprimir connection strings ni credenciales.
- El broker local usado inicialmente resolvia una BBDD modelo con `dbo.Poliza`, pero sin `dbo.Pantalla_Polizas`; se descarta para smoke de lectura real.
- Se selecciona un broker local de pruebas que resuelve una BBDD modelo con `dbo.Pantalla_Polizas` y `dbo.Poliza`.
- Hallazgo de esquema: la vista real `dbo.Pantalla_Polizas` disponible para el smoke no expone `NombreCompleto`, `Cia`, `Riesgo` ni `PAnualCartera`.
- Ajuste aplicado: la lectura SQL proyecta `ClienteId`, `CiaId`, `Riesgo` vacio y `PrimaAnual = 0` para respetar el contrato frontend sin inventar joins ni activar metadata runtime.
- `dotnet run` temporal de smoke API con backend SQL, `DemoSession`, `Polizas:WritesEnabled=true` y permisos `polizas.create/update/delete`: `ApiSqlSmoke=OK`.
- Resultado smoke API: `/ready` 200, login 200, `/api/me` con permisos CRUD, catalogos 200, listado SQL 200 con `5` items sobre total `47778`.
- Las llamadas no mutantes `POST /api/polizas`, `PUT /api/polizas/not-an-id` y `DELETE /api/polizas/not-an-id` devolvieron `400 POLIZAS_VALIDATION_ERROR`; no crean, actualizan ni eliminan datos.
- Smoke UI con frontend temporal `VITE_USE_BACKEND=true` y `VITE_AUTH_MODE=demo-session`: `UiSqlSmoke=OK`, URL `/polizas?page=1&pageSize=25`, `25` filas renderizadas, badge total `(47778)` y boton `Nueva poliza MVP` habilitado.
- Captura local no versionada generada en `%TEMP%\iliniumtech-ui-playwright-smoke\polizas-sql-smoke.png`.

Smoke API CRUD mutante con rollback:

- Se retira `primaAnual` del camino de escritura real para no depender de `PAnualCartera`.
- `POST /api/polizas` y `PUT /api/polizas/{id}` rechazan `primaAnual` explicito con `POLIZAS_VALIDATION_ERROR`.
- Smoke temporal in-process con `WebApplicationFactory`, backend SQL local, `DemoSession`, `Polizas:WritesEnabled=true` y `TransactionScope` rollback: `ApiCrudRollbackSmoke=OK`.
- Resultado smoke CRUD: login 200, create 201, id creado numerico, update 204, delete/baja tecnica 204, detalle posterior 404.
- Conteo verificable sobre BBDD modelo: `ResidualBefore=0` y `ResidualAfterRollback=0` para filas `ILMVP-%`.
- Observacion: la fila sintetica creada en transaccion no queda visible por `dbo.Pantalla_Polizas` antes de la baja tecnica; queda pendiente cerrar con DBA/UAT si las altas MVP deben aparecer inmediatamente en la vista heredada o si se requiere otro contrato de lectura post-create.

Validacion adicional tras retirar `primaAnual` de escritura:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasSqlCommandBuilderTests|PolizasWriteValidatorTests|PolizasApiTests"
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
git diff --check
```

Resultados: backend especifico `109/109` OK, backend completo `147/147` OK, frontend unit `198/198` OK, format/lint/build OK, documentation baseline OK, secret scan sin leaks, dependency audit `0` findings y diff check sin errores.

## Riesgos residuales

- Las escrituras sobre `dbo.Poliza` pueden disparar triggers heredados no cubiertos por tests unitarios.
- El delete fisico no debe aplicarse a polizas existentes ni a altas MVP mientras no haya regla UAT/DBA; la version actual aplica baja tecnica y oculta el registro.
- `DemoSession`, API key MVP y headers locales no son seguridad productiva.
- Los campos sensibles de cliente, direccion, banco, riesgo y contacto siguen fuera de alcance hasta SDD/UAT especifica.
- La BBDD usada para smoke real de lectura no expone `PAnualCartera`; `primaAnual` queda read-only hasta decision DBA/UAT.
- El flujo create/update/baja tecnica via API queda validado con rollback, pero la lectura post-create por `dbo.Pantalla_Polizas` no muestra la fila sintetica; queda pendiente decidir contrato funcional de visibilidad.

## Siguiente paso obligatorio

Antes de desarrollar el resto de paginas del menu, continuar Polizas CRUD BBDD en este orden:

1. Hecho: anadir permisos backend `polizas.create`, `polizas.update` y `polizas.delete`, sin concederlos por defecto.
2. Hecho: anadir gate `Polizas:WritesEnabled` para bloquear escrituras por defecto.
3. Hecho: crear DTOs y validaciones de create/update.
4. Hecho: crear builder de comandos SQL parametrizados e integracion transaccional sobre `dbo.Poliza`.
5. Hecho parcial: probar create/update/baja tecnica contra BBDD local con rollback y sin filas residuales `ILMVP-%`.
6. Hecho parcial: activar UI CRUD solo cuando `/api/me` indique permisos y contexto valido.
7. Hecho: smoke API/UI real contra backend SQL local con lectura, permisos CRUD y validaciones no mutantes.
8. Hecho: retirar `primaAnual` de escritura y ejecutar create/update/baja tecnica via API SQL con rollback y limpieza verificable.
9. Pendiente: cerrar contrato de lectura post-create, porque `dbo.Pantalla_Polizas` no muestra la fila sintetica creada en transaccion.
10. Pendiente: cuando Polizas CRUD este cerrado con evidencia, crear agentes por pagina del menu para evolucionar las siguientes superficies.
