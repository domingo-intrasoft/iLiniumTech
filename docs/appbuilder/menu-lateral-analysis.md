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
- La navegacion visible mantiene la estructura funcional observada: modulos principales compactos, iconos PrimeIcons y Polizas como area activa.
- Detectado en el frontend actual: casi todas las entradas principales del menu tienen rutas Vue protegidas y pantallas MVP estaticas. Estas rutas no equivalen a datos reales, paridad AppBuilder, API nueva ni permisos productivos.
- Detectado en `src/layout/appNavigation.ts`: `Autos Particulares` conserva ruta tecnica `/autos-particulares`, pero sigue marcado como `disabled: true`. Se mantiene aparcada y no reactivada.
- Detectado en `src/layout/appNavigation.ts`: `Flotas` y `Colectivas` tienen rutas explicitas `/polizas/flotas` y `/polizas/colectivas` bajo Polizas. Se tratan como scopes estaticos visibles, no como modulos funcionales conectados.
- `AppSideMenu.vue` usa `RouterLink`, `useRoute` y una funcion local `isNavigationItemActive`; no consulta metadata, GraphQL, store heredado ni endpoints nuevos.
- `AppShell.vue` centraliza sidebar y topbar para evitar duplicacion entre Polizas y Autos Particulares.
- Polizas conserva su contrato read-only actual. Autos Particulares queda como incremento tecnico previo aparcado; su existencia de codigo/ruta no autoriza ampliarlo.

## Diferencias conscientes

- No se replica el motor de layout dinamico, modos de menu por metadata ni permisos de visibilidad heredados.
- No se consume `Menu_GET_BY_APPLICATION_ID`, tablas `IAP_*`, localizaciones heredadas ni `urlComponentStatic` en runtime.
- No hay apertura por hover/anclaje global ni keep-alive dirigido por metadata; se deja un menu compacto y estable para el MVP.
- El logo de broker se representa como marca textual controlada por iLiniumTech, no como URL cargada desde sesion heredada.
- Las rutas MVP estaticas orientan navegacion y pruebas, pero mantienen bloqueadas las capacidades con datos reales, acciones, filtros funcionales, workflows, permisos finos y APIs nuevas hasta SDD, permisos, DBA/UAT y contrato backend explicito.

## Riesgos

- La taxonomia final de modulos podria diferir de la configuracion real por broker/perfil y requerira validacion funcional.
- Los permisos reales aun no estan implementados; cuando exista auth de producto, el menu debera filtrar por permisos iLiniumTech, no por permisos AppBuilder.
- `Autos Particulares` no debe reactivarse por la sola existencia de ruta tecnica; requiere decision de producto, SDD actualizada, regla funcional, DBA/UAT y permisos.
- `Flotas` y `Colectivas` pueden parecer navegables por existir como scopes estaticos, pero siguen bloqueadas para datos reales hasta validar regla funcional, permisos, contrato API y UAT/DBA.
- La version actual prioriza mantenibilidad y trazabilidad sobre paridad visual total con los modos avanzados de Apollo.
