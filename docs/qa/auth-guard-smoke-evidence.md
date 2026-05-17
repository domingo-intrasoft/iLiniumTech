# Evidencia QA - smoke de guardas de autenticacion

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Smoke E2E para validar que las rutas MVP protegidas no quedan accesibles sin sesion demo y que el logout limpia el acceso local.

Rutas cubiertas:

- `/clientes`
- `/login`
- `/polizas`

## Fuera de alcance

- No valida un proveedor de autenticacion productivo.
- No valida claims reales, permisos finales, brokers reales ni `SESSION_CONTEXT`.
- No hace llamadas a backend ni prueba `demo-session` backend.
- No sustituye pruebas 401/403 de API cuando se active auth real.

## Smoke E2E

Archivo:

- `iLiniumTech.Frontend/tests/e2e/auth-guard.smoke.e2e.ts`

Cubre:

- Redireccion de `/clientes` a `/login?redirect=/clientes` sin sesion.
- Login demo local y retorno a la ruta solicitada.
- Render del shell autenticado con menu principal.
- Logout desde el shell.
- Limpieza de claves de sesion local `iliniumtech.auth`.
- Bloqueo posterior de `/polizas` y redireccion a `/login?redirect=/polizas`.
- Ausencia de llamadas a `/api/**`, `localhost:5146/**` y `127.0.0.1:5146/**` en modo fixture.

## Criterios de interpretacion

Este smoke protege el contrato minimo de UX/auth del MVP visible: toda pantalla del menu debe pasar por login demo y el usuario debe poder salir sin conservar acceso en `sessionStorage`.

## Riesgos residuales

- La sesion demo local es compatibilidad MVP, no autenticacion productiva.
- La cobertura productiva futura debe anadir tests backend para token invalido, sin permiso, broker cruzado y expiracion real.
- El smoke usa rutas representativas; la proteccion completa del menu tambien se apoya en unit tests de router y revisiones de rutas nuevas.
