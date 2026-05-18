# Polizas - analisis profundo de contrato de datos AppBuilder

Fecha: 2026-05-18

Estado: analisis operativo para implementar paridad visual/funcional en iLiniumTech. No es contrato runtime ni habilita metadata dinamica.

## Objetivo

Resolver la diferencia detectada en `/polizas`: AppBuilder muestra `N. Documento`, nombre del cliente/tomador, `Ramo` descriptivo y `Riesgo/Matric.`, mientras que iLiniumTech estaba mostrando codigo de cliente y celdas vacias para documento/riesgo.

La regla aprendida aqui debe reutilizarse en el resto de paginas: la metadata AppBuilder sirve para entender origen, joins y semantica, pero iLiniumTech debe exponer DTOs explicitos y Vue estatico.

## Fuentes revisadas

iLiniumTech:

- `reports/polizas-metadata/polizas.metadata.sanitized.json`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/Sql/PolizasSqlQueryBuilder.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Domain/Polizas/PolizaListItem.cs`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`
- `docs/appbuilder/pages/polizas/components/listado-grid.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-014-polizas-appbuilder-visual-parity.md`

AppBuilder:

- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Helpers\Security\HelperDataSourceField.cs`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\AppBuilder\Repositorios\RepositorioSearch.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\dataSourceConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`

## Campos afectados

| Campo AppBuilder | Label visible | Logica heredada | Contrato iLiniumTech |
| --- | --- | --- | --- |
| `NumDocumento` | `N. Documento` / `Documento` | Campo directo, texto buscable. | `documento` en `PolizaListItem` y `PolizaDetail`. |
| `NombreCompleto` / `RazonSocial` | `Cliente` / `Tomador` | Campo de identidad; `vw_ClientePolizas` lo expone como `RazonSocial`. | `clienteNombre`; no usar `ClienteId` como texto visible salvo fallback. |
| `IdRamo` / `Ramo` | `Ramo` | Lookup catalogo `Ramo`; AppBuilder pinta descripcion con sufijo `_colDescLookUp`; `vw_ClientePolizas` ya trae `Ramo` descriptivo. | `ramo`; usar descripcion de vista/catalogo o fallback controlado al id. |
| `Riesgo` | `Riesgo/Matric.` | Campo directo; puede incluir matricula, descripcion de riesgo o texto de la vista. | `riesgo` en listado y detalle. |
| `Cia` / `CiaId` | `Cia.` | Lookup `Compania`; AppBuilder puede pintar logo/descripcion. | Pendiente de mejora de assets; por ahora se usa dato real disponible. |

## Regla de lookup aprendida

AppBuilder no pinta siempre el campo crudo. Cuando un campo tiene lookup:

1. `HelperDataSourceField.BuildLookUp` anade un `LEFT JOIN`.
2. La descripcion del lookup se proyecta con alias `campo_colDescLookUp`.
3. `SearchDetail.vue` usa `getFieldName(col)`.
4. Si `hasLookUpByFieldId(col.field)` es true, la celda usa `col.field + "_colDescLookUp"`.

Consecuencia: si iLiniumTech ve un campo metadata con `lookup.catalog`, no debe mostrar el id por defecto. Debe crear un contrato API explicito con descripcion de producto:

- `IdRamo` heredado -> `ramo` descriptivo.
- `IdSituacion` heredado -> `estado` normalizado en UI.
- `Cia` heredado -> compania/logotipo cuando haya fuente autorizada.

## Origen relacional aprendido

Detectado en modelo AppBuilder:

- `Poliza.ClienteId` apunta a `Identidad.Id`.
- `Identidad.NombreCompleto` es el nombre visible del tomador/cliente.
- `Identidad.NumDocumento` es el documento legal.
- `Poliza.IdRamo` apunta a catalogo `Ramo`; no debe mostrarse como codigo bruto cuando hay descripcion.
- `RiesgoPoliza` y vistas de riesgo alimentan `Riesgo`/matricula en listados historicos.
- `Catalogo` expone `Id`, `TipoId`, `Descripcion`, `Valor`; es la base de la resolucion descriptiva.

Vistas historicas utiles como referencia:

- `vw_ClientePolizas`: expone `PolizaId`, `Poliza`, `RazonSocial`, `NumDocumento`, `IdRamo`, `Ramo`, `Riesgo`, compania y situacion. Es el origen de lectura elegido para el grid enriquecido.
- `vw_Pol_Riesgo`, `vw_Riesgos_Descripcion`, `vw_rpt_ClienteInformePolizas` y `vw_rpt_RiesgoPoliza`: utiles para validar semantica de riesgo, no para copiar SQL sin revisar.

## Decision iLiniumTech

El listado de Polizas pasa a usar contrato explicito:

- `clienteNombre` se proyecta desde `vw_ClientePolizas.RazonSocial`, con fallback a `ClienteId` solo si falta el texto.
- `documento` se proyecta desde `NumDocumento`.
- `riesgo` se proyecta desde `Riesgo`.
- `ramo` se proyecta desde `vw_ClientePolizas.Ramo`, que ya representa la descripcion del lookup, con fallback a `IdRamo`.
- la busqueda de `cliente` usa `RazonSocial`, `NumDocumento` y `ClienteId` para aproximar el comportamiento heredado sin aceptar SQL dinamico.

No se reutiliza metadata ni `QueryStatic` en runtime.

## Riesgos y cautelas

- `NumDocumento`, `NombreCompleto` y `Riesgo/Matric.` pueden contener PII o matriculas. Este incremento responde al MVP local/demo solicitado y debe revisarse antes de produccion con matriz de permisos, UAT y minimizacion.
- No guardar capturas reales ni volcados de filas con estos campos.
- `Autos Particulares` sigue aparcado; si reutiliza `PolizaListItem`, debe sanitizar `documento`, `clienteId` y `riesgo` en su propia respuesta.
- Si una BBDD modelo no tiene `vw_ClientePolizas` o sus columnas `RazonSocial/NumDocumento/Ramo/Riesgo`, bloquear y documentar el origen correcto antes de crear workarounds.

## Patron reutilizable para otras paginas

1. Extraer de metadata sanitizada campos visibles, buscables, ordenables y lookups.
2. Confirmar en AppBuilder si el campo se pinta crudo, formateado o con `_colDescLookUp`.
3. Localizar entidad/vista relacional que aporta el valor descriptivo.
4. Crear DTO de producto en iLiniumTech con nombres de negocio, no nombres heredados.
5. Implementar SQL/API explicitos con parametros y whitelists.
6. Marcar campos sensibles y rutas derivadas que necesiten sanitizacion.
7. Actualizar tests de contrato API y tests de render Vue.
8. Documentar evidencias y diferencias pendientes.
