# Evidencia QA - smoke de paginas de dominio fixture

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Smoke E2E representativo para paginas de dominio que siguen en carril `MVP estatico visible` con fixture local. El objetivo es verificar que cargan como pantallas read-only, filtran localmente y no hacen llamadas backend en modo fixture.

Paginas cubiertas:

- `/agenda`
- `/propuestas`
- `/recibos`
- `/suplementos`
- `/liq-cia`
- `/liq-col`

Fuera de alcance:

- No se conectan APIs nuevas.
- No se validan datos reales.
- No se activan acciones, exportaciones, escrituras, workflows ni permisos finos.
- No se declara paridad AppBuilder.

## Smoke E2E

Archivo:

- `iLiniumTech.Frontend/tests/e2e/domain-fixtures.smoke.e2e.ts`

Cubre por pagina:

- Login demo previo.
- Render de heading y tabla.
- Mensajes `Solo lectura` y `Fixture local sin API`.
- Filtro local por texto.
- Limpieza de filtros.
- Ausencia de llamadas a `/api/**`, `localhost:5146/**` y `127.0.0.1:5146/**`.
- Ausencia de marcadores peligrosos: connection strings, SQL, metadata runtime, secretos, emails, IBAN y cantidades reales.

## Criterios de interpretacion

Este smoke no convierte las paginas en funcionalidad real. Solo aumenta confianza en que las pantallas MVP visibles siguen siendo demos controladas y no se han conectado accidentalmente a backend o datos sensibles.

Para avanzar cualquiera de estas paginas a datos reales hacen falta SDD, permisos iLiniumTech, contrato API explicito, minimizacion de datos y UAT/DBA si aplica.

## Riesgos residuales

- `Clientes`, `Informes`, `Estadisticas` y paginas tecnicas quedan cubiertas por unit tests y otros smokes, pero no por este recorrido E2E de dominio.
- Las comprobaciones de secretos son preventivas y no sustituyen secret scan del repositorio.
- Los fixtures siguen siendo datos demo; no prueban reglas reales de negocio ni multi-tenant.
