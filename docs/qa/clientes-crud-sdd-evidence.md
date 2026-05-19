# Evidencia QA: Clientes CRUD SDD

Fecha: 2026-05-19

Tarea: `T-302-CLIENTES-CRUD-SDD`

## Resultado

Estado: `DONE`

Se crea la SDD de escritura para evolucionar `Clientes` desde lectura SQL local minimizada a CRUD real local guardado. No se modifica codigo de aplicacion y no se ejecutan escrituras reales.

## Artefactos

- `docs/sdd/specs/iLiniumTech/SDD-2026-016-clientes-crud-bbdd.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md`
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`

## Decisiones documentadas

- `Clientes:WritesEnabled=false` por defecto.
- Permisos futuros: `clientes.create`, `clientes.update`, `clientes.delete`.
- Create/update/delete deben usar SQL parametrizado, filtro broker y transacciones.
- Update/delete solo pueden afectar filas creadas por iLiniumTech con marcador `ILMVP-CLI-*`.
- `DELETE` sera baja logica MVP, no borrado fisico.
- No se autoriza `NumDocumento`, contacto, direccion, banco ni PII ampliada en contrato publico o logs.
- El smoke local debe usar datos sinteticos y limpiar o dejar evidencia tecnica sin valores personales.

## Validaciones ejecutadas

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK.
- `git diff --check`: OK.

## Siguiente tarea

Cursor movido a `T-303-CLIENTES-CRUD-BBDD-LOCAL`, con alcance de implementacion guardado por `SDD-2026-016`.

## Riesgos residuales

- La implementacion puede encontrar columnas obligatorias no confirmadas en `Identidad` o `IdentidadCliente`; debe fallar cerrado si no puede insertar de forma segura.
- El smoke SQL real depende de configuracion local fuera de Git. Si falta, debe quedar `SKIPPED_ENV_MISSING`.
- `NombreCompleto`/`RazonSocial` se aceptan como alias visible solo para MVP local y requieren revision antes de entornos no locales.
