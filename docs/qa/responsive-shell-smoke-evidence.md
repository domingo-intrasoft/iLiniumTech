# Evidencia QA - smoke responsive del shell

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Smoke E2E para validar que el login, el shell autenticado, el menu lateral y una pantalla fixture siguen siendo usables en viewport movil.

Superficies cubiertas:

- `/login?redirect=/polizas`
- `/polizas`
- `/clientes`

## Fuera de alcance

- No se valida paridad visual completa con AppBuilder.
- No se activan APIs nuevas ni datos reales.
- No se validan permisos finos, multi-tenant real ni acciones de negocio.
- No se sustituye una revision manual UX/UAT en dispositivos reales.

## Smoke E2E

Archivo:

- `iLiniumTech.Frontend/tests/e2e/responsive-shell.smoke.e2e.ts`

Cubre:

- Render del login en viewport `390x844`.
- Campos de usuario y contrasena visibles y dentro del viewport.
- Login demo y navegacion a `/polizas`.
- Menu principal visible en movil.
- Boton de ocultar/mostrar menu con estado `aria-expanded`.
- Tabla de polizas contenida en `.table-scroll`, con scroll horizontal interno y sin overflow global de pagina.
- Navegacion desde menu a `/clientes`.
- Filtros de pagina fixture visibles y dentro del viewport.
- Ausencia de llamadas a `/api/**`, `localhost:5146/**` y `127.0.0.1:5146/**`.

## Criterios de interpretacion

Este smoke protege que el MVP visible pueda ensenarse y navegarse tambien en movil sin depender de metadata runtime ni backend. Si falla, el cambio debe tratarse como regresion de shell o layout antes de considerar Done una entrega frontend visible.

## Riesgos residuales

- Playwright usa Chromium de escritorio con viewport movil; no sustituye pruebas en navegador movil real.
- El smoke comprueba rutas representativas, no todas las paginas del menu.
- La tabla de polizas se valida como scroll interno; no se exige remaquetacion responsive completa de columnas.
