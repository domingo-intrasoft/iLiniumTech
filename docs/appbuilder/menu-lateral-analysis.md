# Analisis menu lateral AppBuilder

Fecha: 2026-05-15

## Fuentes analizadas

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppSidebar.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenu.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppSubMenu.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenuItem.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppLayout.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\almacen\modules\AuthModule.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\persisted-query-manifest.json`
- Referencia secundaria: `C:\Desarrollo\IntraSoft.iLinium.SDD.ProductWizard\PortalV2\src\apps\general\portal\src\templates\apollo\layout\*`
- Referencia secundaria: `C:\Desarrollo\IntraSoft.iLinium\Resources\scripts\v25.00.11\menu tarificador.sql`

## Comportamiento detectado

- AppBuilder obtiene el menu con la query GraphQL `Menu_GET_BY_APPLICATION_ID`, filtrada por administrador, perfil, aplicacion y version.
- La respuesta contiene `id`, `parentId`, `order`, `title`, `idIcon`, `active`, `componentId`, `urlComponentStatic`, `urlRouteComponentStatic`, `keepAlive` y localizaciones.
- `HelperMenu.buildMenu` convierte esa lista plana en arbol, ordena por `order`, marca `visible` desde `active`, usa `title` como etiqueta e `idIcon` como icono PrimeIcons.
- `AppMenu` lee `store.getters.getApplicationMenus`; `AppSubMenu` y `AppMenuItem` renderizan recursivamente el arbol.
- La plantilla Apollo soporta modos `static`, `icon`, `slim`, `slim-plus`, `horizontal`, `overlay`, `reveal` y `drawer`. En modo icono/slim se priorizan iconos, tooltips y submenus desplegables.
- `AppSidebar` muestra logo de broker desde estado de sesion y pliega/despliega por hover si no esta anclado.
- Los hijos del menu pueden navegar por `router-link` cuando tienen `to`; los padres con hijos abren/cierra submenu. Las rutas activas se detectan por `route.path`.

## Decisiones de replica en iLiniumTech

- El menu lateral de iLiniumTech queda declarado como codigo fuente en `src/layout/appNavigation.ts`.
- La navegacion visible mantiene la estructura funcional observada: modulos principales compactos, iconos PrimeIcons, Polizas como area activa y Autos Particulares como pantalla hija disponible.
- Solo las pantallas existentes tienen enlace real: `/polizas` y `/autos-particulares`. El resto de opciones quedan visibles como alcance futuro no interactivo del MVP.
- `AppSideMenu.vue` usa `RouterLink`, `useRoute` y una funcion local `isNavigationItemActive`; no consulta metadata, GraphQL, store heredado ni endpoints nuevos.
- `AppShell.vue` centraliza sidebar y topbar para evitar duplicacion entre Polizas y Autos Particulares.
- Polizas y Autos conservan sus contratos read-only y sus servicios actuales.

## Diferencias conscientes

- No se replica el motor de layout dinamico, modos de menu por metadata ni permisos de visibilidad heredados.
- No se consume `Menu_GET_BY_APPLICATION_ID`, tablas `IAP_*`, localizaciones heredadas ni `urlComponentStatic` en runtime.
- No hay apertura por hover/anclaje global ni keep-alive dirigido por metadata; se deja un menu compacto y estable para el MVP.
- El logo de broker se representa como marca textual controlada por iLiniumTech, no como URL cargada desde sesion heredada.
- Las entradas sin pantalla implementada no navegan para evitar rutas rotas y falsas capacidades.

## Riesgos

- La taxonomia final de modulos podria diferir de la configuracion real por broker/perfil y requerira validacion funcional.
- Los permisos reales aun no estan implementados; cuando exista auth de producto, el menu debera filtrar por permisos iLiniumTech, no por permisos AppBuilder.
- La jerarquia de Autos Particulares bajo Polizas es una decision MVP razonable, pendiente de UAT de producto.
- La version actual prioriza mantenibilidad y trazabilidad sobre paridad visual total con los modos avanzados de Apollo.
