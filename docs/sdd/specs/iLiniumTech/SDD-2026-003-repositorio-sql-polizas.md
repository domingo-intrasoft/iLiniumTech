# SDD: Repositorio SQL whitelist Pantalla_Polizas

## Metadata

- Spec ID: SDD-2026-003
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: implementado parcialmente con bloqueos externos
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-13

## Contexto

El MVP actual usa `InMemoryPolizasRepository` con datos anonimizados. La siguiente evolucion debe leer `Pantalla_Polizas` de forma controlada, manteniendo paginacion, filtros y ordenaciones por whitelist.

`Pantalla_Polizas` puede depender de `SESSION_CONTEXT`, perfil, oficinas, gestores y permisos, por lo que la lectura real debe tratarse como superficie sensible.

Decision de arquitectura aplicable: el repositorio SQL forma parte del backend API de datos de iLiniumTech. No es un runtime generico de datasources AppBuilder ni debe interpretar metadata `IAP_*` en produccion.

Estado de implementacion: existe `SqlPolizasRepository` activable por configuracion, con query builder parametrizado, whitelist y pruebas unitarias. Tambien existe un resolver `AppBuilderMaster` que localiza la conexion `tipobd-MO` en `IAPM_Connection` por broker y puede descifrar campos con la clave de AppBuilder. El flujo MVP ya puede tomar broker/contexto por request mediante cabeceras solo con opt-in explicito, mantiene `Polizas:BrokerId` como fallback local temporal y aplica `SESSION_CONTEXT` parametrizado antes de consultar. La readiness bloquea entornos no Development si se habilitan headers MVP fuera de desarrollo sin opt-in demo explicito, y el resolver `AppBuilderMaster` no confia en certificados de servidor por defecto. Queda pendiente validarlo contra BBDD real de test o contenedor y sustituir cabeceras MVP por auth real.

Dependencia de seguridad: `SDD-2026-005` define el modelo objetivo para sustituir headers MVP por claims/sesion backend, permisos efectivos y errores 401/403 sanitizados.

## Objetivo

Sustituir progresivamente el repositorio in-memory por un repositorio SQL de solo lectura, parametrizado y limitado por contrato de producto iLiniumTech.

La metadata extraida puede ayudar a crear la whitelist inicial, pero la whitelist final debe vivir como codigo/configuracion propia revisada del backend, no como lectura runtime de AppBuilder.

## Fuera de alcance

- Escritura de polizas.
- Acciones de anulacion, duplicado, suspension, reemplazo o revigorizacion.
- Ejecucion directa de SQL dinamico AppBuilder.
- Interpretacion runtime de `IAP_DataSource`, `QueryStatic` o fragments SQL heredados.
- Workflows, REST/SOAP externos y motor completo de expresiones.
- Exponer campos personales no necesarios para el MVP.

## Contrato de datos

Entrada:

- filtros soportados por `PolizasSearchRequest`;
- pagina y tamano de pagina;
- sort declarado en el contrato de producto/whitelist backend;
- contexto de request con broker efectivo;
- contexto de usuario/perfil cuando se defina autenticacion real.

Salida:

- `PagedResult<PolizaListItem>`;
- `PolizaDetail` sanitizado;
- errores de validacion sin SQL ni trazas internas.

Campos permitidos inicialmente:

- `Poliza`;
- `Aplicacion`;
- `IdTipoPoliza`;
- `NumDocumento`;
- `IdSituacion`;
- `IdRamo`;
- `Riesgo`;
- `F_Efecto`;
- `F_Vencimiento`;
- `F_Anulacion`;
- `IdMotivoAnulacion`;
- `Cia`;
- `PAnualCartera`;
- `NombreCompleto`;
- alertas documentadas.

## Reglas de negocio

- Solo lectura.
- Nombres de tabla, vista, columna y ordenacion validados por whitelist.
- Valores siempre parametrizados.
- Query object propio del backend; no concatenacion de fragments AppBuilder.
- Limite maximo de `pageSize`.
- Sin logs de datos personales innecesarios.
- Cuenta BBDD con minimo privilegio.
- El frontend consume endpoints estables; no necesita conocer metadata `IAP_*` para buscar polizas.

## Criterios de aceptacion

- [x] Existe implementacion SQL detras de `IPolizasRepository`.
- [x] La activacion del repositorio SQL se hace por configuracion, no por cambio de codigo.
- [x] Existe resolver de conexion de modelo desde Master por broker para entornos AppBuilder.
- [x] La whitelist usada por el repositorio esta definida en codigo/configuracion iLiniumTech revisada.
- [x] No hay lectura runtime de `IAP_*` para construir la consulta productiva.
- [x] Sort malicioso o desconocido devuelve error de validacion.
- [x] Filtros maliciosos no alteran SQL estructural.
- [x] El endpoint mantiene API key obligatoria.
- [x] Los errores de validacion no devuelven SQL, connection strings ni trazas internas.
- [x] Hay pruebas unitarias del query object y whitelist.
- [x] El broker efectivo se resuelve por request y no por configuracion global en escenarios MVP multi-broker.
- [x] El fallback `Polizas:BrokerId` queda limitado a ejecucion local/controlada.
- [x] `SESSION_CONTEXT` se establece antes de consultar polizas si las vistas/tablas lo requieren.
- [ ] Auth real sustituye la confianza en headers MVP para broker, usuario y perfil.
- [ ] Hay pruebas de integracion con BBDD de test, contenedor o fixture SQL controlado.

## Impacto tecnico

Backend:

- `iLiniumTech.Backend.Infrastructure/Polizas`.
- `iLiniumTech.Backend.Application/Polizas`.
- `iLiniumTech.Backend.Tests/Polizas`.
- Contrato API de polizas estable para frontend Vue estatico.

Configuracion:

- Variables de entorno o secret store para conexion real.
- Sin connection strings reales en `appsettings`.
- `Polizas:Repository=InMemory` por defecto.
- `Polizas:Repository=Sql` activa `SqlPolizasRepository`.
- `ConnectionStrings:PolizasReadOnly` o `ILINIUMTECH__POLIZAS_CONNECTION` aporta la conexion local.
- `Polizas:ConnectionResolver=AppBuilderMaster` activa resolucion por `IAPM_Connection`.
- `ConnectionStrings:AppBuilderMaster` o `ILINIUMTECH__APPBUILDER_MASTER_CONNECTION` aporta la conexion Master.
- `AppBuilder:EncryptionKey` o `ILINIUMTECH__APPBUILDER_ENCRYPTION_KEY` descifra campos de conexion si vienen cifrados.
- `Polizas:AppBuilderMaster:TrustServerCertificate` o `ILINIUMTECH__APPBUILDER_MASTER_TRUST_SERVER_CERTIFICATE` permite `TrustServerCertificate=true` solo por configuracion explicita para local/demo/test. El valor por defecto es seguro: `false`.
- `Polizas:BrokerId` o `ILINIUMTECH__BROKER_ID` queda como fallback temporal para ejecucion local sin broker por request.
- `Polizas:AllowHeaderExecutionContext` o `ILINIUMTECH__ALLOW_HEADER_EXECUTION_CONTEXT` permite leer cabeceras MVP de contexto. Debe estar desactivado por defecto fuera de entornos controlados.
- Si `Polizas:AllowHeaderExecutionContextOutsideDevelopment=true` queda configurado fuera de Development, `/ready` debe devolver `not_ready` salvo opt-in demo exacto con `Polizas:AllowHeaderExecutionContextDemoOptIn=DEMO_ONLY_NOT_FOR_REAL_DATA` o `ILINIUMTECH__ALLOW_HEADER_EXECUTION_CONTEXT_DEMO_OPT_IN=DEMO_ONLY_NOT_FOR_REAL_DATA`.
- `Polizas:UserId` o `ILINIUMTECH__USER_ID`, `Polizas:ProfileId` o `ILINIUMTECH__PROFILE_ID`, `Polizas:ProfileTypeId` o `ILINIUMTECH__PROFILE_TYPE_ID`, y `Polizas:IsAdmin` o `ILINIUMTECH__IS_ADMIN` son fallback temporal para completar contexto local.

Headers MVP:

- `X-ILiniumTech-Api-Key`: requerido actualmente para `/api/polizas/*`; valor configurado con `ApiSecurity:ApiKey` o `ApiSecurity__ApiKey`.
- `X-Broker-Id`: previsto para resolver el broker efectivo por request en el MVP, hasta que exista autenticacion real. Requiere `Polizas:AllowHeaderExecutionContext=true` y no debe considerarse prueba de identidad.
- `X-User-Id`, `X-Profile-Id`, `X-Profile-Type-Id`, `X-Is-Admin`: previstos solo como contexto MVP opcional para `SESSION_CONTEXT`; no deben considerarse prueba de identidad, perfil ni permiso.
- `Authorization`: pendiente de autenticacion real. Cuando exista, el backend debe derivar `brokerId`, `userId`, `profileId`, `profileTypeId` e `isAdmin` desde claims/sesion autorizada.

`SESSION_CONTEXT` previsto:

- Claves candidatas: `brokerId`, `entityMainId`, `userId`, `profileId`, `profileTypeId`, `isAdmin`, `ip`, `userAgent`.
- Debe configurarse por conexion SQL y antes de ejecutar las consultas de polizas.
- Debe usar `sp_set_session_context` con parametros.
- Queda pendiente confirmar con DBA/vistas reales que claves son obligatorias y si hay claves adicionales.

## Seguridad

- [x] Secretos fuera de Git.
- [x] SQL parametrizado.
- [x] Whitelist para estructura SQL.
- [ ] Resolver por broker validado contra Master real autorizado.
- [ ] Broker por request validado contra usuario/sesion real cuando exista autenticacion.
- [x] Headers MVP tratados como contexto temporal, no como identidad confiable.
- [x] `/ready` falla fuera de Development si se habilitan headers MVP con override externo sin opt-in demo explicito.
- [x] `TrustServerCertificate` en el resolver `AppBuilderMaster` queda en `false` por defecto y solo se activa por configuracion local/demo/test.
- [x] `SESSION_CONTEXT` parametrizado cuando aplique.
- [x] `QueryStatic` y fragments heredados se tratan como evidencia, no como ejecutable.
- [ ] Auth y autorizacion preservadas.
- [ ] Logs con redaccion de datos sensibles.
- [x] Pruebas de inyeccion para filtros y ordenacion.

## Plan de pruebas

- Unitarias: construccion de query, validacion de columnas, limites de pagina.
- Integracion actual: endpoint protegido, catalogos, listado y detalle sobre repositorio in-memory.
- Integracion pendiente: lectura SQL con BBDD de test/contenedor/datos anonimizados.
- Integracion pendiente: broker por request resuelve conexiones distintas sin reutilizar estado global.
- Integracion pendiente: `SESSION_CONTEXT` se aplica y se limpia/aisla entre requests.
- E2E/smoke: `/polizas` muestra datos o estado vacio controlado.
- Seguridad: payloads con `;`, comentarios SQL, subqueries y columnas inexistentes.
- Manual/UAT: contrastar resultados con entorno de pruebas usando usuario autorizado.

## Estado de cierre QA/UAT

Completado con evidencia:

- Backend SQL read-only configurable: `Polizas:Repository=Sql` activa `SqlPolizasRepository`; `InMemory` sigue como default seguro.
- Valores de filtros, detalle y `SESSION_CONTEXT` se aplican mediante parametros.
- Estructura SQL y sort se limitan por whitelist iLiniumTech, sin interpretar `IAP_*` ni `QueryStatic` en runtime.
- Broker/contexto MVP por request existe mediante `X-Broker-Id`, `X-User-Id`, `X-Profile-Id`, `X-Profile-Type-Id` y `X-Is-Admin`, solo con `Polizas:AllowHeaderExecutionContext=true` y restricciones de entorno.
- `SESSION_CONTEXT` se aplica por conexion antes de queries de busqueda/detalle y se limpia con `NULL` si no hay contexto.
- Pruebas versionadas cubren query builder, whitelist, payloads maliciosos, contexto por cabecera, `/api/me`, resolver AppBuilderMaster y `SESSION_CONTEXT`.

Pendiente tecnico:

- Sustituir cabeceras MVP por auth/autorizacion real.
- Ejecutar integracion SQL con BBDD de test, contenedor o fixture SQL controlado.
- Confirmar y documentar mascarado/recorte final de campos sensibles cuando se use dato real.

Bloqueado externo:

- Falta BBDD de test o contenedor SQL Server autorizado.
- Falta confirmacion DBA de claves obligatorias/adicionales de `SESSION_CONTEXT`.
- Falta proveedor/mecanismo de autenticacion real y mapa funcional de permisos.
- Falta validacion de cuenta read-only contra Master/modelo reales.

## Riesgos

- Las restricciones reales pueden depender de contexto de sesion no presente en tests.
- Un broker global por configuracion puede mezclar datos entre requests si se usa en un escenario multi-broker.
- Headers MVP de broker/perfil pueden ser manipulados hasta que exista autenticacion/autorizacion real.
- El modelo puede devolver campos con datos personales que deben recortarse.
- La query original de AppBuilder puede contener logica no portable al producto iLiniumTech.
- Si se intenta reutilizar metadata como runtime, se reintroducen riesgos de AppBuilder y se rompe la decision de arquitectura.

## Work Items

- Crear issue GitHub: `SDD-2026-003 Repositorio SQL whitelist Pantalla_Polizas`.
- Crear issue tecnico: pruebas de inyeccion de filtros y sort.
- Crear issue tecnico: definir contexto de usuario/perfil para lectura.
- Crear issue tecnico: implementar broker por request y retirar dependencia de `Polizas:BrokerId` para escenarios MVP compartidos.
- Crear issue tecnico: aplicar `SESSION_CONTEXT` parametrizado cuando se confirme dependencia real.
- Crear issue tecnico: sustituir headers MVP por autenticacion/autorizacion real.
- Crear issue tecnico: documentar whitelist backend derivada de metadata revisada.

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [x] Documentacion actualizada.
