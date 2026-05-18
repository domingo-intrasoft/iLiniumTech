# Polizas write audit design

Fecha: 2026-05-18
Estado: diseno tecnico inicial, no implementado
SDD relacionada: `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`

## Proposito

Definir una auditoria minima y segura para escrituras de `Polizas` antes de evolucionar el CRUD local hacia entornos con mas usuarios, permisos y datos reales.

Este documento no crea tablas, endpoints, migraciones ni comportamiento runtime. Sirve como base para una SDD o ADR posterior si producto decide implementar auditoria persistente.

## No objetivos

- No registrar datos personales completos.
- No registrar connection strings, nombres internos de servidor, contrasenas, tokens, cookies ni cabeceras completas.
- No guardar SQL completo ni parametros sensibles.
- No copiar trazas, triggers o reglas AppBuilder como mecanismo runtime.
- No sustituir la auditoria real de BBDD si DBA define una fuente corporativa autorizada.

## Eventos candidatos

Eventos de escritura sobre `Polizas`:

- `polizas.create.requested`
- `polizas.create.succeeded`
- `polizas.create.rejected`
- `polizas.update.requested`
- `polizas.update.succeeded`
- `polizas.update.rejected`
- `polizas.delete.requested`
- `polizas.delete.succeeded`
- `polizas.delete.rejected`

Eventos de control:

- `polizas.write.disabled`
- `polizas.write.permission_denied`
- `polizas.write.validation_failed`
- `polizas.write.not_found_or_not_writable`
- `polizas.write.post_create_not_visible`
- `polizas.write.rollback`

## Campos permitidos

Campos tecnicos de correlacion:

- `timestampUtc`
- `correlationId`
- `requestId` si existe en infraestructura
- `operation`
- `result`
- `httpStatus`
- `errorCode` sanitizado
- `durationMs`

Contexto de autorizacion:

- `authMode`
- `userId` interno o pseudonimo, nunca nombre completo ni email
- `brokerId`
- `entityMainId`
- `profileId`
- `profileTypeId`
- permisos evaluados: `polizas.create`, `polizas.update`, `polizas.delete`
- `writesEnabled`

Contexto de recurso:

- `polizaId` tecnico (`dbo.Poliza.Id`)
- `polizaNumeroPrefix` limitado a marcador tecnico, por ejemplo `ILMVP-` o `ILMVP-DELETED-`
- `isMvpRecord`
- `changedFields` como lista de nombres de campo permitidos, sin valores

Contexto de seguridad:

- `clientIpHash` si se aprueba hash con sal por entorno
- `userAgentFamily` reducido, sin cadena completa
- `environment` (`Local`, `Demo`, `UAT`, `Production`)
- `repositoryMode` (`InMemory`, `Sql`)

## Campos prohibidos

No registrar:

- documento legal, NIF, CIF, NIE o pasaporte;
- nombre completo de cliente;
- telefono;
- email;
- direccion;
- cuenta bancaria o datos de cobro;
- matricula completa;
- bastidor;
- riesgo completo;
- prima, importes, comisiones o datos financieros salvo decision especifica de seguridad;
- connection strings, servidor, base de datos, usuario SQL o password;
- SQL completo, procedimientos completos o stack traces con parametros;
- cookies, tokens, API keys o cabeceras completas;
- payload completo de create/update/delete;
- valores antiguos y nuevos de campos sensibles.

## Minimizacion de cambios

Para `update`, la auditoria recomendada debe guardar solo:

- `changedFields`: nombres de campos permitidos;
- `fieldCount`;
- `validationProfile` o version de contrato si se define;
- resultado de autorizacion.

No debe guardar `oldValue` ni `newValue` salvo que una SDD posterior justifique un campo concreto como no sensible y UAT/seguridad lo aprueben.

## Retencion

Pendiente de decision corporativa.

Recomendacion inicial:

- Local/demo: retencion corta y no productiva.
- UAT: retencion alineada con pruebas y limpieza de entorno.
- Produccion: definir con seguridad, legal y DBA antes de implementar.

La retencion debe ser configurable por entorno y no debe depender de borrar manualmente ficheros locales.

## Correlacion y errores

- Toda respuesta de error publica ya deberia usar `correlationId` cuando aplique.
- La auditoria debe usar el mismo `correlationId` para unir API, servicio y repositorio.
- Los errores deben registrar codigos funcionales sanitizados, no mensajes SQL completos.
- Si una escritura falla despues de abrir transaccion, registrar `polizas.write.rollback` con causa funcional reducida.

## Permisos

La auditoria debe registrar el permiso requerido y si fue concedido o denegado.

Permisos actuales:

- `polizas.create`
- `polizas.update`
- `polizas.delete`

La API key MVP y los headers MVP no deben aparecer como autorizacion productiva. En entornos reales, el usuario y broker deben venir de auth/sesion backend aprobada.

## Preguntas UAT/DBA pendientes

- Donde debe vivir la auditoria persistente: API iLiniumTech, BBDD modelo, BBDD central, sistema corporativo externo o combinacion?
- Hay auditoria heredada en triggers de `dbo.Poliza` que deba conservarse o reconciliarse?
- Que retencion aplica por entorno?
- Que actor funcional necesita consultar auditoria y con que permisos?
- Se permite guardar numero completo de poliza para registros no MVP o debe registrarse solo `polizaId` tecnico?
- Que eventos se consideran obligatorios para UAT?
- Debe auditarse lectura de detalle de poliza como evento separado por PII?

## Criterios antes de implementar

- SDD o ADR aprobada.
- Modelo de auth productiva o regla demo/UAT explicitamente aceptada.
- Matriz de permisos real.
- Decision de retencion.
- Reglas DBA sobre triggers y auditoria existente.
- Tests de no exposicion de secretos y PII.
- Evidencia de que errores publicos siguen sanitizados.
