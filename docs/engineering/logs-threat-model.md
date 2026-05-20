# Threat model - Logs

Fecha: 2026-05-20

Estado: borrador operativo para bloquear cualquier API o lectura real de `Logs` hasta que producto, seguridad y operacion aprueben alcance, fuentes y controles.

## Decision

`Logs` no debe conectarse a datos reales, BBDD LOG, API, busqueda, detalle ni exportacion hasta que exista una SDD funcional aprobada y este threat model quede revisado por seguridad.

La ruta `/logs` puede seguir como superficie Vue estatica con fixture redactado. No autoriza leer logs reales ni payloads historicos.

## Alcance

Aplica a cualquier futura superficie de logs de iLiniumTech:

- logs de auditoria;
- logs de errores;
- logs de servicios e integraciones;
- logs de mail y SMS;
- logs de sistema;
- logs de migracion de datasources;
- observabilidad o bitacoras propias de iLiniumTech si se incorporan.

Aplica tanto si el origen futuro es BBDD local heredada, tablas `IAPLOG_*`, observabilidad nueva, ficheros, eventos o un servicio externo.

## Fuera de alcance

- Implementar API, SQL, UI, exportacion o detalle real.
- Consultar BBDD LOG real.
- Ejecutar extractores, queries heredadas o SQL exploratorio.
- Mostrar payloads, stack traces, request/response, cuerpos de mail/SMS o datos personales reales.
- Definir retencion legal definitiva.
- Sustituir una revision de seguridad formal.

## Activos a proteger

| Activo | Sensibilidad | Riesgo principal |
| --- | --- | --- |
| Secrets | Critica | Tokens, API keys, cookies, connection strings o credenciales en payloads o trazas. |
| PII | Critica | Emails, telefonos, documentos, nombres, direcciones, IPs, usuarios o datos de negocio. |
| Payloads | Critica | Request/response REST/SOAP, cuerpos de mail/SMS, mensajes de error o datos de polizas. |
| Infraestructura | Alta | URLs internas, rutas fisicas, servidores, bases, stacks, librerias y endpoints. |
| Actividad de usuario | Alta | Enumeracion de acciones, sesiones, brokers, perfiles y actividad temporal. |
| Datos multi-tenant | Alta | Fuga cruzada entre brokers, aplicaciones u oficinas. |
| Auditoria | Alta | Alteracion, borrado o lectura no auditada de logs de seguridad. |

## Actores

| Actor | Necesidad legitima | Riesgo |
| --- | --- | --- |
| Usuario normal | Ninguna en el primer corte. | Acceso indebido a actividad de otros usuarios o brokers. |
| Soporte interno | Diagnosticar incidencias. | Ver PII/secretos por exceso de permiso. |
| Administrador tecnico | Operacion y trazabilidad. | Exportacion masiva o lectura de payloads sensibles. |
| Seguridad | Investigacion y auditoria. | Necesita permisos fuertes y auditoria de acceso. |
| Desarrollador local | Pruebas en entorno local. | Versionar datos o capturas sensibles accidentalmente. |
| Atacante autenticado | Enumerar actividad o secretos. | Abuso de filtros, exportaciones o detalle. |
| Atacante anonimo | Acceso directo a endpoints. | 401/403 deben ser consistentes y no revelar existencia. |

## Fronteras de confianza

1. Navegador Vue: no es frontera de seguridad; solo muestra capacidades.
2. API .NET: unica autoridad para permisos, broker, minimizacion, redaccion y limites.
3. Proveedor de identidad/sesion: fuente de usuario, brokers permitidos y permisos.
4. Resolver de conexion: solo despues de validar usuario, permiso y broker.
5. BBDD LOG u origen de observabilidad: zona sensible, no consultable con queries libres.
6. Sistema de auditoria: debe registrar lecturas de detalle/exportacion sin guardar payloads.

## Datos prohibidos en primer corte

No deben devolverse, registrarse en evidencia ni renderizarse:

- connection strings, passwords, tokens, API keys, cookies, JWT, refresh tokens o secrets;
- headers completos, especialmente `Authorization`, `Cookie`, `Set-Cookie`, API keys y trazas de proxy;
- request/response completos, bodies REST/SOAP, XML, JSON o payloads de compania;
- cuerpos de email/SMS, destinatarios completos, telefonos completos o adjuntos;
- stack traces completos, rutas fisicas, nombres de servidor o detalles de infraestructura;
- documentos, IBAN, direcciones, emails, telefonos, nombres completos o datos personales;
- polizas, recibos, siniestros, propuestas, clientes o datos de negocio completos;
- URLs internas completas, query strings sensibles o endpoints privados;
- logs reales en capturas, fixtures, tests, documentacion o PRs.

## STRIDE

| Categoria | Amenaza | Control requerido |
| --- | --- | --- |
| Spoofing | Usar headers MVP, broker libre o API key demo para acceder a logs. | Auth productiva, permisos backend y broker desde claims/sesion. |
| Tampering | Manipular filtros/sort para inyectar SQL o consultar tablas no autorizadas. | Whitelists estrictas, SQL parametrizado y tipos de log cerrados. |
| Repudiation | Leer detalle sensible sin trazabilidad. | Auditoria de acceso a detalle/exportacion con usuario, broker, motivo y correlationId. |
| Information disclosure | Fuga de secretos, PII, payloads o datos multi-tenant. | Redaccion por tipo, minimizacion, permisos granulares y tests de no fuga. |
| Denial of service | Consultas amplias por fecha/texto o exportaciones masivas. | Rango temporal obligatorio, pageSize maximo, rate limit y limites por usuario. |
| Elevation of privilege | Ver payloads, mail, SMS o servicios con permiso general. | Permisos separados para lectura, detalle, payload, mail, SMS, servicios y exportacion. |

## Controles obligatorios antes de API

- SDD aprobada con owner funcional y owner de seguridad.
- Matriz de permisos por tipo de log y por accion.
- Rango de fechas obligatorio con maximo cerrado.
- Paginacion server-side con `pageSize` maximo.
- Tipos de log whitelisted; sin nombre de tabla en parametro publico.
- Sort whitelisted.
- Filtros de texto limitados, sanitizados y sin busqueda libre sobre payload completo.
- Redaccion centralizada en backend antes de serializar respuesta.
- Broker/aplicacion desde contexto autenticado, no desde query/header libre.
- Resolver conexion solo despues de auth, permisos y broker.
- Errores publicos sanitizados con `correlationId`.
- Logs de la propia API sin payloads ni secretos.
- Auditoria de accesos a detalle, payload y exportacion.
- Secret scan y revision manual de fixtures/evidencias.

## Permisos candidatos

| Permiso | Alcance |
| --- | --- |
| `logs.read` | Listado minimizado de logs autorizados. |
| `logs.audit.read` | Auditoria redactada. |
| `logs.errors.read` | Errores redactados sin stack completo. |
| `logs.services.read` | Servicios/integraciones sin payload completo. |
| `logs.mail.read` | Metadata de mail sin body ni destinatarios completos. |
| `logs.sms.read` | Metadata de SMS sin body ni telefono completo. |
| `logs.detail` | Detalle redactado. |
| `logs.payload.read` | Payload sensible, solo seguridad/soporte avanzado y con auditoria reforzada. |
| `logs.export` | Exportacion limitada, si se aprueba en SDD posterior. |

`logs.read` no implica `logs.detail`, `logs.payload.read` ni `logs.export`.

## Matriz de redaccion inicial

| Campo/origen | Listado | Detalle redactado | Payload sensible |
| --- | --- | --- | --- |
| `correlationId` | Visible | Visible | Visible |
| Fecha/hora | Visible | Visible | Visible |
| Tipo/nivel | Visible | Visible | Visible |
| Usuario | Alias o id opaco | Alias o id opaco | Segun permiso y auditoria |
| Broker | Etiqueta autorizada | Etiqueta autorizada | Etiqueta autorizada |
| Email/telefono | Oculto o enmascarado | Enmascarado | Solo permiso especifico |
| Headers | Ocultos | Lista segura | Redactados, nunca secretos |
| Body request/response | Oculto | Resumen redactado | Solo permiso especifico |
| Stack trace | Oculto | Mensaje sanitizado | Solo permiso especifico |
| URL interna | Categoria o host redaccionado | Redaccionada | Solo permiso especifico |
| SQL/connection string | Prohibido | Prohibido | Prohibido |

## Reglas de retencion y exportacion

- No hay exportacion en el primer corte.
- No hay descarga de payloads en el primer corte.
- Cualquier exportacion futura necesita SDD propia, permiso `logs.export`, auditoria y limite temporal.
- La UI no debe permitir rangos abiertos.
- El backend debe imponer maximos aunque el frontend falle.
- La documentacion y PRs no deben incluir capturas de logs reales.

## Reglas de implementacion futura

- No usar endpoint generico de tabla/datasource.
- No exponer nombres internos `IAPLOG_*` como contrato publico.
- No usar `SELECT *`.
- No interpolar columnas, tipos de log, tablas, sort ni filtros.
- No usar fixtures reales.
- No aceptar broker desde header libre como autoridad.
- No fallback silencioso a fixtures si `VITE_USE_BACKEND=true`.
- No cachear respuestas sensibles en navegador mas alla de la sesion.
- No registrar busquedas textuales sensibles en claro.

## Pruebas obligatorias si se desarrolla

Backend:

- 401 sin sesion;
- 403 sin `logs.read`;
- broker ausente o no permitido no resuelve conexion;
- rango de fechas obligatorio;
- pageSize maximo;
- tipo de log y sort fuera de whitelist rechazados;
- payloads maliciosos no alteran SQL;
- respuesta no contiene secretos, cookies, tokens, connection strings, stack traces ni payloads completos;
- detalle requiere `logs.detail`;
- payload sensible requiere `logs.payload.read`;
- exportacion requiere `logs.export` y queda auditada;
- errores publicos incluyen `correlationId` sin traza interna.

Frontend:

- ruta protegida sin sesion y sin permiso;
- filtros obligatorios y limpieza;
- listado paginado;
- detalle no disponible sin permiso;
- mensajes de error sanitizados;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings, tokens, emails reales ni documentos.

QA/seguridad:

- `Test-DocumentationBaseline.ps1`;
- `Invoke-SecretScan.ps1`;
- dependency audit si se anaden paquetes;
- CORS audit si toca API/configuracion;
- revision manual de payloads/DOM;
- smoke solo con fixtures redactados o entorno autorizado sin capturas sensibles.

## Preguntas abiertas

- Quien usara `Logs`: soporte, auditoria, seguridad, operacion o administracion?
- Que tipos de log son imprescindibles en el MVP?
- Cual es el rango maximo de consulta permitido?
- Que permisos reales existen por perfil y broker?
- Que campos deben redactarse siempre, incluso para soporte?
- Se permite detalle o solo listado minimizado?
- Se permite exportacion? Con que auditoria y retencion?
- Cual sera el origen autorizado: BBDD LOG heredada, observabilidad nueva o ambos?
- Deben separarse logs de iLiniumTech de logs AppBuilder historicos?
- Como se auditan las lecturas de logs sensibles?

## Estado de cierre

- Threat model creado como requisito previo.
- No se modifica runtime backend/frontend.
- No se consultan datos reales ni BBDD LOG.
- `/logs` sigue como fixture local read-only redactado.
- API, SQL, detalle, payloads, exportacion y logs reales siguen bloqueados hasta SDD y security review.
