# Evidencia QA - baseline datos reales locales

Fecha: 2026-05-19

## Objetivo

Registrar el primer baseline operativo despues de la decision de trabajar con datos reales locales para todas las pantallas funcionales.

## Resultado

Estado: `PARTIAL`

Se confirma:

- La decision de producto esta documentada en `docs/DECISION_DATOS_REALES_LOCALES.md`.
- El frontend local fresco sigue escuchando en `127.0.0.1:5194`.
- El backend local fresco sigue escuchando en `127.0.0.1:5150`.
- En el entorno local existen variables con nombre `ConnectionStrings__DefaultConnection` en ambito `Process` y `Machine`.

No se confirma aun:

- Configuracion local por vertical `Polizas__Repository=Sql`, `Agenda__Repository=Sql`, `Clientes__Repository=Sql`, etc.
- Variables especificas `ConnectionStrings__AgendaModel`, `ConnectionStrings__AppBuilderMaster` o equivalentes por pantalla.
- Smoke SQL local de Agenda CRUD con `Agenda__Repository=Sql`.

## Datos sensibles

No se imprimieron ni copiaron valores de connection strings, usuarios, passwords, resultados SQL ni datos personales.

## Decision operativa

El modo real local queda marcado como `PARTIAL`. La siguiente tarea debe avanzar asi:

1. Si una IA puede resolver configuracion SQL real por variables locales sin imprimir valores, debe ejecutar `T-200B-AGENDA-SMOKE-SQL-LOCAL`.
2. Si no puede resolver Agenda SQL real, debe pasar a `T-301-CLIENTES-SQL-READONLY-LOCAL` y hacer discovery de esquema local con resultados sanitizados.

## Validaciones

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK, sin leaks.
- `git diff --check`: OK.
