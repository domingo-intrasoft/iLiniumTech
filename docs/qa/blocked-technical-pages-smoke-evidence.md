# Evidencia QA - smoke de paginas tecnicas bloqueadas

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Smoke E2E para superficies tecnicas o de alto riesgo que deben seguir visibles solo como MVP read-only, sin acciones reales ni conexion a backend.

Paginas cubiertas:

- `/administracion`
- `/configuracion`
- `/conectividad`
- `/logs`
- `/by-aunna`
- `/controles`
- `/estadisticas`
- `/informes`

## Fuera de alcance

- No se activan usuarios, permisos ni configuracion real.
- No se exponen secretos, URLs internas, payloads, logs reales ni enlaces externos.
- No se ejecutan conectores, informes, controles, estadisticas ni descargas.
- No se crean APIs nuevas ni contratos backend.
- No se declara paridad AppBuilder.

## Smoke E2E

Archivo:

- `iLiniumTech.Frontend/tests/e2e/blocked-technical-pages.smoke.e2e.ts`

Cubre por pagina:

- Login demo previo.
- Render de heading y tabla.
- Mensajes `Solo lectura` y `Fixture local sin API`.
- Filtro local por texto.
- Limpieza de filtros.
- Ausencia de llamadas a `/api/**`, `localhost:5146/**` y `127.0.0.1:5146/**`.
- Ausencia de marcadores peligrosos: connection strings, SQL, metadata runtime, secretos, tokens, URLs, emails, IBAN y cantidades reales.

## Criterios de interpretacion

Este smoke protege el carril de gobierno: las paginas tecnicas pueden orientar al usuario y al equipo sobre alcance futuro, pero no deben convertirse accidentalmente en pantallas operativas sin SDD, threat review, permisos, contrato API y UAT.

## Riesgos residuales

- Las pruebas E2E no sustituyen una revision de seguridad cuando se implemente funcionalidad real.
- Las paginas siguen usando fixtures demo, por lo que no validan reglas productivas.
- Cualquier futura accion de administracion, configuracion, conectividad, logs, informes o descarga debe nacer de una SDD propia.
