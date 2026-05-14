# iLiniumTech

Repositorio de trabajo para transformar conocimiento heredado de AppBuilder en un producto iLiniumTech guiado por especificaciones.

Decision critica: iLiniumTech no sera un runtime dinamico tipo AppBuilder. La metadata solo sirve para extraccion, migracion, trazabilidad y scaffolding inicial. El producto final sera un frontend Vue estatico mantenido como codigo fuente y un backend API de datos con contratos explicitos.

Documento principal:

- [docs/DECISION_PRODUCTO_ARQUITECTURA.md](docs/DECISION_PRODUCTO_ARQUITECTURA.md)
- [docs/APPBUILDER_ANALISIS_ARQUITECTURA.md](docs/APPBUILDER_ANALISIS_ARQUITECTURA.md)
- [docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md](docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md)
- [docs/engineering/README.md](docs/engineering/README.md)
- [docs/PLAN_CICD_GITHUB_ONLY.md](docs/PLAN_CICD_GITHUB_ONLY.md)
- [docs/MVP_POLIZAS_PLAN.md](docs/MVP_POLIZAS_PLAN.md)
- [docs/MVP_POLIZAS_DIFERENCIAS.md](docs/MVP_POLIZAS_DIFERENCIAS.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md](docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md](docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md](docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-004-github-ci-baseline.md](docs/sdd/specs/iLiniumTech/SDD-2026-004-github-ci-baseline.md)
- [docs/ai/github-ai-policy.md](docs/ai/github-ai-policy.md)

## Principios de trabajo

- No guardar credenciales, cadenas de conexion completas ni secretos en este repositorio.
- Documentar rutas, responsabilidades y contratos tecnicos con suficiente detalle para crear issues SDD.
- Mantener la metadata de AppBuilder como insumo de extraccion/scaffolding, no como motor runtime del producto.
- Convertir cualquier scaffold util en codigo iLiniumTech revisado: Vue estatico y API backend explicita.
- Aplicar desde el inicio reglas de seguridad, pruebas y calidad heredadas de AcademiaLasCortes y adaptadas a los riesgos detectados en AppBuilder.

## Estado actual

Fase documental completada, decision de arquitectura fijada y primer corte MVP creado para el componente de polizas:

- metadata AppBuilder localizada y documentada;
- backend y frontend creados;
- UI local de polizas disponible;
- pruebas, build, formato y gates de seguridad ejecutados.
- siguiente orientacion: extractor offline/local, repositorio SQL read-only y evolucion frontend/backend por SDD, sin runtime dinamico.

## Proyectos creados

- `iLiniumTech.Backend`: API .NET 10 con contrato MVP de polizas.
- `iLiniumTech.Frontend`: Vue 3 + Vite + TypeScript para pantalla estatica inicial del componente de polizas.

## Configuracion local de polizas

Por defecto el backend usa fixtures anonimizados:

```json
"Polizas": {
  "Repository": "InMemory"
}
```

Para activar lectura SQL read-only en un entorno local autorizado:

```powershell
$env:Polizas__Repository = "Sql"
$env:ConnectionStrings__PolizasReadOnly = "<connection-string-local>"
# o bien
$env:ILINIUMTECH__POLIZAS_CONNECTION = "<connection-string-local>"
```

No guardar esas cadenas en Git. El repositorio SQL usa whitelist de columnas y parametros para valores; no interpreta metadata AppBuilder en runtime.

Para resolver la conexion de modelo desde Master al estilo AppBuilder en el MVP:

```powershell
$env:Polizas__Repository = "Sql"
$env:Polizas__ConnectionResolver = "AppBuilderMaster"
$env:ConnectionStrings__AppBuilderMaster = "<master-connection-string-local>"
$env:AppBuilder__EncryptionKey = "<appbuilder-encryption-key-local>"
```

`AppBuilder__EncryptionKey` solo es necesario si los campos de `IAPM_Connection` estan cifrados como en AppBuilder. La conexion Master y la clave de descifrado son secretos: deben venir de variables de entorno, secret store o del mecanismo corporativo que se defina.

### Contexto de request MVP

El flujo objetivo para acelerar el MVP es resolver el broker por request, no por una unica configuracion global:

- `X-ILiniumTech-Api-Key`: cabecera obligatoria actual para proteger `/api/polizas/*` en desarrollo/demo. Se configura con `ApiSecurity__ApiKey`. No sustituye autenticacion real.
- `X-Broker-Id`: cabecera MVP prevista para seleccionar el broker efectivo del request mientras no exista sesion autenticada completa. El backend solo la lee si `Polizas__AllowHeaderExecutionContext=true`; debe usarse como mecanismo temporal y validarse contra el contexto autenticado cuando exista auth real.
- `X-User-Id`, `X-Profile-Id`, `X-Profile-Type-Id`, `X-Is-Admin`: cabeceras MVP opcionales para completar `SESSION_CONTEXT` en entornos controlados sin auth real. No prueban identidad ni permisos.
- Headers futuros de autenticacion: `Authorization: Bearer <token>` o el mecanismo corporativo que se apruebe. A partir de ese momento, `brokerId`, `userId`, `profileId`, `profileTypeId` e `isAdmin` deben salir de claims/sesion backend, no de valores confiados al frontend.

Compatibilidad local temporal:

```powershell
$env:Polizas__BrokerId = "<broker-id-local>"
$env:Polizas__AllowHeaderExecutionContext = "true" # solo MVP/dev si se necesita X-Broker-Id
$env:Polizas__UserId = "<user-id-local>"
$env:Polizas__ProfileId = "<profile-id-local>"
$env:Polizas__ProfileTypeId = "<profile-type-id-local>"
$env:Polizas__IsAdmin = "false"
# o bien
$env:ILINIUMTECH__BROKER_ID = "<broker-id-local>"
$env:ILINIUMTECH__ALLOW_HEADER_EXECUTION_CONTEXT = "true" # solo MVP/dev si se necesita X-Broker-Id
$env:ILINIUMTECH__USER_ID = "<user-id-local>"
$env:ILINIUMTECH__PROFILE_ID = "<profile-id-local>"
$env:ILINIUMTECH__PROFILE_TYPE_ID = "<profile-type-id-local>"
$env:ILINIUMTECH__IS_ADMIN = "false"
```

Ese fallback solo sirve para pruebas locales sin broker por request. No debe usarse como modelo de produccion ni como forma de saltarse autorizacion.

### SESSION_CONTEXT previsto

Si las vistas, funciones o triggers de polizas dependen de SQL Server `SESSION_CONTEXT`, el backend debe establecer las claves necesarias antes de cada consulta sobre la conexion de modelo resuelta para el broker. Claves base/candidatas:

- `brokerId`
- `entityMainId`
- `userId`
- `profileId`
- `profileTypeId`
- `isAdmin`
- `ip` y `userAgent` si las vistas, triggers o auditoria los requieren.

Las claves deben enviarse a `sp_set_session_context` con parametros, nunca interpoladas en SQL. Hasta cerrar autenticacion real, cualquier valor procedente de cabeceras es contexto MVP no confiable y debe quedar limitado a entornos de desarrollo/demo o validado por backend.

## Comandos base

Backend:

```powershell
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run lint
npm run test:unit
npm run build
```

El frontend requiere Node.js 20.19 o superior, alineado con las guias traidas de `AcademiaLasCortes` rama `demo`.

Para conectar el frontend al backend local en desarrollo, usar valores publicos de entorno como `VITE_USE_BACKEND=true`, `VITE_API_BASE_URL=http://localhost:5146` y `VITE_ILINIUMTECH_API_KEY=<clave-local>`. Esa clave de frontend solo sirve para desarrollo/demo; no debe tratarse como secreto de produccion.

Quality gate local de fase 7:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Este gate ejecuta pruebas backend, formato/lint/tests/build frontend, auditorias de seguridad y `git diff --check`. Usar `-SkipSecurity` solo durante iteracion local rapida, nunca antes de cerrar una rama.
Si el Node global no cumple `>=20.19.0`, usar `-NodeExe "C:\ruta\a\node.exe"`. El DoD completo queda en [docs/engineering/08-quality-phase-7-dod.md](docs/engineering/08-quality-phase-7-dod.md).

### Runbook demo MVP con backend

Backend:

```powershell
$env:ApiSecurity__ApiKey = "<clave-local-demo>"
$env:Polizas__Repository = "Sql"
$env:Polizas__ConnectionResolver = "AppBuilderMaster"
$env:Polizas__AllowHeaderExecutionContext = "true"
$env:ConnectionStrings__AppBuilderMaster = "<master-connection-string-local>"
$env:AppBuilder__EncryptionKey = "<appbuilder-encryption-key-local>"
dotnet run --project .\iLiniumTech.Backend\src\iLiniumTech.Backend.Api
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
$env:VITE_USE_BACKEND = "true"
$env:VITE_API_BASE_URL = "http://localhost:5146"
$env:VITE_ILINIUMTECH_API_KEY = "<clave-local-demo>"
$env:VITE_BROKER_ID = "<broker-id-local>"
npm run dev
```

`/api/me` devuelve el contexto efectivo que usa la UI para mostrar el broker activo y detectar si falta contexto antes de consultar polizas. Cuando `VITE_USE_BACKEND=true`, los fallos de backend no se sustituyen por fixtures silenciosos: deben mostrarse como error de configuracion/conexion para que la demo sea honesta.

La pantalla de polizas ya envia `page`, `pageSize`, `fechaEfectoDesde` y `fechaEfectoHasta` al backend. En SQL, `fechaEfectoHasta` se traduce como limite superior exclusivo del dia siguiente para cubrir columnas `datetime` con hora.
