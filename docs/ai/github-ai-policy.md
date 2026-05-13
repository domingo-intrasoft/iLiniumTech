# Politica AI GitHub-only

Fecha: 2026-05-13

## Objetivo

Usar GitHub Issues, ramas cortas, PRs y Actions como ciclo completo de trabajo para iLiniumTech, sin Azure DevOps como autoridad final.

## Elegibilidad de una issue

Una issue puede ser ejecutada por IA solo si:

- esta abierta;
- referencia una spec SDD versionada;
- tiene alcance verificable;
- tiene labels `ai-ready` y un unico motor;
- no tiene `needs-human`, `ai-running`, `ai-queued`, `validated`, `preview-ready` ni `done`;
- declara pruebas esperadas;
- declara impacto de seguridad si toca secretos, BBDD, SQL, auth, workflows, pipelines o datos personales.

## Motores permitidos

- `ai-codex`: trabajo local o GitHub Actions con Codex.
- `ai-claude`: trabajo local o GitHub Actions con Claude.
- `ai-codex-web`: trabajo web cuando el alcance este acotado y no requiera secretos locales.
- `ai-claude-web`: trabajo web cuando el alcance este acotado y no requiera secretos locales.

Debe existir exactamente un label de motor por issue.

## Labels de estado

- `ai-ready`: lista para dispatcher.
- `ai-queued`: reservada por dispatcher.
- `ai-running`: en ejecucion.
- `needs-human`: requiere decision humana.
- `validated`: validada por humano o gates.
- `preview-ready`: lista para prueba preview.
- `done`: cerrada y aceptada.

## Ramas

- Codex: `codex/gh-<issue>-<slug>`.
- Claude: `claude/gh-<issue>-<slug>`.
- Funcionalidad humana: `feature/gh-<issue>-<slug>`.
- Bugs: `fix/gh-<issue>-<slug>`.
- Infraestructura: `infra/gh-<issue>-<slug>`.
- Documentacion: `docs/gh-<issue>-<slug>`.

Toda rama que aspire a entrar en producto abre PR contra `main`.

## Rutas sensibles

Requieren aprobacion humana explicita en la issue o PR:

- `.github/workflows/**`;
- `tools/security/**`;
- `.gitleaks.toml`;
- archivos de configuracion de autenticacion, autorizacion o CORS;
- codigo que lea BBDD o ejecute SQL dinamico;
- codigo de despliegue o gestion de secretos.

La aprobacion debe aparecer como comentario con el texto `[AI PLATFORM APPROVAL]` e indicar rutas autorizadas.

## Reglas de seguridad

- Nunca incluir connection strings, passwords, tokens, API keys ni dumps.
- Nunca incluir datos personales reales en fixtures, tests, capturas o artefactos.
- SQL estructural solo desde whitelist y valores siempre parametrizados.
- Auth por defecto; endpoints anonimos solo si estan documentados.
- CORS por `Cors:AllowedOrigins`.
- Dependencias nuevas requieren justificacion en spec o ADR.

## Cierre de una tarea

Cada PR debe indicar:

- spec o issue;
- comandos ejecutados;
- pruebas no ejecutadas y motivo;
- riesgos residuales;
- impacto de seguridad;
- evidencia visual si toca UI.
