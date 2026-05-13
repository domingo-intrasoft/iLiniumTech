# MVP Polizas - Analisis de diferencias visuales

Fecha: 2026-05-13

## Diagnostico

El primer MVP implementado representaba el contrato tecnico minimo de polizas:

- API de solo lectura;
- metadata AppBuilder trazable;
- filtros basicos;
- tabla de resultados anonimizados.

La captura aportada por el usuario muestra otra cosa: el estado final esperado no es solo un listado, sino una pantalla operacional de AppBuilder/Aunna Tech con shell completo y buscador avanzado expandido.

## Que habia pasado

El corte inicial priorizo la arquitectura limpia y segura del dominio `Polizas.ReadOnly.Listado`. Eso era correcto como base tecnica, pero insuficiente como MVP visual porque dejaba fuera varias piezas que AppBuilder considera parte del componente:

- menu lateral con modulos principales;
- topbar con breadcrumb, entorno, perfil y accesos rapidos;
- toolbar de acciones del componente;
- botones contextuales de polizas;
- pestanas de busqueda/resultado;
- panel `Buscar` con acciones `Limpiar Filtros`, `Cerrar Pestanas`, `Guardar busqueda`, `Avanzada`, `Simple` y `+`;
- secciones de filtros agrupadas por negocio;
- controles de filtro con icono lateral por campo;
- layout denso de 12 columnas.

## Origen tecnico en AppBuilder

La pantalla real se compone desde estos elementos del frontend AppBuilder:

- `Search.vue`: cabecera de busqueda, botones y selector simple/avanzada.
- `SearchTree.vue`: seleccion de filtros, arbol de condiciones y extraccion de `searchConfigParams`.
- `SearchFieldsPanels.vue`: paneles agrupados por cabecera.
- `SearchFields.vue`: render de campos, tamanos `field-xs`, `field-small`, `field-medium`, `field-large`, `field-full` y addon de filtro.

La configuracion fina no esta hardcodeada en Vue. Se lee desde `IAP_ComponentDataSourceFieldConfiguration.searchConfigParams`, con claves como:

- `header`;
- `headerOrder`;
- `size`;
- `collapsed`;
- `headerStyle`;
- `contentStyle`;
- `break`;
- `defaultFilter`.

## Ajuste aplicado en iLiniumTech

Se ha cambiado la primera pantalla `/polizas` para que el MVP represente la forma real del componente:

- shell tipo AppBuilder con sidebar, breadcrumb y topbar;
- toolbar superior con iconos de modulo y estado;
- botones contextuales `Polizas de flota`, `Polizas colectivas`, `Polizas Externas`;
- tab strip con busqueda y resultado;
- panel de busqueda avanzada expandido;
- bloques:
  - `Datos de la poliza`;
  - `Datos de gestion`;
  - `Datos del tomador`;
- campos visibles de la captura con disposicion en grid de 12 columnas;
- resultados conservados como resumen inferior del MVP.

## Huecos pendientes para exactitud completa

Para llegar a fidelidad alta falta extraer, sin exponer secretos, la configuracion exacta de BBDD:

- valores reales de `searchConfigParams`;
- orden real de cada campo por `defaultFilterSearchOrder`;
- tipos de control reales por `dataSourceLookUpId`, catalogos y `sqlType`;
- campos que aparecen por perfil, oficina o permisos;
- botones reales activados por object groups/workflows;
- logo e identidad visual final del broker.

## Decision de seguridad

Aunque el usuario facilito acceso al entorno de pruebas, no se ha guardado ninguna cadena de conexion, usuario ni password en el repositorio. La extraccion exacta debe hacerse mediante variables de entorno locales o secret store, y generar solo JSON sanitizado.
