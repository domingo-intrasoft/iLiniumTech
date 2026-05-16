# Agente componente - Polizas toolbar y acciones contextuales

Fecha: 2026-05-16

Estado: documentacion completada con evidencia local. No es contrato runtime.

## Alcance del subagente

Este subagente analiza la barra superior de la pagina `Polizas`, los botones contextuales visibles en el MVP y la frontera entre acciones visuales heredadas y acciones de producto iLiniumTech.

Archivo permitido:

- `docs/appbuilder/pages/polizas/components/toolbar-acciones-contextuales.md`

## Fuentes revisadas

iLiniumTech:

- `docs/MVP_POLIZAS_DIFERENCIAS.md`
- `docs/appbuilder/menu-polizas-mvp-implementation.md`
- `docs/appbuilder/polizas-listado-analysis.md`
- `iLiniumTech.Frontend/src/features/polizas/PolizasView.vue`
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

AppBuilder:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`

## Evidencia y nivel de certeza

La documentacion visual del MVP indica que la pantalla operacional de AppBuilder/Aunna Tech incluia:

- toolbar de iconos de modulo/estado;
- botones contextuales `Polizas de flota`, `Polizas colectivas`, `Polizas Externas`;
- acciones de busqueda `Buscar`, `Limpiar Filtros`, `Cerrar Pestanas`, `Guardar busqueda`, `Avanzada`, `Simple`, `+`;
- acciones de fila con detalle.

La metadata sanitizada `polizas.metadata.sanitized.json` no trae una lista completa de eventos, workflows o botones de toolbar para Polizas. Por tanto:

- hay evidencia visual/documental de esos botones;
- no hay evidencia suficiente para activar comportamiento funcional;
- deben permanecer deshabilitados o fuera de alcance hasta SDD/UAT.

## Estado iLiniumTech actual

`polizasConstants.ts` define:

- `moduleActions`: Autos, Gestion, Favoritos, Servicios, Riesgos.
- `statusActions`: Validar, Pausar, Cerrar.
- `topBadges`: `F`, `?`, `A`, `L`, `E`, `RH`, `WP`, `AU`.

`PolizasView.vue` muestra:

- grupos de botones de toolbar, todos deshabilitados;
- botones contextuales:
  - `Polizas de flota`;
  - `Polizas colectivas`;
  - `Polizas Externas`;
- runtime strip de solo lectura, origen y bloqueo por contexto;
- tab strip de busqueda/tabla.

Este estado es correcto para MVP documental/visual: se parece al origen, pero no promete acciones no implementadas.

## Interpretacion funcional

Los botones pueden significar cosas distintas en AppBuilder:

- filtro preconfigurado;
- subvista;
- navegacion a otra pagina;
- apertura de pestana;
- accion workflow;
- llamada REST/SOAP;
- procedimiento o proceso backend.

Sin metadata completa y UAT, no se debe asumir comportamiento.

## Decision para iLiniumTech

Regla recomendada:

- mantener botones visuales solo si estan deshabilitados o si tienen caso de uso implementado con API explicita;
- no ejecutar acciones AppBuilder por nombre;
- no mapear botones a workflows heredados sin SDD;
- no habilitar `Validar`, `Pausar`, `Cerrar` si no hay endpoint, permisos, auditoria y UAT.

Cada accion futura debe declarar:

- problema de negocio;
- usuario/rol;
- entrada y salida;
- endpoint API explicito;
- permiso;
- auditoria;
- impacto PII;
- pruebas;
- rollback si escribe datos;
- UAT owner.

## Propuesta Vue estatica

Mantener una estructura por grupos:

- acciones de modulo: visibles pero deshabilitadas hasta producto;
- acciones de estado: fuera de DOM o deshabilitadas;
- acciones de contexto: flota/colectivas/externas como futuras vistas/filtros, no acciones activas;
- acciones de busqueda: buscar/limpiar activos, guardar/cerrar/avanzada/simple como deshabilitados si no tienen contrato.

No convertir el toolbar en un motor de acciones dinamico.

## Propuesta API estatica futura

No hay endpoint necesario para esta ronda. Si una accion se reactiva:

- `GET /api/polizas?scope=flota` solo si producto define `scope`;
- `GET /api/polizas?scope=colectivas` solo si producto define regla de datos;
- endpoints de escritura separados, por ejemplo `POST /api/polizas/{id}/...`, solo con SDD.

No aceptar nombres de botones, ids AppBuilder o workflow ids desde frontend para ejecutar comportamiento.

## Permisos candidatos futuros

- `polizas.read`: ver toolbar read-only.
- `polizas.detail`: accion de detalle.
- `polizas.export`: exportacion futura.
- `polizas.savedSearch.manage`: guardar busquedas.
- `polizas.workflow.execute`: no conceder generico; preferir permisos por accion concreta.

## Riesgos

Workflow:

- botones como `Validar`, `Pausar`, `Cerrar` sugieren escrituras o cambios de estado.
- activar sin SDD puede modificar datos reales o disparar integraciones.

PII:

- exportar o copiar resultados desde toolbar puede filtrar datos personales.

Autorizacion:

- permisos AppBuilder de menu/evento no equivalen automaticamente a permisos iLiniumTech.

UX:

- demasiados botones deshabilitados pueden confundir a usuarios UAT. Deben revisarse visualmente.

## Pruebas obligatorias futuras

Frontend:

- toolbar renderiza botones esperados;
- botones sin contrato estan deshabilitados;
- no se emite accion al pulsar botones deshabilitados;
- acciones activas tienen permiso y test propio.

Backend:

- no aplica hasta activar acciones.
- cualquier accion futura debe tener 401/403/validacion/auditoria.

QA/UAT:

- confirmar significado de cada boton;
- decidir si botones deshabilitados siguen visibles;
- confirmar prioridad de `Flota`, `Colectivas` y `Externas`;
- confirmar si guardar busqueda es necesario.

## Bloqueos

- Falta evidencia de eventos/workflows reales de toolbar.
- Falta UAT de significado de botones.
- Falta permisos de producto para acciones no-read-only.

## Estado final del subagente

Completado con evidencia parcial y bloqueos claros. Ninguna accion contextual debe habilitarse sin SDD.
