# Avance autonomo IA - 2026-05-18

Rama: `codex/polizas-crud-bbdd`
Ventana: continuacion autonoma nocturna del MVP tras el cambio de dia local en Atlantic/Canary.

## Objetivo operativo

Avanzar el MVP con incrementos pequenos, seguros y validados, sin activar datos reales, sin nuevas APIs para paginas fixture, sin reintroducir runtime dinamico AppBuilder y manteniendo `Autos Particulares` aparcado.

## Incrementos cerrados

| Commit | Area | Resultado |
| --- | --- | --- |
| `19b6a85` | Frontend accesibilidad | Tablas de superficies tecnicas bloqueadas incorporan captions accesibles con rango, total y alcance read-only/sensible. |
| `c127cb8` | Frontend accesibilidad | Acciones bloqueadas de Clientes comparten descripcion accesible sobre SDD/API/permisos/UAT y minimizacion PII. |
| `d6b09c4` | Frontend accesibilidad | Acciones bloqueadas de Agenda comparten descripcion accesible sobre SDD/API/permisos/UAT y minimizacion de PII/asuntos sensibles. |
| `c5cd403` | Frontend accesibilidad | Acciones bloqueadas de Propuestas comparten descripcion accesible sobre SDD/API/permisos/UAT y emision/conversion/documentos. |
| `fd98387` | Frontend accesibilidad | Acciones bloqueadas de Recibos comparten descripcion accesible sobre SDD/API/permisos/UAT y cobros/remesas/datos bancarios. |
| `cae0866` | Frontend accesibilidad | Acciones bloqueadas de Suplementos comparten descripcion accesible sobre SDD/API/permisos/UAT y workflows/adjuntos/datos restringidos. |
| Este bloque | Polizas CRUD BBDD | Smoke API/UI visible contra backend SQL local: alta, busqueda, edicion, baja tecnica y limpieza exacta sin residuales. |
| Bloque 06:31 | Gobierno IA | Paquete activo de agentes por pagina creado y enlazado desde roadmap, plan maestro, readiness y task pack. |
| Bloque 07:01 | SDD Siniestros | `SDD-2026-008` abierta como draft de listado read-only minimizado, sin detalle ni datos sensibles. |
| Bloque 07:31 | SDD Recibos | `SDD-2026-009` abierta como draft de listado read-only minimizado, sin importes reales ni datos bancarios. |

## Validaciones ejecutadas

Validacion tras captions accesibles en paginas tecnicas bloqueadas:

- `npx vitest run src/features/administracion/AdministracionView.test.ts src/features/configuracion/ConfiguracionView.test.ts src/features/conectividad/ConectividadView.test.ts src/features/logs/LogsView.test.ts src/features/by-aunna/ByAunnaView.test.ts src/features/controles/ControlesView.test.ts src/features/estadisticas/EstadisticasView.test.ts src/features/informes/InformesView.test.ts`: `32/32` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- Primer `npm run test:e2e -- blocked-technical-pages`: fallo por selector amplio que encontraba tambien el `caption` accesible de Administracion; se ajusto el smoke a texto exacto visible.
- `npm run test:e2e -- blocked-technical-pages`: `1` smoke OK con `CI=1`, incluyendo Administracion, Configuracion, Conectividad, Logs, By Aunna, Controles, Estadisticas e Informes.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras descripcion accesible de acciones bloqueadas en Clientes:

- `npx vitest run src/features/clientes/ClientesView.test.ts`: `4/4` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- domain-fixtures`: `1` smoke OK con `CI=1`, incluyendo `/clientes`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras descripcion accesible de acciones bloqueadas en Agenda:

- `npx vitest run src/features/agenda/AgendaView.test.ts`: `4/4` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- domain-fixtures`: `1` smoke OK con `CI=1`, incluyendo `/agenda`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras descripcion accesible de acciones bloqueadas en Propuestas:

- `npx vitest run src/features/propuestas/PropuestasView.test.ts`: `4/4` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- domain-fixtures`: `1` smoke OK con `CI=1`, incluyendo `/propuestas`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras descripcion accesible de acciones bloqueadas en Recibos:

- `npx vitest run src/features/recibos/RecibosView.test.ts`: `2/2` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- domain-fixtures`: `1` smoke OK con `CI=1`, incluyendo `/recibos`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras descripcion accesible de acciones bloqueadas en Suplementos:

- `npx vitest run src/features/suplementos/SuplementosView.test.ts`: `4/4` tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run test:unit`: `195/195` tests OK.
- `npm run build`: OK.
- `npm run test:e2e -- domain-fixtures`: `1` smoke OK con `CI=1`, incluyendo `/suplementos`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion smoke API/UI visible de Polizas CRUD BBDD:

- `dotnet run --project $env:TEMP\iliniumtech-api-crud-visible-smoke\iliniumtech-api-crud-visible-smoke.csproj`: `ApiSqlCrudVisibleSmoke=OK` y `UiSqlCrudVisibleSmoke=OK`.
- Resultado API: create 201, detalle post-create 200, busqueda post-create `1`, update 204, detalle post-update 200, delete/baja tecnica 204 y detalle post-delete 404.
- Resultado UI: login demo, `/polizas`, alta desde formulario Vue con `CiaId`/`ClienteId` existentes, busqueda por numero creado, edicion desde fila, busqueda por numero actualizado, baja tecnica desde fila y busqueda posterior sin resultados.
- Limpieza: `CleanupExactRows=1`, `ResidualAfterCleanupExact=0` y `UiResidualAfterCleanupExact=0`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.

Validacion tras paquete de agentes por pagina:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.
- No se ejecutaron tests frontend/backend porque el cambio es exclusivamente documental y no modifica runtime.

Validacion tras SDD draft de Siniestros:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.
- No se ejecutan tests frontend/backend salvo que se detecte cambio runtime; este bloque solo crea/actualiza documentacion.

Validacion tras SDD draft de Recibos:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport`: sin leaks.
- `git diff --check`: OK.
- No se ejecutan tests frontend/backend salvo que se detecte cambio runtime; este bloque solo crea/actualiza documentacion.

## Riesgos residuales

- Polizas CRUD BBDD queda cerrado como MVP local con evidencia API/UI visible; quedan UAT funcional, auth productiva, permisos finales y confirmacion DBA de triggers/campos para escrituras no sinteticas.
- `Clientes` sigue siendo fixture/read-only con PII real bloqueada; no autoriza ficha real, exportacion, desglose, contacto ni datos bancarios sin SDD/API/UAT.
- `Agenda` sigue siendo fixture/read-only con calendario dinamico, reprogramacion, participantes, workflows y asuntos sensibles bloqueados hasta SDD/API/UAT.
- `Propuestas` sigue siendo fixture/read-only; crear, convertir a poliza, documentos, detalle, exportacion y emision siguen bloqueados hasta SDD/API/permisos/UAT.
- `Recibos` sigue siendo fixture/read-only; detalle, exportacion, cobros, remesas, documentos y datos bancarios siguen bloqueados hasta SDD/API/permisos/UAT.
- `Suplementos` sigue siendo fixture/read-only; detalle, exportacion, workflows, adjuntos y datos restringidos siguen bloqueados hasta SDD/API/permisos/UAT.
- Las paginas tecnicas siguen siendo fixture/read-only o superficies bloqueadas; no autorizan API, datos reales, escrituras, exportaciones ni permisos nuevos sin SDD/API/UAT.
- `DemoSession`, API key MVP y headers MVP siguen siendo compatibilidad local/demo, no autenticacion productiva.
- Datos reales SQL, DBA/UAT, permisos finales y proveedor auth siguen bloqueados externamente.
- `Autos Particulares` sigue aparcado por decision de producto.
- El rollout por pagina no autoriza por si mismo datos reales, APIs, escrituras, exportaciones ni permisos nuevos; cada pagina debe abrir SDD propia antes de salir del carril fixture/bloqueado.
- `SDD-2026-008` deja `Siniestros` en draft: el primer corte propuesto es solo listado read-only minimizado y sigue bloqueado hasta UAT/DBA, permisos, origen de lectura autorizado y security review.
- `SDD-2026-009` deja `Recibos` en draft: el primer corte propuesto es solo listado read-only minimizado sin importes reales y sigue bloqueado hasta UAT/DBA, permisos, origen de lectura autorizado y security review.

## Siguiente paso sugerido

Revisar preguntas UAT/DBA de `Siniestros` y `Recibos`, o preparar la siguiente SDD read-only para `Clientes`.
