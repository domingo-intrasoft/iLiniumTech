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
- revision de configuracion si se tocan pipelines o despliegues.

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
