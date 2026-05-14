# CI/CD y gates

## Orden de adopcion

Fase 1: reproducibilidad.

- lockfiles versionados;
- `npm ci` en CI;
- `dotnet restore`;
- versiones NuGet fijadas.

Fase 2: build y typecheck.

- `npm run typecheck`;
- `npm run build`;
- `dotnet build --configuration Release`.

Fase 3: lint y unit tests.

- `npm run lint`;
- `npm run test:unit`;
- `dotnet test`;
- publicacion de resultados.

Fase 4: integracion.

- API en memoria;
- BBDD de test controlada;
- Testcontainers si hace falta SQL Server;
- reportes TRX/JUnit.

Fase 5: E2E.

- smoke Playwright obligatorio antes de dar por listo el MVP visual;
- traces y screenshots en fallos;
- flujos critical cuando existan fixtures estables.

Fase 6: seguridad.

- Gitleaks bloqueante;
- auditoria de dependencias bloqueante para high/critical;
- auditoria CORS;
- CodeQL para C# y TypeScript cuando GitHub Code Scanning este disponible;
- revision de configuracion si se tocan pipelines o despliegues.

Fase 7: preview controlado.

- `preview-dry-run.yml` solo valida el plan y publica artefacto;
- `dryRun=false` debe fallar hasta que existan destino, environment `preview`, reviewers, secrets y smoke;
- ningun workflow debe desplegar a produccion desde este repositorio hasta que haya decision operativa versionada.

## Politica de activacion

Un gate no debe bloquear merge o deploy hasta que:

- exista el comando local;
- este documentado;
- pase en la app piloto;
- haya responsable de corregirlo cuando falle.

La excepcion son secretos reales: un secreto detectado debe bloquear siempre.

## Evidencias esperadas

Cada PR o ejecucion de agente debe indicar:

- comandos ejecutados;
- resultado;
- pruebas no ejecutadas y motivo;
- riesgos residuales;
- si hubo cambios de seguridad o configuracion.

## Gate local fase 7

El comando local de referencia es:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Este gate envuelve los comandos de CI actuales y anade smoke local backend/frontend y `git diff --check`. Si el Node global no coincide con CI, usar `-NodeExe` con un runtime `>=20.19.0`. El DoD operativo queda en [08-quality-phase-7-dod.md](08-quality-phase-7-dod.md).

## Checks GitHub actuales

- `CI / Documentation and hygiene`: baseline documental, `git diff --check` y artefacto `ci-quality-reports`.
- `CI / Backend build and tests`: restore, build, tests y TRX.
- `CI / Frontend checks`: `npm ci`, format, lint, tests unitarios y build.
- `Security / Secret scan`: Gitleaks.
- `Security / Dependency and CORS audit`: auditorias locales reproducibles.
- `CodeQL / Analyze`: C# y TypeScript, pendiente de confirmar disponibilidad de Code Scanning en GitHub.
- `Preview Dry Run / Preview deployment plan`: manual, sin deploy y bloqueado por `DEPLOY_PREVIEW_ENABLED=false`.

## CODEOWNERS

`.github/CODEOWNERS` marca rutas sensibles de plataforma, seguridad, calidad, SQL/backend security y gobierno documental con el placeholder `@OWNER-REVIEW-REQUIRED`.
Ese placeholder no debe considerarse enforcement real: antes de activar reglas de branch protection basadas en CODEOWNERS hay que sustituirlo por usuarios o equipos GitHub validos.

## Plantilla de pipeline futura

Cuando exista la primera app, la pipeline deberia ejecutar:

```text
checkout
secret scan
dependency audit
restore/install
typecheck
lint
unit tests
build
integration tests
E2E smoke
publish artifacts
```

No desplegar si falla seguridad, build o pruebas criticas salvo override humano documentado.
