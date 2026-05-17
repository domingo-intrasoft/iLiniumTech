# Cierre operativo MVP login, menu y Polizas - 2026-05-17

Rama: `codex/static-polizas-data-api`

## Alcance cerrado con evidencia

Este cierre consolida el estado operativo del MVP vigente:

- login MVP y guardas de rutas protegidas;
- shell y menu lateral estatico, sin metadata runtime;
- pantalla de Polizas read-only con listado, filtros, detalle y broker activo demo;
- paginas del menu en modo fixture/read-only o bloqueadas por SDD;
- endurecimiento de compatibilidad MVP: `DemoSession`, API key local/demo y headers MVP;
- gate local, E2E Playwright y evidencias QA versionadas.

Fuera de alcance confirmado:

- auth productiva real;
- conexion SQL/UAT con datos reales;
- preview real/despliegue;
- escrituras, exportaciones o workflows;
- reactivar `Autos Particulares` como objetivo de producto.

## Estado por area

| Area | Estado | Evidencia | Siguiente paso |
| --- | --- | --- | --- |
| Producto / SDD | Completado con evidencia | Roadmap y plan maestro mantienen Polizas como MVP vigente y `Autos Particulares` aparcado. | Mantener decisiones actualizadas si producto cambia prioridad. |
| Backend / auth MVP | Completado con evidencia parcial | Tests de politicas, demo-session, broker activo, API key fuera de Development y errores sanitizados. | Sustituir compatibilidad MVP por auth real cuando exista decision de proveedor. |
| Backend / datos SQL | Bloqueado externo | Repositorio SQL read-only existe, pero UAT/DBA siguen pendientes. | Validar con BBDD autorizada, cuenta read-only y claves `SESSION_CONTEXT`. |
| Frontend / UX | Completado con evidencia | Login, shell, menu, Polizas, detalle, broker selector y rutas estaticas protegidas. | Mejorar ergonomia Polizas y accesibilidad del shell sin cambiar contrato. |
| QA / gates | Completado con evidencia | `Invoke-MvpQualityGate.ps1`, docs baseline, secret scan, dependency/CORS audit y E2E Playwright. | Reejecutar gate completo antes de PR grande o cambios de runtime/seguridad. |
| Seguridad / privacidad | Completado con evidencia parcial | No secrets, fixtures anonimizadas, PII minimizada, errores publicos sanitizados. | Completar threat model de auth real, broker y datos reales. |
| UAT | Bloqueado externo | Smokes locales y fixtures cubiertos; no hay aceptacion con entorno real. | Ejecutar UAT con responsable funcional y DBA cuando haya entorno autorizado. |

## Evidencia ejecutada en este cierre

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release --filter "FullyQualifiedName~PolizasApiTests"
```

Resultado: `62/62` tests correctos.

```powershell
$nodeBin = 'C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin'
$env:PATH = "$nodeBin;$env:PATH"
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run typecheck
npm run test:e2e
```

Resultado:

- Node usado: `v24.14.0`.
- `npm run format`: correcto.
- `npm run lint`: correcto.
- `npm run typecheck`: correcto.
- `npm run test:e2e`: `8 passed`.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
git diff --check
```

Resultado: documentacion base correcta, secret scan sin leaks y diff check correcto.

## Evidencias versionadas relacionadas

- [login-mvp-evidence.md](login-mvp-evidence.md)
- [login-session-hardening-checklist.md](login-session-hardening-checklist.md)
- [active-broker-change-checklist.md](active-broker-change-checklist.md)
- [polizas-mvp-readonly-evidence.md](polizas-mvp-readonly-evidence.md)
- [menu-navigation-smoke-evidence.md](menu-navigation-smoke-evidence.md)
- [menu-route-parity-smoke-evidence.md](menu-route-parity-smoke-evidence.md)
- [e2e-smoke-coverage.md](e2e-smoke-coverage.md)
- [autonomous-progress-2026-05-17.md](autonomous-progress-2026-05-17.md)

## Riesgos residuales

- `DemoSession`, API key MVP y headers MVP no son auth productiva.
- Las paginas fixture no autorizan datos reales, filtros funcionales, escrituras ni APIs nuevas.
- Los smokes Playwright no sustituyen UAT funcional con datos autorizados.
- El repositorio SQL real requiere DBA, cuenta read-only, matriz de permisos y validacion de PII.
- `Autos Particulares` conserva codigo tecnico aparcado; no debe ampliarse sin decision de producto.

## Siguientes incrementos seguros

1. Preparar matriz documental de permisos productivos para `SDD-2026-005`.
2. Mejorar UX read-only de Polizas sin cambiar contrato API.
3. Revisar accesibilidad/foco del shell y menu lateral.
4. Ampliar regresiones backend de 401/403, `correlationId` y broker cruzado.
5. Reejecutar gate completo con `-RunFrontendE2E` antes de PR o cierre mayor.
