# Quality gate fase 7 y DoD

Este documento fija el cierre minimo reproducible para fase 7 y da soporte a fases 1-6. La intencion es que cualquier agente o persona pueda repetir los mismos gates antes de cerrar una rama.

## Comando local unico

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Si el Node global no cumple `>=20.19.0`, pasar un runtime compatible:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\ruta\a\node.exe"
```

El gate ejecuta:

- `dotnet restore`, `dotnet build --configuration Release` y `dotnet test --configuration Release --no-build`;
- `npm ci`, `npm run format`, `npm run lint`, `npm run test:unit:ci` y `npm run build`;
- smoke HTTP de backend con repositorio `InMemory`: `/health`, `/api/me`, `/api/polizas/catalogs`, `/api/polizas`, login demo `POST /api/auth/login`, cambio de broker `POST /api/auth/broker` y logout `POST /api/auth/logout`;
- smoke frontend sobre `dist/index.html` o sobre `-FrontendSmokeUrl` si se quiere validar un Vite/preview ya levantado;
- smoke E2E de `/polizas` con Playwright Chromium cuando se invoca `-RunFrontendE2E`; usa `VITE_USE_BACKEND=false`, fixture local anonimizadas y no genera screenshots, videos ni traces;
- Gitleaks, auditoria npm/NuGet y auditoria CORS;
- validacion documental de roadmap, SDDs, CI y plantillas;
- `git diff --check`.

## Validacion CI local granular

Para cambios de plataforma/CI que no tocan backend ni frontend, ejecutar como minimo:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Si se modifica configuracion de seguridad, sumar:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

El workflow `preview-dry-run.yml` no despliega: solo comprueba que los documentos y workflows base existen y publica un artefacto de plan. Cualquier intento de `dryRun=false` debe quedar bloqueado hasta definir destino preview, environment, reviewers, secrets, smoke y rollback.

El smoke Playwright se puede ejecutar localmente desde `iLiniumTech.Frontend` con:

```powershell
npx playwright install chromium
npm run test:e2e
```

El gate local no lo ejecuta por defecto para evitar que la instalacion de navegadores bloquee entornos ligeros. Para incluirlo en cierre de rama:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -RunFrontendE2E
```

## Definicion de done

Una rama MVP no esta lista si falta alguna de estas evidencias:

- resultados de backend: restore, build y tests verdes;
- resultados de frontend: install limpio, format, lint, unit tests y build verdes;
- auditorias de seguridad sin findings bloqueantes;
- smoke local ejecutado o bloqueo tecnico documentado;
- cambios funcionales ligados a una SDD o decision documentada;
- riesgos residuales declarados en PR, issue o informe de cierre.

## Bloqueos aceptables

Solo se acepta saltar un gate cuando el bloqueo sea externo al cambio y quede escrito con causa concreta, por ejemplo:

- runtime local incompatible con el definido por CI;
- binarios bloqueados por un servidor dev ya levantado;
- navegador integrado o herramienta de smoke visual no disponible;
- dependencia externa no provisionada localmente.

Los skips de seguridad no son validos para cierre de rama salvo override humano documentado. Los secretos reales detectados bloquean siempre.
