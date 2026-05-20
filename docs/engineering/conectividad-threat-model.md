# Threat model - Conectividad

Fecha: 2026-05-20

Estado: borrador operativo para bloquear cualquier API, conector real o llamada externa de `Conectividad` hasta que producto, seguridad y operacion aprueben alcance, fuentes y controles.

## Decision

`Conectividad` no debe ejecutar llamadas externas, pruebas REST/SOAP, lecturas reales, SQL, previews, importacion WSDL/OpenAPI ni gestion de secretos hasta que exista una SDD funcional aprobada y este threat model quede revisado por seguridad.

La ruta `/conectividad` puede seguir como superficie Vue estatica con fixture sanitizado. No autoriza conectores reales ni payloads historicos.

## Alcance

Aplica a cualquier futura superficie de conectividad de iLiniumTech:

- inventario de integraciones;
- salud de conectores;
- pruebas REST/SOAP;
- configuracion no secreta de endpoints;
- auditoria de pruebas;
- integraciones por broker, aplicacion o entorno;
- cualquier reutilizacion de conceptos AppBuilder de datasources de servicio.

Aplica tanto si el origen futuro es configuracion local, BBDD heredada, servicio de plataforma, secret store o un sistema de observabilidad.

## Fuera de alcance

- Implementar API, UI, SQL, conectores o llamadas externas.
- Consultar BBDD real de configuracion de integraciones.
- Ejecutar REST/SOAP, WSDL, Swagger/OpenAPI o previews.
- Mostrar endpoints internos, tokens, headers, request/response, bodies o datos personales reales.
- Editar secretos desde Vue.
- Sustituir una revision de seguridad formal.

## Activos a proteger

| Activo | Sensibilidad | Riesgo principal |
| --- | --- | --- |
| Secretos | Critica | Tokens bearer, API keys, certificados, passwords o connection strings. |
| Red interna | Critica | SSRF hacia hosts internos, metadata services o servicios no autorizados. |
| Payloads | Critica | Request/response REST/SOAP con PII o datos de negocio. |
| Endpoints | Alta | URLs internas, rutas privadas, WSDL, Swagger y nombres de sistemas. |
| Multi-tenant | Alta | Integraciones de otro broker, aplicacion u oficina. |
| Sistemas externos | Alta | Efectos colaterales por pruebas no idempotentes o reintentos. |
| Auditoria | Alta | Pruebas de conectividad sin trazabilidad ni motivo. |

## Actores

| Actor | Necesidad legitima | Riesgo |
| --- | --- | --- |
| Usuario normal | Ninguna en el primer corte. | Ejecutar pruebas o ver configuracion tecnica. |
| Soporte interno | Diagnosticar estado de integraciones. | Ver secretos, endpoints o payloads. |
| Administrador tecnico | Operacion controlada. | Configurar destinos inseguros o exportar datos. |
| Seguridad | Revisar riesgos de egress y secretos. | Necesita auditoria fuerte y datos minimizados. |
| Desarrollador local | Pruebas con fixtures. | Versionar endpoints o tokens accidentalmente. |
| Atacante autenticado | SSRF, exfiltracion y enumeracion. | Abuso de URL libre, headers o previews. |
| Atacante anonimo | Acceso directo a endpoints. | 401/403 consistentes sin revelar integraciones. |

## Fronteras de confianza

1. Navegador Vue: no maneja secretos ni ejecuta llamadas externas.
2. API .NET: unica autoridad para permisos, broker, allowlists, redaccion y auditoria.
3. Secret store/configuracion local ignorada por Git: unica fuente de secretos.
4. Resolver de integracion: solo despues de validar usuario, permiso, broker y entorno.
5. Capa de egress: aplica allowlists, timeouts, DNS seguro y bloqueo de redes internas.
6. Sistemas externos: nunca se invocan desde URL libre ni sin auditoria.

## Datos prohibidos en primer corte

No deben devolverse, registrarse en evidencia ni renderizarse:

- tokens, API keys, passwords, certificados, cookies, JWT o refresh tokens;
- connection strings, cadenas de broker, usuarios tecnicos o secretos de entorno;
- headers completos, especialmente `Authorization`, `Cookie`, `Set-Cookie` y claves propietarias;
- request/response completos, XML, JSON, SOAP envelopes, WSDL reales o Swagger privados;
- URLs internas completas, hosts privados, IPs internas, rutas administrativas o query strings sensibles;
- bodies con PII, documentos, polizas, recibos, siniestros, clientes o datos financieros;
- errores con stack traces, certificados, DNS, proxy, servidor o infraestructura;
- capturas, fixtures o PRs con endpoints o payloads reales.

## STRIDE

| Categoria | Amenaza | Control requerido |
| --- | --- | --- |
| Spoofing | Usar headers MVP, broker libre o API key demo para probar integraciones. | Auth productiva, permisos backend y broker desde claims/sesion. |
| Tampering | Manipular URL, metodo, headers o body para alcanzar destinos no previstos. | Sin URL libre, allowlists, metodos cerrados y plantillas revisadas. |
| Repudiation | Ejecutar pruebas sin trazabilidad. | Auditoria de cada prueba con usuario, broker, integracion, motivo y correlationId. |
| Information disclosure | Fuga de secretos, endpoints, payloads o PII. | Redaccion backend, DTOs minimizados y secret store fuera de frontend. |
| Denial of service | Timeouts largos, reintentos o llamadas masivas a terceros. | Timeouts cortos, cancelacion, rate limits, circuit breaker y no retry peligroso. |
| Elevation of privilege | Usuario con lectura ejecuta pruebas o ve secretos. | Permisos separados `read`, `test`, `manage`, `secrets.manage` y auditoria reforzada. |

## Controles obligatorios antes de API

- SDD aprobada con owner funcional, owner de seguridad y owner de operacion.
- Caso de uso limitado: salud/inventario, no editor generico AppBuilder.
- Matriz de permisos por accion y entorno.
- Feature flag por entorno para bloquear pruebas reales por defecto.
- Secretos solo en secret store, variables de entorno o configuracion local ignorada por Git.
- No enviar secretos al frontend.
- Destinos allowlist por integracion; sin URL libre.
- Bloquear IPs privadas, loopback, link-local, metadata services y rangos reservados salvo excepcion de plataforma aprobada.
- DNS resuelto y validado contra allowlist antes de conectar.
- Metodos HTTP whitelisted; sin metodo arbitrario.
- Headers whitelisted y redaccionados.
- Body generado desde plantilla revisada; sin editor libre de payloads en primer corte.
- Timeouts cortos, cancelacion, limite de tamano de respuesta y rate limiting.
- No retry automatico salvo operaciones declaradas idempotentes.
- Auditoria de pruebas con resultado sanitizado.
- Errores publicos sanitizados con `correlationId`.
- Logs de la propia API sin request/response completos ni secretos.

## Permisos candidatos

| Permiso | Alcance |
| --- | --- |
| `conectividad.read` | Ver inventario sanitizado y estado no sensible. |
| `conectividad.audit.read` | Ver auditoria sanitizada de pruebas. |
| `conectividad.test` | Ejecutar pruebas controladas en destinos allowlist. |
| `conectividad.manage` | Administrar metadata no secreta aprobada. |
| `conectividad.secrets.manage` | Gestion de secretos fuera de frontend, preferiblemente delegada a plataforma. |

`conectividad.read` no implica `conectividad.test`, `conectividad.manage` ni `conectividad.secrets.manage`.

## Matriz de redaccion inicial

| Campo/origen | Listado | Detalle sanitizado | Prueba real |
| --- | --- | --- | --- |
| Nombre integracion | Visible si no revela secreto | Visible | Visible |
| Entorno | Visible | Visible | Visible |
| Tipo REST/SOAP | Visible | Visible | Visible |
| Endpoint | Alias o host redaccionado | Host redaccionado | Solo backend |
| Secretos | Prohibido | Prohibido | Solo secret store/backend |
| Headers | Ocultos | Lista segura | Redactados |
| Body request | Oculto | Plantilla abstracta | No visible en frontend |
| Response | Estado/codigo | Resumen sanitizado | No visible completo |
| Error tecnico | Categoria | Mensaje sanitizado | Logs internos redaccionados |
| CorrelationId | Visible | Visible | Visible |

## Reglas de implementacion futura

- No migrar `DataSourceComp.vue`, `NewDataSourceWS.vue`, `RepositorioService.cs` ni mutaciones GraphQL de AppBuilder como producto directo.
- No crear un CRUD generico de `IapDataSource*`.
- No ejecutar REST/SOAP desde metadata heredada en runtime.
- No permitir URL libre, headers libres ni body libre en la UI.
- No mostrar ni editar connection strings o tokens en Vue.
- No probar endpoints desde el navegador.
- No guardar payloads reales en fixtures, tests, documentacion o PRs.
- No aceptar broker desde header libre como autoridad.
- No fallback silencioso a fixtures si `VITE_USE_BACKEND=true`.
- No loguear request/response completos.

## SSRF y egress

Controles especificos:

- allowlist de hosts por integracion y entorno;
- validacion de esquema `https` salvo excepcion aprobada;
- bloqueo de `localhost`, `127.0.0.0/8`, `::1`, link-local, metadata services, redes privadas y rangos reservados;
- DNS pinning o revalidacion contra allowlist tras resolver;
- no seguir redirects fuera de allowlist;
- limite de redirects bajo o redirects deshabilitados;
- timeout de conexion y lectura;
- limite maximo de respuesta;
- sin proxy configurable por usuario;
- auditoria de destino real resuelto, redaccionada en evidencias.

## Pruebas obligatorias si se desarrolla

Backend:

- 401 sin sesion;
- 403 sin permiso;
- broker ausente o no permitido no resuelve integracion;
- `conectividad.read` no permite test;
- URL libre rechazada;
- host no allowlist rechazado;
- IP privada/loopback/link-local/metadatos rechazada;
- redirect fuera de allowlist rechazado;
- headers no permitidos rechazados;
- secretos no aparecen en DTO, errores ni logs;
- timeout/cancelacion cubiertos;
- prueba real audita resultado sanitizado;
- errores publicos incluyen `correlationId` sin infraestructura sensible.

Frontend:

- ruta protegida sin sesion y sin permiso;
- acciones de probar/detalle/exportar deshabilitadas sin permiso;
- no se muestra URL interna, token, connection string, header sensible ni payload;
- dialogo de prueba no acepta URL libre;
- error backend sanitizado con `correlationId`;
- DOM sin `AppBuilder`, `IAP_`, `QueryStatic`, SQL, connection strings, tokens, emails reales ni documentos.

QA/seguridad:

- `Test-DocumentationBaseline.ps1`;
- `Invoke-SecretScan.ps1`;
- dependency audit si se anaden paquetes;
- CORS audit si toca API/configuracion;
- pruebas de SSRF con destinos bloqueados usando fixtures;
- smoke solo con fixtures o entorno autorizado sin capturas sensibles.

## Preguntas abiertas

- `Conectividad` sera salud de integraciones, administracion tecnica, soporte o modulo nuevo?
- Que integraciones se pueden listar sin exponer infraestructura?
- Que entornos permiten pruebas reales?
- Quien puede ejecutar pruebas y con que motivo?
- Donde viviran los secretos y quien los administra?
- Que hosts/metodos quedan en allowlist?
- Se permite ver respuesta sanitaria o solo estado/codigo?
- Como se auditan pruebas y errores?
- Que limites de frecuencia y timeout son aceptables?
- Debe separarse conectividad iLiniumTech de datasources AppBuilder historicos?

## Estado de cierre

- Threat model creado como requisito previo.
- No se modifica runtime backend/frontend.
- No se ejecutan llamadas externas, conectores ni SQL.
- `/conectividad` sigue como fixture local read-only sanitizado.
- API, conectores reales, payloads, secretos, pruebas REST/SOAP y llamadas externas siguen bloqueados hasta SDD y security review.
