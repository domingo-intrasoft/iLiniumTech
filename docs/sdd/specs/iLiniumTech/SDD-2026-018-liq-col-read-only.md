# SDD: Liq.Col read-only financiero minimizado

## Metadata

- Spec ID: SDD-2026-018
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-20

## Contexto

La decision vigente del producto pide orientar las pantallas funcionales a datos reales en BBDD local. `Liq.Col` es una superficie de riesgo alto porque el dominio detectado contiene liquidaciones de colaborador, recibos, acuerdos de reparto, comisiones, impuestos, retenciones, liquidos, posibles documentos identificativos, motivos libres y relaciones con `Liq.Cia`, tesoreria y reporting.

La evidencia AppBuilder en `docs/appbuilder/pages/liq-col/README.md` confirma dominio de datos historico de liquidacion de colaborador: `LiquidacionIdentidad`, `LiquidacionIdentidadDetalle`, `LiquidacionIdentidadConcepto`, `LiquidacionIdentidadExclusion`, `vw_Recibo_LiqCol` y vistas de reporting. No confirma una pantalla AppBuilder completa con `componentId`, datasource real de pagina, columnas finales, filtros, acciones, permisos historicos, tabs o submenus.

El frontend actual expone `/liq-col` como superficie Vue estatica, protegida, con fixture local sanitizado y acciones sensibles bloqueadas. Esa pantalla sirve como conversacion de producto y QA, pero no autoriza API, SQL real, comisiones reales, liquidos, retenciones, detalle financiero ni escrituras.

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. Si `Liq.Col` se implementa, debe quedar como frontend Vue/TypeScript estatico y API .NET explicita, con permisos propios, broker validado, SQL parametrizado, whitelists, minimizacion financiera/PII y evidencia sin secretos.

## Objetivo

Preparar el primer incremento funcional futuro de `Liq.Col` como consulta read-only minimizada, sin detalle financiero sensible:

- `GET /api/liq-col` para listado paginado, con filtros y ordenacion por whitelist.
- `GET /api/liq-col/catalogs` solo si DBA/UAT confirma catalogos seguros.
- Frontend `/liq-col` consumiendo API explicita cuando exista backend aprobado.
- Permisos iniciales candidatos `liqCol.read` y `liqCol.catalogs`.
- Mantener detalle, conceptos, exclusiones, exportacion, comisiones reales, liquidos, retenciones, banco, cierre, validacion, recalculo y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen SQL, regla broker/tenant, minimizacion financiera/PII y revision de seguridad.

## Fuera de alcance

- Crear, editar, cerrar, reabrir, validar, recalcular, excluir o anular liquidaciones.
- Generar liquidaciones o recalcular acuerdos de reparto.
- Detalle de recibos, conceptos, exclusiones o relacion con `Liq.Cia`.
- Exportacion, impresion o reporting descargable.
- Comisiones reales, retenciones, liquidos, impuestos, primas, bases o porcentajes en el primer corte.
- Documentos de colaborador/cliente, banco, tesoreria, facturas o datos de cuenta.
- Motivos libres, observaciones, comentarios, validaciones o textos no revisados.
- Navegacion generica a recibo, poliza, cliente, colaborador, `Liq.Cia`, tesoreria o informe.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, endpoints genericos de pantalla/datasource o `SELECT *`.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `referencia` | string | Opcional; longitud maxima; solo identificador visible aprobado. |
| `colaborador` | string/int | Catalogo aprobado; no documento ni texto libre amplio. |
| `oficina` | string/int | Catalogo aprobado si UAT confirma uso. |
| `estado` | string | Solo si producto define estado funcional seguro. |
| `fechaDesde` | date | Fecha de liquidacion; rango acotado. |
| `fechaHasta` | date | No anterior a `fechaDesde`. |
| `fechaCierreDesde` | date | Solo si cierre se aprueba para listado. |
| `fechaCierreHasta` | date | Solo si cierre se aprueba para listado. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por documento, cliente, mediador, poliza, recibo, comision, liquido, retencion, importe, prima, impuesto, motivo, exclusion, banco, tesoreria, factura, texto libre, `IdObjeto`, SQL, vista o nombre interno.

### Salida `LiqColListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada. |
| `colaborador` | Etiqueta anonimizada o alias aprobado; sin documento. |
| `fecha` | Fecha de liquidacion si UAT la confirma. |
| `fechaCierre` | Opcional; solo si no revela estado sensible. |
| `estado` | Etiqueta funcional aprobada. |
| `oficina` | Etiqueta general aprobada. |
| `resumen` | Conteo o etiqueta general, nunca importes. |
| `alcance` | Etiqueta general, no tabla/vista interna. |

Campos prohibidos en el primer corte:

- comisiones reales, retenciones, liquidos, impuestos, primas, bases, porcentajes, importes o diferencias;
- documentos identificativos, cliente, mediador, email, telefono, direccion o PII directa;
- poliza, recibo, acuerdos de reparto o referencias que identifiquen operaciones individuales si no hay permiso y minimizacion;
- motivos, observaciones, exclusiones, validaciones o textos libres;
- banco, tesoreria, facturas, movimientos o cuentas;
- nombres internos de tablas/vistas, SQL, connection strings, metadata AppBuilder o trazas tecnicas;
- detalle de recibos, conceptos, exclusiones, relacion con `Liq.Cia` o reporting.

Regla explicita: `liqCol.read` no concede comisiones, liquidos, retenciones, detalle, conceptos, exclusiones, exportacion ni escrituras.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- La fuente SQL real debe confirmarla DBA; no basta con que existan mapeos EF o constantes historicas.
- El backend valida sesion, permisos y broker antes de resolver conexion o ejecutar consulta.
- El broker activo debe venir de sesion/claims backend, no de un parametro libre del frontend.
- Filtros y sort se implementan con whitelists en codigo iLiniumTech.
- El frontend solo refleja capacidades de `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de liquidaciones de otro broker.
- Comisiones, liquidos, retenciones, detalle, conceptos, exclusiones, banco, exportacion, cierre, validacion, recalculo y escrituras quedan bloqueados.
- La metadata AppBuilder no gobierna UI, queries, permisos ni acciones en runtime.

## Criterios de aceptacion

- [ ] Producto confirma objetivo MVP de `Liq.Col` y columnas/filtros minimos.
- [ ] DBA confirma origen de lectura autorizado, claves estables, indices y filtro broker/tenant.
- [ ] Seguridad confirma que el listado no devuelve comisiones, liquidos, retenciones, PII, documentos, motivos libres ni datos financieros sensibles.
- [ ] `GET /api/liq-col` exige `liqCol.read`.
- [ ] `GET /api/liq-col/catalogs`, si existe, exige `liqCol.catalogs` o `liqCol.read` de forma documentada.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene detalle, conceptos, exportacion y operaciones financieras bloqueadas en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder ni SQL heredado libre.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-018-liq-col-read-only.md`.
- `docs/appbuilder/pages/liq-col/README.md`.
- `docs/qa/liq-col-mvp-evidence.md`.
- `docs/qa/liquidaciones-colaborador-mvp-evidence.md`.
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`.
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `LiqCol` o `LiquidacionesColaborador` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/**` o feature nueva acordada, sin metadata runtime.
- Menu: activar `Liq.Col` solo con ruta y permiso `liqCol.read`.
- QA: evidencia especifica sin datos reales sensibles.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Logs sin comisiones, liquidos, retenciones, documentos, PII, motivos libres, SQL completo ni filtros sensibles.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin exportacion, cierre, validacion, recalculo, exclusion ni escrituras en primer corte.
- [ ] Revision security/privacy antes de datos reales.

## Plan de pruebas

Unitarias backend, cuando exista API:

- 401 sin sesion;
- 403 sin `liqCol.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros, fechas, paginacion y sort validados;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta comisiones, liquidos, retenciones, documentos, PII, motivos libres ni campos prohibidos;
- no se invocan exportaciones, cierres, validaciones, recalculos, exclusiones ni escrituras.

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
- detalle, conceptos, exportacion y operaciones financieras siguen bloqueadas;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- review privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma definicion de listado, columnas, filtros y acciones excluidas;
- DBA confirma fuente de lectura, regla broker, `SESSION_CONTEXT` si aplica y campos prohibidos;
- seguridad confirma minimizacion financiera, PII, documentos, motivos libres, exportaciones y logging.

## Riesgos

- Confundir evidencia de dominio de datos con pantalla real de negocio.
- Exponer comisiones, liquidos, retenciones o documentos desde un listado aparentemente simple.
- Exponer PII por colaborador, cliente, mediador, poliza, recibo, documento, motivos u observaciones.
- Mezclar `Liq.Col` con `Liq.Cia`, recibos, acuerdos de reparto, tesoreria o reporting sin frontera funcional.
- Resolver conexion o ejecutar SQL antes de validar broker y permisos.
- Reintroducir AppBuilder copiando CRUD, eventos, datasources o acciones genericas.
- Crear exportaciones masivas sin permisos, auditoria ni minimizacion.
- Implementar cierres, validaciones, recalculos o exclusiones sin transacciones, rollback y UAT.

## Work Items

- Crear issue GitHub: `SDD-2026-018 Liq.Col read-only financiero minimizado`.
- Crear tarea producto/UAT para confirmar objetivo MVP, identidad funcional, columnas, filtros, estados y acciones excluidas.
- Crear tarea DBA para confirmar origen de lectura, regla broker, indices, claves estables y campos prohibidos.
- Crear tarea seguridad para clasificacion de comisiones, liquidos, documentos, motivos libres, PII, exportacion y logging.
- Crear SDD posterior si se aprueban detalle, comisiones, conceptos, exclusiones, exportacion o cualquier escritura.

## Actualizacion de implementacion 2026-05-20

- Esta SDD se crea como preparacion documental de `T-051-LIQCOL-SDD-READONLY`.
- No se modifica backend, frontend, SQL, configuracion ni runtime.
- `/liq-col` sigue como superficie estatica fixture/read-only.
- Datos reales, API, detalle, conceptos, comisiones, liquidos, retenciones, banco, exportacion y escrituras siguen bloqueados.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `liqCol.read` y `liqCol.catalogs` definidos y probados si se implementa API.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
