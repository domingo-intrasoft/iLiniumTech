# AppBuilder Polizas - detalle y navegacion desde listado

Fecha: 2026-05-15

## Alcance y regla de uso

Este documento analiza como AppBuilder resuelve el detalle de `Polizas` desde el listado y que partes conviene replicar en iLiniumTech como Vue/TypeScript estatico y API explicita.

No propone consumir metadata `IAP_*` en runtime. Las referencias AppBuilder se usan solo como evidencia, trazabilidad, SDD y scaffolding revisado.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`.
- `README.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md`.
- `docs/MVP_POLIZAS_PLAN.md`.
- `docs/MVP_POLIZAS_DIFERENCIAS.md`.
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`.
- `reports/polizas-metadata/polizas.metadata.sanitized.json`.
- `tools/extractor/polizas-metadata/fixtures/polizas-metadata.fixture.json`.
- `iLiniumTech.Frontend/src/features/polizas/PolizasView.vue`.
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`.
- `iLiniumTech.Frontend/src/features/polizas/PolizaDetailView.vue`.
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`.
- `iLiniumTech.Frontend/src/router/index.ts`.

Repositorio AppBuilder:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\DetailCrud.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\saveSearchNavigationHelper.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppBreadcrumb.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppBreadcrumbOnly.vue`.

## Trazabilidad de Polizas

Metadata canonica localizada para el listado:

- Aplicacion: `2`, `Aunna Tech`, version `1`.
- Menu: `10`, titulo `Polizas`, componente `2824`.
- Componente raiz: `2824`, `Busqueda Polizas`, categoria `compcat-seacrh`.
- Componente CRUD: `2825`, `CrudPoliza`, tipo `tipocontrol-crudtbl`, hijo de `2824`.
- ComponentDataSource: `354`, `Dat_PantallaPolizas`, enlaza componente `2825` con datasource `146`.
- DataSource: `146`, `Pantalla_Polizas`, BBDD logica `tipobd-MO`, objeto modelo `Pantalla_Polizas`.

Campos del listado y base del detalle ya detectados en el artefacto sanitizado:

- `Poliza`, `Aplicacion`, `IdTipoPoliza`, `NumDocumento`, `IdSituacion`, `IdRamo`, `Riesgo`.
- `F_Efecto`, `F_Vencimiento`, `F_Anulacion`, `IdMotivoAnulacion`.
- `Cia`, `PAnualCartera`, `NombreCompleto`.
- `AlertaInformativa`, `AlertaExclamativa`, `AlertaRstrictiva`.

El artefacto tambien indica `QueryStatic` presente en `componentDataSource` y `dataSource`, pero redaccionado. Debe permanecer como evidencia: iLiniumTech no debe ejecutarlo ni derivar SQL estructural desde ese texto.

## Comportamiento original en AppBuilder

### Navegacion general

AppBuilder no tiene una pantalla de Polizas codificada como pagina unica. La ruta de menu carga un componente raiz y el runtime generico decide que renderizar.

Flujo observado:

1. El menu `Polizas` apunta al componente raiz `2824`.
2. El render dinamico monta `CrudTable.vue` para el hijo `2825`.
3. `CrudTable.vue` compone busqueda, resultado y detalle segun props y layout metadata.
4. `Search.vue` gestiona botones de busqueda, limpieza, cierre de pestanas, busquedas guardadas y modo simple/avanzado.
5. `SearchDetail.vue` pinta la tabla PrimeVue y las acciones por fila.
6. El click de detalle emite claves de fila, no navega directamente a una URL de negocio.

### Apertura de detalle desde listado

`SearchDetail.vue` muestra detalle de dos formas:

- Icono de ojo `pi pi-eye` cuando existe accion `btndetail` y el usuario tiene permiso `View`.
- Celda clicable cuando `showActionDetail` esta activo y `actionDetailDataSourceFieldId` coincide con la columna configurada.

Metodos relevantes:

- `verDetalle(data)`: emite `click:viewDetail` con `keys: buildDataKeys(data)` e `item: data`.
- `buildDataKeys(data)`: recorre tablas/campos de la datasource y extrae claves primarias por tabla.
- `buildMenuCustom(menuItems, data)`: inyecta acciones dinamicas `btndetail` y `btndelete` si los permisos lo permiten.
- `actionDetailCommandInFilterRequest(columnFilteredData)`: decide si una columna concreta debe actuar como acceso directo al detalle.
- `hasOneMenu()` y `hasMenuActions()`: alternan entre icono directo y menu de acciones por fila.

El evento sube a `CrudTable.vue`:

- `catchEvent('click:viewDetail', event)` guarda `keyData` y `keyDataObject`.
- Si hay layout de detalle (`LAYOUT_DETAIL`) y no hay workflow de detalle, prepara callback para abrir pestana con `addCustomTab(-1, [])`.
- `addCustomTab(componentId, attrs)` abre una pestana asociada a las claves de la fila o activa una existente si ya estaba abierta.
- `buildHeader()` puede construir el titulo de la pestana sustituyendo marcadores `#campo#` desde la fila seleccionada.

### Detalle y pestanas

`DetailCrud.vue` es el contenedor visual del detalle:

- En modo no-tab (`tabMode=false`) muestra boton de volver al listado con icono `pi pi-arrow-left`.
- En modo tab (`tabMode=true`) muestra refrescar pestana y cerrar pestana.
- Si recibe `componentId`, monta otro `FormBuilder`.
- Si recibe `detailComponentId`, monta un componente de detalle especifico.
- Si no hay componente dinamico, usa slots `header`, `buttons` y `detail` inyectados por `CrudTable.vue`.

Esto significa que el detalle original puede venir de:

- un layout de detalle asociado al CRUD;
- un componente hijo dinamico;
- una accion/menu/workflow que cambia `detailComponentId`;
- una pestana reutilizada por claves de fila.

No se encontro en fuente un componente estatico especifico `PolizaDetail.vue` heredado. La evidencia apunta a detalle gobernado por metadata y CRUD generico.

### Breadcrumbs

Hay dos variantes:

- `AppBreadcrumb.vue`: usa `route.meta.breadcrumb` si existe; si no, deriva etiquetas desde `route.fullPath`, filtrando segmentos numericos.
- `AppBreadcrumbOnly.vue`: usa `route.meta.breadcrumb` y traducciones de menu; tambien mira menus activos por `route.path`.

Para Polizas, el breadcrumb visible debe entenderse como shell de navegacion AppBuilder (`Inicio / Polizas /`) mas que como contrato funcional de detalle. En iLiniumTech conviene codificarlo como texto/ruta propio.

### Busquedas guardadas y redireccion

`saveSearchNavigationHelper.ts` puede construir `redirectParams` con:

- `path` de menu.
- `menuId`.
- `componentId` raiz.
- lista `comps` con raiz y componente actual.

Luego `redirectToPath()` hace `router.push({ path })` y dispara interacciones para rehidratar componentes. Esto es un mecanismo AppBuilder de navegacion entre pantallas dinamicas y busquedas guardadas. iLiniumTech no debe replicarlo como motor generico; si se implementan busquedas guardadas, deben ser casos de uso propios y tipados.

## Dependencia de datos y permisos

El listado y el detalle original dependen de:

- DataSource `146`, `Pantalla_Polizas`.
- ComponentDataSource `354`.
- Campos configurados en `IAP_ComponentDataSourceFieldConfiguration`.
- Lookups/catalogos para `IdTipoPoliza`, `IdSituacion`, `IdRamo`, `IdMotivoAnulacion` y `Cia`.
- Permisos `IAP_ObjectGroup` para `View`, `List`, `Add`, `Edit`, `Delete`, `Import`, `Export`, `Execute`.
- Contexto de broker/perfil y posible `SESSION_CONTEXT` SQL.

El detalle no debe asumir que todos los campos visibles en el listado son seguros para exponer completos. `NumDocumento`, telefono, email, direccion, matricula o bastidor deben quedar minimizados o enmascarados hasta tener auth real y decision funcional.

## Decision de replica estatica para iLiniumTech

Replica recomendada:

- Mantener `/polizas` como pantalla Vue estatica.
- Mantener `/polizas/:id` como ruta explicita de detalle, no como pestana generica por metadata.
- Usar `RouterLink` desde el campo `numero` y, si se necesita fidelidad AppBuilder, anadir una columna de acciones con icono de ojo que apunte a la misma ruta.
- Mantener boton `Polizas` o `Volver al listado` en detalle como `RouterLink` a `{ name: 'polizas' }`.
- Codificar breadcrumbs: `Inicio / Polizas / <numero o Detalle>`.
- Codificar secciones de detalle en `polizasConstants.ts`, revisadas por producto.
- Consumir `GET /api/polizas/{id}` para datos de detalle y nunca metadata de pantalla.

No replicar:

- `FormBuilder` para detalle.
- `redirectParams` generico de busquedas guardadas.
- `LAYOUT_DETAIL` desde metadata.
- Acciones de fila dinamicas desde `IAP_ComponentEvent`, workflows o object groups.
- `QueryStatic` ni fragmentos SQL heredados.

## Recomendaciones para la pantalla Vue actual

Estado actual observado:

- `PolizasTable.vue` ya navega al detalle con `RouterLink` sobre la columna `numero`.
- `router/index.ts` define `/polizas/:id` con nombre `poliza-detail`.
- `PolizaDetailView.vue` carga `getPolizaById(id)`, muestra estados `loading`, `error`, detalle read-only y origen de datos.
- `polizasConstants.ts` ya contiene secciones `Datos de poliza`, `Gestion` y `Tomador`.

Recomendaciones concretas:

1. Anadir una accion visual de detalle en tabla con icono de ojo, manteniendo `numero` como enlace. Esto replica mejor AppBuilder sin introducir metadata runtime.
2. Conservar paginacion/filtros al volver desde detalle. Puede hacerse con query params propios (`page`, filtros simples) o estado local controlado; no usar `redirectParams` AppBuilder.
3. Anadir breadcrumb estatico en `PolizaDetailView.vue`: `Inicio / Polizas / <numero>`. Si el numero aun no cargo, usar `Detalle`.
4. Mantener el boton de volver como ruta a `polizas`, pero valorar `router.back()` solo si no rompe entrada directa por URL.
5. Revisar `polizaDetailSections` contra UAT: el detalle actual incluye campos de tomador potencialmente sensibles (`documento`, `email`, `telefono`). Hasta auth real, deben mostrarse vacios, minimizados o condicionados por permiso.
6. Separar acciones futuras del detalle en SDDs propias: anulacion, duplicado, suspension, revigorizacion, reemplazo, documentos, recibos, siniestros o wallet.
7. Para las alertas (`AlertaInformativa`, `AlertaExclamativa`, `AlertaRstrictiva`), decidir si deben aparecer como chips/avisos en listado y cabecera de detalle. Requieren contrato API explicito y textos sanitizados.
8. Confirmar si `Riesgo` en detalle debe representar matricula/resumen de riesgo. No exponer matricula completa, bastidor ni datos de vehiculo sin politica de minimizacion.

## Riesgos y pendientes

- La metadata disponible en `reports/polizas-metadata` procede de fixture/sanitizacion; puede no contener toda la configuracion real de detalle, layouts, workflows o botones de fila.
- El detalle real en AppBuilder puede depender de layouts o workflows no incluidos en el corte actual.
- `Pantalla_Polizas` puede requerir `SESSION_CONTEXT` y permisos por broker, perfil, oficina o gestor.
- Los lookups necesitan validacion contra BBDD autorizada para evitar catalogos incompletos o mal etiquetados.
- Reproducir tabs dinamicos de AppBuilder en Vue podria reintroducir complejidad innecesaria; para iLiniumTech se recomienda ruta explicita y estado propio.
- Las acciones heredadas pueden tener efectos de escritura o integraciones externas. No deben activarse sin SDD, permisos, auditoria y UAT.
- El detalle ampliado puede exponer PII. Cualquier campo de documento legal, contacto, direccion, matricula completa o identificadores de riesgo debe tratarse como sensible.

## Criterio de cierre para una futura implementacion

- La navegacion listado -> detalle esta cubierta por prueba unitaria de `PolizasTable.vue`.
- La entrada directa `/polizas/:id` carga detalle o error sanitizado.
- El breadcrumb y el boton volver funcionan sin depender de historial previo.
- El DOM no expone `IAP_*`, `QueryStatic`, `Pantalla_Polizas`, SQL ni connection strings.
- Los campos sensibles aparecen minimizados o bloqueados segun la decision de auth/permisos vigente.
- Las acciones no-read-only siguen deshabilitadas o fuera del DOM hasta SDD propia.
