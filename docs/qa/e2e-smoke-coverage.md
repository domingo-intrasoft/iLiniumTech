# Cobertura E2E smoke del MVP

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Proposito

Este documento resume la cobertura Playwright smoke actual para que futuras IA y revisores humanos sepan que protegen las pruebas, que no protegen y cuando ampliarlas.

Los smokes actuales se ejecutan en modo frontend fixture con `VITE_USE_BACKEND=false`. No prueban datos reales ni auth productiva; protegen navegacion, guardas, UI MVP read-only, ausencia de llamadas backend accidentales y ausencia de fugas obvias en DOM.

## Comando

```powershell
cd .\iLiniumTech.Frontend
npm run test:e2e
```

Resultado local ultimo: `8 passed`.

## Matriz actual

| Test | Rutas principales | Que protege |
| --- | --- | --- |
| `tests/e2e/auth-guard.smoke.e2e.ts` | `/clientes`, `/login`, `/polizas` | Guardas de rutas protegidas, redirect, login demo, logout y limpieza de `sessionStorage`. |
| `tests/e2e/polizas.smoke.e2e.ts` | `/polizas`, `/polizas/:id` | Listado, detalle, filtros, limpieza, retorno y ausencia de leaks AppBuilder/runtime. |
| `tests/e2e/menu-navigation.smoke.e2e.ts` | `/polizas`, `/clientes`, `/siniestros`, `/polizas/flotas`, `/polizas/colectivas` | Menu lateral, estados de madurez, submenus visibles/bloqueados y navegacion MVP. |
| `tests/e2e/menu-route-parity.smoke.e2e.ts` | rutas navegables de `appNavigation` | Paridad entre menu estatico y rutas protegidas visibles, estados de madurez por ruta, `Autos Particulares` aparcado y ausencia de llamadas backend. |
| `tests/e2e/domain-fixtures.smoke.e2e.ts` | `/agenda`, `/propuestas`, `/recibos`, `/suplementos`, `/liq-cia`, `/liq-col` | Paginas de dominio fixture read-only, filtro local, limpieza y ausencia de backend/leaks. |
| `tests/e2e/blocked-technical-pages.smoke.e2e.ts` | `/administracion`, `/configuracion`, `/conectividad`, `/logs`, `/by-aunna`, `/controles`, `/estadisticas`, `/informes` | Superficies tecnicas bloqueadas/read-only, mensajes de seguridad, filtros locales y ausencia de secretos. |
| `tests/e2e/autos-particulares.smoke.e2e.ts` | `/autos-particulares` | Incremento tecnico aparcado sigue en fixture local y no llama backend. No reactivar producto sin SDD/UAT. |
| `tests/e2e/responsive-shell.smoke.e2e.ts` | `/login`, `/polizas`, `/clientes` | Usabilidad movil representativa, menu ocultar/mostrar, filtros dentro del viewport y scroll interno de tabla. |

## Reglas para ampliar cobertura

- Toda ruta nueva protegida debe quedar cubierta por unit tests de router y, si es visible en menu, por un smoke E2E representativo.
- Toda ruta fixture debe bloquear llamadas a `/api/**`, `localhost:5146/**` y `127.0.0.1:5146/**` usando `tests/e2e/networkGuards.ts`.
- Si una pagina pasa de fixture a backend real, el smoke debe separarse por modo y la ruta necesita SDD, contrato API, permisos, UAT y evidencia de datos autorizados.
- Si cambia el shell, login, menu o layout responsive, ejecutar al menos `auth-guard`, `menu-navigation` y `responsive-shell`.
- Si cambia una pagina de dominio fixture, actualizar su evidencia QA y el test agrupado correspondiente.
- Si se reactiva `Autos Particulares`, no ampliar su smoke como producto vigente sin decision funcional, SDD actualizada, regla de datos y UAT.

## Comandos relacionados

Validacion frontend habitual:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
```

Validacion documental y seguridad minima:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
git diff --check
```

## Riesgos residuales

- Playwright fixture no sustituye UAT con usuarios, BBDD autorizada ni pruebas de permisos productivos.
- Los smokes no validan paridad visual completa con AppBuilder.
- Las paginas fixture demuestran que no hay dependencia runtime de metadata, pero no cierran reglas funcionales reales.
- La cobertura movil usa Chromium con viewport movil; debe complementarse con revision manual si se entrega a UAT.
