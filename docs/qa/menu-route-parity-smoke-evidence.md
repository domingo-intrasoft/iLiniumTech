# Evidencia QA - smoke de paridad menu/rutas

Fecha: 2026-05-17

## Alcance

Smoke Playwright para verificar que las rutas protegidas visibles del menu estatico mantienen paridad con `appNavigation`, renderizan su titulo y estado esperado, y no llaman al backend cuando `VITE_USE_BACKEND=false`.

Fuera de alcance:

- No se toca codigo de aplicacion.
- No se crean ni modifican APIs.
- No se activan datos reales.
- No se reactiva `Autos Particulares`; se verifica que sigue aparcado, deshabilitado y sin enlace de menu.
- No se consume metadata AppBuilder en runtime.

## Archivo nuevo

- `iLiniumTech.Frontend/tests/e2e/menu-route-parity.smoke.e2e.ts`

## Cobertura

- Carga `appNavigation` como fuente de paridad del menu.
- Recorre todas las rutas protegidas navegables del menu estatico.
- Verifica titulo visible por ruta y marcador de estado esperado:
  - `operational`: `/polizas`.
  - `fixture`: paginas MVP read-only con fixture local.
  - `blockedSdd`: rutas bloqueadas por SDD o scopes aparcados.
- Comprueba que `Autos Particulares` permanece `parked`, `disabled` y sin enlace navegable.
- Bloquea patrones `/api/**`, `localhost:5146` y `127.0.0.1:5146` y exige cero llamadas backend.
- Revisa que el DOM no exponga artefactos AppBuilder/runtime ejecutables ni secretos comunes.

## Validacion

Comando objetivo:

```powershell
cd .\iLiniumTech.Frontend
npm run test:e2e -- menu-route-parity
```

Resultado:

- Ejecutado con Node del runtime Codex: `v24.14.0`.
- Resultado: `1 passed`.

Comandos ejecutados:

```powershell
$nodeBin = 'C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin'
$env:PATH = "$nodeBin;$env:PATH"
cd .\iLiniumTech.Frontend
npm run test:e2e -- menu-route-parity
node -v
npm run format
npm run lint
npm run typecheck
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
```

Resultados:

- `npm run test:e2e -- menu-route-parity`: correcto, `1 passed`.
- `npm run format`: correcto tras aplicar Prettier al smoke nuevo.
- `npm run lint`: correcto.
- `npm run typecheck`: correcto.
- `Invoke-SecretScan.ps1`: correcto, sin leaks.

## Riesgos residuales

- El smoke valida paridad visible y estados MVP; no sustituye UAT funcional.
- Las rutas fixture siguen siendo superficies read-only sin API real.
- Las rutas `blockedSdd` siguen bloqueadas hasta SDD, permisos, datos autorizados y UAT.
- La garantia de no backend aplica al modo `VITE_USE_BACKEND=false` del servidor Playwright.
