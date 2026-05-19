# SDD: Clientes read-only minimizado

## Metadata

- Spec ID: SDD-2026-010
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: M
- Estado SDD: draft
- Responsable funcional: pendiente de asignar
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

Tras cerrar `Polizas CRUD BBDD` como vertical MVP local y abrir las SDD draft de `Siniestros` y `Recibos`, el carril de agentes por pagina avanza con `Clientes`. La evidencia AppBuilder en `docs/appbuilder/pages/clientes/README.md` y `docs/appbuilder/pages/clientes/components/*.md` confirma dominio historico de identidad/cliente, vistas relacionadas y componentes candidatos para listado, ficha y tabs. Sin embargo, no confirma una pantalla AppBuilder completa con `componentId`, layout, datasource, acciones, permisos historicos ni UAT.

El frontend actual ya expone `/clientes` como ruta protegida fixture/read-only, con datos anonimizados, filtros locales, PII bloqueada y acciones de ficha/exportacion/desglose deshabilitadas.

Decision de arquitectura: iLiniumTech no es un runtime dinamico tipo AppBuilder. Cualquier futura pantalla funcional de `Clientes` debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech, broker validado, SQL parametrizado y minimizacion estricta de PII.

## Objetivo

Preparar el primer incremento funcional de `Clientes` como listado read-only minimizado, sin ficha y sin tabs relacionadas:

- `GET /api/clientes/catalogs` para catalogos minimos aprobados.
- `GET /api/clientes` para listado paginado con filtros y ordenacion por whitelist.
- Frontend `/clientes` consumiendo contrato explicito, cuando exista backend aprobado.
- Permisos iniciales `clientes.catalogs` y `clientes.read`.
- Mantener ficha, documento legal, contacto, direccion, datos bancarios, metricas economicas, tabs relacionadas, exportacion y escrituras fuera del primer corte.

Esta SDD no autoriza aun implementacion con datos reales. Antes de programar API real deben resolverse UAT, DBA, permisos, origen de lectura, minimizacion PII y revision de seguridad.

## Fuera de alcance

- `GET /api/clientes/{clienteId}` en el primer incremento.
- Ficha resumen o detalle de cliente.
- Tabs de polizas, recibos, riesgos, siniestros o suplementos.
- Documento legal, tipo de documento, email, telefono, direccion, datos bancarios o anotaciones.
- Fechas personales, datos de salud, observaciones libres o informacion de contacto.
- Metricas economicas, primas, comisiones o resumen financiero.
- Busqueda por documento salvo decision PII posterior y SDD especifica.
- Exportacion, importacion, alta, edicion, baja, workflows o comunicaciones.
- Navegacion real a modulos relacionados.
- Permisos historicos AppBuilder convertidos automaticamente en permisos iLiniumTech.
- Uso runtime de metadata AppBuilder, `QueryStatic`, SQL heredado libre, vistas no validadas o endpoints genericos de pantalla/datasource.

## Contrato de datos

### Entradas de busqueda candidatas

Todas las entradas deben validarse en backend y traducirse a SQL parametrizado con estructura por whitelist:

| Campo | Tipo | Regla |
| --- | --- | --- |
| `texto` | string | Texto controlado para referencia interna o nombre mostrable minimizado; longitud maxima y redaccion en logs. |
| `estado` | string | Valor catalogado si producto confirma un estado funcional. |
| `segmento` | string | Valor catalogado si existe origen autorizado. |
| `gestor` | string/int | Solo si hay catalogo autorizado y no expone persona no aprobada. |
| `canalCobro` | string/int | Solo etiqueta general; no datos bancarios. |
| `fechaAltaDesde` | date | ISO date; no posterior a `fechaAltaHasta`. |
| `fechaAltaHasta` | date | ISO date. |
| `page` | integer | `>= 1`. |
| `pageSize` | integer | Rango cerrado, por ejemplo `1..100`. |
| `sort` | string | Solo campos whitelisted. |

No se permiten filtros por documento, email, telefono, direccion, cuenta bancaria, IBAN, datos personales ampliados, notas u observaciones en el primer corte.

### Salida `ClienteListItem` candidata

Campos maximos para primer corte, pendientes de UAT/DBA:

| Campo | Regla de minimizacion |
| --- | --- |
| `id` | Identificador opaco o id tecnico no reversible si es posible. |
| `referencia` | Referencia visible aprobada; no documento legal. |
| `nombreMostrable` | Alias, nombre comercial minimizado o etiqueta aprobada por UAT; nunca documento. |
| `tipoCliente` | Catalogo general si no expone PII sensible. |
| `estado` | Catalogo aprobado. |
| `segmento` | Catalogo aprobado. |
| `gestor` | Equipo o alias no personal si no hay permiso/persona aprobada. |
| `canalCobro` | Etiqueta general no bancaria. |
| `fechaAlta` | Fecha de alta si producto confirma su uso. |

Campos prohibidos en el primer corte:

- `NumDocumento`, tipo de documento o identificadores legales;
- nombre completo legal cuando no haya decision explicita de minimizacion;
- email, telefono, direccion, fecha de nacimiento, sexo, estado civil, profesion o datos personales ampliados;
- datos bancarios, IBAN, cuentas, mandatos o medios de cobro;
- primas, comisiones, metricas economicas, recibos o siniestros relacionados;
- observaciones, anotaciones o textos libres;
- SQL, nombres internos de tablas, connection strings, trazas o metadata AppBuilder.

Regla explicita: `clientes.read` no concede PII ampliada, ficha ni tabs relacionadas. Es solo permiso de listado minimizado.

## Reglas de negocio

- El primer incremento funcional es solo listado read-only minimizado.
- La ficha y las tabs relacionadas permanecen bloqueadas hasta SDD posterior.
- El backend valida sesion, permisos y broker antes de resolver origen de datos.
- El broker activo debe venir de sesion/claims backend; no de parametro libre del frontend.
- Los filtros y sort se implementan con whitelists en codigo iLiniumTech.
- Los campos PII, bancarios y financieros no se proyectan aunque existan en tablas/vistas heredadas.
- El frontend solo refleja capacidades devueltas por `/api/me`; la autorizacion real vive en backend.
- 403/404 no deben revelar existencia de clientes de otro broker.
- La metadata AppBuilder no gobierna UI, queries, permisos ni workflows en runtime.

## Criterios de aceptacion

- [ ] La SDD queda enlazada desde roadmap, plan maestro o paquete de agentes.
- [ ] Antes de implementar API real, producto confirma columnas y filtros del listado.
- [ ] Antes de implementar API real, DBA confirma origen de lectura autorizado y regla por broker.
- [ ] Seguridad confirma que el primer corte no devuelve documento, contacto, direccion, banco ni PII ampliada.
- [ ] `GET /api/clientes/catalogs` exige `clientes.catalogs`.
- [ ] `GET /api/clientes` exige `clientes.read`.
- [ ] `clientes.read` no devuelve ficha, tabs ni PII ampliada.
- [ ] El backend no consulta datos si falta broker o el broker no esta permitido.
- [ ] Filtros y sort fuera de whitelist devuelven error sanitizado.
- [ ] El frontend mantiene ficha, exportacion y desglose bloqueados en el primer corte.
- [ ] No se guardan secretos ni datos sensibles en Git.
- [ ] No se introduce runtime AppBuilder.

## Impacto tecnico

Documental inmediato:

- `docs/sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md`.
- `docs/appbuilder/pages/clientes/README.md`.
- `docs/appbuilder/pages/page-agent-rollout.md`.
- `docs/workflows/parallel-codex-task-pack.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.

Impacto futuro si se implementa:

- Backend: nueva feature `Clientes` en capas `Api`, `Application`, `Domain` e `Infrastructure`.
- Frontend: `iLiniumTech.Frontend/src/features/clientes/**`, reutilizando patrones actuales sin convertirlos en renderer generico.
- Router/menu: solo si la pagina pasa de fixture a funcional y se define permiso `clientes.read`.
- QA: evidencia en `docs/qa/**`.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL parametrizado y estructura por whitelist si se conecta BBDD.
- [ ] Logs sin datos sensibles ni busquedas PII en claro.
- [ ] Errores publicos sanitizados con `correlationId`.
- [ ] Broker validado antes de resolver conexion o consultar.
- [ ] Sin fallback silencioso a fixtures en modo backend real.
- [ ] Sin ficha, exportacion, documento, contacto, banco ni PII ampliada en primer corte.
- [ ] Sin filtros por documento, email, telefono, direccion o banco en primer corte.

## Plan de pruebas

Unitarias backend, cuando exista API:

- validacion de filtros, fechas, paginacion y sort;
- 401 sin sesion;
- 403 sin `clientes.catalogs` o `clientes.read`;
- broker ausente o no permitido no resuelve conexion ni consulta datos;
- filtros/sort fuera de whitelist rechazados;
- payloads maliciosos no aparecen en SQL;
- listado no proyecta documento, contacto, direccion, banco, notas ni PII prohibida.

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
- ficha/exportacion/desglose siguen bloqueados;
- no se renderiza PII real;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings ni metadata runtime.

Seguridad:

- secret scan limpio;
- dependency audit si se anaden paquetes;
- CORS audit si se toca API/configuracion;
- revision privacy/security antes de datos reales.

Manual/UAT:

- responsable funcional confirma columnas, filtros y orden inicial;
- DBA confirma origen de lectura y restricciones por broker;
- seguridad confirma minimizacion PII y ausencia de datos personales ampliados.

## Riesgos

- Exponer PII concentrada: documento, nombre completo, contacto, direccion o datos bancarios.
- Confundir `Identidad` con `Cliente` y mezclar companias, colaboradores u otras identidades.
- Filtrar clientes entre brokers por resolver conexion antes de validar contexto.
- Usar vistas `vw_Cliente*` como contrato sin confirmar layout, permisos ni UAT.
- Loguear busquedas por nombre o documento en claro.
- Abrir ficha o tabs relacionadas demasiado pronto y exponer polizas, recibos, riesgos o siniestros.
- Crear exportaciones masivas sin permisos, auditoria ni minimizacion.
- Reintroducir AppBuilder copiando tabs/CRUD genericos desde metadata.
- Interpretar metricas economicas o acuerdos sin permiso ni contexto funcional.

## Work Items

- Crear issue GitHub: `SDD-2026-010 Clientes read-only minimizado`.
- Crear tarea de producto/UAT para confirmar columnas y filtros del listado.
- Crear tarea DBA para confirmar origen de lectura, filtros por broker y campos prohibidos.
- Crear tarea seguridad para clasificacion PII y threat review del modulo.

## Actualizacion de implementacion 2026-05-18

- Implementado primer corte backend in-memory read-only: `GET /api/clientes/catalogs` y `GET /api/clientes`.
- Permisos implementados: `clientes.catalogs` y `clientes.read`.
- La implementacion no consulta SQL real, no devuelve documento, contacto, direccion, banco ni PII ampliada, y no habilita detalle/exportacion/escritura.
- Este corte no desbloquea datos reales; SQL y CRUD siguen bloqueados hasta UAT/DBA/security review y SDD de escritura si aplica.
- Tests dirigidos `ClientesApiTests`: OK.

## Actualizacion de implementacion 2026-05-19

- Por decision humana posterior, el desarrollo funcional debe orientarse a datos reales en BBDD local.
- Implementado repositorio SQL read-only activable con `Clientes:Repository=Sql`.
- Origen SQL del listado: `IdentidadCliente` + `Identidad`, con `Identidad.BrokerIntegracionId` como filtro de broker.
- La proyeccion no devuelve `NumDocumento`, email, telefono, direccion, IBAN ni datos bancarios.
- `NombreCompleto`/`RazonSocial` se usan como alias visible solo para el MVP local de datos reales; revisar matriz PII antes de cualquier entorno no local.
- El frontend `/clientes` consume API cuando `VITE_USE_BACKEND=true` y mantiene fixture solo en modo backend deshabilitado.
- Las escrituras siguen fuera de alcance hasta SDD CRUD propia.
- Evidencia: `docs/qa/clientes-sql-readonly-local-evidence.md`.

## Definicion de hecho

- [ ] SDD revisada por producto, backend/datos, frontend y seguridad.
- [ ] UAT/DBA confirma origen, columnas, filtros y minimizacion.
- [ ] Permisos `clientes.catalogs` y `clientes.read` definidos y probados.
- [ ] API y frontend implementados solo si los bloqueos externos se resuelven.
- [ ] Gates aplicables ejecutados y documentados.
- [ ] Riesgos residuales y bloqueos externos actualizados.
