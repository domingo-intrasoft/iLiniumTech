# Evidencia QA - Siniestros SQL read-only local

Fecha: 2026-05-19

Tarea: `T-304-SINIESTROS-SQL-READONLY-LOCAL`

Estado: `DONE_WITH_SKIPPED_SQL_SMOKE`

## Alcance

- Repositorio SQL read-only para `Siniestros`, activable con `Siniestros:Repository=Sql`.
- Origen SQL minimizado: `dbo.Siniestro` con joins controlados a `dbo.RiesgoPoliza`, `dbo.Poliza` y `dbo.Catalogo`.
- Filtro broker/tenant por `BrokerIntegracionId` y `SESSION_CONTEXT`.
- Frontend `/siniestros` conectado a API cuando `VITE_USE_BACKEND=true`, con fallback fixture solo cuando backend esta desactivado.
- Detalle, exportacion, escrituras, intervinientes, observaciones, documentos, salud, direccion, contacto, matriculas, importes y EIAC siguen bloqueados.

## Contrato minimizado

Campos proyectados:

- `id`
- `referencia`
- `poliza`
- `cliente` como etiqueta minimizada basada en identificador tecnico, no nombre real.
- `compania` como etiqueta minimizada basada en identificador tecnico, no razon social real.
- `situacion`
- `estado`
- `prioridad`
- `fechaSiniestro`
- `fechaParte`
- `tramitador` como etiqueta tecnica, no nombre personal.

Campos excluidos del SQL publico:

- descripcion, danos, garantias, franquicia;
- reserva, indemnizacion y otros importes;
- intervinientes, emails, telefonos, direcciones, documentos y matriculas;
- observaciones, EIAC, aduana y textos libres.

## Smoke SQL local

Resultado: `SKIPPED_ENV_MISSING`.

Se revisaron solo nombres de variables en `Process`, `User` y `Machine`, sin imprimir valores. No habia configuracion local visible para:

- `ConnectionStrings__SiniestrosModel`
- `ConnectionStrings__SiniestrosReadWrite`
- `ConnectionStrings__SiniestrosReadOnly`
- `ConnectionStrings__AppBuilderMaster`
- `ILINIUMTECH__SINIESTROS_CONNECTION`
- `ILINIUMTECH__APPBUILDER_MASTER_CONNECTION`
- `Siniestros__Repository`
- `Siniestros__ConnectionResolver`
- `Siniestros__RequireExecutionContext`
- `Siniestros__ModelDatabaseTypeId`
- `AppBuilder__EncryptionKey`
- `ILINIUMTECH__APPBUILDER_ENCRYPTION_KEY`

No se ejecutaron consultas reales ni se imprimieron connection strings.

## Validacion ejecutada

| Comando | Resultado |
| --- | --- |
| `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Siniestros` | OK, 19 tests |
| `npm run test:unit -- Siniestros` | OK, 2 tests |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |

## Riesgos residuales

- Falta smoke real contra BBDD local por ausencia de configuracion visible sin secretos.
- Catalogos SQL quedan como valores MVP conservadores; UAT/DBA puede pedir catalogos reales en un corte posterior.
- `Siniestros` sigue sin detalle, exportacion ni escritura por riesgo de PII, importes, intervinientes y workflows heredados.

## Siguiente paso

El cursor operativo pasa a `T-305-RECIBOS-SQL-READONLY-LOCAL`.

## Actualizacion frontend adapter - 2026-05-20

Tarea: `T-011-SIN-FE-API-ADAPTER-BLOCKED`

Estado: `DONE`

Resultado:

- Se confirma que el frontend de `Siniestros` consume API cuando `VITE_USE_BACKEND=true`.
- Se confirma que el fixture queda solo para `VITE_USE_BACKEND=false`, tests/offline o backend desactivado explicitamente.
- Se anade prueba unitaria del adaptador para endpoint de catalogos, endpoint de busqueda y fallback fixture.
- No se tocan backend, SQL, repositorios, permisos, detalle, exportacion, escrituras ni datos sensibles.

Validacion:

| Comando | Resultado |
| --- | --- |
| `npm run test:unit -- Siniestros` | OK |
| `npm run format` | OK |
| `npm run lint` | OK |
| `npm run build` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` | OK |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` | OK |
| `git diff --check` | OK |

Siguiente paso operativo: `T-308-RECIBOS-FE-API-ADAPTER-VERIFY`.
