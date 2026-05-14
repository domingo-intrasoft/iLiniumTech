# SDD: MVP componente Polizas

## Metadata

- Spec ID: SDD-2026-001
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-13

## Contexto

AppBuilder contiene un componente real de busqueda de polizas configurado por metadata. El objetivo es usarlo como primer corte MVP de iLiniumTech, extrayendo su forma y contrato sin migrar todo AppBuilder.

Decision de arquitectura: la metadata se usa para trazabilidad y scaffolding inicial. La pantalla final de iLiniumTech debe ser Vue estatico consumiendo una API backend explicita, no un render dinamico de AppBuilder.

Metadata localizada:

- Aplicacion `2`, version `1`.
- Menu `10`, titulo `Polizas`.
- Componente raiz `2824`, `Busqueda Polizas`.
- Componente CRUD `2825`, `CrudPoliza`.
- ComponentDataSource `354`.
- DataSource `146`, `Pantalla_Polizas`.
- Objeto modelo `Pantalla_Polizas`.

## Objetivo

Crear una primera aplicacion backend/frontend que represente el componente de polizas como listado de solo lectura, con contrato estable, metadata trazable y controles de seguridad desde el inicio.

## Fuera de alcance

- Escritura de polizas.
- Acciones de anulacion, duplicado, reemplazo, suspension o revigorizacion.
- Workflows.
- Google Wallet.
- REST/SOAP externos.
- Motor completo de expresiones.
- Ejecucion directa de SQL dinamico de AppBuilder.
- Render dinamico desde metadata `IAP_*` en runtime.

## Contrato de datos

Trazabilidad:

- Las referencias AppBuilder de aplicacion, menu, componentes y datasource quedan en la spec y documentacion.
- El backend no publica metadata de pantalla como contrato runtime.
- El endpoint historico `/api/polizas/metadata` queda deprecado y no devuelve referencias AppBuilder.

Listado:

- `items`.
- `page`.
- `pageSize`.
- `total`.

Catalogos:

- `tipoPoliza`.
- `compania`.
- `ramo`.
- `oficina`.
- `gestor`.
- catalogos auxiliares necesarios para filtros codificados en Vue.

Campos iniciales:

- `id`.
- `numero`.
- `aplicacion`.
- `estado`.
- `ramo`.
- `clienteId`.
- `clienteNombre`.
- `compania`.
- `fechaEfecto`.
- `fechaVencimiento`.
- `primaAnual`.
- `moneda`.

## Reglas de negocio

- El MVP es de solo lectura.
- Solo se permiten campos de ordenacion declarados por whitelist.
- La UI no debe mostrar datos personales reales en fixtures.
- El backend no debe devolver SQL, connection strings ni trazas internas.
- La trazabilidad AppBuilder debe conservarse en documentacion y artefactos offline, sin gobernar la UI en produccion.

## Criterios de aceptacion

- [x] Existe proyecto `iLiniumTech.Backend`.
- [x] Existe proyecto `iLiniumTech.Frontend`.
- [x] `GET /api/polizas/metadata` queda deprecado y no devuelve referencias AppBuilder.
- [x] `GET /api/polizas/catalogs` devuelve catalogos funcionales para filtros.
- [x] `GET /api/polizas` devuelve listado paginado con datos anonimizados.
- [x] Los endpoints `/api/polizas/*` requieren API key.
- [x] Ordenaciones fuera de whitelist devuelven error de validacion.
- [x] La UI `/polizas` renderiza tabla y filtros desde codigo Vue/TypeScript propio.
- [x] La UI `/polizas/:id` renderiza detalle read-only desde API/fixture.
- [x] La UI `/polizas` valida configuracion runtime y contexto `/api/me` antes de consultar backend.
- [x] No se guardan secretos ni cadenas de conexion en Git.

## Impacto tecnico

Backend:

- `iLiniumTech.Backend.Domain/Polizas`.
- `iLiniumTech.Backend.Application/Polizas`.
- `iLiniumTech.Backend.Infrastructure/Polizas`.
- `iLiniumTech.Backend.Api`.

Frontend:

- `iLiniumTech.Frontend/src/features/polizas`.
- `iLiniumTech.Frontend/src/router`.
- `iLiniumTech.Frontend/src/services`.

## Seguridad

- [x] Secretos fuera de Git.
- [x] API key por configuracion, con placeholder no valido.
- [x] CORS por `Cors:AllowedOrigins`.
- [x] Sin `AllowAnyOrigin`.
- [x] Sort y filtros limitados por contrato.
- [x] Fixtures anonimizados.
- [x] Gitleaks ejecutado.

## Plan de pruebas

- Unitarias backend: validador de busqueda y whitelist de sort.
- Integracion backend: health anonimo, polizas protegido, catalogos, metadata deprecada y error de sort.
- Unitarias frontend: columnas locales, filtros por numero/catalogos y detalle.
- Build frontend: typecheck y Vite build.
- Seguridad: Gitleaks, dependency audit y CORS audit.
- Manual/UAT: abrir `/polizas` y validar que la tabla corresponde al contrato MVP.

## Estado de cierre QA/UAT

Completado con evidencia:

- Frontend polizas es Vue/TypeScript propio y no renderiza desde metadata AppBuilder.
- En modo backend, `usePolizas` bloquea busquedas si falta configuracion runtime o no se puede validar sesion/contexto.
- `runtimeConfig` exige API key no placeholder y valida `VITE_BROKER_ID` como entero positivo antes de enviarlo.
- `/api/me` se usa para exponer el contexto efectivo de polizas al frontend.
- Pruebas versionadas cubren bloqueo por configuracion incompleta, sesion no validada y cabecera `X-Broker-Id`.

Pendiente tecnico:

- Ejecutar smoke/UAT visual en entorno elegido para cada cierre de fase.
- Sustituir API key/headers MVP por auth real segun `SDD-2026-005`.

Bloqueado externo:

- Falta proveedor/mecanismo auth y mapa funcional de permisos para retirar contexto MVP.

## Riesgos

- Las restricciones reales de `Pantalla_Polizas` dependen de `SESSION_CONTEXT`.
- La lectura real puede requerir permisos por perfil, oficina, gestor o mediador.
- La UI real contiene muchas acciones no incluidas.
- La siguiente fase debe disenar un repositorio SQL seguro antes de leer datos reales.

## Work Items

- Crear issue GitHub: `SDD-2026-001 Polizas MVP read-only`.
- Crear issue GitHub: `Extractor seguro metadata polizas`.
- Crear issue GitHub: `Repositorio SQL whitelist Pantalla_Polizas`.

## Definicion de hecho

- [x] Criterios de aceptacion completados para el corte actual.
- [x] Pruebas ejecutadas y documentadas.
- [x] Gates de seguridad aplicables ejecutados.
- [x] Documentacion actualizada.
