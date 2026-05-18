# QA operativo iLiniumTech

Indice vivo para localizar evidencias, gates y bloqueos del MVP. Este documento no sustituye la Definition of Done ni las SDD; sirve como mapa rapido para agentes IA y revisores humanos.

## Lectura minima antes de cerrar cambios

- [definition-of-done.md](definition-of-done.md): criterios generales de Done, QA y UAT.
- [phase-closure-checklist.md](phase-closure-checklist.md): plantilla para cierres de fase o PRs con varias areas.
- [e2e-smoke-coverage.md](e2e-smoke-coverage.md): matriz de smokes Playwright y reglas para ampliarlos.
- [mvp-login-menu-polizas-closure-2026-05-17.md](mvp-login-menu-polizas-closure-2026-05-17.md): cierre operativo vigente de login, menu y Polizas.
- [autonomous-progress-2026-05-17.md](autonomous-progress-2026-05-17.md): log de incrementos autonomos y comandos ejecutados.
- [autonomous-progress-2026-05-18.md](autonomous-progress-2026-05-18.md): continuacion autonoma con evidencias tras el cambio de dia local.

## Evidencia por area

| Area | Evidencia principal | Para que sirve |
| --- | --- | --- |
| Login y sesion | [login-mvp-evidence.md](login-mvp-evidence.md), [login-session-hardening-checklist.md](login-session-hardening-checklist.md), [auth-guard-smoke-evidence.md](auth-guard-smoke-evidence.md) | Validar `/login`, guardas, `DemoSession`, logout y errores seguros. |
| Broker activo | [active-broker-change-checklist.md](active-broker-change-checklist.md) | Validar `currentBrokerId`, `allowedBrokerIds`, cambio de broker y ausencia de broker cruzado. |
| Menu y shell | [menu-navigation-smoke-evidence.md](menu-navigation-smoke-evidence.md), [menu-route-parity-smoke-evidence.md](menu-route-parity-smoke-evidence.md), [responsive-shell-smoke-evidence.md](responsive-shell-smoke-evidence.md) | Validar menu lateral estatico, rutas visibles, estados de madurez y comportamiento movil. |
| Polizas | [polizas-mvp-readonly-evidence.md](polizas-mvp-readonly-evidence.md), [polizas-mvp-evidence.md](polizas-mvp-evidence.md), [polizas-crud-bbdd-evidence.md](polizas-crud-bbdd-evidence.md) | Validar listado, filtros, detalle, errores sanitizados, identificador SQL estable y ausencia de metadata runtime. |
| Paginas fixture de dominio | [domain-fixtures-smoke-evidence.md](domain-fixtures-smoke-evidence.md), [clientes-mvp-evidence.md](clientes-mvp-evidence.md), [siniestros-mvp-evidence.md](siniestros-mvp-evidence.md), [agenda-mvp-evidence.md](agenda-mvp-evidence.md), [propuestas-mvp-evidence.md](propuestas-mvp-evidence.md), [recibos-suplementos-mvp-evidence.md](recibos-suplementos-mvp-evidence.md), [liquidaciones-compania-mvp-evidence.md](liquidaciones-compania-mvp-evidence.md), [liquidaciones-colaborador-mvp-evidence.md](liquidaciones-colaborador-mvp-evidence.md) | Validar rutas protegidas read-only, fixtures locales, filtros locales y ausencia de llamadas backend. |
| Superficies bloqueadas | [blocked-technical-pages-smoke-evidence.md](blocked-technical-pages-smoke-evidence.md), [administracion-configuracion-mvp-evidence.md](administracion-configuracion-mvp-evidence.md), [conectividad-logs-byaunna-mvp-evidence.md](conectividad-logs-byaunna-mvp-evidence.md), [controles-estadisticas-mvp-evidence.md](controles-estadisticas-mvp-evidence.md), [informes-mvp-evidence.md](informes-mvp-evidence.md) | Validar que paginas sensibles siguen bloqueadas hasta SDD/API/permisos/UAT. |
| Autos Particulares | [autos-particulares-mvp-evidence.md](autos-particulares-mvp-evidence.md) | Mantener trazabilidad del incremento tecnico aparcado; no reactivarlo sin decision de producto, SDD, DBA/UAT y permisos. |
| Gobierno y cierre | [mvp-documentation-governance-review-2026-05-17.md](mvp-documentation-governance-review-2026-05-17.md), [autonomous-progress-2026-05-17.md](autonomous-progress-2026-05-17.md), [autonomous-progress-2026-05-18.md](autonomous-progress-2026-05-18.md) | Revisar convenciones de evidencia, riesgos residuales y siguientes pasos. |

## Comandos recomendados

Para cambios pequenos, ejecutar el subconjunto aplicable y documentar cualquier omision.

Backend:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
npm run test:e2e
```

Cierre amplio o PR de fase:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -RunFrontendE2E
```

Seguridad y documentacion:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
git diff --check
```

## Reglas de alcance

- Las paginas fixture no autorizan datos reales, nuevas APIs, escrituras, exportaciones ni permisos nuevos sin SDD/API/UAT.
- `Autos Particulares` sigue aparcado; no ampliarlo ni presentarlo como MVP vigente sin nueva decision funcional.
- `DemoSession`, API key MVP y headers MVP son compatibilidad local/demo, no auth productiva.
- Los smokes con `VITE_USE_BACKEND=false` protegen navegacion y ausencia de llamadas backend; no sustituyen UAT con datos autorizados.
- La metadata AppBuilder nunca debe ser contrato runtime para UI, permisos, queries, rutas o workflows.

## Bloqueos recurrentes

Clasificar pendientes con una sola etiqueta:

- `Completado con evidencia`: hay rutas, commits, comandos, tests o documento versionado.
- `Pendiente tecnico`: falta trabajo dentro del repo o del equipo tecnico.
- `Bloqueado externo`: depende de DBA, BBDD autorizada, secretos, proveedor auth, preview, UAT humano o decision de producto.

Bloqueos actuales que no debe resolver un agente por inferencia:

- auth productiva y matriz real de permisos;
- validacion SQL contra BBDD autorizada;
- UAT con responsable funcional;
- preview/despliegue real con environments y secrets;
- reactivacion de `Autos Particulares` o de cualquier pagina fixture como producto funcional.
