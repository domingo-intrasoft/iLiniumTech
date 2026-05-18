# Liq.Col MVP read-only evidence

Fecha: 2026-05-18

## Alcance

Revision documental de readiness para reporting/liquidaciones. No se modifica codigo de aplicacion, no se activa API y no se usan datos reales.

Fuentes revisadas:

- `AGENTS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/appbuilder/pages/page-agent-rollout.md`
- `docs/appbuilder/pages/liq-col/README.md`
- `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/LiquidacionesColaboradorView.vue`
- `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/LiquidacionesColaboradorView.test.ts`

## Estado actual

- `/liq-col` es una superficie Vue estatica, protegida y read-only.
- Usa fixture local sanitizado, filtros locales, paginacion y empty state.
- No hay API backend, SQL, metadata runtime AppBuilder ni datos reales.
- La UI declara `Solo lectura`, `Fixture local sin API`, `Datos minimizados y sanitizados`, `Detalle y conceptos no operativos` y `Comisiones y liquidos bloqueados`.
- Los tests existentes cubren bloqueo de acciones y ausencia de marcadores de runtime, secretos, documentos, banco e importes reales.

## Acciones bloqueadas

- Detalle.
- Conceptos.
- Exportacion.
- Guardado de busqueda.
- Documentos, banco, comisiones, retenciones, liquidos, exclusiones, cierre, validacion, recalculo y escrituras.

## Riesgos residuales

- Comisiones, retenciones, liquidos, impuestos y acuerdos de reparto son datos financieros sensibles.
- PII de colaborador, mediador, cliente o documento identificativo debe estar minimizada.
- Motivos libres pueden contener PII.
- El broker debe validarse antes de cualquier consulta real.
- Exportacion requiere permisos especificos, auditoria y SDD.

## Dependencias para avanzar

- SDD read-only aprobada.
- UAT owner para identidad funcional, columnas, filtros, estados y reglas de comision.
- DBA owner para fuente read-only, claves estables, relacion con `Liq.Cia`, `SESSION_CONTEXT`, whitelists e indices.
- Seguridad para minimizacion de documentos, motivos libres, importes, logging y exportaciones.

## Evidencia

- Readiness documentado en `docs/appbuilder/pages/liq-col/README.md`.
- No se ejecutaron tests frontend/backend porque no hubo cambio runtime.
- Validacion documental ligera ejecutada: `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` OK.
- `git diff --check` ejecutado sin errores bloqueantes; solo avisos LF/CRLF de normalizacion.
