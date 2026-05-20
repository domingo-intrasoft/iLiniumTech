# Agenda MVP read-only - evidencia QA

Fecha: 2026-05-16

## Alcance

- Pantalla `/agenda` evolucionada desde placeholder a vista Vue estatica con `AppShell`.
- Datos limitados a fixture local sanitizado de 4 eventos demo.
- Filtros locales por texto/referencia, estado, prioridad y fecha desde.
- Listado read-only con paginacion, empty state y acciones no operativas.

## Fuera de alcance confirmado

- Sin API backend.
- Sin calendario dinamico ni drag/drop.
- Sin escrituras, workflows, alta, reprogramacion, exportacion real o desglose operativo.
- Sin metadata AppBuilder como contrato runtime.
- Sin datos personales reales, descripciones libres sensibles ni asuntos reales.

## Evidencia de seguridad funcional

- La banda runtime declara: solo lectura, fixture local sin API, sin calendario dinamico y PII/asuntos sensibles bloqueados.
- Crear, reprogramar, exportar y desglose se renderizan deshabilitados.
- El fixture usa referencias demo (`AGE-2026-*`, `POL-DEMO-*`, `SIN-DEMO-*`, `REC-DEMO-*`) y asuntos genericos.

## Pruebas previstas

- `npm run test:unit -- AgendaView`
- `npm run lint`
- `npm run build`

## Resultado

- `npm run test:unit -- AgendaView`: OK, 4 tests Agenda verdes.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: OK, 38 archivos y 142 tests verdes.
- `npm run build`: OK, typecheck y build Vite completados.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK, no leaks found.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings`: OK, sin findings.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir $env:TEMP\... -FailOnFindings`: OK, total finding count 0.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`: OK, 85 tests backend.
- `git diff --check`: OK, sin errores de whitespace.

## Smoke visual

- Ruta: `http://127.0.0.1:5174/agenda`.
- Estado inicial: titulo `Agenda`, `Fixture local sin API`, `Sin calendario dinamico`, `4 eventos`.
- Filtro probado: texto/referencia `0002`.
- Resultado filtrado: `1 evento`, fila `AGE-2026-0002`, asunto `Seguimiento demo de tramite`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- La pantalla sigue sin SDD funcional completa ni contrato API; solo habilita un MVP visual read-only.
- La futura conexion a datos reales requiere auth, broker efectivo, permisos `agenda.read`, minimizacion PII y validacion DBA/UAT.

## Actualizacion operativa 2026-05-18

- Readiness operativo documentado en `docs/appbuilder/pages/agenda/operational-readiness.md`.
- La SDD draft `SDD-2026-011` limita el primer corte futuro a listado read-only por rango.
- Calendario visual, detalle, descripcion larga, participantes, drag/drop, reprogramacion, exportacion y escrituras siguen bloqueados hasta SDD posterior.

## Actualizacion contrato fixture 2026-05-18

- `AgendaView.vue` queda consumiendo `types.ts`, `fixtures.ts` y `useAgendaFixture.ts`.
- Se mantiene `/agenda` como fixture/read-only sin API ni datos reales.
- PII real, asunto sensible, participantes, descripcion larga y acciones mutantes de calendario siguen bloqueados.
- Prueba dirigida ejecutada: `npm run test:unit -- AgendaView.test.ts` OK, 4 tests.

## Actualizacion backend read-only 2026-05-18

- Se crea API in-memory read-only: `GET /api/agenda/catalogs`, `GET /api/agenda` y alias `GET /api/agenda/events`.
- Permisos propios: `agenda.catalogs` y `agenda.read`; la API key legacy no concede acceso.
- Contrato minimizado sin descripcion larga, participantes, `IdentidadId`, calendario mutante, drag/drop ni escrituras.
- Se valida rango maximo defensivo de fechas y sort por whitelist.
- Prueba dirigida ejecutada: `dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "Agenda"` OK, 6 tests.

## Actualizacion T-310-AGENDA-FE-CRUD-API-ADAPTER-VERIFY - 2026-05-20

Alcance de esta actualizacion:

- Se revisa el frontend de `Agenda` tras `SDD-2026-015`.
- `agendaApi.ts` ya consumia `/api/agenda` cuando `VITE_USE_BACKEND=true`.
- El fixture queda limitado a `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Las funciones de escritura (`createAgendaEvent`, `updateAgendaEvent`, `deleteAgendaEvent`) fallan antes de llamar a API si el backend no esta activo.
- Se anade `agendaApi.test.ts` para cubrir busqueda backend, mapeo minimizado, fallback fixture, bloqueo de escrituras sin backend y llamadas CRUD explicitas en modo backend.
- No se toca backend, SQL, permisos, flags, menu, layout ni se activan escrituras reales nuevas.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Agenda` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Riesgos residuales:

- Sigue pendiente smoke SQL real local por falta de configuracion visible sin secretos.
- Escrituras reales dependen de `Agenda:WritesEnabled`, permisos backend y SDD-2026-015; esta tarea no cambia esa configuracion.
- La siguiente verificacion equivalente pasa a `Clientes`.
