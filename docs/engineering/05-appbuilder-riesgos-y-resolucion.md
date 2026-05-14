# Riesgos AppBuilder y resolucion moderna

Este documento traduce los riesgos detectados durante el analisis de `C:\Desarrollo\AppBuilder` en decisiones para iLiniumTech. No afirma que todos sean vulnerabilidades explotables en produccion; son patrones que no se deben reproducir sin control.

## Secretos en codigo y herramientas

Riesgo:

- Credenciales y cadenas sensibles aparecian en scripts de scaffolding, herramientas de publicacion o appsettings auxiliares del proyecto fuente.

Resolucion:

- No copiar secretos.
- Gitleaks obligatorio.
- Variables de entorno o vault.
- Rotacion si un valor estuvo versionado.
- Plantillas con placeholders.

Pruebas:

- `Invoke-SecretScan.ps1`.
- revision de artefactos publicados.
- busqueda especifica de patrones `Password=`, `User Id=`, `Jwt`, `ApiKey`, `Token`.

## SQL dinamico

Riesgo:

- AppBuilder construye partes de SQL desde metadata. Aunque valores concretos puedan ir parametrizados, fragments estructurales pueden venir de configuracion.

Resolucion:

- AST/query object propio.
- Whitelist de entidades, campos y operaciones.
- Parametros obligatorios para valores.
- Validacion de ordenaciones, grupos y filtros.
- Cuentas de BBDD con minimo privilegio.

Pruebas:

- intentos de inyeccion en filtros;
- columnas inexistentes;
- ordenaciones maliciosas;
- payloads con `;`, comentarios SQL y subqueries;
- verificacion de SQL generado sin concatenar valores.

## CORS permisivo

Riesgo:

- Se observaron configuraciones tipo AllowAll o CORS amplio.

Resolucion:

- `Cors:AllowedOrigins` por entorno.
- Prohibir `AllowAnyOrigin` en APIs privadas.
- No hardcodear origenes en `Program.cs`.

Pruebas:

- `Invoke-CorsAudit.ps1 -FailOnFindings`.
- tests de integracion para origen permitido y no permitido.

## Cifrado reversible y fallback silencioso

Riesgo:

- Algunos helpers de cifrado devuelven el valor original si el desencriptado falla. Esto permite datos cifrados y planos mezclados.

Resolucion:

- Para secretos que deban desencriptarse: Data Protection, DPAPI o Key Vault.
- Para passwords: hashing robusto, no cifrado reversible.
- Fallar rapido si una clave o valor cifrado no puede validarse.
- Migraciones controladas para datos legacy.

Pruebas:

- arranque falla con secreto placeholder;
- desencriptado invalido no devuelve texto plano;
- logs no exponen el valor que fallo.

## Autenticacion y autorizacion dinamica

Riesgo:

- Auth configurable por toggles y multiples esquemas puede dejar rutas expuestas si se configura mal.

Resolucion:

- Auth obligatoria por defecto.
- Politicas explicitas.
- Endpoints anonimos con atributo y documentacion.
- Tests 401/403.
- No depender solo de permisos de UI.

Pruebas:

- usuario anonimo;
- usuario autenticado sin permiso;
- rol incorrecto;
- token caducado;
- ApiKey ausente o invalida.

## GraphQL y operaciones genericas

Riesgo:

- Una API generica puede ampliar mucho la superficie de ataque si no valida operacion, recurso y permisos.

Resolucion:

- Persisted operations para flujos conocidos.
- Limites de profundidad y complejidad.
- Autorizacion por resolver/operacion.
- DTOs y validadores.
- Errores sanitizados.

Pruebas:

- queries no permitidas;
- introspeccion en produccion;
- payload grande;
- campos no autorizados;
- errores sin stack trace.

## Render dinamico heredado

Riesgo:

- Metadata de UI puede controlar componentes, eventos, expresiones y datasources.

Resolucion:

- iLiniumTech no debe implementar un renderer generico de metadata AppBuilder.
- Metadata sanitizada solo para analisis, trazabilidad y scaffolding inicial.
- Componentes Vue estaticos y revisados como codigo fuente.
- Lista cerrada de componentes soportados por el producto.
- Sanitizacion de HTML si se permite contenido enriquecido.
- No ejecutar scripts arbitrarios en cliente.
- Separar metadata confiable de input de usuario.

Pruebas:

- scaffolding con componente desconocido;
- atributo malformado en artefactos de extraccion;
- HTML peligroso;
- evento no permitido;
- datasource no autorizado.

## Workflows y expresiones

Riesgo:

- Workflows pueden ejecutar procedimientos, scripts, servicios, mails, SMS y cambios de datos.

Resolucion:

- MVP sin workflows salvo necesidad.
- Allowlist de nodos.
- Timeouts.
- Auditoria por ejecucion.
- Sandbox para expresiones.
- Permisos por accion.

Pruebas:

- nodo no permitido;
- expresion con funcion no permitida;
- timeout;
- fallo parcial con rollback o estado claro;
- auditoria sin secretos.
