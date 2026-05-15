# Paquete de tareas para agentes Codex paralelos

Este documento convierte el objetivo vigente en tareas pequenas e independientes para agentes Codex. El paquete anterior orientado a `Autos Particulares` queda aparcado: no debe ejecutarse como objetivo activo salvo nueva confirmacion funcional, SDD actualizada y UAT.

Objetivo grande activo: consolidar el MVP actual con login, shell/menu lateral y pantalla de Polizas read-only, preparando auth productiva, multi-tenant/broker, datos reales controlados y QA/preview profesional, sin convertir iLiniumTech en un runtime dinamico tipo AppBuilder.

Este paquete no autoriza por si solo cambios de codigo de aplicacion. Cada tarea funcional debe estar respaldada por SDD, issue o decision documentada antes de implementarse.

## Reglas de coordinacion

- Un agente no debe tocar archivos fuera de su alcance.
- Si necesita tocar un archivo prohibido, debe parar y pedir coordinacion.
- No se deben revertir cambios ajenos.
- Las ramas de agentes deben usar prefijo `codex/`.
- Cada agente debe reportar comandos ejecutados, resultado, rutas tocadas, riesgos residuales y bloqueos.
- El coordinador integra resultados, ejecuta gates y prepara el cierre.

## Paquete vigente - MVP login, menu y Polizas

### Tarea A - Producto, SDD y plan maestro

- **Nombre:** `producto-plan-maestro-mvp-polizas`
- **Alcance exacto:** mantener alineados `docs/PLAN_MAESTRO_IA.md`, roadmap, SDD y objetivo vigente. Registrar objetivos aparcados y preguntas obligatorias.
- **Archivos que puede tocar:** `docs/**`, `AGENTS.md`, `PLANS.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** decisiones de producto y UAT.
- **Criterios de aceptacion:** el objetivo vigente queda claro; Autos Particulares no aparece como fase activa; cada pendiente se clasifica como tecnico o bloqueo externo.
- **Pruebas obligatorias:** validacion documental; `git diff --check`; secret scan si se documentan entornos.
- **Documentacion a actualizar:** plan maestro, roadmap, SDD afectada.
- **Riesgo de conflicto:** bajo, salvo edicion concurrente de docs canonicos.

### Tarea B - Login y sesion

- **Nombre:** `frontend-backend-login-session-hardening`
- **Alcance exacto:** endurecer `/login`, `DemoSession`, `/api/me`, guard de rutas, logout, refresh, 401/403 y estados de sesion.
- **Archivos que puede tocar:** `iLiniumTech.Frontend/**`, `iLiniumTech.Backend/**` solo en auth/session, `docs/qa/**` si aporta evidencia.
- **Archivos que NO debe tocar:** extractor, SQL real, `.github/**` salvo coordinacion.
- **Dependencias:** contrato `/api/me` y decision pendiente de auth productiva.
- **Criterios de aceptacion:** `/login` queda fuera del shell; rutas protegidas redirigen bien; logout limpia sesion; errores son sanitizados.
- **Pruebas obligatorias:** tests backend auth; tests frontend auth/guard; smoke login -> polizas -> logout; secret scan si toca configuracion.
- **Documentacion a actualizar:** evidencia QA y roadmap si cambia estado.
- **Riesgo de conflicto:** medio con permisos y shell.

### Tarea C - Shell, menu lateral y broker UX

- **Nombre:** `frontend-shell-side-menu-broker`
- **Alcance exacto:** consolidar shell estatico, menu lateral, visibilidad por permisos iLiniumTech y estados de broker/permisos.
- **Archivos que puede tocar:** `iLiniumTech.Frontend/**`, `docs/appbuilder/**` si documenta analisis, `docs/qa/**`.
- **Archivos que NO debe tocar:** backend salvo contrato coordinado; metadata/extractor productivo.
- **Dependencias:** `/api/me`, permisos efectivos, futuro endpoint de cambio de broker.
- **Criterios de aceptacion:** menu en codigo fuente; no consume metadata AppBuilder; usuario ve capacidades coherentes con permisos; responsive sin solapes.
- **Pruebas obligatorias:** format/lint/unit/build frontend; smoke visual desktop/mobile si cambia UI.
- **Documentacion a actualizar:** analisis de menu y evidencia visual si aplica.
- **Riesgo de conflicto:** medio con Polizas si se extraen componentes compartidos.

### Tarea D - Polizas listado y detalle profesional

- **Nombre:** `frontend-backend-polizas-readonly-quality`
- **Alcance exacto:** mejorar listado, filtros, busqueda, paginacion, detalle, errores, empty/loading/no-permission y contrato API read-only.
- **Archivos que puede tocar:** `iLiniumTech.Frontend/**`, `iLiniumTech.Backend/**` en Polizas, `docs/qa/**`.
- **Archivos que NO debe tocar:** auth provider real salvo coordinacion; Autos Particulares salvo correccion menor de regresion.
- **Dependencias:** politicas `polizas.*`, contrato de DTOs, UAT de columnas/filtros.
- **Criterios de aceptacion:** filtros y detalle funcionan sin metadata runtime; no se revelan datos sensibles; 403/404 no filtran existencia cruzada.
- **Pruebas obligatorias:** backend build/test; frontend format/lint/unit/build; smoke `/polizas` y detalle.
- **Documentacion a actualizar:** evidencia QA y SDD si cambia contrato.
- **Riesgo de conflicto:** medio con login/permisos y componentes compartidos.

### Tarea E - Auth productiva, permisos y broker seguro

- **Nombre:** `backend-auth-permissions-broker-design-implementation`
- **Alcance exacto:** decidir proveedor auth, modelar contexto interno, retirar headers/API key de entornos reales y validar broker permitido antes de datos.
- **Archivos que puede tocar:** `iLiniumTech.Backend/**`, `docs/sdd/**`, `docs/engineering/**`.
- **Archivos que NO debe tocar:** frontend salvo contrato pactado; extractor; workflows de deploy salvo coordinacion.
- **Dependencias:** decision humana de proveedor auth y matriz de permisos.
- **Criterios de aceptacion:** identidad viene de claims/sesion validada; API key no concede permisos; broker activo pertenece a `allowedBrokerIds`.
- **Pruebas obligatorias:** 401/403, token invalido, broker cruzado, permiso ausente, logs sanitizados.
- **Documentacion a actualizar:** SDD auth, roadmap, README si cambia ejecucion local.
- **Riesgo de conflicto:** alto; requiere coordinacion con frontend y SQL.

### Tarea F - Datos reales, SQL y DBA

- **Nombre:** `backend-sql-real-readonly-dba`
- **Alcance exacto:** validar resolver maestro, conexion modelo, whitelists SQL, `SESSION_CONTEXT`, PII y repositorio Polizas con entorno autorizado.
- **Archivos que puede tocar:** `iLiniumTech.Backend/**`, `docs/sdd/**`, `docs/qa/**`.
- **Archivos que NO debe tocar:** frontend salvo DTO coordinado; archivos con secretos; artefactos de datos reales.
- **Dependencias:** DBA, BBDD test/UAT, cuentas read-only, claves de contexto.
- **Criterios de aceptacion:** no se consulta SQL antes de broker/permisos; no se loguean secretos; datos sensibles minimizados.
- **Pruebas obligatorias:** backend build/test; tests de payloads maliciosos; integracion SQL si hay entorno; secret scan.
- **Documentacion a actualizar:** evidencia DBA/UAT, riesgos y bloqueos externos.
- **Riesgo de conflicto:** alto con auth/broker.

### Tarea G - QA, CI, seguridad y preview

- **Nombre:** `qa-ci-security-preview-readiness`
- **Alcance exacto:** mantener gates, CI, security, CodeQL, preview dry-run, evidencia de cierre y DoD.
- **Archivos que puede tocar:** `.github/**`, `tools/**`, `docs/qa/**`, `docs/engineering/**`, `docs/PLAN_CICD_GITHUB_ONLY.md`.
- **Archivos que NO debe tocar:** runtime backend/frontend salvo correccion pactada de smoke.
- **Dependencias:** destino preview, environments, secrets y checks verdes.
- **Criterios de aceptacion:** gates reproducibles; artifacts revisables; no secrets; preview real bloqueado hasta environment protegido.
- **Pruebas obligatorias:** gate local aplicable; validacion documental; secret/dependency/CORS audit; `git diff --check`.
- **Documentacion a actualizar:** DoD, roadmap, plan CI/CD, evidencias de cierre.
- **Riesgo de conflicto:** medio con workflows y seguridad.

## Paquete historico aparcado - Autos Particulares

Las tareas siguientes pertenecen al objetivo anterior `Autos Particulares`. Se conservan por trazabilidad, pero no son el plan activo. Solo deben reactivarse si producto lo confirma explicitamente y se actualiza `SDD-2026-006`.

## Tarea 0 - Definicion SDD del objetivo grande

- **Nombre:** `producto-sdd-objetivo-grande`
- **Alcance exacto:** mantener `SDD-2026-006 Autos Particulares MVP read-only` como SDD ejecutable con contexto, alcance, fuera de alcance, criterios de aceptacion, seguridad, pruebas, fallback de division y DoD.
- **Archivos que puede tocar:** `docs/sdd/specs/iLiniumTech/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** decision de prioridad frente a fases 3-8 del roadmap; confirmacion DBA/UAT para division `Particulares`.
- **Criterios de aceptacion:** la SDD identifica que `Autos Particulares` es producto iLiniumTech; no pide runtime dinamico; define `/autos-particulares`, API propia bajo `/api/autos-particulares`, DoD, UAT y fallback controlado si la division no puede confirmarse en listado.
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

## Tarea 3B - Frontend Autos Particulares read-only

- **Nombre:** `frontend-autos-particulares-readonly`
- **Alcance exacto:** implementar la ruta `/autos-particulares` como vertical Vue estatico read-only, con listado, detalle, filtros, estados de configuracion/permisos y consumo de API propia.
- **Archivos que puede tocar:** `iLiniumTech.Frontend/**`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `.github/**`, `tools/**`, `docs/**` salvo evidencia UAT si el coordinador lo pide.
- **Dependencias:** `SDD-2026-006`; contrato backend `/api/autos-particulares/polizas`; decision sobre `scope.divisionPendienteUat`.
- **Criterios de aceptacion:** no consume metadata AppBuilder; usa ruta `/autos-particulares`; muestra vertical `Autos Particulares`; cubre `loading`, `empty`, `error`, configuracion incompleta y acceso denegado; no expone documento, matricula completa ni bastidor completo.
- **Pruebas obligatorias:** `npm run format`; `npm run lint`; `npm run test:unit`; `npm run build`; smoke visual si cambia UI.
- **Documentacion a actualizar:** normalmente ninguna; registrar evidencia visual en PR o cierre.
- **Riesgo de conflicto con otras tareas:** medio con backend Autos si el DTO cambia; bajo con Polizas si se extraen componentes compartidos con cambios pequenos.

## Tarea 3C - Backend Autos Particulares API

- **Nombre:** `backend-autos-particulares-api`
- **Alcance exacto:** implementar endpoints propios bajo `/api/autos-particulares`: `GET /api/autos-particulares/catalogs`, `GET /api/autos-particulares/polizas` y `GET /api/autos-particulares/polizas/{id}`, con scope ramo Autos y division Particulares cuando el dato exista.
- **Archivos que puede tocar:** `iLiniumTech.Backend/**`.
- **Archivos que NO debe tocar:** `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`, `docs/**` salvo nota minima en `SDD-2026-006` si cambia un criterio documentado.
- **Dependencias:** `SDD-2026-006`; patrones de `SDD-2026-003`; confirmacion DBA para campo/regla de division; `SDD-2026-005` para permisos futuros.
- **Criterios de aceptacion:** API propia y explicita; filtros/sort por whitelist; SQL parametrizado si aplica; ramo Autos fijado por backend; fallback controlado si division no se confirma; errores sanitizados con `correlationId` cuando aplique; sin metadata runtime.
- **Pruebas obligatorias:** `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`; `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`; secret scan si toca configuracion; CORS audit si toca API/config.
- **Documentacion a actualizar:** SDD-2026-006 solo si se cierra o cambia un criterio; README si se introduce configuracion local nueva.
- **Riesgo de conflicto con otras tareas:** medio con backend Polizas si se comparten repositorios/DTOs; medio con auth si se introducen permisos.

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
- **Alcance exacto:** preparar matriz de aceptacion, evidencias, pruebas manuales, riesgos residuales, bloqueos externos y estado de DoD para cierre de fase. Usar `docs/qa/phase-closure-checklist.md` como plantilla operativa.
- **Archivos que puede tocar:** `docs/qa/**`, `docs/workflows/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** resultados de backend, frontend, extractor, CI y seguridad.
- **Criterios de aceptacion:** cada criterio Done tiene evidencia o bloqueo; UAT de `/polizas` definido; skips justificados; riesgos residuales escritos; los puntos abiertos estan clasificados como `completado con evidencia`, `pendiente tecnico` o `bloqueado externo`.
- **Pruebas obligatorias:** validacion documental; revision de comandos reportados por otros agentes.
- **Documentacion a actualizar:** DoD, roadmap e informe de cierre si se crea.
- **Riesgo de conflicto con otras tareas:** bajo; debe ir al final.

## Tarea 8 - QA/UAT Autos Particulares

- **Nombre:** `qa-uat-autos-particulares`
- **Alcance exacto:** preparar matriz de aceptacion para `SDD-2026-006`, especialmente ramo Autos, division Particulares, fallback controlado, minimizacion PII y evidencia visual/API.
- **Archivos que puede tocar:** `docs/qa/**`, `docs/workflows/**`, `docs/ROADMAP_OBJETIVO_FINAL.md`, `docs/sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Backend/**`, `iLiniumTech.Frontend/**`, `.github/**`, `tools/**`.
- **Dependencias:** resultados de backend/frontend Autos; acceso a entorno autorizado o confirmacion de bloqueo externo por DBA/producto.
- **Criterios de aceptacion:** UAT distingue `ramo Autos confirmado`, `division Particulares confirmada` y `division pendiente`; toda exposicion de documento, matricula, bastidor, telefono, email o direccion queda ausente, enmascarada o justificada por SDD futura.
- **Pruebas obligatorias:** validacion documental; revision de `git diff --check`; revision de comandos reportados por backend/frontend.
- **Documentacion a actualizar:** checklist de fase o SDD-2026-006 con evidencia/bloqueos.
- **Riesgo de conflicto con otras tareas:** bajo; debe ejecutarse despues de cambios tecnicos.

## Orden historico para Autos si se reactiva

No usar este orden como plan activo. El orden vigente esta en `docs/PLAN_MAESTRO_IA.md` y en el paquete vigente de este documento.

1. Ejecutar Tarea 0 para mantener `SDD-2026-006` y roadmap alineados.
2. En paralelo para Autos Particulares: Tarea 3B frontend y Tarea 3C backend, manteniendo contrato de SDD.
3. En paralelo para la base existente: Tarea 1, Tarea 3, Tarea 4 y Tarea 5 si sus dependencias estan claras.
4. Ejecutar Tarea 2 como diseno si aun no hay proveedor auth.
5. Ejecutar Tarea 6 tras los cambios tecnicos.
6. Ejecutar Tarea 8 para UAT especifico de Autos Particulares.
7. Ejecutar Tarea 7 para cierre coordinado.

## Paquete minimo si el objetivo sigue sin concretar

Si un objetivo futuro no se concreta, solo deben ejecutarse tareas documentales y de preparacion:

- Tarea 0.
- Tarea 2 en modo diseno.
- Tarea 5 en modo documentacion de preview.
- Tarea 7 en modo plantilla de cierre.

No deben iniciarse cambios de aplicacion sin SDD/issue especifico.
