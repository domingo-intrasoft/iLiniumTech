# Roadmap objetivo final iLiniumTech

Fecha: 2026-05-14

## Proposito

Este documento es la guia canonica de calidad para llevar iLiniumTech desde el MVP de polizas hasta una fase final profesional. Consolida los documentos existentes sin sustituirlos:

- [MVP_POLIZAS_PLAN.md](MVP_POLIZAS_PLAN.md)
- [DECISION_PRODUCTO_ARQUITECTURA.md](DECISION_PRODUCTO_ARQUITECTURA.md)
- [engineering/README.md](engineering/README.md)
- [PLAN_CICD_GITHUB_ONLY.md](PLAN_CICD_GITHUB_ONLY.md)
- [sdd/specs/iLiniumTech](sdd/specs/iLiniumTech)

Decision no negociable: iLiniumTech no es un runtime dinamico tipo AppBuilder. La metadata heredada sirve para extraccion, trazabilidad, comparativa y scaffolding revisado; el producto final debe quedar como frontend Vue estatico y backend API explicita.

## Estado de partida

- Fase documental y decision de arquitectura: completada.
- MVP read-only de polizas: implementado con frontend Vue, backend API, fixtures anonimizados, API key y pruebas base.
- Repositorio SQL read-only: implementado como backend configurable, parametrizado y con whitelist; pendiente de validar contra entorno real autorizado y auth real.
- Extractor offline de metadata: implementado en `tools/extractor/polizas-metadata` con modos `Fixture`, `DryRun` y `Live`; pendiente de validar modo `Live` contra entorno autorizado y politica final de artefactos.
- Frontend polizas: protegido por configuracion runtime y contexto `/api/me`; no consulta backend si falta API key/contexto de broker requerido.
- CI GitHub: baseline disponible con `ci.yml`, `security.yml`, PR template e issue form SDD.
- Gate local fase 7: disponible y actualizado en `tools/quality/Invoke-MvpQualityGate.ps1` con backend, frontend, smoke, auditorias, validacion documental y `git diff --check`.
- Fase 7 plataforma: CI publica artefacto de calidad documental, seguridad tiene schedule, CodeQL esta definido y preview queda como dry-run manual sin secrets ni deploy real.

## Principios de cierre

- Cada cambio funcional relevante nace de SDD o decision documentada.
- Ninguna credencial, connection string real, dump, dato personal real o captura sensible entra en Git.
- Lo que bloquee merge o cierre de fase debe poder ejecutarse localmente o quedar documentado como bloqueo externo.
- Los agentes trabajan con propiedad acotada y no revierten cambios ajenos.
- Las fases se cierran con evidencias: comandos, resultados, rutas tocadas, riesgos residuales y bloqueos.

## Organizacion por jefes/agentes

### Jefe calidad, CI y documentacion

Propiedad principal: `docs/**`, `tools/**`, `.github/**`, `README.md`.

Responsabilidades:

- Mantener este roadmap, DoD, plantillas, gates y evidencia de cierre.
- Cuidar que SDD, PR template y workflows pidan la informacion minima correcta.
- Ejecutar o coordinar `Invoke-MvpQualityGate.ps1`, auditorias de seguridad y validacion documental.
- Registrar bloqueos de entorno real sin convertirlos en deuda invisible.

### Jefe backend/API/datos

Propiedad principal: `iLiniumTech.Backend/**`.

Responsabilidades:

- Mantener contratos API, validacion, auth/autorizacion, repositorios y pruebas.
- Evitar SQL estructural desde metadata AppBuilder.
- Parametrizar valores, validar whitelists y aislar `SESSION_CONTEXT` por request.
- Documentar cualquier cambio de contrato en SDD y README si afecta ejecucion local.

### Jefe frontend/UX

Propiedad principal: `iLiniumTech.Frontend/**`.

Responsabilidades:

- Mantener pantallas Vue estaticas y componentes propios.
- Cubrir estados `loading`, `empty`, `error`, permisos y fallos de configuracion.
- No consumir metadata `IAP_*` como contrato runtime.
- Aportar pruebas unitarias y smoke visual cuando el flujo sea visible.

### Jefe seguridad/plataforma

Propiedad compartida: `tools/security/**`, `.github/workflows/security.yml`, configuracion de entornos, secretos y despliegues.

Responsabilidades:

- Validar gitleaks, dependencias, CORS, CodeQL futuro y permisos de workflows.
- Definir environments GitHub, secretos y reglas de branch protection cuando existan checks estables.
- Revisar cambios en rutas sensibles y despliegues.

### Jefe producto/SDD/UAT

Propiedad principal: `docs/sdd/**`, criterios funcionales y validacion con usuarios.

Responsabilidades:

- Mantener objetivos, fuera de alcance, criterios de aceptacion y pruebas manuales.
- Separar evidencia AppBuilder de comportamiento productivo iLiniumTech.
- Confirmar UAT y priorizar escrituras, permisos o workflows futuros.

### DBA/entorno real

Propiedad externa al repositorio.

Responsabilidades:

- Proveer BBDD de test, cuentas read-only, vistas autorizadas, claves y reglas de `SESSION_CONTEXT`.
- Confirmar campos sensibles y restricciones por broker, perfil, oficina, gestor y usuario.
- Validar que las pruebas contra datos reales no filtran informacion.

## Fases hasta objetivo final

### Fase 0 - Gobierno y decision arquitectonica

Objetivo: fijar que iLiniumTech es producto Vue/API, no AppBuilder runtime.

DoD:

- Decision producto/arquitectura versionada.
- Engineering docs enlazados.
- SDD template y security review template disponibles.
- Politica GitHub/IA base documentada.
- Secret scan limpio sobre documentacion inicial.

Estado: completada.

### Fase 1 - MVP Polizas read-only reproducible

Objetivo: primer corte vertical de polizas con UI estatica, API backend, fixtures anonimizados y pruebas base.

DoD:

- `GET /health`, `GET /api/polizas/catalogs`, `GET /api/polizas`, `GET /api/polizas/{id}` disponibles.
- `/api/polizas/*` protegido por API key de entorno.
- Frontend `/polizas` y detalle read-only implementados como Vue/TypeScript propio.
- Sort y filtros limitados por contrato/whitelist.
- Fixtures sin datos personales reales.
- Build, lint, tests y auditorias base ejecutados.

Estado: implementada; mantener regresion en CI.

### Fase 2 - Calidad y CI baseline estable

Objetivo: hacer repetibles los checks locales y de GitHub antes de exigirlos como required checks.

DoD:

- `.github/workflows/ci.yml` ejecuta backend y frontend.
- `.github/workflows/security.yml` ejecuta secret scan, dependency audit y CORS audit.
- PR template exige SDD, pruebas, seguridad y riesgos residuales.
- Issue form SDD disponible.
- Gate local `tools/quality/Invoke-MvpQualityGate.ps1` documentado y mantenido.
- Validacion documental local disponible.

Estado: baseline disponible; required checks deben activarse solo despues de una ejecucion verde en GitHub.

### Fase 3 - Repositorio SQL read-only con entorno real controlado

Objetivo: sustituir progresivamente fixtures por lectura SQL segura y validada.

DoD:

- Repositorio SQL activable por configuracion, con `InMemory` como default seguro.
- Connection strings y claves solo en entorno/secret store.
- Whitelist de columnas, sort y estructura SQL definida en codigo/configuracion iLiniumTech revisada.
- Valores parametrizados.
- Broker efectivo resuelto por request en escenarios multi-broker.
- `SESSION_CONTEXT` parametrizado y aislado si la BBDD real lo requiere.
- Pruebas de integracion contra BBDD de test, contenedor o fixture SQL controlado.
- UAT compara resultados con entorno autorizado.

Completado con evidencia versionada:

- `SqlPolizasRepository` activable por configuracion, con `InMemory` como default seguro.
- Query builder con valores parametrizados, sort/columnas por whitelist y pruebas de payloads maliciosos.
- Resolver `AppBuilderMaster` por broker efectivo, con cabeceras MVP solo si hay opt-in de `Polizas:AllowHeaderExecutionContext`.
- `SESSION_CONTEXT` se aplica antes de consultas SQL con `sp_set_session_context` parametrizado y se limpia con valores `NULL` si no hay contexto.
- `/api/me` expone contexto efectivo para que el frontend detecte ausencia de broker antes de consultar.

Pendiente tecnico:

- Sustituir cabeceras MVP por autenticacion/autorizacion real y claims/sesion backend.
- Ejecutar pruebas de integracion SQL contra BBDD de test, contenedor o fixture SQL controlado.
- Ejecutar UAT comparando resultados contra entorno autorizado.

Bloqueado externo:

- Falta confirmar BBDD de test o contenedor SQL Server autorizado.
- Falta confirmar claves obligatorias de `SESSION_CONTEXT`.
- Falta auth real para reemplazar cabeceras MVP de broker, usuario y perfil.
- Falta validar permisos de cuenta read-only y mascarado de campos sensibles.

### Fase 4 - Extractor offline y scaffolding revisado

Objetivo: generar conocimiento AppBuilder sanitizado para SDD y scaffolding, sin introducir runtime dinamico.

DoD:

- Comando local/offline documentado para extraer metadata de polizas.
- Configuracion leida desde variables de entorno o secret store.
- JSON sanitizado sin secretos, datos personales ni connection strings.
- Salida separa trazabilidad AppBuilder de sugerencias de scaffolding iLiniumTech.
- Pruebas unitarias de mapeo, redaccion y campos obligatorios.
- Integracion opcional marcada como skipped si no hay BBDD local autorizada.
- Cualquier salida util se convierte en codigo o documentacion revisada antes de entrar al producto.

Completado con evidencia versionada:

- Comando local `tools/extractor/polizas-metadata/Invoke-PolizasMetadataExtractor.ps1`.
- Modos `Fixture`, `DryRun` y `Live`; el modo `Live` requiere variables de entorno y no debe imprimir secretos.
- README del extractor documenta que el JSON es solo trazabilidad/scaffolding offline, no contrato runtime.
- `QueryStatic` se conserva como evidencia redaccionada con fingerprint y no se ejecuta.
- Pruebas del extractor cubren escritura JSON sanitizada, redaccion de connection strings y fragmentos SQL, y modo `DryRun`.
- Salida por defecto en `reports/polizas-metadata`, ruta no destinada a versionarse.

Pendiente tecnico:

- Definir contrato JSON final si se va a consumir como insumo de SDD/scaffolding en nuevas fases.
- Decidir si algun artefacto sanitizado debe versionarse y en que ruta.
- Ejecutar modo `Live` solo cuando exista entorno read-only autorizado.

Bloqueado externo:

- Falta entorno read-only para `IL_Maestro` y `AunnaTechADM`.
- Falta cuenta con permisos minimos y confirmacion de campos sensibles en metadata real.

### Fase 5 - Autenticacion, autorizacion y permisos de producto

Objetivo: retirar confianza en headers MVP y modelar identidad/permisos como producto.

SDD base: [SDD-2026-005 Auth y permisos de producto](sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md).

DoD:

- Mecanismo de autenticacion aprobado.
- Broker, usuario, perfil e isAdmin derivados de claims/sesion backend.
- Autorizacion backend aplicada antes de leer datos.
- Errores 401/403 sin trazas internas ni datos sensibles.
- Pruebas de acceso sin token, con token invalido, sin broker y sin permiso.
- Documentacion de migracion desde cabeceras MVP.

Estado real a 2026-05-14:

Entregado:

- Modelo documental de auth/permisos definido en `SDD-2026-005`.
- Headers MVP clasificados como bootstrap no confiable para produccion.
- Estrategia incremental definida para pasar de API key/headers a claims o sesion backend sin romper contratos del frontend.
- Contrato conceptual de contexto definido: `currentUserId`, `currentBrokerId`, perfil, roles, permisos, brokers permitidos y `correlationId`.
- Regla reafirmada: permisos AppBuilder pueden servir como evidencia o migracion, nunca como motor runtime dinamico.

Pendiente de implementacion:

- Elegir e integrar mecanismo auth aprobado.
- Normalizar claims/sesion a contexto backend por request.
- Crear politicas backend por permisos efectivos como `polizas.read`, `polizas.detail` y `polizas.catalogs`.
- Aplicar 401/403 sanitizados con `correlationId` y logs seguros.
- Conectar `SESSION_CONTEXT` a valores autenticados, no a headers manipulables.
- Cubrir pruebas de acceso anonimo, token invalido, broker cruzado y falta de permiso.

Bloqueos externos:

- Falta decision de proveedor/mecanismo auth.
- Falta mapa funcional de permisos por broker, perfil, oficina, gestor y usuario.
- Falta confirmar con DBA las claves obligatorias de `SESSION_CONTEXT` asociadas a permisos reales.
- Falta responsable funcional para validar equivalencias entre permisos AppBuilder historicos y permisos iLiniumTech.

Siguientes pasos recomendados:

- Abrir issue `SDD-2026-005 Auth y permisos de producto`.
- Resolver decision humana de proveedor auth antes de programar runtime.
- Levantar matriz funcional de permisos por broker, perfil, oficina, gestor y usuario.
- Implementar primero contrato de contexto y pruebas 401/403; despues retirar headers MVP de preview/produccion.

### Fase 6 - Funcionalidad explicita posterior al MVP

Objetivo: anadir detalle ampliado, acciones, escrituras o workflows solo como casos de uso propios.

DoD:

- Cada accion nueva tiene SDD propia.
- Se declaran datos afectados, permisos, auditoria, rollback y UAT.
- No se ejecutan workflows, expresiones o REST/SOAP heredados de forma generica.
- Escrituras usan transacciones, validacion y pruebas de integracion.
- Logs y eventos no exponen datos personales innecesarios.

Bloqueos actuales:

- Falta priorizacion de acciones fuera del read-only.
- Falta confirmar reglas funcionales reales y responsables UAT.

### Fase 7 - Preview profesional y operacion controlada

Objetivo: disponer de entorno preview reproducible, checks estables y evidencia de entrega.

DoD:

- Branch protection para `main` con PR obligatoria, reviews y checks estables.
- Environments GitHub `ci` y `preview` configurados; `production` solo cuando exista destino real.
- Despliegue preview manual o protegido.
- Smoke E2E de UI y API contra preview.
- Artefactos de pruebas y seguridad publicados.
- Runbook de demo y recuperacion de fallos actualizado.

Bloqueos actuales:

- Falta decidir destino preview para frontend estatico y backend API.
- Falta configurar secrets/environments reales.
- Falta confirmar que GitHub Code Scanning esta disponible y que CodeQL pasa verde antes de hacerlo required.
- Falta ejecutar `preview-dry-run.yml` en GitHub y, despues, definir environment `preview` protegido antes de cualquier despliegue real.

Avance 2026-05-14:

- `ci.yml` incorpora baseline documental y `git diff --check` con artefacto `ci-quality-reports`.
- `security.yml` mantiene secret scan, dependency audit y CORS audit, ahora tambien con schedule semanal.
- `codeql.yml` queda creado para C# y TypeScript sin secrets.
- `preview-dry-run.yml` queda manual, bloqueado por `DEPLOY_PREVIEW_ENABLED=false` y publica solo un plan de preview.
- `Invoke-MvpQualityGate.ps1` agrupa restore/build/test backend, formato/lint/test/build frontend, smoke backend/frontend, pruebas del extractor, auditorias, validacion documental y `git diff --check`.

Pendiente tecnico:

- Ejecutar gate completo como evidencia de rama antes de PR cuando los cambios toquen runtime o seguridad.
- Ejecutar `preview-dry-run.yml` en GitHub y revisar artefactos antes de activar cualquier preview real.

Bloqueado externo:

- Required checks y environments reales dependen de configuracion GitHub/preview por responsables humanos.

### Fase 8 - Preparacion productiva

Objetivo: cerrar criterios de produccion sin depender de supuestos locales.

DoD:

- Configuracion por entorno completa y secreta.
- Observabilidad minima: health, logs sanitizados, correlacion y alertas.
- Backup/restore o estrategia de continuidad definida si aplica.
- Seguridad revisada para datos personales y financieros.
- Pruebas de carga/smoke basicas sobre rutas criticas.
- UAT firmado para flujos incluidos.
- Riesgos residuales aceptados por responsable humano.

Bloqueos actuales:

- Falta infraestructura productiva definida.
- Falta modelo operativo de soporte, releases y aprobaciones.
- Falta evaluacion final de proteccion de datos.

## Evidencias obligatorias por cierre de fase

Cada cierre de fase debe dejar:

- SDD o decision enlazada.
- Rutas tocadas.
- Comandos ejecutados y resultado.
- Evidencias generadas: TRX, JUnit, reportes security, capturas o logs sanitizados.
- Pruebas no ejecutadas y motivo.
- Riesgos residuales.
- Bloqueos por entorno real, con responsable externo si aplica.
- Clasificacion explicita de cada punto abierto como `completado con evidencia`, `pendiente tecnico` o `bloqueado externo`.

Plantilla operativa:

- [qa/phase-closure-checklist.md](qa/phase-closure-checklist.md)

## Comandos de calidad

Gate completo local:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Validacion documental rapida:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/quality/Test-DocumentationBaseline.ps1
```

Auditorias granulares:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-CorsAudit.ps1 -FailOnFindings
```

## Regla de actualizacion

Actualizar este roadmap cuando cambie cualquiera de estos elementos:

- fase activa o DoD de fase;
- decision de arquitectura;
- contrato API o modelo de auth;
- workflow CI/security o gate local;
- bloqueo real de BBDD, secrets, preview o produccion;
- propiedad de jefes/agentes.
