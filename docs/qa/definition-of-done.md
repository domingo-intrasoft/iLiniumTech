# Definition of Done QA/UAT

Este documento define la evidencia minima para considerar Done un cambio en iLiniumTech.

Para cierres de fase o PRs con varias areas, usar tambien la plantilla operativa [phase-closure-checklist.md](phase-closure-checklist.md).

## Done general

Un cambio esta Done cuando:

- responde a una SDD, issue, decision o tarea documentada;
- mantiene la decision de arquitectura: Vue estatico + backend API explicita;
- no introduce secretos, connection strings reales, dumps ni datos personales reales;
- no introduce runtime dinamico basado en metadata AppBuilder;
- esta probado segun el area afectada;
- deja evidencia reproducible;
- declara riesgos residuales y bloqueos externos.

## Criterios por area

### Backend

- Build Release correcto.
- Tests backend correctos.
- Endpoints nuevos o modificados tienen validacion y tests.
- Errores externos estan sanitizados.
- Datos sensibles no se loguean.
- SQL usa parametros para valores.
- Ordenaciones, columnas o estructura SQL usan whitelists revisadas.
- Configuracion y secretos salen de entorno/secret store, no de Git.

Comandos:

```powershell
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

### Frontend

- Typecheck/build correcto.
- Formato y lint correctos.
- Tests unitarios correctos.
- Estados `loading`, `empty`, `error` y permisos considerados cuando aplique.
- Errores de backend se presentan con mensajes seguros.
- No se consume metadata AppBuilder como contrato runtime.
- Si cambia UI visible, hay smoke manual o automatizado.

Comandos:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
```

### Documentacion

- Roadmap, SDD, README o engineering docs actualizados si cambia objetivo, contrato, configuracion, seguridad o flujo.
- La documentacion no contiene secretos ni datos reales.
- La validacion documental pasa.

Comando:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

### Seguridad

- Secret scan limpio.
- Auditoria de dependencias sin findings bloqueantes.
- Auditoria CORS limpia si toca backend, config o despliegue.
- Cambios de auth, API key, broker, permisos o SQL tienen revision explicita.

Comandos:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

## Criterios especificos - login, sesion y auth-broker

Aplican al incremento Fase 2 login/sesion robusto y a cualquier cambio que toque `/login`, `/api/auth/*`, `/api/me`, guards, permisos o broker activo.

- `/login` queda fuera del shell protegido y no renderiza contenido privado.
- En modo backend real (`VITE_USE_BACKEND=true`), la sesion backend validada tiene prioridad sobre cualquier sesion local.
- `/api/me` es el contrato unico para identidad, modo de autenticacion, broker activo, brokers permitidos y permisos efectivos.
- Refresh del navegador y entrada directa a rutas protegidas conservan sesion valida o redirigen/devuelven 401 seguro.
- Logout limpia sesion/cookie demo y estado frontend reutilizable.
- 401, 403 y 404 esperados muestran mensajes publicos sanitizados y `correlationId` cuando aplique.
- Falta de sesion, broker ausente, broker no permitido y permiso ausente se distinguen en UI sin exponer datos ni detalles internos.
- `currentBrokerId` debe pertenecer a `allowedBrokerIds` antes de catalogos, listado, detalle o SQL real.
- Cualquier cambio de broker debe venir de endpoint/contrato backend validado; no de header libre como autoridad.
- API key MVP, headers MVP y `DemoSession` quedan etiquetados como compatibilidad local/demo, no auth productiva.
- No se loguean tokens, cookies, API keys, datos personales, SQL completo, connection strings ni broker ajeno.
- No hay fallback silencioso a fixtures si el backend falla en modo backend real.

Evidencia operativa:

- [login-session-hardening-checklist.md](login-session-hardening-checklist.md)
- [active-broker-change-checklist.md](active-broker-change-checklist.md)

## Gate recomendado antes de cerrar rama

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Este gate agrupa backend, frontend, smoke, auditorias, validacion documental y `git diff --check`.

## UAT

Cuando el cambio afecta comportamiento funcional o UX:

- existe criterio de aceptacion claro;
- se puede reproducir el flujo;
- se validan datos esperados y errores esperados;
- se documenta entorno usado;
- se adjunta captura, log sanitizado o descripcion verificable si no hay automatizacion;
- el usuario funcional acepta o deja observaciones.

Para Polizas MVP, UAT minimo:

- abrir `/polizas`;
- confirmar modo local o backend segun configuracion;
- validar filtros principales;
- validar paginacion;
- abrir detalle;
- comprobar que errores de configuracion/API son comprensibles y no filtran SQL/secrets;
- confirmar que no se renderiza pantalla desde metadata AppBuilder.

## Evidencia obligatoria en PR o cierre

- SDD/issue/decision enlazada.
- Resumen del cambio.
- Rutas tocadas.
- Comandos ejecutados y resultado.
- Tests no ejecutados y motivo.
- Riesgos residuales.
- Bloqueos externos.
- Evidencia UAT si aplica.
- Confirmacion de que no se introducen secretos ni datos personales reales.
- Clasificacion de pendientes como `completado con evidencia`, `pendiente tecnico` o `bloqueado externo`.

## Motivos validos para no ejecutar una prueba

- El cambio no toca esa area.
- Falta entorno externo autorizado.
- Herramienta local no disponible, con alternativa ejecutada o motivo documentado.
- Prueba bloqueada por infraestructura ajena al cambio.

No son skips validos:

- no ejecutar seguridad antes de cerrar una rama con cambios sensibles;
- omitir tests porque fallan sin documentar causa;
- aceptar secretos reales detectados;
- cerrar cambios funcionales sin SDD, issue o decision.
