# Pagina AppBuilder: Clientes

Fecha: 2026-05-16

Rol de este documento: informe del jefe de pagina `Clientes` para migracion controlada a iLiniumTech.

Estado final de la ronda: `bloqueado externo para desarrollo final`, con evidencia tecnica parcial suficiente para preparar SDD y agentes de componentes, pero sin metadata local completa de la pantalla original.

Actualizacion 2026-05-18: iLiniumTech ya contiene ruta protegida `/clientes` y vista Vue fixture/read-only con datos anonimizados, filtros locales, PII bloqueada y acciones de ficha/exportacion/desglose deshabilitadas. Esa vista no autoriza API, datos reales, ficha, tabs, PII, exportacion ni escritura sin SDD, permisos, DBA/UAT y revision de seguridad.

SDD draft abierta para el primer corte futuro: [`SDD-2026-010 Clientes read-only minimizado`](../../../sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md). La SDD limita el primer incremento a listado read-only minimizado y deja ficha, tabs relacionadas, documento, contacto, direccion, banco, metricas economicas, exportacion y escrituras fuera de alcance.

## Regla de producto

iLiniumTech no debe replicar AppBuilder como runtime dinamico. La informacion de AppBuilder documentada aqui solo sirve para analisis, trazabilidad, SDD y scaffolding revisado. La futura pantalla de Clientes debe quedar escrita como Vue/TypeScript estatico y consumir una API .NET explicita.

## Alcance de esta ronda

- Solo documentacion.
- No se modifica frontend, backend, rutas, tests ni extractor.
- No se ejecuta SQL.
- No se ejecuta extractor `Live`.
- No se abren `appsettings`, connection strings, dumps, capturas sensibles ni datos personales reales.
- No se hace commit ni push.

## Fuentes revisadas

Fuentes iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `docs/appbuilder/menu-lateral-analysis.md`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

Fuentes AppBuilder revisadas sin abrir configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\router.js`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenuRelated.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\App\bllMenu.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapMenu.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\Repositorios\RepositorioVistasCliente.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\Repositorios\RepositorioIdentidadCliente.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`
- Componentes genericos de busqueda, CRUD y tabs bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\*`
- Componentes genericos de tabs bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicTab*`

## Evidencia encontrada

### Menu actual iLiniumTech

`iLiniumTech.Frontend/src/layout/appNavigation.ts` ya incluye una entrada:

- label: `Clientes`
- icono: `pi pi-user`
- estado actual iLiniumTech: `fixture`
- ruta: `/clientes`
- permiso: no definido todavia

Esta entrada y la vista fixture son decisiones estaticas de iLiniumTech, no un consumo runtime de metadata. No existe aun contrato funcional con datos reales ni API propia.

### Menu AppBuilder

AppBuilder no tiene rutas de negocio fijas para paginas como `Clientes` en `router.js`; define rutas base como `/mipanel`, `/login` y un `notfound`. El menu de aplicacion se carga dinamicamente con GraphQL:

- Query: `Menu_GET_BY_APPLICATION_ID`
- Argumentos: `administrator`, `profileId`, `applicationId`, `version`
- Campos relevantes: `id`, `parentId`, `order`, `title`, `idIcon`, `active`, `componentId`, `urlComponentStatic`, `urlRouteComponentStatic`, `keepAlive`, localizaciones.

`HelperMenu.buildMenu` transforma la lista plana de `IapMenu` en arbol de navegacion por `parentId`, ordena por `order`, usa `active` como visibilidad, `title` como etiqueta, `idIcon` como icono y `urlComponentStatic`/`urlRouteComponentStatic` como destino.

`bllMenu.getByApplication` filtra menus no administradores por grupos del perfil y directivas de objeto tipo menu. La condicion efectiva encontrada es que exista directiva con `View == true` para el grupo del usuario. Esto confirma permiso historico de visibilidad, pero no da la matriz funcional de Clientes.

### Etiqueta de menu

`Messages.ts` contiene localizacion generica:

- `gestion.clientes` = `Clientes`
- `gestion.clientes` en ingles = `Clients`

Esto confirma la existencia de la etiqueta funcional, pero no identifica el `IapMenu.Id`, `ComponentId`, ruta o jerarquia real del menu en BBDD.

### Datos y vistas de cliente

Se detectan constantes y mapeos EF para entidades/vistas relacionadas con clientes:

- Tablas principales: `Identidad`, `IdentidadCliente`, `IdentidadClienteAcuerdo`.
- Vistas de contexto o detalle: `vw_ClienteInfo`, `vw_ClienteDato`, `vw_Cliente_DatosAdicionales`, `vw_ClienteAcuerdo`, `vw_Cliente_ColaboradorLider`.
- Vistas funcionales asociadas a un cliente: `vw_ClientePolizas`, `vw_ClienteRecibos`, `vw_ClienteRiesgos`, `vw_ClienteSiniestros`, `vw_ClienteSuplementos`.

Estas vistas son evidencia local fuerte de areas internas candidatas para la futura ficha de cliente, pero no prueban por si solas que la pantalla AppBuilder original las renderizara exactamente como pestanas.

### Componentes genericos

AppBuilder contiene infraestructura generica para:

- busqueda/listado CRUD;
- detalle en modo tab;
- abrir/cerrar pestanas;
- formularios dinamicos con `DynamicTabView`, `DynamicTabPanel`, `DynamicDataTable`, `DynamicCrudTabla`.

No se ha encontrado en esta ronda una definicion local especifica de componente `Clientes` que enumere los controles, tabs, acciones y datasources de la pantalla real.

## Evidencia ausente

No se ha localizado de forma segura en el repo local:

- fila concreta de `IapMenu` para `Clientes`;
- `ComponentId` raiz de la pantalla `Clientes`;
- `parentId`, orden e icono reales de la entrada en la BBDD AppBuilder;
- metadata serializada de los hijos/tabs reales de la pagina;
- `QueryStatic`, datasource o configuracion de columnas especifica de `Clientes`;
- permisos historicos por grupo para `Clientes`;
- reglas reales de broker, oficina, gestor, comercial o perfil aplicadas a clientes;
- UAT funcional que confirme columnas, filtros y acciones.

Por tanto, el desarrollo queda bloqueado para implementacion completa hasta ejecutar una extraccion offline sanitizada o recibir especificacion funcional validada.

## Coordinacion de subagentes de componentes

Como jefe de pagina, separo los componentes documentales en dos niveles:

- Evidencia directa de infraestructura: `busqueda-listado`, porque AppBuilder usa componentes genericos de busqueda/listado y tabs.
- Evidencia directa de datos/vistas: `resumen-identidad`, `polizas`, `recibos`, `riesgos`, `siniestros`, `suplementos`.

Documentos generados:

- `components/busqueda-listado.md`
- `components/resumen-identidad.md`
- `components/polizas.md`
- `components/recibos.md`
- `components/riesgos.md`
- `components/siniestros.md`
- `components/suplementos.md`

Los componentes funcionales se documentan como candidatos, no como contrato cerrado, porque falta la metadata local de pantalla que confirme la composicion visual original.

## Propuesta de pagina Vue/API estatica

### Ruta y feature

Ruta candidata:

- `/clientes`

Feature candidata:

- `iLiniumTech.Frontend/src/features/clientes`

Componentes Vue candidatos:

- `ClientesView.vue`: contenedor de listado read-only.
- `ClientesFilters.vue`: filtros de busqueda.
- `ClientesTable.vue`: tabla de resultados.
- `ClienteDetailView.vue`: ficha de cliente.
- `ClienteResumenPanel.vue`: identidad/resumen minimizado.
- `ClientePolizasTab.vue`
- `ClienteRecibosTab.vue`
- `ClienteRiesgosTab.vue`
- `ClienteSiniestrosTab.vue`
- `ClienteSuplementosTab.vue`

No crear un renderer dinamico de tabs desde metadata. Las pestanas se codifican de forma explicita cuando producto confirme que forman parte del alcance.

### API candidata

Endpoints read-only candidatos:

- `GET /api/clientes/catalogs`
- `GET /api/clientes`
- `GET /api/clientes/{clienteId}`
- `GET /api/clientes/{clienteId}/polizas`
- `GET /api/clientes/{clienteId}/recibos`
- `GET /api/clientes/{clienteId}/riesgos`
- `GET /api/clientes/{clienteId}/siniestros`
- `GET /api/clientes/{clienteId}/suplementos`

Reglas:

- Validar sesion, broker activo y permisos antes de resolver conexion o leer datos.
- No aceptar `clienteId`, sort, columnas o filtros como SQL estructural libre.
- Usar whitelists y parametros.
- No devolver documento legal completo, telefonos, emails, direcciones ni datos bancarios por defecto.
- No revelar existencia de un cliente cuando el broker o permisos no autorizan el acceso.

### Permisos iLiniumTech candidatos

Permisos read-only minimos:

- `clientes.read`
- `clientes.detail`
- `clientes.polizas.read`
- `clientes.recibos.read`
- `clientes.riesgos.read`
- `clientes.siniestros.read`
- `clientes.suplementos.read`

Permisos futuros, no MVP sin SDD propia:

- `clientes.create`
- `clientes.update`
- `clientes.delete`
- `clientes.export`
- `clientes.import`

La evidencia AppBuilder de `ObjectGroup` permite acciones genericas `add`, `edit`, `delete`, `view`, `detail/list`, `import`, `export`, `execute`, pero no confirma que todas apliquen a Clientes.

## Datos y filtros candidatos

Filtros candidatos de listado:

- texto libre por nombre/razon social, con normalizacion y longitud maxima;
- documento, solo si producto y seguridad aprueban mascara o busqueda exacta controlada;
- gestor/comercial, si existe catalogo autorizado;
- canal de cobro, si se confirma utilidad;
- situacion/actividad, si se confirma campo funcional;
- paginacion y ordenacion por whitelist.

Columnas candidatas de listado:

- identificador interno no legal;
- nombre comercial minimizado;
- gestor/comercial;
- canal de cobro;
- resumen de polizas/recibos si sale de vista autorizada;
- estado funcional si existe regla confirmada.

Campos sensibles detectados o previsibles:

- `NumDocumento`;
- `NombreCompleto`, `Nombre`, `Apellido1`, `Apellido2`, `RazonSocial`;
- fechas personales como nacimiento o caducidad de documento;
- email, telefono, direccion, datos bancarios si aparecen en vistas o joins futuros;
- anotaciones libres.

## Acciones candidatas

Acciones read-only candidatas:

- buscar;
- limpiar filtros;
- ordenar;
- paginar;
- abrir ficha;
- volver conservando filtros;
- cambiar pestana de ficha;
- copiar identificador interno, solo si no es dato personal;
- navegar a poliza/recibo/siniestro relacionado si el modulo existe y el permiso lo permite.

Acciones bloqueadas hasta SDD:

- alta de cliente;
- edicion de identidad;
- borrado/anulacion;
- importacion/exportacion;
- ejecucion de workflows;
- apertura de documentos o comunicaciones;
- acciones sobre datos bancarios o medios de cobro.

## Riesgos

- PII alta: cliente concentra identidad, documento, datos de contacto, direcciones y posible informacion financiera.
- Multi-tenant: cualquier bug de broker puede exponer datos de clientes de otra entidad.
- Ambiguedad de dominio: `Identidad` es base comun y `IdentidadCliente` especializa cliente; no debe mezclarse con companias, colaboradores u otras identidades.
- Falsa paridad: las vistas `vw_Cliente*` sugieren areas, pero no sustituyen la metadata real de pantalla.
- AppBuilder runtime: replicar `DynamicTab*`, `DynamicCrudTabla` o `QueryStatic` como motor generico violaria la decision arquitectonica.
- Permisos: `View` historico de menu no equivale a permiso de detalle, PII, exportacion o acciones.
- Logs: busquedas por documento/nombre no deben quedar registradas en claro.

## Pruebas obligatorias para desarrollo futuro

Backend:

- filtros parametrizados y sort por whitelist;
- paginacion estable;
- 401 anonimo;
- 403 sin permiso;
- 403/404 generico para broker no autorizado;
- no exposicion de `NumDocumento` completo por defecto;
- no exposicion de email/telefono/direccion sin permiso explicito;
- tests de payloads maliciosos en filtros/sort.

Frontend:

- guard de ruta por `/api/me`;
- estado sin sesion;
- estado sin broker;
- estado sin permiso;
- loading, empty, error;
- filtros sincronizados con URL si se adopta el patron de Polizas;
- vuelta desde detalle conservando busqueda;
- tabs visibles solo si el backend/permiso lo permite;
- no renderizar metadata AppBuilder ni nombres `IAP_*` en DOM.

QA/seguridad:

- `Test-DocumentationBaseline.ps1`;
- `Invoke-SecretScan.ps1`;
- `Invoke-DependencyAudit.ps1 -FailOnFindings` si toca dependencias;
- `Invoke-CorsAudit.ps1 -FailOnFindings` si toca API/configuracion;
- smoke visual cuando exista pantalla;
- UAT con datos sanitizados o entorno autorizado.

## Bloqueos externos

- Hace falta extraer metadata sanitizada de `Clientes` o recibir especificacion funcional equivalente.
- Hace falta confirmar con producto si la primera entrega sera solo listado, listado + ficha, o ficha con tabs.
- Hace falta matriz de permisos real por broker, perfil, oficina, gestor y usuario.
- Hace falta decision de PII: documento, contacto, direccion, datos bancarios y anotaciones.
- Hace falta validar con DBA vistas autorizadas, claves de `SESSION_CONTEXT` y reglas de aislamiento.

## Siguiente paso recomendado

Crear una SDD `Clientes read-only MVP` antes de programar. Esa SDD debe decidir si se empieza por:

1. listado read-only minimizado;
2. listado + ficha resumen;
3. listado + ficha resumen + tabs relacionadas.

La opcion 1 es la mas segura si no hay UAT ni matriz PII cerrada. La opcion 3 solo debe ejecutarse despues de validar la metadata o una especificacion funcional firmada.
