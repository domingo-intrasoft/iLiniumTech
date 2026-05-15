# Login MVP Evidence

Fecha: 2026-05-15

## Alcance validado

- Ruta `/login` visible sin menu lateral.
- Acceso a `/polizas` redirige a login cuando no hay sesion MVP.
- Login demo local crea sesion en `sessionStorage`.
- Tras login se muestra la pantalla actual de polizas dentro de `AppShell`.
- Boton de salida disponible en pantallas con shell.
- La UI sigue sin consumir metadata AppBuilder para construir pantallas.

## Evidencia automatizada

Comandos ejecutados con Node compatible:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -SkipSmoke
```

Resultado:

- Backend restore/build/tests correctos.
- Frontend `npm ci`, `format`, `lint`, `test:unit:ci` y `build` correctos.
- Tests backend: 69 correctos.
- Tests frontend unitarios: 68 correctos.
- Secret scan: sin leaks.
- Dependency audit: 0 findings.
- CORS audit: sin findings.
- Documentation baseline: correcto.
- Git whitespace check: correcto.

Despues de actualizar E2E para el nuevo login:

```powershell
npm run test:e2e
```

Resultado:

- 2 tests E2E correctos.
- `polizas` valida login demo, listado local, detalle y ausencia de metadata runtime.
- `autos-particulares` valida login demo, listado local y ausencia de llamadas backend.

Comandos frontend repetidos tras el cambio E2E:

```powershell
npm run format
npm run lint
npm run test:unit
npm run build
```

Resultado:

- Formato correcto.
- Lint correcto.
- Unit tests: 68 correctos.
- Build correcto.

## Evidencia visual

Servidor local:

- `http://127.0.0.1:5174/login`
- `http://127.0.0.1:5174/polizas`

Capturas generadas:

- `quality-reports/login-mvp-login.png`
- `quality-reports/login-mvp-polizas.png`

Smoke visual:

- Abrir `/polizas` sin sesion redirige a login.
- Login demo con usuario y contrasena no sensibles.
- Redireccion final a `/polizas`.
- Tabla de polizas visible.
- No aparecen `connectionString`, `SELECT *`, `AppBuilder`, `QueryStatic` ni `Pantalla_Polizas` en el cuerpo visible.

## Riesgos residuales

- La sesion es demo/local y no representa autenticacion productiva.
- El backend sigue pendiente de login real y de sustituir cabeceras MVP por claims/sesion validada.
- El proveedor de identidad productivo sigue pendiente de decision funcional/tecnica.

## Incremento backend demo-session

Fecha: 2026-05-15

Alcance implementado:

- `POST /api/auth/login` crea una sesion demo backend mediante cookie `HttpOnly`.
- `POST /api/auth/logout` limpia la cookie demo.
- `/api/me` devuelve usuario, aplicacion, brokers permitidos, permisos y modo de autenticacion.
- El contexto efectivo de polizas prioriza claims/sesion y conserva el fallback MVP por API key/cabeceras.
- Frontend soporta `VITE_AUTH_MODE=demo-session` con `withCredentials` y sin exponer API key.
- La UI de login sigue siendo Vue compilado y no consume metadata AppBuilder.

Evidencia ejecutada:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -SkipSmoke
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

Resultado:

- Gate local con `-SkipSmoke`: correcto.
- Backend restore/build/tests dentro del gate: 73 tests correctos.
- Formato frontend correcto.
- Lint frontend correcto.
- Unit tests frontend: 73 correctos.
- Build frontend correcto.
- Secret scan: sin leaks.
- CORS audit: sin findings.
- Documentation baseline: correcto.

Validacion backend:

```powershell
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Resultado:

- Build backend Release correcto con SDK .NET 10.0.300.
- Tests backend: 73 correctos.
- Se han anadido pruebas backend para login demo, credenciales invalidas, opt-in fuera de Development, logout y `/api/me` con sesion cookie.

Riesgos residuales:

- El modo backend demo no es autenticacion productiva.
- Falta decision de proveedor auth real y reglas finales de permisos/brokers.

## Incremento permisos y broker autorizado

Fecha: 2026-05-15

Alcance implementado:

- Politicas objetivo: `polizas.catalogs`, `polizas.read` y `polizas.detail`.
- Endpoints protegidos por politica: catalogos, listado y detalle de polizas.
- Validacion: `currentBrokerId` debe pertenecer a `allowedBrokerIds` para sesiones con claims antes de consultar catalogos, listado o detalle.
- Compatibilidad temporal: API key MVP y `demo-session` pueden seguir usandose para desarrollo/demo mientras se implementa auth real.
- Limite explicito: no hay autenticacion productiva real, proveedor de identidad aprobado ni matriz funcional definitiva de permisos.
- La metadata AppBuilder se mantiene como evidencia de migracion y no como contrato runtime de permisos, pantallas o queries.
- Frontend clasifica 401/403 como acceso denegado o sesion no autorizada sin exponer mensajes internos.

Pruebas cubiertas:

- `GET /api/polizas/catalogs` sin `polizas.catalogs` devuelve 403 sanitizado.
- `GET /api/polizas` sin `polizas.read` devuelve 403 sanitizado.
- `GET /api/polizas/{id}` sin `polizas.detail` devuelve 403 sanitizado.
- Broker solicitado fuera de `allowedBrokerIds` devuelve 403 antes de crear sesion.
- API key MVP queda como compatibilidad legacy explicita para no romper el MVP actual.
- `demo-session` solo concede permisos demo configurados para entorno controlado.
- 401/403 incluyen `correlationId` cuando aplique y no exponen trazas, SQL, broker ajeno ni existencia de poliza.
- Frontend muestra acceso denegado o estado de configuracion sin recurrir a fixtures silenciosos.

Evidencia ejecutada:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
npm run format
npm run lint
npm run test:unit
npm run build
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado:

- Tests backend: 79 correctos.
- Formato frontend correcto.
- Lint frontend correcto.
- Unit tests frontend: 79 correctos.
- Build frontend correcto.
- E2E frontend: 2 correctos.
- Documentation baseline correcto.

Gate final integrado:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -SkipSmoke
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

Resultado:

- Gate local con `-SkipSmoke`: correcto.
- Backend restore/build/tests dentro del gate: 79 tests correctos.
- Frontend install, formato, lint, unit tests y build dentro del gate: correctos.
- Secret scan: sin leaks.
- Dependency audit: 0 findings.
- CORS audit: sin findings.
- Extractor tests: correctos.
- Documentation baseline: correcto.

Riesgos residuales del siguiente incremento:

- API key MVP y `demo-session` pueden confundirse con seguridad productiva si no se etiquetan en UI, PR y evidencia.
- Falta proveedor auth real y matriz validada de permisos por broker, perfil, oficina, gestor y usuario.
- Falta confirmar con DBA las claves finales de `SESSION_CONTEXT` y restricciones reales por broker.
- Falta UAT contra entorno autorizado para demostrar que broker cruzado no filtra datos.
