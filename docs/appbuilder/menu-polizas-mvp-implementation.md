# Implementacion MVP menu lateral y Polizas

Fecha: 2026-05-15

## Alcance implementado

Este corte integra el menu lateral estatico de iLiniumTech y mantiene la pantalla de Polizas como frontend Vue/TypeScript compilable. No se consume metadata AppBuilder en runtime para pintar menus, filtros, columnas, detalle ni rutas.

## Menu lateral

- `iLiniumTech.Frontend/src/layout/appNavigation.ts` declara la navegacion como codigo fuente.
- `iLiniumTech.Frontend/src/layout/AppSideMenu.vue` renderiza la navegacion con PrimeIcons, enlaces Vue Router y estados deshabilitados para pantallas futuras.
- `iLiniumTech.Frontend/src/layout/AppShell.vue` centraliza sidebar, topbar, estado de sesion y slot de contenido.
- Las pantallas disponibles para prueba quedan accesibles desde el menu: `Polizas` y `Autos Particulares`.
- Las rutas de detalle bajo `/polizas/:id` mantienen activo el grupo `Polizas`.

## Pantalla de Polizas

- `PolizasView.vue` usa `AppShell` y conserva filtros, listado, paginacion, estados de carga/error/vacio y resumen read-only.
- `PolizasTable.vue` mantiene el enlace en el numero de poliza y anade una accion explicita de detalle con icono de ojo para facilitar descubrimiento de navegacion.
- `PolizaDetailView.vue` usa el mismo shell que el listado, muestra breadcrumb estatico de detalle y conserva el estado solo lectura.
- Los filtros, columnas visibles y secciones de detalle siguen declarados en codigo fuente iLiniumTech.

## Trazabilidad documental

Analisis de origen AppBuilder:

- `docs/appbuilder/menu-lateral-analysis.md`
- `docs/appbuilder/polizas-filtros-busqueda-analysis.md`
- `docs/appbuilder/polizas-listado-analysis.md`
- `docs/appbuilder/polizas-detalle-analysis.md`

## Reglas que se mantienen

- AppBuilder no se usa como runtime dinamico.
- No se consultan tablas `IAP_*`, GraphQL de menu ni definiciones de componentes para construir la UI en ejecucion.
- La evolucion futura debe hacerse como una aplicacion normal: nuevas paginas, componentes, rutas, API explicitas, tests y documentacion.
- Las opciones del menu sin pantalla implementada permanecen deshabilitadas para no prometer capacidades inexistentes.

## Evidencia esperada antes de Done

- Formato, lint, unit tests y build frontend.
- Gate local MVP con Node compatible.
- Smoke visual o E2E que demuestre acceso por menu a Polizas y navegacion a detalle.
- Auditoria de secretos limpia.
- Documentacion de riesgos residuales si quedan decisiones pendientes de producto/UAT.
