# Plan CRUD real contra BBDD local

Fecha base: 2026-05-19

Estado: objetivo activo del producto. Sustituye el carril read-only como prioridad principal, pero no autoriza un CRUD generico ni runtime dinamico AppBuilder.

## Regla base

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

ID: `T-200B-AGENDA-SMOKE-SQL-LOCAL`

Estado: `READY`

Objetivo: ejecutar smoke real de Agenda contra BBDD local de modelo, sin versionar secretos.

Archivos permitidos:

- `docs/qa/agenda-crud-bbdd-evidence.md`
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`

Archivos prohibidos:

- codigo de aplicacion salvo bug evidente del smoke;
- `.env*`, dumps, connection strings o capturas con datos reales.

Pasos:

1. Revisar `git status --short --branch`.
2. Confirmar que existe configuracion local fuera de Git para `Agenda__Repository=Sql` y `Agenda__WritesEnabled=true`.
3. Si no hay `ConnectionStrings__AgendaModel` ni `ConnectionStrings__AppBuilderMaster`, documentar `SKIPPED_ENV_MISSING` y promover `T-201-CLIENTES-CRUD-SDD`.
4. Arrancar backend local.
5. Login demo o API local con broker permitido.
6. Crear evento `ILMVP-AGE-SMOKE-<timestamp>`.
7. Buscarlo por API.
8. Actualizar titulo/prioridad.
9. Borrarlo logicamente.
10. Confirmar que no aparece en busqueda.
11. Documentar resultado sin imprimir datos reales ni secretos.

Validacion minima:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter AgendaApiTests
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

## Cola de verticales

| ID | Estado | Vertical | Alcance siguiente | Riesgo |
| --- | --- | --- | --- | --- |
| `T-200-AGENDA-CRUD-BBDD-LOCAL` | `CODE_READY` | Agenda | API + frontend CRUD MVP; pendiente smoke SQL local | medio por PII en asunto |
| `T-200B-AGENDA-SMOKE-SQL-LOCAL` | `READY` | Agenda | Smoke real con BBDD local y limpieza | medio |
| `T-201-CLIENTES-CRUD-SDD` | `TODO` | Clientes | SDD escritura `Identidad` + `IdentidadCliente`, PII minimizada, sin banco/direccion/contacto | alto |
| `T-202-CLIENTES-CRUD-BBDD-LOCAL` | `BLOCKED` | Clientes | API/frontend CRUD tras SDD y smoke | alto |
| `T-203-SINIESTROS-SQL-READONLY` | `TODO` | Siniestros | Primero SQL read-only minimizado; escritura bloqueada por triggers/intervinientes | alto |
| `T-204-RECIBOS-SQL-READONLY` | `TODO` | Recibos | Primero SQL read-only sin importes/banco; escritura bloqueada por finanzas | alto |
| `T-205-SUPLEMENTOS-SQL-READONLY` | `TODO` | Suplementos | Primero SQL read-only; escritura bloqueada por workflows/banco/PII | alto |
| `T-206-PROPUESTAS-DISCOVERY` | `TODO` | Propuestas | Confirmar origen real; no usar `Solicitudes` sin UAT/DBA | alto |
| `T-207-LIQUIDACIONES-SDD` | `TODO` | Liq.Cia/Liq.Col | SDD financiera antes de API real | critico |

## Notas de arquitectura detectadas

- Agenda: `dbo.Agenda` es la primera tabla de escritura viable fuera de Polizas.
- Clientes: agregado real `Identidad` + `IdentidadCliente`; `NumDocumento` no puede ser clave publica.
- Recibos: tiene triggers de correo, situacion, poliza y liquidacion; no escribir sin UAT/DBA.
- Siniestros: tiene triggers de agenda/referencia e hijos sensibles; no escribir sin UAT/DBA.
- Suplementos: tiene trigger de log y tablas hijas por tipo; no escribir sin UAT/DBA.
- Propuestas: origen no confirmado; `Solicitudes` no equivale a propuestas de negocio.
