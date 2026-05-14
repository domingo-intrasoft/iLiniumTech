# Plan CI/CD GitHub-only para iLiniumTech

Fecha: 2026-05-13  
Referencia analizada: `C:\Desarrollo\AcademiaLasCortes-boards-ai`, rama `demo`, commit `dae8b2f5`.

## 1. Objetivo

Montar en iLiniumTech un flujo equivalente al de `AcademiaLasCortes/demo`, pero sin Azure DevOps como autoridad final.

En AcademiaLasCortes el patron fue:

1. Azure Boards actuaba como cola de trabajo.
2. Azure Pipelines ejecutaba dispatcher, runner local, validaciones y despliegues.
3. GitHub se usaba como workspace temporal para Claude/Codex Web.
4. El bridge hacia GitHub creaba rama, Issue y workflow.
5. GitHub Actions ejecutaba Codex o Claude, publicaba rama y PR en GitHub.
6. Otro workflow devolvia el diff a Azure DevOps.
7. Azure DevOps creaba la PR final, validaba policies y cerraba el ciclo de Boards.

En iLiniumTech se sustituye por:

1. GitHub Issues y GitHub Projects actuan como cola de trabajo.
2. GitHub Actions orquesta dispatcher, runners de IA, validaciones y despliegues.
3. GitHub PR es la PR final.
4. No hay mirror DevOps -> GitHub ni retorno GitHub -> DevOps.
5. El cierre de ciclo queda en GitHub: Issue, rama, PR, checks, environment y release.

## 2. Que se aprende de AcademiaLasCortes/demo

Archivos clave revisados:

- `azure-pipelines-ai-dispatcher.yml`: polling cada 10 minutos, reserva de work items, watchdogs y derivacion por motor.
- `azure-pipelines-ai-workitem.yml`: runner manual/local, validacion de politica, reserva `ai-running` y ejecucion controlada.
- `azure-pipelines-ai-codex-web.yml`: bridge Codex Web, sincronizacion DevOps -> GitHub, rama, Issue y dispatch.
- `azure-pipelines-ai-claude-web.yml`: bridge Claude Web equivalente.
- `.github/workflows/codex-web-workitem.yml`: ejecucion Codex en GitHub Actions, commit, PR y comentario.
- `.github/workflows/claude-web-workitem.yml`: ejecucion Claude en GitHub Actions, commit, PR y comentario.
- `.github/workflows/return-ai-web-to-ado.yml`: retorno controlado a Azure DevOps. En iLiniumTech desaparece.
- `azure-pipelines-ai-web-finalizer.yml`: finalizador Azure DevOps. En iLiniumTech se convierte en checks, merge y environment de GitHub.
- `docs/ai/boards-ai-policy.md`: reglas de elegibilidad, motores, bloqueos, overrides y trazabilidad.
- `docs/ai/ai-engine-routing.md`: decision de motor local/web segun riesgo.
- `docs/ai/ai-web-return-contract.md`: contrato de retorno. En iLiniumTech se reemplaza por contrato Issue -> PR -> merge.
- `docs/ai/pipeline-failure-recovery.md`: autorreparacion controlada de fallos de pipeline.
- `docs/engineering/06-ci-cd-quality-gates.md`: fases de gates de calidad.

Lecciones que hay que conservar:

- No ejecutar IA solo porque exista una tarea. Debe estar lista, etiquetada y tener politica.
- Reservar antes de ejecutar para evitar duplicados.
- Usar una rama por Issue.
- Usar una PR por Issue.
- Prohibir rutas sensibles salvo aprobacion explicita.
- Validar antes de mover trabajo a listo para probar.
- No desplegar a entornos compartidos automaticamente salvo workflow manual o environment protegido.
- Los fallos de pipeline pueden generar Issues tecnicas, pero no deben crear bucles infinitos.
- La trazabilidad debe vivir en comentarios estructurados de la Issue/PR.

Lecciones que ya no aplican:

- No hace falta `AI_GITHUB_BRIDGE_TOKEN` para crear artefactos desde DevOps.
- No hace falta `ADO_PAT`.
- No hace falta commit snapshot de DevOps a GitHub.
- No hace falta `return-ai-web-to-ado.yml`.
- No hace falta crear PR final en Azure DevOps.
- No hace falta mover columnas de Azure Boards.

## 3. Politica de ramas decidida

iLiniumTech usara trunk-based development.

Ramas permanentes:

- `main`: unica rama permanente, rama por defecto y fuente de verdad.

Ramas que no se usaran:

- No usar `demo` como rama.
- No usar `develop`.
- No mantener ramas largas de integracion.

Ramas cortas permitidas:

- `feature/gh-<issue>-<slug>` para funcionalidad humana.
- `fix/gh-<issue>-<slug>` para bugs.
- `docs/gh-<issue>-<slug>` para documentacion.
- `infra/gh-<issue>-<slug>` para CI/CD, seguridad o automatizacion.
- `codex/gh-<issue>-<slug>` para trabajo de Codex.
- `claude/gh-<issue>-<slug>` para trabajo de Claude.
- `experiment/<slug>` solo para pruebas no protegidas y sin despliegue.
- `release/<version>` solo si mas adelante hace falta estabilizar una version concreta antes de publicar.

Reglas:

- Toda rama que aspire a entrar en el producto abre PR contra `main`.
- No hay push directo a `main` salvo emergencia documentada y con permisos de administrador.
- Las ramas de IA siempre nacen de `main` actualizado.
- Una rama de trabajo debe vivir lo minimo posible y cerrarse al mergear o descartar la PR.
- Los despliegues no se modelan con ramas; se modelan con GitHub Environments.

Entornos:

- `preview`: entorno compartido de prueba temprana, protegido por aprobacion cuando tenga secretos o coste.
- `production`: entorno futuro, creado solo cuando exista despliegue real.

Motivo:

- El proyecto es nuevo y no necesita heredar la rama `demo` de AcademiaLasCortes.
- GitHub Actions ejecuta workflows programados y manuales desde la rama por defecto, asi que `main` debe concentrar la orquestacion.
- Un trunk unico reduce merges artificiales y hace mas facil que Codex/Claude trabajen con PRs pequenas.
- `preview` expresa mejor que `demo` el uso esperado: validar MVPs y entregas parciales sin convertirlo en rama permanente.

## 4. Estado objetivo del flujo GitHub-only

Flujo normal de una tarea:

1. Se crea una Issue con plantilla SDD.
2. Se etiqueta con `ai-ready` y un motor: `ai-codex`, `ai-claude`, `ai-codex-web` o `ai-claude-web`.
3. La Issue entra en GitHub Project con Status `Ready for AI`.
4. El dispatcher programado la detecta.
5. El dispatcher valida:
   - Issue abierta.
   - Tiene `ai-ready` o Status `Ready for AI`.
   - No tiene `needs-human`, `ai-running`, `ai-queued`, `validated`, `preview-ready` ni `done`.
   - Tiene un unico motor.
   - Tiene area/app valida.
   - Existe politica versionada de esa app.
   - No pide tocar rutas sensibles sin override.
6. El dispatcher reserva la Issue:
   - anade `ai-queued`;
   - comenta `[AI DISPATCHER]`;
   - dispara el workflow del motor.
7. El workflow del motor:
   - transforma `ai-queued` en `ai-running`;
   - crea o reutiliza rama;
   - prepara prompt desde Issue, comentarios y politicas;
   - ejecuta Codex o Claude;
   - rechaza rutas prohibidas;
   - hace commit y push si hay cambios;
   - crea o reutiliza PR;
   - comenta la Issue con `[AI CODEX]` o `[AI CLAUDE]`;
   - quita `ai-running` solo si la PR queda creada correctamente.
8. La PR ejecuta CI:
   - secret scan;
   - dependency audit;
   - typecheck;
   - lint;
   - tests;
   - build;
   - integration/E2E cuando existan.
9. Si CI pasa, la PR queda lista para review humano.
10. Al mergear:
   - se cierra la Issue si la PR usa `Closes #<id>`;
   - se ejecuta deployment `preview` si procede;
   - se publica evidencia en la Issue o en la PR.

## 5. Trabajo que se puede adelantar ahora

Aunque aun no existan los proyectos de aplicacion, se puede preparar la gobernanza del repo.

### 5.1 GitHub repo settings

1. Confirmar propietario y repo definitivo, por ejemplo `domingo-intrasoft/iLiniumTech`.
2. Activar GitHub Issues.
3. Activar GitHub Actions.
4. Crear o confirmar rama por defecto `main`.
5. Activar "Automatically delete head branches" tras merge.
6. Limitar metodos de merge:
   - recomendado: squash merge para PRs IA;
   - opcional: merge commit para PRs de integracion grandes.
7. Desactivar pushes directos a `main` cuando existan checks estables.
8. Crear branch protection o ruleset para `main`:
   - Require pull request before merging.
   - Require approvals: minimo 1.
   - Dismiss stale approvals on new commits.
   - Require conversation resolution.
   - Require status checks cuando ya existan.
   - Restrict force pushes.
   - Restrict deletions.
9. No activar required checks hasta que el primer workflow haya corrido al menos una vez, porque GitHub necesita conocer los nombres de checks.

### 5.2 GitHub Project

Crear un Project para iLiniumTech con estas columnas/Status:

- `Triage`
- `Ready for AI`
- `Doing`
- `PR Created`
- `Validated`
- `Ready for Release`
- `Ready for Test`
- `Blocked`
- `Done`

Campos recomendados:

- `App`: `Core`, `Extractor`, `Renderer`, `API`, `Frontend`, `Infrastructure`, `Docs`.
- `AI Engine`: `codex`, `claude`, `none`.
- `Risk`: `low`, `medium`, `high`, `security-sensitive`.
- `Environment`: `none`, `local`, `preview`, `production`.
- `Spec`: enlace o id de spec SDD.
- `Validation`: `not-started`, `ci-pending`, `ci-passed`, `ci-failed`, `manual-needed`.

Automatizaciones del Project:

1. Nueva Issue con label `ai-candidate`: Status `Triage`.
2. Issue con label `ai-ready`: Status `Ready for AI`.
3. PR abierto con `Closes #id`: Status `PR Created`.
4. PR mergeado: Issue `Done`.
5. Issue con `needs-human`: Status `Blocked`.

### 5.3 Labels

Crear estas labels base:

Motor:

- `ai-codex`
- `ai-claude`
- `ai-codex-web`
- `ai-claude-web`

Estado:

- `ai-candidate`
- `ai-ready`
- `ai-queued`
- `ai-running`
- `needs-human`
- `validated`
- `ready-for-release`
- `preview-ready`
- `done`

Intencion y riesgo:

- `ai-docs`
- `ai-analysis`
- `ai-low-risk`
- `ai-needs-review`
- `ai-platform-change`
- `ai-security-sensitive`
- `ai-pipeline-failure`
- `ai-repair`
- `automation`

Apps/areas iniciales:

- `app:core`
- `app:extractor`
- `app:renderer`
- `app:api`
- `app:frontend`
- `app:infrastructure`
- `app:docs`

Labels tecnicas para CI:

- `ci-failure`
- `dependency`
- `security`
- `e2e`
- `blocked-by-env`

Comandos orientativos con GitHub CLI:

```powershell
gh label create ai-ready --color 0E8A16 --description "Lista para ejecucion asistida"
gh label create ai-running --color FBCA04 --description "Automatizacion IA en curso"
gh label create needs-human --color B60205 --description "Requiere decision humana"
gh label create ai-codex --color 5319E7 --description "Resolver con Codex"
gh label create ai-claude --color 5319E7 --description "Resolver con Claude"
gh label create app:infrastructure --color C5DEF5 --description "Automatizacion, CI/CD, seguridad o plataforma"
```

### 5.4 Issue templates

Crear `.github/ISSUE_TEMPLATE/` con:

1. `sdd-feature.yml`
   - objetivo funcional;
   - contexto AppBuilder;
   - alcance;
   - fuera de alcance;
   - criterios de aceptacion;
   - datos/configuracion necesarios;
   - app/area;
   - validacion esperada;
   - riesgo de seguridad.
2. `ai-task.yml`
   - tarea tecnica;
   - motor sugerido;
   - rutas permitidas;
   - rutas prohibidas;
   - validacion;
   - necesita override: si/no.
3. `bug.yml`
   - comportamiento actual;
   - comportamiento esperado;
   - pasos para reproducir;
   - logs sanitizados;
   - impacto.
4. `pipeline-failure.yml`
   - workflow;
   - run URL;
   - job/step;
   - firma de fallo;
   - resumen sanitizado;
   - reintento permitido.

Tambien crear:

- `.github/pull_request_template.md`
- `.github/CODEOWNERS`
- `docs/ai/github-ai-policy.md`
- `docs/ai/ai-engine-routing.md`
- `docs/ai/apps/README.md`

### 5.5 Politica IA para GitHub

Crear `docs/ai/github-ai-policy.md` adaptando `boards-ai-policy.md`.

Cambios clave:

- "Azure Boards work item" pasa a "GitHub Issue".
- `Area Path` pasa a label `app:*` o campo `App`.
- Columna `Ready for AI` pasa a Project Status `Ready for AI`.
- Comentarios `[TECHNICAL OVERRIDE]` viven en comentarios de GitHub Issue.
- `AI_OVERRIDE_APPROVERS` debe contener GitHub logins permitidos, no identidades Azure DevOps.
- La PR final es GitHub PR contra `main`.
- No hay retorno a Azure DevOps.
- No hay `ADO_PAT`.

Formato de override recomendado:

```text
[TECHNICAL OVERRIDE]
Scope: ruta, modulo, motor o regla concreta
Allow: permiso concreto, por ejemplo tocar .github/workflows/ci.yml
ValidFor: Issue #123
Reason: motivo operativo
Validation: prueba obligatoria
ApprovedBy: @github-login
```

Formato de aprobacion de plataforma:

```text
[AI PLATFORM APPROVAL]
Allow: rutas o modulos autorizados
ValidFor: Issue #123
Reason: motivo operativo
Risk: impacto esperado
Validation: smoke, workflow o comprobacion obligatoria
ApprovedBy: @github-login
```

## 6. Secretos, variables y environments

### 6.1 Repository secrets iniciales

Crear solo cuando exista valor real:

- `OPENAI_API_KEY`: requerido para Codex Action o Codex CLI en Actions.
- `ANTHROPIC_API_KEY`: requerido para Claude Code Action.
- `GITLEAKS_LICENSE`: solo si se usa version que lo requiera.

No crear todavia secretos de BBDD, JWT, IIS o despliegue hasta saber el destino tecnico.

### 6.2 Repository variables iniciales

Variables no secretas recomendadas:

- `AI_DEFAULT_BASE_BRANCH=main`
- `AI_READY_LABEL=ai-ready`
- `AI_MAX_IN_FLIGHT=2`
- `AI_DISPATCH_TOP=5`
- `AI_OVERRIDE_APPROVERS=domingo-intrasoft`
- `AI_FORBIDDEN_PATHS=docs/engineering/**,.gitleaks.toml,tools/security/**,.github/workflows/security.yml`
- `DEPLOY_PREVIEW_ENABLED=false`

Nota: si `AI_OVERRIDE_APPROVERS` se considera sensible por politica interna, moverlo a secret.

### 6.3 Environments

Crear estos GitHub Environments:

1. `ci`
   - sin secretos al principio.
   - sirve para aislar variables de validacion si hiciera falta.
2. `preview`
   - required reviewers: 1 persona.
   - secrets futuros de despliegue de prueba.
   - URL de entorno cuando exista.
   - branch policy: solo `main` o releases.
3. `production`
   - no crear con secretos hasta que exista despliegue real.
   - required reviewers: minimo 1 o equipo.
   - sin secretos hasta que exista produccion real.

Regla: los secretos de entorno solo los leen jobs que declaren `environment: preview` o `environment: production`.

## 7. Workflows a crear

### 7.1 `.github/workflows/ci.yml`

Objetivo: validacion general para PRs y push a `main`.

Triggers:

- `pull_request`
- `push` a `main`
- `workflow_dispatch`

Fase inicial sin proyectos:

- checkout;
- validar que docs y plantillas tienen formato basico;
- ejecutar gitleaks si el binario/script esta disponible;
- no fallar por ausencia de apps.

Cuando exista primera app:

- detectar apps por `package.json`, `.sln`, `.csproj` o registry `.github/apps.yml`;
- ejecutar matriz por app afectada;
- publicar JUnit/TRX;
- publicar cobertura si existe;
- publicar artifacts solo cuando tenga sentido.

Jobs futuros:

- `secret-scan`
- `dependency-audit`
- `frontend`
- `backend`
- `integration`
- `e2e-smoke`

### 7.2 `.github/workflows/security.yml`

Objetivo: seguridad transversal.

Triggers:

- `pull_request`
- `push` a `main`
- `schedule` diario o semanal
- `workflow_dispatch`

Checks:

- Gitleaks bloqueante.
- Dependency audit para npm y dotnet cuando existan.
- CORS audit cuando exista API.
- CodeQL default setup o workflow avanzado cuando haya lenguajes soportados.
- Validacion de que no se versionan `.env`, dumps, backups ni logs sensibles.

Estado actual:

- existe workflow de seguridad con secret scan, dependency audit y CORS audit;
- se ejecuta en PR, push a `main`, `workflow_dispatch` y schedule semanal;
- publica artefactos bajo `security-reports/**` cuando los scripts locales generan reportes;
- no depende de secretos reales.

### 7.2.1 `.github/workflows/codeql.yml`

Objetivo: analisis estatico CodeQL para backend .NET y frontend TypeScript sin depender de secretos ni despliegue.

Fase inicial:

- `pull_request`, `push` a `main`, schedule semanal y `workflow_dispatch`;
- jobs separados para `csharp` y `javascript-typescript`;
- permisos minimos: `contents: read`, `actions: read`, `security-events: write`;
- .NET se prepara solo para el analisis C#;
- no ejecuta despliegues ni lee secrets de entorno.

Bloqueo externo:

- la publicacion de alertas requiere que GitHub Code Scanning este disponible para el repositorio y que los checks pasen al menos una vez antes de marcarlos como required.

### 7.3 `.github/workflows/ai-issue-dispatcher.yml`

Equivalente GitHub-only de `azure-pipelines-ai-dispatcher.yml`.

Triggers:

- `schedule`: cada 10 minutos o cada 15 si se quiere reducir ruido.
- `workflow_dispatch` con inputs:
  - `execute`: boolean.
  - `top`: numero.
  - `issueNumber`: opcional para probar una sola Issue.
  - `diagnosticsOnly`: boolean.

Responsabilidades:

1. Buscar Issues abiertas candidatas:
   - label `ai-ready`;
   - sin `needs-human`;
   - sin `ai-running`;
   - sin `ai-queued`;
   - sin `validated`;
   - sin `preview-ready`.
2. Validar que hay un unico motor.
3. Validar label `app:*`.
4. Validar que existe `docs/ai/apps/<app>.md`.
5. Bloquear si `ai-security-sensitive` usa motor web/cloud sin override.
6. Bloquear si `ai-platform-change` no tiene `[AI PLATFORM APPROVAL]`.
7. Reservar Issue:
   - anadir `ai-queued`;
   - comentar `[AI DISPATCHER]`.
8. Disparar workflow del motor con `workflow_dispatch`.
9. Watchdogs:
   - Issues con `ai-queued` antiguas.
   - Issues con `ai-running` antiguas.
   - PRs abiertas sin checks recientes.

No debe:

- limpiar `ai-running` automaticamente;
- cerrar Issues;
- hacer merge;
- desplegar.

### 7.4 `.github/workflows/ai-codex-workitem.yml`

Equivalente GitHub-only de `codex-web-workitem.yml`, sin retorno a ADO.

Trigger:

- `workflow_dispatch`

Inputs:

- `issueNumber`
- `sourceBranch`
- `baseBranch`
- `app`
- `title`

Permisos:

- `contents: write`
- `issues: write`
- `pull-requests: write`
- `actions: read`

Pasos:

1. Validar inputs.
2. Checkout de `baseBranch`.
3. Crear o cambiar a `sourceBranch`.
4. Leer Issue y comentarios relevantes.
5. Construir prompt con:
   - politica general;
   - politica de app;
   - descripcion de Issue;
   - comentarios `[TECHNICAL OVERRIDE]` validos;
   - rutas permitidas/prohibidas;
   - validacion esperada.
6. Anadir `ai-running`, quitar `ai-queued`.
7. Ejecutar `openai/codex-action@v1`.
8. Detectar cambios.
9. Rechazar rutas prohibidas.
10. Ejecutar validaciones obvias si el cambio toca una app con comandos definidos.
11. Commit y push.
12. Crear o reutilizar PR hacia `baseBranch`.
13. Comentar Issue:
    - rama;
    - PR;
    - resumen;
    - checks ejecutados.
14. Etiquetar:
    - quitar `ai-running`;
    - anadir `ai-needs-review` o mover Project a `PR Created`.

Si no hay cambios:

- comentar `[AI CODEX] sin cambios`;
- quitar `ai-running`;
- anadir `needs-human`;
- fallar workflow para visibilidad.

### 7.5 `.github/workflows/ai-claude-workitem.yml`

Equivalente del anterior usando `anthropics/claude-code-action@v1`.

Diferencias:

- requiere `ANTHROPIC_API_KEY`;
- permite `id-token: write` si la action lo requiere;
- limitar herramientas permitidas;
- usar `--max-turns` conservador;
- mismo contrato de rama, PR y comentarios.

### 7.6 `.github/workflows/pr-validation.yml`

Puede ser parte de `ci.yml` o workflow separado.

Objetivo:

- checks requeridos para PRs.
- nombres de jobs estables y unicos.
- matriz por app cuando exista.

Convencion de checks:

- `security / secret-scan`
- `security / dependency-audit`
- `ci / frontend`
- `ci / backend`
- `ci / integration`
- `ci / e2e-smoke`

### 7.7 `.github/workflows/deploy-preview.yml`

Objetivo: despliegue preview controlado desde GitHub.

Fase inicial:

- `workflow_dispatch`;
- input `dryRun=true`;
- environment `preview`;
- no despliega si `DEPLOY_PREVIEW_ENABLED=false`;
- valida que no faltan variables basicas.

Estado actual seguro:

- existe `.github/workflows/preview-dry-run.yml`;
- solo se ejecuta manualmente con `workflow_dispatch`;
- `DEPLOY_PREVIEW_ENABLED` queda fijado a `false` dentro del job;
- falla si se intenta ejecutar con `dryRun=false`;
- genera artefacto `preview-dry-run-report` con el plan y bloqueos;
- no declara environment ni consume secrets hasta que `preview` tenga destino, reviewers y configuracion aprobada.

Cuando exista app y destino:

- descargar/build artifacts;
- desplegar a destino elegido;
- ejecutar smoke Playwright;
- publicar URL y evidencia.

Destino arquitectonico decidido:

- frontend Vue publicado como build estatico;
- backend API de datos desplegado como servicio .NET;
- extractor de metadata fuera del runtime productivo.

Destinos posibles por concretar:

- GitHub Pages o Azure Static Web Apps para frontend estatico.
- Azure App Service, IIS/Windows mediante self-hosted runner o contenedor para backend API.
- La eleccion final depende de infraestructura disponible, secretos y requisitos de red/BBDD.

### 7.8 `.github/workflows/pipeline-failure-reporter.yml`

Equivalente GitHub-only de `report-pipeline-failure.yml`.

Trigger recomendado:

- `workflow_run` para workflows criticos: `ci`, `security`, `deploy-preview`.

Responsabilidades:

1. Si el workflow concluye `failure`, `cancelled` o `timed_out`, leer metadata del run.
2. Descargar logs si GitHub token lo permite.
3. Sanitizar logs.
4. Calcular firma:
   - workflow;
   - job;
   - step;
   - categoria;
   - error normalizado.
5. Buscar Issue abierta con label `pipeline-failure-<hash>`.
6. Si existe, comentar run nuevo.
7. Si no existe, crear Issue tecnica:
   - label `ai-pipeline-failure`;
   - label `ai-repair`;
   - label `automation`;
   - label `app:infrastructure`;
   - label `needs-human` si falta informacion o parece secreto/infra.
8. No relanzar indefinidamente.

Regla inicial:

- No autorreparar deployments, secretos ni permisos.
- Solo permitir IA en fallos de codigo, tests, YAML no sensible o dependencias.

## 8. Registro de apps

Como los proyectos aun no existen, crear una convencion desde el inicio.

Archivo recomendado:

`docs/ai/apps/README.md`

Cuando nazca una app, crear:

`docs/ai/apps/<app>.md`

Contenido minimo:

- Nombre de app.
- Rutas propiedad de la app.
- Comandos locales.
- Comandos CI.
- Tests obligatorios.
- Smoke esperado.
- Riesgos de seguridad.
- Rutas prohibidas.
- Entorno `preview` si aplica.

Opcionalmente crear `.github/apps.yml`:

```yaml
apps:
  core:
    paths:
      - src/core/**
      - tests/core/**
    type: dotnet
    restore: dotnet restore src/core/Core.sln
    build: dotnet build src/core/Core.sln --configuration Release --no-restore
    test: dotnet test src/core/Core.sln --configuration Release --no-build
  frontend:
    paths:
      - src/frontend/**
    type: node
    install: npm ci
    lint: npm run lint
    typecheck: npm run typecheck
    test: npm run test:unit:ci
    build: npm run build
```

El dispatcher y CI pueden leer este registro para saber que validar.

## 9. Politica de gates por fases

Fase 0: solo documentacion y configuracion.

- CI no debe exigir apps.
- Secret scan si existe.
- Validacion de docs y plantillas.

Fase 1: primera app reproducible.

- lockfile versionado.
- `npm ci` o `dotnet restore`.
- build local documentado.
- CI ejecuta build.

Fase 2: calidad basica.

- typecheck.
- lint.
- unit tests.
- JUnit/TRX publicado.

Fase 3: integracion.

- API en memoria.
- BBDD de test controlada.
- Testcontainers si hace falta.

Fase 4: E2E preview.

- Playwright smoke.
- screenshots/traces en fallos.
- environment `preview` protegido.

Fase 5: seguridad bloqueante.

- Gitleaks obligatorio.
- dependency audit high/critical bloqueante.
- CodeQL.
- CORS audit si hay API.

Regla de activacion:

- Un gate no debe ser required check hasta que pase estable en la app piloto.
- Secretos reales detectados bloquean siempre.

## 10. Checklist inmediata de implantacion

Orden recomendado:

1. Confirmar que `main` sera rama por defecto y unica rama permanente.
2. Crear labels base.
3. Crear GitHub Project y Status.
4. Crear issue forms.
5. Crear PR template y CODEOWNERS.
6. Crear `docs/ai/github-ai-policy.md`.
7. Crear `docs/ai/ai-engine-routing.md`.
8. Crear `docs/ai/apps/README.md`.
9. Crear secrets `OPENAI_API_KEY` y/o `ANTHROPIC_API_KEY` solo si se van a ejecutar motores desde Actions.
10. Crear variables `AI_DEFAULT_BASE_BRANCH`, `AI_MAX_IN_FLIGHT`, `AI_OVERRIDE_APPROVERS`.
11. Crear environments `ci` y `preview`.
12. Crear `ci.yml` en modo no-op seguro.
13. Crear `security.yml` con gitleaks si el script esta listo.
14. Crear `codeql.yml` sin secrets y esperar primera ejecucion verde antes de hacerlo required.
15. Sustituir placeholders de `.github/CODEOWNERS` por usuarios o equipos reales antes de exigir CODEOWNERS en branch protection.
16. Crear `preview-dry-run.yml` manual y bloqueado por defecto.
17. Crear `ai-issue-dispatcher.yml` primero en modo diagnostico.
18. Crear `ai-codex-workitem.yml` con `workflow_dispatch`, sin schedule.
19. Probar con una Issue documental y label `ai-codex`.
20. Revisar que crea rama y PR, pero no hace merge.
21. Activar branch protection con checks que ya hayan corrido.
22. Activar dispatcher programado con `top=1`.
23. Subir a `top=5` solo tras varios ciclos estables.

## 11. Checklist para la primera app que se cree

Cuando exista el primer proyecto:

1. Crear estructura de app.
2. Crear README local con comandos.
3. Crear lockfile si hay Node.
4. Crear solucion/proyecto si hay .NET.
5. Crear test minimo.
6. Crear politica `docs/ai/apps/<app>.md`.
7. Registrar app en `.github/apps.yml` si se adopta.
8. Actualizar `ci.yml` para detectar la app.
9. Ejecutar CI en PR.
10. Activar checks required solo cuando pasen.
11. Crear smoke E2E si hay UI.
12. Definir despliegue `preview`.
13. Mover secretos de prueba al environment `preview`.
14. Activar `deploy-preview.yml` con aprobacion manual.

## 12. Diferencias exactas frente a AcademiaLasCortes/demo

| AcademiaLasCortes/demo | iLiniumTech GitHub-only |
| --- | --- |
| Azure Boards Work Item | GitHub Issue |
| Azure Area Path | label `app:*` o Project field `App` |
| Azure column `Ready for AI` | GitHub Project Status `Ready for AI` |
| Azure dispatcher pipeline | GitHub Actions `ai-issue-dispatcher.yml` |
| Azure runner local | GitHub Actions motor o runner self-hosted si hace falta |
| Bridge DevOps -> GitHub | no existe |
| GitHub mirror branch | no existe |
| `AI_GITHUB_BRIDGE_TOKEN` | no hace falta para repo unico |
| `ADO_PAT` | no hace falta |
| `return-ai-web-to-ado.yml` | no existe |
| Azure DevOps PR final | GitHub PR final |
| Azure finalizer | branch protection, checks y environment de GitHub |
| Variable group Azure | GitHub Secrets, Variables y Environments |
| Azure deployment pool | GitHub-hosted o self-hosted runner |

## 13. Riesgos y mitigaciones

Riesgo: el dispatcher encola la misma Issue dos veces.  
Mitigacion: reservar con label `ai-queued`, usar `concurrency` por Issue y revalidar justo antes de ejecutar.

Riesgo: una IA toca rutas de plataforma sin permiso.  
Mitigacion: lista de rutas prohibidas, override validado y job `Reject forbidden paths`.

Riesgo: `GITHUB_TOKEN` no dispara workflows encadenados por push.  
Mitigacion: usar `workflow_dispatch` explicito para encadenar workflows cuando haga falta, igual que aprendio Academia con el retorno.

Riesgo: checks required bloquean todo por nombres inestables.  
Mitigacion: nombres de jobs estables y unicos; activar required checks solo tras primera ejecucion correcta.

Riesgo: secretos disponibles en jobs que no deben verlos.  
Mitigacion: secrets de environment y jobs con `environment` solo cuando sea necesario.

Riesgo: no hay proyectos todavia y CI se vuelve falso positivo.  
Mitigacion: modo bootstrap que valida documentacion/seguridad y no exige apps hasta registrarlas.

Riesgo: costes o consumo excesivo de IA.  
Mitigacion: `AI_MAX_IN_FLIGHT`, `top`, labels manuales, modo diagnostico y schedule conservador.

Riesgo: autorreparacion de pipeline crea bucles.  
Mitigacion: firma de fallo, deduplicacion, un solo reintento y `needs-human` en fallos de entorno/secretos/permisos.

## 14. Smoke test de la automatizacion IA

Prueba 1: diagnostico sin ejecutar.

1. Crear Issue documental con label `ai-candidate`.
2. Ejecutar `ai-issue-dispatcher.yml` con `diagnosticsOnly=true`.
3. Confirmar que aparece como candidata o que explica por que no lo es.

Prueba 2: Codex en tarea documental.

1. Crear Issue con alcance limitado: actualizar un README o doc.
2. Labels: `ai-ready`, `ai-codex`, `ai-docs`, `ai-low-risk`, `app:docs`.
3. Ejecutar dispatcher manual con `top=1`.
4. Confirmar:
   - label `ai-queued`;
   - workflow Codex lanzado;
   - rama `codex/gh-<issue>-...`;
   - PR creada;
   - comentario estructurado;
   - sin merge automatico.

Prueba 3: bloqueo sensible.

1. Crear Issue que pida tocar `.github/workflows/security.yml`.
2. Labels: `ai-ready`, `ai-codex`, `ai-platform-change`, `app:infrastructure`.
3. Sin `[AI PLATFORM APPROVAL]`.
4. Confirmar que dispatcher bloquea con `needs-human`.

Prueba 4: deployment preview dry run.

1. Ejecutar `deploy-preview.yml` con `dryRun=true`.
2. Confirmar que pide environment `preview` si tiene reviewers.
3. Confirmar que no accede a secretos si `DEPLOY_PREVIEW_ENABLED=false`.

## 15. Referencias oficiales usadas para el diseno

- GitHub Actions `workflow_dispatch`: https://docs.github.com/actions/learn-github-actions/workflow-syntax-for-github-actions#onworkflow_dispatch
- GitHub Actions scheduled workflows: https://docs.github.com/actions/learn-github-actions/workflow-syntax-for-github-actions#onschedule
- `GITHUB_TOKEN`: https://docs.github.com/actions/concepts/security/github_token
- GitHub Actions secrets: https://docs.github.com/en/actions/concepts/security/secrets
- Uso de secrets en workflows: https://docs.github.com/en/actions/how-tos/security-for-github-actions/security-guides/using-secrets-in-github-actions
- Environments y deployment protection rules: https://docs.github.com/en/actions/concepts/workflows-and-actions/deployment-environments
- Branch protection: https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/managing-a-branch-protection-rule
- Issue forms: https://docs.github.com/communities/using-templates-to-encourage-useful-issues-and-pull-requests/syntax-for-issue-forms
- Labels: https://docs.github.com/en/issues/using-labels-and-milestones-to-track-work/managing-labels
- GitHub Projects built-in automations: https://docs.github.com/en/issues/planning-and-tracking-with-projects/automating-your-project/using-the-built-in-automations
- Code scanning default setup: https://docs.github.com/en/code-security/concepts/code-scanning/setup-types
