# Liquidaciones compania MVP evidence

Fecha: 2026-05-16

## Alcance

- Pantalla `/liq-cia` evolucionada desde placeholder a MVP estatico read-only.
- Frontend Vue mantenido como codigo fuente; no consume API, metadata AppBuilder, datasource runtime ni SQL heredado.
- Fixture local de 4 registros sanitizados, sin nombres reales, importes reales, IBAN, documentos, telefonos, emails ni direcciones.
- Filtros locales: referencia/compania, estado, oficina y fecha desde.
- Acciones de detalle, desglose, guardar busqueda y exportacion visibles pero bloqueadas.

## Archivos tocados

- `iLiniumTech.Frontend/src/features/liquidaciones-compania/LiquidacionesCompaniaView.vue`
- `iLiniumTech.Frontend/src/features/liquidaciones-compania/LiquidacionesCompaniaView.test.ts`
- `docs/qa/liquidaciones-compania-mvp-evidence.md`

## Comandos recomendados

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- LiquidacionesCompaniaView
npm run build
```

Validaciones de seguridad recomendadas antes de Done de rama:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
```

## Evidencia esperada

- Render de `Liquidaciones de compania` dentro de `AppShell`.
- Banda runtime con `Solo lectura`, `Fixture local sin API`, `Datos minimizados`, `Operaciones financieras bloqueadas` y `Sin importes reales`.
- Tabla con 4 registros fixture paginados y estado empty cuando no hay coincidencias.
- Tests unitarios cubren render, paginacion, filtrado, empty state y bloqueo de datos/acciones reales.

## Comandos ejecutados

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- InformesView LiquidacionesCompaniaView LiquidacionesColaboradorView
npm run test:unit
npm run build
```

Resultados:

- Formato correcto.
- Lint correcto.
- Tests dirigidos de Informes/Liq.Cia/Liq.Col: 3 archivos, 12 tests pasados.
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

Resultados:

- Documentation baseline correcto.
- Secret scan sin leaks.
- CORS audit sin findings.
- Dependency audit con 0 findings.
- Backend: 85 tests pasados.
- Whitespace diff correcto; Git solo aviso normalizacion LF/CRLF.

Smoke visual en navegador:

- Ruta: `http://127.0.0.1:5174/liq-cia`.
- Estado inicial: titulo `Liquidaciones de compania`, `Fixture local sin API`, `Operaciones financieras bloqueadas`, `4 liquidaciones`.
- Filtro probado: referencia/compania `0002`.
- Resultado filtrado: `1 liquidacion`, fila `LCIA-2026-0002`, compania `Compania demo sur`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- Existe SDD draft `docs/sdd/specs/iLiniumTech/SDD-2026-017-liq-cia-read-only.md`; falta aprobacion UAT/DBA/seguridad para confirmar columnas, estados, permisos reales y origen SQL.
- No existe contrato API de liquidaciones de compania; cualquier conexion futura debe definirse en backend explicito.
- Detalle, desglose, exportacion, banco, facturas, importes reales y operaciones financieras siguen bloqueados.
- La comparativa con AppBuilder queda pendiente de metadata sanitizada o validacion autorizada.
- Los fallos intermedios observados durante el trabajo paralelo quedaron resueltos en la integracion final.

## Actualizacion SDD 2026-05-20

- `T-130-LIQCIA-SDD-READONLY` formaliza la SDD draft `SDD-2026-017 Liq.Cia read-only financiero minimizado`.
- La pantalla `/liq-cia` sigue siendo MVP estatico fixture/read-only.
- La SDD no autoriza todavia API, SQL real, importes, banco, facturas, exportacion, detalle financiero ni escrituras.
