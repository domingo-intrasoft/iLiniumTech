# Manual Codex worker

Fecha: 2026-05-14

## Objetivo

El worker manual de Codex es el primer paso ejecutable despues del dispatcher diagnostico. Su funcion es reservar una Issue concreta, crear una rama de trabajo y dejar un comentario de handoff. No ejecuta Codex automaticamente ni modifica codigo de producto.

Este diseno permite probar el ciclo real sin activar automatismos por eventos:

```text
Issue status:ready-for-ai
  -> ai-codex-manual-worker workflow_dispatch
  -> validacion de labels, app, engine y locks
  -> status:ai-in-progress
  -> rama codex/gh-<issue>-<slug>
  -> comentario de handoff
  -> implementacion manual/local
  -> PR con evidencia
```

## Workflow

Archivo:

- `.github/workflows/ai-codex-manual-worker.yml`

Evento permitido:

- `workflow_dispatch` solamente.

Inputs:

- `issueNumber`: obligatorio.
- `dryRun`: por defecto `true`.

Permisos por job:

- `contents: write` para crear la rama de handoff.
- `issues: write` para actualizar labels y comentario.

No tiene triggers por `issues`, `pull_request`, `schedule` ni `workflow_run`.

## Script

Archivo:

- `tools/github/Start-AiManualWorker.ps1`

Validaciones:

- la Issue existe y esta abierta;
- no es una PR;
- tiene exactamente un `status:*`;
- el estado es `status:ready-for-ai` o `status:ai-in-progress`;
- no tiene `status:blocked` ni `human-decision-required`;
- tiene exactamente un motor AI;
- el motor es `ai-codex` o `ai-codex-web`;
- tiene exactamente una `app:*`;
- no hay otra Issue `status:ai-in-progress` con los mismos `lock:*`.

Acciones en modo real:

- crea una rama `codex/gh-<issue>-<slug>` desde `ci-cd` o `develop`;
- cambia el estado de la Issue a `status:ai-in-progress`;
- publica o actualiza un comentario `ai-manual-worker-handoff`;
- genera artifacts JSON y Markdown.

Acciones en `dryRun=true`:

- valida la Issue;
- calcula rama base y rama worker;
- genera artifacts;
- no crea rama;
- no cambia labels;
- no comenta.

## Ramas

Regla actual:

- `app:infrastructure`, `docs/ai/**`, `.github/**` y `tools/github/**` van a `ci-cd`;
- el resto va a `develop`, salvo que la Issue indique otra decision aprobada.

La rama generada usa formato:

```text
codex/gh-<issue>-<slug>
```

## Guardrails

- No hay merge automatico.
- No hay ejecucion automatica de Codex.
- No hay cambios de codigo dentro del worker.
- No se imprimen secretos.
- Las Issues bloqueadas siguen bloqueadas.
- Los locks se tratan como exclusiones.
- Si el workflow falla, la Issue queda para revision humana.

## Siguiente fase

Cuando este worker manual sea estable, la siguiente fase es anadir un worker que ejecute una tarea pequena y abra PR. Ese paso debe seguir siendo manual al principio y requerir una Issue concreta.
