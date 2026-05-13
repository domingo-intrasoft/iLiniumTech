# App policy: extractor

## Scope

Future safe read-only extraction of AppBuilder metadata from `IL_Maestro`, `AunnaTechADM`, and related model databases.

## Rules

- Read connection strings only from environment variables or secret stores.
- Never print or commit connection strings.
- Generate sanitized JSON only.
- Treat AppBuilder SQL fragments as untrusted metadata.
- Whitelist tables, views, columns, sort fields, and operations.

## Required checks

- Unit tests for mapping and redaction.
- Tests for missing required metadata.
- Tests for rejected SQL fragments and unsupported filters.
- Integration tests only against test databases or containers.

## First target

Polizas:

- application `2`;
- menu `10`;
- root component `2824`;
- CRUD component `2825`;
- component datasource `354`;
- datasource `146`;
- model object `Pantalla_Polizas`.
