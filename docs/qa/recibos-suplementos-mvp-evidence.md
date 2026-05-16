# Evidencia QA minima - Recibos + Suplementos MVP

Fecha: 2026-05-16

Rol: Jefe QA/DevOps/Gobierno IA para el incremento combinado Recibos + Suplementos.

## Alcance gobernado

- Incremento combinado de paginas estaticas Recibos y Suplementos.
- Propiedad de esta pasada: este reporte solamente.
- No se modifico frontend runtime, backend runtime, package files, readiness/workflows ni scripts.
- Recibos y Suplementos siguen en carril de MVP estatico visible, bloqueado para datos o acciones reales hasta SDD/API/UAT.
- No hay API ni datos reales conectados para este incremento en la evidencia revisada. Cualquier listado con importes, detalle, filtros reales, exportaciones, acciones, PII o permisos finos requiere SDD, contrato API explicito, seguridad y UAT/DBA.

## Estado inicial observado

Comando:

```powershell
git status --short
```

Resultado inicial relevante:

```text
 D iLiniumTech.Frontend/src/features/recibos/RecibosView.vue
```

Durante la ejecucion, el estado cambio y tambien aparecio borrado:

```text
 D iLiniumTech.Frontend/src/features/suplementos/SuplementosView.vue
```

Posteriormente, mientras se cerraba este reporte, los mismos archivos reaparecieron como modificados en el working tree:

```text
 M iLiniumTech.Frontend/src/features/recibos/RecibosView.test.ts
 M iLiniumTech.Frontend/src/features/recibos/RecibosView.vue
 M iLiniumTech.Frontend/src/features/suplementos/SuplementosView.test.ts
 M iLiniumTech.Frontend/src/features/suplementos/SuplementosView.vue
?? docs/qa/recibos-suplementos-mvp-evidence.md
```

Limitacion: hay cambios concurrentes en frontend de Recibos/Suplementos hechos por otros agentes o por el usuario. No se revirtieron ni se tocaron. La evidencia dirigida se re-ejecuto tras detectar esa restauracion/modificacion concurrente.

## Entorno local

Comando:

```powershell
node --version; npm --version
```

Resultado:

```text
v18.17.0
9.6.7
```

Bloqueo: `iLiniumTech.Frontend/package.json` exige Node `>=20.19.0`. Por tanto, aunque Vitest llego a arrancar, el Node global no cumple el requisito para validar Done formal de frontend con confianza. Para cierre formal usar Node compatible o el parametro `-NodeExe` del gate local.

## Tests dirigidos frontend

Comando:

```powershell
cd .\iLiniumTech.Frontend
npm run test:unit -- src/features/recibos/RecibosView.test.ts src/features/suplementos/SuplementosView.test.ts src/router/index.test.ts src/layout/appNavigation.test.ts src/layout/AppSideMenu.test.ts
```

Primer resultado: fallo.

Resumen verificable:

- `src/layout/appNavigation.test.ts`: 4 tests pasados.
- `src/layout/AppSideMenu.test.ts`: 4 tests pasados.
- `src/router/index.test.ts`: fallo por no resolver `@/features/recibos/RecibosView.vue`.
- `src/features/recibos/RecibosView.test.ts`: fallo por no resolver `./RecibosView.vue`.
- `src/features/suplementos/SuplementosView.test.ts`: fallo por no resolver `./SuplementosView.vue`.

Diagnostico QA inicial: las rutas/nav tenian cobertura parcial, pero el incremento no podia considerarse validado mientras los componentes Vue de Recibos y Suplementos estuvieran ausentes del working tree.

Segundo comando ejecutado tras detectar restauracion/modificacion concurrente de los componentes:

```powershell
cd .\iLiniumTech.Frontend
npm run test:unit -- src/features/recibos/RecibosView.test.ts src/features/suplementos/SuplementosView.test.ts src/router/index.test.ts src/layout/appNavigation.test.ts src/layout/AppSideMenu.test.ts
```

Segundo resultado: correcto.

Resumen verificable:

- `src/layout/appNavigation.test.ts`: 4 tests pasados.
- `src/layout/AppSideMenu.test.ts`: 4 tests pasados.
- `src/features/suplementos/SuplementosView.test.ts`: 4 tests pasados.
- `src/features/recibos/RecibosView.test.ts`: 2 tests pasados.
- `src/router/index.test.ts`: 5 tests pasados.
- Total: 5 archivos de test pasados, 19 tests pasados.

Nota de gobierno: esta validacion dirigida queda condicionada por Node global `v18.17.0`, que no cumple el engine requerido `>=20.19.0`, y por cambios concurrentes en los archivos frontend.

## Validaciones de documentacion y seguridad

### Documentation baseline

Comando:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: correcto.

Salida clave:

```text
# Documentation Baseline
Required docs, SDD headings, CI files and roadmap links are present.
```

### Secret scan

Comando:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
```

Resultado: correcto.

Salida clave:

```text
no leaks found
```

### CORS audit

Comando:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

Resultado: correcto.

Salida clave:

```text
# CORS Audit
No CORS findings detected.
```

### Dependency audit

Comando ejecutado con reporte en carpeta temporal, para no dejar artefactos en el repo:

```powershell
$reportDir = Join-Path $env:TEMP ("iliniumtech-dependency-audit-" + (Get-Date -Format "yyyyMMddHHmmss"))
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir $reportDir -FailOnFindings
```

Resultado: correcto.

Salida clave:

```text
## npm
### .\iLiniumTech.Frontend
- total: 0

## .NET
- no vulnerable packages detected en proyectos Api, Application, Domain, Infrastructure y Tests

## Result
Total finding count: 0
```

### Whitespace diff

Comando:

```powershell
git diff --check
```

Resultado: correcto, sin errores de whitespace. En la pasada final Git emitio avisos de normalizacion LF/CRLF para archivos frontend modificados por otros agentes, sin fallo del comando.

## Validaciones finales de coordinacion

Despues de estabilizar los cambios de Recibos y Suplementos, el coordinador ejecuto una pasada final adicional.

### Formato, lint y build frontend

Comandos:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run build
```

Resultado: correcto.

Nota: los comandos se ejecutaron con Node global `v18.17.0`, inferior al engine declarado `>=20.19.0`. Aunque pasaron localmente, el cierre formal debe preferir Node compatible o `Invoke-MvpQualityGate.ps1 -NodeExe`.

### Regresion backend

Comando:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado: correcto.

Resumen verificable:

- 85 tests pasados.
- 0 fallos.
- 0 omitidos.

### Smoke visual en navegador

Rutas verificadas en `http://127.0.0.1:5174`:

- `/recibos`: resumen inicial `3 recibos`; filtro `0002`; resumen filtrado `1 recibo`; tabla contiene `REC-2026-0002` y `Cliente anonimo 2`; 0 errores nuevos de consola.
- `/suplementos`: resumen inicial `4 suplementos`; filtro `0002`; resumen filtrado `1 suplemento`; tabla contiene `SUP-2026-0002` y `Regularizacion`; 0 errores nuevos de consola.

La verificacion confirma que ambas paginas son visibles, filtrables y siguen marcadas como fixture local sin API.

## Criterios para Done formal

Para marcar el incremento Recibos + Suplementos como Done formal deben cumplirse, como minimo:

- Mantener `iLiniumTech.Frontend/src/features/recibos/RecibosView.vue` y `iLiniumTech.Frontend/src/features/suplementos/SuplementosView.vue` presentes y revisables, sin consumir metadata AppBuilder en runtime.
- Mantener acciones no operativas/deshabilitadas y copy claro de bloqueo para datos reales.
- Ejecutar con Node `>=20.19.0`: `npm run format`, `npm run lint`, `npm run test:unit` y `npm run build`.
- Repetir tests dirigidos de Recibos, Suplementos, router y nav.
- Si cambia UI visible, aportar smoke visual desktop/mobile.
- Repetir `Test-DocumentationBaseline.ps1`, secret scan, dependency audit, CORS audit si toca API/configuracion y `git diff --check`.
- Documentar que no se introducen secretos, connection strings reales, dumps, capturas sensibles ni datos personales reales.
- No conectar API, importes, filtros reales, detalle, exportacion, permisos finos ni datos reales sin SDD/API/UAT/DBA.

## Riesgos residuales

- Riesgo de coordinacion actual: los componentes Vue de Recibos y Suplementos fueron borrados y despues restaurados/modificados durante esta misma pasada; la evidencia debe re-ejecutarse cuando los agentes frontend estabilicen sus cambios.
- Bloqueo de entorno: Node global `v18.17.0` no cumple `>=20.19.0`.
- Bloqueo funcional/producto: no hay metadata completa, layout confirmado, columnas/filtros/permisos ni UAT para datos reales de Recibos o Suplementos.
- Riesgo financiero/PII: Recibos puede implicar importes, cobros, remesas o cliente; Suplementos puede implicar cambios de poliza y datos personales. Mantener solo placeholder estatico hasta SDD.
- Riesgo de coordinacion: otros agentes estan tocando frontend; cualquier evidencia frontend debe re-ejecutarse tras estabilizar esos cambios.

## Archivos modificados por esta pasada

- `docs/qa/recibos-suplementos-mvp-evidence.md`
