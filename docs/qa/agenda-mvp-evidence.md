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
