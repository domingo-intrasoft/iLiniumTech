# SDD: Propuestas read-only minimizado

## Metadata

- Spec ID: SDD-2026-012
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local y abrir SDD draft para `Siniestros`, `Recibos`, `Clientes` y `Agenda`, el carril de agentes por pagina avanza con `Propuestas`.

La evidencia AppBuilder en `docs/appbuilder/pages/propuestas/README.md` no confirma una pantalla concreta de `Propuestas` con `componentId`, arbol de componentes, datasources, campos, filtros, permisos, tabs, submenus o acciones. Tambien deja explicito que no debe confundirse `Propuestas` con `Solicitudes` sin validacion funcional y DBA.

El frontend actual ya expone `/propuestas` como ruta protegida fixture/read-only, con datos demo sanitizados, filtros locales y acciones de crear, convertir a poliza, documentos y exportar bloqueadas.

Decision de arquitectura: iLiniumTech no es un runtime dinamico tipo AppBuilder. Cualquier futura pantalla funcional de `Propuestas` debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado, SQL parametrizado, origen de datos aprobado y minimizacion estricta de PII, importes y documentos.

## Objetivo

Preparar el primer incremento funcional de `Propuestas` como listado read-only minimizado, sin detalle y sin acciones de negocio:

- `GET /api/propuestas` para listado paginado con filtros y ordenacion por whitelist.
- `GET /api/propuestas/catalogs` solo si producto/DBA confirma catalogos de estado, ramo/producto, canal u origen.
- Frontend `/propuestas` consumiendo contrato explicito, cuando exista backend aprobado.
- Permisos iniciales `propuestas.read` y, si se aprueban catalogos separados, `propuestas.catalogs`.
- Mantener detalle, creacion, edicion, duplicado, conversion a poliza, emision, tarificacion, documentos, exportacion, workflows e integraciones fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura, relacion o no con `Solicitudes`, minimizacion de PII/importes/documentos y revision de seguridad.

## Fuera de alcance

- `GET /api/propuestas/{id}` en el primer incremento.
- Detalle, ficha ampliada o navegacion a cliente/poliza/suplemento.
- Crear propuesta, editar borrador, duplicar, cancelar, cerrar, tarificar, emitir o convertir a poliza.
- Documentos, adjuntos, comunicaciones, observaciones, comentarios o textos libres.
- Importes reales, primas, descuentos, comisiones, tasas, formas de pago o datos bancarios.
- Datos de cliente reales: documento, nombre completo, email, telefono, direccion o cualquier PII ampliada.
- Datos de riesgo, salud, vehiculo, bienes, siniestros previos o cuestionarios.
- Exportacion de listado.
- Workflows, llamadas externas, integraciones de compania, REST/SOAP o procesos batch.
- Uso runtime de metadata AppBuilder, `QueryStatic`, `Solicitudes` por inferencia, SQL heredado libre o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `referencia` | string | Opcional; longitud maxima; solo referencia aprobada, nunca documento ni texto libre amplio. |
| `estado` | string | Valor catalogado si producto confirma un estado funcional. |
| `ramo` | string | Valor catalogado si existe origen autorizado. |
| `producto` | string | Solo catalogo aprobado, no inferido desde metadata. |
| `canal` | string | Solo si UAT/DBA confirma que no expone datos personales o comerciales sensibles. |
| `fechaAltaDesde` | date | ISO date; no posterior a `fechaAltaHasta`. |
| `fechaAltaHasta` | date | ISO date. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por cliente, documento, email, telefono, direccion, importe, prima, cuenta bancaria, riesgo, salud, vehiculo, observaciones, documentos, workflow ni `IdObjeto` libre en el primer corte.

### Salida `PropuestaListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada. |
| `estado` | Catalogo aprobado. |
| `ramo` | Catalogo aprobado. |
| `producto` | Catalogo aprobado si no revela informacion sensible. |
| `fechaAlta` | Fecha de alta si producto confirma su uso. |
| `fechaEfecto` | Opcional; solo si se considera segura y util para listado. |
| `fechaCaducidad` | Opcional; solo si se considera segura y util para listado. |
| `canal` | Etiqueta general aprobada. |
| `origen` | Etiqueta generica no tecnica, si se confirma. |

Campos prohibidos en el primer corte:

- nombre completo, documento, email, telefono, direccion o datos de contacto del cliente;
- importes reales, primas, descuentos, tasas, comisiones, formas de pago o datos bancarios;
- riesgo asegurado, salud, vehiculo, bienes, cuestionarios o datos tecnicos de tarificacion;
- observaciones, notas, documentos, adjuntos o textos libres;
- resultado de tarificacion, motivo de rechazo, scoring o reglas de compania;
- ids genericos de AppBuilder, `Solicitudes`, `IdObjeto` navegable, workflow o metadata;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

Regla explicita: `propuestas.read` no concede detalle, documentos, importes, cliente real, tarificacion, emision, conversion ni escrituras.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- El origen de datos no puede inferirse desde `Solicitudes`; debe confirmarlo DBA/producto.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de propuestas de otro broker.
- Las acciones de crear, convertir, emitir, documentos y exportar permanecen bloqueadas aunque existan datos.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma si `Propuestas` es listado comercial, solicitudes, tarificacion, emision u otro flujo.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] Seguridad confirma que el primer corte no devuelve PII, importes reales, documentos, riesgo ni textos libres.
- [ ] `GET /api/propuestas` exige `propuestas.read`.
- [ ] Si existe `GET /api/propuestas/catalogs`, exige `propuestas.catalogs` o `propuestas.read` de forma explicita y documentada.
- [ ] `propuestas.read` no devuelve detalle, documentos, importes, cliente real, tarificacion, emision ni acciones de escritura.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene crear, convertir, documentos, exportacion y detalle bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder ni dependencia inferida de `Solicitudes`.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-012-propuestas-read-only.md`.
- `docs/appbuilder/pages/propuestas/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Propuestas` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/propuestas/**`, reutilizando la ruta fixture actual sin ampliar acciones bloqueadas.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `propuestas.read`.
- QA: evidencia en `docs/qa/**`.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Logs sin referencias, textos de busqueda, PII, importes ni documentos sensibles en claro.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin detalle, documentos, importes, cliente real, tarificacion, emision ni escrituras en primer corte.
- [ ] Sin llamadas externas ni workflows en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de filtros, fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `propuestas.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta PII, importes, documentos, riesgo, textos libres ni campos prohibidos;
- no se invocan integraciones, workflows ni conversion a poliza.

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
- crear, convertir, documentos, exportar y detalle siguen bloqueados;
- no se renderizan PII, importes reales, documentos ni datos de riesgo;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, `Solicitudes`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma definicion de `Propuestas`, columnas, filtros y acciones excluidas;
- DBA confirma origen de lectura, si existe relacion con `Solicitudes`, y restricciones por broker;
- seguridad confirma minimizacion de PII, importes, riesgo, documentos y textos libres.

## Riesgos

- Confundir `Propuestas` con `Solicitudes` y construir sobre una base equivocada.
- Exponer PII de cliente o datos comerciales sensibles en un listado aparentemente simple.
- Mostrar importes, primas o descuentos antes de definir permisos y UAT.
- Activar conversion a poliza, emision o tarificacion sin rollback, auditoria ni flujo funcional.
- Resolver conexion o origen antes de validar broker y permisos.
- Reintroducir AppBuilder copiando CRUD, eventos, datasources o workflows genericos.
- Exponer documentos, observaciones o textos libres.
- Loguear referencias o busquedas que puedan identificar personas o oportunidades comerciales.
- Crear exportaciones masivas sin permisos, auditoria ni minimizacion.

## Work Items

- Crear issue GitHub: `SDD-2026-012 Propuestas read-only minimizado`.
- Crear tarea de producto/UAT para confirmar significado funcional de `Propuestas`, columnas y filtros.
- Crear tarea DBA para confirmar origen de lectura, relacion o no con `Solicitudes`, filtros por broker y campos prohibidos.
- Crear tarea seguridad para clasificacion PII/importes/documentos/riesgo y threat review del modulo.

## Actualizacion de implementacion 2026-05-18

- Implementado primer corte backend in-memory read-only: `GET /api/propuestas/catalogs` y `GET /api/propuestas`.
- Permisos implementados: `propuestas.catalogs` y `propuestas.read`.
- La implementacion no consulta SQL real, no asume equivalencia con `Solicitudes`, no devuelve solicitante, PII, importes, documentos, tarificacion ni conversion, y no habilita detalle/exportacion/escritura.
- Este corte no desbloquea datos reales; SQL y CRUD siguen bloqueados hasta UAT/DBA/security review y SDD de escritura si aplica.
- Tests dirigidos `PropuestasApiTests`: OK.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `propuestas.read` y, si aplica, `propuestas.catalogs` definidos y probados.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
