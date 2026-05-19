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

## Riesgos residuales

- Agenda permite delete logico solo para filas `ILMVP-AGE-*`; no hay borrado funcional historico.
- UAT/DBA deben confirmar si `IdPrioridad` debe usar catalogos reales.
- Clientes queda como siguiente vertical, pero requiere transaccion `Identidad` + `IdentidadCliente` y reglas PII.
