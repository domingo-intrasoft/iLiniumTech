# Seguridad y configuracion

## Politica de secretos

Nunca versionar:

- connection strings reales;
- passwords;
- tokens;
- API keys;
- claves JWT;
- certificados privados;
- dumps, backups o logs con datos sensibles;
- `.env` con valores reales;
- capturas o fixtures con datos personales.

Los valores reales deben residir en:

- variables de entorno locales ignoradas por Git;
- GitHub Secrets o Azure DevOps secret variables;
- Azure Key Vault u otro vault equivalente;
- secretos de usuario de .NET para desarrollo local;
- configuracion protegida del servidor.

Si un secreto llega a Git, se considera comprometido. Hay que retirarlo, rotarlo y documentar la remediacion.

## Configuracion por entorno

Convenciones recomendadas:

- `ConnectionStrings__DefaultConnection` para una conexion principal.
- `ILINIUMTECH__MASTER_CONNECTION` para la conexion maestra local cuando se necesite leer AppBuilder.
- `ILINIUMTECH__PROGRAM_DATABASE` para el nombre de la BBDD de programa si procede.
- `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1` para origenes permitidos.
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience` si se implementa API propia con JWT.

Los nombres exactos podran cambiar cuando exista el primer servicio, pero el criterio no cambia: secretos fuera del repo y valores por entorno.

## Frontend

- Variables `VITE_*` solo para valores publicos.
- Nunca introducir secretos en bundles.
- No guardar tokens en `localStorage` si existe una alternativa con cookies `HttpOnly`, `Secure` y `SameSite`.
- No imprimir payloads de login, tokens, SQL, connection strings ni datos personales en consola.
- Validar entradas para UX, pero repetir validacion en backend.
- No introducir librerias UI o runtime sin justificacion tecnica.

## Backend

- Autenticacion por defecto; endpoints anonimos solo si estan documentados como publicos.
- Autorizacion por endpoint y por operacion, no solo por visibilidad en UI.
- DTOs para contratos publicos; no exponer entidades EF directamente.
- CORS restrictivo por configuracion.
- Validacion de DTOs antes de persistir o ejecutar consultas.
- Errores sin trazas internas, SQL, rutas fisicas o detalles de infraestructura.
- Logs estructurados con redaccion de secretos y datos personales.
- Passwords con hashing robusto; nunca cifrado reversible.

## Auth y permisos de producto

La SDD canonica para Fase 5 es [SDD-2026-005 Auth y permisos de producto](../sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md).

Reglas minimas hasta cerrar proveedor:

- `X-ILiniumTech-Api-Key` y los headers MVP de broker/usuario/perfil son bootstrap de desarrollo/demo, no identidad de produccion.
- En produccion, `currentUserId`, `currentBrokerId`, perfil, roles y permisos deben salir de claims o sesion backend validada.
- Si coexisten headers MVP y auth real, los claims/sesion tienen prioridad y cualquier broker solicitado se valida contra los brokers permitidos del usuario.
- 401 significa ausencia o invalidez de credenciales; 403 significa usuario autenticado sin broker, permiso o alcance suficiente.
- Los errores publicos de auth deben ser genericos, sanitizados y, cuando aplique, incluir `correlationId`.
- Los logs no deben incluir tokens, cookies, claims completos, connection strings, SQL ni datos personales innecesarios.

## BBDD y SQL dinamico

El modelo heredado de AppBuilder permite construir SQL desde metadata. En iLiniumTech:

- todos los valores van parametrizados;
- nombres de tabla, vista, columna y orden deben validarse contra whitelist extraida de metadata confiable;
- filtros libres se deben representar como AST o query object, no como strings arbitrarias;
- cada datasource debe declarar permisos de lectura/escritura;
- las cuentas de BBDD deben tener minimo privilegio;
- los comandos destructivos requieren pruebas y entorno controlado.

## GraphQL, PQL y APIs dinamicas

Si se implementa GraphQL:

- introspeccion desactivada en produccion salvo necesidad explicita;
- limite de profundidad y complejidad;
- persisted operations para operaciones conocidas;
- validacion fuerte de input;
- errores sanitizados;
- autorizacion por resolver o por servicio.

Si se implementa REST generico:

- no permitir operaciones genericas sin autorizacion;
- registrar operacion, usuario, datasource y resultado;
- no devolver mensajes SQL crudos al cliente.

## Cabeceras y superficie web

Revisar en entornos publicos:

- HTTPS obligatorio.
- `Content-Security-Policy` cuando sea viable.
- `X-Content-Type-Options: nosniff`.
- `Referrer-Policy`.
- cookies `HttpOnly`, `Secure`, `SameSite`.
- limites de tamano de request.
- rate limiting en endpoints anonimos o caros.

## Checklist por cambio

- [ ] No hay secretos nuevos.
- [ ] Configuracion sensible fuera de Git.
- [ ] CORS no usa wildcard.
- [ ] Endpoints nuevos tienen auth definida.
- [ ] Entradas externas validadas.
- [ ] SQL dinamico validado y parametrizado.
- [ ] Logs sin secretos ni datos personales innecesarios.
- [ ] Dependencias nuevas justificadas.
- [ ] Pruebas acordes al riesgo.
- [ ] Spec SDD actualizada.
