# SDD: Agenda read-only por rango

## Metadata

- Spec ID: SDD-2026-011
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local y abrir SDD draft para `Siniestros`, `Recibos` y `Clientes`, el carril de agentes por pagina avanza con `Agenda`. La evidencia AppBuilder en `docs/appbuilder/pages/agenda/README.md` confirma existencia historica de tabla `Agenda`, vista `vw_Agenda` y un control generico `DynamicFullCalendar`, pero no confirma una pantalla AppBuilder concreta con `componentId`, datasource, campos, filtros, permisos o UAT.

El frontend actual ya expone `/agenda` como ruta protegida fixture/read-only, con datos sanitizados, filtros locales, sin calendario dinamico y acciones de crear/reprogramar/exportar bloqueadas.

Decision de arquitectura: iLiniumTech no es un runtime dinamico tipo AppBuilder. Cualquier futura pantalla funcional de `Agenda` debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado, SQL parametrizado, fechas normalizadas y minimizacion de asuntos/textos sensibles.

## Objetivo

Preparar el primer incremento funcional de `Agenda` como listado read-only por rango de fechas, no como calendario interactivo:

- `GET /api/agenda/events` para listado paginado de eventos por rango obligatorio o rango por defecto acotado.
- `GET /api/agenda/catalogs` solo si producto/DBA confirma catalogos de estado, prioridad u origen.
- Frontend `/agenda` consumiendo contrato explicito, cuando exista backend aprobado.
- Permiso inicial `agenda.read`.
- Mantener calendario visual, detalle, descripcion larga, participantes, objeto relacionado, alta, reprogramacion, drag/drop, exportacion y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura, normalizacion de fechas, minimizacion de PII y revision de seguridad.

## Fuera de alcance

- Calendario visual o FullCalendar en el primer incremento.
- Drag/drop, seleccionar fecha, mover eventos o reprogramar.
- `GET /api/agenda/events/{id}` en el primer incremento.
- Detalle con `Descripcion`, participantes, `IdentidadId`, `IdObjeto` o textos libres.
- Alta, edicion, baja, workflows, `commandAdd`, `commandEdit`, `FormBuilder`, `NewRegister` o `doOperationData`.
- Exportacion de listado.
- Navegacion generica a objetos relacionados.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, vistas no validadas o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `fechaDesde` | date | Obligatorio o calculado por defecto; rango acotado. |
| `fechaHasta` | date | Obligatorio o calculado por defecto; no anterior a `fechaDesde`. |
| `texto` | string | Opcional; longitud maxima; solo sobre asunto/titulo minimizado, no descripcion larga. |
| `estado` | string | Valor catalogado si producto confirma un estado funcional. |
| `prioridad` | string/int | Valor catalogado si DBA/UAT lo aprueba. |
| `origen` | string | Solo etiqueta catalogada si existe regla funcional. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por `Descripcion`, `IdentidadId`, participante, telefono, email, direccion, datos de salud, siniestro libre, `IdObjeto` libre ni workflow en el primer corte.

### Salida `AgendaEventListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada. |
| `titulo` | Asunto/titulo sanitizado y recortado; no descripcion larga. |
| `inicio` | Fecha/hora ISO normalizada. |
| `fin` | Fecha/hora ISO normalizada o nula. |
| `estado` | Catalogo aprobado si existe. |
| `prioridad` | Catalogo aprobado si existe. |
| `origen` | Etiqueta generica, por ejemplo manual/sistema/modulo, si se confirma. |
| `objetoRelacionadoTipo` | Opcional; solo etiqueta no navegable y no sensible. |

Campos prohibidos en el primer corte:

- `Descripcion` completa;
- `IdentidadId`, participantes, telefonos, emails, direcciones o datos personales;
- datos de salud, lesionados, siniestros sensibles u observaciones libres;
- `IdObjeto` como enlace generico o identificador navegable;
- workflow, comandos, metadata o campos de auditoria;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

Regla explicita: `agenda.read` no concede detalle, descripcion larga, reprogramacion ni navegacion a objeto relacionado.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only por rango de fechas.
- El calendario visual y el detalle permanecen bloqueados hasta SDD posterior.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- Los asuntos/textos sensibles se minimizan antes de devolver datos.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de eventos de otro broker.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma columnas, filtros y rango por defecto.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] Seguridad confirma que el primer corte no devuelve descripcion larga, participantes ni PII.
- [ ] `GET /api/agenda/events` exige `agenda.read`.
- [ ] Si existe `GET /api/agenda/catalogs`, exige `agenda.read` o permiso especifico documentado.
- [ ] `agenda.read` no devuelve detalle, descripcion larga ni acciones de escritura.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Rango de fechas ausente, invalido o excesivo devuelve error sanitizado.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene calendario dinamico, crear, reprogramar, exportar y desglose bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-011-agenda-read-only.md`.
- `docs/appbuilder/pages/agenda/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Agenda` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/agenda/**`, reutilizando patrones actuales sin introducir FullCalendar hasta SDD posterior.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `agenda.read`.
- QA: evidencia en `docs/qa/**`.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Logs sin datos sensibles ni textos de asunto/descripcion en claro.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin calendario dinamico, detalle, descripcion larga, participantes ni escrituras en primer corte.
- [ ] Sin drag/drop ni actualizacion de fechas en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de rango de fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `agenda.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- parseo defensivo de fecha/hora si se usa `vw_Agenda.Start`/`End`;
- listado no proyecta descripcion larga, participantes, `IdentidadId` ni PII prohibida.

Integracion/backend:

- `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- prueba de integracion SQL solo con entorno autorizado, sin versionar credenciales ni datos.

Frontend:

- `npm run format`;
- `npm run lint`;
- `npm run test:unit`;
- `npm run build`;
- tests de ruta protegida, sin sesion, sin broker, sin permiso, loading, empty, error, filtros y paginacion;
- crear/reprogramar/exportar/desglose siguen bloqueados;
- no se renderiza calendario dinamico;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, `FullCalendar`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma columnas, filtros, rango por defecto y uso de vista lista frente a calendario;
- DBA confirma origen de lectura, parseo de fechas y restricciones por broker;
- seguridad confirma minimizacion de asuntos/textos y ausencia de PII.

## Riesgos

- `Asunto` y `Descripcion` pueden contener PII, salud, siniestros, telefonos, correos u observaciones sensibles.
- `BrokerIntegracionId` exige aislamiento tenant antes de consultar.
- Fechas separadas en tabla y `Start`/`End` texto en vista pueden crear errores de parseo o zona horaria.
- El trigger de siniestros sugiere efectos secundarios si algun dia se escribe.
- `IdentidadId` puede contener identificadores multiples o serializados.
- `IdObjeto` puede inducir navegacion generica heredada.
- FullCalendar heredado arrastra conductas interactivas que no deben entrar en read-only.
- Loguear textos de busqueda o asuntos puede exponer informacion sensible.

## Work Items

- Crear issue GitHub: `SDD-2026-011 Agenda read-only por rango`.
- Crear tarea de producto/UAT para confirmar columnas, filtros, rango y formato de lista.
- Crear tarea DBA para confirmar origen de lectura, parseo de fechas y campos prohibidos.
- Crear tarea seguridad para clasificacion de asuntos/textos libres y threat review del modulo.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros, rango y minimizacion.
- [ ] Permiso `agenda.read` definido y probado.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
