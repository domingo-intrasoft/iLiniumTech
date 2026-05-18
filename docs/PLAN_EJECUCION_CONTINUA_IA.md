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
- Proximo MVP activo: paridad visual de `/polizas` con AppBuilder publicado, manteniendo CRUD real y sin simulaciones.
- Paginas del menu distintas de Polizas siguen en carril fixture/read-only o bloqueadas y quedan pospuestas hasta cerrar el siguiente corte visual de Polizas.
- Ronda documental por pagina completada en `docs/appbuilder/pages/page-agent-coordination-2026-05-18.md`.
- SDD drafts existentes para `Siniestros`, `Recibos`, `Clientes`, `Agenda`, `Propuestas` y `Suplementos`.
- No activar datos reales, APIs nuevas, escrituras, exportaciones ni permisos nuevos para paginas fixture sin SDD aprobada, contrato API, UAT/DBA y seguridad.

## Siguiente tarea activa

ID: `T-041-POL-AUDIT-WRITES`

Estado: `READY`

Nombre: disenar auditoria de escritura de Polizas sin datos sensibles.

Objetivo:

- Documentar un diseno inicial de auditoria para altas, actualizaciones y bajas tecnicas de Polizas.
- No implementar runtime, tablas, endpoints ni migraciones en este paso.
- Evitar datos personales, documentos, telefonos, direcciones, matriculas completas, connection strings o SQL sensible en logs/auditoria.
- Definir eventos, campos permitidos, campos prohibidos, correlacion, permisos y riesgos para una SDD posterior si se implementa.

Fuentes a leer, sin buscar mas salvo bloqueo:

- `AGENTS.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`
- `docs/qa/polizas-crud-bbdd-evidence.md`
- `docs/qa/polizas-visual-parity-evidence.md`
- `docs/engineering/README.md`

Archivos que puede tocar:

- `docs/engineering/polizas-write-audit-design.md`
- `docs/qa/polizas-crud-bbdd-evidence.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`

Archivos que NO debe tocar:

- `iLiniumTech.Backend/**`
- `iLiniumTech.Frontend/**`
- `docs/sdd/**`
- `docs/appbuilder/pages/page-agent-rollout.md`
- `docs/appbuilder/pages/page-agent-coordination-2026-05-18.md`
- cualquier `.env*`, config con secretos, dumps o capturas sensibles.

Pasos de implementacion:

1. Crear `docs/engineering/polizas-write-audit-design.md` con proposito, no objetivos, eventos, campos permitidos, campos prohibidos, retencion, correlacion, seguridad y preguntas UAT/DBA.
2. Enlazar el documento desde `docs/qa/polizas-crud-bbdd-evidence.md` como pendiente tecnico de auditoria.
3. No tocar codigo ni proponer tablas concretas como implementadas.
4. Ejecutar validacion documental, secret scan y `git diff --check`.
5. Si todo pasa, marcar esta tarea `DONE` y mover el cursor a `T-042-POL-CRUD-REGRESSION-SMOKE`.

Pruebas obligatorias:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Criterios de aceptacion:

- Documento de auditoria claro, accionable y sin secretos.
- Campos sensibles quedan explicitamente prohibidos o minimizados.
- Eventos CRUD MVP quedan definidos a nivel conceptual.
- Preguntas UAT/DBA quedan separadas de decisiones ya tomadas.
- Commit pequeno y documental.

Riesgo de conflicto: bajo si se respetan los archivos permitidos.

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
| `T-002-REC-FE-CONTRACT-FIXTURE` | `TODO` | Separar contrato fixture de Recibos sin importes reales ni banco. | `src/features/recibos/**`, `docs/qa/recibos-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-003-CLI-FE-CONTRACT-FIXTURE` | `TODO` | Separar contrato fixture de Clientes con PII bloqueada. | `src/features/clientes/**`, `docs/qa/clientes-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-004-AGE-FE-CONTRACT-FIXTURE` | `TODO` | Separar contrato fixture de Agenda con rango/listado, sin calendario interactivo. | `src/features/agenda/**`, `docs/qa/agenda-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-005-PRO-FE-CONTRACT-FIXTURE` | `TODO` | Separar contrato fixture de Propuestas sin conversion, documentos ni importes reales. | `src/features/propuestas/**`, `docs/qa/propuestas-mvp-evidence.md` | backend, services, router, layout | frontend completo |
| `T-006-SUP-FE-CONTRACT-FIXTURE` | `TODO` | Separar contrato fixture de Suplementos sin detalle, workflows ni adjuntos. | `src/features/suplementos/**`, `docs/qa/suplementos-mvp-evidence.md` | backend, services, router, layout | frontend completo |

### Fase 2 - Preparar contratos read-only API sin datos reales

Objetivo: disenar contratos y pruebas de dominio antes de SQL real. No conectar BBDD hasta que UAT/DBA desbloquee origen y minimizacion.

| ID | Estado | Tarea | Archivos permitidos | Bloqueo |
| --- | --- | --- | --- | --- |
| `T-010-SIN-BE-CONTRACT-DESIGN` | `TODO` | Crear modelos/validadores backend de Siniestros read-only con repositorio fixture/in-memory, sin SQL. | backend Siniestros nuevo, tests backend, docs QA | no SQL real |
| `T-011-SIN-FE-API-ADAPTER-BLOCKED` | `TODO` | Preparar adaptador frontend Siniestros con fallback fixture solo cuando `VITE_USE_BACKEND=false`. | `src/features/siniestros/**` | requiere `T-010` |
| `T-012-REC-BE-CONTRACT-DESIGN` | `TODO` | Modelos/validadores backend Recibos read-only sin importes reales. | backend Recibos nuevo, tests backend | no SQL real |
| `T-013-CLI-BE-CONTRACT-DESIGN` | `TODO` | Modelos/validadores backend Clientes read-only minimizado sin PII ampliada. | backend Clientes nuevo, tests backend | no SQL real |

### Fase 3 - Datos reales read-only por pagina

Objetivo: activar lectura SQL minimizada solo cuando haya SDD, UAT/DBA, permisos y origen autorizado.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-020-SIN-SQL-READONLY` | `BLOCKED_EXTERNAL` | Implementar SQL read-only Siniestros. | UAT columnas/filtros, DBA origen, broker/tenant, security review |
| `T-021-REC-SQL-READONLY` | `BLOCKED_EXTERNAL` | Implementar SQL read-only Recibos. | UAT/DBA, minimizacion financiera, permisos |
| `T-022-CLI-SQL-READONLY` | `BLOCKED_EXTERNAL` | Implementar SQL read-only Clientes. | UAT/DBA, PII, permisos |
| `T-023-AGE-SQL-READONLY` | `BLOCKED_EXTERNAL` | Implementar SQL read-only Agenda por rango. | UAT/DBA, PII en asunto/descripcion |

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
| `T-041-POL-AUDIT-WRITES` | `READY` | Disenar auditoria de escritura sin datos sensibles. | puede ser documental |
| `T-042-POL-CRUD-REGRESSION-SMOKE` | `TODO` | Mantener smoke local API/UI de CRUD real sin secretos. | BBDD local disponible |
| `T-043-POL-APPBUILDER-VISUAL-SHELL` | `DONE` | Aproximar `/polizas` al shell/grid visual de AppBuilder publicado sin simular datos. | captura usuario y SDD-2026-014 |
| `T-044-POL-APPBUILDER-VISUAL-SMOKE` | `DONE` | Ejecutar smoke visual y documentar diferencias pendientes frente a AppBuilder. | `T-043` |

### Fase 6 - Reporting, liquidaciones y superficies tecnicas

Objetivo: no abrir superficies de alto riesgo sin threat model.

| ID | Estado | Tarea | Precondicion |
| --- | --- | --- |
| `T-050-LIQCIA-SDD-READONLY` | `TODO` | Preparar SDD Liq.Cia read-only financiera sin importes reales. | readiness ya existe |
| `T-051-LIQCOL-SDD-READONLY` | `TODO` | Preparar SDD Liq.Col read-only financiera sin comisiones reales. | readiness ya existe |
| `T-052-LOGS-THREAT-MODEL` | `TODO` | Crear threat model para Logs antes de cualquier API. | readiness ya existe |
| `T-053-CONNECTIVITY-THREAT-MODEL` | `TODO` | Crear threat model para Conectividad/secretos/SSRF. | readiness ya existe |

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
