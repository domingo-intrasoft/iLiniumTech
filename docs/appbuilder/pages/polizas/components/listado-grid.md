# Agente componente - Polizas listado y grid

Fecha: 2026-05-16

Estado: documentacion completada con evidencia local. No es contrato runtime.

## Alcance del subagente

Este subagente analiza el listado/grid de `Polizas`: columnas, paginacion, ordenacion, acciones de fila, exportacion heredada y propuesta de tabla Vue/API estatica.

Archivo permitido:

- `docs/appbuilder/pages/polizas/components/listado-grid.md`

## Fuentes revisadas

iLiniumTech:

- `docs/appbuilder/polizas-listado-analysis.md`
- `docs/appbuilder/menu-polizas-mvp-implementation.md`
- `reports/polizas-metadata/polizas.metadata.sanitized.json`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`
- `iLiniumTech.Frontend/src/features/polizas/polizasTypes.ts`

AppBuilder:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\componentes\base\common\tabla\TableExport.vue`
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\App\ServicioSearch.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\AppBuilder\Repositorios\RepositorioSearch.cs`

## Evidencia AppBuilder

`SearchDetail.vue` usa `DataTable` de PrimeVue:

- paginacion lazy;
- `sortMode="multiple"`;
- `multiSortMeta`;
- columnas redimensionables y reordenables;
- filtros de columna;
- seleccion simple/multiple;
- acciones por fila;
- menu de acciones;
- icono `pi pi-eye` para ver detalle;
- icono `pi pi-times` para eliminar si hay permiso/configuracion;
- opciones de page size mediante `rowsPerPageOptions`.

`CrudTable.vue` compone el grid con busqueda y mantiene control de concurrencia con `searchRequestId`, para que una respuesta antigua no sustituya resultados nuevos.

El backend AppBuilder:

- calcula `COUNT`;
- aplica `OFFSET/FETCH`;
- fusiona filtros de columna y criterios de busqueda;
- resuelve `where` y `orderby` desde metadata;
- ejecuta SQL final con Dapper.

Ese patron es evidencia historica. iLiniumTech no debe usar metadata para columnas, ordenacion o SQL.

## Columnas detectadas en metadata sanitizada

| Orden | Campo heredado | Etiqueta | Tipo | Buscable | Ordenable | Decision propuesta |
| --- | --- | --- | --- | --- | --- | --- |
| 10 | `Poliza` | Poliza | string | si | si | Mostrar como `numero` y enlace al detalle |
| 20 | `Aplicacion` | Aplicacion | string | si | si | Mostrar si UAT confirma valor |
| 30 | `IdTipoPoliza` | Tipo poliza | int | si | si | Convertir a descripcion de catalogo |
| 40 | `NumDocumento` | Documento | string | si | si | No mostrar completo por defecto |
| 50 | `IdSituacion` | Situacion | int | si | si | Mostrar como `estado` |
| 60 | `IdRamo` | Ramo | int | si | si | Mostrar como `ramo` |
| 70 | `Riesgo` | Riesgo | string | si | si | Evaluar PII/matricula |
| 80 | `F_Efecto` | Fecha efecto | date | si | si | Mostrar |
| 90 | `F_Vencimiento` | Fecha vencimiento | date | si | si | Mostrar |
| 100 | `F_Anulacion` | Fecha anulacion | date | si | si | Secundaria |
| 110 | `IdMotivoAnulacion` | Motivo anulacion | int | si | si | Secundaria/catalogo |
| 120 | `Cia` | Compania | string | si | si | Mostrar como descripcion |
| 130 | `PAnualCartera` | Prima anual cartera | decimal | no | si | Mostrar con formato moneda |
| 140 | `NombreCompleto` | Tomador | string | si | si | PII; mostrar segun decision |
| 150 | `AlertaInformativa` | Alerta informativa | bool | no | no | Indicador futuro |
| 160 | `AlertaExclamativa` | Alerta exclamativa | bool | no | no | Indicador futuro |
| 170 | `AlertaRstrictiva` | Alerta restrictiva | bool | no | no | Normalizar nombre, no propagar typo |

## Estado iLiniumTech actual

`PolizasTable.vue` tiene:

- columna de accion con icono de ojo;
- enlace desde `numero`;
- tabla HTML estatica;
- skeleton de carga;
- estado de error con reintento;
- estado vacio;
- paginacion simple anterior/siguiente;
- selector page size `[10, 25, 50]`;
- `detailQuery` para conservar filtros al navegar al detalle.

`polizasTableColumns` actuales:

- `compania`;
- `numero`;
- `aplicacion`;
- `estado`;
- `ramo`;
- `fechaEfecto`;
- `fechaVencimiento`;
- `primaAnual`;
- `clienteNombre`.

Esta tabla ya evita exponer nombres AppBuilder en DOM y evita metadata runtime.

## Propuesta Vue estatica

Mantener el listado como componente de producto:

- columnas definidas en TypeScript;
- labels revisados por producto;
- acciones explicitas por permiso;
- paginacion estable;
- ordenacion simple por whitelist antes de multi-sort;
- indicadores de alertas normalizados como objeto de producto si se incorporan.

Prioridad de columnas MVP:

1. Accion detalle.
2. Compania.
3. Poliza.
4. Aplicacion.
5. Situacion.
6. Ramo.
7. Fecha efecto.
8. Fecha vencimiento.
9. Prima anual.
10. Cliente/tomador solo si seguridad lo permite.

Columnas diferidas:

- documento;
- motivo anulacion;
- fecha anulacion;
- riesgo/matricula;
- alertas;
- columnas de oficina, gestor o division si aparecen en detalle/filtros pero no en metadata de listado sanitizada.

## Propuesta API estatica

`GET /api/polizas` debe devolver:

- `items`;
- `page`;
- `pageSize`;
- `total`.

Cada item debe ser un DTO de producto, por ejemplo:

- `id`;
- `numero`;
- `aplicacion`;
- `estado`;
- `ramo`;
- `clienteId` solo si no es documento legal;
- `clienteNombre` minimizado o condicionado;
- `compania`;
- `fechaEfecto`;
- `fechaVencimiento`;
- `primaAnual`;
- `moneda`.

Sort:

- aceptar solo claves de producto;
- rechazar cualquier columna no whitelist;
- no aceptar nombres SQL ni heredados `IAP_*`;
- multi-sort solo si UAT lo exige.

Paginacion:

- limitar `pageSize`;
- documentar default;
- proteger contra page/pageSize no numericos o demasiado grandes.

## Acciones de fila

Permitido en MVP:

- `Ver detalle` mediante ruta `/polizas/:id`.

No permitido sin SDD:

- eliminar;
- editar;
- alta;
- importacion;
- actualizacion masiva;
- exportacion;
- acciones heredadas por menu/workflow;
- ejecucion de procedimientos.

La existencia de acciones en AppBuilder no implica que deban aparecer en iLiniumTech. Cada accion futura necesita endpoint, permiso, auditoria, pruebas y UAT.

## Exportacion

AppBuilder tiene exportacion PDF, CSV, Excel, copiar e imprimir. Riesgo alto:

- puede extraer PII;
- puede traer todo el resultado en lazy mode;
- puede saltarse minimizacion visual si no se disena bien.

Decision recomendada:

- no activar exportacion por defecto;
- si se pide, crear `polizas.export`, limites, auditoria y DTO de exportacion especifico;
- no exportar `NumDocumento`, contacto, matricula completa ni campos sensibles salvo permiso especifico.

## Permisos candidatos

- `polizas.read`: permite listado.
- `polizas.detail`: controla accion/enlace de detalle.
- `polizas.export`: reservado.
- `polizas.alerts.view`: candidato si las alertas tienen significado sensible.

La UI puede ocultar/deshabilitar acciones, pero la proteccion real es backend.

## Riesgos

PII:

- `NombreCompleto`, documento, riesgo y prima pueden ser sensibles.
- no guardar capturas reales de listado.

SQL:

- no derivar columnas ni sort desde metadata.
- no aceptar sort dinamico libre.

UX:

- columnas heredadas pueden ser demasiadas para pantalla diaria.
- multi-sort, seleccion masiva y reordenacion pueden aumentar complejidad sin UAT.

Multi-tenant:

- listado debe validar broker antes de resolver conexion.
- errores 403/404 no deben revelar existencia de polizas de otro broker.

## Pruebas obligatorias futuras

Frontend:

- render de tabla con datos;
- skeleton y error;
- estado vacio;
- detalle conserva query params;
- page y pageSize emiten cambios;
- link de detalle ausente/deshabilitado si falta `polizas.detail`;
- DOM sin `IAP_`, `QueryStatic`, `Pantalla_Polizas`, SQL ni connection strings.

Backend:

- `polizas.read` obligatorio;
- page/pageSize validados;
- sort malicioso rechazado;
- filtros maliciosos parametrizados;
- broker no autorizado bloqueado antes de datos;
- DTO no devuelve documento completo por defecto.

QA/UAT:

- confirmar columnas visibles por perfil;
- confirmar page size inicial;
- confirmar orden inicial;
- confirmar si cliente/tomador se muestra;
- confirmar comportamiento de anuladas y alertas.

## Bloqueos

- UAT de columnas reales pendiente.
- Decision PII pendiente.
- Auth real y matriz de permisos pendiente.
- Entorno SQL autorizado pendiente para validar equivalencia.

## Estado final del subagente

Completado con evidencia para documentacion. Preparado para una futura tarea de desarrollo de tabla estatica si producto confirma columnas y PII.
