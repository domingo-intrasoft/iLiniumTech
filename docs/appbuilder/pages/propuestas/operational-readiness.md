# Propuestas - readiness operativo Negocio diario

Fecha: 2026-05-18

Estado: ruta protegida fixture/read-only existente. Preparada como superficie de navegacion y pruebas, no como modulo con datos reales.

SDD draft relacionada: `docs/sdd/specs/iLiniumTech/SDD-2026-012-propuestas-read-only.md`.

## Estado actual

- Frontend existente: `iLiniumTech.Frontend/src/features/propuestas/PropuestasView.vue` y `PropuestasView.test.ts`.
- Ruta visible: `/propuestas`.
- Estado funcional: fixture local sanitizado con listado read-only, filtros locales, paginacion y acciones bloqueadas.
- Datos actuales: 4 propuestas demo `PROP-2026-*`, solicitantes anonimos, canales demo e importes como etiquetas no operativas.
- Origen real: no confirmado.
- API propia: no existe.
- Permisos productivos: no definidos ni aplicados para Propuestas.
- Metadata AppBuilder: solo evidencia documental; no se consume en runtime.

## Componentes detectados

### Frontend iLiniumTech

- `PropuestasView.vue`: vista estatica con `AppShell`, banda runtime, filtros, tabla, paginacion y acciones bloqueadas.
- `PropuestasView.test.ts`: smoke unitario de render, filtros, empty state, acciones deshabilitadas y ausencia de metadata, secretos, identificadores personales e importes operativos.

### Evidencia AppBuilder

- No hay `componentId`, arbol de componentes, datasource, filtros, acciones, permisos ni tabs confirmados.
- Existe evidencia generica de menu/componentes AppBuilder, pero no prueba una pagina concreta de Propuestas.
- El concepto `Solicitudes` aparece como posible dominio historico, pero no debe tratarse como sinonimo de Propuestas sin validacion funcional y DBA.

## Acciones bloqueadas

- Crear propuesta.
- Convertir a poliza.
- Abrir documentos.
- Exportar.
- Abrir detalle.
- Tarificacion, emision, duplicado, cancelacion y cierre.
- Llamadas externas, integraciones REST/SOAP, workflows y procesos batch.
- Importes reales, cliente real, documentos, riesgo, cuestionarios y textos libres.

## Riesgos

- Confundir Propuestas con Solicitudes y construir sobre origen incorrecto.
- PII de cliente o solicitante en listado aparentemente simple.
- Importes, primas, descuentos o condiciones comerciales sin permiso.
- Conversion a poliza o emision sin rollback, auditoria ni UAT.
- Documentos, observaciones y textos libres con PII.
- Fuga tenant si se resuelve origen antes de validar broker.
- Reintroducir AppBuilder mediante CRUD, datasources, eventos o workflows genericos.
- Logs con referencias o busquedas que identifiquen personas u oportunidades comerciales.

## Dependencias UAT/DBA

- Confirmar significado funcional de Propuestas.
- Confirmar si el origen real es Modelo, Solicitudes u otra base autorizada.
- Confirmar columnas, filtros, estados, ramos/productos y canal.
- Confirmar campos prohibidos: cliente real, documento, contacto, importes, riesgo, documentos y textos libres.
- Confirmar reglas de broker, permisos por perfil y `SESSION_CONTEXT` si aplica.
- Confirmar owner UAT antes de cualquier API.

## Proximas tareas pequenas

1. `producto-propuestas-definition-uat`: resolver si Propuestas equivale a solicitudes, cotizaciones, emision u otro flujo.
2. `frontend-propuestas-fixture-hardening`: solo si hace falta, mejorar accesibilidad/copy del fixture sin crear servicios ni API.
3. `backend-propuestas-readonly-design`: tras SDD aprobada, disenar `GET /api/propuestas` y catalogos minimos.
4. `security-propuestas-threat-review`: clasificar PII, importes, documentos, integraciones y conversion a poliza.

## Criterios de aceptacion

### Fixture actual

- La ruta sigue protegida por login.
- La vista es Vue/TypeScript estatica.
- No hay API, servicios nuevos ni datos reales.
- Crear, convertir, documentos, exportar y detalle siguen deshabilitados.
- No aparecen PII real, importes reales, documentos, metadata AppBuilder, SQL ni secretos.
- Tests frontend aplicables pasan si se toca UI.

### Futuro SDD read-only

- SDD aprobada por producto, backend/datos, frontend y seguridad.
- `propuestas.read` y, si aplica, `propuestas.catalogs` definidos y probados en backend.
- Broker validado antes de resolver datos.
- `GET /api/propuestas` usa parametros y whitelists.
- El primer corte devuelve solo listado minimizado, sin detalle ni acciones de negocio.
- No se devuelve cliente real, documentos, importes, riesgo, tarificacion, emision ni conversion.
- UAT/DBA confirma origen, definicion funcional, columnas, filtros y minimizacion.
- Evidencia sin secretos ni datos reales versionados.
