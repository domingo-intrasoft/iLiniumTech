# Login MVP Evidence

Fecha: 2026-05-15

## Alcance validado

- Ruta `/login` visible sin menu lateral.
- Acceso a `/polizas` redirige a login cuando no hay sesion MVP.
- Login demo local crea sesion en `sessionStorage`.
- Login enfoca el campo usuario al abrir, distingue aviso informativo de sesion no confirmada y marca errores de validacion con `aria-describedby`/`aria-invalid`.
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

## Checklist siguiente incremento MVP login-menu-polizas

Fecha preparacion QA: 2026-05-15

Alcance QA:

- Flujo MVP vigente: `/login` -> `/polizas` -> `/polizas/:id` -> logout.
- Menu lateral y shell estaticos, escritos en Vue/TypeScript, sin carga runtime desde metadata AppBuilder.
- Polizas read-only con permisos `polizas.catalogs`, `polizas.read` y `polizas.detail`.
- Broker efectivo validado por backend antes de consultar catalogos, listado o detalle.
- Errores publicos seguros, con `correlationId` cuando aplique y sin detalles internos.
- `Autos Particulares` queda fuera de este cierre.

Estado actual de partida:

- `Completado con evidencia`: login MVP, `demo-session`, `/api/me`, shell/menu, listado y detalle de polizas ya tienen evidencia versionada en este documento.
- `Completado con evidencia`: politicas backend iniciales para `polizas.catalogs`, `polizas.read` y `polizas.detail` estan documentadas como entregadas para el corte actual.
- `Completado con evidencia`: el incremento 2026-05-15 endurece expiracion de sesion frontend, logout, limpieza local ante 401 y broker no permitido en `/api/me`.
- `Completado con evidencia`: menu lateral mantiene Polizas como flujo activo y deja `Autos Particulares` aparcado/deshabilitado.
- `Bloqueado externo`: auth productiva, matriz funcional real de permisos y validacion DBA/UAT contra datos autorizados.

Evidencia ejecutada para este incremento:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -SkipSmoke
```

Resultado:

- Backend restore/build/test: correcto, 80 tests.
- Frontend `npm ci`, formato, lint, unit tests y build: correcto, 88 tests.
- Secret scan: sin leaks.
- Dependency audit: 0 findings.
- CORS audit: sin findings.
- Extractor tests: correctos.
- Documentation baseline: correcto.
- `git diff --check`: correcto.

Smoke navegador local:

- `/polizas` sin sesion redirige a `/login?redirect=/polizas`.
- `/login?reason=session-check-failed` muestra aviso seguro sin exponer backend ni credenciales.
- Login demo local entra en `/polizas`.
- Polizas muestra resultados en shell/menu.
- No hay enlace activo a `/autos-particulares`; aparece como opcion aparcada/deshabilitada.
- DOM visible sin `IAP_`, `QueryStatic`, `Pantalla_Polizas`, `connectionString` ni `SELECT *`.

Pruebas que debe demostrar el siguiente incremento:

- [ ] Abrir `/login` sin menu lateral ni contenido protegido.
- [ ] Entrar con sesion demo/backend autorizada y redirigir a `/polizas`.
- [ ] Ver shell con menu lateral estatico y opcion Polizas disponible para usuario con permiso.
- [ ] Cargar catalogos/listado de polizas sin usar fixtures cuando `VITE_USE_BACKEND=true`.
- [ ] Abrir una poliza desde el listado y validar detalle read-only en `/polizas/:id`.
- [ ] Volver o navegar sin perder un estado razonable de listado cuando aplique.
- [ ] Ejecutar logout, limpiar sesion/cookie demo y volver a `/login`.
- [ ] Refrescar o entrar directo a `/polizas` sin sesion y comprobar redireccion o 401 seguro segun capa.

Permisos y broker:

- [ ] Usuario sin `polizas.catalogs` recibe 403 seguro al cargar catalogos y la UI muestra estado de acceso/configuracion sin datos.
- [ ] Usuario sin `polizas.read` recibe 403 seguro en listado y la UI no muestra polizas.
- [ ] Usuario sin `polizas.detail` recibe 403 seguro en detalle y la UI no revela datos de la poliza.
- [ ] `currentBrokerId` sale de `/api/me` o sesion backend y pertenece a `allowedBrokerIds`.
- [ ] Broker ausente o no permitido devuelve 403 antes de resolver conexion SQL o leer repositorio real.
- [ ] Si existe seleccion/cambio de broker, se valida en backend y no mediante header libre manipulable.
- [ ] API key MVP, headers MVP y `demo-session` quedan etiquetados como compatibilidad local/demo, no auth productiva.

Errores seguros y privacidad:

- [ ] 401/403/404 esperados devuelven mensaje publico sanitizado y `correlationId` cuando aplique.
- [ ] Errores no contienen SQL, nombres internos de tablas/vistas, connection strings, rutas locales, stack traces, cookies, tokens ni datos personales.
- [ ] El detalle de poliza inexistente o de broker cruzado no revela si la poliza existe.
- [ ] En modo backend real/demo, un fallo de API o configuracion no cae silenciosamente a fixtures.
- [ ] Logs o capturas usados como evidencia estan sanitizados y no contienen secretos ni datos reales.

Ausencia de metadata runtime:

- [ ] La UI no llama endpoints de metadata de pantalla para construir rutas, menu, filtros, columnas, permisos ni queries.
- [ ] El DOM visible y la evidencia E2E no contienen `connectionString`, `SELECT *`, `AppBuilder`, `QueryStatic`, `Pantalla_Polizas` ni prefijos `IAP_`.
- [ ] El menu lateral procede de codigo fuente iLiniumTech y se filtra solo por permisos iLiniumTech.
- [ ] Cualquier referencia AppBuilder queda limitada a documentacion, SDD, extractor offline o trazabilidad sanitizada.

Comandos minimos esperados si el incremento toca runtime:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
powershell -NoProfile -ExecutionPolicy Bypass -File ..\tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File ..\tools\security\Invoke-SecretScan.ps1
```

Criterio de cierre QA:

- No marcar Done si falla el flujo `login -> polizas -> detalle -> logout`.
- No marcar Done si un 401/403 filtra detalle interno o si falta `correlationId` donde el contrato lo exige.
- No marcar Done si el backend permite broker cruzado o permiso ausente antes de llegar al repositorio.
- No marcar Done si el frontend usa metadata AppBuilder como contrato runtime.
- Todo punto no ejecutado debe quedar clasificado como `Pendiente tecnico` o `Bloqueado externo`, con motivo y responsable.
