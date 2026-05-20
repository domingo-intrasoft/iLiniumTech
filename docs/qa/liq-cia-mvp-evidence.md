# Liq.Cia MVP read-only evidence

Fecha: 2026-05-18

## Alcance

Revision documental de readiness para reporting/liquidaciones. No se modifica codigo de aplicacion, no se activa API y no se usan datos reales.

Fuentes revisadas:

- `AGENTS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/appbuilder/pages/page-agent-rollout.md`
- `docs/appbuilder/pages/liq-cia/README.md`
- `iLiniumTech.Frontend/src/features/liquidaciones-compania/LiquidacionesCompaniaView.vue`
- `iLiniumTech.Frontend/src/features/liquidaciones-compania/LiquidacionesCompaniaView.test.ts`

## Estado actual

- `/liq-cia` es una superficie Vue estatica, protegida y read-only.
- Usa fixture local minimizado, filtros locales, paginacion y empty state.
- No hay API backend, SQL, metadata runtime AppBuilder ni datos reales.
- La UI declara `Solo lectura`, `Fixture local sin API`, `Datos minimizados`, `Operaciones financieras bloqueadas` y `Sin importes reales`.
- Los tests existentes cubren bloqueo de acciones y ausencia de marcadores de runtime, secretos y datos financieros/bancarios reales.

## Acciones bloqueadas

- Detalle.
- Desglose.
- Exportacion.
- Guardado de busqueda.
- Banco, facturas, cierre, validacion, importacion, conciliacion, recalculo y escrituras.

## Riesgos residuales

- Importes, comisiones, facturas y banco requieren permisos y minimizacion explicita.
- PII indirecta puede aparecer por cliente, poliza, recibo, gestor, mediador, comentarios u observaciones.
- El broker debe validarse antes de cualquier consulta real.
- Exportacion financiera requiere threat model, auditoria y SDD especifica.

## Dependencias para avanzar

- SDD draft creada: `docs/sdd/specs/iLiniumTech/SDD-2026-017-liq-cia-read-only.md`.
- SDD read-only pendiente de aprobacion por producto, DBA y seguridad.
- UAT owner para columnas, filtros, estados e importes.
- DBA owner para fuente read-only, claves estables, `SESSION_CONTEXT`, whitelists e indices.
- Seguridad para permisos financieros, logging y exportaciones.

## Evidencia

- Readiness documentado en `docs/appbuilder/pages/liq-cia/README.md`.
- No se ejecutaron tests frontend/backend porque no hubo cambio runtime.
- Validacion documental ligera ejecutada: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` OK.
- `git diff --check` ejecutado sin errores bloqueantes; solo avisos LF/CRLF de normalizacion.

## Actualizacion SDD 2026-05-20

- `T-130-LIQCIA-SDD-READONLY` crea la SDD draft `SDD-2026-017`.
- La SDD delimita un primer corte futuro de listado read-only minimizado.
- Siguen bloqueados API real, SQL, importes reales, banco, facturas, exportacion, detalle financiero, cierre, validacion, importacion, conciliacion y escrituras.
- No se modifica runtime backend/frontend ni se usan datos reales.
- Validacion ejecutada: baseline documental OK, secret scan OK y `git diff --check` OK.
