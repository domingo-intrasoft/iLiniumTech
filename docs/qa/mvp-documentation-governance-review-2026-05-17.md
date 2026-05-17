# Revision QA/Docs/Gobierno - Estado vivo MVP

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`
Rol: Jefe QA/Docs/Gobierno

## Objetivo

Revisar si la documentacion viva y las evidencias reflejan el estado real del MVP tras paginas estaticas, login, broker activo y Polizas. Esta revision no modifica codigo de aplicacion, `tools` ni workflows.

## Fuentes revisadas

- `AGENTS.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/workflows/parallel-codex-task-pack.md`
- `docs/qa/definition-of-done.md`
- `docs/qa/login-session-hardening-checklist.md`
- `docs/qa/active-broker-change-checklist.md`
- `docs/qa/polizas-mvp-readonly-evidence.md`
- `docs/appbuilder/pages/development-readiness.md`
- `docs/appbuilder/menu-polizas-mvp-implementation.md`
- `iLiniumTech.Frontend/src/router/index.ts`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- Busquedas con `rg` sobre rutas frontend, endpoints backend y evidencias QA.

## Estado real observado

### Login y sesion

Completado con evidencia MVP:

- `/login` existe como ruta publica fuera del shell protegido.
- Las rutas protegidas usan guard de sesion.
- En modo backend `demo-session`, la UI valida `/api/me`.
- Logout, 401, 403 y limpieza de sesion estan documentados en `login-session-hardening-checklist.md`.
- La evidencia mas reciente registra tests backend, frontend, auditorias y smoke visual de `/login -> /polizas -> detalle -> logout`.

Pendiente tecnico/bloqueado externo:

- Auth productiva real.
- Matriz funcional de permisos por broker, perfil, oficina, gestor y usuario.
- Validacion DBA/UAT de restricciones reales.

### Broker activo

Completado con evidencia MVP:

- Existe `POST /api/auth/broker` en backend.
- El frontend expone selector solo con brokers permitidos y valida contra `allowedBrokerIds`.
- Tras cambio de broker se relee `/api/me` y se refresca/invalida Polizas.
- La evidencia esta en `active-broker-change-checklist.md`.

Pendiente tecnico/bloqueado externo:

- Sustituir `DemoSession` por auth productiva antes de entornos con datos reales.
- Conectar `SESSION_CONTEXT` a identidad autenticada final y validarla con DBA.

### Polizas

Completado con evidencia MVP:

- `/polizas` y `/polizas/:id` existen como Vue/TypeScript explicito.
- Backend expone `/api/polizas/catalogs`, `/api/polizas`, `/api/polizas/{id}` y `/api/polizas/metadata` informativo que no entrega metadata runtime de pantalla.
- Filtros, paginacion, detalle, permisos `polizas.*`, broker requerido y errores seguros estan cubiertos por evidencias QA.
- `polizas-mvp-readonly-evidence.md` documenta gate local con backend, frontend, E2E, auditorias, extractor y baseline documental.

Pendiente tecnico/bloqueado externo:

- Datos reales SQL/UAT.
- Auth real y permisos productivos.
- Confirmacion de campos sensibles y reglas de `SESSION_CONTEXT`.

### Paginas estaticas del menu

Completado con evidencia MVP estatica:

- El router contiene rutas protegidas para `Agenda`, `Clientes`, `Propuestas`, `Polizas`, `Autos Particulares`, `Polizas / Flotas`, `Polizas / Colectivas`, `Recibos`, `Suplementos`, `Siniestros`, `Liq.Cia`, `Liq.Col`, `Informes`, `Controles`, `Estadisticas`, `Administracion`, `Configuracion`, `Conectividad`, `By Aunna` y `Logs`.
- El menu se declara en `appNavigation.ts` como codigo fuente, no como metadata AppBuilder.
- `Autos Particulares` permanece deshabilitado en la navegacion y aparcado como objetivo.
- El resto de paginas estaticas estan documentadas como MVP visible, read-only o superficies bloqueadas en `docs/appbuilder/pages/development-readiness.md`.
- Hay evidencias QA por grupos de paginas en `docs/qa/*-mvp-evidence.md`.

Pendiente tecnico/bloqueado externo:

- Ninguna pagina estatica distinta de Polizas queda autorizada para datos reales, API, filtros funcionales, exportaciones, escrituras, permisos finos o PII sin SDD/API/UAT.
- Varias evidencias anteriores registran Node global fuera del engine requerido; para cierre formal usar Node `>=20.19.0` o `Invoke-MvpQualityGate.ps1 -NodeExe`.

## Desajustes corregidos

- `docs/appbuilder/menu-polizas-mvp-implementation.md` seguia diciendo que solo `Polizas` y `Autos Particulares` estaban disponibles desde el menu. Se actualizo para reflejar las paginas estaticas protegidas actuales y el estado aparcado/deshabilitado de `Autos Particulares`.
- `docs/PLAN_MAESTRO_IA.md`, `docs/ROADMAP_OBJETIVO_FINAL.md` y `AGENTS.md` no dejaban suficientemente explicito que las paginas estaticas del menu ya existen pero no autorizan datos reales ni APIs nuevas. Se ajusto esa frontera.
- `docs/qa/definition-of-done.md` no tenia criterios especificos para paginas estaticas del menu. Se anadio una seccion operativa.

## Comandos ejecutados en esta revision

```powershell
git status --short --branch
```

Resultado: rama `codex/static-polizas-data-api...origin/codex/static-polizas-data-api`; working tree limpio al inicio.

```powershell
rg --files iLiniumTech.Frontend\src
rg "path:|createRouter|appNavigation|polizas|broker|login|auth" iLiniumTech.Frontend\src -n
rg "Map(Get|Post)|api/auth|api/me|api/polizas|broker|polizas\." iLiniumTech.Backend -n
```

Resultado: inventario de rutas frontend, navegacion estatica y endpoints backend confirmado para la revision documental.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: OK. Required docs, SDD headings, CI files and roadmap links are present.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
```

Resultado: OK. `no leaks found`.

```powershell
git diff --check
```

Resultado: OK sin errores de whitespace. Git mostro avisos de normalizacion LF/CRLF en archivos del working copy.

## Cambios concurrentes observados al cierre

Al cierre de esta revision aparecieron cambios fuera del alcance documental en archivos de aplicacion:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/PolizasApiTests.cs`
- `iLiniumTech.Frontend/src/assets/styles/main.scss`
- `iLiniumTech.Frontend/src/layout/AppShell.test.ts`
- `iLiniumTech.Frontend/src/layout/AppShell.vue`

No se revirtieron ni se tocaron desde esta pasada QA/Docs. Para preparar stage/commit, separar los cambios documentales de esta revision de cualquier cambio de aplicacion concurrente.

## Criterio de cierre documental

Esta revision deja la documentacion viva alineada con el estado MVP observado:

- login y sesion demo robusta;
- broker activo MVP validado por backend;
- Polizas read-only como unica pantalla funcional inicial con API;
- resto del menu como paginas estaticas protegidas o superficies bloqueadas;
- Autos Particulares aparcado;
- ausencia de dependencia runtime de metadata AppBuilder.

No se considera cierre productivo porque auth real, DBA/UAT, permisos productivos, datos reales y preview siguen pendientes o bloqueados externamente.
