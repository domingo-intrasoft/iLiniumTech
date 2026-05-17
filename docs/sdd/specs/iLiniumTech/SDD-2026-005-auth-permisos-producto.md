# SDD: Autenticacion, autorizacion y permisos de producto

## Metadata

- Spec ID: SDD-2026-005
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: security
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-14

## Contexto

El MVP de polizas usa una API key, `demo-session` y cabeceras temporales para acelerar la demo multi-broker y el `SESSION_CONTEXT`: `X-Broker-Id`, `X-User-Id`, `X-Profile-Id`, `X-Profile-Type-Id` y `X-Is-Admin`. Esas entradas no prueban identidad productiva, perfil ni permisos reales. Solo son bootstrap de desarrollo/demo mientras no exista autenticacion real.

iLiniumTech debe evolucionar a un modelo profesional donde el backend derive usuario, broker, perfil, roles y permisos desde claims o sesion backend validada. El frontend Vue debe seguir consumiendo una API explicita y estable; no debe conocer ni interpretar metadata AppBuilder para decidir permisos.

Decision de arquitectura aplicable: la metadata AppBuilder puede servir como evidencia para mapear permisos historicos, pero no como runtime dinamico. Los permisos productivos viven como politicas y contratos iLiniumTech revisados.

## Objetivo

Definir el contrato de autenticacion, autorizacion y permisos que otro agente pueda implementar despues, sin programar runtime en esta SDD.

El resultado esperado es:

- identidad real pendiente de proveedor, pero con forma de claims/sesion estable;
- headers MVP declarados como no confiables para produccion;
- reglas para `currentUserId`, `currentBrokerId`, roles, permisos y contexto SQL;
- comportamiento 401/403 y errores sanitizados con `correlationId`;
- estrategia incremental para migrar sin romper contratos del frontend.

## Fuera de alcance

- Elegir proveedor final de identidad.
- Implementar middleware, filtros, politicas, cookies, JWT o login.
- Cambiar codigo backend o frontend.
- Escribir adaptadores contra directorios corporativos, Microsoft Entra ID, Google, Auth0, IdentityServer u otro proveedor.
- Modelar escrituras, workflows o acciones fuera del read-only.
- Ejecutar permisos heredados de AppBuilder como motor generico runtime.
- Guardar secretos, claves de firma, connection strings reales o dumps.

## Contrato de datos

### Entrada de autenticacion objetivo

El backend debe aceptar uno de estos mecanismos cuando se apruebe proveedor:

- `Authorization: Bearer <token>` validado por issuer, audience, firma y expiracion; o
- sesion backend con cookie `HttpOnly`, `Secure` y `SameSite` cuando el proveedor elegido lo requiera.

La eleccion queda bloqueada por decision humana de proveedor. La API no debe acoplar sus contratos de negocio al proveedor.

### Claims o sesion esperados

El backend debe construir un contexto interno por request con estos campos normalizados:

- `currentUserId`: identificador interno estable del usuario autenticado.
- `currentBrokerId`: broker efectivo del request, validado contra brokers permitidos del usuario.
- `profileId`: perfil funcional efectivo, si aplica.
- `profileTypeId`: tipo de perfil, si aplica.
- `isAdmin`: bandera derivada del backend o del proveedor validado; nunca aceptada desde frontend como autoridad.
- `roles`: roles de alto nivel, usados para agrupar permisos, no como unica autorizacion fina.
- `permissions`: permisos efectivos normalizados por producto.
- `allowedBrokerIds`: brokers que el usuario puede seleccionar o consultar.
- `displayName` y `email`: opcionales para UI; tratar como datos personales y no loguear salvo necesidad justificada.

Nombres de permisos iniciales propuestos para el proximo incremento:

- `polizas.catalogs`: permite consultar catalogos necesarios para filtros.
- `polizas.read`: permite consultar listado read-only.
- `polizas.detail`: permite consultar detalle read-only.
- `polizas.export`: reservado para exportacion futura; no concedido por defecto.
- `admin.security.view`: reservado para diagnostico administrativo futuro; no forma parte del MVP.

Estos tres permisos de polizas son el contrato objetivo del incremento de permisos/broker autorizado. Pueden alimentarse temporalmente desde `demo-session` en entornos controlados, pero deben modelarse como politicas backend explicitas y no como metadata AppBuilder runtime. Los nombres definitivos para escrituras, exportaciones o administracion deben cerrarse con producto/UAT antes de implementarlos.

### Incremento permisos/broker autorizado

El siguiente incremento tecnico debe aplicar estas reglas minimas:

- `GET /api/polizas/catalogs` requiere `polizas.catalogs`.
- `GET /api/polizas` requiere `polizas.read`.
- `GET /api/polizas/{id}` requiere `polizas.detail`.
- Antes de resolver conexion SQL o ejecutar repositorio real, `currentBrokerId` debe existir y pertenecer a `allowedBrokerIds`.
- El detalle no debe permitir leer una poliza de otro broker aunque el usuario tenga `polizas.detail`.
- API key MVP puede seguir protegiendo rutas durante la transicion, pero no concede permisos por si sola.
- `demo-session` puede emitir permisos demo para desarrollo/UAT controlada, pero no sustituye proveedor auth productivo.
- Headers MVP solo pueden completar contexto local/demo con opt-in y no pueden elevar permisos, broker, perfil ni `isAdmin`.

### Compatibilidad temporal API key, demo-session y headers MVP

Estos mecanismos quedan permitidos solo como bootstrap temporal y no confiable para produccion:

- `X-ILiniumTech-Api-Key`: proteccion actual de desarrollo/demo. No identifica usuario final ni reemplaza autenticacion real.
- `demo-session`: sesion backend demo con cookie `HttpOnly` para revisar flujo login, `/api/me`, permisos y broker autorizado sin proveedor productivo.
- `X-Broker-Id`: seleccion temporal de broker si `Polizas:AllowHeaderExecutionContext=true`.
- `X-User-Id`, `X-Profile-Id`, `X-Profile-Type-Id`, `X-Is-Admin`: contexto temporal para `SESSION_CONTEXT` en entornos controlados.

Reglas obligatorias:

- fuera de desarrollo/demo no deben habilitarse como fuente de identidad o permisos productivos;
- si se habilitan fuera de Development mediante override temporal, el runtime debe ignorarlos y `/ready` debe quedar `not_ready` salvo opt-in demo exacto y visible: `Polizas:AllowHeaderExecutionContextDemoOptIn=DEMO_ONLY_NOT_FOR_REAL_DATA` o `ILINIUMTECH__ALLOW_HEADER_EXECUTION_CONTEXT_DEMO_OPT_IN=DEMO_ONLY_NOT_FOR_REAL_DATA`;
- si coexisten con autenticacion real, los claims/sesion tienen prioridad;
- cualquier valor de header debe validarse contra el contexto autenticado antes de influir en datos;
- `X-Is-Admin` nunca concede permisos por si solo;
- API key MVP solo demuestra conocimiento de una clave tecnica de entorno, no usuario final, broker autorizado ni permiso funcional;
- `demo-session` debe quedar claramente etiquetada como demo y reemplazable por el proveedor auth aprobado;
- los logs pueden registrar que se uso modo MVP, pero no valores personales o sensibles.

### Contexto SQL

Si `Pantalla_Polizas` o vistas reales requieren `SESSION_CONTEXT`, el backend debe poblarlo desde el contexto autenticado normalizado:

- `brokerId` o `entityMainId`: desde `currentBrokerId`;
- `userId`: desde `currentUserId`;
- `profileId`, `profileTypeId`, `isAdmin`: desde claims/sesion validados;
- `ip` y `userAgent`: solo si hay necesidad de auditoria y con redaccion en logs.

Las claves finales dependen de confirmacion DBA. La escritura de `SESSION_CONTEXT` debe ser parametrizada y aislada por request/conexion.

### Respuesta de contexto para frontend

El frontend debe depender de un contrato estable tipo `/api/me` o equivalente, no de metadata AppBuilder. Respuesta conceptual:

```json
{
  "currentUserId": "user-123",
  "currentBrokerId": "broker-456",
  "profileId": "profile-789",
  "profileTypeId": "type-1",
  "isAdmin": false,
  "roles": ["broker-user"],
  "permissions": ["polizas.read", "polizas.detail", "polizas.catalogs"],
  "correlationId": "request-correlation-id"
}
```

Los valores anteriores son ejemplos sanitizados, no datos reales.

## Reglas de negocio

- Autenticacion obligatoria por defecto en endpoints de producto, salvo endpoints publicos documentados como `/health`.
- Autorizacion siempre en backend por endpoint y operacion; la UI solo mejora experiencia, no protege datos.
- El broker efectivo debe estar dentro de `allowedBrokerIds`.
- Un usuario autenticado sin broker efectivo valido no puede leer polizas.
- Un usuario autenticado con broker no incluido en `allowedBrokerIds` no puede consultar catalogos, listado ni detalle.
- Un usuario con broker valido pero sin permiso `polizas.catalogs` no puede consultar catalogos.
- Un usuario con broker valido pero sin permiso `polizas.read` no puede listar polizas.
- Un usuario con broker valido pero sin permiso `polizas.detail` no puede consultar detalle.
- `currentUserId` y `currentBrokerId` deben existir antes de resolver conexion SQL real.
- La API key MVP no se considera identidad de usuario.
- `demo-session` no se considera autenticacion productiva real.
- Roles agrupan permisos, pero las politicas backend deben comprobar permisos efectivos.
- Los permisos heredados de AppBuilder pueden usarse como referencia de analisis o migracion, no como motor runtime generico.
- No se deben loguear tokens, cookies, claims completos, email, nombre, connection strings, SQL ni datos personales de polizas.

## Criterios de aceptacion

- [ ] Existe decision humana de proveedor o mecanismo auth antes de programar produccion.
- [ ] El backend deriva `currentUserId`, `currentBrokerId`, `profileId`, `profileTypeId`, `isAdmin`, roles y permisos desde claims/sesion validada.
- [ ] Los headers MVP solo funcionan con opt-in de entorno controlado y no conceden permisos de produccion.
- [ ] Si coexisten headers y auth real, el backend prioriza claims/sesion y valida cualquier seleccion de broker.
- [ ] `GET /api/me` o contrato equivalente expone contexto minimo para frontend sin datos sensibles innecesarios.
- [ ] `GET /api/polizas/catalogs` exige `polizas.catalogs`.
- [ ] `GET /api/polizas` exige `polizas.read`.
- [ ] `GET /api/polizas/{id}` exige `polizas.detail`.
- [ ] Broker activo validado contra `allowedBrokerIds` antes de acceder a datos.
- [ ] Sin credenciales devuelve 401 sanitizado.
- [ ] Token o sesion invalida/caducada devuelve 401 sanitizado.
- [ ] Usuario autenticado sin broker valido devuelve 403 sanitizado.
- [ ] Usuario autenticado sin permiso requerido devuelve 403 sanitizado.
- [ ] Usuario autenticado con broker cruzado devuelve 403 sanitizado y no revela existencia de poliza.
- [ ] Errores publicos incluyen `correlationId` cuando aplique y no incluyen trazas internas.
- [ ] Logs de auth/autorizacion registran decision, politica, resultado y `correlationId` sin secretos ni datos personales innecesarios.
- [ ] La UI no consume metadata AppBuilder para permisos runtime.
- [ ] No se guardan secretos ni datos sensibles en Git.

## Impacto tecnico

Backend futuro:

- Middleware o esquema de autenticacion segun proveedor aprobado.
- Servicio interno de contexto de request: `currentUserId`, `currentBrokerId`, perfil, roles y permisos.
- Politicas de autorizacion por permiso.
- Integracion de `SESSION_CONTEXT` desde contexto autenticado.
- Errores publicos 401/403 con payload sanitizado y `correlationId`.

Frontend futuro:

- Cliente HTTP centralizado que soporte el mecanismo aprobado sin exponer secretos.
- Consumo de `/api/me` o equivalente para broker activo, permisos y estados de UI.
- Estados de permiso denegado y configuracion incompleta sin fallback silencioso a fixtures.

Documentacion:

- Roadmap Fase 5.
- SDD-2026-003 para dependencia con broker por request y SQL read-only.
- README solo cuando se implemente configuracion real.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL dinamico validado y parametrizado si aplica.
- [ ] Logs sin datos sensibles.
- [ ] Dependencias revisadas si se anaden paquetes.
- [ ] Tokens/cookies no se guardan en `localStorage` salvo decision explicita y justificada.
- [ ] Claims recibidos se normalizan y se validan antes de convertirse en permisos.
- [ ] CORS se mantiene restrictivo por entorno.
- [ ] 401/403 no revelan existencia de broker, usuario, poliza o permiso interno.

## Estrategia incremental de migracion

1. Documentar y mantener modo MVP actual.
   - API key y headers siguen disponibles solo para desarrollo/demo.
   - `Polizas:AllowHeaderExecutionContext` debe estar desactivado por defecto fuera de entornos controlados.
   - El override `Polizas:AllowHeaderExecutionContextOutsideDevelopment` no puede habilitar headers MVP ni dejar `/ready` verde fuera de Development sin el opt-in demo exacto `DEMO_ONLY_NOT_FOR_REAL_DATA`.

2. Introducir adaptador de identidad sin cambiar contratos de polizas.
   - El backend crea un contexto interno normalizado desde claims/sesion.
   - `/api/me` devuelve el mismo concepto funcional que hoy necesita la UI: broker actual, perfil y permisos.

3. Ejecutar modo dual temporal.
   - Claims/sesion tienen prioridad.
   - Headers MVP solo rellenan campos ausentes en entornos permitidos.
   - Se registran avisos sanitizados cuando se usa fallback MVP.

4. Validar autorizacion real por endpoint.
   - `polizas.catalogs`, `polizas.read` y `polizas.detail` se aplican en backend.
   - Broker solicitado o activo se valida contra `allowedBrokerIds`.
   - `SESSION_CONTEXT` se construye desde contexto autenticado.
   - API key MVP y `demo-session` se mantienen solo como compatibilidad temporal documentada mientras no exista proveedor auth real.

5. Cerrar compatibilidad insegura.
   - Desactivar headers MVP en preview/produccion.
   - Mantenerlos solo en perfiles locales explicitamente documentados o eliminarlos cuando ya no sean necesarios.
   - Actualizar README y runbooks con el mecanismo real aprobado.

Esta migracion no debe romper el contrato del frontend de polizas: los endpoints de negocio permanecen estables y el cambio de autenticacion se concentra en el cliente HTTP y el endpoint de contexto.

## Plan de pruebas

- Unitarias:
  - mapeo de claims/sesion a contexto interno;
  - permisos efectivos desde roles y overrides;
  - validacion de `currentBrokerId` contra `allowedBrokerIds`.
- Integracion:
  - usuario sin `polizas.catalogs` no puede consultar catalogos;
  - sin credenciales devuelve 401;
  - token invalido o caducado devuelve 401;
  - usuario autenticado sin broker valido devuelve 403;
  - usuario con broker no incluido en `allowedBrokerIds` devuelve 403 antes de leer datos;
  - usuario sin `polizas.read` devuelve 403;
  - usuario con `polizas.read` y broker valido puede listar;
  - usuario sin `polizas.detail` devuelve 403 en detalle;
  - usuario con `polizas.detail` no puede leer detalle de broker cruzado;
  - `SESSION_CONTEXT` usa valores autenticados y no headers manipulados.
- E2E/smoke:
  - UI muestra estado de acceso denegado sin fixtures silenciosos;
  - cambio de broker autorizado refresca contexto y listado;
  - broker no autorizado no filtra datos.
- Seguridad:
  - API key MVP no concede permisos si falta contexto autorizado;
  - `demo-session` solo emite permisos demo en entornos controlados;
  - no se loguean tokens, cookies, emails, connection strings ni SQL;
  - 401/403 incluyen `correlationId` y mensaje generico;
  - payloads de headers MVP manipulados no elevan permisos.
- Manual/UAT:
  - matriz de permisos por broker, perfil, oficina, gestor y usuario validada con responsables funcionales.

## Riesgos

- Proveedor auth no decidido: bloquea implementacion productiva.
- Mapa funcional de permisos incompleto: puede provocar sobreexposicion o falsos denegados.
- Permisos AppBuilder no equivalen automaticamente a permisos iLiniumTech: requieren interpretacion funcional.
- Modo dual puede perpetuar headers MVP si no se fija fecha de retirada.
- API key MVP y `demo-session` pueden confundirse con seguridad productiva si la evidencia no lo declara en cada corte.
- `SESSION_CONTEXT` real puede requerir claves adicionales no confirmadas.
- Diferencias entre brokers pueden requerir permisos por oficina, gestor o cartera ademas de broker.

## Work Items

- Crear issue GitHub: `SDD-2026-005 Auth y permisos de producto`.
- Crear decision: proveedor/mecanismo de identidad para MVP profesional.
- Crear issue tecnico: contrato `/api/me` definitivo con campos minimos y redaccion.
- Crear issue tecnico: servicio de contexto autenticado y politicas por permiso.
- Crear issue tecnico: tests 401/403 y broker cruzado.
- Crear issue funcional: matriz de permisos por broker, perfil, oficina, gestor y usuario.
- Crear issue tecnico: plan de retirada de headers MVP en preview/produccion.

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [ ] Documentacion actualizada.
