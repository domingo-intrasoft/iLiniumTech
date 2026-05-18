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
- Requiere SDD, threat model, allowlist, secret store, auditoria y permisos backend.

## Pruebas esperadas si cambia UI

- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`
- Secret scan y security review si se documentan integraciones.

## Validacion de esta ronda

- Revision documental y de frontend en solo lectura.
- Validacion documental ligera ejecutada al cierre de la ronda.
