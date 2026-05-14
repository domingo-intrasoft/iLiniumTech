# Evidencia QA/UAT - Autos Particulares MVP

Fecha: 2026-05-14

SDD base: `docs/sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md`.

## Alcance verificado

- Ruta frontend estatica `/autos-particulares`.
- API explicita del vertical:
  - `GET /api/autos-particulares/catalogs`
  - `GET /api/autos-particulares/polizas`
  - `GET /api/autos-particulares/polizas/{id}`
- Alcance backend fijo a ramo `Autos`.
- `scope` expone `divisionObjetivo=Particulares`, `divisionFiltroAplicado=false` y `divisionPendienteUat=true`.
- UI y fixtures no exponen matricula completa, bastidor, documento legal, telefono, email ni direccion.
- No hay runtime dinamico basado en metadata AppBuilder.

## Evidencia automatica

Gate completo ejecutado:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\Users\DomingoCabezaGuerra\.cache\codex-runtimes\codex-primary-runtime\dependencies\node\bin\node.exe" -RunFrontendE2E
```

Resultado:

- Backend build: OK.
- Backend tests: 67/67 OK.
- Frontend format: OK.
- Frontend lint: OK.
- Frontend unit tests: 54/54 OK.
- Frontend build/typecheck: OK.
- Playwright E2E Chromium: 2/2 OK, incluyendo `/autos-particulares`.
- Backend HTTP smoke: OK.
- Frontend smoke: OK.
- Secret scan: sin leaks.
- Dependency audit: 0 findings.
- CORS audit: sin findings.
- Extractor tests: OK.
- Documentation baseline: OK.
- Git whitespace check: OK.

## Bloqueos externos

- Falta confirmar con DBA/producto el campo, vista o regla autorizada para filtrar `division=Particulares` en BBDD real.
- Falta UAT contra entorno autorizado para cerrar que todos los registros visibles pertenecen a Autos Particulares y no solo a ramo Autos.
- Falta auth/autorizacion real para permisos productivos `autosParticulares.*`.

## Riesgos residuales

- Mientras `divisionFiltroAplicado=false`, el vertical no debe comunicarse como cierre funcional completo de Particulares.
- `vehiculoResumen` queda preparado en frontend/SDD, pero el SQL real no debe rellenarlo hasta tener whitelist revisada que evite matricula y bastidor completos.
- El detalle del vertical se sanitiza por API, pero debe revisarse de nuevo cuando se incorporen campos reales de vehiculo.
