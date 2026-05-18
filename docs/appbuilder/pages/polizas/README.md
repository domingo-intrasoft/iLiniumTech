# Pagina AppBuilder - Polizas

Fecha: 2026-05-16

Rol: jefe de pagina `Polizas`.

Estado: documentacion completada con evidencia local. No se ha programado runtime. No es contrato runtime.

## Regla base

iLiniumTech no es un runtime dinamico tipo AppBuilder. La metadata heredada se usa para analisis, trazabilidad, SDD y scaffolding revisado. La aplicacion final debe compilar y funcionar como frontend Vue/TypeScript estatico y backend API explicita.

## Objetivo visual activo

Desde el 2026-05-18, el siguiente MVP de `Polizas` debe priorizar paridad visual con la pantalla AppBuilder publicada aportada por el usuario. La guia operativa esta en `visual-parity-mvp.md` y la SDD en `docs/sdd/specs/iLiniumTech/SDD-2026-014-polizas-appbuilder-visual-parity.md`.

La fidelidad visual no autoriza atajos: sin metadata runtime, sin datos simulados, sin logos inventados, sin acciones falsas y sin perder el CRUD real contra backend/BBDD local de pruebas ya conseguido.

Para `Polizas`, eso significa:

- no renderizar pantallas desde `IAP_Component`;
- no construir menus desde metadata;
- no ejecutar `QueryStatic`;
- no aceptar `GroupSearch`, `fieldId` o nombres SQL como API publica;
- no activar workflows o acciones AppBuilder sin SDD;
- convertir cada hallazgo util en codigo iLiniumTech revisado, con tests.

## Organizacion de agentes

Como jefe de pagina, se coordinaron estos subagentes documentales:

| Subagente | Entrega | Estado |
| --- | --- | --- |
| Busqueda/Filtros | `components/busqueda-filtros.md` | Completado con evidencia |
| Listado/Grid | `components/listado-grid.md` | Completado con evidencia |
| Detalle/Navegacion | `components/detalle-navegacion.md` | Completado con evidencia |
| Toolbar/Acciones contextuales | `components/toolbar-acciones-contextuales.md` | Completado con evidencia parcial |
| Submenus Polizas | `components/submenus-polizas.md` | Completado con evidencia parcial |

Ningun subagente ha tocado frontend, backend, extractor, configuracion o ficheros sensibles.

## Fuentes revisadas

Gobierno y producto:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`

Documentacion AppBuilder/iLiniumTech:

- `docs/appbuilder/polizas-filtros-busqueda-analysis.md`
- `docs/appbuilder/polizas-listado-analysis.md`
- `docs/appbuilder/polizas-detalle-analysis.md`
- `docs/appbuilder/menu-polizas-mvp-implementation.md`
- `docs/MVP_POLIZAS_DIFERENCIAS.md`
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md`
- `docs/sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md`

Metadata sanitizada:

- `reports/polizas-metadata/polizas.metadata.sanitized.json`

Frontend actual:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`
- `iLiniumTech.Frontend/src/features/polizas/PolizasView.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasFilters.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizaDetailView.vue`
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`
- `iLiniumTech.Frontend/src/features/polizas/polizasTypes.ts`

Backend actual, por referencia:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Security/PolizasAuthorization.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Domain/Polizas`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Application/Polizas`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas`

AppBuilder, revisado mediante busquedas y fuentes ya documentadas sin abrir configuraciones sensibles:

- CRUD frontend: `CrudTable.vue`, `Search.vue`, `SearchDetail.vue`, `DetailCrud.vue`.
- Busqueda/lookup frontend: `SearchTree.vue`, `SearchFields.vue`, `LookUpEditor.vue`.
- GraphQL: `Search_SEARCH.graphql`, `Search_SEARCH_MULTIPLE.graphql`, `Search_SEARCHLOOKUP.graphql`.
- Backend search: `SearchQuery.cs`, `bllSearch.cs`, `ServicioSearch.cs`, `RepositorioSearch.cs`, `HelperDataSourceField.cs`.

No se ha ejecutado SQL, extractor `Live`, ni se han abierto appsettings, dumps, capturas sensibles o connection strings.

## Trazabilidad de pagina

Metadata canonica de `Polizas`:

| Elemento | Valor |
| --- | --- |
| Aplicacion | `2`, `Aunna Tech` |
| Version | `1` |
| Menu | `10`, `Polizas` |
| Componente raiz | `2824`, `Busqueda Polizas` |
| Componente hijo CRUD | `2825`, `CrudPoliza` |
| ComponentDataSource | `354`, `Dat_PantallaPolizas` |
| DataSource | `146`, `Pantalla_Polizas` |
| Model object | `Pantalla_Polizas` |
| Tipo BBDD | `tipobd-MO` |
| Runtime contract | `false` en metadata sanitizada |

Relaciones:

- `2824` -> `2825`: relacion raiz-hijo.
- `2825` -> `354` -> `146`: CRUD enlazado a datasource de Polizas.
- `QueryStatic`: presente y redaccionado. Solo evidencia, no ejecucion.

## Componentes detectados

### Busqueda y filtros

Origen AppBuilder:

- `Search.vue` para acciones y modos simple/avanzado/general.
- `SearchTree.vue` y `SearchFields.vue` para arbol de criterios.
- `GroupSearch` como modelo heredado.
- `LookUp*` para catalogos/lookups.

Traduccion iLiniumTech:

- `PolizasFilters.vue`;
- filtros tipados de producto;
- query params propios;
- catalogos por API explicita.

Documento:

- `components/busqueda-filtros.md`

### Listado y grid

Origen AppBuilder:

- `SearchDetail.vue` con `DataTable` lazy, paginacion, sort multiple, filtros de columna, acciones y exportacion.
- backend `SearchFromDataBase` con count/paginacion.

Traduccion iLiniumTech:

- `PolizasTable.vue`;
- columnas TypeScript;
- page/pageSize;
- accion `Ver detalle`;
- errores/empty/loading.

Documento:

- `components/listado-grid.md`

### Detalle y navegacion

Origen AppBuilder:

- `verDetalle(data)`;
- `click:viewDetail`;
- `buildDataKeys`;
- `addCustomTab`;
- `DetailCrud.vue`;
- posible `FormBuilder`.

Traduccion iLiniumTech:

- ruta `/polizas/:id`;
- `PolizaDetailView.vue`;
- `GET /api/polizas/{id}`;
- vuelta a `/polizas` conservando filtros.

Documento:

- `components/detalle-navegacion.md`

### Toolbar y acciones contextuales

Origen:

- evidencia visual/documental de toolbar y botones;
- acciones AppBuilder pueden ser menus, workflows o eventos;
- metadata sanitizada no confirma evento completo.

Traduccion iLiniumTech:

- botones estaticos deshabilitados;
- acciones futuras solo por SDD/API/permiso.

Documento:

- `components/toolbar-acciones-contextuales.md`

### Submenus

Estado actual:

- `Autos Particulares`: visible como hijo deshabilitado, incremento anterior aparcado.
- `Flotas`: visible como hijo deshabilitado.
- `Colectivas`: visible como hijo deshabilitado.
- `Polizas de flota`, `Polizas colectivas`, `Polizas Externas`: botones contextuales deshabilitados.

No hay evidencia suficiente para convertirlos en funcionalidad activa.

Documento:

- `components/submenus-polizas.md`

## Datos y campos

Campos visibles heredados desde metadata sanitizada:

| Campo heredado | Label | Uso candidato |
| --- | --- | --- |
| `Poliza` | Poliza | Listado, filtro, detalle |
| `Aplicacion` | Aplicacion | Listado/filtro si UAT confirma |
| `IdTipoPoliza` | Tipo poliza | Catalogo/filtro/detalle |
| `NumDocumento` | Documento | PII, bloquear o permiso especifico |
| `IdSituacion` | Situacion | Estado |
| `IdRamo` | Ramo | Catalogo/filtro |
| `Riesgo` | Riesgo | Revisar PII/matricula |
| `F_Efecto` | Fecha efecto | Fecha/filtro |
| `F_Vencimiento` | Fecha vencimiento | Fecha/filtro |
| `F_Anulacion` | Fecha anulacion | Secundario |
| `IdMotivoAnulacion` | Motivo anulacion | Catalogo secundario |
| `Cia` | Compania | Catalogo/filtro |
| `PAnualCartera` | Prima anual cartera | Importe |
| `NombreCompleto` | Tomador | PII |
| `AlertaInformativa` | Alerta informativa | Indicador futuro |
| `AlertaExclamativa` | Alerta exclamativa | Indicador futuro |
| `AlertaRstrictiva` | Alerta restrictiva | Normalizar typo antes de producto |

Catalogos detectados:

- `TipoPoliza`;
- `SituacionPoliza`;
- `Ramo`;
- `MotivoAnulacion`;
- `Compania`.

Catalogos adicionales usados por la UI actual, pendientes de validar contra contrato real:

- oficina;
- division;
- colaborador1;
- administrativo;
- comercial;
- siniestros;
- gestor;
- canalCobro;
- fraccionPago;
- ccaa;
- sexo;
- estadoCivil;
- regimenLaboral;
- profesion.

## Permisos iLiniumTech candidatos

Permisos ya documentados:

- `polizas.catalogs`;
- `polizas.read`;
- `polizas.detail`;
- `polizas.export` reservado.

Permisos candidatos futuros:

- `polizas.search.pii`;
- `polizas.detail.pii`;
- `polizas.detail.financial`;
- `polizas.detail.risk`;
- `polizas.savedSearch.manage`;
- permisos por scope si se reactivan Flotas/Colectivas/Externas.

Los permisos AppBuilder `ObjectGroup` son evidencia historica. La autoridad debe ser backend iLiniumTech.

## Propuesta de arquitectura Vue/API estatica

Frontend:

- `appNavigation.ts` mantiene menu estatico.
- `PolizasView.vue` orquesta filtros, grid y estado de sesion.
- `PolizasFilters.vue` mantiene formulario de producto.
- `PolizasTable.vue` mantiene tabla y navegacion a detalle.
- `PolizaDetailView.vue` mantiene detalle read-only.
- componentes compartidos solo si reducen duplicacion real, no para recrear AppBuilder.

Backend:

- `/api/me` como contrato de identidad, broker, permisos y auth mode.
- `GET /api/polizas/catalogs`.
- `GET /api/polizas`.
- `GET /api/polizas/{id}`.
- `POST /api/auth/broker` para demo/control de broker, sustituible por auth real.
- repositorios explicitos con whitelist y parametros.
- `SESSION_CONTEXT` parametrizado si las vistas reales lo requieren.

Datos:

- resolver de broker antes de SQL;
- `allowedBrokerIds` como control de aislamiento;
- no fallback silencioso a fixtures en modo backend real;
- no logs de SQL, connection strings o DTOs sensibles.

## Lo que no debe replicarse

- render dinamico de `IAP_Component`;
- formularios desde `FormBuilder`;
- `GroupSearch` publico;
- `QueryStatic`;
- placeholders SQL heredados;
- menus hijos desde AppBuilder;
- workflows genericos;
- `ExecuteProc`;
- busquedas guardadas sin SDD;
- exportacion masiva sin permiso/auditoria;
- permisos AppBuilder como motor runtime;
- accion por label de boton.

## Riesgos principales

PII:

- documento, nombre completo, email, telefono, fechas personales, riesgo/matricula y prima.

SQL:

- `QueryStatic` redaccionado indica dependencia historica de SQL dinamico.
- lookups redaccionados deben redisenarse como catalogos API.

Multi-tenant:

- `brokerId`/`EntityMainCurrentId` debe validarse antes de resolver conexion.
- `SESSION_CONTEXT` puede ser obligatorio y debe aplicarse por request.

Workflows:

- acciones de toolbar o detalle pueden escribir datos o llamar integraciones.

Producto:

- `Autos Particulares` queda aparcado.
- `Flotas`, `Colectivas`, `Externas` no tienen regla funcional cerrada.

## Pruebas requeridas antes de desarrollo final

Documentacion:

- validar baseline documental;
- secret scan limpio.

Frontend:

- unit tests de filtros, URL, tabla, detalle y permisos;
- E2E/smoke `/login -> /polizas -> detalle -> volver`;
- DOM sin metadata/SQL/secrets;
- viewport desktop/mobile si cambia visual.

Backend:

- permisos `polizas.catalogs/read/detail`;
- broker no permitido;
- sort/filtros maliciosos;
- errores sanitizados con `correlationId`;
- no PII por defecto;
- `SESSION_CONTEXT` si aplica.

UAT:

- campos visibles;
- labels;
- filtros;
- catalogos;
- estados de anulacion;
- alertas;
- submenus y acciones.

## Bloqueos externos

- Auth productiva no decidida.
- Matriz real de permisos pendiente.
- Reglas de Flotas/Colectivas/Externas pendientes.
- Decision PII pendiente.
- Validacion DBA de SQL, vistas, catalogos y `SESSION_CONTEXT` pendiente.
- UAT con entorno autorizado pendiente.

## Estado final de pagina

Completado con evidencia:

- pagina Polizas identificada y trazada;
- componentes principales documentados;
- submenus clasificados;
- propuesta Vue/API estatica definida;
- riesgos y pruebas escritos.

Pendiente tecnico:

- convertir esta documentacion en tareas de desarrollo por componentes;
- actualizar SDD si se amplian filtros, columnas o acciones;
- validar PII y permisos antes de exponer detalle ampliado.

Bloqueado externo:

- UAT funcional y DBA para equivalencia con datos reales.

## Siguiente ronda recomendada

Despues de revision humana, crear tareas de desarrollo separadas:

1. filtros soportados y query params;
2. grid con columnas UAT y gating de detalle;
3. detalle minimizado por permisos;
4. catalogos backend revisados;
5. pruebas backend/frontend;
6. decision sobre submenus aparcados.

Ninguna tarea debe consumir esta documentacion como runtime. Debe convertirla en codigo fuente revisado.
