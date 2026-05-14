# Runbook MVP local

Objetivo: levantar el MVP de polizas de forma repetible sin guardar secretos en Git.

Hay dos modos soportados para avanzar rapido:

- fixtures locales de frontend: no requiere backend ni SQL;
- backend + SQL controlado: requiere backend local, API key local de demo y conexiones reales fuera del repo.

## Checklist de variables

Valores publicos o no secretos:

- `VITE_USE_BACKEND`: activa o desactiva el consumo del backend desde Vite.
- `VITE_API_BASE_URL`: URL local del backend, por ejemplo `http://localhost:5146`.
- `VITE_BROKER_ID`: identificador de broker para contexto MVP/dev. No prueba identidad.
- `VITE_ILINIUMTECH_API_KEY`: valor incluido en el bundle frontend; solo sirve como llave local de demo/dev y no debe tratarse como secreto productivo.
- `Polizas__Repository`: selecciona `InMemory` o `Sql`.
- `Polizas__ConnectionResolver`: selecciona resolucion directa o `AppBuilderMaster`.
- `Polizas__AllowHeaderExecutionContext`: permite cabeceras MVP de contexto. Usar solo en entornos controlados.

Secretos, siempre fuera de Git:

- `ApiSecurity__ApiKey`: API key que valida el backend local para `/api/polizas/*`.
- `ConnectionStrings__PolizasReadOnly` o `ILINIUMTECH__POLIZAS_CONNECTION`: conexion SQL read-only directa.
- `ConnectionStrings__AppBuilderMaster`: conexion a Master para resolver la BBDD de modelo.
- `AppBuilder__EncryptionKey`: clave de descifrado AppBuilder si aplica.
- passwords, tokens, certificados, dumps, backups, logs con datos personales o cadenas de conexion completas.

## Modo A: frontend con fixtures

Uso recomendado para validar UX, paginacion visual, filtros y estados de UI sin tocar datos reales.

```powershell
cd .\iLiniumTech.Frontend
$env:VITE_USE_BACKEND = "false"
Remove-Item Env:\VITE_API_BASE_URL -ErrorAction SilentlyContinue
Remove-Item Env:\VITE_ILINIUMTECH_API_KEY -ErrorAction SilentlyContinue
Remove-Item Env:\VITE_BROKER_ID -ErrorAction SilentlyContinue
npm run dev
```

Abrir la URL que imprima Vite y entrar en `/polizas`.

Notas:

- Este modo no valida integracion backend ni SQL.
- No necesita `ApiSecurity__ApiKey`.
- Los datos deben ser anonimizados o ficticios.

## Modo B: backend + SQL controlado

Uso recomendado para validar contrato real frontend/backend y lectura SQL read-only en un entorno autorizado.

Terminal 1, backend:

```powershell
$env:ApiSecurity__ApiKey = "local-demo-api-key-change-me"
$env:Polizas__Repository = "Sql"
$env:Polizas__ConnectionResolver = "AppBuilderMaster"
$env:Polizas__AllowHeaderExecutionContext = "true"
$env:ConnectionStrings__AppBuilderMaster = "<MASTER_CONNECTION_STRING_FROM_LOCAL_SECRET_STORE>"
$env:AppBuilder__EncryptionKey = "<APPBUILDER_ENCRYPTION_KEY_FROM_LOCAL_SECRET_STORE>"
dotnet run --project .\iLiniumTech.Backend\src\iLiniumTech.Backend.Api
```

Alternativa si ya tienes una conexion read-only directa a la BBDD de polizas:

```powershell
$env:ApiSecurity__ApiKey = "local-demo-api-key-change-me"
$env:Polizas__Repository = "Sql"
$env:ConnectionStrings__PolizasReadOnly = "<POLIZAS_READONLY_CONNECTION_STRING_FROM_LOCAL_SECRET_STORE>"
dotnet run --project .\iLiniumTech.Backend\src\iLiniumTech.Backend.Api
```

Terminal 2, frontend:

```powershell
cd .\iLiniumTech.Frontend
$env:VITE_USE_BACKEND = "true"
$env:VITE_API_BASE_URL = "http://localhost:5146"
$env:VITE_ILINIUMTECH_API_KEY = "local-demo-api-key-change-me"
$env:VITE_BROKER_ID = "<BROKER_ID_FOR_LOCAL_MVP_CONTEXT>"
npm run dev
```

Notas de seguridad:

- El valor de `VITE_ILINIUMTECH_API_KEY` queda expuesto en el bundle. Usarlo solo para demo/dev.
- `ApiSecurity__ApiKey` y `VITE_ILINIUMTECH_API_KEY` deben coincidir en local, pero solo el valor del backend es secreto operacional.
- `X-Broker-Id` y los headers MVP de usuario/perfil son contexto temporal, no autenticacion.
- No pegar connection strings reales en README, issues, logs, capturas ni comentarios de PR.

## Validacion rapida

Desde la raiz del repositorio:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
cd .\iLiniumTech.Frontend
npm run lint
npm run test:unit
npm run build
npx playwright install chromium
npm run test:e2e
```

Para cambios de documentacion o plantillas, ejecutar al menos:

```powershell
cd ..
git diff -- README.md docs\engineering\08-runbook-mvp-local.md
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1
```
