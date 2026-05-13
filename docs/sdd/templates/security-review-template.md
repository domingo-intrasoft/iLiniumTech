# Revision de seguridad: <titulo corto>

## Metadata

- Spec: <ruta a spec>
- Work Item: <GitHub issue o Azure Boards ID>
- Aplicacion: iLiniumTech
- Fecha: <YYYY-MM-DD>
- Revisor: <nombre o rol>

## Superficie afectada

- [ ] Autenticacion.
- [ ] Autorizacion/permisos.
- [ ] Secretos/configuracion.
- [ ] Datos personales o sensibles.
- [ ] BBDD o SQL dinamico.
- [ ] APIs publicas.
- [ ] GraphQL/PQL.
- [ ] CORS/cabeceras/cookies/tokens.
- [ ] Pipelines, agentes o despliegues.
- [ ] Dependencias.
- [ ] Workflows o expresiones.

## Checks

Secretos:

- [ ] No hay claves, tokens, passwords ni cadenas de conexion en codigo versionado.
- [ ] La configuracion sensible se inyecta por variables o secret stores autorizados.
- [ ] Logs, specs y comentarios no contienen valores sensibles.

Autenticacion y autorizacion:

- [ ] Endpoints protegidos mantienen auth.
- [ ] No se amplia acceso de roles sin aprobacion explicita.
- [ ] Los errores no filtran informacion sensible.

Datos y SQL:

- [ ] Solo se exponen campos necesarios.
- [ ] Se valida entrada de usuario.
- [ ] SQL dinamico usa whitelist y parametros.
- [ ] Se evita registrar datos personales innecesarios.

Frontend:

- [ ] No se persisten tokens o datos sensibles en lugares no autorizados.
- [ ] No se introducen dependencias de terceros sin revisar.
- [ ] No se exponen endpoints internos.

Pipelines/despliegue:

- [ ] No se imprimen secretos en logs.
- [ ] No se relajan gates de calidad o seguridad sin decision documentada.
- [ ] Artefactos publicados no contienen configuracion sensible.

## Resultado

- Decision: <aprobado|aprobado con observaciones|bloqueado>
- Observaciones:
- Acciones pendientes:
