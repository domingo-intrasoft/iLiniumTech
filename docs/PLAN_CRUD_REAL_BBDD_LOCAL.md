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

ID: `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL`

Estado: `READY_GUARDED`

Objetivo: confirmar origen SQL local real de Propuestas sin asumir `Solicitudes` por inferencia y, solo si hay evidencia suficiente, implementar lectura SQL local real minimizada, sin escrituras, conversion, emision, tarificacion, documentos, importes reales/banco/datos sensibles y con filtro broker/tenant.

Archivos permitidos:

- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/sdd/specs/iLiniumTech/*propuestas*`
- `docs/qa/*`
- `iLiniumTech.Backend/src/**/Propuestas/**`
- `iLiniumTech.Backend/src/**/Program.cs`
- `iLiniumTech.Backend/src/**/DependencyInjection*.cs`
- `iLiniumTech.Backend/tests/**/Propuestas*`
- `iLiniumTech.Frontend/src/features/propuestas/**`
- `iLiniumTech.Frontend/src/services/**`

Archivos prohibidos:

- `.env*`, dumps, connection strings o capturas con datos reales;
- pantallas no relacionadas;
- escrituras reales de Propuestas;
- conversion a poliza, emision, tarificacion, workflows, documentos, integraciones externas, llamadas de compania o banco;
- importes reales, documentos, contacto, direccion, banco, observaciones libres o datos sensibles en contrato publico;
- datos personales reales en evidencias.

Pasos:

1. Revisar `git status --short --branch`.
2. Leer `docs/DECISION_DATOS_REALES_LOCALES.md`.
3. Leer `docs/sdd/specs/iLiniumTech/SDD-2026-012-propuestas-read-only.md`.
4. Confirmar origen SQL minimizado desde documentacion AppBuilder ya existente sin asumir `Solicitudes`.
5. Si hay origen confirmado, implementar repositorio SQL read-only activable con `Propuestas:Repository=Sql`.
6. Aplicar broker/tenant, parametros y whitelists; sin escrituras, conversion, emision, tarificacion, documentos, importes reales, banco ni datos sensibles.
7. Si no hay origen confirmado, documentar `BLOCKED_ORIGIN_UNCONFIRMED` y no tocar runtime.
8. Conectar frontend a API si todavia usa fixture en modo backend y hay API SQL segura.
9. Ejecutar tests dirigidos y smoke SQL local si hay configuracion; si falta, documentar `SKIPPED_ENV_MISSING`.

Validacion minima:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Propuestas
cd .\iLiniumTech.Frontend
npm run test:unit -- Propuestas
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
| `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL` | `READY_GUARDED` | Propuestas | Confirmar origen real; no usar `Solicitudes` sin UAT/DBA | alto |
| `T-207-LIQUIDACIONES-SDD` | `TODO` | Liq.Cia/Liq.Col | SDD financiera antes de API real | critico |

## Notas de arquitectura detectadas

- Agenda: `dbo.Agenda` es la primera tabla de escritura viable fuera de Polizas.
- Clientes: agregado real `Identidad` + `IdentidadCliente`; `NumDocumento` no puede ser clave publica.
- Recibos: tiene triggers de correo, situacion, poliza y liquidacion; no escribir sin UAT/DBA.
- Siniestros: tiene triggers de agenda/referencia e hijos sensibles; no escribir sin UAT/DBA.
- Suplementos: tiene trigger de log y tablas hijas por tipo; no escribir sin UAT/DBA.
- Propuestas: origen no confirmado; `Solicitudes` no equivale a propuestas de negocio.
