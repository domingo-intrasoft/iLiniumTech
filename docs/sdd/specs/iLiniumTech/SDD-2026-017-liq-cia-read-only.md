# SDD: Liq.Cia read-only financiero minimizado

## Metadata

- Spec ID: SDD-2026-017
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-20

## Contexto

La decision vigente del producto pide orientar las pantallas funcionales a datos reales en BBDD local. `Liq.Cia` es una superficie de riesgo alto porque el dominio detectado contiene liquidaciones de compania, recibos, facturas, comisiones, importes, posibles datos bancarios, comentarios y relaciones con otras entidades.

La evidencia AppBuilder en `docs/appbuilder/pages/liq-cia/README.md` confirma que existe dominio de datos historico de liquidacion de compania y constantes/vistas candidatas como `LiquidacionCompania`, `LiquidacionCompaniaDetalle`, `vw_LiquidacionCia`, `vw_Recibo_LiqCia` y vistas de reporting. No confirma una pantalla AppBuilder completa con `componentId`, datasource real de pagina, columnas finales, filtros, acciones, permisos historicos, tabs o submenus.

El frontend actual expone `/liq-cia` como superficie Vue estatica, protegida, con fixture local sanitizado y acciones sensibles bloqueadas. Esa pantalla sirve como conversacion de producto y QA, pero no autoriza API, SQL real, importes reales, detalle financiero ni escrituras.

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. Si `Liq.Cia` se implementa, debe quedar como frontend Vue/TypeScript estatico y API .NET explicita, con permisos propios, broker validado, SQL parametrizado, whitelists, minimizacion financiera/PII y evidencia sin secretos.

## Objetivo

Preparar el primer incremento funcional futuro de `Liq.Cia` como consulta read-only minimizada, sin detalle financiero sensible:

- `GET /api/liq-cia` para listado paginado, con filtros y ordenacion por whitelist.
- `GET /api/liq-cia/catalogs` solo si DBA/UAT confirma catalogos seguros.
- Frontend `/liq-cia` consumiendo API explicita cuando exista backend aprobado.
- Permisos iniciales candidatos `liqCia.read` y `liqCia.catalogs`.
- Mantener detalle, desglose, exportacion, banco, facturas, cierre, validacion, importacion, conciliacion, recalculo y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen SQL, regla broker/tenant, minimizacion financiera/PII y revision de seguridad.

## Fuera de alcance

- Crear, editar, cerrar, reabrir, validar, recalcular o anular liquidaciones.
- Importacion/conversion de ficheros de compania.
- Conciliacion bancaria, movimientos bancarios o datos de cuenta.
- Facturas, detalle contable o generacion de documentos.
- Exportacion o reporting descargable.
- Detalle de recibos, movimientos, conceptos o desglose de importes.
- Comentarios, observaciones, textos libres, mediador/gestor detallado o datos de cliente.
- Primas, comisiones, liquido, importe bancario, importe total o desgloses reales en el primer corte.
- Navegacion generica a recibo, poliza, cliente, factura, banco o informe.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, endpoints genericos de pantalla/datasource o `SELECT *`.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `referencia` | string | Opcional; longitud maxima; solo identificador visible aprobado. |
| `compania` | string/int | Catalogo aprobado; no texto libre amplio. |
| `oficina` | string/int | Catalogo aprobado si UAT confirma uso. |
| `estado` | string | Solo si producto define estado funcional seguro. |
| `fechaDesde` | date | Fecha de liquidacion; rango acotado. |
| `fechaHasta` | date | No anterior a `fechaDesde`. |
| `fechaCierreDesde` | date | Solo si cierre se aprueba para listado. |
| `fechaCierreHasta` | date | Solo si cierre se aprueba para listado. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por importe, prima, comision, liquido, banco, factura libre, cliente, poliza, recibo, mediador, gestor, comentario, observacion, texto libre, `IdObjeto`, SQL, vista o nombre interno.

### Salida `LiqCiaListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada. |
| `compania` | Etiqueta de compania aprobada; sin datos de contacto. |
| `fecha` | Fecha de liquidacion si UAT la confirma. |
| `fechaCierre` | Opcional; solo si no revela estado sensible. |
| `estado` | Etiqueta funcional aprobada. |
| `oficina` | Etiqueta general aprobada. |
| `periodo` | Valor derivado seguro si negocio lo necesita. |
| `origen` | Etiqueta general, no tabla/vista interna. |

Campos prohibidos en el primer corte:

- importes reales, primas, comisiones, liquido, importe bancario, importe total, desglose, impuestos o diferencias;
- facturas, movimientos bancarios, cuenta, IBAN, conciliacion o identificadores bancarios;
- cliente, documento, email, telefono, direccion, comentarios, observaciones o textos libres;
- poliza, recibo, mediador, gestor o datos que permitan identificar personas si no hay permiso y minimizacion;
- nombres internos de tablas/vistas, SQL, connection strings, metadata AppBuilder o trazas tecnicas;
- detalle de recibos, conceptos, conversion/importacion o reporting.

Regla explicita: `liqCia.read` no concede detalle financiero, importes, banco, facturas, exportacion ni escrituras.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- La fuente SQL real debe confirmarla DBA; no basta con que existan mapeos EF o constantes historicas.
- El backend valida sesion, permisos y broker antes de resolver conexion o ejecutar consulta.
- El broker activo debe venir de sesion/claims backend, no de un parametro libre del frontend.
- Filtros y sort se implementan con whitelists en codigo iLiniumTech.
- El frontend solo refleja capacidades de `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de liquidaciones de otro broker.
- Importes, comisiones, banco, facturas, exportacion, cierre, validacion, importacion, conciliacion y escrituras quedan bloqueados.
- La metadata AppBuilder no gobierna UI, queries, permisos ni acciones en runtime.

## Criterios de aceptacion

- [ ] Producto confirma objetivo MVP de `Liq.Cia` y columnas/filtros minimos.
- [ ] DBA confirma origen de lectura autorizado, claves estables, indices y filtro broker/tenant.
- [ ] Seguridad confirma que el listado no devuelve importes, banco, facturas, PII, comentarios ni datos financieros sensibles.
- [ ] `GET /api/liq-cia` exige `liqCia.read`.
- [ ] `GET /api/liq-cia/catalogs`, si existe, exige `liqCia.catalogs` o `liqCia.read` de forma documentada.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene detalle, desglose, exportacion y operaciones financieras bloqueadas en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder ni SQL heredado libre.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-017-liq-cia-read-only.md`.
- `docs/appbuilder/pages/liq-cia/README.md`.
- `docs/qa/liq-cia-mvp-evidence.md`.
- `docs/qa/liquidaciones-compania-mvp-evidence.md`.
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`.
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `LiqCia` o `LiquidacionesCompania` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/liquidaciones-compania/**` o feature nueva acordada, sin metadata runtime.
- Menu: activar `Liq.Cia` solo con ruta y permiso `liqCia.read`.
- QA: evidencia especifica sin datos reales sensibles.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Logs sin importes, banco, facturas, PII, SQL completo ni filtros sensibles.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin exportacion, cierre, validacion, importacion, conciliacion ni escrituras en primer corte.
- [ ] Revision security/privacy antes de datos reales.

## Plan de pruebas

Unitarias backend, cuando exista API:

- 401 sin sesion;
- 403 sin `liqCia.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros, fechas, paginacion y sort validados;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta importes, banco, facturas, PII, comentarios ni campos prohibidos;
- no se invocan exportaciones, cierres, importacion, conciliacion ni escrituras.

Integracion/backend:

- `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`;
- smoke SQL solo con entorno autorizado, sin versionar credenciales ni datos.

Frontend:

- `npm run format`;
- `npm run lint`;
- `npm run test:unit`;
- `npm run build`;
- tests de ruta protegida, sin sesion, sin broker, sin permiso, loading, empty, error, filtros y paginacion;
- detalle, desglose, exportacion y operaciones financieras siguen bloqueadas;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- review privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma definicion de listado, columnas, filtros y acciones excluidas;
- DBA confirma fuente de lectura, regla broker, `SESSION_CONTEXT` si aplica y campos prohibidos;
- seguridad confirma minimizacion financiera, PII, banco, facturas, exportaciones y logging.

## Riesgos

- Confundir evidencia de dominio de datos con pantalla real de negocio.
- Exponer importes, comisiones, facturas o banco desde un listado aparentemente simple.
- Exponer PII indirecta por cliente, poliza, recibo, mediador, gestor, comentarios u observaciones.
- Mezclar `Liq.Cia` con `Liq.Col`, recibos, facturas, informes o contabilidad sin frontera funcional.
- Resolver conexion o ejecutar SQL antes de validar broker y permisos.
- Reintroducir AppBuilder copiando CRUD, eventos, datasources o acciones genericas.
- Crear exportaciones masivas sin permisos, auditoria ni minimizacion.
- Implementar cierres, validaciones, importaciones o conciliacion sin transacciones, rollback y UAT.

## Work Items

- Crear issue GitHub: `SDD-2026-017 Liq.Cia read-only financiero minimizado`.
- Crear tarea producto/UAT para confirmar objetivo MVP, columnas, filtros, estados y acciones excluidas.
- Crear tarea DBA para confirmar origen de lectura, regla broker, indices, claves estables y campos prohibidos.
- Crear tarea seguridad para clasificacion de importes, PII indirecta, banco, facturas, exportacion y logging.
- Crear SDD posterior si se aprueban detalle, importes financieros, exportacion o cualquier escritura.

## Actualizacion de implementacion 2026-05-20

- Esta SDD se crea como preparacion documental de `T-130-LIQCIA-SDD-READONLY`.
- No se modifica backend, frontend, SQL, configuracion ni runtime.
- `/liq-cia` sigue como superficie estatica fixture/read-only.
- Datos reales, API, detalle, importes, banco, facturas, exportacion y escrituras siguen bloqueados.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `liqCia.read` y `liqCia.catalogs` definidos y probados si se implementa API.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
