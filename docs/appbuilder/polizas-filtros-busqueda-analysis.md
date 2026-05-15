# Analisis AppBuilder - Polizas filtros y busqueda

Fecha: 2026-05-15

## Alcance

Este documento analiza como AppBuilder construye filtros, busqueda, criterios, catalogos y acciones relacionadas con la pantalla historica de Polizas, con foco en su traduccion segura a iLiniumTech.

iLiniumTech no debe usar metadata AppBuilder en runtime. La metadata y el codigo AppBuilder revisados aqui sirven como evidencia funcional, trazabilidad y entrada de scaffolding revisado. El resultado esperado para producto sigue siendo frontend Vue/TypeScript estatico y backend API explicita con whitelist, parametros y permisos propios.

## Fuentes revisadas

Documentacion y specs iLiniumTech:

- `AGENTS.md`.
- `README.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md`.
- `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`.
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`.
- `docs/MVP_POLIZAS_PLAN.md`.
- `docs/MVP_POLIZAS_DIFERENCIAS.md`.
- `reports/polizas-metadata/polizas.metadata.sanitized.json`.
- `reports/polizas-metadata/polizas.metadata.dryrun.json`.
- `tools/extractor/polizas-metadata/PolizasMetadataExtractor.psm1`.
- `tools/extractor/polizas-metadata/fixtures/polizas-metadata.fixture.json`.

Codigo AppBuilder revisado:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchTree.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchFields.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\LookUpEditor.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\LookUpSearchFields.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchHelper.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchFormHelper.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\filterControlTypeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\filterMatchModeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\filterCrudModeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchConfigParamsConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\domain\SearchFieldConfiguration.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Search_SEARCH.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Search_SEARCH_MULTIPLE.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Search_SEARCHLOOKUP.graphql`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Schema\Query\Builder\Search\SearchQuery.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\Search\bllSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\App\ServicioSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\AppBuilder\Repositorios\RepositorioSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Helpers\Security\HelperDataSourceField.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\GroupSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\SearchData.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\RangeSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\FiltroBusquedaConst.cs`.

## Trazabilidad Polizas

La pantalla historica queda identificada por la metadata ya extraida:

- Aplicacion `2`, version `1`.
- Menu `10`, titulo `Polizas`.
- Componente raiz `2824`, `Busqueda Polizas`.
- Componente CRUD `2825`, `CrudPoliza`.
- `IAP_ComponentDataSource.Id = 354`.
- `IAP_DataSource.Id = 146`, nombre `Pantalla_Polizas`.
- Objeto modelo `Pantalla_Polizas`.

No se encontro una pantalla estatica llamada `Pantalla_Polizas` en AppBuilder; el comportamiento se obtiene por CRUD generico, datasource y configuracion `IAP_*`.

## Componentes y metodos relevantes

Frontend AppBuilder:

- `CrudTable.vue`: orquesta el CRUD, monta `Search`/`SearchDetail`, prepara `lazyParams`, llama a `IServiceSearch.search(...)`, parsea respuesta y evita que respuestas antiguas sobreescriban resultados recientes mediante `searchRequestId`.
- `Search.vue`: alterna modos `Avanzada`, `Simple` y, si hay `searchComponentId`, `General`; inicializa filtros por defecto con `defaultFilterSearch` y `defaultFilterSearchOrder`; emite `buscarDatos`; limpia filtros y guarda busquedas automaticas si hay valores.
- `SearchTree.vue`: lista campos disponibles para busqueda avanzada a partir de `filterSearch` y `filterSearchOrder`; permite grupos anidados con operador `AND`/`OR`; parsea `searchConfigParams` para cabeceras, tamano, estilos, colapsado, saltos y default filter.
- `SearchFields.vue`: renderiza controles de criterio y usa `SearchHelper` para opciones de operador segun tipo de dato.
- `SearchDetail.vue`: gestiona filtros de columna de tabla, ordenacion, valores por defecto, `idFilterControlType`, `idDefaultMatchMode`, `idFilterType`, datos lazy de columnas y columnas visibles.
- `LookUpEditor.vue`, `LookUpSearchFields.vue`, `LookUpSearchTable.vue`: resuelven busquedas de lookups, filtros iniciales y resultados de seleccion.
- `searchHelper.ts`: filtra operadores por tipo, calcula rangos relativos de fechas, limpia criterios y decide si mostrar controles.
- `searchFormHelper.ts`: traduce formularios dinamicos de busqueda a `SearchData`.

Backend AppBuilder:

- `SearchQuery.cs`: expone GraphQL `Search`, `SearchMultiple`, `SearchLookUpData`, `ExecuteProc` y `downloadFile`.
- `bllSearch.searchData(...)`: lee argumentos GraphQL, comprueba permisos de vista de componente/datasource y delega en `ServicioSearch.Search(...)`.
- `ServicioSearch.Search(...)`: elige datasource DB o servicio externo.
- `ServicioSearch.SearchFromDataBase(...)`: toma `QueryStatic` de `IapComponentDataSource` o `IapDataSourceDataBase`, sustituye placeholders como `{where}`, `{orderby}`, `{groupby}`, `{having}`, `{crossapply}` y `{maxRowsReturned}`, fusiona filtros de formulario y filtros lazy, aplica entorno/usuario y ejecuta busqueda.
- `ServicioSearch.GenerateAllRequest(...)`: combina el `GroupSearch` principal con filtros de columnas PrimeVue enviados en `lazyParams`.
- `ServicioSearch.resolveWhere(...)`: une where inicial de datasource/componente, where dinamico por `GroupSearch` y busqueda global.
- `ServicioSearch.resolveOrderBy(...)`: prioriza sort dinamico lazy y cae a order inicial.
- `RepositorioSearch.SearchFromBDQuery(...)`: concatena SQL estructural desde piezas ya resueltas, aplica `OFFSET/FETCH`, count y lazy data.
- `HelperDataSourceField.BuildGroupSearchWhere(...)`: convierte arbol `GroupSearch` en SQL preservando parentesis y operadores.
- `HelperDataSourceField.BuilAllParameters(...)`: crea parametros para valores, excluyendo filtros nulo/no nulo.
- `HelperDataSourceField.BuildFilter(...)` y `BuildValor(...)`: traducen operadores de busqueda a SQL y valores parametrizables.

## Flujo de datos observado

1. AppBuilder carga componente, datasource, campos, configuraciones por campo, catalogos y permisos.
2. El CRUD construye columnas y filtros desde `IAP_ComponentDataSourceFieldConfiguration`.
3. `Search.vue` inicializa un `GroupSearch` con `operatorLogic = AND`, `fields` y `children`.
4. En busqueda simple se agregan campos marcados con `defaultFilterSearch`, ordenados por `defaultFilterSearchOrder`.
5. En busqueda avanzada el usuario agrega campos marcados con `filterSearch`, ordenados por `filterSearchOrder`, y puede crear grupos hijos con operadores logicos.
6. Los filtros de columna de tabla via lazy mode se serializan como metadata PrimeVue y se convierten en otro `GroupSearch`.
7. El frontend llama GraphQL `Search_SEARCH` con `applicationId`, `applicationVersion`, `componentId`, `componentDataSource`, `dataSourceId`, `data`, `parameters`, `maxregs`, `count`, `lazyParams` y `lang`.
8. El backend valida permisos de vista, localiza datasource y genera SQL desde `QueryStatic` y placeholders.
9. Los valores de filtro se parametrizan, pero la estructura SQL procede de metadata y fragmentos heredados.
10. El repositorio ejecuta la consulta y devuelve `items`, `totalRecords` y datos lazy opcionales para agregados o opciones de filtro de columna.

## Modelo de criterios

`GroupSearch` es un arbol:

- `operatorLogic`: operador del grupo, normalmente `AND` u `OR`.
- `fields`: lista de `SearchData`.
- `children`: grupos hijos.

`SearchData` representa un criterio:

- `fieldId`: identificador del campo AppBuilder.
- `filter`: operador funcional, por ejemplo `filterbusqueda-inc`.
- `value`, `valueList`, `valueBool`, `valueNumber`, `valueDateTime`.
- `rangeValue`, `rangeNumber`, `rangeDateTime`.
- `required`: marca de obligatorio.

Operadores relevantes en AppBuilder:

- Texto: contiene, no contiene, empieza por, termina por, igual, distinto.
- Numericos: igual, distinto, menor que, menor o igual, mayor que, mayor o igual.
- Catalogos/lookups: igual o lista `IN`.
- Fechas: entre, fecha igual, antes, despues y rangos relativos como mes actual, mes anterior, ano actual, semana actual y dias atras.
- Nulos: es nulo, no es nulo.

En iLiniumTech conviene conservar la idea funcional de criterios tipados, pero no el contrato `GroupSearch` completo como API publica. Para `/polizas`, el contrato debe seguir siendo un query object explicito: `numero`, `tipoPoliza`, `compania`, `ramo`, `estado`, `oficina`, `gestor`, fechas, paginacion y sort por whitelist.

## Reglas de UI observadas

- La pantalla AppBuilder ofrece modos de busqueda simple, avanzada y, cuando existe componente de busqueda dedicado, general.
- La busqueda simple se inicializa con filtros por defecto; la avanzada permite agregar/quitar criterios.
- Los campos de busqueda se ordenan por metadata.
- Los campos se agrupan por tabla/alias y los calculados aparecen separados.
- Los controles se eligen por tipo:
  - texto para cadenas;
  - fecha/calendario para `date`/`datetime`;
  - dropdown o multiselect para catalogos/lookups;
  - checkbox/toggle para booleanos;
  - input numerico para enteros/decimales.
- Las opciones de operador se reducen segun tipo de campo; por ejemplo, catalogos evitan operadores textuales salvo nulo/no nulo e igual.
- Los rangos de fecha ajustan el limite superior al final del dia en AppBuilder; en iLiniumTech ya se documenta usar limite superior exclusivo del dia siguiente para columnas `datetime`, que es mas limpio y debe mantenerse.
- Los filtros con valor muestran estado visual de campo filtrado.
- Limpiar filtros conserva o reinicializa filtros por defecto segun modo.
- Las busquedas guardadas, ultimas busquedas, listas y configuracion de columnas existen como funcionalidad AppBuilder, pero no son parte necesaria del MVP read-only de iLiniumTech.

## Polizas: filtros y catalogos detectados

La metadata sanitizada de Polizas identifica estos campos visibles, ordenados y revisables para UI:

| Campo heredado | Etiqueta | Tipo | Buscable | Catalogo |
| --- | --- | --- | --- | --- |
| `Poliza` | Poliza | string | si | |
| `Aplicacion` | Aplicacion | string | si | |
| `IdTipoPoliza` | Tipo poliza | int | si | `TipoPoliza` |
| `NumDocumento` | Documento | string | si | |
| `IdSituacion` | Situacion | int | si | `SituacionPoliza` |
| `IdRamo` | Ramo | int | si | `Ramo` |
| `Riesgo` | Riesgo | string | si | |
| `F_Efecto` | Fecha efecto | date | si | |
| `F_Vencimiento` | Fecha vencimiento | date | si | |
| `F_Anulacion` | Fecha anulacion | date | si | |
| `IdMotivoAnulacion` | Motivo anulacion | int | si | `MotivoAnulacion` |
| `Cia` | Compania | string | si | `Compania` |
| `PAnualCartera` | Prima anual cartera | decimal | no | |
| `NombreCompleto` | Tomador | string | si | |
| `AlertaInformativa` | Alerta informativa | bool | no | |
| `AlertaExclamativa` | Alerta exclamativa | bool | no | |
| `AlertaRstrictiva` | Alerta restrictiva | bool | no | |

Los filtros scaffold sugeridos por la metadata sanitizada son:

- Texto con operador `contains`: `Poliza`, `Aplicacion`, `NumDocumento`, `Riesgo`, `NombreCompleto`.
- Catalogo con operador `equals`: `IdTipoPoliza`, `IdSituacion`, `IdRamo`, `IdMotivoAnulacion`, `Cia`.
- Fecha con operador `between`: `F_Efecto`, `F_Vencimiento`, `F_Anulacion`.

Observaciones de seguridad para Polizas:

- `NumDocumento` aparece como campo buscable heredado, pero ya esta clasificado como sensible. En iLiniumTech no debe mostrarse ni devolverse completo por defecto; cualquier busqueda por documento requiere SDD/autorizacion especifica y minimizacion.
- `NombreCompleto` tambien puede ser dato personal. Si se mantiene como busqueda por tomador, debe estar condicionado a permisos reales y no debe aparecer en fixtures con datos reales.
- Los catalogos extraidos requieren backend catalog review: no deben ser consultados dinamicamente desde `IAP_*` en runtime.
- Los controles con `control` redaccionado en el JSON sanitizado indican que habia material sensible o SQL-like en la metadata original; deben convertirse manualmente a selects estaticos/API explicita.

## Acciones y eventos

Acciones/eventos relevantes de busqueda:

- `buscarDatos`: dispara busqueda y puede colapsar el panel si la pantalla esta configurada asi.
- `clear:filterdata`: limpia filtros y reinicializa defaults.
- Guardado automatico de busqueda cuando hay criterios.
- Cambio de modo de busqueda: simple, avanzada, general.
- Seleccion de campo en arbol avanzado.
- Agregar/eliminar grupos hijos.
- Filtros de columna lazy y sort de tabla.
- Solicitud de datos lazy para agregados, count, items y opciones de filtro de columna.
- Lookups: abrir buscador, ejecutar busqueda y seleccionar registro.

Acciones no incluidas en el MVP de iLiniumTech:

- Busquedas guardadas por usuario/perfil.
- Ultimas busquedas.
- Listas persistidas.
- Configuracion dinamica de columnas por usuario.
- Ejecucion de procedimientos, descarga generica, acciones CRUD o workflows.
- Acciones de anulacion, duplicado, reemplazo, suspension, revigorizacion, Google Wallet o integraciones REST/SOAP heredadas.

## Que replicar como codigo estatico iLiniumTech

- Mantener `/polizas` como pantalla Vue/TypeScript propia, no generada en runtime.
- Mantener filtros principales de negocio como controles explicitos:
  - numero de poliza/aplicacion;
  - tipo de poliza;
  - situacion/estado;
  - ramo;
  - compania;
  - fechas de efecto y vencimiento;
  - busqueda por tomador solo si producto y seguridad lo aprueban;
  - anulacion/motivo como filtros secundarios si UAT los necesita.
- Mantener catalogos mediante `GET /api/polizas/catalogs`, con DTOs propios y valores autorizados.
- Mantener paginacion, limite de `pageSize` y sort por whitelist backend.
- Mantener rango de fechas con valores parametrizados y limite superior exclusivo para `fechaHasta`.
- Mantener estados de UI: configuracion incompleta, loading, empty, error sanitizado y contexto sin broker.
- Mantener separacion entre filtro visible de UI y contrato backend; no exponer nombres heredados como contrato si se puede usar nombres de producto.
- Usar la metadata solo para justificar labels, columnas candidatas, catalogos y pruebas de UAT.
- Cubrir pruebas de filtros por texto, catalogo, fechas, limpieza de filtros y sort rechazado.

## Que no replicar

- No implementar un render dinamico de `IAP_Component`, `IAP_ComponentDataSource` o `IAP_ComponentDataSourceFieldConfiguration`.
- No exponer endpoint productivo que sirva metadata AppBuilder para construir `/polizas`.
- No ejecutar `QueryStatic` heredado ni sustituir placeholders `{where}`, `{orderby}`, `{crossapply}` en runtime de producto.
- No aceptar `fieldId`, `filterbusqueda-*`, `GroupSearch` generico o nombres de columnas libres desde frontend como contrato de API.
- No construir SQL estructural desde strings de metadata, labels, aliases o configuracion de usuario.
- No portar busquedas guardadas/listas/columnas de usuario sin SDD propia y modelo iLiniumTech.
- No portar acciones genericas `ExecuteProc`, `SearchMultiple`, REST/SOAP, workflows ni descarga generica.
- No usar permisos AppBuilder como motor de autorizacion runtime; pueden servir como evidencia para mapear permisos iLiniumTech.
- No mostrar `NumDocumento` completo, documento legal, datos personales reales ni trazas internas.

## Recomendaciones para evolucionar `/polizas`

1. Consolidar un mapa explicito de filtros de producto:
   - `numero` para `Poliza`/`Aplicacion` si funcionalmente ambas representan busqueda por identificador;
   - `tipoPoliza`, `estado`, `ramo`, `compania`;
   - `fechaEfectoDesde/Hasta`, `fechaVencimientoDesde/Hasta`;
   - `tomador` y `documento` solo con decision de seguridad.

2. Ampliar `GET /api/polizas/catalogs` con catalogos revisados para `TipoPoliza`, `SituacionPoliza`, `Ramo`, `MotivoAnulacion` y `Compania`, sin leer `IAP_Catalog` en runtime productivo.

3. Separar filtros primarios y secundarios en UI:
   - primarios: numero, compania, ramo, estado, fecha efecto;
   - secundarios: tipo, vencimiento, anulacion, motivo, riesgo, tomador.

4. Mantener el backend como fuente de verdad:
   - whitelist de columnas y sort en codigo/config iLiniumTech;
   - parametros para todos los valores;
   - validacion de rangos de fecha;
   - errores publicos sanitizados con `correlationId` cuando aplique.

5. Tratar `NumDocumento` y `NombreCompleto` como PII:
   - no devolver documento completo;
   - documentar si se permite busqueda exacta por documento con permiso especifico;
   - evitar busqueda fuzzy por documento si no hay necesidad funcional aprobada.

6. No implementar busqueda avanzada generica todavia:
   - si producto la pide, crear SDD propia con un DSL reducido, tipado y whitelisteado;
   - no reutilizar `GroupSearch` AppBuilder ni operadores heredados como contrato externo.

7. Usar la metadata sanitizada como fixture de analisis:
   - versionar solo artefactos sanitizados si se decide formalmente;
   - mantener `reports/` fuera del runtime;
   - convertir cada hallazgo util a codigo, test o SDD revisada.

8. Validar con UAT contra entorno autorizado:
   - equivalencia de resultados por numero, compania, ramo y fechas;
   - comportamiento de polizas anuladas;
   - significado de `Aplicacion`, `Riesgo` y alertas;
   - catalogos reales y valores obsoletos/inactivos.

## Riesgos residuales

- La metadata sanitizada no prueba por si sola la semantica completa de `Pantalla_Polizas`; falta UAT contra BBDD autorizada.
- El comportamiento real puede depender de `SESSION_CONTEXT`, perfil, oficina, gestor, broker y usuario.
- Catalogos y lookups pueden tener filtros internos no visibles en el scaffold sanitizado.
- `QueryStatic` puede contener logica funcional no portada; debe revisarse manualmente, no ejecutarse.
- La busqueda global de AppBuilder concatena campos y puede incluir PII; no debe portarse sin diseno especifico.

## Evidencia de este analisis

No se ejecuto build ni tests de backend/frontend porque la tarea fue documental y solo se creo este archivo. No se tocaron frontend, backend, tools ni `.github`.
