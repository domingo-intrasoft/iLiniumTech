# Rollout de agentes por pagina del menu

Fecha: 2026-05-18

Estado: paquete activo tras cierre local de `Polizas CRUD BBDD` como vertical de referencia.

## Objetivo

Coordinar agentes por pagina del menu para evolucionar el resto de superficies de iLiniumTech sin convertir AppBuilder en runtime. Cada pagina debe pasar por analisis, documentacion, SDD y contrato explicito antes de activar datos reales, API, escrituras, exportaciones o permisos nuevos.

Este documento no autoriza cambios funcionales por si solo. Sirve como mapa de reparto para agentes Codex y como entrada para SDD futuras.

## Regla no negociable

- La navegacion y las pantallas se mantienen como codigo Vue/TypeScript.
- La metadata AppBuilder solo sirve como evidencia, trazabilidad y scaffolding revisado.
- Ninguna pagina fixture puede crear API, leer datos reales, escribir, exportar, mostrar PII real ni activar permisos nuevos sin SDD, contrato API, permisos backend, UAT y revision de seguridad si aplica.
- `Autos Particulares` sigue aparcado.
- `Polizas CRUD BBDD` queda como referencia local de vertical completo, no como permiso para copiar CRUD a otras paginas.

## Organizacion de jefes

| Jefe | Paginas | Mision inmediata | Salida esperada |
| --- | --- | --- | --- |
| Negocio diario | `Agenda`, `Clientes`, `Propuestas` | Confirmar alcance real y preparar SDD/backlog sin activar PII ni workflows. | Matriz por pagina, preguntas UAT, tareas frontend fixture y SDD futura. |
| Operativa seguros | `Recibos`, `Suplementos`, `Siniestros`, `Polizas / Flotas`, `Polizas / Colectivas` | Separar placeholders visibles de futuros contratos API. | SDD candidates, riesgos PII/financieros y permisos por pagina. |
| Reporting y liquidaciones | `Liq.Cia`, `Liq.Col`, `Informes`, `Estadisticas` | Evitar que importes, informes y agregados salgan sin permisos ni minimizacion. | Matriz financiera/reporting con UAT, permisos, exportaciones y evidencia requerida. |
| Tecnico/admin | `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna`, `Logs` | Mantener superficies bloqueadas y evitar reintroducir Builder, secretos, conectores o logs reales. | Security backlog, SDD/threat model requerido y pruebas de bloqueo. |

## Orden recomendado

1. Ronda documental: mantener este paquete, `development-readiness.md` y los READMEs de pagina alineados con el estado real de router/menu.
2. Ronda fixture segura: solo hardening visual/accesibilidad/tests de paginas ya existentes, sin backend ni servicios nuevos.
3. Ronda SDD read-only: abrir SDD una pagina cada vez para `Siniestros`, `Recibos`, `Clientes`, `Agenda`, `Propuestas` y `Suplementos`, segun prioridad de producto.
4. Ronda financiera/reporting: `Liq.Cia`, `Liq.Col`, `Informes` y `Estadisticas`, siempre con UAT, permisos y minimizacion antes de exportar o calcular.
5. Ronda tecnica/admin: `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna` y `Logs` solo tras SDD, threat model y decision de arquitectura.

## Matriz operativa por pagina

| Pagina | Estado actual | Tarea siguiente segura | Requiere SDD para | Archivos permitidos para placeholder | Archivos prohibidos sin SDD/coordinacion | Pruebas obligatorias |
| --- | --- | --- | --- | --- | --- | --- |
| Agenda | Ruta protegida `fixture`; Vue read-only con filtros locales y acciones bloqueadas. | `frontend-agenda-static-readiness-polish`. | Calendario real, API, creacion, reprogramacion, drag/drop, permisos `agenda.*`. | `iLiniumTech.Frontend/src/features/agenda/**`, `docs/qa/agenda-mvp-evidence.md`. | Backend, `src/services/**`, router, navegacion, extractor, package files. | Frontend format/lint/unit/build si toca UI; baseline docs si toca docs. |
| Clientes | Ruta protegida `fixture`; Vue read-only con PII bloqueada y componentes documentales. | `frontend-clientes-static-pii-hardening`; SDD draft: [`SDD-2026-010`](../../sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md). | Listado real minimizado; ficha, tabs, documento, contacto, direccion, banco, metricas, exportacion y relaciones quedan para SDD posterior. | `iLiniumTech.Frontend/src/features/clientes/**`, `docs/appbuilder/pages/clientes/**`, `docs/qa/clientes-mvp-evidence.md`. | Backend, servicios API, router, navegacion, datos reales, permisos nuevos. | Frontend completo si UI; secret scan si se documenta PII/entornos. |
| Propuestas | Ruta protegida `fixture`; alcance real no confirmado. | `frontend-propuestas-static-scope-clarity`. | Detalle, emision, conversion a poliza, documentos, importes, workflow. | `iLiniumTech.Frontend/src/features/propuestas/**`, `docs/qa/propuestas-mvp-evidence.md`. | Backend, servicios API, permisos nuevos, deducciones de `Solicitudes`. | Frontend completo si UI; baseline docs si documentacion. |
| Recibos | Ruta protegida `fixture`; importes demo y acciones financieras bloqueadas. | `producto-sdd-recibos-readonly`; SDD draft: [`SDD-2026-009`](../../sdd/specs/iLiniumTech/SDD-2026-009-recibos-read-only.md). | Listado real minimizado; importes reales, banco, remesas, detalle, EIAC, cobro y exportacion quedan para SDD posterior. | `iLiniumTech.Frontend/src/features/recibos/**`, `docs/appbuilder/pages/recibos/**`, `docs/qa/**`. | Backend y `src/services/**` sin SDD/API; router/navegacion salvo coordinacion. | Frontend completo si UI; backend/security solo con API. |
| Suplementos | Ruta protegida `fixture`; workflows y adjuntos bloqueados. | `producto-sdd-suplementos-readonly`. | Datos reales, detalle, alta/edicion, documentos, workflows. | `iLiniumTech.Frontend/src/features/suplementos/**`, `docs/appbuilder/pages/suplementos/**`, `docs/qa/**`. | Backend, servicios API, workflows heredados, metadata runtime. | Frontend completo si UI; secret scan si se documentan adjuntos/entornos. |
| Siniestros | Ruta protegida `fixture`; datos sensibles bloqueados. | `producto-sdd-siniestros-readonly-security`; SDD draft: [`SDD-2026-008`](../../sdd/specs/iLiniumTech/SDD-2026-008-siniestros-read-only.md). | Listado real minimizado; detalle, intervinientes, salud/lesiones, pagos, EIAC, observaciones y exportacion quedan para SDD posterior. | `iLiniumTech.Frontend/src/features/siniestros/**`, `docs/appbuilder/pages/siniestros/**`, `docs/qa/**`. | Backend, servicios API, datos reales, exportaciones, observaciones libres. | Frontend completo si UI; security/privacy review antes de API. |
| Polizas / Flotas | Ruta protegida `blockedSdd`; scope estatico bajo Polizas. | `producto-sdd-polizas-flotas-scope`. | Filtro real, listado, API, permiso propio, regla funcional de flota. | `iLiniumTech.Frontend/src/features/polizas-scopes/**`, `docs/appbuilder/pages/polizas-flotas/**`. | Extender `/api/polizas`, router/nav, SQL o permisos sin SDD. | Test compartido de scopes si UI; backend tests solo con API. |
| Polizas / Colectivas | Ruta protegida `blockedSdd`; scope estatico bajo Polizas. | `producto-sdd-polizas-colectivas-scope`. | Filtro real, listado, API, permiso propio, regla funcional colectiva. | `iLiniumTech.Frontend/src/features/polizas-scopes/**`, `docs/appbuilder/pages/polizas-colectivas/**`. | Extender `/api/polizas`, router/nav, SQL o permisos sin SDD. | Test compartido de scopes si UI; backend tests solo con API. |
| Liq.Cia | Ruta protegida `fixture`; liquidacion de compania estatica. | `producto-sdd-liq-cia-readonly-finance`. | Importes reales, facturas, banco, desglose, cierre, exportacion. | `iLiniumTech.Frontend/src/features/liquidaciones-compania/**`, `docs/appbuilder/pages/liq-cia/**`, `docs/qa/**`. | Backend, servicios API, SQL financiero, acciones de cierre. | Frontend completo si UI; security/finance review antes de API. |
| Liq.Col | Ruta protegida `fixture`; comisiones y liquidos bloqueados. | `producto-sdd-liq-col-readonly-finance`. | Comisiones reales, retenciones, conceptos, relacion con compania, exportacion. | `iLiniumTech.Frontend/src/features/liquidaciones-colaborador/**`, `docs/appbuilder/pages/liq-col/**`, `docs/qa/**`. | Backend, servicios API, SQL financiero, calculos reales. | Frontend completo si UI; security/finance review antes de API. |
| Informes | Ruta protegida `fixture`; catalogo candidato no operativo. | `producto-informes-inventory-sdd`. | Catalogo real, ejecucion, descarga, formatos, historial, permisos. | `iLiniumTech.Frontend/src/features/informes/**`, `docs/appbuilder/pages/informes/**`, `docs/qa/**`. | Backend, servicios de descarga, URLs externas, generacion documental. | Frontend completo si UI; threat model antes de descargas. |
| Estadisticas | Ruta protegida `fixture`; KPIs candidatos sin cifras reales. | `producto-estadisticas-metrics-sdd`. | Metricas reales, agregados, drilldown, cache, exportacion. | `iLiniumTech.Frontend/src/features/estadisticas/**`, `docs/appbuilder/pages/estadisticas/**`, `docs/qa/**`. | Backend, SQL, cache, graficos conectados a datos reales. | Frontend completo si UI; privacy review por agregados. |
| Administracion | Ruta protegida `blockedSdd`; riesgo Builder. | `producto-admin-scope-decision`. | Usuarios, permisos, auditoria, configuracion, acciones admin. | `iLiniumTech.Frontend/src/features/administracion/**`, `docs/appbuilder/pages/administracion/**`, `docs/qa/**`. | Backend, servicios API, Builder, router/nav, configuracion real. | Frontend completo si UI; SDD/security antes de cualquier accion. |
| Configuracion | Ruta protegida `blockedSdd`; configuracion real bloqueada. | `security-configuracion-redaction-policy`. | Valores reales, secretos, auth, integraciones, preferencias persistentes. | `iLiniumTech.Frontend/src/features/configuracion/**`, `docs/appbuilder/pages/configuracion/**`, `docs/qa/**`. | Backend, env/config real, secretos, servicios API. | Secret scan obligatorio si se documenta configuracion. |
| Conectividad | Ruta protegida `blockedSdd`; conectores REST/SOAP heredados bloqueados. | `security-conectividad-threat-model`. | Llamadas externas, pruebas de conexion, tokens, allowlists, payloads. | `iLiniumTech.Frontend/src/features/conectividad/**`, `docs/appbuilder/pages/conectividad/**`, `docs/qa/**`. | Backend, conectores, URLs reales, tokens, workflows heredados. | Threat model y secret scan antes de cualquier API. |
| Controles | Ruta protegida `blockedSdd`; motor dinamico no migrable por deduccion. | `producto-controles-meaning-discovery`. | Cualquier caso funcional real, validaciones, workflows, motor de controles. | `iLiniumTech.Frontend/src/features/controles/**`, `docs/appbuilder/pages/controles/**`, `docs/qa/**`. | Backend, motores genericos, metadata runtime, workflows. | Frontend completo si UI; SDD antes de funcionalidad. |
| By Aunna | Ruta protegida `blockedSdd`; branding/contenido no confirmado. | `producto-by-aunna-functional-decision`. | Contenido real, publicacion, enlaces, descargas, administracion. | `iLiniumTech.Frontend/src/features/by-aunna/**`, `docs/appbuilder/pages/by-aunna/**`, `docs/qa/**`. | Backend, enlaces externos operativos, CMS, permisos nuevos. | Frontend completo si UI; SDD si se vuelve modulo. |
| Logs | Ruta protegida `blockedSdd`; logs redactados fixture. | `security-logs-observability-sdd`. | Logs reales, busqueda, payloads, exportacion, retencion, detalle. | `iLiniumTech.Frontend/src/features/logs/**`, `docs/appbuilder/pages/logs/**`, `docs/qa/**`. | Backend, logs reales, payloads, secretos, stack traces, descargas. | Security/privacy review, secret scan y permisos antes de API. |

## Plantilla de tarea para un agente de pagina

- Nombre:
- Pagina:
- Objetivo:
- Alcance exacto:
- Fuera de alcance:
- Fuentes a leer:
- Archivos que puede tocar:
- Archivos que no debe tocar:
- Dependencias:
- Riesgos de PII, tenant, permisos, seguridad o finanzas:
- Pruebas obligatorias:
- Documentacion a actualizar:
- Criterios de aceptacion:
- Evidencia requerida:
- Riesgo de conflicto:

## Criterio Done por pagina

Carril fixture:

- La pagina sigue protegida por login.
- Es Vue/TypeScript estatico, sin metadata AppBuilder runtime.
- No crea servicios API ni lee datos reales.
- Acciones reales estan deshabilitadas o bloqueadas.
- Tests frontend/documentales aplicables pasan.
- Riesgos y bloqueos quedan escritos.

Carril datos/API:

- SDD aprobada.
- Permisos iLiniumTech backend definidos y probados.
- Broker/tenant validado antes de consultar o escribir.
- Contrato API explicito y sin endpoints genericos de pantalla.
- PII/finanzas minimizadas o enmascaradas por permiso.
- UAT/DBA o bloqueo externo documentado.
- Build, tests, auditorias y evidencia reproducible sin secretos.
