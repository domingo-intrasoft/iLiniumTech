# Evidencia QA rapida - Siniestros MVP

Fecha: 2026-05-16
Rama revisada: `codex/static-polizas-data-api`
Responsable: Jefe QA/DevOps/Gobierno IA

## Alcance

Evaluacion rapida de validacion para el siguiente incremento relacionado con `Siniestros`, sin tocar runtime frontend/backend, package files ni workflows CI.

Fuera de alcance:

- Implementar API, datos reales, permisos o UI productiva de `Siniestros`.
- Ejecutar gate local completo o comandos de deploy.
- Validar contra BBDD real, preview o UAT humano.

## Contexto revisado

- `AGENTS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/qa/definition-of-done.md`
- `tools/quality/Invoke-MvpQualityGate.ps1`
- `iLiniumTech.Frontend/package.json`
- `iLiniumTech.Backend/iLiniumTech.Backend.slnx`
- Tests frontend existentes de `Siniestros`, router y navegacion.

## Comandos ejecutados

| Comando | Resultado | Evidencia |
| --- | --- | --- |
| `git status --short --branch` | Limpio antes de documentar esta evidencia | Rama `codex/static-polizas-data-api...origin/codex/static-polizas-data-api` |
| `node --version; npm --version; dotnet --version` | Node global fuera de rango; npm y .NET disponibles | `node v18.17.0`, `npm 9.6.7`, `.NET 10.0.300` |
| `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --logger "trx;LogFileName=backend-qa-quick.trx"` | Correcto | 85 tests superados, 0 fallos, 0 omitidos. TRX: `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/TestResults/backend-qa-quick.trx` |
| `npm run test:unit -- src/features/siniestros/SiniestrosView.test.ts src/router/index.test.ts src/layout/appNavigation.test.ts` | Correcto, pero ejecutado con Node no soportado por el proyecto | 3 archivos, 10 tests superados |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | Correcto | Required docs, headings, CI files y roadmap links presentes |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | Correcto | `no leaks found` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings` | Correcto | No CORS findings detected |

## Bloqueos y limites reales

- Node global es `v18.17.0`, pero `iLiniumTech.Frontend/package.json` y `Invoke-MvpQualityGate.ps1` exigen `>=20.19.0`. El test dirigido de Vitest paso, pero no debe sustituir al gate frontend formal porque el gate oficial falla por version antes de `npm ci`, lint, unit CI y build.
- No existe backend/API de `Siniestros` en la solucion. La evidencia actual es frontend estatico/documental y navegacion.
- No existe SDD funcional aprobada para implementar `Siniestros`; `docs/appbuilder/pages/siniestros/README.md` indica que no se debe programar la pagina productiva hasta tener metadata sanitizada, SDD aprobada, matriz de permisos y validacion DBA/UAT.
- No se ejecuto `Invoke-MvpQualityGate.ps1` completo para evitar gate largo y porque el bloqueo de Node haria fallar la fase frontend.
- No se ejecuto dependency audit completo en esta pasada rapida. Para cierre de rama debe ejecutarse con Node compatible o documentar bloqueo tecnico.
- No se ejecuto smoke visual/E2E porque no hay cambio runtime en esta tarea y el entorno frontend no cumple la version Node requerida.

## Recomendacion de gate minimo para cerrar el incremento

Antes de marcar Done un incremento de `Siniestros`:

1. Instalar o apuntar a Node `>=20.19.0` y ejecutar:
   - `cd .\iLiniumTech.Frontend`
   - `npm ci`
   - `npm run format`
   - `npm run lint`
   - `npm run test:unit`
   - `npm run build`
2. Mantener como minimo los checks ya verdes:
   - `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings`
3. Ejecutar `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings` con entorno Node compatible.
4. Si se toca UI visible, anadir smoke frontend o `npm run test:e2e` si Playwright/Chromium esta disponible.
5. Si se toca backend/API/configuracion, ejecutar build backend, tests backend, CORS audit y smoke API.

## Estado ejecutivo

`Siniestros` esta validado solo como vista MVP estatica bloqueada y ruta/navegacion protegida por tests unitarios dirigidos. No esta listo para desarrollo funcional sin SDD, permisos, contrato API, decision de minimizacion PII y validacion DBA/UAT.

No se introdujeron secretos, datos reales ni runtime AppBuilder. La metadata AppBuilder revisada se mantiene como evidencia documental, no como contrato runtime.

## Actualizacion Operativa seguros 2026-05-18

Alcance de esta actualizacion:

- Sin cambios de codigo de aplicacion.
- `docs/appbuilder/pages/siniestros/README.md` queda alineado con el estado fixture actual, `SDD-2026-008` y el carril read-only minimizado.
- Se mantienen bloqueados API real, datos reales, detalle, exportacion, intervinientes, importes, EIAC, observaciones, documentos y escrituras.

Evidencia documental esperada:

- UAT confirma columnas/filtros.
- DBA confirma origen de lectura autorizado y regla por broker.
- Permisos `siniestros.catalogs` y `siniestros.read` probados en backend antes de datos reales.
- Revision de seguridad confirma ausencia de PII, importes, textos libres y EIAC en el primer corte.

Prueba ligera de esta ronda:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: ver cierre de la tarea integradora de Operativa seguros.

## Actualizacion T-001-SIN-FE-CONTRACT-FIXTURE - 2026-05-18

Alcance de esta actualizacion:

- Se separan los tipos del contrato fixture en `siniestrosTypes.ts`.
- Se separa el dataset fixture anonimizado en `siniestrosFixture.ts`.
- Se separa la logica local de filtros, paginacion y captions en `useSiniestrosFixture.ts`.
- `SiniestrosView.vue` queda como composicion de UI sobre el composable.
- No se crean servicios API, endpoints backend, datos reales, escrituras ni exportaciones.
- Las acciones de detalle/exportacion siguen bloqueadas por `siniestros-blocked-actions`.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `npx vitest run src/features/siniestros/SiniestrosView.test.ts` | OK, 2 tests |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run test:unit` | OK, 39 archivos y 198 tests |
| `npm run build` | OK |

Riesgos residuales:

- `Siniestros` sigue siendo fixture local read-only.
- No existe API real ni contrato SQL autorizado.
- El paso a datos reales sigue bloqueado por UAT, DBA, permisos y revision de seguridad segun `SDD-2026-008`.

## Actualizacion T-110-SINIESTROS-BE-READONLY-CONTRACT - 2026-05-18

Alcance de esta actualizacion:

- Se anade backend explicito in-memory para `Siniestros`.
- Endpoints disponibles: `GET /api/siniestros/catalogs` y `GET /api/siniestros`.
- Permisos propios: `siniestros.catalogs` y `siniestros.read`.
- API key legacy no concede permisos de Siniestros; se exige sesion/permisos explicitos.
- No se activa SQL real, detalle, exportacion, importes, intervinientes, EIAC, observaciones ni escrituras.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "SiniestrosApiTests"` | OK, 5 tests |

Riesgos residuales:

- El backend de `Siniestros` es todavia in-memory/read-only.
- La conexion a SQL real sigue bloqueada por UAT, DBA, permisos, minimizacion PII y revision de seguridad.

## Actualizacion T-304-SINIESTROS-SQL-READONLY-LOCAL - 2026-05-19

Alcance de esta actualizacion:

- Se anade repositorio SQL read-only activable con `Siniestros:Repository=Sql`.
- La lectura real local usa `dbo.Siniestro` como origen principal y joins minimizados a `RiesgoPoliza`, `Poliza` y `Catalogo`.
- Se aplica filtro `BrokerIntegracionId`, SQL parametrizado, whitelists de sort y `SESSION_CONTEXT`.
- El frontend `/siniestros` consume API cuando `VITE_USE_BACKEND=true`; el fixture queda solo para backend desactivado/tests offline.
- Siguen bloqueados detalle, exportacion, escrituras, intervinientes, observaciones, documentos, salud, contacto, direccion, matriculas, importes y EIAC.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Siniestros` | OK, 19 tests |
| `npm run test:unit -- Siniestros` | OK, 2 tests |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |

Smoke SQL local:

- `SKIPPED_ENV_MISSING`: no hay configuracion `Siniestros__*`, `ConnectionStrings__Siniestros*` ni `ConnectionStrings__AppBuilderMaster` visible por nombres de entorno sin imprimir secretos.

Evidencia detallada:

- `docs/qa/siniestros-sql-readonly-local-evidence.md`.

## Actualizacion T-011-SIN-FE-API-ADAPTER-BLOCKED - 2026-05-20

Alcance de esta actualizacion:

- Se revisa el frontend de `Siniestros` tras `T-304`.
- `siniestrosApi.ts` ya consumia `/api/siniestros/catalogs` y `/api/siniestros` cuando `VITE_USE_BACKEND=true`.
- El fixture queda limitado a `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se anade `siniestrosApi.test.ts` para cubrir catalogs/search en modo backend y fallback fixture sin llamadas API.
- No se toca backend, SQL, detalle, exportacion, escrituras, intervinientes, documentos, textos libres ni PII real.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Siniestros` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Riesgos residuales:

- Sigue pendiente smoke SQL real local por falta de configuracion visible sin secretos.
- Detalle, exportacion y escrituras siguen bloqueados hasta SDD/UAT/DBA especificos.
- La siguiente verificacion equivalente pasa a `Recibos`.
