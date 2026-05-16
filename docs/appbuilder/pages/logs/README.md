# Pagina AppBuilder: Logs

Fecha: 2026-05-16

Rol IA: jefe de pagina `Logs`.

Ronda: solo documentacion. No se programa frontend, backend, extractor `Live` ni SQL. Esta documentacion es evidencia para analisis, SDD y scaffolding revisado; no es contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Motivo: `Logs` existe como entrada del menu objetivo de iLiniumTech, pero no se ha encontrado metadata local suficiente que identifique una pagina AppBuilder real con `menuId`, `componentId`, componente raiz, hijos, tabs, submenus, datasource de pantalla, filtros, acciones o permisos historicos concretos.

Si hay evidencias tecnicas de infraestructura de logs:

- tipo de BBDD `tipobd-LO`;
- contextos EF de logs;
- entidades y repositorios para `AuditLog`, `ErrorlLog`, `MailLog`, `ServiceLog` y `SmsLog`;
- registro de auditoria/error/mail/sms/servicios desde backend AppBuilder;
- referencias a request/response de REST/SOAP, workflow, login/auth y datasource migration log.

Esa evidencia prueba que AppBuilder registra logs, pero no prueba que exista una pantalla de menu `Logs` migrable tal cual. Por tanto, no se han creado documentos de componentes internos ni subagentes de pestanas/submenus.

## Regla base aplicada

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico.

Para `Logs`, esto implica:

- no crear una pantalla generica que lea tablas `IAPLOG_*` o `LogDbContext` sin SDD;
- no exponer request/response, bodies, stack traces, tokens, URLs internas, connection strings o datos personales;
- no convertir tablas historicas de log en contrato publico de API;
- no usar metadata `IAP_*`, menus AppBuilder ni catalogos heredados para decidir layout, permisos o queries en runtime;
- no ejecutar consultas SQL heredadas ni extractor `Live` para completar la pantalla;
- no habilitar descarga/exportacion de logs sin permiso, limites, redaccion y auditoria.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- documentos existentes en `docs/appbuilder/pages/*` como patron de inventario documental

Repositorio AppBuilder, solo lectura y evitando configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\TipoBBDDConst.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\DataBaseTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\AppLog\AppLogDbContext.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Log\LogDbContext.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppLog\IaplogAuditLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppLog\IaplogErrorlLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppLog\IaplogMailLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppLog\IaplogServiceLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppLog\IaplogSmsLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\Log\ServicioLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Interfaces\Log\IServicioLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiHelper\HelperLog.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\WorkFlow\WorkflowExecutor.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Helper\HelperAppModel.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Helper\HelperAppSolicitudes.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiAuth\Controllers\UserController.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiAuth\Controllers\ErrorController.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Type\App\ComponentDataSourceMigrationLogType.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Input\App\DataSourceMigrationLogInputType.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Mutation\Builder\App\DataSourceMigrationLogMutation.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\datasource\domain\iapDataSourceMigrationLog.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\datasource\application\DataSourceMigrationLogService.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\datasource\infrastructure\DataSourceMigrationLogApolloClientRepository.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ComponentDataSourceMigrationLog_ADD.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ComponentDataSourceMigrationLog_UPDATE.graphql`
- referencias frontend seguras a `setMailLog` desde auth/email dinamico.

Fuentes evitadas expresamente:

- `appsettings*`
- archivos `.config`
- connection strings
- dumps, backups, capturas y binarios
- datos personales reales
- scripts o seed de migracion con informacion de conexion
- extractor `Live`
- consultas SQL reales

## Busquedas ejecutadas

Busquedas locales con `rg`, acotadas por globs para evitar configuracion y binarios:

- `Logs`
- `Log`
- `AuditLog`
- `Auditoria`
- `ErrorlLog`
- `MailLog`
- `SmsLog`
- `ServiceLog`
- `Iaplog`
- `LogDbContext`
- `AppLogDbContext`
- `TipoBBDDConst.LOG`
- `tipobd-LO`
- `DataSourceMigrationLog`
- `setMailLog`

Resultado:

- No aparece una pagina Vue/AppBuilder concreta llamada `Logs`.
- No aparece metadata local sanitizada con fila real de menu `Logs`.
- No aparece `componentId` raiz asociado a una pagina `Logs`.
- No aparecen pestanas ni submenus reales de `Logs`.
- Si aparecen piezas tecnicas de logging y auditoria que podrian alimentar una futura SDD.

## Evidencia de menu actual iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Logs` aparece como item de primer nivel:

| Campo | Valor |
| --- | --- |
| Etiqueta | `Logs` |
| Icono | `pi pi-database` |
| Ruta | No definida |
| Estado | `disabled: true` |
| Permiso | No definido |
| Hijos | No definidos |

Interpretacion: hay intencion de navegacion futura, pero no existe pagina iLiniumTech implementada ni contrato funcional aprobado.

## Evidencia AppBuilder encontrada

### Tipo de conexion LOG

El tipo de BBDD `LOG` aparece definido como `tipobd-LO` en backend y frontend:

- `TipoBBDDConst.LOG`
- `DataBaseTypeConst.LOG`

Tambien aparece documentado en iLiniumTech como tipo de conexion AppBuilder. Esto confirma que AppBuilder separa o reconoce una conexion de log por broker/aplicacion.

### Contextos y tablas de logs

`AppLogDbContext` declara entidades orientadas a tablas/vistas `IAPLOG_*`:

| DbSet | Tabla |
| --- | --- |
| `IaplogAuditLogs` | `IAPLOG_AuditLog` |
| `IaplogErrorlLogs` | `IAPLOG_ErrorlLog` |
| `IaplogMailLogs` | `IAPLOG_MailLog` |
| `IaplogServiceLogs` | `IAPLOG_ServiceLog` |
| `IaplogSmsLogs` | `IAPLOG_SmsLog` |

`LogDbContext` declara otro conjunto historico o paralelo:

| DbSet | Tabla |
| --- | --- |
| `AuditLogs` | `AuditLog` |
| `CompanyLogs` | `CompanyLog` |
| `CrmentitiesLogs` | `CRMEntitiesLog` |
| `ErrorlLogs` | `ErrorlLog` |
| `IntegrationServiceLogs` | `IntegrationServiceLog` |
| `MailLogs` | `MailLog` |
| `MailLogAttachments` | `MailLogAttachment` |
| `SystemLogs` | `SystemLog` |

Interpretacion: hay dominio de datos de logs, pero no hay evidencia de que todos estos grupos fueran tabs de una pagina `Logs`.

### Servicio de escritura de logs

`ServicioLog` implementa operaciones `Add` y `SaveChanges*` para:

- auditoria;
- errores;
- mail;
- servicios;
- SMS.

`IServicioLog` expone la misma forma de servicio. Varias partes de AppBuilder usan ese servicio para registrar operaciones tecnicas, autenticacion, errores, workflows, scheduler, llamadas REST/SOAP o envio de comunicaciones.

Interpretacion: `Logs` tiene mucha importancia operativa, pero aparece como servicio transversal de escritura, no como pantalla identificada de consulta.

### Auditoria y errores

`HelperLog` registra auditoria y errores. La documentacion existente de AppBuilder tambien advierte que:

- las ejecuciones de workflow pueden registrar nodos, entradas, salidas y estados;
- las llamadas REST/SOAP pueden registrar request/response;
- existe BBDD `LOG` como tipo de conexion;
- los logs deben usar ids logicos y nunca connection strings.

Esto refuerza que cualquier futura pantalla de `Logs` debe tratar la informacion como sensible.

### DataSourceMigrationLog

Hay una pieza distinta llamada `DataSourceMigrationLog`:

- GraphQL type/input/mutation `ComponentDataSourceMigrationLog`;
- frontend repository/service para `add` y `update`;
- campos principales: `id`, `dataSourceMigrationId`, `description`, `fcr`, `ucr`.

Interpretacion: esto parece una bitacora tecnica de migracion/configuracion de datasources, no una pagina general de `Logs`. No debe tratarse como tab real de `Logs` sin metadata adicional.

### Mail/SMS logs

Hay referencias a `MailLog` y `SmsLog`, y el frontend AppBuilder puede invocar `setMailLog` desde flujos de email/autenticacion. Los campos incluyen destinatarios, cuerpo, asunto, mensaje y referencia.

Interpretacion: contienen PII y contenido de comunicaciones. Una pantalla futura no debe mostrar cuerpo, destinatarios completos ni payloads sin permiso especifico, minimizacion y auditoria.

## Evidencia ausente

No se ha encontrado evidencia local suficiente de:

- `menuId` de AppBuilder para `Logs`;
- `componentId` raiz;
- arbol de componentes hijo;
- datasource de pantalla;
- tabs o submenus reales;
- filtros configurados;
- columnas reales de grid;
- acciones de UI historicas;
- permisos historicos asociados al menu;
- localizacion/label de pagina AppBuilder concreta;
- capturas o metadata sanitizada de la pantalla.

## Componentes internos

No se crean `components/*.md`.

Razon: no hay pestanas, submenus ni componentes internos reales de `Logs` detectados. Las categorias tecnicas `AuditLog`, `ErrorlLog`, `MailLog`, `ServiceLog`, `SmsLog`, `SystemLog`, `IntegrationServiceLog` o `DataSourceMigrationLog` podrian convertirse en secciones futuras, pero hoy no hay evidencia de que fueran tabs/submenus de la pagina AppBuilder.

Crear esos subdocumentos ahora generaria falsa trazabilidad y podria empujar a desarrollar una pantalla inventada.

## Datos candidatos si producto desbloquea la pagina

Solo como inventario de dominio tecnico, no como contrato:

| Dominio | Evidencia | Sensibilidad |
| --- | --- | --- |
| Auditoria | `IAPLOG_AuditLog`, `AuditLog` | Alta: request, response, IP, user agent, usuario, sesion |
| Errores | `IAPLOG_ErrorlLog`, `ErrorlLog` | Alta: stack trace, mensaje, server, user agent |
| Mail | `IAPLOG_MailLog`, `MailLog` | Muy alta: from, to, cc, subject, body |
| SMS | `IAPLOG_SmsLog` | Muy alta: telefono, body, mensaje |
| Servicios | `IAPLOG_ServiceLog`, `CompanyLog`, `IntegrationServiceLog` | Alta: request/response, endpoints, payloads |
| Sistema | `SystemLog` | Alta: exception, logger, message |
| Migracion datasource | `IapDataSourceMigrationLog` | Media/alta: descripcion tecnica y usuario creador |

Antes de desarrollar, producto/seguridad debe decidir si la pagina existe como herramienta interna de soporte, auditoria, operaciones, seguridad o administracion.

## Filtros candidatos para una SDD futura

No son filtros confirmados de AppBuilder. Son candidatos razonables si se aprueba producto:

- rango de fechas obligatorio;
- tipo de log: auditoria, errores, mail, sms, servicios, sistema;
- nivel: info, warning, error, critical, si el origen lo soporta;
- metodo/proceso;
- usuario interno;
- broker o aplicacion, siempre derivado de auth backend;
- `correlationId` o id externo;
- estado/error;
- busqueda textual limitada y redactada.

Regla recomendada: toda busqueda debe tener limite temporal y paginacion estricta para evitar extraccion masiva.

## Acciones candidatas para una SDD futura

Primer incremento recomendado si se desbloquea:

- listado read-only minimizado;
- detalle redaccionado por permiso;
- copiar `correlationId` o id tecnico no sensible;
- filtros por fecha/tipo/estado;
- paginacion server-side.

Acciones bloqueadas hasta nueva SDD:

- exportacion;
- descarga de payloads;
- ver cuerpo completo de email/SMS;
- ver request/response completos;
- reintentar integraciones;
- reenviar comunicaciones;
- borrar logs;
- modificar retencion;
- consultar stack trace completo.

## Permisos candidatos iLiniumTech

No estan aprobados. Propuesta para discusion:

| Permiso | Uso |
| --- | --- |
| `logs.read` | Ver pagina general minimizada |
| `logs.audit.read` | Ver auditoria redaccionada |
| `logs.errors.read` | Ver errores redaccionados |
| `logs.services.read` | Ver servicios/integraciones redaccionados |
| `logs.mail.read` | Ver metadata de mail sin body ni destinatarios completos |
| `logs.sms.read` | Ver metadata de SMS sin body ni telefono completo |
| `logs.detail` | Abrir detalle redaccionado |
| `logs.export` | Exportacion limitada, si se aprueba |
| `logs.payload.read` | Ver payload sensible, solo con seguridad/UAT |

El backend debe aplicar permisos antes de resolver BBDD o leer datos. La UI solo puede ocultar capacidades; no es barrera de seguridad.

## Propuesta Vue/API estatica si se desbloquea

### Frontend

Rutas candidatas:

- `/logs`
- `/logs/:type/:id`

Componentes estaticos, no metadata runtime:

- `LogsView.vue`
- `LogsFilters.vue`
- `LogsTable.vue`
- `LogDetailView.vue`
- `logsTypes.ts`
- `logsService.ts`

Estados obligatorios:

- sin sesion;
- sin broker;
- sin permiso;
- rango de fechas obligatorio;
- loading;
- empty;
- error sanitizado;
- datos redaccionados;
- detalle no disponible por permiso.

### Backend

Endpoints candidatos:

- `GET /api/logs/catalogs`
- `GET /api/logs`
- `GET /api/logs/{type}/{id}`

Contrato recomendado:

- whitelists de tipos y columnas;
- filtros parametrizados;
- page/pageSize con maximo estricto;
- rango de fechas obligatorio;
- redaccion por permiso;
- broker y aplicacion desde contexto autenticado;
- errores con `correlationId`;
- sin `SELECT *`;
- sin SQL libre heredado;
- sin fallback a headers MVP en entornos con datos reales.

## Riesgos principales

- Fuga de PII: emails, telefonos, cuerpos de mensajes, documentos, nombres, direcciones o payloads de negocio.
- Fuga de secretos: tokens, API keys, cookies, headers, connection strings o URLs internas dentro de request/response.
- Fuga de infraestructura: servidor, base, stack trace, rutas fisicas, nombres internos.
- Enumeracion masiva de actividad por usuario, broker o fecha.
- Mezcla de brokers si se resuelve `tipobd-LO` antes de validar `currentBrokerId`.
- Exponer errores internos como detalle publico.
- Convertir logs historicos AppBuilder en API generica sin minimizacion.
- Crear una pagina de administracion sensible sin auth productiva ni matriz de permisos.
- Usar datos de logs para UAT sin anonimizacion.

## Pruebas obligatorias si se desarrolla

Backend:

- 401 anonimo;
- 403 sin permiso;
- 403 broker cruzado antes de SQL;
- validacion de rango de fechas obligatorio;
- pageSize maximo;
- whitelist de tipo de log;
- whitelist de sort;
- payloads maliciosos en filtros;
- redaccion de PII/secretos;
- errores sanitizados con `correlationId`;
- ausencia de connection strings y stack traces en respuesta publica.

Frontend:

- guard de sesion;
- guard de permiso;
- filtros iniciales y limpieza;
- rango de fechas obligatorio;
- listado paginado;
- detalle redaccionado;
- error 401/403;
- no renderizar `connectionString`, SQL, tokens, headers sensibles ni payloads completos.

QA/seguridad:

- `Test-DocumentationBaseline.ps1`;
- `Invoke-SecretScan.ps1`;
- auditoria de dependencias si toca frontend/backend;
- CORS audit si toca API/configuracion;
- smoke visual solo con datos anonimizados o fixtures;
- revision manual de DOM y payloads para no exponer datos sensibles.

## Bloqueos externos

- Falta metadata sanitizada real de la pagina `Logs`.
- Falta confirmar si `Logs` es un modulo de producto, soporte, auditoria, seguridad u operacion.
- Falta propietario funcional/UAT.
- Falta matriz de permisos real.
- Falta decision de auth productiva.
- Falta politica de retencion y minimizacion.
- Falta DBA/seguridad para validar tablas/vistas autorizadas.
- Falta decision sobre si una herramienta de logs debe consultar BBDD historica AppBuilder o una observabilidad nueva de iLiniumTech.

## Decision recomendada

No desarrollar `Logs` todavia.

Abrir una SDD solo si producto confirma:

1. quien usara la pagina;
2. que problema operativo resuelve;
3. que fuentes de log estan autorizadas;
4. que campos se pueden ver y cuales deben redactarse;
5. que permisos y broker aplican;
6. cual es el rango de fechas maximo;
7. si se permiten detalle, exportacion o payload completo.

Hasta entonces, mantener `Logs` deshabilitado en el menu iLiniumTech.

## Estado de cierre de esta ronda

| Punto | Estado |
| --- | --- |
| Entrada de menu iLiniumTech localizada | Completado con evidencia |
| Tipo de BBDD `LOG` identificado | Completado con evidencia |
| Dominios tecnicos de logs inventariados | Completado con evidencia |
| Pagina AppBuilder real identificada | Bloqueado externo |
| Componentes internos/tabs/submenus identificados | Bloqueado externo |
| Desarrollo frontend/backend | Fuera de alcance |
| Subagentes de componentes | No creados por falta de evidencia |
| Riesgos de seguridad/PII | Documentados |
