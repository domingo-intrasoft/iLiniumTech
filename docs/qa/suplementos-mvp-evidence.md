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

## Actualizacion contrato fixture 2026-05-18

- `SuplementosView.vue` queda consumiendo `types.ts`, `fixtures.ts` y `useSuplementosFixture.ts`.
- Se mantiene `/suplementos` como fixture/read-only sin API ni datos reales.
- No se activan workflows, adjuntos, banco, importes reales ni escrituras.
- Prueba dirigida ejecutada: `npm run test:unit -- SuplementosView.test.ts` OK, 4 tests.

## Actualizacion backend read-only 2026-05-18

- Se crea API in-memory read-only: `GET /api/suplementos/catalogs` y `GET /api/suplementos`.
- Permisos propios: `suplementos.catalogs` y `suplementos.read`; la API key legacy no concede acceso.
- Contrato minimizado sin tomador, beneficiario, documento, contacto, direccion, IBAN, cuenta, importes, documentos, adjuntos, workflows ni escrituras.
- No hay SQL real, detalle, exportacion ni acciones operativas.
- Prueba dirigida ejecutada: `dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "Suplementos"` OK, 5 tests.

## Actualizacion T-306-SUPLEMENTOS-SQL-READONLY-LOCAL - 2026-05-20

- Se anade lectura SQL local real minimizada para `Suplementos`, activable con `Suplementos:Repository=Sql`.
- La consulta usa `dbo.Suplemento` con join minimizado a `dbo.Poliza` y `dbo.Catalogo`, filtro por `BrokerIntegracionId`, parametros y whitelist de ordenacion.
- El contrato no proyecta `Concepto` real, `Valor`, `ValorAnterior`, `EmailComunicacion`, tomador, beneficiario, documentos, contacto, direccion, banco, importes, recibos/declaraciones, adjuntos ni workflows.
- El frontend `/suplementos` consume API en `VITE_USE_BACKEND=true` y conserva fixture solo para tests/offline.
- La columna `Origen` se elimina del listado visible porque no pertenece al contrato real.
- Smoke SQL local: `SKIPPED_ENV_MISSING` por ausencia de configuracion local visible sin secretos.
- Evidencia detallada: `docs/qa/suplementos-sql-readonly-local-evidence.md`.

## Pruebas de esta ronda

Ronda documental. Se ejecuto validacion documental ligera desde raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: ver cierre de la tarea integradora de Operativa seguros.

## Actualizacion T-309-SUPLEMENTOS-FE-API-ADAPTER-VERIFY - 2026-05-20

Alcance de esta actualizacion:

- Se revisa el frontend de `Suplementos` tras `T-306`.
- `suplementosApi.ts` ya consumia `/api/suplementos/catalogs` y `/api/suplementos` cuando `VITE_USE_BACKEND=true`.
- El fixture queda limitado a `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se anade `suplementosApi.test.ts` para cubrir catalogs/search en modo backend y fallback fixture sin llamadas API.
- No se toca backend, SQL, detalle, exportacion, workflows, adjuntos, banco, importes, documentos ni escrituras.

Evidencia de validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Suplementos` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Riesgos residuales:

- Sigue pendiente smoke SQL real local por falta de configuracion visible sin secretos.
- Detalle, tabs por tipo, workflows, adjuntos, recibos/declaraciones, exportacion, importes, banco y escrituras siguen bloqueados hasta SDD/UAT/DBA especificos.
- La siguiente verificacion equivalente pasa a `Agenda`.
