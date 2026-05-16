# Evidencia QA: Conectividad, Logs y By Aunna MVP read-only

Fecha: 2026-05-16.

## Alcance

- `iLiniumTech.Frontend/src/features/conectividad/ConectividadView.vue`
- `iLiniumTech.Frontend/src/features/conectividad/ConectividadView.test.ts`
- `iLiniumTech.Frontend/src/features/logs/LogsView.vue`
- `iLiniumTech.Frontend/src/features/logs/LogsView.test.ts`
- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaView.vue`
- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaView.test.ts`

## Resumen funcional

- `Conectividad`, `Logs` y `By Aunna` pasan de placeholder a pantallas Vue estaticas read-only con `AppShell`.
- Las tres superficies usan fixtures locales sanitizados, filtros locales, tabla paginada, empty state y acciones no operativas.
- No se conecta API, backend, router, menu, estilos globales ni metadata runtime.

## Controles de seguridad aplicados

- Copy visible: fixture local sin API.
- Copy visible: secretos, logs reales, payloads, enlaces externos y llamadas remotas bloqueados.
- Acciones de prueba externa, detalle, exportacion, publicacion y enlaces reales deshabilitadas.
- Fixtures con identificadores `*-DEMO-*`, categorias candidatas y estados/riesgos no productivos.
- Sin endpoints reales, URLs internas, credenciales, payloads reales, logs reales, datos personales ni enlaces externos.

## Pruebas cubiertas por pagina

- Render de shell read-only con banda runtime.
- Filtrado local por texto, area/categoria, estado y riesgo.
- Empty state sin resultados.
- Bloqueo de acciones sensibles.
- Ausencia de marcadores runtime, secretos, URLs reales, tokens y datos personales.

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

- `/conectividad`: titulo visible, banda `Fixture local sin API`, filtro `002`, resultado `CON-DEMO-002`, acciones sensibles deshabilitadas.
- `/logs`: titulo visible, banda `Fixture local sin API`, filtro `002`, resultado `LOG-DEMO-002`, acciones sensibles deshabilitadas.
- `/by-aunna`: titulo visible, banda `Fixture local sin API`, filtro `002`, resultado `AUN-DEMO-002`, acciones sensibles deshabilitadas.
- Errores recientes de consola: 0.

## Riesgos residuales

- Las pantallas siguen siendo MVP estaticas: no validan permisos reales ni datos de backend.
- La activacion funcional futura requiere SDD, contrato API explicito, owner de UAT y revision de seguridad.
- Cualquier detalle, descarga, publicacion, prueba remota o enlace externo debe seguir bloqueado hasta nueva decision.
