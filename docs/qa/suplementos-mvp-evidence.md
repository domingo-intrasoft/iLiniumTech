# Evidencia QA - Suplementos MVP read-only

Fecha: 2026-05-18

## Alcance

- Superficie `/suplementos` revisada como fixture Vue read-only.
- Sin cambios de codigo de aplicacion.
- Sin API, datos reales, detalle, importes, banco, documentos, recibos/declaraciones relacionados, workflows, adjuntos, exportacion ni escrituras.
- Documentacion alineada con `SDD-2026-013 Suplementos read-only minimizado`.

## Estado actual documentado

- `iLiniumTech.Frontend/src/features/suplementos/SuplementosView.vue` usa fixture local sanitizado.
- `SuplementosView.test.ts` cubre render, paginacion, filtros, empty state, acciones bloqueadas y ausencia de marcadores runtime/secretos/campos sensibles.
- `docs/appbuilder/pages/suplementos/README.md` clasifica la pagina como fixture visible y bloqueada para datos reales hasta SDD/UAT/DBA/permisos.

## Bloqueos mantenidos

- PII de tomador, beneficiario, documentos, telefonos, emails y direcciones.
- IBAN, titular, cuenta, mandato o domiciliacion.
- Importes, primas, valores anteriores/nuevos, tasas, rescates, aportaciones y calculos.
- Tabs por tipo, detalle, documentos, adjuntos, EIAC, workflows y escrituras.

## Evidencia esperada para cierre funcional futuro

- UAT confirma si Suplementos es pagina independiente, subflujo de Polizas o ambos.
- DBA confirma origen de lectura autorizado y regla por broker.
- Permisos `suplementos.catalogs` y `suplementos.read` probados en backend.
- Payloads maliciosos y sort/filtros fuera de whitelist cubiertos por tests.
- Secret scan, dependency audit y CORS audit si se toca API/configuracion.

## Pruebas de esta ronda

Ronda documental. Se ejecuto validacion documental ligera desde raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: ver cierre de la tarea integradora de Operativa seguros.
