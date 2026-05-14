# Orquestacion multiagente GitHub-only

Fecha: 2026-05-14

## Objetivo

Montar un flujo de trabajo por eventos para que iLiniumTech avance de forma continua sin convertir los agentes en un sistema sin control. El objetivo no es que un agente "mande libremente", sino que un coordinador aplique reglas, limite paralelismo, desbloquee dependencias y exija evidencia antes de mover trabajo a QA, UAT o merge.

Este documento define el modelo operativo. Las automatizaciones concretas deben implementarse por fases en `.github/workflows/**` desde la rama `ci-cd`.

En la rama `ci-cd`, este documento es la fuente operativa para la Fase 1 de orquestacion. El roadmap maestro de producto vive en `develop`; cuando se sincronizan ramas, ambos documentos deben quedar alineados.

## Flujo objetivo

```text
Issue grande / epica
  -> Agente coordinador
  -> Issues pequenos y acotados
  -> Agentes programadores
  -> Pull Request
  -> Agente revisor / QA
  -> Ready for QA / Ready for UAT / correcciones
  -> Coordinador desbloquea el siguiente trabajo
```

El sistema debe reaccionar a eventos:

- issue creada o etiquetada;
- issue marcada como `status:ready-for-ai`;
- PR abierta o actualizada;
- workflow `ci` o `security` completado;
- review solicitada;
- label `status:blocked` o `human-decision-required`.

No debe depender de un agente mirando el tablero constantemente.

## Roles

### Agente coordinador

Puede:

- descomponer epicas en issues pequenos;
- priorizar y ordenar dependencias;
- asignar area, riesgo, locks y motor sugerido;
- lanzar agentes mediante labels o `workflow_dispatch`;
- detectar bloqueos;
- pedir revision humana;
- mover trabajo entre estados cuando hay evidencia;
- cerrar tareas documentales si la evidencia es suficiente.

No puede:

- hacer merge automatico a `main`;
- cambiar arquitectura sin ADR o SDD aprobada;
- lanzar trabajo paralelo sin respetar locks;
- tocar codigo directamente como parte de la coordinacion;
- saltarse QA/UAT;
- debilitar CI, seguridad, branch protection o CODEOWNERS;
- aprobar su propia PR.

### Agente programador

Puede:

- trabajar solo en el issue asignado;
- crear una rama corta;
- modificar un conjunto de rutas permitido;
- ejecutar validaciones requeridas;
- abrir PR con evidencia.

No puede:

- ampliar alcance sin comentario de coordinador;
- tocar rutas prohibidas sin override;
- leer o registrar secretos;
- mezclar backend, frontend, BBDD, pipelines y arquitectura si el issue no lo autoriza;
- mergear su propio trabajo.

### Agente revisor / QA

Puede:

- revisar PRs;
- verificar diffs, tests, seguridad y cumplimiento SDD;
- pedir cambios;
- marcar `status:ready-for-qa` o `status:ready-for-uat`;
- bloquear con `human-decision-required`.

No puede:

- reescribir la solucion completa durante la review;
- aceptar cambios sin evidencia;
- convertir una review tecnica en decision funcional de negocio.

### Humano responsable

Debe intervenir cuando:

- cambia arquitectura;
- hay decision funcional ambigua;
- se toca identidad, autorizacion, datos reales, BBDD, secretos o despliegue;
- hay conflicto entre SDD y comportamiento observado;
- se quiere promover a `main` o produccion;
- QA/UAT encuentra discrepancias.

## Estados

Usar labels `status:*` como fuente simple y compatible con GitHub Projects:

| Estado | Label | Significado |
| --- | --- | --- |
| New | `status:new` | Issue creado, aun sin preparar. |
| Ready for AI | `status:ready-for-ai` | Puede ser tomado por dispatcher/agente. |
| AI In Progress | `status:ai-in-progress` | Agente trabajando. |
| AI Done | `status:ai-done` | Agente termino y dejo evidencia. |
| AI Review | `status:ai-review` | PR o resultado esperando revision tecnica. |
| Ready for QA | `status:ready-for-qa` | Checks y review tecnica suficientes para QA. |
| Ready for UAT | `status:ready-for-uat` | QA pasada, pendiente validacion funcional. |
| Ready for Merge | `status:ready-for-merge` | Listo para merge segun reglas de rama. |
| Done | `status:done` | Merge/cierre completado. |
| Blocked | `status:blocked` | Falta decision, dependencia, entorno o secreto. |

Reglas:

- Un issue debe tener como maximo un `status:*` activo.
- El dispatcher solo toma issues con `status:ready-for-ai`.
- Los estados de QA/UAT no sustituyen checks de CI.
- `status:ready-for-merge` no autoriza merge a `main` sin reglas de proteccion.

## Labels base

### Identidad AI

- `ai-parent`: epica o issue coordinador.
- `ai-task`: tarea acotada ejecutable.
- `ai-generated`: PR o rama generada por agente.
- `ai-review-required`: requiere review de agente o humana.
- `ai-coordinator`: issue de coordinacion.

### Control y calidad

- `qa-required`
- `uat-required`
- `test-plan-linked`
- `blocked-by-dependency`
- `merge-conflict-risk`
- `human-decision-required`

### Apps y areas

El dispatcher usa exactamente una label `app:*` como ruta canonica de trabajo:

- `app:api`
- `app:frontend`
- `app:extractor`
- `app:infrastructure`
- `app:docs`
- `app:core`

Las labels `area:*` son complementarias para riesgo y reporting, no sustituyen a `app:*`:

- `area:backend`
- `area:frontend`
- `area:database`
- `area:architecture`
- `area:pipelines`
- `area:docs`
- `area:security`
- `area:extractor`

### Locks de paralelismo

- `lock:database`
- `lock:architecture`
- `lock:pipelines`
- `lock:security`
- `lock:release`

Los locks no son decorativos: el coordinador y los workflows deben tratarlos como exclusiones.

## Limites de paralelismo

Limites recomendados:

| Tipo | Maximo simultaneo | Motivo |
| --- | ---: | --- |
| Backend | 3 | Permite avance sin pisar contratos. |
| Frontend | 2 | Reduce conflictos visuales y de estado. |
| Base de datos | 1 | Evita cambios incompatibles de esquema/queries. |
| Arquitectura | 1 | Requiere decision y coherencia global. |
| Pipelines | 1 | Evita romper CI/CD mientras se cambia. |
| Seguridad/identidad | 1 | Alto riesgo y necesidad de revision humana. |
| Release/main | 1 | Evita promociones simultaneas. |

Implementacion en GitHub Actions:

- usar `concurrency` por workflow para evitar duplicados por rama;
- usar `concurrency` por issue para que dos agentes no trabajen el mismo ticket;
- usar grupos logicos para locks criticos, por ejemplo `ai-lock-database`;
- usar `cancel-in-progress: false` en trabajos de agentes cuando se quiera que el run actual termine;
- usar `cancel-in-progress: true` en CI por rama cuando el commit nuevo invalida el anterior.

## Reglas de ramas

Ramas permanentes:

- `main`: estable, protegida, solo cortes aprobados.
- `develop`: producto y funcionalidad.
- `ci-cd`: plataforma, workflows, gobierno, seguridad y automatizacion.

Ramas cortas:

- `codex/<slug>` para trabajo asistido por Codex.
- `feature/<slug>` para funcionalidad.
- `fix/<slug>` para correcciones.
- `infra/<slug>` para plataforma.
- `docs/<slug>` para documentacion.

Rutas:

- producto hacia `develop`;
- plataforma hacia `ci-cd`;
- estabilizacion hacia `main` mediante PR revisada.

## Eventos recomendados

### Issue preparada

Evento:

- issue etiquetada con `status:ready-for-ai`.

Accion:

1. Validar que tiene `ai-task` o cuelga de `ai-parent`.
2. Validar que existe exactamente una label canonica `app:*`.
3. Validar paths permitidos/prohibidos.
4. Validar locks y limite de paralelismo.
5. Si OK, mover a `status:ai-in-progress` y lanzar agente.
6. Si KO, mover a `status:blocked` y comentar motivo.

### PR abierta por agente

Evento:

- `pull_request` opened/synchronize.

Accion:

1. Ejecutar `ci` y `security`.
2. Verificar que la PR referencia issue y SDD.
3. Aplicar label `ai-generated` si procede.
4. Solicitar review si checks pasan.

### Checks completados

Evento:

- `workflow_run` completado para `ci` o `security`.

Accion:

1. Si falla, mantener en `status:ai-review` o `status:blocked`.
2. Si pasa y hay review suficiente, mover a `status:ready-for-qa`.
3. Si requiere UAT, mover despues a `status:ready-for-uat`.
4. Nunca mergear automaticamente a `main`.

### Review con cambios

Evento:

- review `changes_requested`.

Accion:

1. Devolver a `status:ready-for-ai` si la correccion es acotada.
2. Mantener `status:blocked` si requiere decision humana.
3. Evitar relanzar mas de una vez sin revisar el motivo.

## Plantilla minima de epica

Una epica debe contener:

- objetivo final;
- SDD o decision asociada;
- alcance;
- fuera de alcance;
- lista inicial de subtareas;
- dependencias;
- locks esperados;
- criterios de salida;
- evidencias de QA/UAT.

## Plantilla minima de tarea AI

Una tarea AI debe contener:

- objetivo unico;
- issue padre;
- rama base;
- area;
- paths permitidos;
- paths prohibidos;
- validacion obligatoria;
- labels de lock;
- criterio de done;
- instrucciones de PR.

El contrato completo de entrada, evidencia, PR, transiciones y manejo de fallos esta definido en `docs/ai/issue-agent-pr-contract.md`.

## Fases de implantacion

### Fase 0 - Gobierno manual versionado

Ya puede funcionar con:

- docs de politica;
- labels creadas;
- issue templates;
- PR template;
- CODEOWNERS;
- control humano de estados.

### Fase 1 - Dispatcher diagnostico

Crear workflow manual/scheduled que:

- lista issues candidatas;
- valida labels, locks y bloqueos;
- no lanza agentes;
- comenta diagnostico.

Implementacion inicial:

- workflow `.github/workflows/ai-issue-dispatcher.yml`;
- ejecucion manual con `workflow_dispatch`;
- schedule conservador semanal solo en modo diagnostico;
- job principal con permisos `contents: read` e `issues: read`;
- job opcional de comentario con `issues: write`, limitado a `comment=true` y `issueNumber` explicito, porque GitHub requiere permiso de escritura para crear comentarios en issues;
- comentarios idempotentes mediante marcador `ai-issue-dispatcher-diagnostics`, actualizando el comentario anterior si ya existe;
- script `tools/github/Invoke-AiIssueDispatcherDiagnostics.ps1`;
- no lanza agentes, no cambia labels, no crea ramas, no modifica codigo.

### Fase 2 - Dispatcher con `workflow_dispatch`

Permitir que el coordinador lance una tarea concreta:

- input `issueNumber`;
- input `execute=true`;
- maximo `top=1`;
- sin schedule automatico al principio.

Primer corte manual:

- workflow `.github/workflows/ai-codex-manual-worker.yml`;
- script `tools/github/Start-AiManualWorker.ps1`;
- `workflow_dispatch` con `issueNumber` obligatorio;
- `dryRun=true` por defecto;
- valida elegibilidad, locks, engine y app;
- crea rama de handoff y mueve a `status:ai-in-progress` solo en modo real;
- no ejecuta Codex automaticamente;
- no abre PR si no hay cambios de codigo.

### Fase 3 - Agente programador

Workflow por motor:

- crea rama;
- ejecuta cambios;
- abre PR;
- comenta evidencia;
- no mergea.

### Fase 4 - Revisor / QA

Workflow o agente de review:

- lee PR;
- comprueba diffs;
- revisa tests;
- marca `status:ai-review`, `status:ready-for-qa` o `status:blocked`.

### Fase 5 - Orquestacion por eventos

Activar:

- `workflow_run` para checks completados;
- limits por `concurrency`;
- deduplicacion por issue;
- watchdogs sin autorreparacion agresiva.

## Guardrails

- Maximo 5 issues AI abiertos por epica salvo aprobacion humana.
- Maximo 1 reintento automatico por fallo de agente.
- Cualquier cambio en `.github/workflows/**`, `tools/security/**`, auth, autorizacion, BBDD real o secretos requiere `human-decision-required`.
- Las tareas con `lock:*` no pueden correr si ya hay una tarea activa con el mismo lock.
- Las PRs generadas deben ser pequenas; si superan el alcance, se parten.
- No se hacen merges automaticos a `main`.

## Referencias oficiales

- GitHub Actions `concurrency`: https://docs.github.com/en/actions/reference/workflows-and-actions/workflow-syntax#concurrency
- Eventos que disparan workflows, incluido `workflow_run`: https://docs.github.com/en/actions/reference/workflows-and-actions/events-that-trigger-workflows
- Gestion de labels en issues y PRs: https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels
