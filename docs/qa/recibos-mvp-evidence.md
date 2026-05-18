# Evidencia QA - Recibos MVP read-only

Fecha: 2026-05-18

## Alcance

- Superficie `/recibos` revisada como fixture Vue read-only.
- Sin cambios de codigo de aplicacion.
- Sin API, datos reales, importes reales, banco, remesas, cobro, EIAC, exportacion ni escrituras.
- Documentacion alineada con `SDD-2026-009 Recibos read-only minimizado`.

## Estado actual documentado

- `iLiniumTech.Frontend/src/features/recibos/RecibosView.vue` usa fixture local sanitizado.
- `RecibosView.test.ts` cubre render, filtros, limpieza, empty state, acciones bloqueadas y ausencia de marcadores runtime/secretos.
- `docs/appbuilder/pages/recibos/README.md` clasifica la pagina como fixture visible y bloqueada para datos reales hasta SDD/UAT/DBA/permisos.

## Bloqueos mantenidos

- Importes reales, comisiones, impuestos y liquidaciones.
- Datos bancarios, IBAN, remesas, titularidad y cobro operativo.
- PII de cliente/tomador.
- Detalle, exportacion, EIAC, incidencias, documentos y escrituras.

## Evidencia esperada para cierre funcional futuro

- UAT de columnas/filtros.
- DBA confirma origen de lectura y tenant/broker.
- Permisos `recibos.catalogs` y `recibos.read` probados en backend.
- Payloads maliciosos y sort/filtros fuera de whitelist cubiertos por tests.
- Secret scan, dependency audit y CORS audit si se toca API/configuracion.

## Actualizacion contrato fixture 2026-05-18

- `RecibosView.vue` queda consumiendo `types.ts`, `fixtures.ts` y `useRecibosFixture.ts`.
- Se mantiene `/recibos` como fixture/read-only sin API ni datos reales.
- No se activan importes reales, banco, remesas, cobro, detalle, exportacion ni escrituras.
- Prueba dirigida ejecutada: `npm run test:unit -- RecibosView.test.ts` OK, 2 tests.

## Actualizacion T-120-RECIBOS-BE-READONLY-CONTRACT - 2026-05-18

- Se anade backend explicito in-memory para `Recibos`.
- Endpoints disponibles: `GET /api/recibos/catalogs` y `GET /api/recibos`.
- Permisos propios: `recibos.catalogs` y `recibos.read`.
- API key legacy no concede permisos de Recibos; se exige sesion/permisos explicitos.
- No se activa SQL real, importes reales, banco, remesas, cobro, detalle, exportacion ni escrituras.
- Prueba dirigida ejecutada: `dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "RecibosApiTests"` OK, 5 tests.

## Pruebas de esta ronda

Ronda documental. Se ejecuto validacion documental ligera desde raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Resultado: ver cierre de la tarea integradora de Operativa seguros.
