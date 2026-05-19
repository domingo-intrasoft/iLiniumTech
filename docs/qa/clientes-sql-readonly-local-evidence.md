# Evidencia QA: Clientes SQL read-only local

Fecha: 2026-05-19

Tarea: `T-301-CLIENTES-SQL-READONLY-LOCAL`

## Resultado

Estado: `DONE_WITH_SKIPPED_SQL_SMOKE`

Se implementa el primer vertical `Clientes` con lectura SQL local real minimizada, activable por configuracion, sin escrituras y con fallback in-memory para tests/desarrollo offline.

## Cambios verificados

- Backend: `SqlClientesRepository`, `SqlClientesQueryBuilder`, seleccion por `Clientes:Repository=Sql`, filtro por `Identidad.BrokerIntegracionId`, `SESSION_CONTEXT` y endpoints protegidos.
- Frontend: `clientesApi.ts`, `useClientes.ts` y `/clientes` conectado a API cuando `VITE_USE_BACKEND=true`.
- Las acciones de ficha, exportacion, desglose y escritura siguen bloqueadas.

## Origen SQL

Detectado en codigo AppBuilder local:

- `IdentidadCliente.ClienteId` es PK y FK hacia `Identidad.Id`.
- `Identidad.BrokerIntegracionId` existe y se usa como filtro de broker.
- `IdentidadCliente.FCR` e `Identidad.FCR` existen como fechas tecnicas disponibles.
- `Identidad.NumDocumento` existe, pero no se proyecta ni se usa en filtros de este incremento.

No se documentan connection strings, valores de entorno ni filas reales.

## Smoke SQL local

Estado: `SKIPPED_ENV_MISSING`.

Motivo: el entorno visible en esta sesion solo expone nombres de variables genericas y no confirma una configuracion por vertical `Clientes` contra la BBDD modelo iLiniumTech. El `ConnectionStrings__DefaultConnection` detectado previamente no contiene el modelo de Clientes esperado.

Para ejecutar smoke local sin guardar secretos:

```powershell
$env:Clientes__Repository='Sql'
$env:Clientes__ConnectionResolver='AppBuilderMaster'
$env:ConnectionStrings__AppBuilderMaster='<valor-local-fuera-de-git>'
$env:VITE_USE_BACKEND='true'
```

Validar despues:

- `GET /api/clientes/catalogs` con sesion y permiso `clientes.catalogs`;
- `GET /api/clientes?page=1&pageSize=25` con sesion, permiso `clientes.read` y broker permitido;
- la respuesta no debe incluir documento legal, email, telefono, direccion, IBAN ni datos bancarios.

## Validaciones ejecutadas

- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Clientes`: OK, 13 tests.
- `npm run test:unit -- Clientes`: OK, 4 tests.

## Riesgos residuales

- `NombreCompleto`/`RazonSocial` se usan como alias visible local porque el usuario ha pedido datos reales locales; revisar antes de cualquier uso no local o produccion.
- La regla de broker queda implementada por `Identidad.BrokerIntegracionId`, pero debe confirmarse con DBA/UAT para modelos donde el aislamiento venga solo por conexion.
- Las escrituras de `Clientes` siguen bloqueadas hasta SDD propia, permisos `clientes.create/update/delete`, transacciones, auditoria y smoke con limpieza.
