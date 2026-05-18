# Evidencia QA - Clientes MVP read-only

Fecha: 2026-05-16

## Alcance

- Pantalla `/clientes` evolucionada desde placeholder a vista Vue estatica con `AppShell`.
- Datos exclusivamente fixture local, sin API, sin backend y sin metadata runtime.
- Listado read-only con 4 registros sanitizados `CLI-2026-0001` a `CLI-2026-0004`.
- Filtros locales por referencia/alias anonimo, estado, segmento y fecha alta desde.
- Acciones `Abrir ficha`, `Exportar` y `Desglose` visibles pero deshabilitadas/no operativas.

## Privacidad

- No se incorporan nombres reales, documentos reales, email, telefono, direccion ni datos bancarios.
- La tabla usa alias anonimos, identificadores demo y copy explicito de PII bloqueada.
- Los campos sensibles quedan representados como bloqueo funcional, no como datos.

## Pruebas previstas

- `npm run test:unit -- ClientesView`
- `npm run lint`
- `npm run build`
- `npm run format`
- `npm run test:unit`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`

## Resultado

- `npm run test:unit -- ClientesView`: OK, 1 archivo / 4 tests.
- `npm run format`: OK, Prettier check limpio.
- `npm run lint`: OK, 0 warnings.
- `npm run test:unit`: OK, 38 archivos / 142 tests.
- `npm run build`: OK, typecheck y build Vite completados.
- `Invoke-SecretScan.ps1`: OK, no leaks found.
- `Invoke-DependencyAudit.ps1 -FailOnFindings`: OK, total finding count 0.
- `Test-DocumentationBaseline.ps1`: OK, baseline documental presente.
- `Invoke-CorsAudit.ps1 -FailOnFindings`: OK, sin findings.
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`: OK, 85 tests backend.
- `git diff --check`: OK, sin errores de whitespace.

## Smoke visual

- Ruta: `http://127.0.0.1:5174/clientes`.
- Estado inicial: titulo `Clientes`, `Fixture local sin API`, `PII bloqueada`, `4 clientes demo`.
- Filtro probado: referencia/alias anonimo `0002`.
- Resultado filtrado: `1 cliente demo`, fila `CLI-2026-0002`, alias `Alias anonimo B`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- Falta SDD funcional de Clientes para columnas, ficha, permisos y tabs relacionadas definitivas.
- La pantalla no representa paridad AppBuilder; solo entrega un MVP estatico minimizado y honesto.
- Las acciones reales deben permanecer bloqueadas hasta API explicita, permisos efectivos, UAT y decision PII.

## Actualizacion operativa 2026-05-18

- Readiness operativo documentado en `docs/appbuilder/pages/clientes/operational-readiness.md`.
- La SDD draft `SDD-2026-010` limita el primer corte futuro a listado read-only minimizado.
- Ficha, tabs relacionadas, documento, contacto, direccion, banco, metricas, exportacion y escrituras siguen bloqueados hasta SDD posterior.

## Actualizacion contrato fixture 2026-05-18

- `ClientesView.vue` queda consumiendo `types.ts`, `fixtures.ts` y `useClientesFixture.ts`.
- Se mantiene `/clientes` como fixture/read-only sin API ni datos reales.
- PII real, documento, contacto, direccion, banco y metricas reales siguen bloqueados.
- Prueba dirigida ejecutada: `npm run test:unit -- ClientesView.test.ts` OK, 4 tests.

## Actualizacion backend read-only 2026-05-18

- Se crea API in-memory read-only: `GET /api/clientes/catalogs` y `GET /api/clientes`.
- Permisos propios: `clientes.catalogs` y `clientes.read`; la API key legacy no concede acceso.
- Contrato minimizado sin documento, contacto, direccion, banco ni PII ampliada.
- No hay SQL real, detalle, exportacion ni escrituras.
- Prueba dirigida ejecutada: `dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "Clientes"` OK, 5 tests.
