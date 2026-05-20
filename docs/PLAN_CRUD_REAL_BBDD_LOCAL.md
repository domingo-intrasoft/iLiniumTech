# Plan CRUD real contra BBDD local

Fecha base: 2026-05-19

Estado: objetivo activo del producto. Sustituye el carril fixture/read-only como prioridad principal, pero no autoriza un CRUD generico ni runtime dinamico AppBuilder.

Decision relacionada: [`DECISION_DATOS_REALES_LOCALES.md`](DECISION_DATOS_REALES_LOCALES.md).

## Regla base

Desde el 2026-05-19 el usuario pide trabajar con datos reales en local para todas las pantallas. Por tanto:

- toda pantalla funcional que se toque debe orientarse a BBDD local real;
- fixtures e in-memory quedan para tests, desarrollo offline o fallback `SKIPPED_ENV_MISSING`;
- las evidencias no deben imprimir secretos ni datos personales;
- la lectura real local es prioritaria;
- la escritura real sigue siendo vertical, protegida por SDD, permisos, flags, transacciones y smoke de limpieza.

Cada pagina se convierte en vertical propia:

1. evidencia AppBuilder/DBA minima;
2. SDD de escritura;
3. permisos `read/create/update/delete`;
4. flag `<Pagina>:WritesEnabled=false` por defecto;
5. SQL parametrizado y whitelisted;
6. filtro broker/tenant;
7. transacciones;
8. smoke create-read-update-delete con limpieza verificable;
9. frontend conectado a API;
10. evidencia QA.

## Siguiente tarea activa

ID: `T-309-SUPLEMENTOS-FE-API-ADAPTER-VERIFY`

Estado: `READY`

Objetivo: verificar/adaptar el frontend de `Suplementos` para consumir API cuando `VITE_USE_BACKEND=true` y usar fixture solo con `VITE_USE_BACKEND=false`, tests/offline o fallback explicito, sin tocar backend, SQL, detalle, exportacion, escrituras, workflows, adjuntos, banco ni importes.

Archivos permitidos:

- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/qa/*suplementos*`
- `iLiniumTech.Frontend/src/features/suplementos/**`
- `iLiniumTech.Frontend/src/services/**`

Archivos prohibidos:

- `.env*`, dumps, connection strings o capturas con datos reales;
- backend, SQL real, repositorios, migraciones, extractores o scripts de BBDD;
- router, layout, menu, auth o pantallas no relacionadas;
- detalle, exportacion, escrituras, workflows, adjuntos, documentos, banco, importes, textos libres o PII real;
- cambios de contrato backend sin SDD.

Pasos:

1. Revisar `git status --short --branch`.
2. Leer `docs/DECISION_DATOS_REALES_LOCALES.md`, SDD/evidencias de `Suplementos` y feature frontend actual.
3. Confirmar si el composable/servicio de `Suplementos` ya usa API con `VITE_USE_BACKEND=true`.
4. Si falta, implementar adaptador frontend explicito con fallback fixture solo cuando `VITE_USE_BACKEND=false`.
5. Actualizar tests y evidencia QA.
6. No tocar backend ni BBDD.

Validacion minima:

```powershell
cd .\iLiniumTech.Frontend
npm run test:unit -- Suplementos
npm run lint
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

## Cola de verticales

| ID | Estado | Vertical | Alcance siguiente | Riesgo |
| --- | --- | --- | --- | --- |
| `T-200-AGENDA-CRUD-BBDD-LOCAL` | `CODE_READY` | Agenda | API + frontend CRUD MVP; pendiente smoke SQL local | medio por PII en asunto |
| `T-200B-AGENDA-SMOKE-SQL-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Agenda | Smoke real con BBDD local y limpieza | medio |
| `T-300-REALDATA-LOCAL-BASELINE` | `DONE` | Todas | Baseline operativo para trabajar con BBDD real local sin secretos | medio |
| `T-301-CLIENTES-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Clientes | SQL local real minimizado; sin banco/direccion/contacto completos | alto |
| `T-302-CLIENTES-CRUD-SDD` | `DONE` | Clientes | SDD escritura `Identidad` + `IdentidadCliente`, PII minimizada | alto |
| `T-303-CLIENTES-CRUD-BBDD-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Clientes | API/frontend CRUD segun SDD-2026-016; update/delete solo MVP-owned | alto |
| `T-304-SINIESTROS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Siniestros | SQL local real minimizado; escritura bloqueada por triggers/intervinientes | alto |
| `T-305-RECIBOS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Recibos | SQL local real minimizado; sin banco y sin importes reales en primer corte | alto |
| `T-306-SUPLEMENTOS-SQL-READONLY-LOCAL` | `DONE_WITH_SKIPPED_SQL_SMOKE` | Suplementos | SQL local real minimizado; escritura bloqueada por workflows/banco/PII | alto |
| `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL` | `BLOCKED_ORIGIN_UNCONFIRMED` | Propuestas | Origen real no confirmado; no usar `Solicitudes` sin UAT/DBA | alto |
| `T-130-LIQCIA-SDD-READONLY` | `DONE` | Liq.Cia | SDD/readiness financiera read-only antes de API real | critico |
| `T-051-LIQCOL-SDD-READONLY` | `DONE` | Liq.Col | SDD/readiness financiera read-only antes de API real | critico |
| `T-207-LIQUIDACIONES-SDD` | `SUPERSEDED_BY_T-130_T-051` | Liq.Cia/Liq.Col | SDD financiera antes de API real | critico |
| `T-052-LOGS-THREAT-MODEL` | `DONE` | Logs | Threat model antes de API real o lectura de logs | critico |
| `T-053-CONNECTIVITY-THREAT-MODEL` | `DONE` | Conectividad | Threat model antes de conectores reales, APIs o llamadas externas | critico |
| `T-011-SIN-FE-API-ADAPTER-BLOCKED` | `DONE` | Siniestros | Frontend API con fallback fixture explicito | medio |
| `T-308-RECIBOS-FE-API-ADAPTER-VERIFY` | `DONE` | Recibos | Frontend API con fallback fixture explicito | alto |
| `T-309-SUPLEMENTOS-FE-API-ADAPTER-VERIFY` | `READY_ACTIVE` | Suplementos | Frontend API con fallback fixture explicito | alto |

## Notas de arquitectura detectadas

- Agenda: `dbo.Agenda` es la primera tabla de escritura viable fuera de Polizas.
- Clientes: agregado real `Identidad` + `IdentidadCliente`; `NumDocumento` no puede ser clave publica.
- Recibos: tiene triggers de correo, situacion, poliza y liquidacion; no escribir sin UAT/DBA.
- Siniestros: tiene triggers de agenda/referencia e hijos sensibles; no escribir sin UAT/DBA.
- Suplementos: tiene trigger de log y tablas hijas por tipo; no escribir sin UAT/DBA.
- Propuestas: origen no confirmado; `Solicitudes` no equivale a propuestas de negocio.
