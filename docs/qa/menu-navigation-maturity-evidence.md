# Evidencia QA - estados de menu y scopes Polizas

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Incremento frontend/documental para hacer visible el estado de madurez de las entradas del menu lateral y corregir la trazabilidad documental de `Polizas / Flotas` y `Polizas / Colectivas`.

Fuera de alcance:

- No se crean APIs nuevas.
- No se conectan datos reales.
- No se reactiva `Autos Particulares`.
- No se cambian reglas backend, permisos efectivos, SQL, workflows ni metadata runtime.

## Cambios validados

- `Polizas` queda clasificada como `Operativo`: MVP read-only con contrato iLiniumTech existente.
- Entradas con fixture local quedan clasificadas como `Fixture`.
- Entradas tecnicas o sin SDD quedan clasificadas como `Bloqueado SDD`.
- `Autos Particulares` queda clasificada como `Aparcado` y sigue deshabilitada en el menu.
- `Flotas` y `Colectivas` siguen navegables como scopes estaticos, pero bloqueadas para datos reales hasta SDD, regla funcional, permisos, DBA/UAT y API explicita.
- La documentacion del menu ya no afirma que solo `/polizas` y `/autos-particulares` tienen ruta real.

## Comandos ejecutados

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
```

Resultado:

- Formato correcto.
- Lint correcto.
- Unit frontend: 39 archivos, 185 tests superados.
- Build frontend correcto.
- E2E frontend: 2 tests superados.

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
git diff --check
```

Resultado:

- Backend: 88 tests superados.
- Baseline documental correcto.
- Secret scan sin leaks.
- `git diff --check` sin errores; solo avisos locales de normalizacion LF/CRLF.

## Incidencia encontrada y resuelta

El primer `npm run test:e2e` fallo porque el indicador accesible de estado alteraba el nombre del enlace `Polizas`. Se corrigio para que el enlace mantenga su nombre accesible original y el estado quede en indicador visual/tooltip. Tras la correccion, `npm run test:e2e` paso correctamente.

## Riesgos residuales

- Los indicadores de madurez son orientativos de MVP; no sustituyen permisos reales backend.
- `Flotas` y `Colectivas` siguen sin regla funcional ni contrato de datos.
- `Autos Particulares` conserva codigo/ruta tecnica, pero continua aparcada por decision de producto.
- Las entradas `Bloqueado SDD` no deben avanzar a datos reales sin SDD, threat review si aplica, permisos y UAT.
