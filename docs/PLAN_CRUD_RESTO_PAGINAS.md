# Plan CRUD resto de paginas

Fecha base: 2026-05-18

Estado: plan de rollout para evolucionar las paginas del menu despues de `Polizas CRUD BBDD`.

Ultima actualizacion: 2026-05-18.

## Regla principal

El objetivo de producto es que el resto de paginas evolucionen hacia API explicita y, cuando proceda, CRUD real. No se autoriza un CRUD generico ni una replica de AppBuilder runtime.

Cada pagina debe avanzar por estos hitos:

1. Analisis AppBuilder/documental suficiente.
2. Contrato frontend mantenible: tipos, fixture sanitizado y composable local.
3. SDD de datos/API con campos permitidos, campos prohibidos, permisos, origen SQL y UAT.
4. API read-only explicita con repositorio in-memory o fixture de backend.
5. SQL read-only con whitelists, parametros, broker validado y `SESSION_CONTEXT` si aplica.
6. CRUD real solo si hay decision funcional, DBA, permisos de escritura, transacciones, rollback/limpieza, auditoria y smoke local.

## Decision de alcance para esta ronda

El usuario ha pedido "CRUD al resto de paginas". El analisis de jefes concluye que ninguna pagina distinta de Polizas esta lista hoy para escritura real segura:

- `Recibos`: alto riesgo financiero/bancario; empezar por contrato fixture y despues read-only minimizado.
- `Clientes`: PII directa; empezar por read-only minimizado sin documento/contacto/banco.
- `Siniestros`: alto riesgo por intervinientes, salud/danos, importes y textos libres; empezar por API read-only minimizada.
- `Agenda`: posible PII en asunto/descripcion; empezar por read-only por rango.
- `Propuestas`: origen funcional no confirmado; no asumir que equivale a `Solicitudes`.
- `Suplementos`: workflows, PII, banco, adjuntos e importes; empezar por read-only.
- `Liquidaciones`, `Informes`, `Logs`, `Conectividad`, `Configuracion` y `Administracion`: requieren threat model o SDD especifica antes de datos reales.

Por tanto, la primera fase del objetivo CRUD es preparar cada pagina para API explicita sin activar escrituras. La escritura se desbloquea por pagina cuando supere los criterios anteriores.

## Orden de trabajo

### Fase A - Preparacion frontend/API por pagina

Objetivo: que cada pagina tenga contrato local mantenible y lista para adaptador API futuro.

1. `Recibos`: `DONE` - tipos, fixture y composable separados; sin importes reales ni banco.
2. `Clientes`: `DONE` - tipos, fixture y composable separados; PII bloqueada.
3. `Agenda`: `DONE` - tipos, fixture y composable separados; acciones de calendario bloqueadas.
4. `Propuestas`: `DONE` - tipos, fixture y composable separados; conversion/emision bloqueadas.
5. `Suplementos`: `DONE` - tipos, fixture y composable separados; workflows/adjuntos bloqueados.
6. `Liquidaciones`, `Informes`, `Estadisticas`: separar contrato fixture despues de SDD financiera/reporting.
7. Tecnicas/admin: no activar CRUD; primero threat model.

### Fase B - Backend read-only explicito

Objetivo: crear APIs no mutantes por pagina, con permisos y repositorios in-memory, sin SQL real.

Orden recomendado:

1. `Siniestros`: `DONE` - `GET /api/siniestros/catalogs` y `GET /api/siniestros` con repositorio in-memory minimizado y permisos propios.
2. `Recibos`: `DONE` - `GET /api/recibos/catalogs` y `GET /api/recibos` con repositorio in-memory minimizado, sin importes reales ni banco.
3. `Clientes`: `DONE` - `GET /api/clientes/catalogs` y `GET /api/clientes`, con minimizacion PII.
4. `Agenda`: `DONE` - `GET /api/agenda/catalogs`, `GET /api/agenda` y alias `GET /api/agenda/events`, sin detalle/calendario mutante.
5. `Propuestas`: `DONE` - `GET /api/propuestas/catalogs` y `GET /api/propuestas`, contrato recortado sin solicitante, importes, documentos ni conversion.
6. `Suplementos`: `DONE` - `GET /api/suplementos/catalogs` y `GET /api/suplementos`, sin workflows, adjuntos, importes, banco ni escrituras.

### Fase C - SQL read-only autorizado

Objetivo: conectar datos locales o UAT solo con origen confirmado, whitelists y pruebas.

Cada pagina debe tener:

- vista/tabla autorizada por DBA;
- filtro de broker/tenant confirmado;
- campos prohibidos documentados;
- pruebas de sort/filtros maliciosos;
- evidencia sin secretos ni datos sensibles.

### Fase D - CRUD real por pagina

Objetivo: habilitar create/update/delete solo para casos de uso aprobados.

Precondiciones obligatorias:

- SDD de escritura propia;
- permisos `<pagina>.create`, `<pagina>.update`, `<pagina>.delete` o permisos mas especificos;
- `<Pagina>:WritesEnabled=false` por defecto;
- transacciones;
- validacion post-create de visibilidad;
- rollback o limpieza verificable;
- auditoria sin PII;
- smoke API/UI local;
- UAT/DBA.

## Siguiente tarea activa

ID: `T-130-LIQCIA-SDD-READONLY`

Nombre: preparar SDD/readiness de `Liq.Cia` antes de cualquier API o dato real.

Alcance:

- Analizar solo documentacion existente y menu para `Liq.Cia`.
- Definir alcance read-only, campos prohibidos financieros, permisos candidatos y bloqueos UAT/DBA/security.
- No crear backend, frontend, SQL, exportaciones, importes reales ni escrituras.

Archivos permitidos:

- `docs/sdd/specs/iLiniumTech/**`
- `docs/appbuilder/pages/**`
- `docs/qa/**`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`
- `docs/PLAN_CRUD_RESTO_PAGINAS.md`

Archivos prohibidos:

- `iLiniumTech.Backend/**`
- `iLiniumTech.Frontend/**`
- `iLiniumTech.Frontend/src/services/**`
- `iLiniumTech.Frontend/src/router/**`
- `iLiniumTech.Frontend/src/layout/**`
- cualquier `.env*`, secreto, dump o captura sensible.

Pruebas:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

## Regla de avance

Cuando una tarea se cierre:

1. Actualizar evidencia QA.
2. Actualizar `docs/PLAN_EJECUCION_CONTINUA_IA.md`.
3. Promover la siguiente tarea pequena.
4. No saltar a SQL o escritura si faltan SDD/UAT/DBA/security review.
