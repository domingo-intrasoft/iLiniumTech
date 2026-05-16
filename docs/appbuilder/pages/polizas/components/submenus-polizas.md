# Agente componente - Polizas submenus y vistas hijas

Fecha: 2026-05-16

Estado: documentacion completada con evidencia local. No es contrato runtime.

## Alcance del subagente

Este subagente analiza los submenus actuales bajo `Polizas` en iLiniumTech y la evidencia disponible sobre vistas relacionadas: `Autos Particulares`, `Flotas`, `Colectivas` y botones contextuales `Polizas de flota`, `Polizas colectivas`, `Polizas Externas`.

Archivo permitido:

- `docs/appbuilder/pages/polizas/components/submenus-polizas.md`

## Fuentes revisadas

iLiniumTech:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- `iLiniumTech.Frontend/src/layout/AppSideMenu.test.ts`
- `iLiniumTech.Frontend/src/layout/appNavigation.test.ts`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/workflows/parallel-codex-task-pack.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md`
- `docs/qa/autos-particulares-mvp-evidence.md`
- `docs/appbuilder/menu-lateral-analysis.md`
- `docs/appbuilder/polizas-listado-analysis.md`
- `docs/MVP_POLIZAS_DIFERENCIAS.md`

Metadata:

- `reports/polizas-metadata/polizas.metadata.sanitized.json`

## Evidencia actual

`appNavigation.ts` define `Polizas` como item activo con permiso:

- label `Polizas`;
- ruta `/polizas`;
- permiso `polizas.read`;
- hijos:
  - `Autos Particulares`, ruta `/autos-particulares`, disabled;
  - `Flotas`, disabled;
  - `Colectivas`, disabled.

`PolizasView.vue` muestra botones contextuales deshabilitados:

- `Polizas de flota`;
- `Polizas colectivas`;
- `Polizas Externas`.

`docs/ROADMAP_OBJETIVO_FINAL.md` y `docs/PLAN_MAESTRO_IA.md` indican que `Autos Particulares` existe como incremento tecnico anterior, pero esta aparcado y no es objetivo MVP vigente. Si se reactiva, requiere confirmacion funcional, SDD actualizada y UAT.

La metadata sanitizada de Polizas solo confirma menu `10` y componentes `2824`/`2825`. No confirma subcomponentes especificos para Autos Particulares, Flotas o Colectivas.

## Niveles de certeza

| Elemento | Evidencia | Estado |
| --- | --- | --- |
| Polizas | Metadata sanitizada y frontend actual | Activo MVP |
| Autos Particulares | Ruta/codigo existente y SDD aparcada | Aparcado/deshabilitado |
| Flotas | Submenu en navegacion y boton contextual | Pendiente evidencia funcional |
| Colectivas | Submenu en navegacion y boton contextual | Pendiente evidencia funcional |
| Polizas Externas | Boton contextual documentado | Pendiente evidencia funcional |

## Decision para iLiniumTech

Regla:

- `Polizas` es la pagina activa.
- Los submenus se mantienen como navegacion estatica deshabilitada hasta SDD.
- No se debe inferir una regla SQL por el nombre del submenu.
- No se debe usar metadata AppBuilder para construir hijos de menu en runtime.
- Si se reactiva un submenu, se implementa como pagina o vista iLiniumTech normal, con contrato propio.

## Propuesta Vue estatica futura

Opciones validas si producto reactiva una vista:

1. Ruta hija explicita:
   - `/polizas/flotas`;
   - `/polizas/colectivas`;
   - `/polizas/externas`;
   - `/autos-particulares` solo si producto decide mantenerlo como vertical separado.

2. Filtro de alcance dentro de `/polizas`:
   - query param propio como `vista=flotas`;
   - solo si backend tiene regla validada y permiso.

3. Feature flag de producto:
   - ocultar/deshabilitar hasta UAT.

No hacer:

- `menuId` dinamico;
- `componentId` dinamico;
- cargar hijos desde AppBuilder;
- decidir SQL desde label del submenu.

## Propuesta API estatica futura

Si el submenu es una vista filtrada:

- `GET /api/polizas?scope=flotas`;
- `GET /api/polizas?scope=colectivas`;
- `GET /api/polizas?scope=externas`;

Solo si `scope` es enum backend cerrado y cada valor tiene:

- definicion funcional;
- regla SQL validada;
- permiso;
- tests;
- UAT.

Si el submenu es vertical independiente:

- endpoint propio bajo `/api/<vertical>`;
- DTO propio;
- permiso propio;
- SDD propia.

## Permisos candidatos

- `polizas.read`: pagina base.
- `polizas.scope.flotas.read`: candidato si hay vista de flotas.
- `polizas.scope.colectivas.read`: candidato si hay vista de colectivas.
- `polizas.scope.externas.read`: candidato si hay vista de externas.
- `autosParticulares.read`: ya estaba planteado en SDD aparcada, no reactivar sin confirmacion.

## Riesgos

Producto:

- `Autos Particulares` fue identificado como objetivo por error y queda aparcado.
- nombres como `Flotas` o `Colectivas` pueden no corresponder a una regla unica de BBDD.

SQL:

- filtrar por ramo/division/tipo sin confirmacion puede falsear datos.
- cada vista necesita regla backend whitelist, no filtro libre.

UAT:

- hay que validar muestras contra entorno autorizado sin guardar datos reales.

UX:

- submenus visibles deshabilitados pueden ser utiles para orientar roadmap, pero tambien pueden confundir al usuario probador.

## Pruebas obligatorias futuras

Frontend:

- submenus aparcados permanecen deshabilitados;
- `Polizas` activo mantiene permiso `polizas.read`;
- no se puede navegar a vistas no habilitadas desde menu;
- si una vista se habilita, test de permisos/feature flag.

Backend:

- no aplica mientras esten deshabilitados;
- si se habilita scope, test de regla, broker, permisos y filtros.

QA/UAT:

- confirmar con producto si `Flotas`, `Colectivas`, `Externas` son:
  - vistas;
  - filtros;
  - paginas;
  - acciones;
  - modales;
  - workflows.

## Bloqueos

- Falta evidencia AppBuilder completa de submenus reales.
- Falta definicion funcional de Flotas/Colectivas/Externas.
- Falta decision sobre Autos Particulares tras quedar aparcado.
- Falta regla BBDD y UAT para cualquier vista hija.

## Estado final del subagente

Completado con evidencia parcial. Submenus deben permanecer aparcados/deshabilitados hasta SDD.
