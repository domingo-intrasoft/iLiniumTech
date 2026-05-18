# Inventario IA de paginas AppBuilder

Fecha: 2026-05-16

Este directorio centraliza el analisis por pagina del menu actual para migracion controlada a iLiniumTech.

Regla base: estos documentos son evidencia, trazabilidad y entrada para SDD/scaffolding revisado. No son contrato runtime. El frontend y el backend de iLiniumTech no deben consumir metadata AppBuilder para decidir pantallas, menus, permisos, queries ni workflows.

## Fuente del inventario

El inventario se toma del menu estatico actual de iLiniumTech:

- [`iLiniumTech.Frontend/src/layout/appNavigation.ts`](../../../iLiniumTech.Frontend/src/layout/appNavigation.ts)
- [`docs/appbuilder/menu-lateral-analysis.md`](../menu-lateral-analysis.md)

La regla de arquitectura que gobierna esta carpeta esta en:

- [`docs/DECISION_PRODUCTO_ARQUITECTURA.md`](../../DECISION_PRODUCTO_ARQUITECTURA.md)
- [`docs/PLAN_MAESTRO_IA.md`](../../PLAN_MAESTRO_IA.md)
- [`docs/ROADMAP_OBJETIVO_FINAL.md`](../../ROADMAP_OBJETIVO_FINAL.md)

## Readiness consolidada

Detalle operativo para futuras IA:

- [`development-readiness.md`](development-readiness.md): readiness consolidada por pagina.
- [`page-agent-rollout.md`](page-agent-rollout.md): paquete activo de coordinacion de agentes por pagina tras el cierre local de `Polizas CRUD BBDD`.

Resumen ejecutivo:

- `Polizas` queda como pagina funcional inicial con busqueda/listado/detalle.
- Detectado en el frontend actual: casi todo el menu ya tiene rutas Vue protegidas y pantallas MVP estaticas con contenido de modulo, acciones bloqueadas o no operativas y sin consumo runtime de metadata.
- `MVP estatico visible` no significa datos reales ni paridad AppBuilder. Para conectar API, filtros, acciones, PII, importes o workflows siguen haciendo falta SDD, permisos, DBA/UAT y contrato backend explicito.
- `Flotas` y `Colectivas` existen como scopes estaticos bajo Polizas. Son visibles y navegables en el MVP, pero quedan bloqueadas para datos reales hasta SDD, permisos, regla funcional, DBA/UAT y API explicita.
- `Autos Particulares` conserva ruta tecnica, pero esta aparcada y no reactivada.
- Las entradas administrativas, tecnicas o genericas (`Administracion`, `Configuracion`, `Conectividad`, `Controles`, `Logs`, `By Aunna`, etc.) son especialmente sensibles: se muestran como superficies bloqueadas para orientar al usuario, pero no replican Builder ni habilitan configuracion real.

| Entrada de menu actual | Estado de readiness | Documento |
| --- | --- | --- |
| Agenda | MVP estatico visible; SDD draft de listado read-only por rango; datos/API pendientes | [`agenda/README.md`](agenda/README.md), [`SDD-2026-011`](../../sdd/specs/iLiniumTech/SDD-2026-011-agenda-read-only.md) |
| Clientes | MVP estatico visible; SDD draft de listado read-only minimizado; datos/API/PII pendientes | [`clientes/README.md`](clientes/README.md), [`SDD-2026-010`](../../sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md) |
| Propuestas | MVP estatico visible; SDD draft de listado read-only minimizado; datos/API pendientes | [`propuestas/README.md`](propuestas/README.md), [`SDD-2026-012`](../../sdd/specs/iLiniumTech/SDD-2026-012-propuestas-read-only.md) |
| Polizas | Lista para desarrollo inicial | [`polizas/README.md`](polizas/README.md) |
| Polizas / Autos Particulares | Aparcada; ruta tecnica deshabilitada, no reactivada | Sin documento de pagina en esta carpeta; ver [`SDD-2026-006`](../../sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md) |
| Polizas / Flotas | MVP estatico visible; scope bajo Polizas; datos/API pendientes | [`polizas-flotas/README.md`](polizas-flotas/README.md) |
| Polizas / Colectivas | MVP estatico visible; scope bajo Polizas; datos/API pendientes | [`polizas-colectivas/README.md`](polizas-colectivas/README.md) |
| Recibos | MVP estatico visible; SDD draft de listado read-only minimizado; datos/API/importes pendientes | [`recibos/README.md`](recibos/README.md), [`SDD-2026-009`](../../sdd/specs/iLiniumTech/SDD-2026-009-recibos-read-only.md) |
| Suplementos | MVP estatico visible; SDD draft de listado read-only minimizado; datos/API pendientes | [`suplementos/README.md`](suplementos/README.md), [`SDD-2026-013`](../../sdd/specs/iLiniumTech/SDD-2026-013-suplementos-read-only.md) |
| Siniestros | MVP estatico visible; SDD draft de listado read-only minimizado; datos/API/PII pendientes | [`siniestros/README.md`](siniestros/README.md), [`SDD-2026-008`](../../sdd/specs/iLiniumTech/SDD-2026-008-siniestros-read-only.md) |
| Liq.Cia | MVP estatico visible; datos/API/importes pendientes | [`liq-cia/README.md`](liq-cia/README.md) |
| Liq.Col | MVP estatico visible; datos/API/importes pendientes | [`liq-col/README.md`](liq-col/README.md) |
| Informes | MVP estatico visible; inventario funcional/API pendientes | [`informes/README.md`](informes/README.md) |
| Controles | MVP estatico visible; alcance funcional pendiente | [`controles/README.md`](controles/README.md) |
| Estadisticas | MVP estatico visible; casos analiticos/API pendientes | [`estadisticas/README.md`](estadisticas/README.md) |
| Administracion | MVP estatico visible; acciones bloqueadas por riesgo Builder | [`administracion/README.md`](administracion/README.md) |
| Configuracion | MVP estatico visible; acciones bloqueadas por riesgo configuracion | [`configuracion/README.md`](configuracion/README.md) |
| Conectividad | MVP estatico visible; acciones bloqueadas por riesgo REST/SOAP heredado | [`conectividad/README.md`](conectividad/README.md) |
| By Aunna | MVP estatico visible; definicion funcional pendiente | [`by-aunna/README.md`](by-aunna/README.md) |
| Logs | MVP estatico visible; retencion/permisos/API pendientes | [`logs/README.md`](logs/README.md) |

## Documentos de componentes ya existentes

Polizas tiene documentacion por componentes y por eso es la unica pagina lista para desarrollo inicial:

- [`polizas/components/busqueda-filtros.md`](polizas/components/busqueda-filtros.md)
- [`polizas/components/listado-grid.md`](polizas/components/listado-grid.md)
- [`polizas/components/detalle-navegacion.md`](polizas/components/detalle-navegacion.md)
- [`polizas/components/toolbar-acciones-contextuales.md`](polizas/components/toolbar-acciones-contextuales.md)
- [`polizas/components/submenus-polizas.md`](polizas/components/submenus-polizas.md)

Scopes estaticos bajo Polizas:

- [`polizas-flotas/README.md`](polizas-flotas/README.md)
- [`polizas-colectivas/README.md`](polizas-colectivas/README.md)

Clientes tiene subdocumentos utiles para futura SDD, pero siguen bloqueados para desarrollo porque falta confirmar metadata real de pagina:

- [`clientes/components/busqueda-listado.md`](clientes/components/busqueda-listado.md)
- [`clientes/components/resumen-identidad.md`](clientes/components/resumen-identidad.md)
- [`clientes/components/polizas.md`](clientes/components/polizas.md)
- [`clientes/components/recibos.md`](clientes/components/recibos.md)
- [`clientes/components/suplementos.md`](clientes/components/suplementos.md)
- [`clientes/components/siniestros.md`](clientes/components/siniestros.md)
- [`clientes/components/riesgos.md`](clientes/components/riesgos.md)

## Salida minima por pagina

Cada agente jefe de pagina debe documentar:

- fuentes revisadas;
- evidencia de menu/metadata encontrada o ausencia de evidencia;
- componente raiz y componentes hijos si existen;
- pestanas, submenus o areas internas detectadas;
- datos, catalogos, filtros y acciones heredadas relevantes;
- permisos historicos observados y permiso iLiniumTech candidato;
- propuesta de pagina Vue/API estatica;
- que no debe replicarse;
- riesgos de PII, SQL, workflows, permisos o multi-tenant;
- pruebas necesarias para desarrollo futuro;
- estado final: `Lista para desarrollo inicial`, `Investigacion parcial`, `Bloqueada por falta de metadata de pagina`, `Aparcada` o equivalente justificado.

## Reglas de uso

- Esta documentacion sirve para trazabilidad, analisis, comparativa, SDD y scaffolding revisado.
- Ningun README de esta carpeta, JSON de extractor ni metadata `IAP_*` debe ser consumido por frontend/backend como contrato runtime.
- No generar endpoints de metadata para pintar pantallas, menus, permisos, queries o workflows.
- No convertir componentes genericos de AppBuilder (`Search`, `DynamicTabView`, `DynamicCrudTabla`, controles, workflows, REST/SOAP genericos) en producto por deduccion.
- Todo desarrollo posterior debe quedar como Vue/TypeScript estatico y API .NET explicita, con permisos iLiniumTech en backend.
- No leer ni copiar connection strings, passwords, dumps, capturas sensibles ni datos personales reales.
- No ejecutar extractor `Live` ni consultas SQL reales sin aprobacion explicita y entorno read-only.
- No modificar frontend/backend durante una ronda documental de pagina.

## Siguiente ciclo recomendado de agentes

Cuando aparezca metadata real de una pagina:

1. Crear o actualizar el README de la pagina con evidencia sanitizada de menu, `menuId`, `componentId`, componente raiz, hijos, tabs/submenus, datasources, acciones, permisos historicos y bloqueos.
2. Solo si existen componentes o pestanas reales confirmadas, abrir agentes por componente/pestana y crear `components/*.md`.
3. Abrir o actualizar una SDD funcional con objetivo, fuera de alcance, contrato Vue/API, permisos, riesgos de datos y UAT owner.
4. Desarrollar despues de la SDD como codigo fuente iLiniumTech revisado, nunca como runtime dinamico AppBuilder.
5. Cerrar con pruebas, evidencia sin secretos, riesgos residuales y confirmacion explicita de que no se introdujo dependencia runtime de metadata AppBuilder.
