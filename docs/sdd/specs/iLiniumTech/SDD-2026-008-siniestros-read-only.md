# SDD: Siniestros read-only minimizado

## Metadata

- Spec ID: SDD-2026-008
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local, el siguiente bloque de trabajo abre agentes por pagina del menu. `Siniestros` es una superficie de alto valor, pero tambien de alto riesgo por PII, intervinientes, observaciones libres, datos de salud/lesiones, vehiculos, importes, reservas, pagos, recobros y dominios EIAC/aduana.

La documentacion AppBuilder disponible en `docs/appbuilder/pages/siniestros/README.md` confirma dominio historico amplio y claves de busqueda, pero no confirma una pantalla AppBuilder completa con `componentId`, layout, tabs, datasource, acciones, permisos historicos ni UAT. El frontend actual ya expone `/siniestros` como ruta protegida fixture/read-only, con datos sanitizados y acciones de detalle/exportacion bloqueadas.

Decision de arquitectura: iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. La futura pagina de `Siniestros`, si se implementa con datos, debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado y minimizacion de datos.

## Objetivo

Preparar el primer incremento funcional de `Siniestros` como listado read-only minimizado, sin detalle y sin exportacion:

- `GET /api/siniestros/catalogs` para catalogos minimos aprobados.
- `GET /api/siniestros` para listado paginado con filtros y ordenacion por whitelist.
- Frontend `/siniestros` consumiendo contrato explicito, cuando exista backend aprobado.
- Permisos iniciales `siniestros.catalogs` y `siniestros.read`.
- Mantener detalle, intervinientes, importes, EIAC, observaciones y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura y reglas de minimizacion.

## Fuera de alcance

- `GET /api/siniestros/{id}` en el primer incremento.
- Detalle de siniestro.
- Tabs o subcomponentes de intervinientes, indemnizaciones, franquicias, agenda, EIAC o aduana.
- Descripcion, danos, observaciones, lesionados, matriculas, telefonos, emails, direcciones o textos libres.
- Importes, reservas, indemnizaciones, pagos, recobros, franquicias y exportaciones.
- Alta, edicion, baja, validacion EIAC, workflows, documentos o comunicaciones.
- Permisos historicos AppBuilder convertidos automaticamente en permisos iLiniumTech.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, vistas no validadas o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `referencia` | string | Referencia opaca o numero de siniestro; no buscar por documento ni texto libre sensible. |
| `poliza` | string | Numero de poliza visible y minimizado. |
| `estado` | string | Valor catalogado. |
| `situacion` | string | Valor catalogado si DBA/UAT lo aprueba. |
| `prioridad` | string | Valor catalogado si aplica al proceso real. |
| `fechaSiniestroDesde` | date | ISO date; no posterior a `fechaSiniestroHasta`. |
| `fechaSiniestroHasta` | date | ISO date. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

### Salida `SiniestroListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia de siniestro visible, sin observaciones. |
| `poliza` | Numero de poliza visible; no incluir riesgo ni tomador completo. |
| `compania` | Nombre o codigo normalizado si no expone datos sensibles. |
| `estado` | Catalogo aprobado. |
| `situacion` | Catalogo aprobado. |
| `prioridad` | Solo si negocio confirma uso real. |
| `fechaSiniestro` | Fecha del siniestro. |
| `fechaParte` | Fecha de parte. |
| `tramitador` | Equipo o alias no personal si no hay permiso/persona aprobada. |

Campos prohibidos en el primer corte:

- documento, telefono, email, direccion;
- nombres de intervinientes o terceros;
- matricula o bastidor;
- descripcion, danos, garantias, observaciones y textos libres;
- lesionados o datos de salud;
- reserva, indemnizacion, pagos, recobros, franquicia y cualquier importe;
- datos EIAC/aduana;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- El detalle permanece bloqueado hasta SDD posterior.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- Los campos sensibles no se proyectan aunque existan en tablas/vistas heredadas.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de siniestros de otro broker.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma columnas y filtros del listado.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] `GET /api/siniestros/catalogs` exige `siniestros.catalogs`.
- [ ] `GET /api/siniestros` exige `siniestros.read`.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El listado no devuelve campos sensibles prohibidos.
- [ ] El frontend mantiene detalle/exportacion bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-008-siniestros-read-only.md`.
- `docs/appbuilder/pages/siniestros/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Siniestros` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/siniestros/**`, reutilizando patrones actuales sin convertirlos en renderer generico.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `siniestros.read`.
- QA: evidencia en `docs/qa/**`.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Logs sin datos sensibles.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin detalle, exportacion, textos libres ni importes en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de filtros, fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `siniestros.catalogs` o `siniestros.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta campos sensibles prohibidos.

Integracion/backend:

- `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- prueba de integracion SQL solo con entorno autorizado, sin versionar credenciales ni datos.

Frontend:

- `npm run format`;
- `npm run lint`;
- `npm run test:unit`;
- `npm run build`;
- tests de ruta protegida, sin sesion, sin broker, sin permiso, loading, empty, error y filtros;
- detalle/exportacion siguen bloqueados;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma columnas, filtros y orden inicial;
- DBA confirma origen de lectura y restricciones por broker;
- seguridad confirma minimizacion de PII y ausencia de importes/textos libres.

## Riesgos

- Confundir dominio de datos amplio con pantalla real confirmada.
- Abrir detalle demasiado pronto y exponer intervinientes, lesionados, observaciones o importes.
- Reutilizar vistas PBI, EIAC o aduana sin validacion DBA/UAT.
- Convertir `identidad-EXS` u otras claves historicas en permisos iLiniumTech sin matriz real.
- Filtrar siniestros entre brokers por resolver conexion antes de validar contexto.
- Exponer existencia de siniestros ajenos mediante errores, conteos o 404/403.
- Loguear descripcion, danos, telefonos, emails, matriculas, importes o textos libres.
- Crear exportaciones masivas sin permisos ni auditoria.

## Work Items

- Crear issue GitHub: `SDD-2026-008 Siniestros read-only minimizado`.
- Crear tarea de producto/UAT para confirmar columnas y filtros del listado.
- Crear tarea DBA para confirmar origen de lectura, filtros por broker y campos prohibidos.
- Crear tarea seguridad para clasificacion PII y threat review del modulo.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `siniestros.catalogs` y `siniestros.read` definidos y probados.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
