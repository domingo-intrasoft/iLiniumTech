# SDD: Repositorio SQL whitelist Pantalla_Polizas

## Metadata

- Spec ID: SDD-2026-003
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-13

## Contexto

El MVP actual usa `InMemoryPolizasRepository` con datos anonimizados. La siguiente evolucion debe leer `Pantalla_Polizas` de forma controlada, manteniendo paginacion, filtros y ordenaciones por whitelist.

`Pantalla_Polizas` puede depender de `SESSION_CONTEXT`, perfil, oficinas, gestores y permisos, por lo que la lectura real debe tratarse como superficie sensible.

Decision de arquitectura aplicable: el repositorio SQL forma parte del backend API de datos de iLiniumTech. No es un runtime generico de datasources AppBuilder ni debe interpretar metadata `IAP_*` en produccion.

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
- contexto de usuario/perfil cuando se defina.

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

- [ ] Existe implementacion SQL detras de `IPolizasRepository`.
- [ ] La activacion del repositorio SQL se hace por configuracion, no por cambio de codigo.
- [ ] La whitelist usada por el repositorio esta definida en codigo/configuracion iLiniumTech revisada.
- [ ] No hay lectura runtime de `IAP_*` para construir la consulta productiva.
- [ ] Sort malicioso o desconocido devuelve error de validacion.
- [ ] Filtros maliciosos no alteran SQL estructural.
- [ ] El endpoint mantiene API key obligatoria.
- [ ] Los errores no devuelven SQL, connection strings ni trazas internas.
- [ ] Hay pruebas unitarias del query object y whitelist.
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

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] SQL parametrizado.
- [ ] Whitelist para estructura SQL.
- [ ] `QueryStatic` y fragments heredados se tratan como evidencia, no como ejecutable.
- [ ] Auth y autorizacion preservadas.
- [ ] Logs con redaccion de datos sensibles.
- [ ] Pruebas de inyeccion para filtros y ordenacion.

## Plan de pruebas

- Unitarias: construccion de query, validacion de columnas, limites de pagina.
- Integracion: endpoint protegido y lectura SQL con datos anonimizados.
- E2E/smoke: `/polizas` muestra datos o estado vacio controlado.
- Seguridad: payloads con `;`, comentarios SQL, subqueries y columnas inexistentes.
- Manual/UAT: contrastar resultados con entorno de pruebas usando usuario autorizado.

## Riesgos

- Las restricciones reales pueden depender de contexto de sesion no presente en tests.
- El modelo puede devolver campos con datos personales que deben recortarse.
- La query original de AppBuilder puede contener logica no portable al producto iLiniumTech.
- Si se intenta reutilizar metadata como runtime, se reintroducen riesgos de AppBuilder y se rompe la decision de arquitectura.

## Work Items

- Crear issue GitHub: `SDD-2026-003 Repositorio SQL whitelist Pantalla_Polizas`.
- Crear issue tecnico: pruebas de inyeccion de filtros y sort.
- Crear issue tecnico: definir contexto de usuario/perfil para lectura.
- Crear issue tecnico: documentar whitelist backend derivada de metadata revisada.

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [ ] Documentacion actualizada.
