# Evidencia MVP - Conectividad

Fecha: 2026-05-18

## Estado

`Conectividad` esta disponible como ruta Vue protegida y fixture local read-only. No activa API, endpoints reales, llamadas externas, pruebas REST/SOAP, tokens, payloads ni logs reales.

## Evidencia revisada

- `iLiniumTech.Frontend/src/features/conectividad/ConectividadView.vue`
- `iLiniumTech.Frontend/src/features/conectividad/ConectividadView.test.ts`
- `docs/appbuilder/pages/conectividad/README.md`
- `docs/appbuilder/pages/page-agent-rollout.md`

## Controles de seguridad visibles

- Acciones de probar, detalle y exportar deshabilitadas.
- URLs, credenciales, payloads y enlaces externos bloqueados.
- Fixture local sin API.
- Sin metadata AppBuilder runtime ni conectores heredados.

## Riesgos residuales

- Riesgo critico de SSRF, fuga de secretos, exposicion de request/response y ejecucion de integraciones si se conecta sin threat model.
- Threat model creado: `docs/engineering/conectividad-threat-model.md`.
- Requiere SDD, security review, allowlist, secret store, auditoria y permisos backend antes de cualquier API.

## Pruebas esperadas si cambia UI

- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`
- Secret scan y security review si se documentan integraciones.

## Validacion de esta ronda

- Revision documental y de frontend en solo lectura.
- Validacion documental ligera ejecutada al cierre de la ronda.

## Actualizacion threat model 2026-05-20

- `T-053-CONNECTIVITY-THREAT-MODEL` crea `docs/engineering/conectividad-threat-model.md`.
- El threat model deja bloqueados API real, SQL, conectores reales, llamadas externas, pruebas REST/SOAP, URL libre, payloads reales, secretos, tokens, headers, cookies, responses, URLs internas, documentos y PII.
- `/conectividad` sigue como fixture local read-only sanitizado.
- No se modifica runtime backend/frontend ni se ejecutan llamadas reales.
- Validacion ejecutada: baseline documental OK, secret scan OK y `git diff --check` OK.
