# Paquete de tareas para agentes Codex paralelos

Este documento convierte el objetivo vigente en tareas pequenas e independientes para agentes Codex. El paquete anterior orientado a `Autos Particulares` queda aparcado: no debe ejecutarse como objetivo activo salvo nueva confirmacion funcional, SDD actualizada y UAT.

Objetivo grande activo: con `Polizas CRUD BBDD` cerrado como MVP local de referencia, crear y coordinar agentes por pagina del menu para evolucionar el resto de superficies con SDD propia, sin activar datos reales, APIs nuevas, escrituras, exportaciones ni permisos nuevos hasta contrato equivalente.

Este paquete no autoriza por si solo cambios de codigo de aplicacion. Cada tarea funcional debe estar respaldada por SDD, issue o decision documentada antes de implementarse.

Estado de coordinacion 2026-05-18: el carril de paginas del menu vuelve a ser el paquete activo, pero en modo controlado por SDD. Las paginas fixture pueden recibir hardening estatico y pruebas; cualquier dato real o API pasa antes por SDD, permisos, UAT/DBA y revision de seguridad si aplica. El detalle operativo esta en [`../appbuilder/pages/page-agent-rollout.md`](../appbuilder/pages/page-agent-rollout.md).

## Reglas de coordinacion

- Un agente no debe tocar archivos fuera de su alcance.
- Si necesita tocar un archivo prohibido, debe parar y pedir coordinacion.
- No se deben revertir cambios ajenos.
- Las ramas de agentes deben usar prefijo `codex/`.
- Cada agente debe reportar comandos ejecutados, resultado, rutas tocadas, riesgos residuales y bloqueos.
- El coordinador integra resultados, ejecuta gates y prepara el cierre.

## Paquete vigente - agentes por pagina del menu

### Tarea M0 - Coordinacion de rollout por pagina

- **Nombre:** `producto-page-agent-rollout`
- **Alcance exacto:** mantener el mapa de agentes por pagina, grupos de coordinacion, orden recomendado, carriles permitidos y bloqueos por SDD.
- **Archivos que puede tocar:** `docs/appbuilder/pages/page-agent-rollout.md`, `docs/appbuilder/pages/development-readiness.md`, `docs/appbuilder/pages/README.md`, `docs/workflows/parallel-codex-task-pack.md`.
- **Archivos que NO debe tocar:** `iLiniumTech.Frontend/**`, `iLiniumTech.Backend/**`, `.github/**`, `tools/**`.
- **Dependencias:** estado real de `appNavigation.ts`, `router/index.ts` y evidencias QA existentes.
- **Criterios de aceptacion:** cada pagina queda asignada a un jefe, con alcance permitido, SDD requerida, archivos permitidos/prohibidos, pruebas y riesgos.
- **Pruebas obligatorias:** validacion documental, `git diff --check`, secret scan si se mencionan entornos o configuracion.
- **Documentacion a actualizar:** rollout por pagina y readiness.
- **Riesgo de conflicto:** bajo; coordinar si otro agente edita docs canonicos.

### Tarea M1 - Negocio diario

- **Nombre:** `page-agents-business-daily`
- **Alcance exacto:** analizar y preparar tareas para `Agenda`, `Clientes` y `Propuestas`, manteniendo fixtures seguros y proponiendo SDD para datos/API.
- **Archivos que puede tocar:** `docs/appbuilder/pages/agenda/**`, `docs/appbuilder/pages/clientes/**`, `docs/appbuilder/pages/propuestas/**`, `docs/qa/**`; solo si se aprueba UI, `iLiniumTech.Frontend/src/features/agenda/**`, `clientes/**`, `propuestas/**`.
- **Archivos que NO debe tocar:** backend, servicios API, router, navegacion, package files, extractor y datos reales.
- **Dependencias:** UAT/producto para campos, permisos y origenes de datos.
- **Criterios de aceptacion:** PII y workflows quedan bloqueados; las tareas futuras distinguen placeholder de API real. Para `Clientes`, el primer contrato documental es [`SDD-2026-010`](../sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md), limitado a listado read-only minimizado sin ficha ni PII ampliada.
- **Pruebas obligatorias:** frontend format/lint/unit/build si toca UI; validacion documental si toca docs.
- **Documentacion a actualizar:** README de pagina, evidencia QA y SDD futura si procede.
- **Riesgo de conflicto:** medio si varios agentes tocan componentes fixture al mismo tiempo.

### Tarea M2 - Operativa seguros

- **Nombre:** `page-agents-insurance-operations`
- **Alcance exacto:** analizar y preparar tareas para `Recibos`, `Suplementos`, `Siniestros`, `Polizas / Flotas` y `Polizas / Colectivas`.
- **Archivos que puede tocar:** docs de esas paginas, `docs/qa/**`; solo con encargo frontend, sus carpetas `src/features/**` correspondientes.
- **Archivos que NO debe tocar:** backend/API, servicios, SQL, router, navegacion y metadata runtime sin SDD.
- **Dependencias:** SDD, permisos, DBA/UAT y privacy review para datos reales o PII.
- **Criterios de aceptacion:** no se infieren reglas funcionales desde nombres de menu; cada API futura queda separada por contrato. Para `Siniestros`, el primer contrato documental es [`SDD-2026-008`](../sdd/specs/iLiniumTech/SDD-2026-008-siniestros-read-only.md), limitado a listado read-only minimizado. Para `Recibos`, el primer contrato documental es [`SDD-2026-009`](../sdd/specs/iLiniumTech/SDD-2026-009-recibos-read-only.md), limitado a listado read-only minimizado sin importes reales.
- **Pruebas obligatorias:** frontend completo si UI; security/privacy review antes de API real.
- **Documentacion a actualizar:** SDD candidates y README de pagina.
- **Riesgo de conflicto:** medio con Polizas si se intenta compartir contratos o scopes.

### Tarea M3 - Reporting y liquidaciones

- **Nombre:** `page-agents-reporting-finance`
- **Alcance exacto:** analizar y preparar tareas para `Liq.Cia`, `Liq.Col`, `Informes` y `Estadisticas`.
- **Archivos que puede tocar:** docs de esas paginas, `docs/qa/**`; carpetas frontend correspondientes solo para hardening fixture.
- **Archivos que NO debe tocar:** backend, SQL financiero, servicios de descarga/exportacion, cache real, router/navegacion.
- **Dependencias:** owner UAT, permisos, minimizacion financiera/PII y definicion de exportaciones.
- **Criterios de aceptacion:** importes, comisiones, agregados y exportaciones siguen bloqueados hasta SDD/API.
- **Pruebas obligatorias:** frontend completo si UI; threat/privacy review para descargas, reporting o agregados.
- **Documentacion a actualizar:** inventario funcional, SDD futura y evidencia QA.
- **Riesgo de conflicto:** bajo-medio; alto si se conectan datos o exportaciones sin contrato.

### Tarea M4 - Superficies tecnicas y admin

- **Nombre:** `page-agents-technical-admin-blocked`
- **Alcance exacto:** analizar y mantener bloqueadas `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna` y `Logs`.
- **Archivos que puede tocar:** docs de esas paginas, `docs/qa/**`; carpetas frontend correspondientes solo para copy/tests de bloqueo.
- **Archivos que NO debe tocar:** backend, configuracion real, conectores, logs reales, workflows, router/navegacion, package files.
- **Dependencias:** SDD, threat model, security review y decision de arquitectura.
- **Criterios de aceptacion:** no se reintroduce Builder, no se muestran secretos/logs/payloads, no hay acciones operativas.
- **Pruebas obligatorias:** frontend completo si UI; secret scan siempre que se documente configuracion, logs, conectividad o entornos.
- **Documentacion a actualizar:** SDD/security backlog y evidencias de bloqueo.
- **Riesgo de conflicto:** bajo si permanece fixture; critico si se habilitan acciones reales sin SDD.

## Paquete de referencia - MVP login, menu y Polizas

Este paquete se conserva como referencia tecnica. No desplaza la prioridad activa de agentes por pagina.

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
- **Criterios de aceptacion:** `/login` queda fuera del shell; rutas protegidas redirigen bien; `/api/me` es fuente unica de identidad/broker/permisos; refresh y expiracion son consistentes; logout limpia sesion; 401/403/404 son sanitizados; no hay fallback silencioso a fixtures en modo backend real.
- **Pruebas obligatorias:** tests backend auth/session/permisos; tests frontend auth/guard/composables; smoke login -> polizas -> detalle -> logout; secret scan si toca configuracion.
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

## Paquete operativo aparcado - paginas estaticas del menu

Este paquete queda aparcado como referencia porque las pantallas estaticas trabajadas ya tienen evidencias QA en `docs/qa/*-mvp-evidence.md`. No autoriza programar backend, crear APIs, tocar paquetes, cambiar router/navegacion ni activar datos reales. Solo debe retomarse para regresiones, ajustes documentales o si producto decide enriquecer nuevas paginas estaticas.

### Regla de carriles

- **Carril A - Desarrollable ahora con fixture/estatico:** solo pagina Vue protegida, contenido especifico de modulo, acciones deshabilitadas, tests de vista/ruta y trazabilidad documental. No hay datos reales ni contratos nuevos.
- **Carril B - Requiere contrato backend/SDD:** cualquier dato real, filtro funcional, catalogo, detalle, exportacion, importes, PII, logs operativos, workflow, escritura o permiso nuevo.

### Tarea H - Coordinacion de paginas estaticas

- **Nombre:** `producto-menu-static-pages-readiness`
- **Alcance exacto:** mantener la matriz operativa de paginas, priorizar carriles y resolver dudas de alcance antes de que agentes frontend toquen UI.
- **Archivos que puede tocar:** `docs/appbuilder/pages/development-readiness.md`, `docs/workflows/parallel-codex-task-pack.md`, `docs/qa/phase-closure-checklist.md` si hace falta.
- **Archivos que NO debe tocar:** `iLiniumTech.Frontend/**`, `iLiniumTech.Backend/**`, package files, `appNavigation.ts`, `router/index.ts`.
- **Dependencias:** documentacion AppBuilder disponible y estado real reportado por agentes frontend/QA.
- **Criterios de aceptacion:** cada pagina queda clasificada como placeholder estatico o bloqueada por SDD/API; riesgos y dependencias visibles; no se inventan contratos.
- **Pruebas obligatorias:** validacion documental si se cambia estructura; `git diff --check`.
- **Documentacion a actualizar:** readiness de paginas y este pack.
- **Riesgo de conflicto:** bajo; coordinar si otros agentes editan los mismos docs.

### Tarea I - Placeholders de dominio del menu

- **Nombre:** `frontend-menu-domain-static-placeholders`
- **Alcance exacto:** enriquecer solo paginas estaticas de dominio ya existentes: `Agenda`, `Clientes`, `Propuestas`, `Recibos`, `Suplementos`, `Siniestros`, `Liq.Cia`, `Liq.Col`, `Informes`, `Estadisticas`, `Polizas / Flotas` y `Polizas / Colectivas`.
- **Archivos que puede tocar:** carpetas frontend de esas features y tests asociados, `docs/qa/**` solo para evidencia.
- **Archivos que NO debe tocar:** backend, servicios API nuevos, package files, `appNavigation.ts`, `router/index.ts`, extractor, SDDs salvo evidencia coordinada.
- **Dependencias:** matriz en `docs/appbuilder/pages/development-readiness.md`; no depende de DBA ni API porque el alcance es estatico.
- **Criterios de aceptacion:** las paginas muestran estado de modulo, alcance, proximos pasos y riesgos; acciones no operativas estan deshabilitadas; no hay llamadas a APIs nuevas ni metadata runtime.
- **Pruebas obligatorias:** `npm run format`; `npm run lint`; `npm run test:unit`; `npm run build`; smoke visual si cambia layout de menu/shell.
- **Documentacion a actualizar:** evidencia QA si se ejecuta smoke visual.
- **Riesgo de conflicto:** medio con agentes frontend que trabajen shell/menu o Polizas.

### Tarea J - Superficies tecnicas bloqueadas

- **Nombre:** `frontend-menu-technical-blocked-pages`
- **Alcance exacto:** mantener `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna` y `Logs` como paginas informativas bloqueadas, sin habilitar acciones reales.
- **Archivos que puede tocar:** carpetas frontend de esas features y tests asociados, `docs/qa/**` solo para evidencia.
- **Archivos que NO debe tocar:** backend, configuracion real, conectores, logs reales, workflows, package files, `appNavigation.ts`, `router/index.ts`.
- **Dependencias:** decision de arquitectura que prohibe AppBuilder runtime.
- **Criterios de aceptacion:** cada pagina comunica bloqueo y dependencia de SDD/seguridad; no expone secretos, payloads, rutas internas sensibles ni acciones ejecutables.
- **Pruebas obligatorias:** `npm run format`; `npm run lint`; `npm run test:unit`; `npm run build`; secret scan si se documentan nombres de entorno o integraciones.
- **Documentacion a actualizar:** evidencia QA si se toca UI.
- **Riesgo de conflicto:** bajo-medio; alto impacto si alguien intenta activar funcionalidad real.

### Tarea K - SDD/API futura por pagina

- **Nombre:** `producto-sdd-api-pages-backlog`
- **Alcance exacto:** preparar backlog documental para paginas que quieran salir del placeholder: problema de negocio, owner UAT, datos, permisos, PII, contratos API y pruebas esperadas.
- **Archivos que puede tocar:** `docs/sdd/**`, `docs/appbuilder/pages/**`, `docs/ROADMAP_OBJETIVO_FINAL.md` si cambia prioridad.
- **Archivos que NO debe tocar:** frontend/backend runtime salvo coordinacion posterior; package files; router/navegacion.
- **Dependencias:** producto, DBA/UAT, metadata sanitizada o evidencia funcional suficiente.
- **Criterios de aceptacion:** ninguna pagina pasa a datos reales sin SDD; queda claro si requiere API read-only, auth/permisos, broker, minimizacion PII o auditoria.
- **Pruebas obligatorias:** validacion documental; secret scan si se mencionan entornos/configuracion.
- **Documentacion a actualizar:** SDD de pagina o issue documental; readiness si cambia estado.
- **Riesgo de conflicto:** medio con roadmap/SDD si hay varias prioridades abiertas.

### Orden de ejecucion recomendado para paginas estaticas

1. Ejecutar Tarea H para confirmar la matriz y detectar cambios concurrentes.
2. Ejecutar Tarea I si el objetivo es mejorar paginas de dominio visibles sin datos reales.
3. Ejecutar Tarea J si el objetivo es endurecer superficies tecnicas bloqueadas.
4. Ejecutar Tarea K solo cuando producto quiera conectar datos o acciones reales de una pagina.
5. Ejecutar QA/cierre con Tarea G o Tarea 7 segun el alcance tocado.

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
