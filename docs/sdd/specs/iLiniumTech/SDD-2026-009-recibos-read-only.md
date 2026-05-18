# SDD: Recibos read-only minimizado

## Metadata

- Spec ID: SDD-2026-009
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local y abrir `SDD-2026-008 Siniestros`, el carril de agentes por pagina avanza con `Recibos`. La evidencia AppBuilder en `docs/appbuilder/pages/recibos/README.md` confirma dominio historico de recibos, vistas auxiliares, busqueda generica y relaciones con polizas, clientes, liquidaciones, cobro, EIAC y aduana. Sin embargo, no confirma una pantalla AppBuilder completa con `componentId`, layout, tabs, datasource, acciones, permisos historicos ni UAT.

El frontend actual ya expone `/recibos` como ruta protegida fixture/read-only, con datos sanitizados, filtros locales, importes demo anonimizados y acciones de detalle/exportacion/cobro bloqueadas.

Decision de arquitectura: iLiniumTech no es un runtime dinamico tipo AppBuilder. Cualquier futura pantalla funcional de `Recibos` debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado, SQL parametrizado y minimizacion financiera/PII.

Actualizacion 2026-05-18: se implementa el primer contrato backend in-memory/read-only para `Recibos` con `GET /api/recibos/catalogs` y `GET /api/recibos`. Este avance no activa SQL real, datos reales, importes reales, banco, remesas, cobro, detalle, exportacion ni escrituras; sirve como base tecnica para adaptar el frontend y validar permisos propios.

## Objetivo

Preparar el primer incremento funcional de `Recibos` como listado read-only minimizado, sin importes reales, sin detalle, sin cobro y sin exportacion:

- `GET /api/recibos/catalogs` para catalogos minimos aprobados.
- `GET /api/recibos` para listado paginado con filtros y ordenacion por whitelist.
- Frontend `/recibos` consumiendo contrato explicito, cuando exista backend aprobado.
- Permisos iniciales `recibos.catalogs` y `recibos.read`.
- Mantener importes reales, datos bancarios, remesas, detalle, EIAC, exportacion y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura, minimizacion financiera/PII y revision de seguridad.

## Fuera de alcance

- `GET /api/recibos/{id}` en el primer incremento.
- Detalle de recibo.
- Importes reales: prima neta, prima total, comisiones, impuestos, liquidaciones o diferencias.
- Datos bancarios, IBAN, cuenta, remesas, titularidad bancaria o gestor de cobro sensible.
- Cobro, modificacion de situacion, conciliacion EIAC, incidencias, devoluciones o documentos.
- Exportacion de listado o detalle.
- Relacion navegable real con cliente, liquidaciones, documentos o comunicaciones.
- Alta, edicion, anulacion, escritura o workflow.
- Permisos historicos AppBuilder convertidos automaticamente en permisos iLiniumTech.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, vistas no validadas o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `recibo` | string | Referencia de recibo o recibo compania confirmada por UAT; no buscar por documento ni cuenta. |
| `poliza` | string | Numero de poliza visible y minimizado. |
| `situacion` | string | Valor catalogado. |
| `tipo` | string | Valor catalogado si producto lo confirma. |
| `compania` | string | Codigo o id catalogado si DBA/UAT lo aprueba. |
| `fechaVencimientoDesde` | date | ISO date; no posterior a `fechaVencimientoHasta`. |
| `fechaVencimientoHasta` | date | ISO date. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por importe, comision, cuenta, IBAN, documento, telefono, email, direccion, observaciones ni texto libre financiero en el primer corte.

### Salida `ReciboListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `recibo` | Referencia visible aprobada; sin identificadores legacy innecesarios. |
| `poliza` | Numero de poliza visible; no incluir tomador completo. |
| `compania` | Nombre/codigo normalizado si no expone datos sensibles. |
| `tipo` | Catalogo aprobado. |
| `situacion` | Catalogo aprobado. |
| `fechaEfecto` | Fecha de efecto si producto la confirma. |
| `fechaVencimiento` | Fecha de vencimiento. |
| `estadoCobro` | Etiqueta no operativa y no bancaria, solo si UAT la aprueba. |
| `canal` | Etiqueta general no sensible, sin cuenta ni remesa. |

Campos prohibidos en el primer corte:

- prima neta, prima total, comision, impuestos, liquidacion, diferencias o cualquier importe real;
- cuenta bancaria, IBAN, remesa, mandato, titular bancario o datos de cobro sensibles;
- documento, telefono, email, direccion o datos personales de cliente/tomador;
- observaciones, incidencias, comunicaciones o textos libres;
- datos EIAC/aduana;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

Regla explicita: `recibos.read` no concede importes reales. Los importes requieren SDD posterior y permiso separado, por ejemplo `recibos.financial`.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- El detalle permanece bloqueado hasta SDD posterior.
- Los importes reales permanecen bloqueados hasta SDD posterior con permiso `recibos.financial`.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- Los campos financieros, bancarios y PII no se proyectan aunque existan en tablas/vistas heredadas.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de recibos de otro broker.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma columnas y filtros del listado.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] Seguridad confirma que el primer corte no devuelve importes reales, banco ni PII no aprobada.
- [ ] `GET /api/recibos/catalogs` exige `recibos.catalogs`.
- [ ] `GET /api/recibos` exige `recibos.read`.
- [x] Primer backend in-memory/read-only implementado sin SQL real, sin importes reales y sin banco.
- [ ] `recibos.read` no devuelve importes reales.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene detalle, exportacion, cobro, remesas y datos bancarios bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-009-recibos-read-only.md`.
- `docs/appbuilder/pages/recibos/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Recibos` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/recibos/**`, reutilizando patrones actuales sin convertirlos en renderer generico.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `recibos.read`.
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
- [ ] Sin detalle, exportacion, cobro, banco ni importes reales en primer corte.
- [ ] Sin filtros por importe o banco en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de filtros, fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `recibos.catalogs` o `recibos.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta importes, comisiones, banco, remesas ni PII prohibida.

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
- detalle/exportacion/cobro siguen bloqueados;
- no se renderizan importes reales;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma columnas, filtros y orden inicial;
- DBA confirma origen de lectura y restricciones por broker;
- seguridad confirma minimizacion financiera/PII y ausencia de importes reales.

## Riesgos

- Mostrar importes o comisiones sin matriz de permisos y UAT.
- Interpretar mal primas, impuestos, liquidaciones, situacion de cobro o diferencias.
- Exponer datos bancarios, remesas, cuenta, IBAN o titularidad.
- Exponer PII de cliente/tomador por reutilizar vistas heredadas.
- Filtrar recibos entre brokers por resolver conexion antes de validar contexto.
- Usar vistas PBI, EIAC, aduana o reporting sin validacion DBA/UAT.
- Permitir filtros por importes que revelen informacion aunque no se muestre la cantidad.
- Crear exportaciones masivas sin permisos, auditoria ni minimizacion.
- Loguear importes, cuentas, documentos, referencias internas o textos libres.
- Confundir busqueda/gestion generica AppBuilder con pantalla real completa.

## Work Items

- Crear issue GitHub: `SDD-2026-009 Recibos read-only minimizado`.
- Crear tarea de producto/UAT para confirmar columnas y filtros del listado.
- Crear tarea DBA para confirmar origen de lectura, filtros por broker y campos prohibidos.
- Crear tarea seguridad para clasificacion financiera/PII y threat review del modulo.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `recibos.catalogs` y `recibos.read` definidos y probados.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
