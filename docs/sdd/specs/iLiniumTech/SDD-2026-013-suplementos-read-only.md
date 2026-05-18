# SDD: Suplementos read-only minimizado

## Metadata

- Spec ID: SDD-2026-013
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local y abrir SDD draft para `Siniestros`, `Recibos`, `Clientes`, `Agenda` y `Propuestas`, el carril de agentes por pagina avanza con `Suplementos`.

La evidencia AppBuilder en `docs/appbuilder/pages/suplementos/README.md` confirma que `Suplementos` existe como concepto de menu, busqueda y dominio de datos. Tambien identifica entidades y tablas relacionadas como `Suplemento`, `SuplementoDatosTomador`, `SuplementoBeneficiario`, `SuplementoDomiciliacionBancaria`, relaciones con `Poliza`, `Recibo` y `RegularizacionDeclaracion`, y triggers/logs. Sin embargo, no confirma una pantalla AppBuilder concreta con `componentId`, datasource, columnas, filtros, permisos, tabs, submenus o acciones.

El frontend actual ya expone `/suplementos` como ruta protegida fixture/read-only, con datos sanitizados, filtros locales y acciones de detalle/exportacion bloqueadas.

Decision de arquitectura: iLiniumTech no es un runtime dinamico tipo AppBuilder. Cualquier futura pantalla funcional de `Suplementos` debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado, SQL parametrizado, origen de datos aprobado y minimizacion estricta de PII, importes, datos bancarios, documentos y workflows.

## Objetivo

Preparar el primer incremento funcional de `Suplementos` como listado read-only minimizado, sin detalle y sin acciones operativas:

- `GET /api/suplementos/catalogs` para catalogos minimos aprobados.
- `GET /api/suplementos` para listado paginado con filtros y ordenacion por whitelist.
- Frontend `/suplementos` consumiendo contrato explicito, cuando exista backend aprobado.
- Permisos iniciales `suplementos.catalogs` y `suplementos.read`.
- Mantener detalle, alta, edicion, anulacion, importacion, exportacion, actualizacion masiva, workflows, documentos, recibos/declaraciones relacionados, comunicaciones y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura, vistas/tablas autorizadas, minimizacion de PII/importes/banco/documentos y revision de seguridad.

## Fuera de alcance

- `GET /api/suplementos/{id}` en el primer incremento.
- Detalle de suplemento, tabs por tipo o secciones condicionales.
- Alta, edicion, anulacion, borrado, importacion, actualizacion masiva o workflow.
- Exportacion de listado.
- Documentos, adjuntos, recibos, declaraciones, comunicaciones o EIAC.
- Datos de tomador, beneficiario, gestor de cobro, domicilio, documento, email, telefono o direccion.
- Datos bancarios, IBAN, titular, cuenta, mandato o domiciliacion.
- Importes reales, primas, valores anteriores/nuevos, tasas, comisiones, rescates, aportaciones, descuentos o calculos.
- Datos de riesgo, salud, bienes, garantias o cuestionarios.
- Navegacion real a poliza, recibo, cliente, riesgo o declaracion.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, vistas no validadas o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `referencia` | string | Opcional; longitud maxima; referencia aprobada de suplemento. |
| `poliza` | string | Opcional; numero/referencia de poliza visible y minimizado. |
| `tipo` | string | Valor catalogado si producto/DBA confirma `IdTipo` o equivalente funcional. |
| `situacion` | string | Valor catalogado si producto/DBA confirma `IdSituacion` o equivalente funcional. |
| `fechaEfectoDesde` | date | ISO date; no posterior a `fechaEfectoHasta`. |
| `fechaEfectoHasta` | date | ISO date. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por cliente/tomador, documento, email, telefono, direccion, IBAN, importe, valor, prima, riesgo, salud, observaciones, documentos, workflow, recibo/declaracion relacionada ni texto libre amplio en el primer corte.

### Salida `SuplementoListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada. |
| `poliza` | Numero/referencia de poliza visible; no incluye tomador ni cliente. |
| `referenciaCia` | Opcional si UAT/DBA la aprueba. |
| `tipo` | Catalogo aprobado. |
| `situacion` | Catalogo aprobado. |
| `fechaEfecto` | Fecha de efecto si producto confirma su uso. |
| `concepto` | Solo catalogo/etiqueta minimizada; no texto libre sensible. |
| `resumen` | Opcional; etiqueta neutra, sin importes, PII, riesgo ni datos bancarios. |

Campos prohibidos en el primer corte:

- `EmailComunicacion`;
- `SuplementoDatosTomador.*`, documento, nombre, apellidos, direccion, telefono, movil o email;
- `SuplementoBeneficiario.*`, CIF, razon social, domicilio o datos de prestamo;
- `SuplementoDomiciliacionBancaria.*`, IBAN, titular, cuenta o documento;
- importes reales, primas, valores anteriores/nuevos, tasas, comisiones, rescates, aportaciones o calculos;
- datos de riesgo, salud, garantias, bienes, cuestionarios u observaciones libres;
- recibos, declaraciones, documentos, adjuntos, EIAC, incidencias o workflow;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

Regla explicita: `suplementos.read` no concede detalle, PII, importes, banco, documentos, recibos/declaraciones relacionados, workflows ni escrituras.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- El detalle y las secciones por tipo de suplemento permanecen bloqueadas hasta SDD posterior.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- Los campos PII, bancarios, financieros, de riesgo y documentos no se proyectan aunque existan en tablas/vistas heredadas.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de suplementos de otro broker.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma columnas, filtros y si `Suplementos` es pantalla independiente o subflujo de `Polizas`.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] Seguridad confirma que el primer corte no devuelve PII, importes, banco, documentos, riesgo ni textos libres.
- [ ] `GET /api/suplementos/catalogs` exige `suplementos.catalogs`.
- [ ] `GET /api/suplementos` exige `suplementos.read`.
- [ ] `suplementos.read` no devuelve detalle, PII, importes, banco, documentos ni acciones de escritura.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene detalle, exportacion, workflows y adjuntos bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-013-suplementos-read-only.md`.
- `docs/appbuilder/pages/suplementos/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Suplementos` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/suplementos/**`, reutilizando la ruta fixture actual sin activar acciones bloqueadas.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `suplementos.read`.
- QA: evidencia en `docs/qa/**`.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Logs sin referencias, PII, importes, banco, documentos ni textos sensibles en claro.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin detalle, PII, importes, banco, documentos, workflows ni escrituras en primer corte.
- [ ] Sin importacion/exportacion/actualizacion masiva en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de filtros, fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `suplementos.catalogs` o `suplementos.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta PII, importes, banco, documentos, riesgo, textos libres ni campos prohibidos;
- no se invocan workflows, escrituras, importacion, exportacion ni actualizacion masiva.

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
- detalle/exportacion/workflows/adjuntos siguen bloqueados;
- no se renderizan PII, importes reales, banco, documentos ni datos de riesgo;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma columnas, filtros y si la pagina es independiente o subflujo de Polizas;
- DBA confirma origen de lectura, vistas/tablas autorizadas y restricciones por broker;
- seguridad confirma minimizacion de PII, importes, banco, documentos, riesgo y textos libres.

## Riesgos

- Confundir tablas especializadas por tipo de suplemento con tabs reales de UI.
- Exponer datos de tomador, beneficiario, documento, email, telefono, direccion, IBAN o importes.
- Mostrar cambios de valor anterior/nuevo que revelen informacion financiera o de riesgo.
- Filtrar suplementos entre brokers por resolver conexion antes de validar contexto.
- Copiar SQL legacy incompleto o generico de `RepositorioBusqueda`.
- Activar alta, edicion, anulacion, importacion, exportacion o workflows sin SDD, auditoria ni rollback.
- Exponer documentos, recibos, declaraciones o EIAC relacionados.
- Usar catalogos/lookups historicos sin confirmar equivalencia funcional.
- Presentar `Suplementos` como MVP cerrado sin UAT ni metadata real.
- Reintroducir AppBuilder copiando componentes, eventos, datasources o workflows genericos.

## Work Items

- Crear issue GitHub: `SDD-2026-013 Suplementos read-only minimizado`.
- Crear tarea de producto/UAT para confirmar columnas, filtros y relacion con Polizas.
- Crear tarea DBA para confirmar origen de lectura, vistas/tablas autorizadas, filtros por broker y campos prohibidos.
- Crear tarea seguridad para clasificacion PII/importes/banco/documentos/riesgo y threat review del modulo.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `suplementos.catalogs` y `suplementos.read` definidos y probados.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
