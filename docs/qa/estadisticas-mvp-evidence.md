# Estadisticas MVP read-only evidence

Fecha: 2026-05-18

## Alcance

Revision documental de readiness para reporting/estadisticas. No se modifica codigo de aplicacion, no se activa API y no se usan datos reales.

Fuentes revisadas:

- `AGENTS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/appbuilder/pages/page-agent-rollout.md`
- `docs/appbuilder/pages/estadisticas/README.md`
- `iLiniumTech.Frontend/src/features/estadisticas/EstadisticasView.vue`
- `iLiniumTech.Frontend/src/features/estadisticas/EstadisticasView.test.ts`

## Estado actual

- `/estadisticas` es una superficie Vue estatica, protegida y read-only.
- Usa fixture local de KPIs candidatos, filtros locales, paginacion y empty state.
- No hay API backend, SQL, metadata runtime AppBuilder ni cifras reales.
- La UI declara `Solo lectura`, `Fixture local sin API`, `KPIs no confirmados`, `Sin drilldown ni exportacion` y `SDD/UAT pendiente`.
- Los tests existentes cubren bloqueo de refresco/exportacion/drilldown y ausencia de marcadores de runtime, secretos y datos reales.

## Acciones bloqueadas

- Refrescar metricas reales.
- Exportar.
- Drilldown.
- Cambiar periodo contra datos reales.
- Guardar preferencias, cachear agregados, navegar a listados filtrados o abrir detalle operativo.

## Riesgos residuales

- Reidentificacion por agregados pequenos o filtros combinados.
- Exposicion indirecta de primas, recibos, siniestros, clientes, polizas o colaboradores mediante drilldown.
- KPIs inconsistentes por definiciones no aprobadas.
- Cache compartida entre brokers, usuarios, oficinas o permisos.
- Exportaciones con granularidad excesiva.

## Dependencias para avanzar

- SDD read-only aprobada con 1-2 KPIs como maximo para primer incremento.
- UAT owner para formulas, periodos, dimensiones, tolerancias y casos de comparacion.
- DBA owner para fuentes, agregaciones, rendimiento, indices, whitelists y cache.
- Seguridad para supresion de grupos pequenos, drilldown y exportacion.

## Evidencia

- Readiness documentado en `docs/appbuilder/pages/estadisticas/README.md`.
- No se ejecutaron tests frontend/backend porque no hubo cambio runtime.
- Validacion documental ligera ejecutada: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` OK.
- `git diff --check` ejecutado sin errores bloqueantes; solo avisos LF/CRLF de normalizacion.
