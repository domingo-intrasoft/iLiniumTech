# Checklist QA - Fase 2 login, sesion y auth-broker

Fecha de preparacion: 2026-05-16

Estado: evidencia ejecutada del incremento Fase 2 login/sesion robusto.

## Alcance

- Flujo `/login -> /polizas -> /polizas/:id -> logout`.
- Sesion backend `DemoSession` en modo local/demo mientras se decide auth productiva.
- Contrato `/api/me` como fuente de identidad, broker activo, brokers permitidos, permisos y modo de autenticacion.
- Guards frontend, estados de sesion, 401/403/404 seguros y broker autorizado.

Fuera de alcance:

- Elegir proveedor auth productivo.
- Activar datos reales o SQL live.
- Cambios de UI o backend no relacionados con login, sesion, permisos o broker.
- Reabrir `Autos Particulares` como objetivo activo.

## Criterios Done especificos

- [x] `/login` se muestra sin shell protegido, menu lateral ni datos privados.
- [x] Ruta protegida sin sesion redirige a login o devuelve 401 seguro segun capa.
- [x] En `VITE_USE_BACKEND=true`, la UI valida sesion backend antes de consumir Polizas.
- [x] Refresh con sesion valida conserva contexto desde `/api/me`.
- [x] Refresh o entrada directa sin sesion no reutiliza estado local obsoleto.
- [x] Logout limpia cookie/sesion demo, estado frontend y vuelve a `/login`.
- [x] Expiracion o 401 posterior limpia estado sensible y fuerza reautenticacion.
- [x] Usuario sin `polizas.catalogs`, `polizas.read` o `polizas.detail` recibe 403 sanitizado.
- [x] Broker ausente o no permitido impide catalogos, listado, detalle y SQL real.
- [x] Cambio de broker, si existe, usa endpoint backend validado contra `allowedBrokerIds`.
- [x] `/api/me` no expone secretos, tokens, connection strings ni datos personales innecesarios.
- [x] Errores publicos incluyen `correlationId` cuando aplique y no filtran trazas, SQL, broker ajeno ni existencia de poliza.
- [x] No existe fallback silencioso a fixtures si backend real falla.
- [x] API key MVP, headers MVP y `DemoSession` quedan documentados como compatibilidad local/demo.
- [x] No se introduce runtime AppBuilder para permisos, menu, pantallas, queries o workflows.

## Comandos esperados

Resultados ejecutados el 2026-05-16 en `codex/static-polizas-data-api`.

```powershell
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
```

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
git diff --check
```

Resultados:

- `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`: OK, 0 warnings, 0 errors.
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`: OK, 86 tests passed.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: OK, 38 files, 171 tests passed.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK, no leaks found.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir <temp> -FailOnFindings`: OK, total finding count 0.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings`: OK.
- `git diff --check`: OK, solo avisos CRLF del working copy.

Smoke recomendado:

Smoke visual ejecutado contra `http://127.0.0.1:5174`:

- `/polizas` sin sesion redirige a `/login`.
- Login demo no sensible entra en `/polizas`.
- Shell/menu visible en `/polizas`, con origen `Fixture local`.
- Busqueda por `0002` muestra `POL-2026-0002`.
- Detalle `/polizas/POL-1002` abre en modo solo lectura.
- Logout devuelve a `/login`.
- Errores recientes de consola: 0.

## Evidencias a adjuntar o referenciar

- Salida resumida de comandos con fecha, entorno y rama.
- Captura o descripcion verificable del smoke si cambia UI visible.
- TRX/JUnit/reportes generados cuando existan.
- Evidencia de secret scan limpio.
- Confirmacion de que no se versionan datos reales, secretos ni capturas sensibles.

## Riesgos y bloqueos actuales

- `DemoSession`, API key MVP y headers MVP son compatibilidad local/demo; no equivalen a auth productiva.
- Falta decision humana de proveedor auth productivo.
- Falta matriz funcional real de permisos por broker, perfil, oficina, gestor y usuario.
- Falta validacion DBA/UAT sobre restricciones reales por broker y claves finales de `SESSION_CONTEXT`.
- Sin entorno autorizado no se puede demostrar aislamiento con datos reales.
- Un fallback a fixtures en modo backend real ocultaria incidencias de sesion o permisos y bloquea Done.
