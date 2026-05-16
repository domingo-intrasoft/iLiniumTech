# Componente Clientes: busqueda y listado

Fecha: 2026-05-16

Estado: `bloqueado externo para desarrollo final`.

## Rol del componente

Componente raiz candidato para la pagina `/clientes`: permite buscar clientes, ver resultados paginados y abrir la ficha de detalle.

## Evidencia AppBuilder

Fuentes:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SaveSearchTabPanels.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`

Evidencia:

- AppBuilder tiene componentes genericos de busqueda, detalle y tabla CRUD.
- `CrudTable.vue` y `Search.vue` soportan tabs de resultados/detalle y eventos de abrir/cerrar.
- `NombreTablasConst.ts` expone `IDENTIDAD`, `IDENTIDADCLIENTE`, `VW_CLIENTEINFO` y varias vistas `VW_CLIENTE*`.
- `ModeloDbContext.cs` mapea `Identidad` con campos de identidad y `IdentidadCliente` con especializacion de cliente.

Ausencia:

- No se localiza metadata de columnas, filtros o acciones especificas de `Clientes`.
- No se localiza `ComponentId` del listado de Clientes.
- No se localiza datasource o `QueryStatic` de Clientes sanitizado.

## Datos candidatos

Fuente candidata principal:

- `Identidad` + `IdentidadCliente`, o vista autorizada equivalente si DBA la define.

Campos detectados relevantes:

- `Identidad.NombreCompleto`
- `Identidad.Nombre`
- `Identidad.Apellido1`
- `Identidad.Apellido2`
- `Identidad.RazonSocial`
- `Identidad.NumDocumento`
- `Identidad.IdTipoDocumento`
- `Identidad.IdActividad`
- `Identidad.IdIdioma`
- `IdentidadCliente.ClienteId`
- `IdentidadCliente.IdCanalCobro`
- `IdentidadCliente.IdGestor`
- `IdentidadCliente.ComercialId`

Campos sensibles:

- `NumDocumento`
- nombre completo y razon social;
- fechas personales;
- cualquier contacto/direccion que aparezca por joins futuros.

## Filtros candidatos

- texto por nombre/razon social;
- documento solo con politica PII aprobada;
- gestor/comercial;
- canal de cobro;
- actividad o situacion si producto confirma el valor funcional;
- paginacion;
- sort por whitelist.

## Acciones candidatas

Read-only:

- buscar;
- limpiar;
- ordenar;
- paginar;
- abrir detalle;
- conservar filtros en URL.

Bloqueadas:

- alta;
- edicion;
- borrado;
- importacion;
- exportacion;
- ejecucion de acciones/workflows.

## Permisos candidatos

- `clientes.read`
- `clientes.detail`

Los permisos historicos AppBuilder se filtran por `ObjectGroup` y `View == true` para menus. No hay evidencia local suficiente para permisos de acciones de Clientes.

## Propuesta Vue/API estatica

Vue:

- `ClientesView.vue`
- `ClientesFilters.vue`
- `ClientesTable.vue`
- composable `useClientes.ts`
- servicio `clientesApi.ts`
- tipos `clientesTypes.ts`

API:

- `GET /api/clientes/catalogs`
- `GET /api/clientes?page=&pageSize=&texto=&gestor=&canalCobro=&sort=`

La API debe devolver identificador interno no legal y campos minimizados.

## Pruebas obligatorias

Backend:

- filtros parametrizados;
- sort solo por whitelist;
- paginacion estable;
- 401, 403 y broker cruzado;
- no devuelve documento completo por defecto.

Frontend:

- formulario emite criterios normalizados;
- tabla renderiza loading/empty/error;
- boton limpiar resetea filtros;
- detalle conserva query de vuelta;
- sin permiso no consulta API.

## Riesgos

- PII en busqueda por documento o nombre.
- Falsa equivalencia entre identidad y cliente.
- Reintroducir CRUD generico AppBuilder.
- Falta de UAT para columnas prioritarias.

## Bloqueo

No desarrollar hasta disponer de SDD de Clientes y confirmacion de columnas/filtros por producto.
