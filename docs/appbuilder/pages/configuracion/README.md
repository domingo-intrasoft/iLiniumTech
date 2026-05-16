# Pagina AppBuilder: Configuracion

Fecha: 2026-05-16

Rol IA: jefe de pagina `Configuracion`.

Ronda: solo documentacion. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se convierte metadata AppBuilder en contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

`Configuracion` existe como entrada de primer nivel en el menu objetivo de iLiniumTech, pero no hay evidencia local suficiente para afirmar que exista una pagina AppBuilder concreta de menu `Configuracion` con `menuId`, `componentId`, componente raiz, hijos, tabs, submenus, datasource, filtros, permisos historicos o acciones propias.

La evidencia local encontrada apunta a tres conceptos distintos:

- configuracion tecnica de una aplicacion AppBuilder dentro de la pestana `Configuracion` del detalle de aplicacion;
- panel lateral generico de configuracion visual del layout AppBuilder;
- datos de configuracion de modelo expuestos por `Configuration_GET_ALL` y entidades `Configuracion`/`IapmoConfiguration`.

Ninguno de esos conceptos confirma una pantalla de negocio migrable como pagina autonoma `Configuracion` en iLiniumTech. Por tanto, no se debe desarrollar esta pagina hasta contar con SDD funcional, metadata sanitizada o validacion de producto/DBA/UAT.

No se crean documentos `components/*.md` en esta ronda porque no se han detectado pestanas, submenus o componentes internos reales especificos de una pagina `Configuracion`. Los componentes observados son internos/genericos de AppBuilder y no deben convertirse en producto por deduccion.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`.
- `PLANS.md`.
- `README.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/appbuilder/pages/README.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- `docs/appbuilder/pages/informes/README.md`, como referencia de estructura documental.
- Busquedas locales en `docs/**`, `iLiniumTech.Frontend/**` e `iLiniumTech.Backend/**`.

Repositorio AppBuilder, solo lectura y sin abrir configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\router.js`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\main.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\AppWrapper.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\container.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppLayout.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppTopbar.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\componentes\base\common\layout\AppConfig.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\componentes\base\common\composables\layout.js`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\ApplicationDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\ApplicationDetailConfig.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\domain\iapApplicationConfiguration.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ApplicationConfiguration_GET_BY_APP_ID.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ApplicationConfiguration_GET_ALL.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ApplicationConfiguration_GET_BY_APP_AND_TYPES.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\AddApplicationConfiguration.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\UpdateApplicationConfiguration.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\DeleteApplicationConfiguration.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Configuration_GET_ALL.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builderModel\configuration\domain\iapmoConfiguration.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builderModel\configuration\infrastructure\ConfigurationApolloClientRepository.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\catalog\domain\const\CatalogConfigurationConst.ts`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\CatalogConfigurationConst.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapApplicationConfiguration.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppModel\IapmoConfiguration.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Query\Builder\App\ApplicationConfigurationQuery.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Query\Builder\Model\ConfigurationQuery.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Schema\Query\Builder\Model\ConfigurationQuery.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\Modelo\Repositorios\RepositorioConfiguracion.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.

Restricciones aplicadas:

- No se abrieron `appsettings`, archivos `.config`, connection strings, dumps, capturas, binarios ni datos personales reales.
- No se ejecuto SQL.
- No se ejecuto extractor `Live`.
- No se copiaron valores reales de configuracion, passwords, connection strings, claves OAuth, correos, URLs internas ni datos personales.

## Evidencia de menu y metadata

### iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Configuracion` aparece como item de primer nivel:

- etiqueta: `Configuracion`;
- icono: `pi pi-cog`;
- estado: `disabled: true`;
- ruta: no definida;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma un hueco de navegacion objetivo, pero no una pantalla implementada ni una especificacion funcional.

En `docs/appbuilder/pages/README.md`, `Configuracion` figura en el inventario de paginas con estado inicial `Pendiente de evidencia metadata local`.

### AppBuilder router/menu

El router base de AppBuilder contiene rutas bootstrap como `/`, `/login`, `/mipanel`, `/changingprofile` y not-found. Las rutas reales se reconstruyen dinamicamente a partir de menus/metadata tras login.

No se encontro en codigo local una ruta estatica `configuracion`, `configuration` o equivalente que confirme una pagina concreta.

No se encontro una entrada local de menu con etiqueta historica `Configuracion`, `Settings` o `Ajustes` asociada a `componentId` o `menuId`.

### Configuracion de aplicacion AppBuilder

`ApplicationDetail.vue` contiene una pestana `Configuracion` dentro del detalle de una aplicacion AppBuilder. Esa pantalla tiene otras pestanas hermanas: `Datos Generales`, `Notas`, `Conexiones` y `Aplicaciones Relacionadas`.

La pestana `Configuracion` renderiza `ApplicationDetailConfig.vue`. Este componente:

- muestra un `DataTable` agrupado por entorno;
- trabaja sobre `appConfig`;
- edita claves `idKeyType`, `idEnvironmentType`, `keyValue` y `keyBinaryValue`;
- usa controles especificos para colores, idiomas, logos, favicon, plantilla, tema, estilo, flags, proveedor de login y CSS personalizado;
- permite anadir configuracion por entorno;
- permite eliminar configuraciones con `idEnvironmentType` no nulo;
- llama a servicios de `ApplicationConfiguration` para `getByApplicationId`, `add`, `update` y `delete`.

Operaciones GraphQL relacionadas:

- `ApplicationConfiguration_GET_BY_APP_ID`;
- `ApplicationConfiguration_GET_ALL`;
- `ApplicationConfiguration_GET_BY_APP_AND_TYPES`;
- `AddApplicationConfiguration`;
- `UpdateApplicationConfiguration`;
- `DeleteApplicationConfiguration`.

Campos detectados en el contrato frontend de `IapApplicationConfiguration`:

- `id`;
- `applicationId`;
- `applicationVersion`;
- `idKeyType`;
- `idEnvironmentType`;
- `keyValue`;
- `keyBinaryValue`;
- `order`;
- `fcr`;
- `ucr`;
- `uum`;
- `fum`.

Claves de configuracion observadas en constantes AppBuilder:

- `appconfig-color1`;
- `appconfig-color2`;
- `appconfig-language`;
- `appconfig-logopng`;
- `appconfig-logosvg`;
- `appconfig-template`;
- `appconfig-favicon`;
- `appconfig-template-theme`;
- `appconfig-template-theme-style`;
- `appconfig-template-config`;
- `appconfig-template-config-simple`;
- `appconfig-template-urlreportlauncher`;
- `appconfig-isapp-external`;
- `appconfig-license`;
- `appconfig-signinwith`;
- `appconfig-signinwith-google`;
- `appconfig-signinwith-microsoft`;
- `appconfig-footer`;
- `appconfig-render`;
- `appconfig-componentlayout`;
- `appconfig-title`;
- `appconfig-version`;
- `appconfig-customcss`;
- `appconfig-initialurlpage`;
- `appconfig-ms-appid`;
- `appconfig-ms-tenantid`;
- `appconfig-ms-urlredirect`;
- `appconfig-gg-appid`;
- `appconfig-defaultemail`;
- `appconfig-defaultemail-imapport`;
- `appconfig-defaultemail-imapserver`;
- `appconfig-defaultemail-smtpserver`;
- `appconfig-defaultemail-smtpport`;
- `appconfig-defaultemail-smtppass`.

Importante: esta evidencia pertenece al Builder/administracion de aplicaciones. No confirma que el producto de negocio tenga una pagina final `Configuracion` para usuarios de iLiniumTech.

### Panel visual de layout

`AppLayout.vue` importa y monta `AppConfig.vue` como panel generico de layout. `AppTopbar.vue` contiene un boton de engranaje condicionado por `appconfig-template-config`, aunque en la plantilla revisada ese bloque esta comentado.

`AppConfig.vue` permite modificar preferencias visuales del layout:

- escala;
- tema de componente;
- superficie;
- preset;
- `colorScheme`;
- `inputStyle`;
- `ripple`;
- `menuMode`;
- `menuTheme`.

`layout.js` persiste cambios mediante `setLayoutConfig`. Esto es configuracion de experiencia/layout de AppBuilder, no una pagina de negocio.

### Configuracion de modelo

`Configuration_GET_ALL.graphql` consulta:

- `id`;
- `idKey`;
- `value`;
- `fcr`;
- `fum`;
- `ucr`;
- `uum`.

El contrato frontend `IapmoConfiguration` tiene esos mismos campos basicos.

En backend, `ConfigurationQuery` expone `GetAll` con la descripcion `Obtiene la configuracion de la entidad principal`. `RepositorioConfiguracion` lee la tabla `Configuracion` y EF mapea `Configuracion`, `RepositorioConfiguracion`, `RepositorioConfiguracionGlobal` y `vw_Configuracion`.

Esta evidencia confirma un dominio de configuracion de modelo, pero no una pantalla de menu `Configuracion`.

## Componente raiz e hijos

### Componente raiz

No identificado.

No hay evidencia local suficiente de:

- `menuId` historico de `Configuracion`;
- `componentId` raiz;
- nombre de componente AppBuilder raiz;
- ruta heredada;
- datasource principal de pagina;
- relacion raiz-hijo;
- `QueryStatic` o metadata sanitizada asociada a una pagina de menu `Configuracion`;
- permisos historicos concretos de una pagina final.

### Componentes internos observados pero no asignables a pagina

No se crean subdocumentos porque estos componentes no prueban una pagina de menu `Configuracion`:

- `ApplicationDetailConfig.vue`: pestana interna del detalle de aplicacion AppBuilder.
- `AppConfig.vue`: panel global de layout.
- `Configuration_GET_ALL`/`IapmoConfiguration`: configuracion de modelo, sin layout de pagina.
- `ApplicationConfiguration_*`: API GraphQL de configuracion del Builder, no contrato iLiniumTech.
- `DataSourceServiceConfiguration*`, `ComponentDataSourceFieldConfiguration*`, `ObjectDataSourceFieldConfiguration*`, `SearchConfig*` e `ImportConfig*`: configuraciones genericas de AppBuilder que no deben convertirse en runtime dinamico.

## Pestanas, submenus y areas internas

Estado: no confirmado para una pagina `Configuracion`.

Pestanas confirmadas en AppBuilder, pero pertenecientes a `ApplicationDetail`, no a una pagina `Configuracion`:

- `Datos Generales`;
- `Configuracion`;
- `Notas`;
- `Conexiones`;
- `Aplicaciones Relacionadas`;
- dentro de aplicaciones relacionadas, expansion con `Conexiones` y `Menus`.

Areas candidatas si producto decide crear una pagina propia de configuracion iLiniumTech:

- preferencias visuales del usuario;
- configuracion funcional de aplicacion;
- proveedores de login;
- idiomas;
- branding;
- pagina inicial;
- integracion de informes;
- configuracion de email;
- seguridad/licencia;
- configuracion por entorno;
- configuracion de broker/tenant.

Estas areas son candidatas derivadas de nombres y claves AppBuilder. No son alcance aprobado.

## Datos detectados

Datos tecnicos observados:

- configuraciones por aplicacion/version;
- configuraciones por entorno;
- claves y valores de aplicacion;
- valores binarios para logo, favicon u otros assets;
- CSS personalizado;
- preferencias de tema/layout;
- configuracion de proveedores de login;
- configuracion de correo;
- configuracion de launcher de informes;
- configuracion de modelo/entidad principal;
- auditoria tecnica `fcr`, `ucr`, `fum`, `uum`.

Datos sensibles o peligrosos:

- passwords SMTP o equivalentes;
- ids de aplicacion Microsoft/Google;
- tenant ids y URLs de redireccion;
- URLs internas;
- CSS editable que podria inyectar contenido visual no revisado;
- valores binarios;
- claves de licencia o certificados;
- configuraciones por entorno;
- cualquier valor que se use despues para conectar servicios externos.

Regla recomendada: ninguna configuracion sensible debe llegar al frontend sin minimizacion. Si iLiniumTech necesita configuracion propia, debe vivir en backend, variables de entorno o secret store, con DTOs sanitizados y permisos explicitos.

## Filtros

No se encontraron filtros propios de una pagina `Configuracion`.

Filtros/agrupaciones observados en componentes internos:

- agrupacion de `ApplicationDetailConfig` por entorno (`groupKey`);
- orden por `idEnvironmentType` y `order`;
- seleccion de tipo de clave (`idKeyType`);
- seleccion de entorno (`idEnvironmentType`);
- catalogos de tipos de configuracion y entornos.

No deben migrarse automaticamente. Si producto aprueba una pagina, los filtros deben definirse como contrato iLiniumTech.

## Acciones historicas

Acciones confirmadas en piezas internas de AppBuilder:

- listar configuracion por aplicacion/version;
- anadir configuracion por entorno;
- editar valores en celdas;
- eliminar configuraciones de entorno;
- subir logo/favicon;
- editar CSS personalizado;
- cambiar plantilla/tema/render/layout;
- persistir preferencias de layout;
- consultar configuracion de modelo.

Acciones no confirmadas para una pagina de menu `Configuracion`:

- administrar configuracion general de iLiniumTech;
- administrar parametros de negocio;
- administrar tenants o brokers;
- administrar permisos;
- administrar integraciones;
- administrar mail/SMS;
- administrar autenticacion productiva;
- administrar licencias;
- auditar cambios de configuracion.

Estas acciones requieren SDD, permisos, auditoria y revision de seguridad antes de entrar en producto.

## Permisos

No se localizaron permisos historicos especificos de una pagina `Configuracion`.

Permisos iLiniumTech candidatos, no aprobados:

- `configuracion.read`: ver configuraciones no sensibles aprobadas.
- `configuracion.edit`: editar configuraciones funcionales no secretas.
- `configuracion.branding.read`: ver configuracion visual/branding.
- `configuracion.branding.edit`: editar branding o layout autorizado.
- `configuracion.security.read`: ver configuracion de seguridad sanitizada.
- `configuracion.security.edit`: modificar parametros de seguridad con doble control.
- `configuracion.integrations.read`: ver integraciones sin secretos.
- `configuracion.integrations.edit`: administrar integraciones con revision.
- `configuracion.audit.read`: consultar historial de cambios.

Regla recomendada: el backend debe ser la autoridad. La UI puede ocultar o deshabilitar acciones segun `/api/me`, pero la API debe aplicar permisos efectivos, broker/tenant y auditoria.

## Propuesta Vue/API estatica futura

No desarrollar en esta ronda.

Si producto aprueba SDD, hay dos enfoques posibles. Debe elegirse uno, no mezclar ambos sin decision.

### Opcion A: Configuracion de usuario

Adecuada si la pagina sirve para preferencias personales no sensibles.

Frontend Vue:

- ruta `/configuracion`;
- pagina `ConfiguracionView.vue`;
- secciones estaticas: apariencia, idioma, pagina inicial y preferencias de navegacion;
- sin secretos ni datos de infraestructura;
- estados `loading`, `empty`, `error`, `sin permiso`;
- formularios con validacion local y backend.

Backend API:

- `GET /api/configuracion/usuario`;
- `PUT /api/configuracion/usuario`;
- DTO limitado a preferencias permitidas;
- auditoria ligera de cambios;
- sin uso de metadata AppBuilder runtime.

### Opcion B: Configuracion administrativa

Adecuada si la pagina es para administradores tecnicos o funcionales.

Frontend Vue:

- ruta `/configuracion`;
- tabs estaticas aprobadas por SDD, por ejemplo `General`, `Branding`, `Integraciones`, `Seguridad`, `Auditoria`;
- cada tab con componentes Vue propios, no render dinamico;
- valores sensibles siempre ocultos o sustituidos por indicadores;
- cambios criticos con confirmacion, motivo y evidencia.

Backend API:

- `GET /api/configuracion/general`;
- `PATCH /api/configuracion/general`;
- `GET /api/configuracion/branding`;
- `PATCH /api/configuracion/branding`;
- `GET /api/configuracion/integraciones`;
- endpoints de rotacion/validacion de secretos sin devolver secretos;
- `GET /api/configuracion/auditoria`;
- validacion por permisos, entorno y tenant/broker cuando aplique;
- auditoria de usuario, broker/tenant, campo cambiado, valor anterior redaccionado, valor nuevo redaccionado y `correlationId`.

No debe hacerse:

- endpoint generico que reciba clave AppBuilder y devuelva `keyValue`/`keyBinaryValue`;
- frontend que liste todas las claves `appconfig-*`;
- UI que permita editar secretos desde el navegador sin secret store;
- migrar `ApplicationDetailConfig.vue` como pagina final;
- reutilizar `AppConfig.vue` como panel productivo sin rediseno;
- leer `IAP_ApplicationConfiguration`, `IAPMO_Configuration`, `Configuracion` o `RepositorioConfiguracion` en runtime para decidir pantallas o permisos.

## Riesgos

Seguridad y datos:

- exposicion de secretos por mostrar `keyValue` o `keyBinaryValue` heredado;
- fuga de passwords SMTP, tenant ids, app ids, URLs internas o licencias;
- modificacion de configuracion de seguridad sin control;
- open redirect o exfiltracion por URLs configurables;
- CSS personalizado como vector de manipulacion visual o inyeccion;
- configuracion por entorno visible a usuarios sin rol;
- logs con valores sensibles;
- cambios sin auditoria ni rollback.

Arquitectura:

- recrear AppBuilder como runtime de configuraciones genericas;
- hacer que el frontend consuma `appconfig-*` para construir UI;
- usar claves heredadas como contrato productivo;
- mezclar configuracion de producto con secretos de infraestructura;
- convertir preferencias visuales en deuda global no testeada.

Producto/UAT:

- `Configuracion` puede significar preferencias de usuario, administracion tecnica, branding, integraciones, permisos o parametros de negocio;
- no hay propietario funcional definido;
- no hay matriz de permisos;
- no hay decision sobre que debe ser editable en UI y que debe quedar en entorno/secret store.

## Pruebas necesarias para desarrollo futuro

Antes de implementar:

- SDD que defina si `Configuracion` es pagina de usuario o administrativa.
- Inventario de valores permitidos, prohibidos y secretos.
- Decision de seguridad sobre secret store, rotacion y redaccion.
- Matriz de permisos y roles.
- UAT owner para validar campos visibles/editables.

Backend:

- anonimo devuelve 401;
- usuario sin permiso devuelve 403 con `correlationId`;
- valores sensibles nunca se devuelven completos;
- campos no whitelistados devuelven 400;
- cambios criticos requieren permiso especifico;
- auditoria registra cambios con valores redaccionados;
- configuracion por tenant/broker no cruza datos;
- no se aceptan nombres de claves AppBuilder como entrada libre;
- logs no contienen secretos ni datos personales.

Frontend:

- ruta protegida por sesion;
- menu visible solo si producto lo activa;
- estados `loading`, `empty`, `error`, `sin permiso`;
- formularios con validacion y mensajes seguros;
- valores secretos aparecen como `configurado/no configurado`, nunca como texto;
- acciones criticas piden confirmacion;
- no hay dependencia de metadata AppBuilder.

QA/CI:

- `dotnet build`.
- `dotnet test`.
- `npm run format`.
- `npm run lint`.
- `npm run test:unit`.
- `npm run build`.
- `Invoke-SecretScan.ps1`.
- `Invoke-DependencyAudit.ps1 -FailOnFindings` si toca dependencias.
- `Invoke-CorsAudit.ps1 -FailOnFindings` si toca API/configuracion.
- smoke visual solo cuando exista pagina activa.

## Bloqueos

Bloqueos externos:

- no hay metadata local sanitizada de una pagina de menu `Configuracion`;
- no hay `menuId` ni `componentId` raiz;
- no hay tabs/submenus verificados de una pagina autonoma;
- no hay datasource, filtros, acciones ni permisos historicos propios;
- no hay SDD que defina significado funcional de `Configuracion`;
- no hay decision de seguridad sobre valores editables desde UI;
- no hay matriz de permisos ni owner UAT;
- no hay validacion DBA de tablas/vistas de configuracion que puedan consultarse de forma segura.

Pendiente tecnico:

- decidir si `Configuracion` sera preferencias de usuario, administracion funcional, administracion tecnica, branding o integraciones;
- crear SDD si se prioriza;
- definir contrato API explicito y DTOs redaccionados;
- definir almacenamiento propio iLiniumTech o integracion con secret store;
- definir auditoria y rollback de cambios;
- decidir si el menu debe seguir deshabilitado, ocultarse o activarse por permiso.

Completado con evidencia:

- navegacion iLiniumTech revisada;
- documentos de gobierno revisados;
- busquedas locales en iLiniumTech y AppBuilder ejecutadas sin abrir configuracion sensible;
- evidencias de `ApplicationDetailConfig`, `AppConfig` y `Configuration_GET_ALL` documentadas;
- ausencia de pagina concreta `Configuracion` documentada;
- subagentes/component docs no creados por falta de tabs/submenus reales especificos de una pagina `Configuracion`.
