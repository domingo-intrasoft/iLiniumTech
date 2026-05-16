# Evidencia QA: Administracion y Configuracion MVP read-only

Fecha: 2026-05-16.

## Alcance

- `iLiniumTech.Frontend/src/features/administracion/AdministracionView.vue`
- `iLiniumTech.Frontend/src/features/administracion/AdministracionView.test.ts`
- `iLiniumTech.Frontend/src/features/configuracion/ConfiguracionView.vue`
- `iLiniumTech.Frontend/src/features/configuracion/ConfiguracionView.test.ts`

## Resumen funcional

- Se sustituyen los placeholders `MvpPageShell` por pantallas Vue estaticas con `AppShell`.
- Ambas superficies quedan read-only y usan fixture local sanitizado.
- No hay llamadas API, escrituras, backend nuevo ni metadata runtime.
- Cada pagina incluye 4 registros demo, filtros locales, tabla paginada y empty state.
- Las acciones administrativas, permisos, secretos y cambios de configuracion quedan deshabilitados.

## Seguridad y datos

- No se versionan usuarios reales, permisos reales, claves, connection strings, valores de entorno, URLs internas ni credenciales.
- Los datos visibles son identificadores `*-DEMO-*` y etiquetas candidatas.
- El copy visible marca que la pantalla usa fixture local sin API y que requiere SDD, API explicita, permisos productivos y UAT antes de activar datos reales.

## Pruebas ejecutadas

| Comando | Resultado |
| --- | --- |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run test:unit -- AdministracionView ConfiguracionView ControlesView EstadisticasView ConectividadView LogsView ByAunnaView` | OK, 7 files, 28 tests |
| `npm run test:unit` | OK fuera de sandbox, 38 files, 163 tests. En sandbox fallaba por resolucion de ruta `C:/Users/CodexSandboxOffline/...`, no por codigo. |
| `npm run build` | OK fuera de sandbox. En sandbox fallaba por ruta relativa emitida por Vite/Rollup hacia el workspace real, no por codigo. |
| `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release` | OK fuera de sandbox, 85 tests passed. En sandbox no podia leer `NuGet.Config` del perfil de usuario. |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport` | OK fuera de sandbox. En sandbox `gitleaks` no estaba en `PATH`. |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir <temp> -FailOnFindings` | OK, npm 0 findings y .NET sin paquetes vulnerables. |

## Smoke visual

Validado en navegador embebido contra `http://127.0.0.1:5174` con login demo:

- `/administracion`: titulo visible, banda `Fixture local sin API`, filtro `AUDITORIA`, resultado `ADM-DEMO-AUDITORIA`, acciones sensibles deshabilitadas.
- `/configuracion`: titulo visible, banda `Fixture local sin API`, filtro `SEGURIDAD`, resultado `CFG-DEMO-SEGURIDAD`, acciones sensibles deshabilitadas.
- Errores recientes de consola: 0.

## Riesgos residuales

- Las pantallas siguen siendo MVP estaticas read-only.
- La activacion funcional futura requiere SDD, contrato API explicito, permisos productivos, threat review y UAT.
- Cualquier gestion real de usuarios, permisos, secretos o configuracion debe implementarse en una tarea separada con validacion de seguridad.
