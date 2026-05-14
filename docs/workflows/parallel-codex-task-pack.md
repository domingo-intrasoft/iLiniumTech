# Paquete de tareas para agentes Codex paralelos

Este documento convierte el roadmap actual en tareas pequenas e independientes para agentes Codex. El objetivo grande concreto todavia debe sustituir el placeholder del prompt original. Hasta que se concrete, estas tareas se orientan al objetivo final documentado: avanzar el MVP de Polizas hacia una entrega profesional sin convertir iLiniumTech en un runtime dinamico tipo AppBuilder.

Este paquete no autoriza por si solo cambios de codigo de aplicacion. Cada tarea funcional debe estar respaldada por SDD, issue o decision documentada antes de implementarse.

## Reglas de coordinacion

- Un agente no debe tocar archivos fuera de su alcance.
- Si necesita tocar un archivo prohibido, debe parar y pedir coordinacion.
- No se deben revertir cambios ajenos.
- Las ramas de agentes deben usar prefijo `codex/`.
- Cada agente debe reportar comandos ejecutados, resultado, rutas tocadas, riesgos residuales y bloqueos.
- El coordinador integra resultados, ejecuta gates y prepara el cierre.

## Tarea 0 - Definicion SDD del objetivo grande

- **Nombre:** `producto-sdd-objetivo-grande`
- **Alcance exacto:** convertir el objetivo grande en una SDD ejecutable con contexto, alcance, fuera de alcance, criterios de aceptacion, seguridad, pruebas y DoD.
- **Archivos que puede tocar:** `docs/sdd/specs/iLiniumTech/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** objetivo funcional concreto del usuario; decision de prioridad frente a fases 3-8 del roadmap.
- **Criterios de aceptacion:** la SDD identifica que es producto iLiniumTech y que es solo evidencia AppBuilder; no pide runtime dinamico; define DoD y UAT.
- **Pruebas obligatorias:** `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`; `git diff --check`.
- **Documentacion a actualizar:** SDD nueva o existente; roadmap si cambia la fase activa.
- **Riesgo de conflicto con otras tareas:** bajo, salvo edicion concurrente de roadmap.

## Tarea 1 - Backend SQL read-only y contexto de polizas

- **Nombre:** `backend-polizas-sql-context`
- **Alcance exacto:** avanzar los criterios pendientes de `SDD-2026-003` sobre broker por request, fallback local, `SESSION_CONTEXT`, errores sanitizados, tests de whitelist e inyeccion.
- **Archivos que puede tocar:** `iLiniumTech.Backend/**`.
- **Archivos que NO debe tocar:** `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`, `docs/**` salvo una nota minima en `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md` si se completa un criterio.
- **Dependencias:** SDD-2026-003; disponibilidad de BBDD de test o fixture controlado; claves obligatorias de `SESSION_CONTEXT`.
- **Criterios de aceptacion:** SQL parametrizado; estructura SQL protegida por whitelist; headers MVP no se tratan como identidad; no se exponen secretos en errores/logs.
- **Pruebas obligatorias:** `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`; `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`; secret scan si toca configuracion.
- **Documentacion a actualizar:** README si cambia configuracion local; SDD-2026-003 si se cierran criterios.
- **Riesgo de conflicto con otras tareas:** medio con autenticacion/permisos y seguridad.

## Tarea 2 - Diseno de autenticacion y permisos producto

- **Nombre:** `security-auth-permissions-design`
- **Alcance exacto:** preparar una decision o SDD para sustituir cabeceras MVP por claims/sesion backend y modelar permisos por broker, perfil, oficina, gestor y usuario.
- **Archivos que puede tocar:** `docs/sdd/**`, `docs/engineering/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** decision humana de proveedor auth; reglas funcionales de permisos; relacion con entorno real.
- **Criterios de aceptacion:** flujo 401/403 definido; origen de broker/usuario/perfil definido; migracion desde headers MVP documentada; riesgos residuales claros.
- **Pruebas obligatorias:** validacion documental; secret scan si se documentan nombres de variables o entornos.
- **Documentacion a actualizar:** nueva SDD/ADR de auth; roadmap fase 5.
- **Riesgo de conflicto con otras tareas:** medio-alto con backend SQL si ambos cambian contrato de sesion.

## Tarea 3 - Frontend Polizas UX y contrato runtime

- **Nombre:** `frontend-polizas-ux-contract`
- **Alcance exacto:** estabilizar la pantalla `/polizas` y detalle como Vue estatico: estados, errores seguros, accesibilidad, runtime config, UAT visual y pruebas.
- **Archivos que puede tocar:** `iLiniumTech.Frontend/**`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `tools/**`, `.github/**`, `docs/**` salvo evidencia UAT si el coordinador lo pide.
- **Dependencias:** contrato API estable; codigos de error backend; decision sobre modo local/backend.
- **Criterios de aceptacion:** no consume metadata AppBuilder; cubre `loading`, `empty`, `error`, permisos/configuracion; errores no filtran SQL ni secretos.
- **Pruebas obligatorias:** `npm run format`; `npm run lint`; `npm run test:unit`; `npm run build`; smoke visual si cambia UI.
- **Documentacion a actualizar:** `docs/qa/definition-of-done.md` solo si cambia el flujo UAT esperado.
- **Riesgo de conflicto con otras tareas:** bajo-medio con backend si cambian DTOs o `/api/me`.

## Tarea 4 - Extractor offline de metadata sanitizada

- **Nombre:** `extractor-polizas-metadata-offline`
- **Alcance exacto:** implementar o especificar el extractor offline de `SDD-2026-002` para producir metadata sanitizada de Polizas sin dependencia runtime.
- **Archivos que puede tocar:** carpeta de herramienta nueva si se aprueba, `docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md`, `docs/MVP_POLIZAS_PLAN.md`.
- **Archivos que NO debe tocar:** endpoints productivos backend, runtime frontend, `iLiniumTech.Frontend/src/features/polizas/**`.
- **Dependencias:** contrato JSON del extractor; acceso read-only a `IL_Maestro` y `AunnaTechADM` o fixture local; politica de ubicacion de artefactos sanitizados.
- **Criterios de aceptacion:** salida sin secretos ni datos personales; no ejecuta `QueryStatic`; separa trazabilidad AppBuilder de sugerencias iLiniumTech.
- **Pruebas obligatorias:** tests de mapeo/redaccion/campos obligatorios; secret scan; validacion documental.
- **Documentacion a actualizar:** SDD-2026-002; README si se anade comando local.
- **Riesgo de conflicto con otras tareas:** bajo, salvo si comparte docs con producto/SDD.

## Tarea 5 - CI, preview y gates

- **Nombre:** `platform-ci-preview-gates`
- **Alcance exacto:** preparar la ruta hacia fase 7: checks required estables, artefactos, smoke preview, environments y runbook sin activar produccion.
- **Archivos que puede tocar:** `.github/**`, `tools/**`, `docs/PLAN_CICD_GITHUB_ONLY.md`, `docs/engineering/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/src/**`, `iLiniumTech.Frontend/src/**` salvo necesidad acordada para smoke.
- **Dependencias:** destino preview decidido; secrets/environments disponibles; nombres de checks estables en GitHub.
- **Criterios de aceptacion:** pipeline documentado; artefactos de tests y seguridad publicados; gates reproducibles localmente; no imprime secretos.
- **Pruebas obligatorias:** `.\tools\quality\Invoke-MvpQualityGate.ps1`; validacion documental; secret/dependency/CORS audit.
- **Documentacion a actualizar:** CI/CD docs; roadmap fase 7.
- **Riesgo de conflicto con otras tareas:** medio con seguridad/plataforma.

## Tarea 6 - Revision seguridad y privacidad

- **Nombre:** `security-privacy-review`
- **Alcance exacto:** revisar secretos, datos personales, logs, CORS, headers, errores publicos, SQL dinamico y exposicion de configuracion.
- **Archivos que puede tocar:** `tools/security/**`, `docs/engineering/01-seguridad-y-configuracion.md`, `docs/engineering/05-appbuilder-riesgos-y-resolucion.md`, `docs/sdd/templates/security-review-template.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**` o `iLiniumTech.Frontend/**` salvo hallazgo critico aprobado por coordinador.
- **Dependencias:** cambios tecnicos de backend/frontend/extractor listos o en PR.
- **Criterios de aceptacion:** findings documentados; secretos ausentes; riesgos residuales claros; mitigaciones propuestas o implementadas en tareas propietarias.
- **Pruebas obligatorias:** secret scan; dependency audit; CORS audit; `git diff --check`.
- **Documentacion a actualizar:** security review o engineering docs si aparecen riesgos nuevos.
- **Riesgo de conflicto con otras tareas:** medio; idealmente se ejecuta despues de cambios tecnicos.

## Tarea 7 - QA/UAT y cierre de fase

- **Nombre:** `qa-uat-phase-closure`
- **Alcance exacto:** preparar matriz de aceptacion, evidencias, pruebas manuales, riesgos residuales, bloqueos externos y estado de DoD para cierre de fase.
- **Archivos que puede tocar:** `docs/qa/**`, `docs/workflows/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** resultados de backend, frontend, extractor, CI y seguridad.
- **Criterios de aceptacion:** cada criterio Done tiene evidencia o bloqueo; UAT de `/polizas` definido; skips justificados; riesgos residuales escritos.
- **Pruebas obligatorias:** validacion documental; revision de comandos reportados por otros agentes.
- **Documentacion a actualizar:** DoD, roadmap e informe de cierre si se crea.
- **Riesgo de conflicto con otras tareas:** bajo; debe ir al final.

## Orden recomendado

1. Ejecutar Tarea 0 para concretar objetivo y SDD.
2. En paralelo: Tarea 1, Tarea 3, Tarea 4 y Tarea 5 si sus dependencias estan claras.
3. Ejecutar Tarea 2 como diseno si aun no hay proveedor auth.
4. Ejecutar Tarea 6 tras los cambios tecnicos.
5. Ejecutar Tarea 7 para cierre coordinado.

## Paquete minimo si el objetivo sigue sin concretar

Si el objetivo grande no se concreta, solo deben ejecutarse tareas documentales y de preparacion:

- Tarea 0.
- Tarea 2 en modo diseno.
- Tarea 5 en modo documentacion de preview.
- Tarea 7 en modo plantilla de cierre.

No deben iniciarse cambios de aplicacion sin SDD/issue especifico.
