# Pagina AppBuilder: Administracion

Fecha de analisis: 2026-05-16.

Rol de esta ronda: jefe de pagina para `Administracion`.

Ronda: solo documentacion. No se programa frontend, backend, extractor ni SQL.

Estado final: `bloqueado externo`.

Motivo del estado: `Administracion` existe como entrada futura en el menu objetivo de iLiniumTech, pero no se ha encontrado metadata local suficiente para identificar una pagina AppBuilder real llamada `Administracion` con `menuId`, `componentId`, raiz, hijos, tabs, submenus, datasources, filtros, acciones o permisos historicos concretos.

La evidencia local si muestra un area administrativa generica del propio AppBuilder, especialmente `Sistema > Builder` y la ruta `/aplicaciones/gestion`, con pestanas internas reales para gestionar aplicaciones, menus, origenes de datos, layout, componentes, procesos y ayuda. Esa evidencia es relevante para entender el origen tecnico, pero no prueba que el menu iLiniumTech `Administracion` deba ser esa pantalla ni que debamos migrar el Builder. Migrar esa superficie como producto final supondria alto riesgo de reintroducir AppBuilder como runtime dinamico.

## Regla base

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. La metadata heredada sirve para analisis, trazabilidad, SDD y scaffolding revisado. Si en el futuro se aprueba una pagina `Administracion`, debera quedar como Vue/TypeScript estatico y backend .NET con API explicita por caso de uso.

No se debe hacer que el frontend/backend productivo consulte `IAP_Menu`, `IAP_Component`, `IAP_DataSource`, `IAP_Application`, workflows, expresiones, directivas heredadas o configuraciones de conexiones para pintar, autorizar o ejecutar la pantalla en runtime.

## Fuentes revisadas

### Gobierno iLiniumTech

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`

### Codigo iLiniumTech

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- busqueda segura en `docs`, `iLiniumTech.Frontend/src` e `iLiniumTech.Backend/src` para `Administracion`, variantes acentuadas y `Admin`.

### Fuentes AppBuilder revisadas

Sin abrir configuraciones sensibles ni ejecutar SQL:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\modelos\menu\custommenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\modelos\menu\custommenuprime.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\almacen\modules\AuthModule.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\funciones\HelperSecurity.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Application_GET_ALL.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\ApplicationConnection_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\User_GET_ALL_USERS_WITH_PROFILES.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations` para inventario de operaciones `Application`, `Menu`, `Component`, `DataSource`, `Configuration`, `Workflow`, `User` y `UserOption`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\Applications.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\ApplicationDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\ApplicationDetailConfig.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\application\infrastructure\component\ApplicationConnection.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\MenuComp.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\MenuRelated.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\MenuTree.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\MenuTreeValue.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\NewMenu.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\component\SecurityValue.vue`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Query\Builder\App\MenuQuery.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\App\bllMenu.cs`

### Fuentes evitadas expresamente

- `appsettings*`
- `*.config`
- `.env*`
- perfiles de publicacion
- connection strings
- dumps, backups, ficheros comprimidos y capturas
- datos personales reales
- extractor `Live`
- consultas SQL reales

## Busquedas ejecutadas

Se usaron busquedas locales con `rg`, con globs de exclusion para evitar configuraciones sensibles, binarios, caches y artefactos no utiles. Terminos principales:

- `Administracion`
- `Administracion` con variante acentuada
- `Administracion` con variantes normalizadas
- `Administration`
- `Admin`
- `administrador`
- `administrator`
- `Sistema`
- `Builder`
- `aplicaciones/gestion`
- `applicationsadmin`
- `IapMenu`
- `Menu_GET_BY_APPLICATION_ID`
- `Application_GET_ALL`
- `ApplicationConnection`
- `User_GET_ALL_USERS_WITH_PROFILES`

Resultado relevante:

- No se encontro `Administracion` como pagina AppBuilder concreta en las fuentes revisadas.
- No se encontro una fila local sanitizada de menu cuyo `title` o localizacion confirme `Administracion`.
- No se encontro `componentId` raiz para `Administracion`.
- No se encontraron hijos, tabs, submenus, datasources, acciones ni permisos concretos asociados a `Administracion`.
- Si se encontro infraestructura administrativa generica de AppBuilder: carga dinamica de menus, administracion de aplicaciones, gestion de menus, gestion de componentes, origenes de datos, workflows, opciones de usuario y permisos por grupos/objetos.
- Si se encontro una pantalla interna del Builder llamada tecnicamente `applicationsadmin`, accesible por la ruta `/aplicaciones/gestion` cuando AppBuilder inyecta `Sistema > Builder` para determinados usuarios administradores.

## Evidencia en iLiniumTech

Fuente: `iLiniumTech.Frontend/src/layout/appNavigation.ts`.

Entrada actual:

| Campo | Valor |
| --- | --- |
| `label` | `Administracion` |
| `icon` | `pi pi-table` |
| `disabled` | `true` |
| `to` | ausente |
| `requiredPermission` | ausente |
| `children` | ausente |

Interpretacion:

- iLiniumTech reserva una entrada visual para `Administracion`.
- La entrada esta aparcada.
- No existe ruta Vue.
- No existe permiso iLiniumTech.
- No existe contrato API.
- No hay submenus definidos en la navegacion estatica.
- No hay evidencia suficiente para activarla ni para inferir estructura funcional.

## Evidencia AppBuilder generica de menu

AppBuilder resuelve el menu desde metadata. La operacion `Menu_GET_BY_APPLICATION_ID.graphql` carga menus por:

- `administrator`;
- `profileId`;
- `applicationId`;
- `version`.

El contrato devuelve campos como:

- `id`;
- `applicationId`;
- `applicationVersion`;
- `componentId`;
- `urlComponentStatic`;
- `urlRouteComponentStatic`;
- `title`;
- `idIcon`;
- `parentId`;
- `contract`;
- `builtin`;
- `order`;
- `active`;
- `keepAlive`;
- `localizations`.

El modelo `IapMenu` confirma que una entrada heredada puede estar enlazada a un componente dinamico (`componentId`), a rutas estaticas y a una jerarquia por `parentId`.

El modulo `AuthModule.ts` carga menus de aplicacion usando el estado de administrador del usuario, el perfil actual, la aplicacion y la version. Despues llama a `HelperMenu.addMenuBuilder` y a `HelperMenu.buildMenu`.

`bllMenu.cs` obtiene menus de aplicacion y menus relacionados; para usuarios no administradores filtra por grupos de usuario y directivas de objeto. Esto demuestra que AppBuilder mezcla menu, perfil, grupos y metadata de objetos para decidir visibilidad.

Para `Administracion`, esta evidencia solo explica como funciona AppBuilder de forma generica. No identifica la pagina concreta ni permite migrar comportamiento sin una extraccion sanitizada.

## Evidencia AppBuilder relacionada con `Sistema > Builder`

`HelperMenu.addMenuBuilder` contiene una inyeccion especial:

- solo se activa fuera de aplicacion estatica;
- requiere que la aplicacion no sea la aplicacion base de Builder;
- requiere usuario administrador;
- restringe por dominios corporativos concretos en el nombre de usuario;
- si no existe un menu `Sistema`, lo crea como padre;
- agrega un hijo `Builder`;
- el hijo usa ruta `/aplicaciones/gestion`;
- el componente estatico asociado es `builder/application/infrastructure/component/Applications.vue`.

La pantalla `Applications.vue` declara `name: 'applicationsadmin'` y presenta pestanas internas reales:

| Tab observada | Componente o area asociada | Interpretacion |
| --- | --- | --- |
| `detail` | `ApplicationDetail` | Datos generales de la aplicacion seleccionada |
| `menus` | `MenuComp` | Gestion del arbol de menus de una aplicacion |
| `datasources` | `Designer` en modo origenes de datos | Gestion de origenes de datos del Builder |
| `layout` | `Designer` en modo layout | Gestion de layout/componentes de pagina |
| `components` | `Designer` en modo componentes | Gestion de componentes AppBuilder |
| `engine` | area de procesos | Procesos/workflows del Builder |
| `help` | `GeneralHelpComp` | Ayuda general |
| `back` | navegacion | Vuelta/listado |

Ademas, el directorio de componentes de aplicacion contiene:

- `ApplicationDetail.vue`;
- `ApplicationDetailConfig.vue`;
- `ApplicationConnection.vue`;
- `Applications.vue`.

Y el directorio de componentes de menu contiene:

- `MenuComp.vue`;
- `MenuRelated.vue`;
- `MenuTree.vue`;
- `MenuTreeValue.vue`;
- `NewMenu.vue`;
- `SecurityValue.vue`.

Interpretacion:

- Esta si es una pantalla administrativa real del propio AppBuilder.
- Sus tabs son verificables, pero estan vinculadas a `Sistema > Builder`, no a una pagina `Administracion` confirmada del menu funcional de iLiniumTech.
- No se debe asumir que `Administracion` en iLiniumTech equivale a `Sistema > Builder`.
- Reimplementar `Sistema > Builder` seria una decision de producto y seguridad distinta, de alto impacto, porque permitiria administrar metadata, menus, componentes, datasources, conexiones, configuracion y procesos.

## Evidencia AppBuilder de operaciones administrativas

El inventario seguro de operaciones GraphQL muestra capacidades administrativas del motor:

| Area | Operaciones observadas | Riesgo si se migra sin SDD |
| --- | --- | --- |
| Aplicaciones | `Application_GET_ALL`, `Application_ADD`, `Application_UPDATE`, `Application_DELETE`, `Application_CLONE` | Administracion de producto/meta-aplicaciones |
| Configuracion | `ApplicationConfiguration_*`, `Configuration_GET_ALL` | Posible exposicion de configuracion interna |
| Conexiones | `ApplicationConnection_*` | Riesgo alto por infraestructura y secretos si se consulta dato real |
| Menus | `Menu_*`, `MenuDirective_*`, `MenuRelated_*` | Reintroduccion de menu dinamico |
| Componentes | `Component_*`, `ComponentAttribute_*`, `ComponentProperty_*`, `ComponentEvent_*` | Reintroduccion de UI dinamica |
| Datasources | `DataSource_*`, `DataSourceField_*`, `DataSourceService_*` | Reintroduccion de SQL/servicios dinamicos |
| Usuarios/opciones | `User_*`, `UserOption_*`, `SearchConfig_*` | Riesgo PII y permisos |
| Workflows | `WorkFlow_*`, `WorkFlowActivity_*` | Riesgo de ejecucion generica de procesos |
| ObjectGroups | `ObjectGroup_*` | Permisos heredados por objeto/grupo |

Esta evidencia ayuda a entender que una pagina de administracion heredada podria tener mucho alcance. Precisamente por eso no debe activarse ni replicarse sin definicion funcional, threat review, permisos productivos y UAT.

## Evidencia ausente

No se ha encontrado evidencia local verificable de:

- `IapMenu` real cuyo `title` sea `Administracion`;
- localizacion de menu equivalente a `Administracion`;
- `menuId`;
- `componentId` raiz;
- ruta estatica heredada propia;
- menu padre;
- hijos de menu;
- tabs concretas de una pagina `Administracion`;
- submenus funcionales;
- componente raiz AppBuilder;
- arbol de componentes;
- datasources;
- campos, columnas o catalogos;
- filtros;
- acciones;
- permisos por `IapObjectGroup` aplicados a esta pagina;
- directiva historica concreta;
- workflows o expresiones propios;
- owner funcional;
- criterios UAT.

## Raiz, hijos, tabs y submenus

Estado detectado:

| Elemento | Resultado |
| --- | --- |
| Menu raiz heredado `Administracion` | No encontrado |
| `componentId` raiz | No encontrado |
| Ruta estatica heredada propia | No encontrada |
| Hijos de menu | No encontrados |
| Tabs reales de `Administracion` | No encontradas |
| Submenus reales de `Administracion` | No encontrados |
| Componentes internos reales | No encontrados |
| Datasources reales | No encontrados |
| Workflows reales | No encontrados |

Decision de esta ronda:

- No se crean subagentes de componentes.
- No se crean ficheros bajo `docs/appbuilder/pages/administracion/components/*.md`.

Razon: las pestanas reales observadas pertenecen a la pantalla interna `Sistema > Builder`, no a una pagina `Administracion` confirmada. Crear documentos de componentes ahora generaria falsa trazabilidad y podria empujar a desarrollar una pantalla inventada o demasiado amplia.

Condicion para crear subagentes en una ronda futura:

- localizar una fila sanitizada de `IAP_Menu` o equivalente con `title = Administracion` o localizacion confirmada;
- localizar `componentId` raiz o ruta estatica propia;
- obtener arbol sanitizado de componentes;
- confirmar si sus tabs/submenus son funcionales o tecnicos;
- confirmar con producto si debe migrarse como modulo iLiniumTech o si se descarta por ser administracion del motor AppBuilder;
- asignar un subagente por componente real con escritura acotada a `docs/appbuilder/pages/administracion/components/<slug>.md`.

## Datos y origenes

Datos concretos de una pagina `Administracion`:

- No encontrados.

Datos administrativos genericos observados en AppBuilder:

- aplicaciones;
- configuraciones de aplicacion;
- relaciones entre aplicaciones;
- menus;
- componentes;
- atributos, propiedades y eventos;
- datasources;
- campos de datasource;
- conexiones de aplicacion;
- usuarios;
- perfiles y opciones de usuario;
- grupos y permisos por objeto;
- workflows y actividades.

Estos datos son del motor AppBuilder, no de una pagina `Administracion` de negocio confirmada.

Regla para futuro desarrollo:

- no crear endpoints de `Administracion` hasta confirmar el problema de negocio;
- no exponer conexiones, configuracion interna, SQL, metadata o workflows;
- no usar `ApplicationConnection_*` para mostrar datos sensibles;
- no ejecutar `QueryStatic`;
- no construir SQL estructural desde metadata;
- validar sesion, broker y permisos antes de cualquier acceso a datos;
- clasificar PII y secretos antes de definir DTOs.

## Filtros

No hay filtros heredados verificables para `Administracion`.

Filtros candidatos solo si producto define una pagina administrativa concreta:

- texto libre;
- estado activo/inactivo;
- tipo de entidad administrativa;
- aplicacion;
- perfil;
- grupo;
- permiso;
- fecha de alta/modificacion;
- usuario modificador;
- broker o entidad principal si aplica.

Estos filtros son hipotesis de producto, no evidencia AppBuilder. No deben implementarse sin SDD y UAT.

## Acciones

No hay acciones heredadas verificables de una pagina `Administracion`.

Acciones observadas en el Builder interno, no migrables automaticamente:

- crear, editar, eliminar y clonar aplicaciones;
- obtener, editar y eliminar menus;
- gestionar componentes;
- gestionar datasources;
- gestionar configuraciones;
- gestionar conexiones;
- refrescar cache;
- gestionar ayuda;
- gestionar procesos/workflows.

Acciones candidatas para una futura pagina iLiniumTech solo si se aprueba alcance:

- listar elementos administrativos;
- consultar detalle;
- activar/desactivar;
- editar configuracion no sensible;
- gestionar permisos de producto;
- consultar auditoria;
- exportar inventario no sensible.

Toda accion de escritura o ejecucion requeriria SDD propia, auth productiva, permisos efectivos, auditoria, pruebas de integracion, revision de seguridad y UAT.

## Permisos historicos

Evidencia generica:

- AppBuilder pasa `administrator` y `profileId` al obtener menus.
- Backend AppBuilder filtra menus de no administradores por grupos de usuario y directivas de objeto.
- Existen `ObjectGroup` y permisos por accion en metadata.
- La pantalla interna `Sistema > Builder` se inyecta solo para usuarios administradores concretos en condiciones restringidas.

Permisos historicos concretos de `Administracion`:

- No encontrados.

Permisos iLiniumTech candidatos, pendientes de SDD:

| Permiso candidato | Uso posible | Estado |
| --- | --- | --- |
| `administracion.read` | Ver landing o listado administrativo read-only | No confirmado |
| `administracion.users.read` | Consultar usuarios/perfiles si se aprueba | No confirmado |
| `administracion.permissions.read` | Consultar matriz de permisos iLiniumTech | No confirmado |
| `administracion.permissions.update` | Modificar permisos de producto | Alto riesgo, no confirmado |
| `administracion.audit.read` | Consultar auditoria | No confirmado |
| `administracion.config.read` | Consultar configuracion no sensible | No confirmado |
| `administracion.config.update` | Cambiar configuracion de producto | Alto riesgo, no confirmado |

No se debe mapear `isAdmin` heredado directamente a permisos productivos. En iLiniumTech, la autoridad debe venir de auth productiva y permisos explicitos del backend.

## Propuesta Vue estatica futura

Esta propuesta no autoriza desarrollo. Solo marca un camino si producto confirma el alcance.

Ruta candidata:

- `/administracion`

Menu candidato:

- `label`: `Administracion`
- `icon`: pendiente de validar; el actual `pi pi-table` parece placeholder
- `requiredPermission`: `administracion.read`
- `disabled`: `false` solo cuando exista SDD, API, permisos, tests y UAT minimo

Estructura candidata read-only:

- `iLiniumTech.Frontend/src/features/administracion/AdministracionView.vue`
- `iLiniumTech.Frontend/src/features/administracion/AdministracionDashboard.vue`
- `iLiniumTech.Frontend/src/features/administracion/AdministracionSectionList.vue`
- `iLiniumTech.Frontend/src/features/administracion/AdministracionAuditTable.vue`
- `iLiniumTech.Frontend/src/features/administracion/administracionTypes.ts`
- `iLiniumTech.Frontend/src/features/administracion/useAdministracion.ts`
- `iLiniumTech.Frontend/src/services/administracion.ts`

Areas candidatas, solo con SDD:

- usuarios y perfiles iLiniumTech;
- brokers permitidos y broker activo;
- permisos iLiniumTech;
- auditoria;
- configuracion de producto no sensible;
- estado de integraciones, sin secretos;
- runbooks o enlaces internos, sin credenciales.

Reglas frontend:

- no reutilizar `Applications.vue`;
- no traer `FormBuilder`;
- no renderizar menus desde `IAP_Menu`;
- no renderizar componentes desde `IAP_Component`;
- no mostrar `IAP_*`, `QueryStatic`, SQL, connection strings, rutas internas sensibles ni secretos;
- leer identidad, broker y permisos desde `/api/me`;
- mostrar estados `loading`, `empty`, `error`, `sin sesion`, `sin broker`, `sin permiso` y `bloqueado`;
- no mostrar acciones de escritura sin permiso efectivo y SDD;
- no permitir configuracion arbitraria desde querystring.

## Propuesta API estatica futura

Solo si hay SDD, origen de datos autorizado y permiso funcional confirmado:

- `GET /api/administracion/summary`
- `GET /api/administracion/users`
- `GET /api/administracion/permissions`
- `GET /api/administracion/audit`
- `GET /api/administracion/configuration`

Si se aprueban escrituras, cada una debe tener SDD especifica:

- `PUT /api/administracion/permissions/{id}`
- `PUT /api/administracion/configuration/{key}`
- `POST /api/administracion/users/{id}/disable`

Reglas backend:

- validar sesion antes de resolver datos;
- validar `currentBrokerId` contra `allowedBrokerIds` si el dato depende de broker;
- aplicar permisos por endpoint;
- usar DTOs explicitos;
- usar repositorios explicitos;
- usar whitelists para filtros, sort y columnas;
- parametrizar valores;
- devolver errores sanitizados con `correlationId`;
- no exponer secretos, connection strings, tokens, SQL, nombres internos innecesarios ni PII;
- auditar lecturas sensibles y todas las escrituras;
- no aceptar `isAdmin` desde cabeceras/frontend como autoridad.

## Que no debe replicarse

No replicar:

- `Sistema > Builder` como modulo productivo sin decision explicita;
- `Applications.vue` como runtime de administracion iLiniumTech;
- `FormBuilder`;
- `Designer`;
- `MenuComp` como editor de menus productivo;
- `IAP_Menu` como contrato runtime;
- `IAP_Component` como contrato runtime;
- `IAP_DataSource` como contrato runtime;
- `ApplicationConnection` con datos reales o sensibles;
- workflows Rete;
- expresiones dinamicas;
- operaciones GraphQL genericas `Application_*`, `Component_*`, `DataSource_*`, `Menu_*`, `WorkFlow_*`;
- permisos AppBuilder como autoridad productiva;
- SQL heredado o `QueryStatic`;
- endpoints genericos de metadata;
- visibilidad por `administrator` o `isAdmin` sin matriz de permisos iLiniumTech.

## Riesgos

### Producto

- Confundir `Administracion` del producto final con la administracion tecnica del Builder.
- Convertir una entrada placeholder en una pantalla demasiado amplia.
- Crear un modulo sin owner funcional ni UAT.
- Migrar capacidades de bajo nivel que el usuario final no necesita.

### Seguridad

- Exponer usuarios, perfiles, grupos o permisos sin auth productiva.
- Exponer conexiones, configuraciones, rutas internas o secretos.
- Permitir cambios de menu, componentes, datasources o workflows desde el producto.
- Usar API key, demo-session o headers MVP como seguridad productiva.
- No auditar acciones administrativas.

### Datos y privacidad

- Fuga de datos personales de usuarios o perfiles.
- Fuga de configuracion sensible.
- Fuga de metadata interna que facilite movimiento lateral.
- Logs con payloads administrativos sensibles.

### Multi-tenant

- No saber si la administracion es global, por broker, por aplicacion o por perfil.
- Mostrar permisos o usuarios de otro broker.
- Resolver datos antes de validar broker.
- Cachear informacion administrativa sin aislarla por usuario/broker/perfil.

### Tecnicos

- Reintroducir AppBuilder por la puerta trasera.
- Crear una pantalla generica de metadata que sea dificil de probar.
- Activar escrituras sobre configuracion antes de tener rollback y auditoria.
- Mezclar administracion de producto iLiniumTech con administracion heredada AppBuilder.

## Bloqueos

Bloqueos externos:

- Confirmar con producto que significa `Administracion` en iLiniumTech.
- Confirmar si debe existir equivalencia con AppBuilder o si es funcionalidad nueva.
- Confirmar si `Sistema > Builder` debe descartarse, documentarse como herramienta interna o migrarse parcialmente.
- Obtener metadata sanitizada de `IAP_Menu` si existe una pagina heredada real.
- Obtener `componentId` raiz o ruta estatica confirmada.
- Obtener arbol sanitizado de componentes si aplica.
- Obtener datasources/campos/configuraciones sanitizadas si se va a usar como evidencia.
- Confirmar origen de datos autorizado con DBA o responsables de plataforma.
- Confirmar alcance: read-only, CRUD, administracion de permisos, auditoria, configuracion o workflows.
- Confirmar permisos funcionales por usuario, broker, perfil, oficina y rol.
- Confirmar owner UAT.
- Completar decision de auth productiva antes de escrituras o datos administrativos reales.

Bloqueos tecnicos:

- No existe ruta Vue.
- No existe contrato API.
- No existe fixture anonimizado.
- No existe SDD.
- No existe matriz de permisos.
- No existe evidencia de tabs/submenus propios de `Administracion`.

## Pruebas necesarias para desarrollo futuro

### Documentacion

- SDD de `Administracion` aprobada.
- Decision de alcance: administracion de producto iLiniumTech frente a Builder heredado.
- Threat review si hay usuarios, permisos, configuracion, conexiones, auditoria o escrituras.
- Evidencia de no runtime AppBuilder.

### Backend

- 401 sin sesion.
- 403 sin `administracion.read`.
- 403 para broker no permitido si aplica.
- 403 para escritura sin permiso especifico.
- Broker ausente cuando el dato sea tenant-specific.
- Whitelist de filtros y sort.
- Payloads maliciosos.
- Errores sanitizados con `correlationId`.
- No fuga de secrets, connection strings, SQL, tokens ni metadata interna sensible.
- Auditoria de detalle y escrituras.
- Tests de aislamiento por usuario/broker/perfil.

### Frontend

- Menu visible o habilitado solo con permiso.
- Ruta protegida.
- Estados `loading`, `empty`, `error`, `no permission`, `no broker`, `blocked`.
- No renderiza metadata AppBuilder.
- No aparecen `AppBuilder`, `QueryStatic`, `IAP_`, nombres SQL, connection strings ni secretos en DOM.
- Acciones de escritura ocultas/deshabilitadas sin permiso.
- Responsive desktop/mobile.

### E2E/UAT

- Login -> Administracion -> consultar area permitida -> logout.
- Acceso sin permiso.
- Cambio de broker validado por backend si aplica.
- Intento de acceso a dato administrativo de otro broker.
- UAT con responsable humano y datos sanitizados.

### Seguridad

- Secret scan.
- Dependency audit si se introduce libreria nueva.
- CORS audit si se toca API/configuracion.
- Revision de PII.
- Revision de logs.
- Revision de auditoria.

## Decision sobre subagentes de componentes

No se crean subagentes en esta ronda.

Razon:

- no hay tabs reales verificadas de una pagina `Administracion`;
- no hay submenus reales verificados;
- no hay componentes internos reales verificados;
- las tabs de `Applications.vue` pertenecen a `Sistema > Builder`, no a `Administracion` confirmada;
- documentarlas como componentes de `Administracion` generaria una relacion no demostrada.

Si en una ronda futura se confirma que `Administracion` debe mapear a `Sistema > Builder`, entonces la organizacion documental deberia reabrirse con subagentes por area real:

- `detalle-aplicacion`;
- `menus`;
- `origenes-datos`;
- `layout`;
- `componentes`;
- `procesos`;
- `ayuda`;
- `conexiones`;
- `seguridad-menu`.

Pero esa ronda requiere antes una decision explicita de producto y seguridad. Sin esa decision, no se deben crear esos documentos ni desarrollar codigo.

## Evidencia de comandos

Comandos ejecutados durante la ronda:

```powershell
git status --short --branch
Get-Content -Raw AGENTS.md
Get-Content -Raw PLANS.md
Get-Content -Raw README.md
Get-Content -Raw docs\PLAN_MAESTRO_IA.md
Get-Content -Raw docs\ROADMAP_OBJETIVO_FINAL.md
Get-Content -Raw docs\DECISION_PRODUCTO_ARQUITECTURA.md
Get-Content -Raw docs\appbuilder\pages\README.md
Get-Content -Raw iLiniumTech.Frontend\src\layout\appNavigation.ts
rg -n "Administracion|Administracion|Admin\b|Administrar" docs iLiniumTech.Frontend\src iLiniumTech.Backend\src
rg -n "Administracion|Administracion|Administration" C:\Desarrollo\AppBuilder\src
rg -n "administrador|administrative|administrator" C:\Desarrollo\AppBuilder\src
rg -n "Sistema|Builder|aplicaciones/gestion|applicationsadmin" C:\Desarrollo\AppBuilder\src
rg --files C:\Desarrollo\AppBuilder\src
```

Las busquedas se ejecutaron excluyendo, cuando procedia:

- `.git`;
- `bin`;
- `obj`;
- `packages`;
- `node_modules`;
- `*.tsbuildinfo`;
- `appsettings*.json`;
- `*.config`;
- `*.pubxml`;
- `.env*`;
- backups;
- comprimidos;
- imagenes y PDFs.

Validaciones pendientes tras crear este documento:

- `Test-DocumentationBaseline.ps1`
- `Invoke-SecretScan.ps1`
- `git diff --check`

No se ejecutan build ni tests backend/frontend porque la ronda es exclusivamente documental y no toca codigo de aplicacion.

## Estado final de pagina

Clasificacion:

- Completado con evidencia: entrada `Administracion` localizada en el menu iLiniumTech; mecanismo generico AppBuilder de menus; evidencia relacionada de `Sistema > Builder`; inventario de areas administrativas heredadas de alto riesgo.
- Pendiente tecnico: localizar metadata sanitizada real de una pagina `Administracion`, si existe.
- Bloqueado externo: definicion funcional, alcance, permisos, origen de datos, auth productiva, multi-tenant y UAT.

Conclusion:

`Administracion` no esta lista para desarrollo. El siguiente paso correcto es preguntar a producto que significa exactamente esta entrada de menu y decidir si se trata de administracion de producto iLiniumTech, de una pantalla heredada real aun no localizada o de una herramienta interna tipo Builder que no debe migrarse al producto final.

Confirmacion de arquitectura:

- No se introduce runtime AppBuilder.
- No se consume metadata AppBuilder en frontend/backend.
- No se han creado documentos de componentes sin evidencia.
- La propuesta futura, si se desbloquea, debe ser Vue estatico + API explicita.

## Readiness tecnico/admin actualizado 2026-05-18

Estado actual en iLiniumTech:

- Existe ruta protegida `/administracion` como pagina Vue estatica.
- Existe fixture local read-only con filtros/paginacion locales y acciones deshabilitadas.
- No existe API backend de administracion.
- No existen permisos iLiniumTech aprobados para administracion.
- No se han conectado usuarios reales, permisos reales, conexiones, configuracion interna ni auditoria real.
- La evidencia frontend verifica que no se renderizan marcadores como `IAP_`, `QueryStatic`, `ComponentDataSource`, `SELECT *`, secretos ni datos reales.

Acciones bloqueadas:

- crear usuarios;
- editar permisos;
- exportar auditoria;
- abrir detalle operativo;
- consultar datos administrativos reales;
- administrar Builder, menus, componentes, datasources, conexiones o workflows heredados.

Permisos candidatos no aprobados:

- `administracion.read`;
- `administracion.users.read`;
- `administracion.permissions.read`;
- `administracion.permissions.update`;
- `administracion.audit.read`;
- `administracion.config.read`;
- `administracion.config.update`.

Threat model requerido antes de cualquier dato/API:

- identidad productiva y permisos efectivos;
- separacion entre administracion iLiniumTech y administracion AppBuilder/Builder;
- acceso a usuarios, perfiles, permisos y auditoria;
- minimizacion de PII y secretos;
- auditoria de lecturas sensibles y escrituras;
- aislamiento por broker, perfil y rol;
- redaccion de errores y logs.

Tareas futuras pequenas recomendadas:

1. Confirmar con producto que significa `Administracion` en iLiniumTech.
2. Decidir explicitamente si `Sistema > Builder` queda descartado, documentado como herramienta interna o migrado parcialmente.
3. Preparar SDD read-only solo si hay owner UAT y caso de uso.
4. Definir matriz de permisos candidata sin concederla en runtime.
5. Mantener la pagina fixture bloqueada y sus tests de no secretos/no runtime AppBuilder.

Criterios de aceptacion para avanzar:

- SDD aprobada;
- security review completada;
- permisos backend definidos y probados;
- contrato API explicito y no generico;
- ninguna metadata AppBuilder como runtime;
- ninguna exposicion de usuarios reales, permisos reales, conexiones, secretos, SQL o rutas internas;
- UAT owner y datos sanitizados confirmados.
