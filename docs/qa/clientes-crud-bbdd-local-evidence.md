# Evidencia QA: Clientes CRUD BBDD local

Fecha: 2026-05-19

Tarea: `T-303-CLIENTES-CRUD-BBDD-LOCAL`

## Resultado

Estado: `DONE_WITH_SKIPPED_SQL_SMOKE`

Se implementa CRUD real local de `Clientes` segun `SDD-2026-016`, con API explicita, permisos, flag de escritura, SQL parametrizado, transacciones y frontend minimo. Las escrituras quedan desactivadas por defecto y update/delete solo pueden afectar filas MVP-owned con marcador `ILMVP-CLI-*`.

## Cambios verificados

- Backend:
  - permisos `clientes.create`, `clientes.update`, `clientes.delete`;
  - endpoints `POST /api/clientes`, `PUT /api/clientes/{id}`, `DELETE /api/clientes/{id}`;
  - flag `Clientes:WritesEnabled=false` por defecto;
  - `ClientesWriteValidator` con campos minimizados;
  - `SqlClientesCommandBuilder` y `SqlClientesWriteRepository`;
  - transacciones en create/update/delete;
  - baja logica por `ILMVP-CLI-DELETED-*`;
  - lectura SQL excluye bajas MVP.
- Frontend:
  - llamadas `createCliente`, `updateCliente`, `deleteCliente`;
  - acciones visibles solo en modo backend con permisos;
  - update/delete deshabilitados para filas no `ILMVP-CLI-*`;
  - no se solicita documento, contacto, direccion ni banco.

## Smoke SQL local

Estado: `SKIPPED_ENV_MISSING`.

Motivo: no hay nombres de variables de entorno visibles para `Clientes__*`, `ConnectionStrings__Clientes*`, `ConnectionStrings__AppBuilderMaster`, `ILINIUMTECH__CLIENTES*` ni `ILINIUMTECH__APPBUILDER_MASTER*` en `Process` o `Machine`. No se imprimen ni se piden secretos.

Cuando exista configuracion local fuera de Git, ejecutar smoke con datos sinteticos:

```powershell
$env:Clientes__Repository = "Sql"
$env:Clientes__ConnectionResolver = "AppBuilderMaster"
$env:ConnectionStrings__AppBuilderMaster = "<secret-local-fuera-de-git>"
$env:Clientes__WritesEnabled = "true"
```

Flujo esperado:

1. `POST /api/clientes` con `nombreMostrable` sintetico.
2. `GET /api/clientes?texto=<token sintetico>` devuelve una fila `ILMVP-CLI-*`.
3. `PUT /api/clientes/{id}` actualiza alias/segmento permitido.
4. `DELETE /api/clientes/{id}` aplica baja logica.
5. `GET /api/clientes?texto=<token sintetico>` no devuelve activos.
6. Evidencia solo con codigos HTTP, conteos y estado residual tecnico, sin PII ni connection strings.

## Validaciones ejecutadas

- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Clientes`: OK, 23 tests.
- `npm run test:unit -- Clientes`: OK, 4 tests.
- `npm run format`: OK tras aplicar Prettier a `ClientesView.vue` y `useClientes.ts`.
- `npm run lint`: OK.
- `npm run build`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK.
- `git diff --check`: OK.
- `Invoke-WebRequest http://127.0.0.1:5194/clientes?fresh=t303-clientes-crud`: OK, HTTP 200.

## Riesgos residuales

- El smoke SQL real queda pendiente hasta que exista configuracion local fuera de Git.
- El esquema local puede tener columnas obligatorias adicionales no detectadas; el repositorio debe fallar cerrado y no hacer commit parcial por la transaccion.
- La baja no es baja funcional de negocio; es marcador MVP `ILMVP-CLI-DELETED-*`.
- Clientes historicos no se pueden actualizar ni eliminar desde este MVP.
