# Evidencia QA - MVP Polizas

Fecha: 2026-05-15

## Alcance del incremento

- La pantalla de Polizas mantiene filtros heredados visibles para trazabilidad visual, pero solo deja operativos los campos incluidos en el contrato API actual.
- Los filtros fuera de contrato quedan deshabilitados y marcados como pendientes de contrato API, sin consultar metadata AppBuilder en runtime.
- La busqueda se bloquea cuando falta configuracion, sesion o broker requerido por el contexto de ejecucion.
- `GET /api/polizas/{id}` devuelve un error publico sanitizado con `correlationId` cuando la poliza no existe.
- El SQL de listado de Polizas usa un desempate estable por `Poliza ASC` cuando se ordena por campos no unicos.

## Evidencia funcional esperada

- En modo fixture local, `/polizas` permite buscar por poliza, situacion/tipo, compania, ramo, rango de fecha efecto y cliente basico.
- Campos como riesgo, oficina, division, colaboradores, canal, fraccion, datos personales adicionales y otros criterios heredados quedan visibles pero no operativos hasta tener contrato backend explicito.
- En modo backend, si falta API key o broker requerido, la UI no debe lanzar busquedas de Polizas.
- El detalle inexistente no debe devolver cuerpo vacio ni filtrar SQL, nombres de tabla, connection strings o trazas internas.

## Pruebas enfocadas ejecutadas

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter "FullyQualifiedName~PolizasApiTests|FullyQualifiedName~PolizasSqlQueryBuilderTests"
```

Resultado: 45 pruebas superadas.

```powershell
npm run test:unit -- PolizasFilters PolizasView
```

Resultado: 6 pruebas superadas.

## Gate completo ejecutado

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -RunFrontendE2E
```

Resultado:

- Backend restore/build/tests: 69 pruebas superadas.
- Frontend format/lint/unit/build: 60 pruebas unitarias superadas.
- Frontend E2E Playwright: 2 pruebas superadas.
- Backend HTTP smoke y frontend smoke superados.
- Secret scan sin leaks.
- Dependency audit sin findings.
- CORS audit sin findings.
- Polizas metadata extractor tests superados.
- Documentation baseline y `git diff --check` superados.

## Pendiente externo

- Mantener UAT contra BBDD real como bloqueo externo hasta disponer de entorno autorizado.
