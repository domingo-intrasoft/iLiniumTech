# Plan de ejecucion continua IA

Fecha base: 2026-05-18

Estado: plan operativo canonico para automatizaciones y agentes que continen el MVP iLiniumTech sin perder tiempo reanalizando todo el repositorio.

## Instruccion corta para cualquier IA

1. Lee `AGENTS.md` y este archivo.
2. Revisa `git status --short --branch`.
3. Ejecuta solo la tarea marcada en **Siguiente tarea activa**.
4. No hagas analisis amplio salvo que la tarea este bloqueada o los archivos indicados no existan.
5. Respeta estrictamente los archivos permitidos y prohibidos de la tarea.
6. Ejecuta las pruebas indicadas en la tarea.
7. Actualiza la evidencia y, si cierras la tarea, mueve esta seccion a la siguiente tarea `READY`.
8. Haz commit y push si el cambio queda coherente, pequeno y validado.

## Estado actual resumido

- Producto: iLiniumTech es Vue estatico + API .NET explicita. No es runtime dinamico AppBuilder.
- Rama activa: `codex/polizas-crud-bbdd`.
- `Polizas CRUD BBDD` queda como vertical MVP local de referencia, con smoke API/UI visible contra BBDD local de pruebas.
- `/login`, shell, menu lateral y rutas protegidas del menu existen.
- `Autos Particulares` queda aparcado; no ampliarlo.
- Paridad visual de `/polizas` con AppBuilder publicado refinada en segundo corte: chrome demo, menu lateral compacto, buscador iconificado, grid plano y menor ruido visual.
- Paridad de datos del grid de `Polizas` cerrada como MVP local/demo: `N. Documento`, cliente descriptivo, `Ramo` y `Riesgo/Matric.` salen del contrato API real y la logica AppBuilder queda documentada para el resto de paginas.
- Nueva prioridad humana 2026-05-19: pasar paginas del menu a verticales CRUD reales contra BBDD local. La fuente operativa queda en `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`.
- Nueva decision humana 2026-05-19: trabajar con datos reales locales para todas las pantallas funcionales a partir de ahora. Ver `docs/DECISION_DATOS_REALES_LOCALES.md`.
- `Agenda CRUD BBDD` queda como primera vertical fuera de Polizas: API + frontend CRUD MVP, permisos `agenda.create/update/delete`, `Agenda:WritesEnabled`, SQL parametrizado, filtro broker y SDD propia `SDD-2026-015`. Pendiente smoke SQL local con secretos fuera de Git.
- Paginas del menu distintas de Polizas y Agenda conservan base fixture/in-memory transitoria, pero la siguiente evolucion debe ir a BBDD real local minimizada.
- `Recibos`, `Clientes`, `Agenda`, `Propuestas` y `Suplementos` ya tienen contrato frontend fixture separado en tipos/fixture/composable.
- `Siniestros` ya tiene primer backend read-only explicito in-memory: `GET /api/siniestros/catalogs` y `GET /api/siniestros`, con permisos propios y sin SQL real.
- `Recibos` ya tiene primer backend read-only explicito in-memory: `GET /api/recibos/catalogs` y `GET /api/recibos`, con permisos propios, sin importes reales ni banco.
- `Clientes`, `Agenda`, `Propuestas` y `Suplementos` ya tienen backend read-only explicito in-memory con permisos propios, contratos minimizados y tests dirigidos.
- Ronda documental por pagina completada en `docs/appbuilder/pages/page-agent-coordination-2026-05-18.md`.
- SDD drafts existentes para `Siniestros`, `Recibos`, `Clientes`, `Agenda`, `Propuestas` y `Suplementos`.
- No versionar ni exponer secretos/datos reales. Activar lectura real local por vertical; activar escrituras solo con SDD, permisos, flags, transacciones, auditoria y smoke de limpieza.

## Siguiente tarea activa

ID: `T-308-RECIBOS-FE-API-ADAPTER-VERIFY`

Estado: `READY`

Nombre: Verificar/adaptar frontend Recibos a API con fallback fixture.

Objetivo:

- Confirmar que `/recibos` consume API cuando `VITE_USE_BACKEND=true`.
- Mantener fixture solo cuando `VITE_USE_BACKEND=false`, tests/offline o backend no configurado explicitamente.
- No tocar backend, SQL, permisos, menu, layout ni otras pantallas.
- Mantener detalle, exportacion, escritura, banco, importes no aprobados, liquidaciones y datos sensibles bloqueados.
- Si ya esta implementado, documentar la evidencia y cerrar la tarea sin cambios runtime.

Fuentes a leer, sin buscar mas salvo bloqueo real:

- `AGENTS.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/DECISION_DATOS_REALES_LOCALES.md`
- `docs/sdd/specs/iLiniumTech/*recibos*`
- `docs/qa/recibos-mvp-evidence.md`
- `docs/qa/recibos-sql-readonly-local-evidence.md`
- `iLiniumTech.Frontend/src/features/recibos/**`

Archivos que puede tocar:

- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/qa/*recibos*`
- `iLiniumTech.Frontend/src/features/recibos/**`
- `iLiniumTech.Frontend/src/services/**`

Archivos que NO debe tocar:

- cualquier `.env*`, config con secretos, dumps o capturas sensibles;
- backend, SQL real, repositorios, migraciones, extractores o scripts de BBDD;
- router, layout, menu, auth o pantallas no relacionadas;
- detalle, exportacion, escrituras, banco, importes no aprobados, liquidaciones, workflow, documentos, textos libres o PII real;
- cambios de contrato backend sin SDD.

Pasos de implementacion:

1. Revisar `git status --short --branch`.
2. Leer solo las fuentes indicadas.
3. Revisar si el composable/servicio de `Recibos` usa API con `VITE_USE_BACKEND=true`.
4. Si falta, implementar adaptador frontend explicito con fallback fixture solo cuando `VITE_USE_BACKEND=false`.
5. Actualizar tests y evidencia QA.
6. Actualizar planes y mover cursor si queda cerrado.

Pruebas obligatorias:

```powershell
cd .\iLiniumTech.Frontend
npm run test:unit -- Recibos
npm run lint
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Criterios de aceptacion:

- `/recibos` usa API en modo backend y fixture solo en modo offline/test explicito.
- No se introducen datos sensibles, detalle/exportacion/escrituras, banco, importes no aprobados ni SQL.
- Tests frontend dirigidos y build quedan OK.
- Evidencia QA explica modo API/fallback y riesgos residuales.

Riesgo de conflicto: medio; tocar solo `Recibos` frontend y servicios necesarios.

## Cola de tareas pequenas

### Fase 0 - Mantener gobierno y cursor de trabajo

| ID | Estado | Tarea | Archivos principales | Validacion minima |
| --- | --- | --- | --- | --- |
| `T-000-PLAN-CONTINUO` | `DONE` | Crear este plan y automatizacion de continuidad. | `docs/PLAN_EJECUCION_CONTINUA_IA.md` | baseline docs, secret scan, diff check |
| `T-000B-CURSOR-HYGIENE` | `TODO` | Si una automatizacion encuentra una tarea cerrada, mover el cursor a la siguiente `READY`. | este archivo | baseline docs |

### Fase 1 - Endurecer paginas fixture sin backend

Objetivo: que cada pagina fixture tenga estructura mantenible y lista para API futura, sin activar datos reales.

| ID | Estado | Tarea | Archivos permitidos | No tocar | Pruebas |
| --- | --- | --- | --- | --- | --- |
| `T-001-SIN-FE-CONTRACT-FIXTURE` | `DONE` | Separar contrato fixture de Siniestros. | `src/features/siniestros/**`, `docs/qa/siniestros-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-002-REC-FE-CONTRACT-FIXTURE` | `SUPERSEDED` | Sustituida por `T-100-RECIBOS-FE-CONTRACT-FIXTURE` dentro del plan CRUD resto de paginas. | `src/features/recibos/**`, `docs/qa/recibos-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-003-CLI-FE-CONTRACT-FIXTURE` | `SUPERSEDED` | Sustituida por `T-101-CLIENTES-FE-CONTRACT-FIXTURE`. | `src/features/clientes/**`, `docs/qa/clientes-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-004-AGE-FE-CONTRACT-FIXTURE` | `SUPERSEDED` | Sustituida por `T-102-AGENDA-FE-CONTRACT-FIXTURE`. | `src/features/agenda/**`, `docs/qa/agenda-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-005-PRO-FE-CONTRACT-FIXTURE` | `SUPERSEDED` | Sustituida por `T-103-PROPUESTAS-FE-CONTRACT-FIXTURE`. | `src/features/propuestas/**`, `docs/qa/propuestas-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-006-SUP-FE-CONTRACT-FIXTURE` | `SUPERSEDED` | Sustituida por `T-104-SUPLEMENTOS-FE-CONTRACT-FIXTURE`. | `src/features/suplementos/**`, `docs/qa/suplementos-mvp-evidence.md` | backend, services, router, layout | frontend completo |

### Fase 2 - Preparar contratos read-only API sin datos reales

Objetivo: disenar contratos y pruebas de dominio antes de SQL real. No conectar BBDD hasta que UAT/DBA desbloquee origen y minimizacion.

| ID | Estado | Tarea | Archivos permitidos | Bloqueo |
| --- | --- | --- | --- | --- |
| `T-010-SIN-BE-CONTRACT-DESIGN` | `SUPERSEDED` | Sustituida por `T-110-SINIESTROS-BE-READONLY-CONTRACT`. | backend Siniestros nuevo, tests backend, docs QA | no SQL real |
| `T-011-SIN-FE-API-ADAPTER-BLOCKED` | `DONE` | Preparar adaptador frontend Siniestros con fallback fixture solo cuando `VITE_USE_BACKEND=false`. | `src/features/siniestros/**` | `T-110`/`T-304` cerradas |
| `T-308-RECIBOS-FE-API-ADAPTER-VERIFY` | `READY_ACTIVE` | Verificar/adaptar frontend Recibos con fallback fixture solo cuando `VITE_USE_BACKEND=false`. | `src/features/recibos/**` | `T-120`/`T-305` cerradas |
| `T-012-REC-BE-CONTRACT-DESIGN` | `SUPERSEDED` | Sustituida por `T-120-RECIBOS-BE-READONLY-CONTRACT`. | backend Recibos nuevo, tests backend | no SQL real |
| `T-013-CLI-BE-CONTRACT-DESIGN` | `SUPERSEDED` | Sustituida por `T-121-CLIENTES-BE-READONLY-CONTRACT`. | backend Clientes nuevo, tests backend | no SQL real |

### Fase 1B - Preparar CRUD resto de paginas sin escrituras prematuras

Objetivo: convertir la peticion de CRUD al resto de paginas en verticales explicitas, pequenas y seguras. Primero se separan contratos frontend y despues se abren APIs read-only; las escrituras se desbloquean pagina a pagina con SDD/UAT/DBA/security review.

| ID | Estado | Tarea | Archivos permitidos | No tocar | Pruebas |
| --- | --- | --- | --- | --- | --- |
| `T-100-RECIBOS-FE-CONTRACT-FIXTURE` | `DONE` | Preparar Recibos para API/CRUD futura separando tipos, fixture y composable, sin importes reales ni banco. | `src/features/recibos/**`, `docs/qa/recibos-mvp-evidence.md`, planes | backend, services, router, layout | frontend completo |
| `T-101-CLIENTES-FE-CONTRACT-FIXTURE` | `DONE` | Preparar Clientes para API futura separando tipos/fixture/composable con PII bloqueada. | `src/features/clientes/**`, `docs/qa/clientes-mvp-evidence.md`, planes | backend, services, router, layout | frontend completo |
| `T-102-AGENDA-FE-CONTRACT-FIXTURE` | `DONE` | Preparar Agenda para API futura por rango sin calendario mutante. | `src/features/agenda/**`, `docs/qa/agenda-mvp-evidence.md`, planes | backend, services, router, layout | frontend completo |
| `T-103-PROPUESTAS-FE-CONTRACT-FIXTURE` | `DONE` | Preparar Propuestas sin asumir Solicitudes ni activar emision/conversion. | `src/features/propuestas/**`, `docs/qa/propuestas-mvp-evidence.md`, planes | backend, services, router, layout | frontend completo |
| `T-104-SUPLEMENTOS-FE-CONTRACT-FIXTURE` | `DONE` | Preparar Suplementos sin workflows, adjuntos, banco ni escrituras. | `src/features/suplementos/**`, `docs/qa/suplementos-mvp-evidence.md`, planes | backend, services, router, layout | frontend completo |
| `T-110-SINIESTROS-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Siniestros con permisos propios y sin SQL real. | backend Siniestros, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-120-RECIBOS-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Recibos sin importes reales ni banco. | backend Recibos, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-121-CLIENTES-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Clientes con PII bloqueada. | backend Clientes, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-122-AGENDA-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Agenda sin calendario mutante ni PII. | backend Agenda, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-123-PROPUESTAS-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Propuestas sin solicitante real, importes, documentos ni conversion. | backend Propuestas, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-124-SUPLEMENTOS-BE-READONLY-CONTRACT` | `DONE` | Crear backend read-only in-memory para Suplementos sin workflows, banco, adjuntos, importes ni escrituras. | backend Suplementos, API, tests, docs QA | SQL real, frontend services | backend dirigido |
| `T-130-LIQCIA-SDD-READONLY` | `DONE` | Preparar SDD/readiness de Liq.Cia antes de API por riesgo financiero. | docs SDD, appbuilder pages, QA, planes | backend, frontend, SQL real | docs |

### Fase 3 - Datos reales read-only por pagina

Objetivo: activar lectura SQL minimizada solo cuando haya SDD, UAT/DBA, permisos y origen autorizado.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-300-REALDATA-LOCAL-BASELINE` | `DONE` | Preparar modo operativo de datos reales locales para todas las pantallas. | decision humana 2026-05-19 |
| `T-301-CLIENTES-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Implementar SQL local real minimizado para Clientes. | BBDD local disponible, no secretos en Git |
| `T-302-CLIENTES-CRUD-SDD` | `DONE` | Definir SDD de escritura para `Identidad` + `IdentidadCliente` antes de CRUD real. | documentacion, sin codigo app |
| `T-303-CLIENTES-CRUD-BBDD-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Implementar CRUD real local de Clientes segun `SDD-2026-016`; update/delete solo MVP-owned. | SDD-2026-016 |
| `T-304-SINIESTROS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Implementar SQL local real minimizado para Siniestros. | BBDD local disponible, minimizacion |
| `T-305-RECIBOS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Implementar SQL local real minimizado para Recibos. | BBDD local disponible, cuidado financiero |
| `T-306-SUPLEMENTOS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Implementar SQL local real minimizado para Suplementos. | BBDD local disponible, workflows bloqueados |
| `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL` | `BLOCKED_ORIGIN_UNCONFIRMED` | Confirmar origen real local de Propuestas y crear SQL read-only. | no asumir `Solicitudes` sin evidencia |

### Fase 4 - Auth, permisos y multi-tenant productivo

Objetivo: dejar de depender de compatibilidad MVP antes de produccion.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-030-AUTH-PROVIDER-ADR` | `BLOCKED_HUMAN` | Decidir proveedor auth y documentar ADR. | decision humana |
| `T-031-PERMISSIONS-MATRIX` | `BLOCKED_HUMAN` | Definir matriz real de permisos por broker/perfil/usuario. | producto/seguridad |
| `T-032-MVP-HEADERS-RETIREMENT` | `TODO` | Preparar plan tecnico para retirar headers MVP fuera de local/demo. | `T-030`, `T-031` |

### Fase 5 - Pólizas profesional y UAT

Objetivo: endurecer lo ya conseguido en Polizas sin ampliar reglas no aprobadas.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-040-POL-UAT-FIELDS` | `BLOCKED_HUMAN` | Confirmar campos editables definitivos y semantica de baja. | UAT/DBA |
| `T-041-POL-AUDIT-WRITES` | `DONE` | Disenar auditoria de escritura sin datos sensibles. | puede ser documental |
| `T-042-POL-CRUD-REGRESSION-SMOKE` | `DONE` | Mantener smoke local API/UI de CRUD real sin secretos. | BBDD local disponible |
| `T-043-POL-APPBUILDER-VISUAL-SHELL` | `DONE` | Aproximar `/polizas` al shell/grid visual de AppBuilder publicado sin simular datos. | captura usuario y SDD-2026-014 |
| `T-044-POL-APPBUILDER-VISUAL-SMOKE` | `DONE` | Ejecutar smoke visual y documentar diferencias pendientes frente a AppBuilder. | `T-043` |
| `T-045-POL-APPBUILDER-VISUAL-PARITY-REFINE` | `DONE` | Segundo corte de paridad visual: barra demo, chrome AppBuilder, menu compacto, buscador icon-only, grid sin warnings visibles y estados reales normalizados. | captura usuario y feedback visual 2026-05-18 |
| `T-046-POL-APPBUILDER-ASSETS-UAT` | `BLOCKED_HUMAN` | Validar logos/columnas sensibles/campos reales necesarios para pixel parity. | UAT/producto/DBA, origen autorizado de logos y datos |
| `T-047-POL-APPBUILDER-DATA-PARITY` | `DONE` | Cerrar paridad de datos del grid: documento, cliente descriptivo, ramo lookup y riesgo/matricula. | feedback usuario 2026-05-18, SDD-2026-014 |

### Fase 6 - Reporting, liquidaciones y superficies tecnicas

Objetivo: no abrir superficies de alto riesgo sin threat model.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-050-LIQCIA-SDD-READONLY` | `SUPERSEDED_BY_T-130` | Preparar SDD Liq.Cia read-only financiera sin importes reales. | readiness ya existe |
| `T-051-LIQCOL-SDD-READONLY` | `DONE` | Preparar SDD Liq.Col read-only financiera sin comisiones reales. | readiness ya existe |
| `T-052-LOGS-THREAT-MODEL` | `DONE` | Crear threat model para Logs antes de cualquier API. | readiness ya existe |
| `T-053-CONNECTIVITY-THREAT-MODEL` | `DONE` | Crear threat model para Conectividad/secretos/SSRF. | readiness ya existe |

## Regla para mover la siguiente tarea

Cuando una tarea se cierre:

1. Cambiar su estado a `DONE`.
2. Anadir una linea de evidencia con commit, comandos y resultado.
3. Copiar la siguiente tarea `READY` a la seccion **Siguiente tarea activa**.
4. Si no hay `READY`, promover la primera `TODO` no bloqueada a `READY`.
5. Si la siguiente tarea requiere decision humana, dejarla como `BLOCKED_HUMAN` y elegir otra tarea tecnica/documental segura.

## Comandos de validacion por tipo

Documentacion:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Backend:

```powershell
dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Gate completo:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

## Registro de evidencias del plan

- 2026-05-18: creado el plan continuo y seleccionada `T-001-SIN-FE-CONTRACT-FIXTURE` como siguiente tarea segura.
- 2026-05-18: `T-001-SIN-FE-CONTRACT-FIXTURE` cerrada. Se separaron tipos, fixture y composable de `Siniestros`; validacion OK con test dirigido, format, lint, unit, build, baseline documental, secret scan y `git diff --check`. El cursor queda en `T-002-REC-FE-CONTRACT-FIXTURE`.
- 2026-05-18: el usuario redefine el siguiente MVP: `/polizas` debe parecerse a la pantalla AppBuilder publicada, pero manteniendo CRUD real y sin atajos/simulaciones. `T-002` queda pospuesta y el cursor pasa a `T-043-POL-APPBUILDER-VISUAL-SHELL`.
- 2026-05-18: `T-043-POL-APPBUILDER-VISUAL-SHELL` cerrada. Se compactaron toolbar/buscador/tabla de `Polizas`, se agregaron columnas objetivo con valores neutros para datos no entregados por API, badge `En Vigor`, tests visuales de tabla y evidencia en `docs/qa/polizas-visual-parity-evidence.md`. Validacion OK con targeted tests `23/23`, format, lint, unit completo `198/198`, build, baseline documental, secret scan y `git diff --check`. El cursor queda en `T-044-POL-APPBUILDER-VISUAL-SMOKE`.
- 2026-05-18: `T-044-POL-APPBUILDER-VISUAL-SMOKE` cerrada. Se ejecuto smoke en navegador local y smoke controlado de escritorio con Vite fixture temporal: toolbar, buscador, grid, columnas, scopes y badge `En Vigor` OK. Evidencia actualizada en `docs/qa/polizas-visual-parity-evidence.md`. El cursor queda en `T-041-POL-AUDIT-WRITES`.
- 2026-05-18: `T-041-POL-AUDIT-WRITES` cerrada. Se creo `docs/engineering/polizas-write-audit-design.md` y se enlazo desde la evidencia CRUD. El diseno define eventos candidatos, campos permitidos, campos prohibidos, minimizacion, retencion y preguntas UAT/DBA sin implementar runtime ni guardar PII. El cursor queda en `T-042-POL-CRUD-REGRESSION-SMOKE`.
- 2026-05-18: `T-042-POL-CRUD-REGRESSION-SMOKE` cerrada. Se reejecuto el smoke temporal API/UI CRUD real contra BBDD local de pruebas con resultados sanitizados: create 201, detail 200, search 1, update 204, delete 204, detail post-delete 404, limpieza exacta API/UI y residuales `0`. Evidencia actualizada en `docs/qa/polizas-crud-bbdd-evidence.md`. El cursor queda en `T-002-REC-FE-CONTRACT-FIXTURE`.
- 2026-05-18: `T-047-POL-APPBUILDER-DATA-PARITY` cerrada. Se documento la logica AppBuilder de campos directos/lookups, se cambio lectura enriquecida de Polizas a `vw_ClientePolizas`, se incorporaron `documento` y `riesgo` al listado, `clienteNombre` ya no cae al codigo cliente, `Autos Particulares` sanitiza campos sensibles, smoke API/UI real OK contra BBDD local y validacion completa OK. El cursor vuelve a `T-002-REC-FE-CONTRACT-FIXTURE`.
- 2026-05-18: el usuario cambia prioridad a "CRUD al resto de paginas". Se crea `docs/PLAN_CRUD_RESTO_PAGINAS.md` y se sustituye `T-002` por `T-100-RECIBOS-FE-CONTRACT-FIXTURE` como primer paso seguro. Escrituras reales quedan bloqueadas por pagina hasta SDD/UAT/DBA/permisos/transacciones/smoke.
- 2026-05-18: `T-100`, `T-101`, `T-102`, `T-103` y `T-104` cerradas por agentes frontend con write scopes disjuntos. Recibos, Clientes, Agenda, Propuestas y Suplementos quedan separados en tipos, fixtures y composables locales; tests dirigidos OK. Se creo tambien `T-110-SINIESTROS-BE-READONLY-CONTRACT` y queda cerrado con API in-memory minimizada y tests `SiniestrosApiTests` OK. Cursor pasa a `T-120-RECIBOS-BE-READONLY-CONTRACT`.
- 2026-05-18: `T-120-RECIBOS-BE-READONLY-CONTRACT` cerrada. Se creo API in-memory read-only de Recibos con permisos `recibos.catalogs` y `recibos.read`, sin importes reales ni banco; tests `RecibosApiTests` OK. Cursor pasa a `T-121-CLIENTES-BE-READONLY-CONTRACT`.
- 2026-05-18: `T-121`, `T-122`, `T-123` y `T-124` cerradas. Se crearon APIs in-memory read-only de Clientes, Agenda, Propuestas y Suplementos con permisos propios y contratos minimizados; tests dirigidos `Clientes|Agenda|Propuestas|Suplementos` OK, 21 tests. Cursor pasa a `T-130-LIQCIA-SDD-READONLY`.
- 2026-05-19: el usuario decide trabajar con datos reales locales para todas las pantallas. Se crea `docs/DECISION_DATOS_REALES_LOCALES.md`, se refuerzan `AGENTS.md`, `PLAN_CRUD_REAL_BBDD_LOCAL.md`, `PLAN_CRUD_RESTO_PAGINAS.md` y este plan. El cursor pasa a `T-300-REALDATA-LOCAL-BASELINE`.
- 2026-05-19: `T-300-REALDATA-LOCAL-BASELINE` ejecutada como `PARTIAL`. Se detectan solo nombres de entorno `ConnectionStrings__DefaultConnection` en `Process`/`Machine`; no se imprimen valores. No se confirma configuracion SQL por vertical. Evidencia en `docs/qa/real-data-local-baseline.md`. Cursor pasa a `T-301-CLIENTES-SQL-READONLY-LOCAL`.
- 2026-05-19: `T-301-CLIENTES-SQL-READONLY-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se implemento repositorio SQL read-only de Clientes sobre `IdentidadCliente` + `Identidad`, filtro por `BrokerIntegracionId`, frontend API en `VITE_USE_BACKEND=true` y tests dirigidos OK. Smoke SQL real local queda `SKIPPED_ENV_MISSING` porque no hay configuracion por vertical confirmada sin secretos en el entorno. Evidencia en `docs/qa/clientes-sql-readonly-local-evidence.md`. Cursor pasa a `T-302-CLIENTES-CRUD-SDD`.
- 2026-05-19: `T-302-CLIENTES-CRUD-SDD` cerrada. Se creo `docs/sdd/specs/iLiniumTech/SDD-2026-016-clientes-crud-bbdd.md` con contrato CRUD, permisos, flag, transacciones, auditoria sin PII, matriz de campos, smoke y regla MVP-owned `ILMVP-CLI-*`. Validacion documental y seguridad OK. Cursor pasa a `T-303-CLIENTES-CRUD-BBDD-LOCAL`.
- 2026-05-19: `T-303-CLIENTES-CRUD-BBDD-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se implemento API/frontend CRUD Clientes con permisos `clientes.create/update/delete`, `Clientes:WritesEnabled`, SQL transaccional parametrizado y update/delete limitado a `ILMVP-CLI-*`; tests dirigidos backend/frontend OK. Smoke SQL real local queda `SKIPPED_ENV_MISSING` por ausencia de configuracion local visible sin secretos. Evidencia en `docs/qa/clientes-crud-bbdd-local-evidence.md`. Cursor pasa a `T-200B-AGENDA-SMOKE-SQL-LOCAL`.
- 2026-05-19: `T-200B-AGENDA-SMOKE-SQL-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se revisaron solo nombres de variables en `Process`, `User` y `Machine`; no hay configuracion `Agenda__*`, `ConnectionStrings__Agenda*` ni `ConnectionStrings__AppBuilderMaster` visible sin secretos. Evidencia actualizada en `docs/qa/agenda-crud-bbdd-evidence.md`. Cursor pasa a `T-304-SINIESTROS-SQL-READONLY-LOCAL`.
- 2026-05-19: `T-304-SINIESTROS-SQL-READONLY-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se implemento repositorio SQL read-only de Siniestros sobre `dbo.Siniestro` con filtro `BrokerIntegracionId`, joins minimizados a `RiesgoPoliza`/`Poliza`/`Catalogo`, frontend API en `VITE_USE_BACKEND=true` y acciones de detalle/exportacion bloqueadas. Smoke SQL real local queda `SKIPPED_ENV_MISSING` por ausencia de configuracion `Siniestros__*`, `ConnectionStrings__Siniestros*` o `ConnectionStrings__AppBuilderMaster` visible sin secretos. Evidencia en `docs/qa/siniestros-sql-readonly-local-evidence.md`. Cursor pasa a `T-305-RECIBOS-SQL-READONLY-LOCAL`.
- 2026-05-20: `T-305-RECIBOS-SQL-READONLY-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se implemento repositorio SQL read-only de Recibos sobre `dbo.Recibo`, join minimizado a `Poliza`/`Catalogo`, filtro `BrokerIntegracionId`, frontend API en `VITE_USE_BACKEND=true` y columna de importes eliminada. Smoke SQL real local queda `SKIPPED_ENV_MISSING` por ausencia de configuracion `Recibos__*`, `ConnectionStrings__Recibos*` o `ConnectionStrings__AppBuilderMaster` visible sin secretos. Evidencia en `docs/qa/recibos-sql-readonly-local-evidence.md`. Cursor pasa a `T-306-SUPLEMENTOS-SQL-READONLY-LOCAL`.
- 2026-05-20: `T-306-SUPLEMENTOS-SQL-READONLY-LOCAL` cerrada como `DONE_WITH_SKIPPED_SQL_SMOKE`. Se implemento repositorio SQL read-only de Suplementos sobre `dbo.Suplemento`, join minimizado a `Poliza`/`Catalogo`, filtro `BrokerIntegracionId`, frontend API en `VITE_USE_BACKEND=true` y columna `Origen` eliminada. No se proyectan `Concepto` real, `Valor`, `ValorAnterior`, `EmailComunicacion`, tomador, beneficiario, documentos, banco, importes, recibos/declaraciones, adjuntos ni workflows. Smoke SQL real local queda `SKIPPED_ENV_MISSING` por ausencia de configuracion `Suplementos__*`, `ConnectionStrings__Suplementos*` o `ConnectionStrings__AppBuilderMaster` visible sin secretos. Evidencia en `docs/qa/suplementos-sql-readonly-local-evidence.md`. Cursor pasa a `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL`.
- 2026-05-20: `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL` cerrada como `BLOCKED_ORIGIN_UNCONFIRMED`. Se revisan `SDD-2026-012` y `docs/appbuilder/pages/propuestas/README.md`; no hay `componentId`, datasource, tabla/vista origen, regla broker/tenant ni equivalencia `Solicitudes` confirmada. No se toca runtime ni se crea SQL especulativo. Validacion OK: backend `Propuestas` 5 tests, frontend `Propuestas` 4 tests, baseline documental, secret scan y `git diff --check`. Evidencia en `docs/qa/propuestas-sql-discovery-local-evidence.md`. Cursor pasa a `T-130-LIQCIA-SDD-READONLY`.
- 2026-05-20: `T-130-LIQCIA-SDD-READONLY` cerrada. Se crea `docs/sdd/specs/iLiniumTech/SDD-2026-017-liq-cia-read-only.md` y se enlaza desde evidencia AppBuilder/QA. No se toca runtime, API, SQL ni BBDD; importes, banco, facturas, exportacion, detalle financiero y escrituras siguen bloqueados. Validacion OK: baseline documental, secret scan y `git diff --check`. Cursor pasa a `T-051-LIQCOL-SDD-READONLY`.
- 2026-05-20: `T-051-LIQCOL-SDD-READONLY` cerrada. Se crea `docs/sdd/specs/iLiniumTech/SDD-2026-018-liq-col-read-only.md` y se enlaza desde evidencia AppBuilder/QA. No se toca runtime, API, SQL ni BBDD; comisiones reales, liquidos, retenciones, banco, exportacion, detalle financiero y escrituras siguen bloqueados. Validacion OK: baseline documental, secret scan y `git diff --check`. Cursor pasa a `T-052-LOGS-THREAT-MODEL`.
- 2026-05-20: `T-052-LOGS-THREAT-MODEL` cerrada. Se crea `docs/engineering/logs-threat-model.md` y se enlaza desde evidencia AppBuilder/QA. No se toca runtime, API, SQL ni BBDD; logs reales, payloads, secretos, tokens, headers, cookies, stack traces sensibles, URLs internas, PII, detalle y exportacion siguen bloqueados. Validacion OK: baseline documental, secret scan y `git diff --check`. Cursor pasa a `T-053-CONNECTIVITY-THREAT-MODEL`.
- 2026-05-20: `T-053-CONNECTIVITY-THREAT-MODEL` cerrada. Se crea `docs/engineering/conectividad-threat-model.md` y se enlaza desde evidencia AppBuilder/QA. No se toca runtime, API, SQL ni BBDD; conectores reales, llamadas externas, URL libre, payloads, secretos, tokens, headers, cookies, responses, URLs internas, PII, detalle y exportacion siguen bloqueados. Validacion OK: baseline documental, secret scan y `git diff --check`. Cursor pasa a `T-011-SIN-FE-API-ADAPTER-BLOCKED`.
- 2026-05-20: `T-011-SIN-FE-API-ADAPTER-BLOCKED` cerrada. Se confirma que `Siniestros` usa API con `VITE_USE_BACKEND=true` y fixture solo con backend desactivado; se anade prueba unitaria del adaptador para catalogs/search backend y fallback fixture, sin tocar backend, SQL, detalle, exportacion ni escrituras. Validacion OK: `npm run test:unit -- Siniestros`, `npm run format`, `npm run lint`, `npm run build`, baseline documental, secret scan y `git diff --check`. Cursor pasa a `T-308-RECIBOS-FE-API-ADAPTER-VERIFY`.
