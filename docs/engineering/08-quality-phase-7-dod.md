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
- smoke HTTP de backend con repositorio `InMemory`;
- smoke frontend sobre `dist/index.html` o sobre `-FrontendSmokeUrl` si se quiere validar un Vite/preview ya levantado;
- Gitleaks, auditoria npm/NuGet y auditoria CORS;
- `git diff --check`.

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
