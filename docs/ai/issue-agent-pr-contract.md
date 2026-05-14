# Issue to agent to PR contract

Fecha: 2026-05-14

## Objetivo

Este contrato define como una Issue preparada se convierte en trabajo de agente, como se deja evidencia y como se entrega una PR revisable. Es la pieza operativa entre la cola GitHub-only y los agentes programadores/revisores.

El contrato aplica a:

- dispatcher diagnostico;
- worker manual de Codex;
- futuros workers que ejecuten cambios y abran PR;
- agentes locales usados mientras el ciclo automatico madura.

## Entrada minima

Una Issue solo puede entrar al flujo de agente si contiene informacion suficiente para trabajar sin adivinar alcance.

Labels obligatorias:

- exactamente un `status:*`;
- `status:ready-for-ai` para empezar desde dispatcher;
- exactamente un motor: `ai-codex`, `ai-claude`, `ai-codex-web` o `ai-claude-web`;
- exactamente una ruta canonica `app:*`;
- `ai-task`, salvo que sea una epica `ai-parent`;
- `test-plan-linked` cuando cambie comportamiento o validacion.

Campos o secciones obligatorias en el cuerpo:

- objetivo unico;
- Issue padre, SDD, ADR o documento de decision;
- rama base esperada;
- paths permitidos;
- paths prohibidos;
- validacion obligatoria;
- criterio de done;
- instrucciones de PR.

Labels de control opcionales:

- `qa-required`;
- `uat-required`;
- `ai-review-required`;
- `merge-conflict-risk`;
- `blocked-by-dependency`;
- `lock:*`.

Labels bloqueantes:

- `status:blocked`;
- `human-decision-required`;
- `status:ready-for-qa`;
- `status:ready-for-uat`;
- `status:ready-for-merge`;
- `status:done`.

## Payload normalizado

Antes de lanzar un worker, el coordinador o workflow debe transformar la Issue en un payload equivalente a este modelo:

```json
{
  "issueNumber": 10,
  "title": "AI: definir contrato operativo Issue a agente a PR",
  "parent": "#9",
  "engine": "ai-codex",
  "app": "app:docs",
  "status": "status:ready-for-ai",
  "baseBranch": "ci-cd",
  "workerBranch": "codex/gh-10-ai-definir-contrato-operativo",
  "allowedPaths": ["docs/ai/**"],
  "forbiddenPaths": [".github/workflows/**", "tools/security/**"],
  "locks": [],
  "requiredValidation": [
    "review docs/ai/github-ai-policy.md",
    "review docs/ai/multi-agent-orchestration.md"
  ],
  "doneCriteria": [
    "PR references issue",
    "evidence included",
    "residual risks stated"
  ]
}
```

El payload no debe incluir secretos, connection strings, tokens, dumps, datos personales reales ni SQL con identificadores sensibles.

## Rutas y ramas

Rama base:

- producto: `develop`;
- plataforma y automatizacion: `ci-cd`;
- estabilizacion: `main` solo por PR revisada.

Rama worker:

```text
codex/gh-<issue>-<slug>
claude/gh-<issue>-<slug>
feature/gh-<issue>-<slug>
fix/gh-<issue>-<slug>
```

Reglas:

- una rama worker pertenece a una Issue principal;
- no se reutiliza una rama para otra Issue;
- una PR debe apuntar a la rama base declarada;
- si el alcance real exige cambiar de rama base, la Issue vuelve a `status:blocked`.

## Transiciones permitidas

Transiciones de coordinacion:

```text
status:new
  -> status:ready-for-ai
  -> status:ai-in-progress
  -> status:ai-review
  -> status:ready-for-qa
  -> status:ready-for-uat
  -> status:ready-for-merge
  -> status:done
```

Transiciones de bloqueo:

```text
status:ready-for-ai -> status:blocked
status:ai-in-progress -> status:blocked
status:ai-review -> status:blocked
```

Transiciones de correccion:

```text
status:ai-review -> status:ready-for-ai
status:ready-for-qa -> status:ai-review
status:ready-for-uat -> status:ai-review
```

Reglas:

- solo puede haber un `status:*` activo;
- el dispatcher solo toma `status:ready-for-ai`;
- un worker puede mover a `status:ai-in-progress` cuando reserva la tarea;
- un agente programador mueve a `status:ai-review` al abrir PR o dejar evidencia final;
- un revisor/QA mueve a `status:ready-for-qa`, `status:ready-for-uat`, `status:blocked` o devuelve a `status:ready-for-ai`;
- `status:ready-for-merge` no sustituye branch protection ni review humana.

## Evidencia del agente programador

Cada entrega de agente debe dejar evidencia en la PR y, cuando proceda, en comentario de Issue.

Formato minimo:

```text
Issue: #<number>
Parent/Epic: #<number>
SDD/ADR/Policy: <path or id>
Base branch: <branch>
Worker branch: <branch>
Scope completed:
- <short item>

Files changed:
- <path>

Validation:
- <command>: <result>

Security:
- Secrets not added.
- Sensitive data not logged.
- Dynamic SQL/auth/config impact: <none|explained>

Not run:
- <command>: <reason>

Residual risk:
- <risk or none>
```

La evidencia debe ser concreta. No basta con decir "tests passed" si no aparece el comando o el check que lo demuestra.

## PR contract

Una PR generada o asistida por agente debe cumplir:

- referencia la Issue con `Closes #<number>` solo si el merge debe cerrarla;
- enlaza SDD, ADR o politica;
- explica alcance y fuera de alcance;
- enumera validaciones ejecutadas;
- declara validaciones no ejecutadas y motivo;
- no incluye secretos ni datos reales;
- mantiene cambios pequenos y revisables;
- no mezcla producto, plataforma, seguridad y BBDD salvo aprobacion explicita;
- no desactiva checks, branch protection, CODEOWNERS ni seguridad.

Labels recomendadas en PR:

- `ai-generated` si la rama o PR fue creada por agente;
- `ai-review-required` cuando necesita review tecnica;
- `qa-required` o `uat-required` si aplica;
- area/app equivalentes a la Issue.

## Review y QA

El revisor debe comprobar:

- la PR toca solo paths permitidos;
- no hay cambios fuera de alcance;
- la evidencia coincide con los cambios;
- CI y security estan verdes o el fallo esta justificado;
- los riesgos de seguridad estan tratados;
- la SDD sigue alineada con el comportamiento;
- la Issue queda en el siguiente estado correcto.

El revisor no debe aprobar:

- PRs sin Issue;
- PRs sin evidencia;
- PRs con secretos o datos reales;
- cambios de auth, BBDD, workflows o permisos sin approval block;
- PRs que reduzcan calidad o seguridad para hacer pasar checks.

## Fallos y bloqueos

Un worker debe bloquear o devolver la tarea si detecta:

- labels inconsistentes;
- mas de una `app:*` o mas de un motor;
- lock activo;
- paths prohibidos;
- falta de SDD/ADR cuando el cambio altera comportamiento;
- dependencia externa no disponible;
- credenciales o datos reales necesarios;
- conflicto de rama;
- fallo repetido de CI que excede el reintento permitido.

Comentario de bloqueo:

```text
Blocked reason:
- <reason>

Needed decision:
- <specific decision or input>

Safe next step:
- <action>
```

## Politica de reintentos

- Un worker puede reintentarse una vez si el fallo es tecnico y acotado.
- Un segundo fallo requiere `status:blocked` o review humana.
- Un fallo por seguridad, secretos, permisos o BBDD real bloquea inmediatamente.
- No se relanza una tarea si el alcance cambio sin actualizar la Issue.

## Criterios para pasar a automatizacion completa

Antes de permitir que un worker ejecute codigo y abra PR automaticamente, deben cumplirse estos puntos:

- dispatcher diagnostico estable en `main`;
- worker manual probado con `dryRun=true` y `dryRun=false`;
- comentarios de handoff idempotentes;
- labels y locks funcionando en Issues reales;
- PR template usado de forma consistente;
- branch protection activa en `main`;
- tareas sensibles bloqueadas por defecto;
- evidencias de al menos una tarea de bajo riesgo completada end-to-end.

Hasta entonces, la implementacion puede hacerse con agentes locales o trabajo manual, pero siempre dejando la misma evidencia contractual.
