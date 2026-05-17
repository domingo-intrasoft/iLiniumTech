# Implementacion MVP menu lateral, paginas estaticas y Polizas

Fecha: 2026-05-15
Actualizacion QA/Docs: 2026-05-17

## Alcance implementado

Este corte integra el menu lateral estatico de iLiniumTech, las paginas protegidas estaticas del menu y la pantalla funcional inicial de Polizas como frontend Vue/TypeScript compilable. No se consume metadata AppBuilder en runtime para pintar menus, filtros, columnas, detalle ni rutas.

## Menu lateral

- `iLiniumTech.Frontend/src/layout/appNavigation.ts` declara la navegacion como codigo fuente.
- `iLiniumTech.Frontend/src/layout/AppSideMenu.vue` renderiza la navegacion con PrimeIcons, enlaces Vue Router y estados deshabilitados para pantallas futuras.
- `iLiniumTech.Frontend/src/layout/AppShell.vue` centraliza sidebar, topbar, estado de sesion y slot de contenido.
- Las pantallas disponibles para prueba quedan accesibles desde el menu como rutas Vue protegidas: `Agenda`, `Clientes`, `Propuestas`, `Polizas`, `Polizas / Flotas`, `Polizas / Colectivas`, `Recibos`, `Suplementos`, `Siniestros`, `Liq.Cia`, `Liq.Col`, `Informes`, `Controles`, `Estadisticas`, `Administracion`, `Configuracion`, `Conectividad`, `By Aunna` y `Logs`.
- `Autos Particulares` sigue existiendo como incremento tecnico aparcado bajo `Polizas`, pero permanece deshabilitado en la navegacion y no es objetivo activo.
- Salvo Polizas, estas paginas son MVP estatico visible o superficies bloqueadas: no conectan datos reales, no habilitan APIs nuevas y no representan paridad AppBuilder.
- Las rutas de detalle bajo `/polizas/:id` mantienen activo el grupo `Polizas`.

## Pantalla de Polizas

- `PolizasView.vue` usa `AppShell` y conserva filtros, listado, paginacion, estados de carga/error/vacio y resumen read-only.
- `PolizasTable.vue` mantiene el enlace en el numero de poliza y anade una accion explicita de detalle con icono de ojo para facilitar descubrimiento de navegacion.
- `PolizaDetailView.vue` usa el mismo shell que el listado, muestra breadcrumb estatico de detalle y conserva el estado solo lectura.
- Los filtros, columnas visibles y secciones de detalle siguen declarados en codigo fuente iLiniumTech.

## Paginas estaticas del menu

- Las rutas estaticas existen para facilitar navegacion, revision visual y futuro crecimiento por SDD.
- Cada pagina debe mantenerse como codigo Vue/TypeScript explicito, con acciones no operativas o bloqueadas cuando no exista contrato backend.
- Cualquier dato real, filtro funcional, detalle, exportacion, escritura, importes, PII, permiso fino o workflow requiere SDD, API explicita, pruebas y UAT/DBA si aplica.
- La readiness consolidada esta en `docs/appbuilder/pages/development-readiness.md`.
- Las evidencias QA por pagina estan en `docs/qa/*-mvp-evidence.md`.

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
- Las opciones con pagina estatica implementada pueden estar visibles, pero deben comunicar claramente su alcance read-only/bloqueado. Las opciones sin alcance funcional confirmado no deben activar datos ni acciones reales.

## Evidencia esperada antes de Done

- Formato, lint, unit tests y build frontend.
- Gate local MVP con Node compatible.
- Smoke visual o E2E que demuestre acceso por menu a Polizas y navegacion a detalle.
- Para paginas estaticas visibles: smoke o tests de vista/ruta que confirmen ausencia de APIs nuevas, acciones reales, secretos, PII y runtime AppBuilder.
- Auditoria de secretos limpia.
- Documentacion de riesgos residuales si quedan decisiones pendientes de producto/UAT.
