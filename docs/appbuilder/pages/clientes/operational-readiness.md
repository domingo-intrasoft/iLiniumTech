# Clientes - readiness operativo Negocio diario

Fecha: 2026-05-18

Estado: ruta protegida fixture/read-only existente. Preparada como superficie de navegacion y pruebas, no como modulo con datos reales.

SDD draft relacionada: `docs/sdd/specs/iLiniumTech/SDD-2026-010-clientes-read-only.md`.

## Estado actual

- Frontend existente: `iLiniumTech.Frontend/src/features/clientes/ClientesView.vue` y `ClientesView.test.ts`.
- Ruta visible: `/clientes`.
- Estado funcional: fixture local sanitizado con listado read-only, filtros locales, paginacion y PII bloqueada.
- Datos actuales: 4 clientes demo `CLI-2026-*`, alias anonimos y estados demo.
- Origen real: no conectado.
- API propia: no existe.
- Permisos productivos: no definidos ni aplicados para Clientes.
- Metadata AppBuilder: solo evidencia documental; no se consume en runtime.

## Componentes detectados

### Frontend iLiniumTech

- `ClientesView.vue`: vista estatica con `AppShell`, banda runtime, filtros, tabla, paginacion y acciones bloqueadas.
- `ClientesView.test.ts`: smoke unitario de render, filtros, empty state, acciones deshabilitadas y ausencia de PII/runtime metadata.

### Documentacion AppBuilder

- `components/busqueda-listado.md`: componente candidato para listado read-only.
- `components/resumen-identidad.md`: ficha/resumen candidato, fuera del primer corte.
- `components/polizas.md`: tab candidata de polizas, fuera del primer corte.
- `components/recibos.md`: tab candidata de recibos, fuera del primer corte.
- `components/riesgos.md`: tab candidata de riesgos, fuera del primer corte.
- `components/siniestros.md`: tab candidata de siniestros, fuera del primer corte.
- `components/suplementos.md`: tab candidata de suplementos, fuera del primer corte.

No hay `componentId`, datasource, layout, columnas, permisos historicos ni UAT confirmados para la pantalla real de Clientes.

## Acciones bloqueadas

- Abrir ficha.
- Exportar.
- Desglose.
- Alta, edicion y baja de cliente.
- Tabs de polizas, recibos, riesgos, siniestros y suplementos.
- Documento, contacto, direccion, datos bancarios, metricas y anotaciones.
- Workflows, importaciones, comunicaciones y navegacion real a modulos relacionados.

## Riesgos

- PII alta: documento, nombre legal, contacto, direccion, fechas personales y datos bancarios.
- Fuga tenant si se consulta por broker equivocado o antes de validar permisos.
- Ambiguedad entre `Identidad` e `IdentidadCliente`.
- Vistas `vw_Cliente*` pueden inducir falsa paridad con tabs no confirmadas.
- Riesgo financiero en metricas, recibos, acuerdos y comisiones.
- Riesgo especialmente sensible si se exponen siniestros o riesgos desde ficha.
- Busquedas por nombre/documento no deben quedar en logs en claro.
- Copiar `DynamicTab*`, `DynamicCrudTabla` o `QueryStatic` reintroduciria AppBuilder runtime.

## Dependencias UAT/DBA

- Confirmar origen autorizado de listado minimizado.
- Confirmar columnas, filtros, orden y page size inicial.
- Confirmar regla de broker, oficina, gestor, comercial y perfil.
- Confirmar si `nombreMostrable` puede ser alias, nombre comercial o campo minimizado.
- Confirmar campos prohibidos: documento, contacto, direccion, banco, metricas, notas y relaciones.
- Confirmar permisos funcionales y matriz por broker/perfil antes de datos reales.

## Proximas tareas pequenas

1. `producto-clientes-uat-questions`: preparar preguntas de listado minimizado, columnas y filtros.
2. `frontend-clientes-fixture-hardening`: solo si hace falta, mejorar accesibilidad/copy del fixture sin crear servicios ni API.
3. `backend-clientes-readonly-design`: tras SDD aprobada, disenar `GET /api/clientes` y catalogos minimos.
4. `security-clientes-pii-review`: clasificar identidad, contacto, direccion, banco, metricas y tabs relacionadas.

## Criterios de aceptacion

### Fixture actual

- La ruta sigue protegida por login.
- La vista es Vue/TypeScript estatica.
- No hay API, servicios nuevos ni datos reales.
- Las acciones de ficha, exportacion y desglose siguen deshabilitadas.
- No aparecen documento, email, telefono, direccion, banco, metadata AppBuilder, SQL ni secretos.
- Tests frontend aplicables pasan si se toca UI.

### Futuro SDD read-only

- SDD aprobada por producto, backend/datos, frontend y seguridad.
- `clientes.catalogs` y `clientes.read` definidos y probados en backend.
- Broker validado antes de resolver datos.
- `GET /api/clientes` usa parametros y whitelists.
- El primer corte devuelve solo listado minimizado, sin ficha ni tabs.
- No se devuelve documento, contacto, direccion, banco, metricas, anotaciones ni relaciones sensibles.
- UAT/DBA confirma origen, columnas, filtros y minimizacion.
- Evidencia sin secretos ni datos reales versionados.
