# Evidencia QA - Recibos SQL read-only local

Fecha: 2026-05-20

## Alcance

- Tarea: `T-305-RECIBOS-SQL-READONLY-LOCAL`.
- Vertical: Recibos.
- Tipo: lectura SQL local real minimizada, activable por configuracion.
- Escrituras: no implementadas y bloqueadas.

## Cambios validados

- Backend:
  - nuevo repositorio `SqlRecibosRepository`;
  - query builder SQL parametrizado `SqlRecibosQueryBuilder`;
  - activacion por `Recibos:Repository=Sql`;
  - resolucion de conexion por `Recibos:*`, `AppBuilderMaster` o fallback de Polizas segun `DependencyInjection`;
  - filtro broker/tenant con `BrokerIntegracionId`;
  - filtro de contexto en `GET /api/recibos/catalogs` y `GET /api/recibos`.
- Frontend:
  - `/recibos` consume API cuando `VITE_USE_BACKEND=true`;
  - fixture queda como fallback de tests/offline;
  - columna de importes eliminada del listado;
  - estados de loading/error y modo `BBDD local/API` visibles.

## Minimizacion aplicada

El contrato publico no proyecta:

- importes reales, primas, comisiones, impuestos, liquidaciones ni diferencias;
- datos bancarios, cuenta, IBAN, remesa ni titularidad;
- documentos, telefono, email, direccion u observaciones libres;
- EIAC, aduana, workflows o triggers heredados;
- metadata AppBuilder ni SQL heredado libre.

Los campos `Cliente` y `Compania` se devuelven como etiquetas tecnicas minimizadas (`Cliente <id>`, `Cia <id>`) cuando existan ids asociados a la poliza, sin nombres reales ni documento.

## Smoke SQL local

Resultado: `SKIPPED_ENV_MISSING`.

Se comprobaron solo nombres de variables en `Process`, `User` y `Machine`. No habia configuracion visible para:

- `ConnectionStrings__RecibosModel`;
- `ConnectionStrings__RecibosReadWrite`;
- `ConnectionStrings__RecibosReadOnly`;
- `ConnectionStrings__AppBuilderMaster`;
- `ILINIUMTECH__RECIBOS_CONNECTION`;
- `ILINIUMTECH__APPBUILDER_MASTER_CONNECTION`;
- `Recibos__Repository`;
- `Recibos__ConnectionResolver`;
- `Recibos__RequireExecutionContext`;
- `Recibos__ModelDatabaseTypeId`;
- `AppBuilder__EncryptionKey`;
- `ILINIUMTECH__APPBUILDER_ENCRYPTION_KEY`.

No se imprimieron ni versionaron valores de secretos.

## Pruebas ejecutadas

Todas correctas:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Recibos
cd .\iLiniumTech.Frontend
npm run test:unit -- Recibos
npm run format
npm run lint
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Resultados:

- Backend Recibos: 26 tests OK.
- Frontend Recibos: 2 tests OK.
- `npm run format`: OK.
- `npm run lint`: OK.
- `npm run build`: OK.
- Baseline documental: OK.
- Secret scan: OK, sin leaks.
- `git diff --check`: OK, solo avisos de normalizacion CRLF ya existentes en Git para archivos tocados.

## Riesgos residuales

- Smoke real pendiente hasta que exista configuracion local fuera de Git.
- Los catalogos SQL de Recibos se mantienen conservadores (`No informado`) hasta confirmar taxonomia real por UAT/DBA.
- El detalle, cobro, remesas, exportacion e importes requieren SDD posterior y permisos separados.

## Actualizacion frontend adapter - 2026-05-20

Tarea: `T-308-RECIBOS-FE-API-ADAPTER-VERIFY`

Estado: `DONE`

Resultado:

- Se confirma que el frontend de `Recibos` consume API cuando `VITE_USE_BACKEND=true`.
- Se confirma que el fixture queda solo para `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se anade prueba unitaria del adaptador para endpoint de catalogos, endpoint de busqueda y fallback fixture.
- No se tocan backend, SQL, repositorios, permisos, detalle, exportacion, cobro, banco, importes reales, liquidaciones ni escrituras.

Validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Recibos` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Siguiente paso operativo: `T-309-SUPLEMENTOS-FE-API-ADAPTER-VERIFY`.
