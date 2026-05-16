# Agente componente - Polizas busqueda y filtros

Fecha: 2026-05-16

Estado: documentacion completada con evidencia local. No es contrato runtime.

## Alcance del subagente

Este subagente analiza la zona de busqueda y filtros de la pagina `Polizas`. La salida sirve para desarrollo futuro de Vue/API estatica, SDD y pruebas. No autoriza consumir metadata AppBuilder en runtime.

Archivos permitidos en esta ronda:

- `docs/appbuilder/pages/polizas/components/busqueda-filtros.md`

Archivos no tocados:

- frontend;
- backend;
- extractor;
- metadata no sanitizada;
- configuraciones con secretos.

## Fuentes revisadas

iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/polizas-filtros-busqueda-analysis.md`
- `docs/appbuilder/polizas-listado-analysis.md`
- `reports/polizas-metadata/polizas.metadata.sanitized.json`
- `iLiniumTech.Frontend/src/features/polizas/PolizasView.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasFilters.vue`
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`
- `iLiniumTech.Frontend/src/features/polizas/polizasTypes.ts`

AppBuilder, busqueda local sin abrir `appsettings`, dumps ni connection strings:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchTree.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchFields.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\LookUpEditor.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchHelper.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchFormHelper.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Search_SEARCH.graphql`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\Search\bllSearch.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\App\ServicioSearch.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Helpers\Security\HelperDataSourceField.cs`

## Evidencia AppBuilder

La pantalla historica `Polizas` no aparece como pagina Vue especifica. Se monta desde el CRUD generico:

- menu `10`, titulo `Polizas`;
- componente raiz `2824`, nombre `Busqueda Polizas`, categoria `compcat-seacrh`;
- hijo `2825`, nombre `CrudPoliza`, tipo `tipocontrol-crudtbl`;
- component datasource `354`, nombre `Dat_PantallaPolizas`;
- datasource `146`, nombre/model object `Pantalla_Polizas`;
- `QueryStatic` presente y redaccionado en metadata sanitizada. No debe ejecutarse en iLiniumTech.

`CrudTable.vue` monta `Search.vue` y `SearchDetail.vue`, y `Search.vue` emite:

- `buscarDatos`;
- `clear:filterdata`;
- cambio de modo simple/avanzado/general;
- actualizacion de filtros, sort y columnas seleccionadas.

`Search.vue` ofrece modos:

- `Avanzada`;
- `Simple`;
- `General` solo si existe un componente de busqueda dedicado (`searchComponentId > 0`).

El modelo original de criterios es `GroupSearch`:

- `operatorLogic`, normalmente `AND` u `OR`;
- `fields`;
- `children`;
- criterios `SearchData` con campo, operador, valores simples, listas o rangos.

`ServicioSearch.SearchFromDataBase(...)` toma `QueryStatic` de componente o datasource y sustituye placeholders como `where`, `orderby`, `groupby`, `having` y `maxRowsReturned`. Este patron explica el origen historico, pero queda prohibido como runtime de iLiniumTech.

## Filtros detectados en metadata sanitizada

| Campo heredado | Etiqueta | Tipo | Control observado | Operador | Catalogo | Riesgo |
| --- | --- | --- | --- | --- | --- | --- |
| `Poliza` | Poliza | string | text | contains | | Identificador de negocio |
| `Aplicacion` | Aplicacion | string | text | contains | | Identificador de negocio |
| `IdTipoPoliza` | Tipo poliza | int | redaccionado | equals | `TipoPoliza` | Catalogo a revisar |
| `NumDocumento` | Documento | string | text | contains | | PII alta |
| `IdSituacion` | Situacion | int | redaccionado | equals | `SituacionPoliza` | Catalogo a revisar |
| `IdRamo` | Ramo | int | redaccionado | equals | `Ramo` | Catalogo a revisar |
| `Riesgo` | Riesgo | string | text | contains | | Puede incluir matricula/riesgo |
| `F_Efecto` | Fecha efecto | date | date | between | | Fecha de negocio |
| `F_Vencimiento` | Fecha vencimiento | date | date | between | | Fecha de negocio |
| `F_Anulacion` | Fecha anulacion | date | date | between | | Fecha de negocio |
| `IdMotivoAnulacion` | Motivo anulacion | int | redaccionado | equals | `MotivoAnulacion` | Catalogo a revisar |
| `Cia` | Compania | string | redaccionado | equals | `Compania` | Catalogo a revisar |
| `NombreCompleto` | Tomador | string | text | contains | | PII alta |

Campos visibles no buscables detectados:

- `PAnualCartera`;
- `AlertaInformativa`;
- `AlertaExclamativa`;
- `AlertaRstrictiva`.

Nota: los controles de catalogo aparecen redaccionados porque la metadata original podia incluir fragmentos SQL o configuracion sensible. La aplicacion nueva debe convertirlos en catalogos API revisados.

## Estado iLiniumTech actual

`PolizasFilters.vue` ya es codigo Vue estatico. Tiene estas secciones:

- `Datos de la poliza`;
- `Datos de gestion`;
- `Datos del tomador`.

Campos actualmente soportados por contrato:

- `poliza` -> query `numero`;
- `tipoPoliza` -> query `estado` en el mapeo actual, pendiente de renombrado funcional;
- `cia` -> query `compania`;
- `ramo` -> query `ramo`;
- `efectoInicial` -> query `fechaEfectoDesde`;
- `efectoFinal` -> query `fechaEfectoHasta`;
- `nombreCompleto`, `documento`, `nombre` -> query `cliente`.

`PolizasView.vue` sincroniza query params:

- `numero`;
- `cliente`;
- `estado`;
- `compania`;
- `ramo`;
- `fechaEfectoDesde`;
- `fechaEfectoHasta`;
- `page`;
- `pageSize`.

El componente bloquea busqueda si el contexto de sesion/broker/permisos no permite consultar.

## Propuesta Vue estatica

Mantener un formulario estatico, versionado y testeable:

- controles tipados por campo;
- catalogos de producto desde `/api/polizas/catalogs`;
- filtros primarios visibles por defecto;
- filtros secundarios disponibles solo si el backend los soporta;
- estados de carga/error/bloqueo por broker y permisos;
- query params propios para soporte y retorno desde detalle.

No replicar:

- `GroupSearch` como contrato publico;
- arbol de grupos AppBuilder;
- filtros por `fieldId`;
- operadores `filterbusqueda-*`;
- busquedas guardadas sin SDD;
- lookups con SQL o metadata;
- configuracion dinamica de columnas/filtros por usuario sin modelo iLiniumTech.

## Propuesta API estatica

`GET /api/polizas/catalogs`:

- requiere `polizas.catalogs`;
- devuelve catalogos revisados, no `IAP_*`;
- debe incluir solo valores autorizados para el broker/perfil si aplica.

`GET /api/polizas`:

- requiere `polizas.read`;
- parametros permitidos por whitelist:
  - `numero`;
  - `cliente` o filtros PII equivalentes solo con decision de seguridad;
  - `estado`;
  - `compania`;
  - `ramo`;
  - `fechaEfectoDesde`;
  - `fechaEfectoHasta`;
  - `page`;
  - `pageSize`;
  - `sort`;
- no acepta nombres SQL, `fieldId`, `GroupSearch`, `QueryStatic` ni operadores heredados.

Rangos de fecha:

- `fechaEfectoHasta` debe tratarse como limite superior exclusivo del dia siguiente en SQL para columnas `datetime`.

## Permisos candidatos

- `polizas.catalogs`: carga de catalogos de filtro.
- `polizas.read`: busqueda/listado.
- `polizas.detail`: no aplica al filtro, pero afecta enlaces posteriores.
- `polizas.search.pii`: candidato futuro si se permite busqueda por documento/tomador sensible.

Los permisos AppBuilder `View`, `List`, `Execute` solo sirven como evidencia historica. No son autoridad runtime.

## Riesgos

PII:

- `NumDocumento` y `NombreCompleto` no deben exponerse completos por defecto.
- `Riesgo` puede contener matricula u otros identificadores.
- filtros por documento o nombre pueden facilitar enumeracion si no hay rate limiting, permisos y auditoria.

SQL:

- la metadata tiene `QueryStatic`; no ejecutar ni reconstruir.
- lookups redaccionados pueden ocultar SQL heredado.
- sort/filtros deben ser whitelist propia.

Multi-tenant:

- antes de cargar catalogos o listado, el backend debe validar broker activo contra `allowedBrokerIds`.
- si hay `SESSION_CONTEXT`, se debe poblar por request y limpiar/sobrescribir por conexion.

UX:

- mantener demasiados campos heredados puede producir una pantalla pesada sin contrato backend real.
- campos deshabilitados deben explicar internamente su bloqueo en documentacion, no prometer funcionalidad.

## Pruebas obligatorias futuras

Frontend:

- inicializa filtros desde query params;
- limpiar filtros vacia query params y vuelve a pagina 1;
- busqueda actualiza URL;
- campos no soportados quedan deshabilitados;
- sin broker/permisos no llama al backend;
- no aparece `IAP_`, `QueryStatic`, `Pantalla_Polizas` ni SQL en DOM.

Backend:

- catalogos requieren `polizas.catalogs`;
- listado requiere `polizas.read`;
- sort/filtro malicioso se rechaza;
- fechas invalidas se rechazan o normalizan de forma explicita;
- broker cruzado devuelve 403 sanitizado antes de SQL;
- errores publicos no incluyen SQL, tablas, connection strings ni datos personales.

QA/UAT:

- validar campos primarios con producto;
- confirmar si `Aplicacion` y `Poliza` se fusionan como `numero` o se separan;
- confirmar si documento/tomador se permiten, con que permisos y mascara;
- validar catalogos reales en entorno autorizado sin versionar muestras.

## Bloqueos

- Falta matriz funcional de permisos real.
- Falta confirmacion DBA/UAT de catalogos y filtros definitivos.
- Falta decision sobre PII en busqueda por documento y tomador.
- Falta auth productiva.

## Estado final del subagente

Completado con evidencia para documentacion. Pendiente de desarrollo solo despues de revision de producto/arquitectura.
