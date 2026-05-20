# Evidencia QA - Propuestas SQL discovery local

Fecha: 2026-05-20.

## Alcance

- Tarea: `T-307-PROPUESTAS-SQL-DISCOVERY-LOCAL`.
- Resultado: `BLOCKED_ORIGIN_UNCONFIRMED`.
- No se modifica runtime backend/frontend.
- No se implementa repositorio SQL de `Propuestas`.
- No se activan datos reales, escrituras, conversion, emision, tarificacion, documentos, importes, banco, riesgo, workflows ni integraciones.

## Fuentes revisadas

- `AGENTS.md`.
- `docs/DECISION_DATOS_REALES_LOCALES.md`.
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`.
- `docs/PLAN_CRUD_REAL_BBDD_LOCAL.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-012-propuestas-read-only.md`.
- `docs/appbuilder/pages/propuestas/README.md`.
- `docs/qa/propuestas-mvp-evidence.md`.

## Decision tecnica

No se implementa `Propuestas:Repository=Sql` porque no existe evidencia suficiente para confirmar el origen funcional y SQL de la pagina. La decision humana de trabajar con datos reales locales se mantiene vigente, pero no autoriza inferir que `Propuestas` equivale a `Solicitudes` ni crear SQL especulativo.

Faltan, como minimo:

- `componentId` o fila de menu AppBuilder real de `Propuestas`.
- Arbol sanitizado de componentes, datasources, columnas, filtros y acciones.
- Tabla/vista origen aprobada para listado.
- Regla de broker/tenant para ese origen.
- Confirmacion funcional de si `Propuestas` es solicitud, oportunidad, tarificacion, emision, conversion u otro flujo.
- Validacion DBA/UAT/seguridad sobre minimizacion de PII, importes, documentos, riesgo y textos libres.

## Evidencia encontrada

- iLiniumTech contiene ruta protegida `/propuestas` y API in-memory read-only minimizada.
- La SDD `SDD-2026-012` declara explicitamente que SQL real sigue bloqueado hasta UAT/DBA/security review.
- La documentacion AppBuilder de `Propuestas` indica estado `bloqueado externo`.
- `Solicitudes` aparece como concepto tecnico de AppBuilder, pero la evidencia disponible no demuestra equivalencia funcional con la pantalla `Propuestas`.

## Smoke SQL local

- Resultado: `SKIPPED_ORIGIN_UNCONFIRMED`.
- Motivo: no hay origen SQL funcional confirmado. Ejecutar un smoke contra una tabla/vista elegida por inferencia generaria evidencia enganosa.
- No se imprimen ni guardan connection strings, credenciales, dumps, capturas sensibles ni datos personales reales.

## Validacion

- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter Propuestas`: OK, 5 tests.
- `npm run test:unit -- Propuestas`: OK, 4 tests.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK, no leaks found.
- `git diff --check`: OK, sin errores de whitespace.

## Riesgos residuales

- Producto/DBA debe decidir si `Propuestas` se desarrolla como modulo independiente, vista de `Solicitudes`, flujo comercial, tarificacion, emision o conversion.
- Cualquier implementacion futura debe empezar por actualizar `SDD-2026-012` con origen SQL, permisos, campos publicos, campos prohibidos y UAT owner.
- Mantener la API in-memory actual solo como soporte transitorio/test/offline, no como objetivo funcional final.
