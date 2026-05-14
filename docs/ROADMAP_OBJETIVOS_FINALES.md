# Roadmap de objetivos finales iLiniumTech

Fecha: 2026-05-14

## Proposito

Esta guia fija el camino de alto nivel para llevar iLiniumTech desde el MVP actual de Polizas hasta un producto profesional, seguro, mantenible y desplegable. Sirve como mapa de grandes objetivos: las SDD concretas, issues y PRs deben colgar de estas fases.

Decision base: iLiniumTech no sera un runtime dinamico tipo AppBuilder. AppBuilder queda como fuente heredada de conocimiento, extraccion, trazabilidad y scaffolding revisado. El producto final sera una aplicacion Vue estatica con backend API propio, contratos explicitos, seguridad por diseno y CI/CD gobernado.

## Norte del producto

Objetivo final:

- producto web profesional para operar funcionalidades de negocio empezando por Polizas;
- frontend Vue 3 mantenible, probado y no dependiente de metadata AppBuilder en runtime;
- backend .NET API con contratos versionados, autorizacion, auditoria y acceso a datos controlado;
- extractor offline seguro para convertir conocimiento AppBuilder en SDD, inventarios y scaffolding revisado;
- pipeline CI/CD con gates obligatorios de build, test, E2E, seguridad y revision;
- despliegue reproducible por entornos, con secretos fuera de Git y observabilidad real.

Principios no negociables:

- SDD antes de cambios funcionales relevantes.
- Ningun secreto, connection string real, dump ni dato personal en Git.
- Toda lectura SQL real debe pasar por repositorios explicitos, parametros y whitelists.
- La UI productiva se define como codigo fuente revisado, no como render dinamico de `IAP_*`.
- La autorizacion vive en backend; la visibilidad de UI no es seguridad.
- Cada fase debe cerrar con pruebas y criterios verificables.

## Estado actual de partida

Ya existe:

- organizacion GitHub `intrasoft-ilinium` y repositorio transferido;
- ramas `main`, `develop` y `ci-cd`;
- proteccion de `main` con checks requeridos;
- workflows `ci` y `security`;
- backend .NET para Polizas read-only;
- frontend Vue para `/polizas` y `/polizas/:id`;
- repositorio SQL activable por configuracion y fixtures anonimizados;
- paginacion y filtros de fecha;
- runbook MVP local;
- smoke E2E Playwright local;
- documentacion SDD y decision de arquitectura.

La rama de producto estable para seguir funcionalidad es `develop`. La rama de plataforma para evolucionar CI/CD y gobierno es `ci-cd`. `main` debe recibir solo cortes estabilizados.

## Fase 1 - Gobierno tecnico y CI/CD profesional

Objetivo: que cada cambio importante tenga una puerta de calidad automatica, repetible y visible.

Pasos:

1. Incorporar Playwright E2E al workflow `ci / frontend` desde la rama `ci-cd`.
2. Definir convencion de ramas y PRs:
   - `develop` para producto;
   - `ci-cd` para plataforma, workflows y gobierno;
   - `feature/*` o `codex/*` para trabajo incremental;
   - PRs a `main` solo desde ramas estabilizadas.
3. Alinear checks requeridos de `main` con los jobs reales del workflow.
4. Crear reglas de proteccion para `develop` cuando el flujo este maduro:
   - PR obligatorio;
   - checks `ci` y `security`;
   - revision minima;
   - sin force push.
5. Separar artefactos de CI:
   - resultados backend;
   - resultados frontend unitarios;
   - reporte E2E;
   - reportes de seguridad.
6. Documentar runbook de release:
   - que entra en `develop`;
   - como se promueve a `main`;
   - como se etiqueta una version;
   - como se revierte un despliegue.

Entregables:

- workflows actualizados;
- documento de release;
- checks remotos verdes;
- reglas de ramas documentadas y aplicadas.

Criterio de salida:

- una PR de producto no puede entrar sin build, unit tests, E2E smoke, secret scan, dependency audit y CORS audit verdes.

## Fase 2 - Seguridad e identidad MVP

Objetivo: sustituir los mecanismos temporales de demo por un modelo de identidad, autorizacion y contexto de broker defendible.

Pasos:

1. Definir SDD de autenticacion/autorizacion MVP:
   - proveedor de identidad previsto;
   - claims minimos;
   - usuario;
   - broker activo;
   - perfil;
   - permisos por modulo.
2. Sustituir `X-Broker-Id` como fuente confiable por contexto autenticado.
3. Mantener `X-Broker-Id` solo para desarrollo controlado, detras de opt-in explicito.
4. Convertir la API key actual en mecanismo de demo/dev o service-to-service temporal, no en autenticacion final de usuario.
5. Definir modelo de autorizacion backend:
   - permiso para listar polizas;
   - permiso para ver detalle;
   - reglas por broker/oficina/gestor/perfil;
   - errores 401/403 consistentes.
6. Sanitizar logs y errores:
   - sin connection strings;
   - sin SQL completo;
   - sin datos personales innecesarios;
   - correlation id por request.
7. Revisar CORS por entorno y documentar origenes permitidos.

Entregables:

- SDD de seguridad e identidad;
- middleware/auth handlers reales;
- pruebas de autorizacion backend;
- actualizacion de runbooks y variables.

Criterio de salida:

- ningun endpoint de datos reales depende de headers manipulables como identidad confiable.

## Fase 3 - Datos reales Polizas con calidad de producto

Objetivo: que el vertical Polizas lea datos reales de forma segura, trazable y validada contra un entorno autorizado.

Pasos:

1. Validar esquema real de `Pantalla_Polizas` y restricciones asociadas:
   - columnas;
   - tipos;
   - indices;
   - `SESSION_CONTEXT`;
   - permisos por broker/perfil.
2. Crear entorno de integracion controlado:
   - BBDD de test o replica autorizada;
   - usuario read-only;
   - secretos gestionados fuera de Git.
3. Sustituir usos fragiles de acceso SQL por parametros tipados cuando aplique.
4. Ampliar filtros soportados:
   - numero de poliza;
   - cliente/documento;
   - compania;
   - ramo;
   - estado;
   - fechas;
   - ordenacion whitelist.
5. Definir contrato estable de respuesta:
   - paginacion;
   - campos nulos;
   - formatos de fecha;
   - importes y moneda;
   - codigos frente a etiquetas.
6. Anadir pruebas de integracion SQL:
   - query builder;
   - limites de paginacion;
   - filtros;
   - ausencia de inyeccion estructural;
   - contexto de broker requerido.
7. Preparar UAT funcional con datos anonimizados o autorizados.

Entregables:

- SDD Polizas datos reales;
- repositorio SQL endurecido;
- suite de integracion;
- evidencias UAT;
- contrato API actualizado.

Criterio de salida:

- `/polizas` funciona contra backend real en entorno controlado sin exponer secretos ni saltarse autorizacion.

## Fase 4 - Experiencia Polizas profesional

Objetivo: convertir la pantalla MVP en una herramienta de trabajo usable, clara y preparada para usuarios reales.

Pasos:

1. Revisar UX con criterios operativos:
   - busqueda rapida;
   - filtros relevantes;
   - lectura densa pero clara;
   - estados loading, empty, error y forbidden;
   - accesibilidad basica de teclado y foco.
2. Completar tabla:
   - columnas finales;
   - ordenacion;
   - paginacion robusta;
   - persistencia de filtros en URL si aporta valor;
   - formato de importes y fechas.
3. Completar detalle read-only:
   - resumen;
   - datos de cliente;
   - vigencia;
   - compania/ramo;
   - alertas;
   - auditoria visible si procede.
4. Definir acciones futuras como placeholders controlados o retirarlas hasta tener SDD:
   - duplicar;
   - anular;
   - suspender;
   - reemplazar;
   - revigorizar.
5. Ampliar pruebas frontend:
   - unitarias de filtros;
   - componentes;
   - E2E de busqueda;
   - E2E de detalle;
   - E2E de error sin backend/contexto.
6. Validar con usuarios o responsables funcionales.

Entregables:

- pantalla Polizas v1 funcional;
- suite E2E ampliada;
- checklist UAT;
- incidencias funcionales priorizadas.

Criterio de salida:

- un usuario puede localizar y revisar una poliza real con confianza, sin entender AppBuilder ni configuracion tecnica.

## Fase 5 - Extractor AppBuilder seguro y trazabilidad SDD

Objetivo: convertir AppBuilder en una fuente controlada de conocimiento, no en una dependencia productiva.

Pasos:

1. Crear SDD del extractor offline:
   - alcance;
   - tablas permitidas;
   - datos prohibidos;
   - formato de salida;
   - reglas de sanitizacion.
2. Implementar extractor read-only:
   - conexion por entorno/secret store;
   - sin logging de credenciales;
   - consultas parametrizadas;
   - salida JSON sanitizada.
3. Extraer inventario de Polizas:
   - componentes;
   - datasources;
   - campos;
   - acciones;
   - permisos observados;
   - dependencias REST/SOAP/workflow si existen.
4. Generar comparativa AppBuilder vs iLiniumTech:
   - que se migra;
   - que se descarta;
   - que requiere SDD propia;
   - riesgos.
5. Usar la salida solo para:
   - documentacion;
   - scaffolding inicial;
   - pruebas;
   - trazabilidad UAT.
6. Preparar el extractor para otros modulos cuando Polizas este probado.

Entregables:

- herramienta de extraccion;
- JSON sanitizados;
- specs SDD derivadas;
- inventario de brechas.

Criterio de salida:

- se puede explicar que viene de AppBuilder, que se implemento en iLiniumTech y por que, sin depender de metadata en runtime.

## Fase 6 - Operacion, despliegue y observabilidad

Objetivo: que el producto pueda ejecutarse en entornos reales con configuracion, secretos, logs y diagnostico profesionales.

Pasos:

1. Definir entornos:
   - local;
   - integracion;
   - preproduccion;
   - produccion.
2. Definir configuracion por entorno:
   - variables publicas frontend;
   - settings backend;
   - secretos;
   - origenes CORS;
   - connection strings.
3. Crear estrategia de despliegue:
   - build frontend estatico;
   - backend API;
   - migraciones si aplican;
   - smoke post-deploy.
4. Anadir observabilidad:
   - logs estructurados;
   - correlation id;
   - health checks;
   - metricas basicas;
   - errores trazables sin datos sensibles.
5. Definir backup/rollback:
   - versionado;
   - rollback de artefactos;
   - invalidacion de configuracion erronea;
   - respuesta ante secreto expuesto.
6. Documentar runbooks operativos:
   - levantar entorno;
   - diagnosticar API;
   - comprobar CORS;
   - revisar logs;
   - recuperar despliegue.

Entregables:

- runbook de despliegue;
- configuracion por entorno;
- health checks;
- logging estructurado;
- smoke post-deploy.

Criterio de salida:

- una version puede promoverse y diagnosticarse sin intervencion artesanal ni secretos en archivos locales versionados.

## Fase 7 - Ampliacion funcional controlada

Objetivo: crecer mas alla de Polizas sin crear deuda estructural ni repetir AppBuilder.

Pasos:

1. Priorizar siguientes modulos segun valor y riesgo.
2. Para cada modulo, crear SDD:
   - alcance;
   - contrato API;
   - reglas de autorizacion;
   - datos personales;
   - pruebas;
   - trazabilidad AppBuilder.
3. Reutilizar patrones ya probados:
   - repositorios read-only;
   - query objects;
   - whitelists;
   - componentes Vue;
   - E2E smoke;
   - runbooks.
4. Introducir escrituras solo con una fase especifica:
   - validacion de dominio;
   - permisos;
   - auditoria;
   - idempotencia cuando aplique;
   - pruebas transaccionales.
5. Evitar plataformas genericas prematuras:
   - no motor dinamico de workflows;
   - no SQL libre;
   - no permisos solo configurados en UI;
   - no pantallas generadas en runtime.

Entregables:

- backlog SDD priorizado;
- modulo 2 implementado con la misma calidad que Polizas;
- patrones reutilizables documentados.

Criterio de salida:

- el segundo modulo demuestra que la arquitectura escala sin copiar el motor AppBuilder.

## Fase 8 - Profesionalizacion continua

Objetivo: mantener calidad alta cuando el producto y el equipo crezcan.

Pasos:

1. Crear definition of done por tipo de cambio:
   - feature;
   - bugfix;
   - seguridad;
   - datos;
   - CI/CD;
   - documentacion.
2. Mantener ADRs o decisiones tecnicas cuando cambie arquitectura.
3. Medir calidad:
   - tiempo de CI;
   - fallos de tests;
   - cobertura util;
   - vulnerabilidades;
   - deuda tecnica priorizada.
4. Establecer cadencia:
   - grooming SDD;
   - revisiones de seguridad;
   - revisiones de dependencias;
   - cortes de release.
5. Preparar onboarding:
   - como levantar el repo;
   - como crear una SDD;
   - como abrir PR;
   - como validar localmente;
   - como no manejar secretos.

Entregables:

- definition of done oficial;
- backlog tecnico visible;
- guia de onboarding;
- cadencia de revision.

Criterio de salida:

- la calidad no depende de memoria individual; queda en procesos, checks y documentacion.

## Secuencia recomendada inmediata

Orden recomendado desde el estado actual:

1. `ci-cd`: meter Playwright E2E en GitHub Actions y actualizar checks.
2. `develop`: SDD de identidad/contexto de broker.
3. `develop`: pruebas y contrato de autorizacion backend.
4. `develop`: integracion SQL real en entorno controlado.
5. `develop`: completar UX Polizas v1 y UAT.
6. `ci-cd`: runbook de release y proteccion gradual de `develop`.
7. `develop`: extractor offline AppBuilder con salida sanitizada.
8. `main`: primer corte estable cuando Polizas v1 tenga UAT y gates verdes.

## Como usar esta guia

- Cada fase debe convertirse en issues o SDD pequenas antes de implementarse.
- Si una fase descubre un riesgo mayor, se actualiza esta guia y la SDD afectada.
- Las PRs deben mencionar la fase y el objetivo que avanzan.
- No se debe saltar a escrituras o workflows hasta cerrar identidad, autorizacion y auditoria.
- El objetivo no es ir mas lento; es avanzar sin construir una segunda deuda heredada.
