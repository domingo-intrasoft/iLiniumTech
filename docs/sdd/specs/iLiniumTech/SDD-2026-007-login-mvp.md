# SDD: Login MVP y contexto inicial de aplicacion

## Metadata

- Spec ID: SDD-2026-007
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-15

## Contexto

El MVP actual entra directamente en `/polizas` y muestra la pantalla dentro de un `AppShell` con menu lateral estatico. El usuario quiere introducir una pantalla de login para que, una vez iniciada la sesion, se muestre la pantalla actual.

AppBuilder mezcla en el login conceptos que iLiniumTech debe separar: usuario, broker activo, perfil, aplicacion/version, conexiones y menus dinamicos. iLiniumTech no debe usar metadata AppBuilder para montar pantallas en runtime. El login debe abrir una sesion de producto y la UI debe seguir siendo Vue/TypeScript compilado.

Esta SDD complementa `SDD-2026-005 Auth y permisos de producto`. La autenticacion productiva completa sigue pendiente de decision de proveedor, pero se define un primer MVP revisable.

El incremento siguiente de permisos debe apoyarse en este login solo como `demo-session`: una sesion backend de desarrollo/demo que permite probar `/api/me`, broker autorizado y politicas `polizas.catalogs`, `polizas.read` y `polizas.detail`. No es auth productiva real.

## Objetivo

Crear un MVP de login que permita revisar el flujo:

1. El usuario entra en `/login`.
2. Introduce credenciales demo o activa una sesion local controlada.
3. La aplicacion crea un contexto de sesion MVP.
4. El usuario es redirigido a `/polizas`.
5. La pantalla actual de polizas se muestra dentro del menu lateral.
6. El usuario puede cerrar sesion y volver a `/login`.

El resultado debe ser una base incremental para sustituir despues la sesion demo por autenticacion backend real sin rehacer la UI de polizas.

## Fuera de alcance

- Implementar proveedor productivo de identidad.
- Integrar Microsoft Entra ID, Google, Auth0, IdentityServer u otro SSO.
- Reutilizar `/api/token/auth`, `LoginParameters`, `JwtData` o `whoAmI` de AppBuilder como contrato runtime.
- Cargar menus, permisos o pantallas desde metadata AppBuilder.
- Cambiar la logica funcional de busqueda/listado/detalle de polizas.
- Introducir escrituras o workflows.
- Guardar secretos, usuarios reales, passwords reales, connection strings reales o datos personales en Git.

## Contrato de datos

### Sesion frontend MVP

Para el primer incremento visual, la sesion puede ser local y efimera:

```json
{
  "mode": "demo",
  "user": {
    "id": "demo-user",
    "displayName": "Usuario demo"
  },
  "application": {
    "key": "iliniumtech",
    "name": "iLiniumTech"
  },
  "currentBrokerId": 42,
  "permissions": ["polizas.catalogs", "polizas.read", "polizas.detail"]
}
```

Reglas:

- Los valores son ejemplos sanitizados.
- La sesion local no concede autorizacion real sobre backend.
- En modo backend, `/api/me` seguira siendo la autoridad del contexto de polizas.
- No se deben almacenar tokens productivos en `localStorage`.

### Login backend MVP demo

El segundo incremento mantiene un contrato propio de iLiniumTech y no reutiliza el login AppBuilder. En modo demo backend se expone:

- `POST /api/auth/login`
- `POST /api/auth/logout`
- `GET /api/me`

`POST /api/auth/login` acepta credenciales demo solo para facilitar el MVP revisable. En Development queda disponible por defecto salvo `Auth:Demo:Enabled=false`; fuera de Development requiere `Auth:Demo:Enabled=true` y opt-in explicito `Auth:Demo:OptIn` con el valor demo aprobado por las politicas de contexto. Este contrato no sustituye la decision pendiente de proveedor productivo.

`GET /api/me` devuelve, como minimo:

- usuario visible minimo;
- aplicacion activa;
- broker activo;
- brokers permitidos;
- perfil efectivo si aplica;
- permisos efectivos;
- indicadores de contexto requerido para polizas.

La cookie de sesion demo es `HttpOnly`, no contiene metadata de pantalla y el backend prioriza claims/sesion para construir el contexto de polizas cuando existe autenticacion. El modo frontend `VITE_AUTH_MODE=demo-session` usa cookie y no exige `VITE_ILINIUMTECH_API_KEY`. Si la API key MVP sigue activa durante la transicion, debe tratarse como compatibilidad tecnica y no como identidad ni permiso funcional.

## Reglas de negocio

- La ruta raiz `/` debe llevar al flujo autenticado: `/polizas` si hay sesion, `/login` si no la hay.
- `/polizas`, `/polizas/:id` y rutas futuras de producto deben requerir sesion frontend MVP.
- `/login` no debe mostrar el menu lateral.
- Al cerrar sesion se limpian los datos de sesion local y se redirige a `/login`.
- El menu lateral permanece estatico en codigo fuente.
- La aplicacion activa para este MVP es iLiniumTech; no hay selector runtime de aplicacion salvo decision funcional posterior.
- Si hay multiples brokers en un futuro, el broker elegido debe validarse en backend antes de consultar datos.
- La UI no debe enviar permisos, perfil o bandera admin como autoridad.
- La API sigue autorizando en backend; el guard frontend solo mejora la experiencia.
- El backend debe ser la autoridad de `allowedBrokerIds` y de las politicas `polizas.catalogs`, `polizas.read` y `polizas.detail`.
- `demo-session` puede declarar permisos demo para pruebas controladas, pero esos permisos deben reemplazarse por claims/sesion validados cuando exista proveedor auth real.

## Criterios de aceptacion

- [ ] Al abrir `/login`, se muestra una pantalla de login sin menu lateral.
- [ ] Sin sesion, navegar a `/polizas` redirige a `/login`.
- [ ] Tras login demo correcto, se redirige a `/polizas`.
- [ ] La pantalla de polizas actual se ve igual que antes dentro de `AppShell`.
- [ ] `AppShell` muestra el usuario demo o etiqueta de sesion MVP.
- [ ] El boton salir limpia la sesion y redirige a `/login`.
- [ ] Refrescar la pagina conserva la sesion demo solo dentro de la misma politica acordada para MVP.
- [ ] El codigo no consulta metadata AppBuilder para construir pantallas, menu o permisos.
- [ ] No se guardan secretos ni datos reales en Git.

## Impacto tecnico

Frontend primer incremento:

- nueva feature `src/features/auth`;
- servicio/composable de sesion de login MVP;
- ruta `/login`;
- guard de router;
- conexion de logout desde `AppShell`;
- tests unitarios de servicio, guard y vista basica.

Backend segundo incremento:

- endpoint de login/logout demo controlado por configuracion;
- ampliacion de `/api/me` con usuario, aplicacion, brokers permitidos, permisos y modo de auth;
- contexto autenticado por claims para sustituir headers MVP cuando exista sesion;
- politicas por permisos de polizas.

Backend siguiente incremento de permisos:

- politicas explicitas `polizas.catalogs`, `polizas.read` y `polizas.detail`;
- validacion de `currentBrokerId` contra `allowedBrokerIds`;
- 401/403 sanitizados con `correlationId`;
- compatibilidad temporal con API key MVP y `demo-session`, sin presentarlas como auth productiva.

Documentacion:

- mantener este SDD;
- enlazar evidencia en `docs/appbuilder/login-auth-multitenant-application-analysis.md`;
- actualizar roadmap cuando se implemente.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] La sesion demo queda marcada como no productiva.
- [ ] No se almacenan tokens productivos en `localStorage`.
- [ ] No se aceptan permisos, perfil, broker ni `isAdmin` desde frontend como autoridad real.
- [ ] Backend sigue siendo autoridad de datos.
- [ ] 401/403 futuros deben ser sanitizados y con `correlationId`.
- [ ] CORS no se relaja por introducir login.
- [ ] No se loguean passwords, tokens, emails reales ni datos personales.

## Plan de pruebas

- Unitarias:
  - servicio de sesion demo crea, lee y limpia sesion;
  - guard redirige anonimos a `/login`;
  - guard permite rutas protegidas con sesion;
  - logout limpia sesion.
- Integracion frontend:
  - login demo navega a `/polizas`;
  - `/login` no renderiza `AppShell`;
  - `/polizas` mantiene menu lateral y contenido actual.
- E2E/smoke:
  - abrir `/login`;
  - completar login demo;
  - validar que aparece la pantalla de polizas;
  - cerrar sesion y comprobar redireccion.
- Pruebas esperadas para el incremento de permisos:
  - sin sesion ni API key valida devuelve 401 o redirige segun capa probada;
  - sesion demo sin `polizas.catalogs` recibe 403 en catalogos;
  - sesion demo sin `polizas.read` recibe 403 en listado;
  - sesion demo sin `polizas.detail` recibe 403 en detalle;
  - broker no incluido en `allowedBrokerIds` recibe 403 antes de leer datos;
  - los errores publicos incluyen `correlationId` cuando aplique y no revelan existencia de poliza o broker.
- Seguridad:
  - secret scan limpio;
  - no aparecen passwords reales ni connection strings;
  - no se introduce dependencia nueva sin auditoria.
- Manual/UAT:
  - producto valida textos, aspecto y flujo login -> polizas.

## Riesgos

- Proveedor auth productivo no decidido: el MVP local no debe venderse como seguridad final.
- Un guard frontend puede confundirse con autorizacion real: se documenta que backend sigue siendo autoridad.
- Si se mete seleccion de broker demasiado pronto, puede parecer validada aunque no lo este: aplazar hasta backend.
- Mantener API key y login demo a la vez puede ser confuso: mostrar claramente el modo en UI y documentacion.
- El proximo incremento puede dejar una falsa sensacion de seguridad si no se separa `demo-session` de auth productiva real.
- Reintroducir metadata AppBuilder seria una regresion arquitectonica.

## Work Items

- Crear issue: `SDD-2026-007 Login MVP frontend demo`.
- Crear issue posterior: `Auth backend MVP y /api/me ampliado`.
- Crear issue posterior: `Contexto autenticado para polizas y retirada de headers MVP`.
- Crear issue funcional: `Decision proveedor autenticacion productiva`.

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Tests frontend ejecutados: `npm run format`, `npm run lint`, `npm run test:unit`, `npm run build`.
- [ ] Smoke visual ejecutado y evidenciado si cambia UI.
- [ ] Secret scan limpio.
- [ ] Documentacion actualizada.
- [ ] Riesgos residuales escritos en PR o documento de cierre.
