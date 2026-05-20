# Pagina Conectividad - Analisis AppBuilder

Fecha: 2026-05-16

Jefe de pagina: Conectividad

Ronda: solo documentacion. No se ha programado, no se ha ejecutado SQL, no se ha ejecutado extractor `Live` y no se han abierto `appsettings`, `.config`, dumps, capturas sensibles ni datos personales reales.

## Estado final

`bloqueado externo`

`Conectividad` existe como entrada del menu objetivo de iLiniumTech, pero no hay metadata local suficiente para desarrollar una pagina funcional equivalente de AppBuilder. No se ha encontrado una pantalla real con `menuId`, `componentId`, raiz, hijos, tabs, submenus, datasource, filtros, acciones ni permisos historicos propios.

Si en el futuro se quiere desarrollar esta pagina, debe abrirse SDD propia y confirmar con producto/seguridad/DBA si el objetivo es:

- una pantalla operativa de salud de integraciones;
- una administracion de endpoints/integraciones;
- una consola tecnica de conectividad;
- o una capacidad nueva sin equivalente directo en AppBuilder.

Hasta entonces, no debe implementarse nada funcional bajo `Conectividad`.

Actualizacion 2026-05-20: se crea [`docs/engineering/conectividad-threat-model.md`](../../../engineering/conectividad-threat-model.md). El threat model es requisito previo para cualquier API, llamada externa, prueba REST/SOAP, SQL, lectura real, ejecucion de conectores o gestion de secretos; no autoriza todavia datos reales ni payloads.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/appbuilder/pages/README.md`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`
- documentos existentes bajo `docs/appbuilder/pages/**` como referencia de formato y criterios de bloqueo.

Repositorio AppBuilder, solo codigo fuente no sensible:

- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/infrastructure/Component/DataSourceComp.vue`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/infrastructure/Component/NewDataSourceWS.vue`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/infrastructure/Component/NewDataSourceSWConfiguration.vue`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/infrastructure/Component/DataSourceWSPreview.vue`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/application/IServiceDataSourceService.ts`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/datasource/domain/IDataSourceServiceRepository.ts`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/catalog/domain/const/CatalogServiceType.ts`
- `C:/Desarrollo/AppBuilder/src/frontend/shared/src/entidades/builder/catalog/domain/const/DataSourceSWConst.ts`
- `C:/Desarrollo/AppBuilder/src/frontend/tools/graphql/operations/DataSourceService_CALL_WS_REST.graphql`
- `C:/Desarrollo/AppBuilder/src/frontend/tools/graphql/operations/DataSourceService_CALL_WS_SOAP.graphql`
- `C:/Desarrollo/AppBuilder/src/backend/Infraestructura/Apis/Intrasoft.ApiBuilder/Schema/Mutation/Builder/App/DataSourceServiceMutation.cs`
- `C:/Desarrollo/AppBuilder/src/backend/Infraestructura/Datos/AppBuilder.Infraestructura.DataAccess/Services/AppBuilder/RepositorioService.cs`
- `C:/Desarrollo/AppBuilder/src/backend/Aplicacion/AppBuilder.Aplicacion/Servicios/Builder/App/ServicioDataSourceService.cs`
- `C:/Desarrollo/AppBuilder/src/backend/Aplicacion/AppBuilder.Aplicacion/Servicios/Builder/App/ServicioSearch.cs`

Busqueda local realizada con `rg`, excluyendo rutas y patrones sensibles como `appsettings*`, `*.config`, ficheros de connection strings, dumps, binarios, imagenes, paquetes, `bin`, `obj`, `.git`, `.vs` y `node_modules`.

## Evidencia de menu iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Conectividad` aparece como elemento estatico del menu:

- etiqueta: `Conectividad`;
- icono: `pi pi-code`;
- estado: `disabled: true`;
- ruta: no definida;
- permiso requerido: no definido;
- hijos: no definidos.

En `docs/appbuilder/pages/README.md`, `Conectividad` esta incluida en el inventario inicial como pagina pendiente de evidencia metadata local y documento objetivo `conectividad/README.md`.

Conclusiones:

- La entrada existe en el menu objetivo del producto, pero esta aparcada.
- No hay aun contrato de navegacion, ruta Vue, permiso iLiniumTech ni SDD funcional.
- El menu iLiniumTech no demuestra por si solo que exista una pagina AppBuilder equivalente.

## Evidencia AppBuilder encontrada

### Coincidencia exacta de `Conectividad`

La busqueda exacta de `Conectividad` en AppBuilder no localiza una pantalla, menu, componente ni datasource. La unica coincidencia observada en codigo fuente AppBuilder es un comentario tecnico en servicio de mail sobre falta de conectividad de un dominio Microsoft 365.

Esta coincidencia no representa una pagina de menu ni una funcionalidad de producto migrable.

### Infraestructura generica de conexiones de aplicacion

AppBuilder contiene infraestructura para resolver conexiones por aplicacion, version, entorno, tipo de base de datos, usuario y broker. Esa evidencia ya esta documentada en:

- `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`
- `docs/appbuilder/login-auth-multitenant-application-analysis.md`

Para iLiniumTech esta parte ya se esta tratando como infraestructura backend controlada, no como pagina `Conectividad`.

Regla aplicable:

- `IAPM_Connection` y `IAP_ApplicationConnection` pueden informar resolucion de conexion autorizada.
- No deben convertirse en una pantalla generica donde el frontend gestione connection strings o credenciales.
- No deben alimentar runtime dinamico de pantallas, permisos, queries ni workflows.

### Infraestructura generica de datasources REST/SOAP

La evidencia AppBuilder mas cercana al concepto funcional de conectividad esta en el editor generico de fuentes de datos de servicio web.

Elementos observados:

- `DataSourceComp.vue` permite abrir dialogos de origen de datos de servicio web y usa tabs por tipo de servicio.
- `NewDataSourceWS.vue` contiene secciones de conexion, definicion JSON para REST, configuracion SOAP, WSDL, Swagger/OpenAPI, metodos, mensajes, bindings, namespaces y generacion de request SOAP.
- `NewDataSourceSWConfiguration.vue` permite registrar configuraciones clave/valor por entorno para un datasource de servicio.
- `DataSourceWSPreview.vue` permite hacer una vista preliminar llamando al servicio configurado.
- `DataSourceService_CALL_WS_REST.graphql` y `DataSourceService_CALL_WS_SOAP.graphql` exponen mutaciones GraphQL genericas para invocar servicios.
- `DataSourceServiceMutation.cs` expone operaciones GraphQL de servicio de datasource.
- `RepositorioService.cs` implementa llamadas SOAP y REST, lectura WSDL y lectura Swagger/OpenAPI.
- `ServicioSearch.cs` puede usar configuracion de datasource de servicio en flujos dinamicos de busqueda.

Esta evidencia prueba que AppBuilder tiene un motor generico de conectividad, pero no prueba que exista una pagina de menu `Conectividad` para usuarios finales de iLiniumTech.

### Riesgo especial de los valores de configuracion

La infraestructura observada maneja conceptos sensibles:

- URL o host de servicio;
- endpoint/metodo;
- token bearer;
- parametros;
- body JSON;
- request SOAP;
- configuracion por entorno;
- posibles requests/responses de preview;
- posible relacion con logs de ejecucion.

No se han copiado valores reales ni se han abierto archivos de configuracion sensible. Si una futura SDD decide migrar parte de esta capacidad, todos esos valores deben vivir fuera de Git, en secret store o configuracion segura por entorno.

## Evidencia AppBuilder ausente

No se ha encontrado evidencia local verificable de:

- `menuId` de `Conectividad`;
- `componentId` raiz;
- `urlComponentStatic` o ruta heredada;
- arbol de componentes;
- tabs o submenus propios de la pagina;
- datasource principal de pantalla;
- columnas o grid de listado;
- filtros historicos;
- acciones historicas de pagina;
- permisos `objectGroups` asociados a una pantalla concreta;
- workflows especificos de una pagina `Conectividad`;
- propietario funcional o UAT.

La ausencia es importante: desarrollar ahora seria inventar producto o exponer una consola tecnica de alto riesgo sin validacion.

## Raiz, hijos, tabs y submenus

Raiz:

- No detectada.

Hijos:

- No detectados.

Tabs:

- No hay tabs de pagina `Conectividad` detectadas.
- Si se mira el motor generico de datasources, aparecen tabs por tipo de servicio REST/SOAP dentro del editor AppBuilder, pero no son tabs de una pagina funcional iLiniumTech.

Submenus:

- No detectados.

Componentes internos:

- No se han creado documentos bajo `components/*.md`, porque no hay pestanas, submenus ni componentes internos reales verificables de `Conectividad`.

Subagentes:

- No se han creado subagentes de componente. Crear subagentes para REST, SOAP, Swagger u otros apartados ahora mezclaria motor AppBuilder con producto iLiniumTech sin evidencia de pagina.

## Datos y entidades heredadas relacionadas

La evidencia local apunta a entidades genericas AppBuilder, no a una pantalla `Conectividad`:

- `IapDataSourceService`
- `IapDataSourceServiceConfiguration`
- `IapDataSourceServiceClass`
- `IapDataSourceServiceMethodDefinition`
- `IapDataSourceServiceMethodResponseConfiguration`
- `IapDataSource`
- `IapDataSourceDataBase`
- `IapDataSourceField`
- `IapComponentDataSource`
- `IapComponentDataSourceServiceConfiguration`

Tambien existe evidencia transversal de conexiones de aplicacion y maestro:

- conexiones por aplicacion/version/entorno;
- conexiones por broker/identidad;
- tipos de base de datos Builder, Master, Model, Log, Documentos u otros.

Estas entidades no deben migrarse como CRUD generico. Si alguna se necesita en iLiniumTech, debe transformarse en contrato explicito, limitado y revisado.

## Filtros heredados

No se han detectado filtros de una pagina `Conectividad`.

Filtros que podria requerir una SDD futura, solo como propuesta y no como contrato:

- entorno;
- tipo de integracion;
- estado activo/inactivo;
- broker o aplicacion;
- proveedor;
- ultimo resultado de prueba;
- nivel de riesgo;
- texto libre sobre nombre interno sanitizado.

Ninguno de estos filtros debe implementarse sin definir antes el caso de uso y el origen autorizado de datos.

## Acciones heredadas observadas

No se han detectado acciones propias de una pagina `Conectividad`.

Acciones genericas AppBuilder relacionadas con conectividad:

- crear/modificar datasource de servicio;
- configurar pares clave/valor por entorno;
- parsear WSDL;
- parsear Swagger/OpenAPI;
- generar estructuras/tablas/campos a partir de servicios;
- ejecutar preview REST;
- ejecutar preview SOAP;
- invocar endpoint REST con token bearer;
- invocar servicio SOAP con `SOAPAction`;
- usar datasources de servicio desde busqueda o workflows.

Estas acciones no deben migrarse directamente. Algunas pueden hacer llamadas externas, exponer tokens, registrar payloads, modificar metadata o disparar integraciones.

## Permisos historicos

No se ha encontrado permiso historico concreto para `Conectividad`.

La infraestructura AppBuilder usa permisos/`objectGroups` de forma generica sobre menus, componentes, datasources y eventos, pero no se ha localizado una asociacion verificable con una pantalla `Conectividad`.

Permisos iLiniumTech candidatos solo si se aprueba una SDD futura:

- `conectividad.read`: ver estado sanitario y listado no sensible.
- `conectividad.test`: ejecutar pruebas controladas contra endpoints allowlist.
- `conectividad.manage`: administrar definiciones no secretas de integracion.
- `conectividad.secrets.manage`: permiso excepcional, preferiblemente fuera de frontend y delegado a plataforma/secret store.
- `conectividad.audit.read`: consultar auditoria sanitizada.

Recomendacion: evitar `conectividad.manage` hasta que exista modelo de secretos, auditoria, RBAC y segregacion por entorno.

## Propuesta Vue/API estatica futura

Esta propuesta no autoriza desarrollo. Solo define una forma segura si producto decide reactivar la pagina.

### Frontend

Ruta candidata:

- `/conectividad`

Pagina Vue candidata:

- `iLiniumTech.Frontend/src/features/conectividad/ConectividadView.vue`

Componentes candidatos, solo si hay SDD:

- `ConectividadStatusPanel.vue`: resumen de estado sin secretos.
- `ConectividadIntegrationsTable.vue`: listado de integraciones sanitizadas.
- `ConectividadFilters.vue`: filtros por entorno/tipo/estado.
- `ConectividadAuditTable.vue`: eventos sanitizados.
- `ConectividadTestDialog.vue`: prueba controlada, sin editar secretos.

Estados obligatorios:

- loading;
- empty;
- sin permiso;
- sin broker si aplica;
- error backend sanitizado con `correlationId`;
- integracion bloqueada por configuracion;
- prueba no permitida en entorno actual.

Reglas UI:

- No mostrar connection strings.
- No mostrar tokens.
- No mostrar passwords.
- No mostrar request/response completos si contienen PII o secretos.
- No permitir URL libre en un formulario.
- No permitir ejecutar REST/SOAP generico desde metadata.
- No mostrar nombres tecnicos heredados como contrato de producto si no han sido revisados.

### Backend

Endpoints candidatos, solo si hay SDD:

- `GET /api/conectividad/integrations`
- `GET /api/conectividad/integrations/{id}`
- `POST /api/conectividad/integrations/{id}/test`
- `GET /api/conectividad/audit`

Contratos esperados:

- DTOs sanitizados;
- secretos siempre redaccionados;
- endpoints allowlist;
- metodos HTTP allowlist;
- timeouts cortos;
- cancelacion por request;
- rate limit o proteccion contra abuso;
- permisos explicitos;
- auditoria de pruebas;
- `correlationId` en errores.

Datos:

- Configuracion no secreta podria venir de BBDD/configuracion revisada.
- Secretos deben vivir en secret store o variables de entorno por ambiente, nunca en Git ni en payloads de frontend.
- Las pruebas reales de conectividad deben ejecutarse desde backend, nunca directamente desde el navegador con secretos.

## Que no debe replicarse

No migrar:

- editor generico AppBuilder de datasources;
- CRUD de `IapDataSource*`;
- edicion de `IAP_ApplicationConnection` desde frontend;
- visualizacion o edicion de connection strings;
- edicion de tokens en Vue;
- ejecucion generica de REST/SOAP definida por metadata;
- preview libre de servicios externos;
- importacion generica de WSDL/OpenAPI como runtime productivo;
- workflows AppBuilder asociados a datasources;
- llamadas externas sin SDD, permisos, auditoria, allowlist y UAT.

Regla critica: `Conectividad` no debe convertirse en un AppBuilder miniatura dentro de iLiniumTech.

## Riesgos

### Seguridad

- Fuga de secrets por mostrar o versionar configuracion.
- SSRF si se permite URL libre para pruebas.
- Uso indebido de tokens bearer historicos.
- Ejecucion de llamadas externas sin auditoria.
- Logs con request/response sensibles.
- Errores que revelen host, rutas internas, certificados o infraestructura.

### Privacidad y datos

- Responses REST/SOAP pueden contener PII.
- Preview de datos puede imprimir payloads completos.
- Auditoria puede almacenar payloads sensibles si no se minimiza.
- Capturas de pantalla de conectividad pueden contener endpoints o datos operativos.

### Multi-tenant

- Integraciones pueden variar por broker, aplicacion, entorno o usuario.
- El broker debe validarse antes de resolver cualquier configuracion especifica.
- Una prueba de conectividad no debe revelar existencia de integraciones de otro broker.

### Operacion

- Pruebas de conectividad pueden impactar sistemas externos.
- Timeouts largos pueden bloquear threads o degradar API.
- Retrys no controlados pueden duplicar efectos si el endpoint externo no es idempotente.
- Entornos Development, UAT, Preview y Production deben tener reglas distintas.

### Producto

- No esta claro si `Conectividad` es funcionalidad para usuario de negocio, soporte, administracion tecnica o plataforma.
- Sin definicion, el riesgo de construir una pantalla que nadie puede validar es alto.

## Pruebas necesarias si se desarrolla

Backend:

- anonimo devuelve 401;
- usuario sin permiso devuelve 403 con `correlationId`;
- broker ausente/no permitido no resuelve configuracion;
- secrets redaccionados en DTOs y errores;
- URL libre rechazada;
- endpoint no allowlist rechazado;
- timeout/cancelacion cubiertos;
- no se registran tokens ni payloads sensibles;
- prueba de conectividad audita resultado sanitizado;
- payloads maliciosos no alteran headers, URL ni body estructural.

Frontend:

- menu muestra `Conectividad` solo si esta habilitada por feature flag/permiso;
- pantalla bloqueada muestra estado seguro si falta permiso;
- no aparecen tokens, connection strings, passwords ni endpoints internos en DOM;
- filtros actualizan URL si procede;
- dialogo de prueba no acepta URL libre;
- errores muestran mensaje sanitizado y `correlationId`;
- responsive sin solapes.

Seguridad/calidad:

- `Invoke-SecretScan.ps1` limpio;
- dependency audit sin bloqueantes;
- CORS audit si se toca API/config;
- `git diff --check`;
- smoke local con datos ficticios;
- UAT con payloads sanitizados, no capturas sensibles.

## Bloqueos externos

- Falta confirmar si existe una pantalla real `Conectividad` en AppBuilder y su `componentId`.
- Falta metadata sanitizada de menu/arbol/componentes si realmente existe.
- Falta propietario funcional.
- Falta SDD.
- Falta decision de seguridad sobre gestion de secretos.
- Falta decision de plataforma sobre allowlists, secret store, timeouts y auditoria.
- Falta UAT que defina que debe poder ver/hacer un usuario.
- Falta clasificacion de entornos y broker para integraciones.

## Decisiones para futuras IA

- No crear subcomponentes ni codigo de `Conectividad` mientras el estado siga `bloqueado externo`.
- No reutilizar `DataSourceComp.vue`, `NewDataSourceWS.vue` ni `RepositorioService.cs` como base directa de producto.
- Si producto confirma alcance, empezar por una SDD minima con caso de uso, permisos, datos, riesgos, auditoria, pruebas y UAT.
- Preferir una pantalla operacional limitada y segura antes que un editor generico de integraciones.
- Mantener toda metadata AppBuilder como evidencia offline, nunca como runtime de iLiniumTech.

## Validacion de esta ronda

Comandos ejecutados:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools/quality/Test-DocumentationBaseline.ps1` -> OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1` -> OK, sin leaks.
- `git diff --check` -> OK.
- Revision ASCII del documento -> OK.

Pruebas no ejecutadas:

- Backend build/test: no aplica, no se ha tocado backend.
- Frontend format/lint/unit/build: no aplica, no se ha tocado frontend.
- Smoke local: no aplica, no se ha cambiado runtime visible.

## Readiness tecnico/admin actualizado 2026-05-18

Estado actual en iLiniumTech:

- Existe ruta protegida `/conectividad` como pagina Vue estatica.
- Existe fixture local read-only con entradas candidatas, filtros locales y paginacion.
- No existe API backend de conectividad.
- No existen permisos iLiniumTech aprobados para conectividad.
- No se ejecutan llamadas externas, pruebas REST/SOAP, previews, importaciones WSDL/OpenAPI ni validaciones reales.
- La evidencia frontend verifica que no se renderizan marcadores como `IAP_`, `QueryStatic`, `ComponentDataSource`, `connectionString`, `SELECT *`, `appsettings`, endpoints reales, tokens, API keys, emails ni documentos.

Acciones bloqueadas:

- probar integracion;
- exportar;
- abrir detalle operativo;
- usar URL libre;
- mostrar endpoints, credenciales, headers, request/response o payloads;
- invocar conectores REST/SOAP heredados.

Permisos candidatos no aprobados:

- `conectividad.read`;
- `conectividad.test`;
- `conectividad.manage`;
- `conectividad.secrets.manage`;
- `conectividad.audit.read`.

Threat model creado como requisito antes de cualquier dato/API:

- [`docs/engineering/conectividad-threat-model.md`](../../../engineering/conectividad-threat-model.md) define SSRF y allowlist de destinos;
- gestion de secretos fuera del frontend y fuera de Git;
- timeouts, cancelacion, rate limit y no retry peligroso;
- auditoria de pruebas;
- redaccion de request/response y errores;
- separacion por entorno y broker;
- bloqueo de URL libre y metodos no autorizados;
- politica de logging sin payloads sensibles.

Tareas futuras pequenas recomendadas:

1. Confirmar si `Conectividad` es salud de integraciones, administracion tecnica o modulo nuevo.
2. Revisar y aprobar el threat model de conectividad antes de cualquier API.
3. Definir modelo allowlist y secret store.
4. Crear SDD read-only de inventario sanitizado si producto lo aprueba.
5. Mantener pruebas que garantizan acciones deshabilitadas y ausencia de secretos.

Criterios de aceptacion para avanzar:

- SDD aprobada;
- threat model aprobado;
- permisos backend especificos;
- endpoints allowlist y sin URL libre;
- secretos nunca visibles en DOM, respuestas, logs ni capturas;
- auditoria de pruebas y errores con `correlationId`;
- pruebas de SSRF, redaccion y no metadata runtime.
