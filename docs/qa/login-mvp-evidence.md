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
