# SDD: Autos Particulares MVP read-only

## Metadata

- Spec ID: SDD-2026-006
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-14

## Contexto

iLiniumTech debe ampliar el horizonte del MVP desde Polizas hacia un vertical explicito de producto: `Autos Particulares`.

Esta fase no convierte iLiniumTech en un runtime dinamico tipo AppBuilder. La metadata heredada, si se usa, solo puede servir para analisis, trazabilidad, extraccion offline, comparativa y scaffolding revisado. La pantalla final debe ser Vue/TypeScript mantenido como codigo fuente y el backend debe exponer una API explicita propia para el vertical.

La implementacion puede reutilizar componentes, estilos, servicios HTTP, validadores, patrones de filtros, paginacion, errores y repositorios de Polizas cuando sea razonable. Esa reutilizacion debe ser reutilizacion de codigo iLiniumTech revisado, no interpretacion runtime de metadata `IAP_*`, `QueryStatic` ni configuracion AppBuilder.

Dependencias documentales:

- `SDD-2026-001`: MVP Polizas read-only como vertical inicial.
- `SDD-2026-003`: repositorio SQL de Polizas, whitelists, broker por request y `SESSION_CONTEXT`.
- `SDD-2026-005`: autenticacion, autorizacion y permisos de producto.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`: iLiniumTech no es AppBuilder runtime.

## Decision de producto

Se crea `Autos Particulares` como vertical explicito del producto.

Decisiones:

- La ruta frontend objetivo es `/autos-particulares`.
- La API objetivo usa endpoints propios bajo `/api/autos-particulares`.
- El primer MVP es read-only.
- El alcance funcional se limita al ramo `Autos`.
- La division objetivo es `Particulares` cuando el dato exista y pueda filtrarse o confirmarse de forma fiable.
- Si la BBDD real no permite confirmar la division `Particulares` en el listado, se permite un fallback controlado: listar solo ramo `Autos`, marcar internamente que el filtro de division no esta confirmado, no presentar el resultado como validado para UAT y dejar comparativa funcional pendiente con DBA/producto.
- El fallback no autoriza a mezclar otros ramos ni a ocultar la incertidumbre en la evidencia de cierre.
- No se introduce endpoint generico de pantallas, formularios o datasources.

## Objetivo

Definir el contrato de producto para implementar un listado y detalle read-only de Autos Particulares con:

- ruta Vue estatica `/autos-particulares`;
- API backend explicita bajo `/api/autos-particulares`;
- filtros, paginacion y ordenacion por whitelist;
- tratamiento seguro de datos personales y datos de vehiculo;
- permisos y contexto alineados con la estrategia de autenticacion de producto;
- UAT pendiente para confirmar equivalencia contra entorno real autorizado.

## Fuera de alcance

- Escritura, alta, modificacion o baja de polizas.
- Emision, reemplazo, anulacion, suspension, revigorizacion o duplicado.
- Cotizacion, tarificacion o comparativa de precios.
- Gestion de siniestros.
- Documentos, recibos, suplementos, certificados o descarga de ficheros.
- Workflows, expresiones, acciones REST/SOAP heredadas o motor generico AppBuilder.
- Ejecucion directa de `QueryStatic` o fragments SQL heredados.
- Render dinamico desde metadata `IAP_*` en runtime.
- Exponer documento legal completo, matricula completa, bastidor completo, telefono, email o direccion en listado por defecto.
- Resolver definitivamente autenticacion real; esta dependencia queda en `SDD-2026-005`.

## Contrato API

### Endpoints

Base path objetivo:

- `/api/autos-particulares`

Endpoints objetivo del primer MVP:

- `GET /api/autos-particulares/polizas`
- `GET /api/autos-particulares/polizas/{id}`

Endpoint de catalogos propio del vertical:

- `GET /api/autos-particulares/catalogs`

Reglas:

- Los endpoints son propios del vertical, aunque internamente puedan reutilizar servicios o repositorios de Polizas.
- No se publica metadata AppBuilder como contrato runtime.
- No se devuelve SQL, connection strings, nombres internos de tablas, trazas ni errores de proveedor.
- Los errores publicos deben estar sanitizados y, cuando aplique, incluir `correlationId`.

### Listado

Entrada inicial:

- `page`
- `pageSize`
- `numero`
- `estado`
- `compania`
- `fechaEfectoDesde`
- `fechaEfectoHasta`
- `sort`

Filtros de alcance:

- `ramo=Autos` debe aplicarse por backend mediante contrato/whitelist, no como texto libre enviado desde UI.
- `division=Particulares` debe aplicarse cuando exista campo o regla fiable validada con BBDD real.
- Si no existe confirmacion fiable de division en listado, el backend debe usar el fallback controlado descrito en esta SDD y dejarlo visible en evidencia tecnica/UAT.

Salida conceptual:

- `items`
- `page`
- `pageSize`
- `total`
- `scope`

`scope` debe permitir distinguir:

- `ramo`: `Autos`
- `divisionObjetivo`: `Particulares`
- `divisionFiltroAplicado`: `true` o `false`
- `divisionPendienteUat`: `true` cuando el listado no pueda confirmar division contra BBDD real

Campos iniciales por item:

- `id`
- `numero`
- `aplicacion`
- `estado`
- `ramo`
- `division`
- `clienteNombre`
- `compania`
- `fechaEfecto`
- `fechaVencimiento`
- `primaAnual`
- `moneda`
- `vehiculoResumen`

`vehiculoResumen` debe ser no sensible y apto para listado. No debe incluir matricula completa, bastidor completo ni datos de localizacion salvo SDD futura y permiso explicito.

### Detalle

`GET /api/autos-particulares/polizas/{id}` devuelve un detalle read-only del mismo alcance:

- datos de poliza necesarios para identificar el contrato;
- estado, compania, ramo y division cuando exista;
- datos de cliente minimizados;
- datos de vehiculo minimizados;
- fechas y prima necesarias para UAT del MVP;
- `scope` o equivalente para indicar si la division fue confirmada o sigue pendiente.

El detalle no debe ampliar PII respecto al listado sin decision funcional, permiso y pruebas especificas.

### Catalogos

`GET /api/autos-particulares/catalogs` devuelve solo catalogos necesarios para filtros del vertical:

- `estado`
- `compania`
- `scope`
- otros catalogos estrictamente necesarios para UI read-only, siempre revisados y minimizados

El catalogo de ramo no debe abrir el endpoint a otros ramos. Si se muestra algun selector futuro, debe estar limitado por contrato de producto.

## Contrato UI

Ruta objetivo:

- `/autos-particulares`

Pantallas:

- listado read-only de Autos Particulares;
- detalle read-only de un registro seleccionado.

Reglas UI:

- Vue 3 + TypeScript como codigo fuente mantenido.
- Puede reutilizar componentes de `/polizas` si quedan como componentes compartidos revisados.
- No consume metadata AppBuilder, JSON del extractor ni `IAP_*` en runtime.
- Debe cubrir estados `loading`, `empty`, `error`, configuracion incompleta y acceso denegado.
- Si el backend informa `divisionPendienteUat=true`, la evidencia de cierre debe registrarlo como UAT pendiente; la UI no debe presentar la division como validada funcionalmente.
- El copy visible debe hablar de `Autos Particulares` como vertical, sin convertir la pantalla en un explorador generico de polizas.

## Reglas de negocio

- Solo lectura.
- El backend fija el alcance a ramo `Autos`.
- La division `Particulares` se filtra o confirma solo cuando exista dato fiable.
- Si falta dato fiable de division, el fallback es temporal, documentado y bloquea cierre funcional completo.
- La paginacion y el sort tienen limites y whitelist.
- La UI no decide autorizacion ni alcance de datos.
- El backend debe validar broker, permisos y contexto antes de leer datos reales.
- No se mezclan datos entre brokers ni entre usuarios.
- No se usan headers MVP como identidad confiable fuera de entornos controlados.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] API key MVP o autenticacion real requerida segun fase activa.
- [ ] Permisos objetivo definidos: `autosParticulares.catalogs`, `autosParticulares.read`, `autosParticulares.detail`.
- [ ] Broker efectivo validado contra claims/sesion real cuando exista auth.
- [ ] Headers MVP tratados como bootstrap temporal y no como permisos.
- [ ] Errores 401/403 sanitizados con `correlationId` cuando aplique.
- [ ] SQL parametrizado.
- [ ] Estructura SQL, columnas, filtros y sort protegidos por whitelist.
- [ ] `SESSION_CONTEXT` parametrizado si la BBDD real lo requiere.
- [ ] Logs sin documento legal, matricula completa, bastidor, telefono, email, direccion, connection strings, SQL ni tokens.
- [ ] CORS mantiene politica restrictiva por entorno.

## Datos y PII

Clasificacion inicial:

- Documento legal del cliente: dato personal sensible para el MVP; no exponer completo.
- Nombre de cliente: dato personal; minimizar en listado y no usar en logs.
- Matricula: dato personal o identificador sensible; no exponer completa en listado por defecto.
- Bastidor/VIN: identificador sensible; no exponer completo por defecto.
- Telefono, email y direccion: fuera de alcance.
- Prima, fechas y compania: datos de negocio; exponer solo lo necesario para read-only.

Fixtures y pruebas:

- Los datos de prueba deben ser anonimizados o sinteticos.
- No versionar dumps, capturas sensibles ni resultados reales.
- Cualquier evidencia de UAT con datos reales debe quedar fuera de Git o sanitizada.

Fallback de division:

- Si la BBDD real no permite confirmar division `Particulares` en listado, los resultados deben clasificarse como `ramo Autos con division pendiente de confirmar`.
- Esa situacion es aceptable para avance tecnico del MVP, pero no para cierre UAT completo.
- El responsable funcional y DBA deben confirmar campo, vista, regla o combinacion autorizada para distinguir `Particulares`.

## Plan de pruebas

Backend:

- Unitarias de validacion de request, limites de pagina y sort.
- Unitarias de scope: ramo `Autos` fijo y division `Particulares` cuando exista regla.
- Unitarias de fallback: `divisionFiltroAplicado=false` y `divisionPendienteUat=true` cuando no se pueda confirmar division.
- Pruebas de whitelist para filtros, columnas y ordenaciones.
- Pruebas de errores 401/403/400 sanitizados.
- Integracion con repositorio in-memory o fixture controlado.
- Integracion pendiente contra BBDD real/test autorizada para validar ramo, division, broker y `SESSION_CONTEXT`.

Frontend:

- Unitarias de servicio y mapeo del contrato `/api/autos-particulares/polizas`.
- Unitarias de estados `loading`, `empty`, `error`, configuracion incompleta y acceso denegado.
- Unitarias de filtros y navegacion a detalle.
- Smoke visual de `/autos-particulares` y detalle cuando exista UI.

Seguridad/calidad:

- Secret scan.
- Dependency audit si se anaden dependencias.
- CORS audit si toca API/configuracion.
- `git diff --check`.
- Validacion documental.

Manual/UAT:

- Comparar conteos y muestras contra entorno autorizado.
- Confirmar que todos los registros del listado son ramo `Autos`.
- Confirmar si la division `Particulares` se filtra por dato fiable.
- Si no se confirma division en listado, registrar UAT pendiente y regla necesaria.
- Validar que no se muestran identificadores personales o de vehiculo completos sin permiso.

## Criterios de aceptacion

- [ ] Existe ruta frontend `/autos-particulares`.
- [ ] La UI es Vue/TypeScript propio o componentes compartidos revisados; no renderiza desde metadata.
- [ ] Existe `GET /api/autos-particulares/polizas`.
- [ ] Existe `GET /api/autos-particulares/polizas/{id}`.
- [ ] Existe `GET /api/autos-particulares/catalogs` con catalogos minimizados para el vertical.
- [ ] El backend fija el alcance a ramo `Autos`.
- [ ] La division `Particulares` se aplica cuando exista dato fiable.
- [ ] Si la division no puede confirmarse en listado, el fallback queda expuesto en `scope` o evidencia equivalente y UAT queda pendiente.
- [ ] Los endpoints estan protegidos por el mecanismo vigente.
- [ ] Filtros, sort y columnas usan whitelist y parametros.
- [ ] No hay dependency runtime de metadata AppBuilder.
- [ ] No se exponen secretos, SQL, connection strings ni trazas internas.
- [ ] No se exponen documento, matricula ni bastidor completos por defecto.
- [ ] Pruebas backend/frontend aplicables ejecutadas.
- [ ] UAT o bloqueo externo documentado.

## Impacto tecnico previsto

Backend futuro:

- Nuevo contrato de aplicacion para `AutosParticulares`.
- Endpoints propios bajo `/api/autos-particulares`.
- Reutilizacion controlada de modelos, paginacion, errores, seguridad y acceso SQL de Polizas cuando encaje.
- Whitelist propia del vertical aunque comparta columnas con Polizas.
- Permisos futuros propios: `autosParticulares.catalogs`, `autosParticulares.read`, `autosParticulares.detail`.

Frontend futuro:

- Ruta `/autos-particulares`.
- Feature propia o componentes compartidos extraidos desde Polizas de forma revisada.
- Servicio HTTP propio para el vertical.
- Estados de error/configuracion/permisos alineados con Polizas.

Documentacion:

- Roadmap con nueva fase/vertical.
- Paquete de tareas paralelas con backend, frontend y QA/UAT para Autos Particulares.
- Evidencia de fallback/UAT pendiente si la division no se puede confirmar.

## Riesgos

- La BBDD real puede no exponer division `Particulares` de forma directa en listado.
- El uso de ramo `Autos` sin division confirmada puede sobreincluir datos y requiere UAT antes de presentarlo como vertical cerrado.
- Matricula, bastidor y documento legal pueden aparecer en fuentes reales y deben recortarse o enmascararse.
- Reutilizar demasiado contrato de Polizas puede diluir el vertical si no se fijan endpoints y permisos propios.
- Reutilizar metadata AppBuilder como runtime romperia la decision de arquitectura.
- Auth real pendiente puede limitar el cierre productivo.
- `SESSION_CONTEXT` puede requerir claves adicionales no confirmadas por DBA.

## Work Items

- Crear issue GitHub: `SDD-2026-006 Autos Particulares MVP read-only`.
- Crear issue backend: API explicita `/api/autos-particulares/polizas` con scope Autos/Particulares.
- Crear issue frontend: ruta Vue `/autos-particulares` read-only.
- Crear issue datos/DBA: confirmar campo o regla fiable para division `Particulares`.
- Crear issue QA/UAT: comparativa contra entorno autorizado y evidencia de minimizacion PII.
- Crear issue seguridad: permisos `autosParticulares.*` y revision de PII vehiculo/cliente.

## Definicion de hecho

- [ ] Criterios de aceptacion completados o bloqueos externos documentados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [ ] UAT de ramo `Autos` ejecutada contra entorno autorizado o bloqueada por DBA/entorno.
- [ ] Division `Particulares` confirmada o fallback controlado documentado como pendiente.
- [ ] Documentacion actualizada.
