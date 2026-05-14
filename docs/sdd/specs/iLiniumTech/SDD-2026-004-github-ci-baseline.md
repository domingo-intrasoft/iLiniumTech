# SDD: Baseline CI/CD GitHub-only

## Metadata

- Spec ID: SDD-2026-004
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: infra
- Tamano SDD: S
- Estado SDD: implementado con pendientes de activacion externa
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-13

## Contexto

La documentacion define un flujo GitHub-only con `main` como rama permanente, issues SDD, PRs cortas y gates incrementales. El repo ya tiene backend, frontend y scripts de seguridad, pero no tenia carpeta `.github`.

## Objetivo

Crear una base de GitHub Actions y plantillas para validar build, tests, lint, seguridad y trazabilidad SDD en cada PR.

## Fuera de alcance

- Dispatcher AI automatico con schedule.
- Ejecucion de Codex o Claude en Actions.
- Despliegue preview real.
- Environments protegidos con secretos.
- Merge automatico.

## Contrato de datos

Entradas:

- PR contra `main`;
- push a `main`;
- ejecucion manual `workflow_dispatch`.

Salidas:

- checks de backend;
- checks de frontend;
- audit de dependencias;
- audit CORS;
- secret scan;
- artefactos de test y seguridad cuando existan.

## Reglas de negocio

- Los workflows no requieren secretos reales.
- Node en CI debe ser compatible con `package.json`.
- .NET en CI debe estar alineado con la solucion actual.
- Los checks deben poder reproducirse localmente.
- `format` queda como check manual hasta estabilizar Prettier en el frontend existente.
- Cualquier cambio posterior en `.github/workflows/**` requiere revision humana si lo ejecuta IA.

## Criterios de aceptacion

- [x] Existe workflow CI para backend y frontend.
- [x] Existe workflow de seguridad.
- [x] Existe plantilla de PR con pruebas y seguridad.
- [x] Existe issue form SDD.
- [x] Existe politica AI GitHub-only versionada.
- [x] Los comandos equivalentes pasan localmente.
- [x] Gate local MVP documentado y actualizado con backend, frontend, smoke, extractor, auditorias, validacion documental y `git diff --check`.

## Impacto tecnico

- `.github/workflows/ci.yml`.
- `.github/workflows/security.yml`.
- `.github/pull_request_template.md`.
- `.github/ISSUE_TEMPLATE/**`.
- `docs/ai/**`.

## Seguridad

- [x] No se anaden secretos.
- [x] Los workflows no imprimen variables sensibles.
- [x] Secret scan se ejecuta en PR y push.
- [x] Auditoria de dependencias falla si hay findings.
- [x] CORS audit falla si hay configuracion permisiva.

## Plan de pruebas

- Unitarias: no aplica.
- Integracion: ejecutar comandos locales equivalentes.
- E2E/smoke: no aplica.
- Seguridad: ejecutar scripts de seguridad locales.
- Manual/UAT: crear una PR de prueba y revisar checks en GitHub.

## Estado de cierre QA/UAT

Completado con evidencia:

- CI y seguridad baseline existen en `.github/workflows`.
- Gate local `tools/quality/Invoke-MvpQualityGate.ps1` agrupa checks backend/frontend, smoke, extractor, seguridad, validacion documental y `git diff --check`.
- Validacion documental local existe en `tools/quality/Test-DocumentationBaseline.ps1`.
- Roadmap y DoD enlazan el gate y los comandos granulares.

Pendiente tecnico:

- Ejecutar gate completo como evidencia de cada rama que toque runtime, seguridad o configuracion.
- Revisar primera ejecucion GitHub verde antes de convertir checks en required.

Bloqueado externo:

- Activar branch protection, Code Scanning required y preview real depende de configuracion GitHub/environments por responsables humanos.

## Riesgos

- `gitleaks/gitleaks-action` puede requerir ajuste si cambia su interfaz.
- GitHub-hosted runners pueden no tener exactamente el mismo SDK que local.
- La activacion de required checks debe esperar a que la primera PR pase de forma estable.

## Work Items

- Crear issue GitHub: `SDD-2026-004 Baseline CI/CD GitHub-only`.
- Crear issue futuro: dispatcher AI diagnostico.
- Crear issue futuro: preview dry run.

## Definicion de hecho

- [x] Criterios de aceptacion completados.
- [x] Pruebas ejecutadas y documentadas.
- [x] Gates de seguridad aplicables ejecutados.
- [x] Documentacion actualizada.
