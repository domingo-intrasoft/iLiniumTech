# Analisis AppBuilder - Polizas listado/grid

Fecha: 2026-05-15

## Alcance y regla de arquitectura

Este analisis documenta el comportamiento observado del listado/grid de Polizas en AppBuilder para usarlo como evidencia funcional, trazabilidad y entrada de SDD/scaffolding revisado. iLiniumTech no debe usar metadata AppBuilder en runtime para pintar columnas, construir queries, decidir permisos ni ejecutar acciones.

Ruta iLiniumTech objetivo:

- Frontend Vue/TypeScript estatico en `iLiniumTech.Frontend/src/features/polizas`.
- Backend API explicita en `iLiniumTech.Backend`, con repositorio SQL parametrizado y whitelist propia.
- Metadata AppBuilder solo en documentacion, reportes sanitizados o extractor offline.

## Fuentes revisadas

Repositorio iLiniumTech:

- `README.md`
- `AGENTS.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md`
- `docs/MVP_POLIZAS_PLAN.md`
- `docs/MVP_POLIZAS_DIFERENCIAS.md`
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`
- `reports/polizas-metadata/polizas.metadata.sanitized.json`

Repositorio AppBuilder:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\componentes\base\common\tabla\TableExport.vue`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\Search\bllSearch.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\App\ServicioSearch.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\AppBuilder\Repositorios\RepositorioSearch.cs`

No se han copiado secretos, connection strings, dumps ni datos personales reales.

## Trazabilidad AppBuilder de la pantalla

La pantalla de Polizas esta definida como metadata y se ejecuta mediante el CRUD generico de AppBuilder:

- Aplicacion: `2`, version `1`.
- Menu: `10`, titulo `Polizas`.
- Componente raiz: `2824`, `Busqueda Polizas`.
- Componente CRUD: `2825`, `CrudPoliza`.
- ComponentDataSource: `354`, enlace `Dat_PantallaPolizas`.
- DataSource: `146`, `Pantalla_Polizas`.
- Objeto modelo: `Pantalla_Polizas`.
- Tipo de base objetivo: `tipobd-MO`, resuelto por broker/entidad activa.

No se encontro una pantalla estatica `Pantalla_Polizas` en el codigo AppBuilder; el comportamiento nace de `IAP_Component*`, `IAP_DataSource*`, configuracion de campos, permisos y `QueryStatic`.

## Columnas observadas

El reporte sanitizado del extractor contiene 17 columnas visibles para el listado, ordenadas por configuracion:

| Orden | Campo AppBuilder | Etiqueta | Tipo observado | Buscable | Ordenable | Nota |
| --- | --- | --- | --- | --- | --- | --- |
| 10 | `Poliza` | Poliza | string | si | si | Candidato a numero/id visible. |
| 20 | `Aplicacion` | Aplicacion | string | si | si | Texto. |
| 30 | `IdTipoPoliza` | Tipo poliza | int | si | si | Lookup/catalogo, revisar origen backend. |
| 40 | `NumDocumento` | Documento | string | si | si | Dato sensible; no exponer completo por defecto. |
| 50 | `IdSituacion` | Situacion | int | si | si | Lookup/catalogo. |
| 60 | `IdRamo` | Ramo | int | si | si | Lookup/catalogo. |
| 70 | `Riesgo` | Riesgo | string | si | si | Texto. |
| 80 | `F_Efecto` | Fecha efecto | date | si | si | Rango de fechas. |
| 90 | `F_Vencimiento` | Fecha vencimiento | date | si | si | Rango de fechas. |
| 100 | `F_Anulacion` | Fecha anulacion | date | si | si | Rango de fechas. |
| 110 | `IdMotivoAnulacion` | Motivo anulacion | int | si | si | Lookup/catalogo. |
| 120 | `Cia` | Compania | string | si | si | Lookup/catalogo. |
| 130 | `PAnualCartera` | Prima anual cartera | decimal | no | si | Formato importe. |
| 140 | `NombreCompleto` | Tomador | string | si | si | Dato personal; minimizar/mascarar segun permisos. |
| 150 | `AlertaInformativa` | Alerta informativa | bool | no | no | Estado visual/indicador. |
| 160 | `AlertaExclamativa` | Alerta exclamativa | bool | no | no | Estado visual/indicador. |
| 170 | `AlertaRstrictiva` | Alerta restrictiva | bool | no | no | Nombre heredado con typo; no propagarlo como contrato publico. |

Lookups/catalogos candidatos:

- `IdTipoPoliza` -> `TipoPoliza`
- `IdSituacion` -> `SituacionPoliza`
- `IdRamo` -> `Ramo`
- `IdMotivoAnulacion` -> `MotivoAnulacion`
- `Cia` -> `Compania`

En el reporte sanitizado, varios controles select aparecen redaccionados porque la configuracion de lookup contenia fragmentos SQL. Para iLiniumTech deben convertirse en catalogos backend explicitos, revisados y parametrizados.

## Filtros y busqueda

Comportamiento AppBuilder:

- `Search.vue` muestra botones `Buscar`, `Limpiar Filtros`, `Cerrar Pestanas`, `Guardar busqueda` y selector `Avanzada`/`Simple`; puede incluir `General` si hay componente de formulario de busqueda.
- La busqueda simple inicializa campos marcados como `defaultFilterSearch` y respeta `defaultFilterSearchOrder`.
- La busqueda avanzada usa `SearchTree.vue` con grupos, operadores y campos definidos por metadata.
- `SearchDetail.vue` anade busqueda global sobre columnas no catalogo y no fecha.
- Los filtros de columna de PrimeVue pueden disparar nuevas consultas en modo lazy.
- Las busquedas recientes, listas, columnas seleccionadas y sort pueden guardarse como configuracion de usuario.

Filtros detectados en Polizas:

- Texto con operador `contains`: `Poliza`, `Aplicacion`, `NumDocumento`, `Riesgo`, `NombreCompleto`.
- Select/lookup con operador `equals`: `IdTipoPoliza`, `IdSituacion`, `IdRamo`, `IdMotivoAnulacion`, `Cia`.
- Fecha con operador `between`: `F_Efecto`, `F_Vencimiento`, `F_Anulacion`.

Recomendacion iLiniumTech:

- Mantener filtros como DTOs tipados del producto, no como arbol generico AppBuilder.
- Exponer solo filtros aprobados por SDD: por ejemplo `numero`, `tipoPoliza`, `situacion`, `ramo`, `compania`, `fechaEfectoDesde/Hasta`, y decidir si `documento`/`tomador` son permitidos con auth real.
- Aplicar rangos de fecha con limite superior exclusivo para cubrir columnas `datetime`.
- Si se implementa busqueda global, convertirla en parametro explicito con whitelist de campos y limite de longitud.

## Paginacion y ordenacion

Comportamiento AppBuilder observado:

- `SearchDetail.vue` usa PrimeVue `DataTable` en modo lazy por defecto.
- Paginador: `FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown`.
- Texto de paginador: `Mostrando de {first} a {last} de {totalRecords} registros`.
- Opciones de pagina por defecto en `CrudTable.vue`: `[5, 10, 25, 50, 100]`.
- `rows` por defecto: `10`; estado interno inicial observado en `CrudTable.vue`: `25`, pero la prop publicada por el componente es `10`.
- Sorting multiple: `sortMode="multiple"` y `multiSortMeta`.
- `removableSort` permite quitar ordenacion desde la UI.
- En lazy, pagina/sort/filtros emiten `click:refreshData` con `provideData` para `ITEMS`, `COUNT` y, si aplica, agregados.
- Backend AppBuilder aplica `OFFSET ... ROWS FETCH NEXT ... ROWS ONLY` y calcula count mediante envoltorio `cteResults`.

Recomendacion iLiniumTech:

- Mantener `page`, `pageSize`, `sort` y `total` como contrato REST simple.
- Soportar inicialmente una ordenacion principal por whitelist; multi-sort solo si UAT lo requiere y con contrato propio.
- Incluir opciones UI `[10, 25, 50, 100]` o `[5, 10, 25, 50, 100]` tras confirmar UAT.
- No aceptar nombres de columna del frontend; aceptar claves de producto como `numero`, `fechaEfecto`, `fechaVencimiento`, `primaAnual`.
- Devolver errores de validacion sanitizados cuando `sort` no este en whitelist.

## Seleccion y acciones de fila

Comportamiento AppBuilder:

- La tabla puede mostrar columna de seleccion multiple si `canSelectMultiple` o `canUpdateMassive` con permiso `EDIT`.
- El modo `select all` en lazy solicita todos los registros de la busqueda antes de poblar `selectedItems`.
- La columna `acciones` puede incluir:
  - menu contextual por fila (`pi-ellipsis-v`);
  - ver detalle (`pi-eye`);
  - eliminar (`pi-times`) si existe menu/permisos;
  - acciones de cabecera como refrescar, nuevo, importar, actualizacion masiva, limpiar filtros y exportar.
- La visibilidad depende de permisos `ObjectGroup` y configuracion de menus/workflows.

Acciones especificas conocidas por documentacion/captura del MVP visual:

- `Polizas de flota`
- `Polizas colectivas`
- `Polizas Externas`
- Ver detalle de poliza

Fuera de alcance actual iLiniumTech:

- Alta, edicion, eliminacion, importacion, actualizacion masiva.
- Anulacion, duplicado, reemplazo, suspension, revigorizacion u otros workflows.
- Ejecucion generica de acciones AppBuilder.

Recomendacion iLiniumTech:

- Implementar solo acciones explicitas por SDD, permiso y endpoint propio.
- Para el listado read-only, mantener `Ver detalle` como accion principal.
- Tratar `Polizas de flota`, `Polizas colectivas` y `Polizas Externas` como filtros/vistas o rutas de producto solo tras confirmacion funcional.
- No mostrar botones heredados que no tengan backend, permisos y UAT propios.

## Exportaciones

Comportamiento AppBuilder:

- `TableExport.vue` ofrece menu de exportacion con PDF, CSV, Excel, copiar e imprimir; Word aparece condicionado a `rfpId`.
- El CSV usa el exportador del `DataTable` y separador `;`.
- Excel se genera con `xlsx`, ajusta anchos y conserva numericos cuando puede.
- PDF se genera con `jspdf-autotable`.
- Copiar usa portapapeles con tabuladores.
- Imprimir abre una ventana con tabla HTML.
- En modo lazy, antes de exportar se fuerza `selectAll=true` y se refrescan datos para traer el conjunto exportable.

Riesgo: exportar todo el resultado puede extraer datos personales, documentos y volumenes altos fuera del control visual.

Recomendacion iLiniumTech:

- No activar exportacion por herencia AppBuilder.
- Si producto pide exportar, crear endpoint/API explicito con permiso `polizas.export`, limites de filas, auditoria, minimizacion PII y formato definido.
- Exportar solo columnas permitidas del contrato iLiniumTech, no todas las visibles en AppBuilder.
- Bloquear `NumDocumento` y `NombreCompleto` completos salvo decision de seguridad/autorizacion.

## Flujo de datos original

Flujo simplificado de AppBuilder:

1. Frontend carga componente CRUD por metadata.
2. `CrudTable.vue` compone `Search.vue` y `SearchDetail.vue`.
3. El usuario ejecuta busqueda, cambia pagina, ordena o filtra columna.
4. Frontend construye `GroupSearch`, `LazyParams`, filtros PrimeVue y `multiSortMeta`.
5. Servicio GraphQL generico llama a `SearchQuery.Search`.
6. `bllSearch.searchData(...)` valida permisos `View`.
7. `ServicioSearch.Search(...)` carga aplicacion, componente, datasource, campos, configuraciones y lookups.
8. Para datasource DB, `SearchFromDataBase(...)` toma `QueryStatic` de `IapComponentDataSource` o `IapDataSourceDataBase`.
9. Sustituye placeholders como `{where}`, `{orderby}`, `{groupby}`, `{having}`, `{maxRowsReturned}`.
10. `SetConnection(...)` resuelve la conexion de modelo `tipobd-MO`.
11. `RepositorioSearch.StaticSearchFromBDQuery(...)` aplica `SESSION_CONTEXT`, count y paginacion, y ejecuta con Dapper.
12. La respuesta vuelve como items dinamicos, total y datos auxiliares lazy.

Flujo objetivo iLiniumTech:

1. Vue estatico envia DTO tipado a `/api/polizas`.
2. Backend valida API/auth/contexto, filtros, pagina, pageSize y sort.
3. Repositorio iLiniumTech resuelve broker autorizado, aplica `SESSION_CONTEXT` parametrizado si procede.
4. Query SQL propia usa vista/tabla autorizada, columnas whitelist, parametros y paginacion.
5. API devuelve DTOs sanitizados, sin SQL, sin metadata y sin secretos.

## Riesgos de metadata runtime

- `QueryStatic` permite SQL estructural dinamico y sustitucion de placeholders; usarlo en runtime reintroduce la superficie que iLiniumTech quiere retirar.
- Las columnas, filtros, menus y acciones pueden cambiar por metadata sin revision de codigo ni pruebas.
- Los lookups pueden contener SQL o dependencias de perfil no evidentes.
- Los permisos AppBuilder (`ObjectGroup`) mezclan visibilidad UI y autorizacion; no sustituyen permisos backend de producto.
- La seleccion masiva y exportacion global pueden exfiltrar datos sensibles.
- `NumDocumento`, `NombreCompleto`, riesgo, prima y fechas son datos sensibles o de negocio; no deben aparecer por defecto en dumps, logs, fixtures o exportaciones.
- `SESSION_CONTEXT` y broker activo son criticos para evitar mezcla de datos entre entidades.
- El typo heredado `AlertaRstrictiva` no debe convertirse en nombre publico estable del nuevo producto.

## Recomendaciones concretas para iLiniumTech Vue/API

Frontend Vue:

- Definir columnas en codigo TypeScript, con labels de producto y no con nombres `IAP_*`.
- Separar columnas visibles MVP de campos sensibles disponibles solo con permiso.
- Representar alertas como indicadores visuales derivados de campos backend normalizados, por ejemplo `alertas.informativa`, `alertas.exclamativa`, `alertas.restrictiva`.
- Mantener estados `loading`, `empty`, `error`, `configuracion incompleta` y `sin permiso`.
- Incluir selector de pageSize y contador total si UAT confirma paridad con AppBuilder.
- No implementar selector de columnas guardable hasta tener modelo de preferencias iLiniumTech.
- No exponer `Pantalla_Polizas`, `QueryStatic`, ids de metadata ni nombres SQL en DOM.

Backend API:

- Ampliar `GET /api/polizas/catalogs` solo con catalogos revisados: tipo poliza, situacion, ramo, motivo anulacion y compania.
- Mantener whitelist de sort/filtros en codigo iLiniumTech.
- Modelar filtros sensibles (`documento`, `tomador`) tras auth real y decision de minimizacion.
- Mantener `NumDocumento` fuera del listado por defecto; si se necesita, devolver mascara no reversible o ultimos caracteres bajo permiso.
- Tratar `NombreCompleto` como dato personal; confirmar si listado lo muestra completo, resumido o no lo muestra.
- Implementar exportacion solo con SDD propia, permiso, limites y auditoria.
- Tests obligatorios: sort malicioso, filtros maliciosos, pageSize maximo, broker cruzado, falta de permiso, errores sin SQL ni connection strings.

Documentacion/SDD:

- Si se incorporan nuevas columnas o acciones, abrir SDD o actualizar la existente con impacto de seguridad y UAT.
- Mantener este documento como evidencia; no generar endpoint de metadata para consumirlo.

## Pendientes UAT

- Confirmar columnas visibles reales por perfil/broker en el listado de Polizas.
- Confirmar si `NumDocumento` y `NombreCompleto` deben verse en listado, y con que mascara/permisos.
- Confirmar labels finales: `Compania`, `Tomador`, `Prima anual cartera`, `Fecha anulacion`.
- Confirmar significado visual de `AlertaInformativa`, `AlertaExclamativa` y `AlertaRstrictiva`.
- Confirmar catalogos reales para tipo poliza, situacion, ramo, motivo anulacion y compania contra entorno autorizado.
- Confirmar orden inicial por defecto y si AppBuilder aplica sort inicial en alguna columna.
- Confirmar pageSize inicial y opciones de pageSize esperadas.
- Confirmar si multi-sort es necesario o basta sort simple.
- Confirmar si seleccion multiple tiene caso de uso en Polizas read-only.
- Confirmar si exportacion es requerida, formatos permitidos, limite de filas y permisos.
- Confirmar acciones `Polizas de flota`, `Polizas colectivas` y `Polizas Externas`: si son filtros, rutas, modales o acciones.
- Confirmar dependencia real de `SESSION_CONTEXT` para `Pantalla_Polizas` o vistas equivalentes.
- Ejecutar UAT comparando muestras contra entorno read-only autorizado, sin guardar capturas sensibles ni datos reales en Git.

## Pruebas y verificaciones de este analisis

Ejecutado:

- Lectura de documentos canonicos y SDDs de Polizas.
- Busqueda en `C:\Desarrollo\AppBuilder` de componentes CRUD, grid, exportacion y flujo backend.
- Lectura de `reports/polizas-metadata/polizas.metadata.sanitized.json`.
- `git status --short` antes de editar: sin cambios reportados.

No ejecutado:

- Builds backend/frontend: no aplica, solo se modifica documentacion.
- Extractor en modo `Live`: bloqueado por falta de entorno autorizado y porque no se deben manejar secretos en este documento.
- UAT visual con datos reales: pendiente de responsable funcional/DBA.

Riesgo residual:

- La metadata real puede variar por perfil, broker, permisos o entorno. Este documento debe validarse con UAT antes de cerrar paridad funcional del listado.
