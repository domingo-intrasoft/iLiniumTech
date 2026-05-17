# Checklist QA - Cambio de broker activo validado por backend

Fecha de preparacion: 2026-05-16

Estado: incremento implementado y validado con tests automatizados. Smoke visual en navegador integrado parcial por limitacion de escritura del Browser en esta sesion.

## Alcance

- Selector de broker activo en frontend cuando exista contexto backend.
- Cambio de broker mediante contrato backend explicito.
- Actualizacion de `/api/me` como fuente de verdad de broker activo, brokers permitidos y permisos efectivos.
- Refresco de Polizas tras cambiar broker activo.
- Manejo seguro de errores esperados 400, 401 y 403.

Fuera de alcance:

- Elegir proveedor de autenticacion productiva.
- Activar datos reales o SQL live sin entorno autorizado.
- Cambiar contratos de Polizas no relacionados con broker activo.
- Usar metadata AppBuilder como fuente runtime de brokers, permisos, pantallas o queries.

## Criterios QA obligatorios

Marcar solo cuando exista evidencia real del incremento.

- [x] El selector de broker muestra exclusivamente brokers presentes en `allowedBrokerIds` recibidos desde `/api/me` o contrato backend equivalente.
- [x] El frontend no permite seleccionar ni enviar un broker que no este en `allowedBrokerIds`.
- [x] El cambio de broker se solicita con `POST /api/auth/broker` o endpoint backend equivalente documentado.
- [x] El backend valida el broker solicitado contra `allowedBrokerIds` antes de actualizar el contexto activo.
- [x] No se usa ningun header libre, query string manipulable ni storage local como autoridad para broker activo.
- [x] Tras cambiar broker, el frontend vuelve a consultar `/api/me` y refleja el `currentBrokerId` actualizado.
- [x] Tras cambiar broker, catalogos/listado/detalle de Polizas se refrescan o invalidan cache para evitar datos del broker anterior.
- [x] Catalogos, listado y detalle no se consultan si el contexto queda sin broker activo valido.
- [x] Si el selector de broker muestra error, el control expone `aria-invalid` y descripcion accesible del motivo.
- [x] Error 400 por payload invalido o broker ausente se muestra con mensaje publico sanitizado.
- [x] Error 401 por sesion ausente o expirada fuerza reautenticacion sin conservar datos sensibles.
- [x] Error 403 por broker no permitido se muestra sanitizado y no revela brokers ajenos ni existencia de polizas.
- [x] Errores publicos incluyen `correlationId` cuando aplique y no exponen trazas, SQL, connection strings, tokens ni cookies.
- [x] No se introducen secretos, datos personales reales, dumps ni capturas sensibles.
- [x] API key MVP, headers MVP y `DemoSession` siguen clasificados como compatibilidad local/demo, no auth productiva.
- [x] No se introduce runtime dinamico basado en metadata AppBuilder.

## Evidencia ejecutada

Resultados registrados durante el cierre del incremento.

| Evidencia | Resultado | Artefacto / ruta | Motivo si no ejecutado |
| --- | --- | --- | --- |
| `git status --short --branch` | OK antes de stage: rama `codex/static-polizas-data-api` con cambios del incremento | Salida local de Git |  |
| Revision de rutas tocadas dentro del alcance | OK | `src/services/session.ts`, `src/features/auth/authSession.ts`, `src/layout/AppShell.vue`, vistas/tests de Polizas, checklist QA |  |
| Tests backend de `POST /api/auth/broker` valido, invalido, sin sesion y no permitido | OK: 86 tests backend superados | `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release` |  |
| Tests frontend del selector limitado a `allowedBrokerIds` | OK: `AppShell.test.ts` y `PolizasView.test.ts` | `npm run test:unit` |  |
| Tests frontend de refresh de `/api/me` y Polizas tras cambio | OK: `session.test.ts`, `authSession.test.ts`, `PolizasView.test.ts` | `npm run test:unit` |  |
| Smoke manual o automatizado `/login -> /polizas -> cambiar broker -> polizas refresca` | Parcial | Navegador integrado renderiza `/login`; flujo interactivo cubierto por tests de vista | Escritura del Browser fallo por portapapeles virtual en esta sesion. |
| Validacion de errores 400/401/403 sanitizados | OK | Backend tests y frontend tests de 401/limpieza de sesion |  |
| Secret scan | OK: sin fugas | `tools\security\Invoke-SecretScan.ps1` |  |
| Baseline documental | OK | `tools\quality\Test-DocumentationBaseline.ps1` |  |
| Auditoria dependencias | OK: 0 findings | `tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings` |  |
| Auditoria CORS | OK | `tools\security\Invoke-CorsAudit.ps1 -FailOnFindings` |  |
| Build frontend | OK | `npm run build` |  |
| Lint/formato frontend | OK | `npm run format`, `npm run lint` |  |
| Whitespace diff | OK, solo avisos CRLF de Git | `git diff --check` |  |

## Comandos ejecutados

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

## Riesgos y bloqueos

- El smoke interactivo completo queda pendiente por limitacion local del Browser; el flujo esta cubierto por tests automatizados de vista/servicio.
- `POST /api/auth/broker` debe mantenerse como autoridad unica del cambio; aceptar headers libres como autoridad bloquearia Done.
- Falta confirmar auth productiva y matriz real de permisos por broker, perfil, oficina, gestor y usuario.
- Falta entorno autorizado para demostrar aislamiento con datos reales.
- Sin DBA/UAT no se puede confirmar que `SESSION_CONTEXT` y restricciones reales de broker coinciden con la regla productiva.
- Los errores 403/404 deben evitar revelar existencia de polizas o brokers ajenos.
