# Agenda - readiness operativo Negocio diario

Fecha: 2026-05-18

Estado: ruta protegida fixture/read-only existente. Preparada como superficie de navegacion y pruebas, no como modulo con datos reales.

SDD draft relacionada: `docs/sdd/specs/iLiniumTech/SDD-2026-011-agenda-read-only.md`.

## Estado actual

- Frontend existente: `iLiniumTech.Frontend/src/features/agenda/AgendaView.vue` y `AgendaView.test.ts`.
- Ruta visible: `/agenda`.
- Estado funcional: fixture local sanitizado con listado read-only, filtros locales y paginacion.
- Datos actuales: 4 eventos demo `AGE-2026-*`, referencias demo a objetos `POL-DEMO-*`, `SIN-DEMO-*`, `REC-DEMO-*` y `GEN-DEMO-*`.
- Origen real: no conectado.
- API propia: no existe.
- Permisos productivos: no definidos ni aplicados para Agenda.
- Metadata AppBuilder: solo evidencia documental; no se consume en runtime.

## Componentes detectados

### Frontend iLiniumTech

- `AgendaView.vue`: vista estatica con `AppShell`, banda runtime, filtros, tabla, paginacion y acciones bloqueadas.
- `AgendaView.test.ts`: smoke unitario de render, filtros, empty state, acciones deshabilitadas y ausencia de marcadores inseguros.

### Evidencia AppBuilder

- Tabla candidata historica: `Agenda`.
- Vista candidata historica: `vw_Agenda`.
- Control generico candidato: `DynamicFullCalendar`, no confirmado como componente real de esta pagina.
- No hay `componentId`, datasource, arbol de componentes, permisos ni layout confirmados para Agenda.

## Acciones bloqueadas

- Crear evento.
- Reprogramar.
- Exportar.
- Abrir desglose operativo.
- Calendario dinamico, drag/drop y seleccion de fecha.
- Detalle con descripcion larga, participantes, `IdentidadId` o `IdObjeto`.
- Workflows heredados `commandAdd`, `commandEdit`, `FormBuilder`, `NewRegister` y `doOperationData`.

## Riesgos

- PII en `Asunto` y `Descripcion`, incluyendo telefonos, correos, salud, siniestros u observaciones sensibles.
- Fuga tenant por `BrokerIntegracionId` si se consulta antes de validar broker.
- Fechas separadas en tabla frente a `Start`/`End` texto en vista; riesgo de parseo, zona horaria y eventos de dia completo.
- `IdentidadId` puede contener identificadores multiples o serializados.
- `IdObjeto` puede inducir navegacion generica heredada.
- El trigger de siniestros sugiere efectos secundarios si se escribe en el futuro.
- FullCalendar heredado trae interacciones que no pertenecen al primer corte read-only.

## Dependencias UAT/DBA

- Confirmar si Agenda es pagina propia, panel de inicio o componente de otro flujo.
- Confirmar origen autorizado: tabla, vista o vista nueva minimizada.
- Confirmar rango por defecto, columnas visibles, filtros y orden.
- Confirmar catalogos de estado, prioridad y origen si existen.
- Confirmar reglas de broker, `SESSION_CONTEXT` y permisos por perfil.
- Clasificar campos libres y definir minimizacion de asuntos.

## Proximas tareas pequenas

1. `producto-agenda-uat-questions`: preparar preguntas UAT sobre rango, columnas, estados, prioridad y uso de lista frente a calendario.
2. `frontend-agenda-fixture-hardening`: solo si hace falta, mejorar accesibilidad/copy del fixture sin crear servicios ni API.
3. `backend-agenda-readonly-design`: tras SDD aprobada, disenar contrato `GET /api/agenda/events` con rango obligatorio.
4. `security-agenda-privacy-review`: clasificar asunto, descripcion, identidad y objeto relacionado antes de datos reales.

## Criterios de aceptacion

### Fixture actual

- La ruta sigue protegida por login.
- La vista es Vue/TypeScript estatica.
- No hay API, servicios nuevos ni datos reales.
- Las acciones reales siguen deshabilitadas.
- No aparecen metadata AppBuilder, SQL, secretos ni PII real en DOM.
- Tests frontend aplicables pasan si se toca UI.

### Futuro SDD read-only

- SDD aprobada por producto, backend/datos, frontend y seguridad.
- `agenda.read` definido y probado en backend.
- Broker validado antes de resolver datos.
- `GET /api/agenda/events` usa rango acotado, parametros y whitelists.
- No se devuelve descripcion larga, participantes, `IdentidadId`, objeto navegable ni PII.
- UAT/DBA confirma origen, rango, columnas y minimizacion.
- Evidencia sin secretos ni datos reales versionados.
