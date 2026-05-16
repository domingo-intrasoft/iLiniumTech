# Evidencia QA: Controles y Estadisticas MVP read-only

Fecha: 2026-05-16.

## Alcance

- `iLiniumTech.Frontend/src/features/controles/ControlesView.vue`
- `iLiniumTech.Frontend/src/features/controles/ControlesView.test.ts`
- `iLiniumTech.Frontend/src/features/estadisticas/EstadisticasView.vue`
- `iLiniumTech.Frontend/src/features/estadisticas/EstadisticasView.test.ts`

## Resumen funcional

- `Controles` y `Estadisticas` evolucionan desde placeholder a pantallas Vue estaticas con `AppShell`.
- Ambas superficies usan fixture local sanitizado, filtros locales, tabla paginada, empty state y acciones no operativas.
- No se conecta API, backend, router, menu, estilos globales ni metadata runtime.
- Los controles, KPIs, permisos, origenes de datos, exportaciones y drilldowns quedan marcados como candidatos o pendientes de SDD/UAT.

## Cobertura funcional

Controles:

- 4 registros fixture sanitizados.
- Filtros locales por texto, area, estado y riesgo.
- Tabla read-only con resultado, paginacion y empty state.
- Acciones `Validar alcance`, `Refrescar`, `Exportar` y `Abrir detalle` deshabilitadas.

Estadisticas:

- 4 registros fixture sanitizados.
- Filtros locales por texto, area, estado y periodo.
- Tabla read-only con resultado, paginacion y empty state.
- Acciones `Refrescar`, `Exportar`, `Drilldown` y `Cambiar periodo` deshabilitadas.

## Seguridad y datos

- No se introducen datos reales, datos personales, URLs internas, secretos, connection strings ni consultas SQL.
- Los tests verifican ausencia de marcadores peligrosos como `IAP_`, `QueryStatic`, `ComponentDataSource`, `connectionString` y `SELECT *`.
- El copy visible marca que son fixtures locales sin API y que la definicion funcional queda pendiente de SDD/UAT.

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

- `/controles`: titulo visible, banda `Fixture local sin API`, filtro `0002`, resultado `CTRL-2026-0002`, acciones sensibles deshabilitadas.
- `/estadisticas`: titulo visible, banda `Fixture local sin API`, filtro `0002`, resultado `EST-2026-0002`, acciones sensibles deshabilitadas.
- Errores recientes de consola: 0.

## Riesgos residuales

- No existe SDD funcional aprobada para activar datos reales, permisos, exportacion, drilldown o ejecucion de controles.
- Los nombres de categorias, controles y KPIs son candidatos para conversar alcance, no definiciones funcionales cerradas.
- Falta UAT owner para confirmar si estas pantallas deben avanzar mas alla del MVP read-only.
