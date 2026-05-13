# SDD: <titulo>

## Metadata

- Spec ID: <SDD-YYYY-NNN>
- Work Item: <GitHub issue o Azure Boards ID>
- Aplicacion: iLiniumTech
- Tipo: <feature|bug|security|infra|docs>
- Tamano SDD: <S|M|L>
- Estado SDD: <draft|spec-ready|needs-clarification|deprecated>
- Responsable funcional: <rol/persona>
- Responsable tecnico: <rol/persona>
- Fecha: <YYYY-MM-DD>

## Contexto

Describe el problema, la relacion con AppBuilder y el motivo de negocio.

## Objetivo

Resultado observable que debe existir al cerrar la spec.

## Fuera de alcance

Lista explicita de lo que no se implementara en esta spec.

## Contrato de datos

Entradas, salidas, campos obligatorios, ejemplos sanitizados y reglas de redaccion.

## Reglas de negocio

- <regla verificable>

## Criterios de aceptacion

- [ ] Dado <contexto>, cuando <accion>, entonces <resultado>.
- [ ] No se guardan secretos ni datos sensibles en Git.

## Impacto tecnico

Rutas, modulos, tablas conceptuales, APIs, dependencias y decisiones relevantes.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Autenticacion/autorizacion definida.
- [ ] Entradas externas validadas.
- [ ] SQL dinamico validado y parametrizado si aplica.
- [ ] Logs sin datos sensibles.
- [ ] Dependencias revisadas si se anaden paquetes.

## Plan de pruebas

- Unitarias:
- Integracion:
- E2E/smoke:
- Seguridad:
- Manual/UAT:

## Riesgos

- <riesgo y mitigacion>

## Work Items

- <issue o tarea relacionada>

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [ ] Documentacion actualizada.
