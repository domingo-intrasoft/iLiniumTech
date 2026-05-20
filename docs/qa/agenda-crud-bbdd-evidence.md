# Agenda CRUD BBDD - evidencia

Fecha: 2026-05-19

## Alcance validado

- Backend Agenda expone CRUD con permisos `agenda.create`, `agenda.update`, `agenda.delete`.
- Escrituras protegidas por `Agenda:WritesEnabled`.
- SQL Agenda usa tabla `dbo.Agenda`, `BrokerIntegracionId`, transacciones y parametros.
- Frontend Agenda usa API cuando `VITE_USE_BACKEND=true` y conserva fixture cuando esta desactivado.

## Comandos ejecutados

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter AgendaApiTests
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- AgendaView
npm run test:unit
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Resultado:

- `AgendaApiTests`: 9/9 OK.
- Backend build Release: OK.
- Backend tests completos: 183/183 OK.
- `AgendaView`: 4/4 OK.
- Frontend tests completos: 201/201 OK.
- Frontend format/lint/build: OK.
- Documentation baseline: OK.
- Secret scan: OK.
- `git diff --check`: OK.

## Smoke SQL local

Pendiente en esta evidencia porque no hay connection string de modelo Agenda/AppBuilderMaster configurada en variables de entorno del proceso actual. Solo se detecto `ConnectionStrings__DefaultConnection`, que apunta a otra BBDD no modelo.

La siguiente tarea activa debe ejecutar el smoke con secretos locales fuera de Git:

```powershell
$env:Agenda__Repository = "Sql"
$env:Agenda__WritesEnabled = "true"
# configurar ConnectionStrings__AgendaModel o ConnectionStrings__AppBuilderMaster fuera de Git
```

## Actualizacion T-200B 2026-05-19

Estado: `SKIPPED_ENV_MISSING`.

Se revisaron solo nombres de variables, nunca valores, en ambitos `Process`, `User` y `Machine`.

No se detectaron nombres compatibles con:

- `Agenda__*`
- `ConnectionStrings__Agenda*`
- `ConnectionStrings__AppBuilderMaster`
- `ILINIUMTECH__AGENDA*`
- `ILINIUMTECH__APPBUILDER_MASTER*`
- `AppBuilder__EncryptionKey`

Por tanto no se ejecuta smoke real para evitar pedir, imprimir o versionar secretos. La API y frontend Agenda siguen code-ready segun esta evidencia; el smoke real debe repetirse cuando exista configuracion local fuera de Git.

## Riesgos residuales

- Agenda permite delete logico solo para filas `ILMVP-AGE-*`; no hay borrado funcional historico.
- UAT/DBA deben confirmar si `IdPrioridad` debe usar catalogos reales.
- El smoke SQL real sigue pendiente hasta configurar secretos locales.

## Actualizacion frontend adapter - 2026-05-20

Tarea: `T-310-AGENDA-FE-CRUD-API-ADAPTER-VERIFY`

Estado: `DONE`

Resultado:

- Se confirma que el frontend de `Agenda` consume API cuando `VITE_USE_BACKEND=true`.
- Se confirma que el fixture queda solo para `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se confirma que las escrituras del adaptador no llaman API si el backend no esta activo.
- Se anade prueba unitaria del adaptador para busqueda backend, mapeo minimizado, fallback fixture y llamadas CRUD explicitas.
- No se tocan backend, SQL, repositorios, permisos, flags ni se activan escrituras reales nuevas.

Validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Agenda` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Siguiente paso operativo: `T-311-CLIENTES-FE-CRUD-API-ADAPTER-VERIFY`.
