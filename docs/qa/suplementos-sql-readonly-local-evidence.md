# Evidencia QA - Suplementos SQL read-only local

Fecha: 2026-05-20

## Alcance

- Tarea: `T-306-SUPLEMENTOS-SQL-READONLY-LOCAL`.
- Vertical: Suplementos.
- Tipo: lectura SQL local real minimizada, activable por configuracion.
- Escrituras: no implementadas y bloqueadas.

## Cambios validados

- Backend:
  - nuevo repositorio `SqlSuplementosRepository`;
  - query builder SQL parametrizado `SqlSuplementosQueryBuilder`;
  - activacion por `Suplementos:Repository=Sql`;
  - resolucion de conexion por `Suplementos:*`, `AppBuilderMaster` o fallback de Polizas segun `DependencyInjection`;
  - filtro broker/tenant con `BrokerIntegracionId`;
  - filtro de contexto en `GET /api/suplementos/catalogs` y `GET /api/suplementos`.
- Frontend:
  - `/suplementos` consume API cuando `VITE_USE_BACKEND=true`;
  - fixture queda como fallback de tests/offline;
  - columna `Origen` eliminada del contrato visible;
  - estados de loading/error y modo `BBDD local/API` visibles.

## Minimizacion aplicada

El contrato publico no proyecta:

- tomador, beneficiario, documento, contacto, direccion ni textos libres;
- `Concepto` real, `Valor`, `ValorAnterior` ni `EmailComunicacion`;
- importes reales, primas, tasas, comisiones, rescates, aportaciones ni calculos;
- datos bancarios, IBAN, cuenta, titular, mandato ni domiciliacion;
- recibos, declaraciones, documentos, adjuntos, EIAC, workflows ni triggers heredados;
- metadata AppBuilder ni SQL heredado libre.

El campo `Concepto` se mantiene como etiqueta neutra (`No informado`) y `Resumen` como `Listado read-only minimizado`, sin leer texto libre real de BBDD.

## Smoke SQL local

Resultado: `SKIPPED_ENV_MISSING`.

Se comprobaron solo nombres de variables en `Process`, `User` y `Machine`. No habia configuracion visible para:

- `ConnectionStrings__SuplementosModel`;
- `ConnectionStrings__SuplementosReadWrite`;
- `ConnectionStrings__SuplementosReadOnly`;
- `ConnectionStrings__AppBuilderMaster`;
- `ILINIUMTECH__SUPLEMENTOS_CONNECTION`;
- `ILINIUMTECH__APPBUILDER_MASTER_CONNECTION`;
- `Suplementos__Repository`;
- `Suplementos__ConnectionResolver`;
- `Suplementos__RequireExecutionContext`;
- `Suplementos__ModelDatabaseTypeId`;
- `AppBuilder__EncryptionKey`;
- `ILINIUMTECH__APPBUILDER_ENCRYPTION_KEY`.

No se imprimieron ni versionaron valores de secretos.

## Pruebas ejecutadas

Todas correctas:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Suplementos
cd .\iLiniumTech.Frontend
npm run test:unit -- Suplementos
npm run format
npm run lint
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Resultados:

- Backend Suplementos: 31 tests OK.
- Frontend Suplementos: 4 tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- Baseline documental: OK.
- Secret scan: OK, sin leaks.
- `git diff --check`: OK, solo avisos de normalizacion CRLF ya existentes en Git para archivos tocados.

## Riesgos residuales

- Smoke real pendiente hasta que exista configuracion local fuera de Git.
- Los catalogos SQL de Suplementos se mantienen conservadores (`No informado`) hasta confirmar taxonomia real por UAT/DBA.
- Detalle, tabs por tipo, workflows, adjuntos, recibos/declaraciones, exportacion y escrituras requieren SDD posterior.

## Actualizacion frontend adapter - 2026-05-20

Tarea: `T-309-SUPLEMENTOS-FE-API-ADAPTER-VERIFY`

Estado: `DONE`

Resultado:

- Se confirma que el frontend de `Suplementos` consume API cuando `VITE_USE_BACKEND=true`.
- Se confirma que el fixture queda solo para `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se anade prueba unitaria del adaptador para endpoint de catalogos, endpoint de busqueda y fallback fixture.
- No se tocan backend, SQL, repositorios, permisos, detalle, exportacion, workflows, adjuntos, documentos, banco, importes reales ni escrituras.

Validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Suplementos` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Siguiente paso operativo: `T-310-AGENDA-FE-CRUD-API-ADAPTER-VERIFY`.
