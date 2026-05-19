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
- Decision humana 2026-05-19: el desarrollo funcional debe orientarse a datos reales en BBDD local para todas las pantallas del menu. Lee `docs/DECISION_DATOS_REALES_LOCALES.md` antes de trabajar en cualquier pagina de datos.
- No guardes secretos, connection strings reales, dumps, capturas sensibles ni datos personales reales.
- No borres archivos ni reviertas cambios ajenos sin orden explicita.
- No refactorices fuera del alcance de la tarea.
- No conviertas metadata AppBuilder en contrato runtime.
- No ejecutes SQL heredado libre ni construyas SQL estructural desde strings no revisados.
- No uses fixtures o repositorios in-memory como objetivo funcional final; son soporte de tests, desarrollo offline o fallback temporal documentado.
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
- `Polizas CRUD BBDD` queda como vertical MVP local de referencia, documentado en `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`.
- Objetivo activo desde 2026-05-19: pasar paginas del menu a verticales CRUD reales contra BBDD local, una pagina cada vez, segun `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`.
- Objetivo reforzado desde 2026-05-19: todas las pantallas funcionales deben trabajar con datos reales locales en cuanto se toque su vertical. Ver `docs/DECISION_DATOS_REALES_LOCALES.md`.
- `Siniestros`, `Recibos`, `Clientes`, `Propuestas` y `Suplementos` tienen primer backend read-only in-memory con permisos propios y contratos minimizados. Es una base tecnica transitoria: la siguiente evolucion debe reemplazarla por SQL local real minimizado.
- `Agenda CRUD BBDD` es la primera vertical fuera de Polizas. Esta documentada en `docs/sdd/specs/iLiniumTech/SDD-2026-015-agenda-crud-bbdd.md`: API + frontend CRUD MVP, permisos propios, flag `Agenda:WritesEnabled`, SQL parametrizado y filtro broker. Sigue pendiente smoke SQL local con secretos fuera de Git.
- No activar escrituras reales en paginas distintas de `Polizas` y `Agenda` sin SDD propia de escritura, origen SQL/UAT/DBA confirmado, permisos, transacciones, auditoria y smoke local. La lectura real local si es objetivo prioritario.
- Para CRUD de `Polizas`, no uses `Pantalla_Polizas.Poliza` como identificador de escritura; la BBDD local muestra duplicados. El recurso SQL debe evolucionar a `dbo.Poliza.Id` y `Poliza` queda como numero visible.
- Las escrituras de `Polizas` requieren permisos explicitos (`polizas.create`, `polizas.update`, `polizas.delete`), `Polizas:WritesEnabled`, transacciones, SQL parametrizado y evidencia local sin secretos.
- Paginas estaticas protegidas del menu en `src/features/*`; son superficies MVP iniciales pendientes de migracion a datos reales locales mediante API explicita. No autorizan acciones mutantes sin SDD.
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
- evidencia de que no se introducen secretos ni se versionan/exponen datos reales sensibles;
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
