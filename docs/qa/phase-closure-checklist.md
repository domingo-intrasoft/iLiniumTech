# Checklist de cierre de fase QA/UAT

Plantilla para cerrar una fase o PR relevante de iLiniumTech sin mezclar evidencia real con supuestos. Usar junto con `docs/qa/definition-of-done.md`, `PLANS.md`, SDD aplicable y `docs/ROADMAP_OBJETIVO_FINAL.md`.

## Identificacion

- Fase o PR:
- Rama:
- SDD / issue / decision:
- Responsable de cierre:
- Fecha:
- Alcance incluido:
- Fuera de alcance confirmado:

## Estado ejecutivo

Clasificar cada punto abierto con una sola etiqueta:

- `Completado con evidencia`: implementado y respaldado por rutas, tests, comandos, logs sanitizados o documentos versionados.
- `Pendiente tecnico`: falta trabajo dentro del repo o del equipo tecnico y no depende de un tercero.
- `Bloqueado externo`: depende de BBDD, secretos, cuentas, preview, UAT humano, proveedor auth, DBA o aprobacion fuera del repo.

Resumen:

| Area | Estado | Evidencia / motivo | Siguiente paso | Responsable |
| --- | --- | --- | --- | --- |
| Producto / SDD |  |  |  |  |
| Backend / datos |  |  |  |  |
| Frontend / UX |  |  |  |  |
| Extractor / metadata offline |  |  |  |  |
| CI / gate local |  |  |  |  |
| Seguridad / privacidad |  |  |  |  |
| UAT |  |  |  |  |

## Evidencia Done

- [ ] `git status --short --branch` revisado y cambios ajenos respetados.
- [ ] Rutas tocadas listadas.
- [ ] SDD, issue o decision enlazada.
- [ ] No se tocaron rutas fuera de alcance.
- [ ] No se introdujeron secretos, connection strings reales, dumps, capturas sensibles ni datos personales reales.
- [ ] No se introdujo runtime dinamico basado en metadata AppBuilder.
- [ ] Roadmap/SDD actualizados si cambio estado, contrato, configuracion, fase o bloqueo.
- [ ] Riesgos residuales documentados.
- [ ] Bloqueos externos documentados con responsable o dependencia.

## Evidencia de pruebas

Marcar solo lo ejecutado en este cierre. Si no aplica o no se pudo ejecutar, escribir motivo.

| Comando / prueba | Resultado | Artefacto o evidencia | Motivo si no ejecutado |
| --- | --- | --- | --- |
| `dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx` |  |  |  |
| `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release` |  |  |  |
| `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release` |  |  |  |
| `npm run format` |  |  |  |
| `npm run lint` |  |  |  |
| `npm run test:unit` |  |  |  |
| `npm run build` |  |  |  |
| `.\tools\quality\Invoke-MvpQualityGate.ps1` |  |  |  |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1` |  |  |  |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1` |  |  |  |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings` |  |  |  |
| `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings` |  |  |  |
| `git diff --check` |  |  |  |
| Smoke backend/API |  |  |  |
| Smoke frontend/UI |  |  |  |

## UAT minimo Polizas

- [ ] Entorno usado identificado: fixture local, backend `InMemory`, SQL local autorizado o preview.
- [ ] `/polizas` carga o muestra error de configuracion comprensible.
- [ ] La UI no consulta backend si falta API key o contexto de broker requerido.
- [ ] Filtros principales funcionan segun contrato vigente.
- [ ] Paginacion funciona.
- [ ] Detalle read-only abre correctamente.
- [ ] Errores no muestran SQL, connection strings, trazas internas ni secretos.
- [ ] No se renderiza UI desde metadata AppBuilder.
- [ ] Evidencia visual o descripcion verificable adjunta si cambio la UI.
- [ ] Aceptacion funcional registrada o incidencias abiertas.

## Cierre PR

La PR o resumen final debe incluir:

- Resumen del cambio.
- SDD/issue/decision enlazada.
- Rutas principales tocadas.
- Comandos ejecutados y resultado.
- Pruebas no ejecutadas y motivo.
- Impacto de seguridad/configuracion.
- Confirmacion de ausencia de secretos y datos reales.
- Riesgos residuales.
- Bloqueos externos.
- Evidencia UAT o smoke visual si cambia UI.

No marcar listo si hay secretos, falla un gate aplicable sin explicacion, falta SDD para cambio funcional relevante, se reintroduce runtime AppBuilder, hay SQL no parametrizado nuevo o errores publicos no sanitizados.
