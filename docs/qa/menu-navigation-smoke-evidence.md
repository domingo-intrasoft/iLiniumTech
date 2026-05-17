# Evidencia QA - smoke de menu lateral

Fecha: 2026-05-17
Rama revisada: `codex/static-polizas-data-api`

## Alcance

Incremento de navegacion lateral para que la madurez de cada entrada sea entendible y verificable:

- Leyenda compacta con micro-etiquetas `OK`, `FIC`, `PA` y `SDD`.
- Tooltips y `aria-label` con descripcion completa de cada estado.
- Smoke E2E representativo de menu, estados y rutas MVP.

Fuera de alcance:

- No se activan datos reales.
- No se crean APIs ni servicios nuevos.
- No se cambian permisos backend.
- No se reactiva `Autos Particulares`.
- No se consume metadata AppBuilder en runtime.

## Validacion realizada

Comandos frontend con Node `v24.14.0`:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
```

Resultado:

- Formato correcto.
- Lint correcto.
- Unit frontend: 39 archivos, 186 tests superados.
- Build frontend correcto.
- E2E frontend: 3 tests superados.

Comandos backend/documentacion/seguridad:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
git diff --check
```

Resultado:

- Backend: 88 tests superados.
- Baseline documental correcto.
- Secret scan sin leaks.
- `git diff --check` sin errores; solo avisos locales de normalizacion LF/CRLF.

## Smoke E2E nuevo

Archivo:

- `iLiniumTech.Frontend/tests/e2e/menu-navigation.smoke.e2e.ts`

Cubre:

- Login demo hacia `/polizas`.
- Menu lateral visible.
- Leyenda de estados con `Operativo`, `Fixture`, `Aparcado` y `Bloqueado SDD`.
- `Autos Particulares` visible como texto deshabilitado, sin enlace de menu.
- Navegacion por muestra representativa: `/clientes`, `/siniestros`, `/polizas/flotas`, `/polizas/colectivas`.
- Ausencia de llamadas backend en modo fixture.
- Ausencia en DOM de marcadores de metadata/runtime AppBuilder y secretos comunes.

## Incidencias corregidas

- El primer E2E dirigido fallo porque el texto oculto de la leyenda duplicaba `Fixture local sin API`. Se ajusto el selector exacto.
- La bateria completa detecto despues que el texto `sr-only` largo de la leyenda seguia contaminando busquedas de contenido. Se sustituyo por `aria-label` en el chip, manteniendo accesibilidad sin duplicar texto en DOM.

## Riesgos residuales

- Los estados de menu son informacion de madurez MVP, no permisos productivos.
- Las rutas fixture siguen siendo demos read-only hasta SDD/API/UAT.
- `Flotas` y `Colectivas` continuan sin regla funcional ni backend real.
- `Autos Particulares` mantiene ruta tecnica pero no queda ofrecida como navegacion de menu.
