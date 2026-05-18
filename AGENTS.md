# AGENTS.md

Reglas de gobierno para agentes IA y colaboradores que trabajen en iLiniumTech.

## Principio base

iLiniumTech no es un runtime dinamico tipo AppBuilder. La metadata heredada se usa para analisis, extraccion, trazabilidad, SDD y scaffolding revisado. El producto debe quedar como frontend Vue estatico y backend API explicita.

## Alcance de este repo

- `iLiniumTech.Backend`: API .NET 10 con capas `Api`, `Application`, `Domain` e `Infrastructure`.
- `iLiniumTech.Frontend`: Vue 3 + Vite + TypeScript.
- `docs`: decisiones, SDD, roadmap, riesgos y guias de ingenieria.
- `tools`: gates locales y auditorias de seguridad.
- `.github`: CI, seguridad, plantillas de issue/PR.

## Reglas para todos los agentes

- Lee `README.md`, `docs/PLAN_EJECUCION_CONTINUA_IA.md`, `docs/PLAN_MAESTRO_IA.md`, `docs/ROADMAP_OBJETIVO_FINAL.md`, `docs/DECISION_PRODUCTO_ARQUITECTURA.md` y la SDD relacionada antes de cambiar comportamiento.
- No guardes secretos, connection strings reales, dumps, capturas sensibles ni datos personales reales.
- No borres archivos ni reviertas cambios ajenos sin orden explicita.
- No refactorices fuera del alcance de la tarea.
- No conviertas metadata AppBuilder en contrato runtime.
- No ejecutes SQL heredado libre ni construyas SQL estructural desde strings no revisados.
- Manten cambios pequenos, revisables y ligados a una SDD, decision o issue.
- Documenta pruebas ejecutadas, pruebas omitidas, riesgos residuales y bloqueos externos.
- Si hay cambios concurrentes, trabaja con ellos y no los deshagas.

## Convenciones detectadas

Backend:

- Solucion: `iLiniumTech.Backend\iLiniumTech.Backend.slnx`.
- Capas separadas: API, Application, Domain, Infrastructure.
- Tests en `iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests`.
- Repositorio de polizas por defecto: `InMemory`.
- Acceso SQL solo por configuracion, parametros y whitelists.
- Errores publicos deben ser sanitizados y, cuando aplique, incluir `correlationId`.

Frontend:

- Proyecto en `iLiniumTech.Frontend`.
- Feature funcional principal actual: `src/features/polizas`.
- Objetivo MVP activo desde 2026-05-18: `Polizas CRUD BBDD`, documentado en `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`.
- Antes de avanzar en otras paginas del menu, prioriza cerrar `Polizas` con CRUD contra BBDD local autorizada o documentar bloqueo tecnico explicito.
- Para CRUD de `Polizas`, no uses `Pantalla_Polizas.Poliza` como identificador de escritura; la BBDD local muestra duplicados. El recurso SQL debe evolucionar a `dbo.Poliza.Id` y `Poliza` queda como numero visible.
- Las escrituras de `Polizas` requieren permisos explicitos (`polizas.create`, `polizas.update`, `polizas.delete`), `Polizas:WritesEnabled`, transacciones, SQL parametrizado y evidencia local sin secretos.
- Paginas estaticas protegidas del menu en `src/features/*`; son superficies MVP read-only y no autorizan datos reales, APIs nuevas ni acciones sin SDD.
- `src/features/autos-particulares` existe como incremento tecnico aparcado; no ampliarlo sin decision de producto, SDD actualizada y UAT/DBA.
- Servicios compartidos en `src/services`.
- La UI de pantallas es Vue/TypeScript mantenido como codigo fuente, no metadata runtime.
- Node requerido: `>=20.19.0`.

Documentacion:

- Specs SDD en `docs/sdd/specs/iLiniumTech`.
- Plantillas SDD y security review en `docs/sdd/templates`.
- Decisiones y reglas de ingenieria en `docs/engineering`.
- Plan operativo continuo en `docs/PLAN_EJECUCION_CONTINUA_IA.md`.
- Plan maestro IA en `docs/PLAN_MAESTRO_IA.md`.
- Roadmap canonico en `docs/ROADMAP_OBJETIVO_FINAL.md`.

## Comandos base

Backend:

```powershell
dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run format
npm run lint
npm run test:unit
npm run build
```

Gate local completo:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Si el Node global no cumple la version requerida:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\ruta\a\node.exe"
```

Validacion documental:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Auditorias granulares:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

## Reglas de ramas

- Rama principal protegida: `main`.
- Los agentes IA deben usar ramas con prefijo `codex/`.
- Nombre recomendado: `codex/<sdd-o-area>-<descripcion-corta>`.
- Ejemplos: `codex/polizas-readiness`, `codex/sdd-2026-003-sql-tests`.
- Una rama debe cubrir un objetivo coherente; evita mezclar frontend, backend y documentacion no relacionada salvo que la SDD lo pida.
- No hacer push directo a `main`.

## Reglas de PR

Cada PR debe incluir:

- resumen claro del cambio;
- SDD, issue o decision enlazada;
- rutas principales tocadas;
- comandos ejecutados y resultado;
- pruebas no ejecutadas y motivo;
- impacto de seguridad/configuracion;
- evidencia de que no se introducen secretos ni datos reales;
- riesgos residuales;
- capturas o smoke visual si cambia UI.

No debe marcarse listo si:

- falta una SDD para cambio funcional relevante;
- falla build, lint, tests o seguridad;
- se introducen credenciales o datos personales reales;
- se reintroduce runtime dinamico basado en metadata AppBuilder;
- hay errores no sanitizados o SQL no parametrizado en rutas nuevas.

## Evidencia minima antes de Done

- `git status` revisado.
- Build backend o justificacion de no aplicacion.
- Tests backend o justificacion de no aplicacion.
- `npm run format`, `npm run lint`, `npm run test:unit`, `npm run build` si toca frontend.
- Auditoria de secretos limpia.
- Auditoria de dependencias sin findings bloqueantes.
- Auditoria CORS limpia si toca API/configuracion.
- Smoke backend/frontend si cambia runtime visible.
- Documentacion actualizada cuando cambian contratos, configuracion o fases.
- Riesgos residuales escritos en PR, issue o documento de cierre.
