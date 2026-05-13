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

## Contrato de datos

Metadata:

- `resource`: `polizas`.
- `version`: version del contrato.
- `appBuilder`: referencias a aplicacion, menu, componentes y datasource.
- `fields`: columnas visibles/filter/sort permitidas.

Listado:

- `items`.
- `page`.
- `pageSize`.
- `total`.

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
- La metadata debe conservar la trazabilidad a AppBuilder.

## Criterios de aceptacion

- [x] Existe proyecto `iLiniumTech.Backend`.
- [x] Existe proyecto `iLiniumTech.Frontend`.
- [x] `GET /api/polizas/metadata` devuelve referencias AppBuilder `2824`, `2825`, `354` y `146`.
- [x] `GET /api/polizas` devuelve listado paginado con datos anonimizados.
- [x] Los endpoints `/api/polizas/*` requieren API key.
- [x] Ordenaciones fuera de whitelist devuelven error de validacion.
- [x] La UI `/polizas` renderiza tabla y filtros.
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
- Integracion backend: health anonimo, polizas protegido, metadata y error de sort.
- Unitarias frontend: metadata AppBuilder, filtro por numero y whitelist visible.
- Build frontend: typecheck y Vite build.
- Seguridad: Gitleaks, dependency audit y CORS audit.
- Manual/UAT: abrir `/polizas` y validar que la tabla corresponde al contrato MVP.

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
