# Evidencia QA - Liquidaciones de colaborador MVP read-only

Fecha: 2026-05-16

## Alcance

Se evoluciona `/liq-col` desde placeholder a pantalla Vue estatica read-only, sin API, sin backend y sin motor heredado en runtime.

Rutas tocadas:

- `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/LiquidacionesColaboradorView.vue`
- `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/LiquidacionesColaboradorView.test.ts`
- `docs/qa/liquidaciones-colaborador-mvp-evidence.md`

## Comportamiento cubierto

- `AppShell` con banda de contexto read-only.
- Fixture local de 4 registros sanitizados.
- Filtros locales por referencia/colaborador, estado, oficina y fecha desde.
- Tabla read-only con paginacion, resultado visible y empty state.
- Acciones de detalle, conceptos y exportacion deshabilitadas.
- Copy explicito: fixture local sin API, datos minimizados, comisiones y liquidos bloqueados.
- Sin documentos, datos bancarios, importes reales, nombres reales ni connection strings.

## Comandos recomendados

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- LiquidacionesColaboradorView
npm run build
```

Auditorias recomendadas desde la raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
git diff --check
git status --short --branch
```

## Evidencia ejecutada en integracion

Comandos:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- InformesView LiquidacionesCompaniaView LiquidacionesColaboradorView
npm run test:unit
npm run build
```

Resultado:

- Formato correcto.
- Lint correcto.
- Tests dirigidos: 3 archivos, 12 tests pasados.
- Regresion frontend completa: 38 archivos, 133 tests pasados.
- Build frontend correcto.

Validaciones adicionales desde raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir $env:TEMP\... -FailOnFindings
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
git diff --check
```

Resultado:

- Documentation baseline correcto.
- Secret scan sin leaks.
- CORS audit sin findings.
- Dependency audit con 0 findings.
- Backend: 85 tests pasados.
- Whitespace diff correcto; Git solo aviso normalizacion LF/CRLF.

Smoke visual en navegador:

- Ruta: `http://127.0.0.1:5174/liq-col`.
- Estado inicial: titulo `Liquidaciones de colaborador`, `Fixture local sin API`, `Comisiones y liquidos bloqueados`, `4 liquidaciones`.
- Filtro probado: referencia/colaborador `0002`.
- Resultado filtrado: `1 liquidacion`, fila `LC-2026-0002`, colaborador `Colaborador anonimo B`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- La pantalla usa fixture local y no valida contratos reales de API.
- Existe SDD draft `docs/sdd/specs/iLiniumTech/SDD-2026-018-liq-col-read-only.md`; falta aprobacion UAT/DBA/seguridad para confirmar identidad funcional, columnas, estados, permisos reales y origen SQL.
- Comisiones, retenciones, liquidos, documentos y datos bancarios permanecen bloqueados hasta permisos y minimizacion backend.
- No se habilitan escrituras, cierres, recalculos, validaciones ni exportaciones.

## Actualizacion SDD 2026-05-20

- `T-051-LIQCOL-SDD-READONLY` formaliza la SDD draft `SDD-2026-018 Liq.Col read-only financiero minimizado`.
- La pantalla `/liq-col` sigue siendo MVP estatico fixture/read-only.
- La SDD no autoriza todavia API, SQL real, comisiones reales, liquidos, retenciones, banco, exportacion, detalle financiero ni escrituras.
