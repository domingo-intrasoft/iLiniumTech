# Decision datos reales locales - iLiniumTech

Fecha: 2026-05-19

## Decision

A partir de esta decision, el objetivo operativo del MVP iLiniumTech es trabajar con datos reales en la BBDD local del usuario para todas las pantallas funcionales del menu.

Esto sustituye el carril anterior donde las paginas distintas de Polizas podian evolucionar durante mas tiempo sobre fixtures o repositorios in-memory. Desde ahora, los fixtures e in-memory quedan como soporte de tests, desarrollo offline o fallback temporal documentado, pero no como objetivo funcional del producto.

## Alcance

Aplica a:

- Polizas.
- Agenda.
- Clientes.
- Recibos.
- Siniestros.
- Propuestas.
- Suplementos.
- Liquidaciones.
- Informes, estadisticas y superficies de consulta cuando se aborden.

No implica crear un CRUD generico ni recuperar AppBuilder como runtime dinamico. Cada pantalla sigue siendo un vertical Vue/API explicito.

## Reglas obligatorias

- Las conexiones reales se configuran solo por variables de entorno, secret store o configuracion local ignorada por Git.
- No se versionan connection strings, passwords, dumps, capturas con datos personales, resultados SQL con PII ni ficheros `.env`.
- La documentacion de evidencia debe ser sanitaria: estados HTTP, conteos, nombres de tablas/vistas cuando no sean sensibles, ids tecnicos sinteticos y resultados de pruebas sin datos personales.
- La lectura real debe pasar por repositorios explicitos, SQL parametrizado, whitelists y filtro broker/tenant.
- Las escrituras reales deben estar bloqueadas por defecto con flags `<Pagina>:WritesEnabled=false`.
- Cada escritura necesita SDD de escritura, permisos propios, transacciones, rollback o limpieza verificable, auditoria sin PII y smoke local.
- Si una pantalla contiene datos financieros, bancarios, salud, intervinientes, documentos o textos libres, primero se implementa lectura real minimizada y despues se decide el CRUD exacto.
- Los pantallazos usados para comparar UI no deben capturar datos personales reales salvo que el usuario los aporte conscientemente para UAT local.

## Implicacion para agentes IA

La siguiente IA no debe volver a plantear fixtures como objetivo de producto. Si no encuentra configuracion local suficiente para datos reales, debe:

1. dejar `SKIPPED_ENV_MISSING` en la evidencia;
2. no pedir ni imprimir secretos;
3. avanzar con el siguiente vertical que pueda documentarse o implementarse sin comprometer secretos;
4. mantener el cursor en `docs/PLAN_EJECUCION_CONTINUA_IA.md`.

## Orden recomendado

1. Mantener Polizas como vertical real de referencia.
2. Cerrar smoke SQL local de Agenda, que ya tiene API y frontend CRUD MVP.
3. Pasar Clientes a lectura real local minimizada antes de escritura.
4. Pasar Recibos y Siniestros a lectura real local minimizada, por riesgo financiero/intervinientes.
5. Confirmar origen real de Propuestas antes de CRUD.
6. Pasar Suplementos a lectura real local y despues decidir workflows/escrituras.
7. Abordar Liquidaciones, Informes y superficies tecnicas con SDD/threat model propio.

## Decision complementaria

La decision de arquitectura principal sigue vigente: iLiniumTech no es AppBuilder runtime. La metadata heredada se usa para analizar y trazar, no para renderizar pantallas ni ejecutar queries genericas en runtime.
