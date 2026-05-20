# Evidencia MVP - Logs

Fecha: 2026-05-18

## Estado

`Logs` esta disponible como ruta Vue protegida y fixture local read-only redactado. No activa API, BBDD LOG, detalle real, exportacion, payloads, trazas, identidades, request/response ni cuerpos de comunicaciones.

## Evidencia revisada

- `iLiniumTech.Frontend/src/features/logs/LogsView.vue`
- `iLiniumTech.Frontend/src/features/logs/LogsView.test.ts`
- `docs/appbuilder/pages/logs/README.md`
- `docs/appbuilder/pages/page-agent-rollout.md`

## Controles de seguridad visibles

- Acciones de detalle y exportacion deshabilitadas.
- Datos redactados y sanitizados.
- Secretos, logs, payloads y enlaces externos bloqueados.
- Sin metadata AppBuilder runtime.

## Riesgos residuales

- Riesgo critico de PII, secretos, tokens, stack traces, URLs internas y payloads en logs reales.
- Threat model creado: `docs/engineering/logs-threat-model.md`.
- Requiere SDD, privacy/security review, politica de retencion, redaccion por permiso y auditoria antes de cualquier API.

## Pruebas esperadas si cambia UI

- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`
- Secret scan y privacy review si se documentan fuentes reales de logs.

## Validacion de esta ronda

- Revision documental y de frontend en solo lectura.
- Validacion documental ligera ejecutada al cierre de la ronda.

## Actualizacion threat model 2026-05-20

- `T-052-LOGS-THREAT-MODEL` crea `docs/engineering/logs-threat-model.md`.
- El threat model deja bloqueados API real, SQL, logs reales, payloads, secretos, tokens, headers, cookies, stack traces sensibles, URLs internas, documentos, PII, detalle y exportacion.
- `/logs` sigue como fixture local read-only redactado.
- No se modifica runtime backend/frontend ni se usan datos reales.
- Validacion ejecutada: baseline documental OK, secret scan OK y `git diff --check` OK.
