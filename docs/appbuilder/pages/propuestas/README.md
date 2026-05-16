# Pagina AppBuilder: Propuestas

Fecha de analisis: 2026-05-16.

Rol de esta ronda: jefe de pagina para `Propuestas`.

Estado final: `bloqueado externo`.

Motivo del estado: existe la entrada `Propuestas` en el menu objetivo de iLiniumTech, pero no se ha encontrado metadata local suficiente que permita identificar de forma verificable su menu AppBuilder real, su `componentId`, su arbol de componentes, sus tabs, sus submenus, sus datasources, sus filtros o sus acciones. No debe iniciarse desarrollo funcional de la pagina hasta obtener una extraccion sanitizada o una consulta autorizada de metadata AppBuilder.

## Regla base

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. Este documento es evidencia de analisis y guia para una futura SDD/scaffolding revisado. La pagina final, si se aprueba, debe quedar como Vue/TypeScript estatico y backend .NET con API explicita. Ningun frontend/backend productivo debe consultar `IAP_Menu`, `IAP_Component`, `IAP_DataSource` o metadata heredada para pintar la pantalla en runtime.

## Fuentes revisadas

Documentacion iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`

Codigo iLiniumTech:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

Fuentes AppBuilder revisadas, sin ejecutar SQL ni extractor Live:

- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapMenu.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapMenuRelated.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapComponent.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder\Schema\Query\Builder\App\MenuQuery.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Schema\Type\App\MenuType.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\modelos\menu\CustomMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\MenuApolloClientRepository.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\query\menuQuery.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\almacen\modules\AuthModule.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenu.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenuItem.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Component_GET_COMPONENT_TREE_ALLOBJCETS_BY_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\ControlTypeConst.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\CrudTableTypeConst.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\TabPanelTypeConst.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ControlTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\TabPanelTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Helper\HelperAppSolicitudes.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\TipoBBDDConst.cs`

Fuentes evitadas o no usadas como evidencia:

- `appsettings*`, `*.config`, connection strings, dumps, capturas, binarios y salidas sensibles.
- Ficheros de migracion/scaffold historicos que pueden contener credenciales o valores de entorno. No deben usarse para documentar la pagina.
- Extractor `Live` y SQL real.

## Busquedas ejecutadas

Se usaron busquedas locales con `rg` sobre `C:\Desarrollo\AppBuilder` e iLiniumTech con terminos:

- `Propuestas`
- `Propuesta`
- `propuest`
- `Solicitud`
- `Solicitudes`
- `Cotizacion`
- `Cotizacion`
- `Cotizar`
- `Menu`
- `IapMenu`
- `ComponentId`
- `tab`
- `submenu`

Resultado relevante:

- No aparece una pantalla concreta llamada `Propuestas` en los ficheros de fuente revisados.
- No se ha encontrado un `componentId` local asociado a `Propuestas`.
- No se ha encontrado un arbol de componentes especifico de `Propuestas`.
- Aparece el concepto `Solicitudes`, pero como tipo/base logica y servicios auxiliares de sesion/usuario, no como prueba suficiente de que la pagina de negocio `Propuestas` sea esa pantalla.

## Evidencia en iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts` existe una entrada:

- `label`: `Propuestas`
- `icon`: `pi pi-folder-open`
- `disabled`: `true`
- `to`: ausente
- `requiredPermission`: ausente
- `children`: ausente

Interpretacion:

- iLiniumTech reconoce `Propuestas` como item de navegacion futuro.
- Actualmente no existe pagina Vue, ruta, permiso, contrato API ni submenu definido.
- No hay evidencia de tabs o hijos dentro de nuestra navegacion estatica actual.

## Evidencia AppBuilder generica

AppBuilder resuelve menus y pantallas de forma dinamica desde metadata:

- `IapMenu` modela entradas de menu con `ApplicationId`, `ApplicationVersion`, `ComponentId`, `UrlComponentStatic`, `UrlRouteComponentStatic`, `Title`, `IdIcon`, `ParentId`, `Order`, `Active` y `KeepAlive`.
- `IapMenuRelated` repite ese modelo para aplicaciones relacionadas.
- `MenuQuery.getByApplicationId` devuelve menus por aplicacion, version, perfil y modo administrador, aplicando directivas de usuario.
- `Menu_GET_BY_APPLICATION_ID.graphql` obtiene campos de menu y localizaciones.
- `AuthModule.SET_MENU` llama al servicio de menus, pasa por `HelperMenu.buildMenu` y guarda el arbol en Vuex.
- `HelperMenu.buildMenu` transforma `ParentId` en `items` recursivos para PrimeVue.
- `AppMenu.vue` lee `getApplicationMenus` desde el store.
- `AppMenuItem.vue` renderiza nodos con `item.items` como submenus recursivos y rutas `item.to`.

Esta evidencia explica como funcionaria el menu heredado, pero no identifica la pagina `Propuestas` concreta.

## Evidencia de componentes AppBuilder

AppBuilder modela componentes genericos mediante `IapComponent`:

- identidad: `Id`, `ApplicationId`, `ApplicationVersion`, `ParentId`, `TreePath`;
- descripcion: `Name`, `Description`, `IdType`, `IdSubType`, `IdCategoryType`, `IdIcon`;
- jerarquia: `Parent`, `InverseParent`, `Order`;
- comportamiento: atributos, eventos, datasources, expresiones, workflows y permisos por grupos;
- reutilizacion: `Reusable`, `ReusableComponentId`, `ReusableApplicationId`, `ReusableApplicationVersion`.

La operacion `Component_GET_COMPONENT_TREE_ALLOBJCETS_BY_ID.graphql` puede devolver:

- arbol de componentes;
- atributos;
- eventos;
- datasources;
- campos y configuraciones de campos;
- tablas/alias;
- lookups;
- expresiones;
- workflows;
- `objectGroups` con flags `add`, `edit`, `list`, `delete`, `view`, `import`, `export`, `execute`.

Pero no se ha podido ejecutar contra metadata real de `Propuestas`, y no hay fixture local sanitizado equivalente. Por tanto, no hay componentes hijos reales de `Propuestas` que documentar en ficheros `components/*.md`.

## Tabs, submenus y componentes internos

Componentes genericos existentes en AppBuilder que podrian aparecer en una pantalla:

- `tipocontrol-tabv`: TabView.
- `tipocontrol-tabpnl`: TabPanel.
- `tipocontrol-tablist`: lista de tabs.
- `tipocontrol-tabpnls`: contenedor de paneles.
- `tipocontrol-tab`: tab individual.
- `tipocontrol-crudtbl`: tabla CRUD/listado dinamico.
- `layouttemp-crudtbl-detail`: layout de detalle de tabla CRUD.
- `layouttemp-crudtbl-header`: cabecera de CRUD.
- `layouttemp-crudtbl-menu`: menu de acciones.
- `layouttemp-crudtbl-controlssearch`: controles de busqueda.
- `layouttemp-crudtbl-buttonsresult`: botones de resultados.
- `layouttemp-tabpnl-default`: contenido por defecto de tab panel.
- `layouttemp-tabpnl-header`: cabecera de tab panel.

Estado para `Propuestas`:

- Tabs reales detectadas: ninguna.
- Submenus reales detectados: ninguno.
- Componentes internos reales detectados: ninguno.
- Subagentes de componentes creados: ninguno, porque crear documentacion de componentes sin `componentId` o metadata concreta seria inventar estructura.

## Datos y origenes

Evidencia encontrada:

- AppBuilder distingue tipos logicos de base de datos. Entre ellos existe `tipobd-SO`, descrito como `Solicitudes`.
- Hay servicios y repositorios para `Solicitudes` relacionados con `Session` y `UserBcEntity`.
- `HelperAppSolicitudes` localiza la conexion de `Solicitudes` a partir de conexiones de broker.

Lo que no queda probado:

- Que `Propuestas` use `tipobd-SO`.
- Que `Propuestas` sea sinonimo de `Solicitudes`.
- Que las propuestas vivan en Modelo, Solicitudes, Motor u otra base.
- Que existan tablas/vistas concretas para listado/detalle de propuestas.
- Que existan catalogos de estado, ramo, producto, cliente, compania o mediador asociados a esta pagina.

Conclusion de datos:

- No se debe construir API de Propuestas hasta obtener metadata sanitizada o una definicion funcional/DBA aprobada.
- Cualquier futuro acceso debe resolverse por broker autorizado y repositorio explicito, no por `QueryStatic`.

## Filtros

No hay filtros heredados verificables para `Propuestas`.

Filtros candidatos que podrian tener sentido funcional, pero que no deben implementarse sin validacion:

- numero de propuesta;
- cliente;
- estado;
- fecha de alta/efecto/caducidad;
- producto/ramo;
- compania;
- mediador/oficina/gestor;
- texto libre.

Regla para futuro desarrollo:

- Cada filtro debe venir de SDD/UAT o de metadata sanitizada revisada.
- Cada filtro backend debe mapearse a columnas whitelisted y parametros.
- No se debe ejecutar SQL heredado ni interpolar campos libres.

## Acciones

No hay acciones heredadas verificables para `Propuestas`.

Acciones candidatas a validar antes de cualquier desarrollo:

- listar propuestas;
- buscar/filtrar;
- abrir detalle;
- crear propuesta;
- editar borrador;
- duplicar;
- convertir a poliza;
- anular/cerrar;
- exportar;
- adjuntar/ver documentos;
- lanzar workflow de tarificacion/emision.

Regla:

- En una primera SDD se recomienda limitar `Propuestas` a read-only si producto quiere avanzar rapido.
- Cualquier escritura, conversion, emision, workflow, REST/SOAP o integracion externa necesita SDD propia, permisos, auditoria, rollback y UAT.

## Permisos historicos

Evidencia generica:

- AppBuilder usa `objectGroups` sobre menus/componentes/datasources/eventos.
- Los flags observables en metadata de componentes incluyen `add`, `edit`, `list`, `delete`, `view`, `import`, `export` y `execute`.
- `MenuQuery.getByApplicationId` recibe `administrator`, `profileId`, `applicationId` y `version`, por lo que el menu heredado depende de perfil/directivas.

Permisos historicos concretos de `Propuestas`:

- No encontrados.

Permisos iLiniumTech candidatos, pendientes de SDD:

- `propuestas.read`
- `propuestas.detail`
- `propuestas.create`
- `propuestas.update`
- `propuestas.cancel`
- `propuestas.convertToPolicy`
- `propuestas.export`
- `propuestas.documents.read`
- `propuestas.workflow.execute`

No deben activarse permisos de escritura sin auth productiva, broker validado, matriz funcional y UAT.

## Propuesta Vue estatica futura

Solo si producto confirma alcance y existe SDD, la primera version recomendable seria read-only:

- Ruta: `/propuestas`.
- Entrada de menu estatica: `Propuestas`, con `requiredPermission: 'propuestas.read'`.
- Pagina Vue: `PropuestasView.vue`.
- Componentes esperados:
  - `PropuestasFilters.vue`;
  - `PropuestasTable.vue`;
  - `PropuestaStatusBadge.vue`;
  - `PropuestasEmptyState.vue`;
  - `PropuestasErrorState.vue`;
  - `PropuestaDetailView.vue` si se aprueba detalle;
  - componentes compartidos solo si ya existen patrones estables en Polizas.
- Estados obligatorios:
  - cargando;
  - sin resultados;
  - error backend;
  - sin sesion;
  - sin broker;
  - sin permiso;
  - datos no disponibles por bloqueo externo.
- Navegacion:
  - filtros en query params si la pantalla se usa de forma diaria;
  - vuelta desde detalle conservando filtros;
  - no mostrar acciones sin permiso efectivo.

Esta propuesta no implica que se programe ahora. Es una guia para una SDD posterior.

## Propuesta API estatica futura

Solo si hay SDD y origen de datos autorizado:

- `GET /api/propuestas/catalogs`
- `GET /api/propuestas`
- `GET /api/propuestas/{id}`

Si se aprueban acciones:

- `POST /api/propuestas`
- `PUT /api/propuestas/{id}`
- `POST /api/propuestas/{id}/cancel`
- `POST /api/propuestas/{id}/convert-to-policy`
- `GET /api/propuestas/{id}/documents`

Reglas backend:

- validar sesion y broker antes de resolver conexion;
- aplicar permisos por endpoint;
- devolver errores sanitizados con `correlationId`;
- usar repositorios explicitos, whitelists y parametros;
- no ejecutar `QueryStatic`;
- no exponer SQL, nombres internos de metadata ni connection strings;
- minimizar PII por defecto;
- registrar auditoria de detalle y acciones sensibles.

## Riesgos

Riesgos de producto:

- Confundir `Propuestas` con `Solicitudes` sin confirmacion funcional.
- Convertir un nombre de menu en una pantalla sin entender el caso de uso.
- Implementar escrituras o conversion a poliza sin SDD ni UAT.

Riesgos tecnicos:

- Reintroducir runtime AppBuilder mediante menu/component metadata.
- Replicar un CRUD generico y no una API de producto.
- Depender de `QueryStatic` o de datasources heredados.
- No conocer columnas reales, claves primarias, joins o estados.

Riesgos de seguridad:

- Exponer PII de clientes/propuestas.
- Permitir broker cruzado.
- Mostrar existencia de propuesta sin permiso.
- Usar API key/demo-session como seguridad productiva.
- Loguear SQL, connection strings, tokens o payloads sensibles.

Riesgos multi-tenant:

- No saber si Propuestas vive en Modelo, Solicitudes u otra base.
- Resolver conexion antes de validar broker.
- No confirmar si requiere `SESSION_CONTEXT`.

## Bloqueos

Bloqueos externos:

- Obtener metadata sanitizada de `IAP_Menu` para la aplicacion/version real donde aparezca `Propuestas`.
- Obtener `componentId` raiz de `Propuestas`.
- Obtener arbol sanitizado de `IAP_Component` para ese `componentId`.
- Obtener datasources/campos/configuraciones sanitizadas si se van a usar como evidencia.
- Confirmar con producto si `Propuestas` es read-only, CRUD, tarificacion, emision o flujo de conversion.
- Confirmar con DBA el origen de datos real y vistas/tablas autorizadas.
- Confirmar permisos funcionales por broker, perfil, oficina, gestor y usuario.
- Confirmar UAT owner.

Bloqueos tecnicos:

- No existe ruta Vue de Propuestas.
- No existe contrato API de Propuestas.
- No existe fixture anonimizadora de Propuestas.
- No existe SDD de Propuestas.

## Pruebas necesarias para desarrollo futuro

Backend:

- 401 sin sesion.
- 403 sin permiso `propuestas.read`.
- 403/404 generico para broker cruzado.
- validacion de filtros y sort whitelist.
- payloads maliciosos en busqueda/sort.
- no fuga de PII en listado.
- no fuga de SQL/metadata en errores.
- tests de `SESSION_CONTEXT` si aplica.

Frontend:

- menu muestra/oculta Propuestas segun permiso.
- ruta protegida redirige o muestra estado sin permiso.
- filtros se inicializan y limpian correctamente.
- paginacion conserva query params.
- detalle conserva vuelta al listado.
- estados loading/empty/error/sin broker/sin permiso.
- no aparecen `AppBuilder`, `QueryStatic`, nombres de tablas ni connection strings en DOM.

QA/UAT:

- comparativa con AppBuilder solo con datos autorizados y sanitizados;
- validacion de columnas visibles;
- validacion de estados y acciones permitidas;
- smoke visual desktop/mobile;
- evidencia sin capturas sensibles ni datos personales reales.

## Decision sobre subagentes de componentes

No se crean documentos en `docs/appbuilder/pages/propuestas/components/*.md` en esta ronda.

Razon: no hay tabs, submenus ni componentes internos reales identificados. Crear subdocumentos ahora generaria una falsa precision y podria guiar a otros agentes hacia una implementacion inventada.

Condicion para crear subagentes/documentos de componentes en la siguiente ronda:

- disponer de un `componentId` raiz confirmado para `Propuestas`;
- disponer de un arbol sanitizado de componentes;
- clasificar cada hijo real como tab, submenu, CRUD, detalle, toolbar, formulario, documento, workflow u otro tipo;
- asignar un subagente por componente real con permisos de escritura acotados a `docs/appbuilder/pages/propuestas/components/<slug>.md`.

## Recomendacion siguiente

Antes de programar:

1. Crear SDD `Propuestas` con alcance funcional minimo.
2. Ejecutar extractor offline en modo autorizado/sanitizado o aportar un JSON revisado que contenga menu y componentes de `Propuestas`.
3. Revisar ese artefacto para separar:
   - evidencia historica;
   - propuesta de UI estatica;
   - contrato API explicito;
   - permisos;
   - datos sensibles;
   - bloqueos DBA/UAT.
4. Solo despues, activar jefes/agentes de desarrollo frontend/backend/QA.

Clasificacion:

- Completado con evidencia: analisis de navegacion iLiniumTech y mecanismo generico AppBuilder de menu/componentes.
- Pendiente tecnico: inventario real de metadata de `Propuestas` sanitizado.
- Bloqueado externo: definicion funcional, origen de datos, permisos reales y UAT.
