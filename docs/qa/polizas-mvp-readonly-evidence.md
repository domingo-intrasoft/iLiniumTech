# Evidencia QA - Polizas MVP read-only

Fecha: 2026-05-15
Rama esperada: `codex/static-polizas-data-api`
Responsable de cierre: Jefe QA/DevOps/Gobierno IA

## Alcance del incremento

Este documento prepara la evidencia del incremento Polizas read-only sobre el MVP vigente:

- login/sesion MVP, shell y menu lateral estatico;
- listado de Polizas read-only con filtros, paginacion y detalle;
- filtros de listado sincronizados con URL;
- detalle con prevalidacion de sesion, broker y permiso antes de leer datos;
- cambio de broker demo solo si lo entrega backend mediante validacion explicita;
- criterios de smoke local reproducibles.

Fuera de alcance confirmado:

- `Autos Particulares` queda aparcado y no forma parte de este cierre;
- autenticacion productiva real, proveedor OIDC/SSO y matriz funcional definitiva;
- escrituras, workflows, acciones heredadas o runtime AppBuilder;
- validacion contra BBDD real si no existe entorno autorizado DBA/UAT.

## Referencias de gobierno

- `AGENTS.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/qa/definition-of-done.md`
- `docs/qa/login-mvp-evidence.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-007-login-mvp.md`

## Estado ejecutivo

| Area | Estado | Evidencia / criterio | Siguiente paso | Responsable |
| --- | --- | --- | --- | --- |
| Producto / SDD | Completado con evidencia | MVP vigente confirmado: login/sesion, shell/menu y Polizas read-only. Autos aparcado. | Mantener SDD-2026-001/003/005/007 como base del cierre. | Producto / Arquitectura |
| Filtros en URL | Completado con evidencia | `PolizasView` sincroniza filtros soportados, paginacion y query params; normaliza paginas/page sizes/fechas invalidas. | Mantener cobertura unitaria y E2E al ampliar filtros. | Frontend / UX |
| Resumen filtros activos | Completado con evidencia | El listado muestra chips de filtros activos derivados del estado Vue/query params y traduce valores de catalogo visibles. | Mantenerlo como resumen estatico; acciones adicionales requieren nueva cobertura. | Frontend / UX |
| Accesos scopes Polizas | Completado con evidencia | Los accesos superiores a Flotas y Colectivas navegan a paginas estaticas bloqueadas por SDD; Polizas Externas permanece deshabilitado. | No activar datos, filtros ni permisos de scopes sin SDD/API/UAT. | Frontend / UX |
| Detalle prevalidado | Completado con evidencia | La UI valida sesion, broker requerido y `polizas.detail` antes de consultar detalle en modo backend; si falta permiso, el listado muestra motivo visible y acciones deshabilitadas con descripcion accesible. | Completar UAT con matriz real de permisos cuando exista. | Frontend / Backend |
| Cambio broker demo | Completado MVP | `POST /api/auth/broker` exige `demo-session`, valida `allowedBrokerIds`, reemite cookie y no acepta API key sola. | Retirar o reemplazar en auth productiva. | Backend / Datos |
| Seguridad / privacidad | Completado con gate local | Gate completo, secret scan, dependencia, CORS, build, tests, E2E y smoke sin findings bloqueantes. | Repetir en PR/CI. | QA / Seguridad |
| UAT | Bloqueado externo | Requiere entorno autorizado y responsable funcional si se valida contra datos reales. | Registrar UAT o bloqueo DBA/funcional. | Producto / DBA |

## Criterios de aceptacion QA

### Filtros en URL

- `/polizas` debe poder reconstruir el estado del listado desde query params permitidos.
- Query params permitidos para este corte: `numero`, `cliente`, `estado`, `compania`, `ramo`, `fechaEfectoDesde`, `fechaEfectoHasta`, `page`, `pageSize` y `sort` si el contrato lo mantiene.
- Campos heredados fuera de contrato no deben entrar en URL ni enviarse a backend como filtros efectivos.
- Valores vacios deben eliminarse de la URL o normalizarse sin dejar ruido.
- Fechas invalidas, paginas menores que 1, page sizes no permitidos o sort fuera de whitelist deben normalizarse o bloquearse con error seguro.
- Cambiar filtros debe resetear `page` a 1 salvo decision UX documentada.
- Paginacion y page size deben reflejarse en URL.
- Recargar navegador, abrir enlace compartido o usar atras/adelante debe conservar un estado coherente del listado.
- La URL no debe contener datos personales reales, tokens, cookies, API keys, connection strings ni valores de headers MVP.

### Detalle con prevalidacion de sesion y permisos

- Entrada directa a `/polizas/:id` sin sesion debe redirigir a `/login` o devolver estado 401 seguro, segun la capa probada.
- Con `VITE_USE_BACKEND=true`, el frontend debe validar `/api/me` o contexto equivalente antes de consultar detalle.
- Si falta broker requerido o `currentBrokerId` no pertenece a `allowedBrokerIds`, no debe consultarse el detalle.
- Si falta permiso `polizas.detail`, la UI debe mostrar acceso denegado sin revelar datos de la poliza.
- Si el listado ya esta visible pero la sesion no concede `polizas.detail`, los enlaces de detalle deben quedar deshabilitados y explicar el motivo sin consultar el detalle.
- El detalle debe mostrar el contexto superior coherente con el listado: broker activo, broker requerido o aviso por sesion/permiso cuando aplique.
- Backend debe exigir `polizas.detail` y broker autorizado antes de leer repositorio o resolver SQL.
- Una poliza inexistente, no autorizada o de broker cruzado no debe revelar si existe.
- 401/403/404 esperados deben incluir mensaje publico sanitizado y `correlationId` cuando aplique.
- El boton/enlace de vuelta desde detalle debe conservar filtros y paginacion de origen cuando la navegacion venga desde `/polizas`.
- En modo backend real/demo, un fallo de API o configuracion no debe caer silenciosamente a fixtures.

### Cambio de broker demo, si backend lo entrega

- El cambio de broker solo es aceptable mediante endpoint o contrato backend explicito; no mediante `X-Broker-Id` libre desde la UI como autoridad.
- El backend debe validar el broker solicitado contra `allowedBrokerIds` de la sesion demo o claims disponibles.
- Un broker permitido debe actualizar el contexto efectivo y refrescar `/api/me` antes de recargar catalogos/listado.
- Un broker no permitido debe devolver 403 sanitizado, mantener el broker anterior y no leer datos.
- Sin sesion valida debe devolver 401 o redirigir segun capa, sin cambiar contexto.
- La UI debe ocultar o deshabilitar selector si no hay multiples brokers permitidos o si el contrato backend no esta disponible.
- La evidencia debe declarar la ruta backend, payload, resultado esperado y tests asociados.
- API key MVP, headers MVP y `demo-session` deben quedar etiquetados como compatibilidad local/demo, no auth productiva.

### Ausencia de runtime AppBuilder

- El menu lateral, filtros, columnas, rutas y permisos visibles proceden de codigo iLiniumTech, no de metadata `IAP_*`.
- La UI no llama endpoints de metadata para construir pantallas, queries, permisos, menus o workflows.
- El DOM visible y capturas de smoke no deben contener `IAP_`, `QueryStatic`, `ComponentDataSource`, `Pantalla_Polizas`, `connectionString`, `SELECT *` ni nombres internos de tablas.
- Cualquier referencia a AppBuilder queda limitada a documentacion, SDD, extractor offline o trazabilidad sanitizada.

## Smoke local minimo

Entorno aceptado para smoke:

- fixture local si se valida solo UX sin backend;
- backend `InMemory` con `demo-session` para login/permisos;
- SQL local autorizado solo si DBA lo habilita y sin datos personales reales en evidencia.

Pasos minimos:

1. Revisar `git status --short --branch` y confirmar rama `codex/static-polizas-data-api`.
2. Abrir `/login` y confirmar que no aparece menu lateral ni contenido protegido.
3. Iniciar sesion demo/backend con credenciales no sensibles.
4. Llegar a `/polizas` dentro de shell/menu estatico.
5. Aplicar filtros soportados y confirmar que la URL refleja el estado permitido.
6. Recargar la URL filtrada y confirmar que filtros, pagina y resultados se reconstruyen.
7. Cambiar pagina y page size; confirmar URL y resultados.
8. Abrir detalle desde el listado y confirmar vista read-only.
9. Volver al listado y confirmar conservacion razonable de filtros/paginacion.
10. Probar entrada directa a detalle sin sesion y con permiso ausente si el entorno permite crear esa sesion.
11. Si hay cambio broker demo, seleccionar broker permitido y no permitido; comprobar validacion backend y ausencia de fuga.
12. Ejecutar logout y confirmar vuelta a `/login` sin datos de sesion reutilizables.
13. Revisar DOM/capturas: sin metadata runtime, secretos, SQL ni PII real.

## Evidencia tecnica ejecutada

Comandos ejecutados para cerrar el incremento:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -RunFrontendE2E
```

Resultado del gate:

- backend restore/build/test: 85 tests OK;
- frontend install, format, lint, unit tests y build: 94 tests OK;
- frontend E2E smoke: 2 tests OK;
- backend HTTP smoke y frontend smoke: OK;
- secret scan: sin leaks;
- dependency audit npm/.NET: 0 findings;
- CORS audit: sin findings;
- extractor metadata Polizas: OK;
- documentation baseline: OK;
- git whitespace check: OK.

## Evidencia a aportar por area

| Area | Prueba / evidencia requerida | Resultado esperado |
| --- | --- | --- |
| Frontend filtros URL | Unit tests de parseo/serializacion y E2E con reload/back-forward. | Query params permitidos reconstruyen listado; valores invalidos se normalizan o bloquean. |
| Frontend filtros activos | Test de listado con busqueda, limpieza, query params y valores de catalogo. | Chips visibles reflejan filtros activos sin invocar metadata ni nuevas APIs. |
| Frontend scopes Polizas | Unit test de shortcuts y smoke E2E de navegacion a Flotas. | Flotas/Colectivas solo abren paginas estaticas bloqueadas; Externas sigue sin accion. |
| Frontend detalle | Tests de entrada directa, sin sesion, sin `polizas.detail` y vuelta conservando filtros. | No se consulta detalle si falta sesion/contexto/permiso; error visible seguro. |
| Frontend contexto detalle | Test de detalle con broker valido, broker ausente y permiso ausente. | Badge superior refleja broker activo o estado de atencion sin ocultar el error principal. |
| Frontend listado sin detalle | Tests de tabla/listado con `polizas.read` sin `polizas.detail`. | Acciones de detalle deshabilitadas, sin enlaces navegables y con motivo visible/accesible. |
| Backend permisos | Tests de `polizas.catalogs`, `polizas.read`, `polizas.detail`, broker ausente y broker cruzado. | 401/403 sanitizados con `correlationId`; sin lectura de repositorio si falla autorizacion. |
| Broker demo | Tests de broker permitido, broker no permitido y sesion ausente. | Backend valida `allowedBrokerIds`; no se usa header libre como autoridad. |
| Seguridad | Secret scan, CORS si toca API/config, errores sanitizados y capturas redaccionadas. | Sin leaks, sin SQL/trazas/secrets/PII en respuestas o evidencia. |
| No AppBuilder runtime | E2E/DOM y revision de llamadas de red. | Sin endpoints metadata para UI, permisos, filtros, columnas, menus o queries. |

## Bloqueos y riesgos residuales

- Auth productiva y proveedor de identidad siguen pendientes de decision externa.
- La matriz real de permisos por broker, perfil, oficina, gestor y usuario sigue pendiente de validacion funcional.
- La validacion contra SQL real depende de BBDD/cuenta read-only autorizada y confirmacion DBA de `SESSION_CONTEXT`.
- El cambio de broker demo no puede cerrarse si backend no entrega contrato validado.
- API key MVP, headers MVP y `demo-session` no deben presentarse como seguridad productiva.
- Capturas o logs de smoke con datos reales quedan prohibidos; usar fixtures anonimizadas o redaccion previa.

## Criterio de no Done

No marcar este incremento como Done si se cumple cualquiera de estas condiciones:

- filtros en URL no sobreviven a reload o comparten estado no permitido;
- detalle consulta datos antes de validar sesion, broker y `polizas.detail`;
- broker no permitido llega a resolver conexion SQL o leer repositorio;
- 401/403/404 filtran SQL, connection strings, trazas, tokens, cookies o existencia de poliza/broker;
- frontend cae a fixtures silenciosamente en modo backend real/demo;
- se introduce runtime AppBuilder para rutas, menu, filtros, columnas, permisos o queries;
- falta documentar pruebas no ejecutadas, riesgos residuales o bloqueos externos.
