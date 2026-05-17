# Avance autonomo IA - 2026-05-17

Rama: `codex/static-polizas-data-api`
Ventana: trabajo autonomo solicitado para avanzar sin supervision humana durante el dia.

## Objetivo operativo

Avanzar el MVP sin activar datos reales, sin reintroducir runtime dinamico AppBuilder, manteniendo `Autos Particulares` aparcado y dejando evidencias para futuras IA.

## Incrementos cerrados

| Commit | Area | Resultado |
| --- | --- | --- |
| `ea06fe3` | Frontend QA | Smoke responsive de login, shell, menu, Polizas y Clientes en viewport movil. |
| `6ee60c4` | Frontend QA | Smoke de guardas: rutas protegidas, login demo local, logout y limpieza de sesion. |
| `5db2022` | QA docs | Matriz viva de cobertura Playwright E2E en `docs/qa/e2e-smoke-coverage.md`. |
| `07685bd` | Quality gate | Gate local prueba `DemoSession`: login, `/api/me`, cambio de broker y logout. |
| `4e90cee` | Backend seguridad | Headers MVP fuera de Development requieren opt-in demo tambien en runtime, no solo en `/ready`. |
| `e426838` | Backend seguridad | `DemoSession` fuera de Development exige opt-in y `Auth:Demo:Password` explicita no predeterminada. |
| `45e5781` | Backend seguridad | API key MVP solo concede permisos funcionales `polizas.*` en Development. |
| `f129f48` | Frontend UX/auth | Login muestra mensaje generico cuando una sesion backend no puede confirmarse y conserva redirect. |
| `dfc30e9` | Backend QA | Regresiones que prueban que denegaciones de permisos/auth cortocircuitan antes de tocar servicio de Polizas. |
| `b859b0d` | QA docs | DoD de fase 7 alineado con la cobertura E2E real. |
| `d486a30` | QA/backend docs | Smoke de paridad menu/rutas, test de broker denegado y cierre operativo MVP login/menu/Polizas. |
| `91da188` | Auth docs | Matriz documental de permisos productivos para broker, perfil, oficina, gestor y usuario. |
| `e659589` | Frontend accesibilidad | Menu lateral colapsado sale del arbol de foco con `inert` y mantiene `aria-hidden`. |
| `f0728db` | Frontend accesibilidad | Estados del menu expuestos por `aria-describedby` sin alterar nombres de enlace; `sr-only` no genera overflow movil. |
| `75d55a4` | Frontend accesibilidad | Skip link del shell mueve foco real al contenido principal y crea `tabindex=-1` si falta. |
| `3664c64` | QA docs | Gate local completo registrado tras los incrementos autonomos. |
| `a32a23a` | QA docs | Log autonomo alineado con commits cerrados. |
| `7916670` | Frontend Polizas | Listado explica de forma visible/accesible cuando `polizas.detail` no permite abrir detalle. |
| `281e924` | Frontend Polizas | Detalle de poliza alinea badge superior con broker activo, broker requerido y permiso ausente. |
| `856913a` | Frontend Polizas | Resumen visible de filtros activos sincronizado con URL, catalogos y retorno desde detalle. |
| `0fc94fb` | Frontend auth | Login enfoca usuario, separa aviso de sesion de error y expone estado accesible durante validacion. |
| `0fe67fd` | Frontend menu | Leyenda compacta del menu muestra rotulo visible `Estado` y conserva descripcion accesible. |
| `c9f8eda` | QA gate | Gate local completo ejecutado tras los incrementos autonomos de login/menu/Polizas. |
| `54467f0` | Frontend Polizas | Accesos superiores a Flotas/Colectivas enlazan a scopes estaticos bloqueados por SDD; Externas sigue deshabilitado. |
| `125c5f6` | Frontend shell | Acciones placeholder de Notificaciones/Configuracion quedan deshabilitadas y etiquetadas como pendientes de SDD. |
| `5fbb3c2` | Frontend shell | Selector de broker enlaza `aria-invalid` con descripcion accesible del error. |
| `1259602` | Frontend Polizas | Acciones heredadas de filtros, campos fuera de contrato y busqueda bloqueada explican motivo accesible sin activar nuevas APIs. |
| `2f507e4` | Frontend Polizas | Toolbar superior de Polizas mantiene acciones bloqueadas y describe motivo MVP read-only por SDD/API/permisos/UAT. |
| `1f5f090` | Frontend Polizas | Accesos superiores a scopes Flotas/Colectivas/Externas describen que no cargan datos ni activan permisos sin SDD/API/UAT. |

## Validaciones ejecutadas

Checkpoint completo tras los cambios de backend/QA:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -RunFrontendE2E -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe"
```

Resultado:

- Backend restore/build OK.
- Backend tests OK: `95` tests.
- Frontend `npm ci`, format, lint, unit tests y build OK.
- Frontend unit tests OK: `186` tests en el checkpoint; despues del feedback de login quedan `187`.
- Playwright E2E OK: `7` smokes.
- Backend HTTP smoke OK, incluyendo API key local y `DemoSession`.
- Gitleaks OK: `no leaks found`.
- Dependency audit OK: `0` findings.
- CORS audit OK.
- Extractor Polizas metadata tests OK.
- Documentation baseline OK.
- `git diff --check` OK.

Validaciones adicionales del ultimo incremento frontend:

- `npx vitest run src/router/index.test.ts src/features/auth/LoginView.test.ts`: `12` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `187` tests OK.
- `npm run build`: OK.
- `npm run test:e2e`: `7` smokes OK.
- `Test-DocumentationBaseline.ps1`: OK.
- `Invoke-SecretScan.ps1 -NoReport`: OK.

Validaciones adicionales del checkpoint `d486a30`:

- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter "FullyQualifiedName~PolizasApiTests"`: `62/62` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run typecheck`: OK.
- `npm run test:e2e`: `8` smokes OK con Node `v24.14.0` del runtime Codex.
- `Test-DocumentationBaseline.ps1`: OK.
- `Invoke-SecretScan.ps1 -NoReport`: OK.
- `git diff --check`: OK.

Validaciones adicionales tras matriz de permisos y accesibilidad shell:

- `Test-DocumentationBaseline.ps1`: OK.
- `Invoke-SecretScan.ps1 -NoReport`: OK.
- `npx vitest run src/layout/AppShell.test.ts`: `3/3` tests OK.
- `npm run test:e2e -- responsive-shell`: `1` smoke OK.
- `npm run test:unit`: `187/187` tests OK.
- `npm run test:e2e`: `8` smokes OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run typecheck`: OK.
- `npm run build`: OK.

Validaciones adicionales tras descripcion accesible del menu:

- `npx vitest run src/layout/AppSideMenu.test.ts`: `4/4` tests OK.
- `npm run test:e2e -- responsive-shell menu-navigation menu-route-parity`: `3` smokes OK con `CI=1` para reconstruir preview.
- `npm run test:e2e`: `8` smokes OK con `CI=1`.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `187/187` tests OK.
- `npm run build`: OK.
- `Test-DocumentationBaseline.ps1`: OK.
- `Invoke-SecretScan.ps1 -NoReport`: OK.
- `git diff --check`: OK.

Validaciones adicionales tras foco real del skip link:

- `npx vitest run src/layout/AppShell.test.ts`: `4/4` tests OK.
- `npm run test:e2e -- responsive-shell`: `1` smoke OK con `CI=1`.

Checkpoint completo tras los incrementos autonomos de QA, auth docs y accesibilidad:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -RunFrontendE2E -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe"
```

Resultado:

- Backend restore/build OK.
- Backend tests OK: `98` tests.
- Frontend `npm ci`, format, lint, unit tests y build OK.
- Frontend unit tests OK: `188` tests.
- Playwright E2E OK: `8` smokes.
- Backend HTTP smoke OK.
- Frontend smoke OK.
- Gitleaks OK: `no leaks found`.
- Dependency audit OK: `0` findings.
- CORS audit OK.
- Extractor Polizas metadata tests OK.
- Documentation baseline OK.
- `git diff --check` OK.

Validaciones adicionales tras aviso de permiso de detalle en Polizas:

- `npx vitest run src/features/polizas/PolizasTable.test.ts src/features/polizas/PolizasView.test.ts`: `14/14` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `188/188` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras paridad de contexto en detalle de Polizas:

- `npx vitest run src/features/polizas/PolizaDetailView.test.ts`: `10/10` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras resumen de filtros activos en Polizas:

- `npx vitest run src/features/polizas/PolizasView.test.ts`: `8/8` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras pulido accesible de login:

- `npx vitest run src/features/auth/LoginView.test.ts`: `3/3` tests OK.
- `npm run test:e2e -- auth-guard`: `1` smoke OK con `CI=1`.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras rotulo de leyenda del menu:

- `npx vitest run src/layout/AppSideMenu.test.ts`: `4/4` tests OK.
- `npm run test:e2e -- menu-navigation`: `1` smoke OK con `CI=1`.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Gate completo tras incrementos autonomos:

- Primer intento: backend restore/build/tests OK (`98/98`), bloqueado en `npm ci` por binario nativo de Rollup retenido por Vite local en `127.0.0.1:5174`.
- Accion: se detuvo el proceso Vite de `5174` y se relanzo el gate.
- `.\tools\quality\Invoke-MvpQualityGate.ps1 -RunFrontendE2E -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe"`: OK.
- Backend restore/build: OK.
- Backend tests: `98/98` OK.
- Frontend `npm ci`, `npm run format`, `npm run lint`, `npm run test:unit:ci`, `npm run build`: OK.
- Frontend unit tests: `189/189` OK.
- Frontend E2E: `8/8` smokes OK.
- Backend HTTP smoke y frontend smoke: OK.
- Secret scan: sin leaks.
- Dependency audit: `0` findings.
- CORS audit: sin findings.
- Polizas metadata extractor tests: OK.
- Documentation baseline: OK.
- `git diff --check`: OK.

Validacion adicional tras accesos superiores a scopes de Polizas:

- `npx vitest run src/features/polizas/PolizasView.test.ts`: `8/8` tests OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras placeholder actions del shell:

- `npx vitest run src/layout/AppShell.test.ts`: `5/5` tests OK.
- `npm run test:e2e -- responsive-shell`: `1` smoke OK con `CI=1`.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras descripcion accesible del error de broker:

- `npx vitest run src/layout/AppShell.test.ts`: `6/6` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras descripcion accesible de filtros bloqueados en Polizas:

- `npx vitest run src/features/polizas/PolizasFilters.test.ts`: `6/6` tests OK.
- `npm run format`: OK tras aplicar Prettier al alcance tocado.
- `npm run lint`: OK.
- `npm run test:unit`: `192/192` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras descripcion accesible de acciones superiores de Polizas:

- `npx vitest run src/features/polizas/PolizasView.test.ts`: `8/8` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `192/192` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion adicional tras descripcion accesible de scopes superiores de Polizas:

- `npx vitest run src/features/polizas/PolizasView.test.ts`: `8/8` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `192/192` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- polizas`: `1` smoke OK con `CI=1`.

## Estado local visible

El servidor frontend de desarrollo queda levantado en:

```text
http://127.0.0.1:5174/
```

Nota operativa: el gate local ejecuta `npm ci`, por lo que conviene parar cualquier Vite/esbuild activo del workspace antes de correrlo para evitar bloqueo de `node_modules/@esbuild/.../esbuild.exe` en Windows.

## Riesgos residuales

- `DemoSession`, API key MVP y headers MVP siguen siendo compatibilidad local/demo, no autenticacion productiva.
- Datos reales SQL, DBA/UAT, permisos finales y proveedor auth siguen bloqueados externamente.
- Las paginas de menu distintas de Polizas siguen siendo fixture/read-only o superficies bloqueadas; no autorizan API, PII, escrituras ni exportaciones.
- `Autos Particulares` sigue aparcado por decision de producto.
