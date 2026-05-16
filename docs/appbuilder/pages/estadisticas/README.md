# Pagina AppBuilder: Estadisticas

Fecha: 2026-05-16

Ronda: solo documentacion.

Jefe de pagina: Estadisticas.

Estado final: bloqueado externo para desarrollo funcional.

## Resumen ejecutivo

`Estadisticas` existe en el menu objetivo de iLiniumTech como entrada estatica deshabilitada, sin ruta, sin permiso candidato declarado y sin hijos en `appNavigation.ts`.

En las fuentes locales de AppBuilder se ha encontrado infraestructura generica relacionada con estadisticas, dashboards y graficos:

- directiva historica candidata `stats_acceso`;
- opcion de usuario `dashboard`;
- controles genericos `DynamicChartBar`, `DynamicChartCirc`, `DynamicEmailDashboard` y `DynamicOrganizationChart`;
- modelo de menu AppBuilder con `componentId`, `urlComponentStatic`, `urlRouteComponentStatic`, `parentId`, `order`, `active` y relacion padre/hijos.

No se ha encontrado metadata local suficiente para identificar una pantalla real de `Estadisticas`:

- no hay `componentId` confirmado;
- no hay registro de menu AppBuilder concreto;
- no hay componente raiz;
- no hay tabs, submenus o componentes internos verificables;
- no hay datasource, campos, filtros, acciones, permisos por objeto ni workflows concretos;
- no hay contrato API heredado que pueda migrarse con seguridad.

Por tanto, esta pagina queda documentada como evidencia parcial y bloqueada para programacion. Cualquier desarrollo debe partir de SDD, extraccion sanitizada autorizada, validacion funcional y UAT.

## Alcance de esta ronda

Se ha documentado exclusivamente la evidencia local disponible. No se ha programado frontend, backend ni extractor.

No se han creado documentos en `components/*.md` porque no se detectaron pestanas, submenus o componentes internos reales de `Estadisticas`. Los controles de graficos encontrados son infraestructura generica de AppBuilder, no componentes de una pagina `Estadisticas` verificable.

## Restricciones aplicadas

- No se han leido `appsettings`, `.config`, connection strings, dumps ni capturas.
- No se ha ejecutado SQL.
- No se ha ejecutado extractor `Live`.
- No se han abierto datos personales reales.
- No se ha modificado codigo de aplicacion.
- No se ha creado runtime dinamico basado en metadata AppBuilder.
- No se ha hecho commit ni push.

## Fuentes revisadas

### Gobierno iLiniumTech

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

### Busqueda local en iLiniumTech

Busqueda de terminos:

- `Estadisticas`
- `Estadisticas` con variantes acentuadas
- `Statistics`
- `Stats`
- `Dashboard`
- `KPI`
- `Indicador`
- `Grafico` y variantes acentuadas

Resultado relevante:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts` contiene la entrada estatica `Estadisticas`, deshabilitada.
- `docs/appbuilder/pages/README.md` lista `Estadisticas` como pendiente de evidencia metadata local.
- No existe SDD especifica de `Estadisticas`.
- No existe documentacion previa de pagina `Estadisticas`.

### Fuentes AppBuilder revisadas

Se hicieron busquedas con `rg` en `C:\Desarrollo\AppBuilder`, excluyendo rutas y ficheros sensibles o irrelevantes como `appsettings*`, `.config`, dumps, capturas, `bin`, `obj`, `node_modules`, `packages`, `package-lock.json` y `*.tsbuildinfo`.

Fuentes relevantes leidas:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\DirectiveConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\UserOptionTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\UserOptionTypeConst.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ControlTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\ControlTypeConst.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ChartTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ChartBarTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ChartCircTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\ChartLabelDataConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Functions\ComponentRenderHelper.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicChartBar.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicChartCirc.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicEmailDashboard.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicOrganizationChart.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapMenu.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\modelos\menu\CustomMenu.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\AppBuilder\Repositorios\RepositorioMenu.cs`

## Entrada en el menu iLiniumTech

Fuente: `iLiniumTech.Frontend/src/layout/appNavigation.ts`.

Estado actual:

| Campo | Valor observado |
| --- | --- |
| Label | `Estadisticas` |
| Icono | `pi pi-chart-bar` |
| Ruta | No definida |
| Disabled | `true` |
| Permiso requerido | No definido |
| Hijos | No definidos |

Interpretacion:

- iLiniumTech ya reserva una entrada visual para `Estadisticas`.
- La entrada esta aparcada, igual que otros modulos pendientes.
- No hay contrato funcional suficiente para activarla.
- No debe asumirse que la entrada equivale a una pantalla AppBuilder localizada.

## Evidencia AppBuilder encontrada

### Directiva candidata

Fuente: `DirectiveConst.ts`.

Evidencia:

```ts
static ESTADISTICAS = 'stats_acceso'
```

Interpretacion:

- Existe una directiva historica candidata relacionada con acceso a estadisticas.
- No se ha encontrado en fuentes locales una asignacion completa de esta directiva a una pagina concreta, menu, componente raiz o grupo de objetos.
- Puede servir como evidencia para futura SDD, pero no como permiso runtime automatico.

Permiso iLiniumTech candidato:

- `estadisticas.read`

Mapeo propuesto, pendiente de validacion:

| Evidencia heredada | Permiso iLiniumTech candidato | Estado |
| --- | --- | --- |
| `stats_acceso` | `estadisticas.read` | Pendiente de validacion funcional |

### Opcion `dashboard`

Fuentes:

- `UserOptionTypeConst.ts`
- `UserOptionTypeConst.cs`

Evidencia:

- AppBuilder declara `dashboard` como tipo de opcion de usuario.

Interpretacion:

- La opcion indica que AppBuilder puede almacenar preferencias o configuraciones relacionadas con dashboards.
- No prueba que exista una pagina `Estadisticas` concreta ni que su diseno este disponible localmente.
- No debe migrarse como configuracion runtime para construir pantallas en iLiniumTech.

### Controles genericos de graficos

Fuentes:

- `ControlTypeConst.ts`
- `ControlTypeConst.cs`
- `ChartTypeConst.ts`
- `ChartBarTypeConst.ts`
- `ChartCircTypeConst.ts`
- `ChartLabelDataConst.ts`
- `ComponentRenderHelper.ts`
- `DynamicChartBar.vue`
- `DynamicChartCirc.vue`

Evidencia:

| Control AppBuilder | Significado probable | Estado para Estadisticas |
| --- | --- | --- |
| `tipocontrol-chart` | Grafico de barras u otro grafico basado en Chart.js | Generico, no vinculado a pagina real |
| `tipocontrol-chartcirc` | Grafico circular, pie o doughnut | Generico, no vinculado a pagina real |
| `type` | Tipo visual del grafico | Propiedad generica |
| `data` | Datos del grafico | Propiedad generica, origen no identificado |
| `options` | Opciones Chart.js | Propiedad generica |
| `plugins` | Plugins Chart.js | Propiedad generica |
| `width` / `height` | Dimensiones | Propiedad generica |
| `labelData` | Orientacion de etiquetas `col` o `row` | Propiedad generica |

Interpretacion:

- AppBuilder dispone de un motor dinamico para renderizar graficos desde metadata.
- No hay evidencia de que esos controles pertenezcan a `Estadisticas`.
- iLiniumTech no debe replicar `DynamicChartBar` o `DynamicChartCirc` como motor generico. Si se desarrolla Estadisticas, los graficos deben ser componentes Vue estaticos con contratos API explicitos.

### Dashboard de correo

Fuente: `DynamicEmailDashboard.vue` y constantes relacionadas.

Evidencia:

- Existe un control `tipocontrol-dynamicemaildashboard`.
- Tiene layouts relacionados con correo: destinatarios, cuerpo, botones y objetos.

Interpretacion:

- Es infraestructura generica de AppBuilder para una experiencia de correo/dashboard.
- No hay evidencia de que sea parte de `Estadisticas`.
- No debe arrastrarse a iLiniumTech salvo que una SDD futura defina un modulo de correo o comunicaciones.

### Modelo de menu AppBuilder

Fuentes:

- `iapMenu.ts`
- `IapMenu.cs`
- `HelperMenu.ts`
- `CustomMenu.ts`
- `RepositorioMenu.cs`

Campos relevantes:

| Campo | Uso en AppBuilder |
| --- | --- |
| `id` | Identificador del menu |
| `applicationId` | Aplicacion |
| `applicationVersion` | Version de aplicacion |
| `componentId` | Componente dinamico asociado si existe |
| `urlComponentStatic` | URL o ruta estatica usada por el menu |
| `urlRouteComponentStatic` | Ruta de componente estatico cuando no hay `componentId` |
| `title` | Titulo visible |
| `idIcon` | Icono |
| `parentId` | Jerarquia padre/hijo |
| `order` | Orden |
| `active` | Visibilidad/activacion |
| `keepAlive` | Cache de pantalla |

Interpretacion:

- AppBuilder resuelve menus desde BBDD y puede enlazar una entrada a `componentId` dinamico o ruta estatica.
- La busqueda local no proporciona el registro real de `Estadisticas`.
- Sin extractor autorizado o consulta sanitizada no se puede conocer si `Estadisticas` tenia `componentId`, hijos, ruta estatica o permisos propios.

## Evidencia ausente

No se ha encontrado evidencia local verificable de:

- `IapMenu` real cuyo `title` sea `Estadisticas`;
- `componentId` raiz;
- componente raiz AppBuilder;
- componentes hijos;
- tabs;
- submenus;
- paneles de KPI reales;
- graficos concretos;
- datasource o vista SQL asociada;
- `QueryStatic` o consulta heredada especifica;
- campos de filtro;
- columnas o metricas;
- acciones de exportacion, drilldown, navegacion o refresco;
- permisos por `IapObjectGroup`;
- workflows o expresiones asociadas;
- contrato multi-tenant especifico de la pagina;
- responsable UAT o criterios funcionales.

## Raiz, hijos, tabs y submenus

### Raiz

No identificada en AppBuilder.

En iLiniumTech existe solo la entrada de menu aparcada:

- label: `Estadisticas`;
- icono: `pi pi-chart-bar`;
- sin ruta;
- sin hijos;
- deshabilitada.

### Hijos

No identificados.

### Tabs

No identificadas.

Aunque AppBuilder tiene controles genericos de tabs (`tipocontrol-tabv`, `tipocontrol-tabpnl`, `tipocontrol-tab`, `tipocontrol-tablist`, `tipocontrol-tabpnls`), no hay evidencia de que `Estadisticas` los use.

### Submenus

No identificados.

Aunque el modelo `IapMenu` soporta `parentId` e `inverseParent`, no hay registros locales de hijos para `Estadisticas`.

### Componentes internos

No identificados.

No se crean subdocumentos de componentes porque seria inventar estructura.

## Datos, catalogos y filtros

No hay datasource confirmado.

No hay filtros confirmados.

No hay catalogos confirmados.

Hipotesis funcionales que no deben convertirse en codigo sin SDD:

- periodo de analisis;
- broker;
- ramo;
- compania;
- oficina;
- gestor;
- estado;
- comparativa anual;
- KPIs de polizas, recibos, siniestros o suplementos.

Estas hipotesis son razonables para una pantalla de estadisticas, pero no estan respaldadas por metadata local concreta.

## Acciones heredadas

No hay acciones confirmadas.

Acciones candidatas para futura SDD, no implementables todavia:

- refrescar dashboard;
- cambiar periodo;
- exportar;
- navegar a detalle/listado filtrado;
- guardar preferencias;
- cambiar broker;
- drilldown por grafico.

Cada accion requeriria permiso, contrato backend, auditoria y pruebas propias.

## Permisos

Evidencia heredada:

- `stats_acceso` en `DirectiveConst.ts`.

Permisos iLiniumTech candidatos:

| Permiso | Uso propuesto | Estado |
| --- | --- | --- |
| `estadisticas.read` | Ver la pagina y consultar KPIs agregados | Pendiente de SDD/UAT |
| `estadisticas.export` | Exportar datos agregados | No confirmado |
| `estadisticas.drilldown` | Navegar a listados filtrados desde graficos | No confirmado |

Regla de producto:

- La visibilidad en menu debe venir de permisos iLiniumTech, no de metadata AppBuilder.
- El backend debe aplicar autorizacion antes de consultar datos.
- `stats_acceso` solo puede usarse como evidencia historica para mapear permisos, no como motor runtime.

## Propuesta Vue/API estatica futura

Esta propuesta es orientativa. No autoriza desarrollo sin SDD.

### Frontend

Ruta candidata:

- `/estadisticas`

Feature candidata:

- `iLiniumTech.Frontend/src/features/estadisticas`

Componentes estaticos posibles:

- `EstadisticasView.vue`: pagina raiz protegida.
- `EstadisticasFilters.vue`: periodo, broker si aplica, ramo/compania/oficina/gestor si se valida.
- `EstadisticasKpiGrid.vue`: tarjetas KPI agregadas.
- `EstadisticasCharts.vue`: graficos concretos definidos por producto.
- `EstadisticasDrilldownTable.vue`: tabla opcional si se aprueba drilldown.
- `useEstadisticas.ts`: composable de carga, errores, permisos y query params.

Reglas UX:

- No renderizar graficos desde metadata AppBuilder.
- No permitir metricas arbitrarias desde querystring.
- Mostrar estados `loading`, `empty`, `error`, `sin permiso`, `sin broker` y `sin datos`.
- Mantener filtros en URL solo cuando esten definidos y normalizados.
- Evitar mostrar datos personales en tarjetas o tooltips.
- Si hay pocos registros en un grupo, aplicar supresion o agrupacion para evitar inferencia.

### Backend

Endpoints candidatos:

- `GET /api/estadisticas/catalogs`
- `GET /api/estadisticas/resumen`
- `GET /api/estadisticas/series`
- `GET /api/estadisticas/distribuciones`
- `GET /api/estadisticas/drilldown` solo si UAT lo aprueba.

Contratos candidatos:

- `EstadisticasQuery`: periodo, broker activo, filtros whitelisted.
- `EstadisticasResumenResponse`: KPIs agregados.
- `EstadisticasSerieResponse`: series temporales.
- `EstadisticasDistribucionResponse`: agrupaciones por dimension aprobada.

Reglas backend:

- Resolver broker desde sesion/claims validados.
- Rechazar broker ausente o no permitido antes de cualquier consulta.
- Usar SQL parametrizado y whitelists.
- No aceptar nombres de metricas, columnas, tablas o agrupaciones libres desde frontend.
- No exponer consultas heredadas ni nombres de tablas internas.
- Sanitizar errores y devolver `correlationId` cuando aplique.
- Aplicar minimizacion de PII incluso en drilldown.

### Datos

Origen pendiente:

- vistas autorizadas o repositorios explicitos iLiniumTech;
- no metadata `IAP_*`;
- no `QueryStatic` heredado;
- no SQL dinamico estructural.

Metricas candidatas a validar por producto:

- polizas activas;
- nuevas polizas;
- anulaciones;
- recibos pendientes/cobrados;
- primas agregadas;
- siniestros agregados;
- suplementos agregados.

Estas metricas no estan confirmadas para la pagina AppBuilder y no deben programarse todavia.

## Que no debe replicarse

- Un motor generico que lea metadata AppBuilder para decidir graficos.
- Un endpoint `GET /api/metadata/estadisticas`.
- Un dashboard configurable desde BBDD heredada.
- Ejecucion de `QueryStatic`.
- Construccion de SQL con nombres de columnas, tablas o metricas recibidos del frontend.
- Uso de `stats_acceso` como autorizacion productiva directa sin mapeo iLiniumTech.
- Preferencias `dashboard` heredadas como contrato runtime.
- Reutilizacion de `DynamicChartBar`, `DynamicChartCirc` o `DynamicEmailDashboard` como runtime dinamico.

## Riesgos

### Producto

- Inventar una pantalla de estadisticas que no coincida con AppBuilder ni con necesidades reales.
- Mezclar indicadores de polizas, recibos, siniestros y suplementos sin owner funcional.
- Presentar agregados como cierre funcional sin UAT.

### Seguridad y privacidad

- Reidentificacion por agregados con pocos registros.
- Exposicion de primas, documentos, clientes, matriculas u otros datos sensibles en drilldown.
- Filtrado cruzado entre brokers si el contexto no esta validado antes de consultar.
- Logs con filtros o payloads sensibles.

### Datos y SQL

- Consultas pesadas sobre vistas reales.
- Falta de indices o agregaciones precomputadas.
- Uso de SQL heredado no parametrizado.
- Metricas inconsistentes con AppBuilder por reglas no documentadas.

### Frontend

- Dashboard generico que recree AppBuilder.
- Graficos sin estados de error/empty.
- Tooltips o labels con informacion sensible.
- UI dificil de validar si las metricas no estan cerradas.

### Multi-tenant

- Broker activo no validado.
- Cache compartida entre brokers.
- Agregados que mezclan permisos por oficina, gestor o perfil.

## Pruebas necesarias para desarrollo futuro

### Backend

- 401 anonimo.
- 403 sin `estadisticas.read`.
- 403 broker no permitido.
- Broker ausente.
- Filtros de fecha invalidos.
- Whitelist de dimensiones y ordenaciones.
- SQL injection en filtros y dimensiones.
- No fuga de nombres internos ni detalles SQL en errores.
- Supresion de grupos pequenos si producto la exige.
- Tests de cache aislada por broker/usuario/permisos si hay cache.

### Frontend

- Menu visible/oculto o deshabilitado por permiso.
- Ruta protegida.
- Estados `loading`, `empty`, `error`, `no permission`, `no broker`.
- Filtros normalizados en URL si se implementan.
- Graficos renderizados con datos fixture anonimizados.
- No mostrar datos personales en DOM.
- Responsive desktop/mobile.

### E2E/UAT

- Login -> Estadisticas -> cambiar filtro -> refrescar -> logout.
- Acceso sin permiso.
- Cambio de broker validado por backend.
- Comparacion de KPIs contra AppBuilder o fuente autorizada con datos sanitizados.
- Smoke de rendimiento con volumen representativo.

### Seguridad

- Secret scan.
- Dependency audit.
- CORS audit si se toca API/configuracion.
- Revision de PII en payloads y logs.

## Bloqueos externos

- Falta localizar metadata real de `Estadisticas`: `IapMenu`, `componentId`, componente raiz, hijos, datasources y permisos.
- Falta decidir si `Estadisticas` forma parte del MVP o de una fase posterior.
- Falta owner funcional que defina metricas, filtros y criterios UAT.
- Falta matriz real de permisos para estadisticas.
- Falta confirmar origen de datos autorizado y reglas de agregacion.
- Falta validacion DBA sobre rendimiento, vistas, indices y restricciones multi-tenant.
- Falta criterio de privacidad para agregados y drilldown.

## Criterios para desbloquear

Antes de programar, se necesita al menos una de estas dos vias:

1. Extraccion sanitizada autorizada de metadata AppBuilder que identifique menu, componente raiz, hijos, datasources, acciones y permisos.
2. SDD funcional nueva que defina `Estadisticas` como producto iLiniumTech, aunque no se encuentre equivalencia exacta en AppBuilder.

En ambos casos deben quedar definidos:

- objetivo de negocio;
- usuarios y permisos;
- broker/multi-tenant;
- metricas;
- filtros;
- fuentes de datos;
- minimizacion de PII;
- contratos API;
- criterios UAT;
- pruebas obligatorias.

## Decision de subagentes/componentes

No se han creado subagentes de componentes ni documentos en `components/*.md`.

Motivo:

- no se detectaron pestanas reales;
- no se detectaron submenus reales;
- no se detectaron componentes internos reales;
- los controles de graficos encontrados son genericos del motor AppBuilder y no pertenecen de forma verificable a una pagina `Estadisticas`.

Crear documentacion de componentes ahora introduceria estructura inventada y podria empujar al equipo a desarrollar una pantalla falsa.

## Evidencia de comandos

Comandos ejecutados durante la ronda:

```powershell
git status --short --branch
Get-Content -Raw -Path AGENTS.md
Get-Content -Raw -Path PLANS.md
Get-Content -Raw -Path README.md
Get-Content -Raw -Path docs\PLAN_MAESTRO_IA.md
Get-Content -Raw -Path docs\ROADMAP_OBJETIVO_FINAL.md
Get-Content -Raw -Path docs\DECISION_PRODUCTO_ARQUITECTURA.md
Get-Content -Raw -Path docs\appbuilder\pages\README.md
Get-Content -Raw -Path iLiniumTech.Frontend\src\layout\appNavigation.ts
rg -n -i "Estadisticas|Estadisticas|Estadistica|Estadistica|Statistics|Stats|Dashboard|KPI|Indicador|Grafico|Grafico|Graficos|Graficos" C:\Desarrollo\AppBuilder
rg --files C:\Desarrollo\AppBuilder
```

Las busquedas se ejecutaron con exclusiones para no abrir `appsettings*`, `.config`, dumps, capturas, `bin`, `obj`, `node_modules`, `packages`, `package-lock.json` ni `*.tsbuildinfo` cuando procedia.

Validaciones pendientes tras crear este documento:

- `Test-DocumentationBaseline.ps1`
- `Invoke-SecretScan.ps1`
- `git diff --check`

No se ejecutan build ni tests backend/frontend porque la ronda es exclusivamente documental.

## Estado final

Clasificacion:

- Completado con evidencia: entrada `Estadisticas` localizada en menu iLiniumTech; directiva `stats_acceso`; opcion `dashboard`; controles genericos de graficos/dashboard; modelo de menu AppBuilder.
- Pendiente tecnico: localizar metadata real o crear SDD funcional.
- Bloqueado externo: desarrollo de pagina `Estadisticas` hasta tener owner funcional, metadata sanitizada o SDD, permisos, fuentes de datos y UAT.

Confirmacion de arquitectura:

- No se introduce runtime AppBuilder.
- No se consume metadata AppBuilder en frontend/backend.
- La propuesta futura es Vue estatico + API explicita.
