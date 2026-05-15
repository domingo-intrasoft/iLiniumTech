# Flujo de trabajo con IA

Este flujo define como deben trabajar agentes IA y colaboradores humanos en iLiniumTech. Complementa `AGENTS.md`, `PLANS.md`, `docs/PLAN_MAESTRO_IA.md`, el roadmap y las plantillas SDD.

Para repartir trabajo entre agentes paralelos, usar tambien `docs/workflows/parallel-codex-task-pack.md`.

## 1. Entrada de trabajo

Todo cambio relevante debe partir de una de estas entradas:

- SDD en `docs/sdd/specs/iLiniumTech`;
- issue GitHub con contexto funcional;
- decision tecnica documentada;
- tarea de mantenimiento acotada;
- correccion de bug con evidencia reproducible.

Si el cambio afecta comportamiento de producto, datos, seguridad, permisos o UX visible, debe existir SDD o decision enlazada antes de marcar Done.

## 2. Preparacion

1. Revisar estado:

   ```powershell
   git status --short --branch
   ```

2. Leer documentos base:

   - `README.md`
   - `AGENTS.md`
   - `PLANS.md`
   - `docs/PLAN_MAESTRO_IA.md`
   - `docs/ROADMAP_OBJETIVO_FINAL.md`
   - `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
   - SDD relacionada

3. Identificar alcance:

   - backend;
   - frontend;
   - documentacion;
   - CI/gates;
   - seguridad;
   - entorno real o BBDD.

4. Crear o actualizar plan usando `PLANS.md`.

## 3. Ramas

- No trabajar directamente en `main`.
- Para agentes IA usar prefijo `codex/`.
- Nombre recomendado: `codex/<area>-<descripcion>`.
- Mantener una rama por objetivo coherente.
- Si aparecen cambios ajenos, no revertirlos; ajustar el plan o coordinar.

## 4. Desarrollo

Reglas durante la implementacion:

- Cambiar solo lo necesario.
- Mantener la arquitectura Vue estatica + API explicita.
- No introducir runtime dinamico de metadata AppBuilder.
- No introducir secretos, datos personales reales ni connection strings.
- Mantener fixtures anonimizados.
- Parametrizar valores SQL y usar whitelists para estructura.
- Sanitizar errores expuestos al usuario.
- Documentar cambios de contrato, configuracion o seguridad.

Propiedad recomendada por area:

- Backend/API/datos: `iLiniumTech.Backend/**`.
- Frontend/UX: `iLiniumTech.Frontend/**`.
- Calidad/CI/docs: `docs/**`, `tools/**`, `.github/**`, `README.md`.
- SDD/UAT: `docs/sdd/**`.

## 5. Build y tests

Backend:

```powershell
dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run format
npm run lint
npm run test:unit
npm run build
```

Gate local:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Si Node global no cumple `>=20.19.0`, usar:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1 -NodeExe "C:\ruta\a\node.exe"
```

Validacion documental:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

## 6. Seguridad

Ejecutar cuando cierre una rama o cuando el cambio toque dependencias, configuracion, backend, CI o documentacion sensible:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

Un secreto real detectado bloquea siempre el cierre.

## 7. PR

La PR debe usar `.github/pull_request_template.md` y declarar:

- resumen;
- SDD/issue;
- pruebas ejecutadas;
- seguridad;
- riesgos residuales;
- evidencia visual si cambia UI;
- pruebas omitidas y motivo.

La PR debe ser pequena y revisable. Si el cambio mezcla varias areas, explicar por que pertenecen al mismo objetivo.

## 8. Evidencia antes de Done

Antes de marcar Done se necesita:

- rama actual y `git status` limpio o explicado;
- rutas tocadas;
- comandos ejecutados con resultado;
- build y tests aplicables;
- auditorias aplicables;
- smoke visual/API si cambia runtime visible;
- documentacion actualizada si cambian contratos o configuracion;
- riesgos residuales;
- bloqueos externos identificados.

## 9. Cierre

1. Revisar diff:

   ```powershell
   git diff --stat
   git diff --check
   ```

2. Confirmar que no hay artefactos temporales versionados:

   - `TestResults`;
   - `reports`;
   - `quality-reports`;
   - `dist`;
   - logs locales.

3. Preparar resumen final:

   - que cambio;
   - por que;
   - como se valido;
   - riesgos residuales;
   - siguiente paso.

4. Si el cierre afecta una fase, UAT o PR con varias areas, completar `docs/qa/phase-closure-checklist.md` o copiar sus apartados al cuerpo de PR/cierre. Cada pendiente debe quedar clasificado como `completado con evidencia`, `pendiente tecnico` o `bloqueado externo`.
