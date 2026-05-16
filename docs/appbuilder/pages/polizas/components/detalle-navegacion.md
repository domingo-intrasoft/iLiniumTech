# Agente componente - Polizas detalle y navegacion

Fecha: 2026-05-16

Estado: documentacion completada con evidencia local. No es contrato runtime.

## Alcance del subagente

Este subagente analiza la navegacion listado -> detalle de Polizas, el modelo de pestanas de AppBuilder y la traduccion recomendada a rutas Vue/API explicitas.

Archivo permitido:

- `docs/appbuilder/pages/polizas/components/detalle-navegacion.md`

## Fuentes revisadas

iLiniumTech:

- `docs/appbuilder/polizas-detalle-analysis.md`
- `reports/polizas-metadata/polizas.metadata.sanitized.json`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizaDetailView.vue`
- `iLiniumTech.Frontend/src/features/polizas/polizasConstants.ts`
- `iLiniumTech.Frontend/src/router/index.ts`

AppBuilder:

- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\DetailCrud.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\saveSearchNavigationHelper.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppBreadcrumb.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppBreadcrumbOnly.vue`

## Evidencia AppBuilder

`SearchDetail.vue` abre detalle de dos formas:

- icono `pi pi-eye` cuando existe accion `btndetail`;
- celda clicable cuando una columna esta configurada como accion de detalle.

Metodos/eventos relevantes:

- `verDetalle(data)` emite `click:viewDetail`;
- `buildDataKeys(data)` construye claves primarias de la fila;
- `actionDetailCommandInFilterRequest(...)` decide si una columna actua como acceso directo;
- `hasOneMenu('btndetail')` y `hasMenuActions()` alternan icono directo y menu.

`CrudTable.vue` recibe `click:viewDetail`:

- guarda `keyData` y `keyDataObject`;
- puede abrir pestana con `addCustomTab(-1, [])`;
- puede reutilizar una pestana existente si las claves coinciden;
- puede construir cabecera sustituyendo marcadores `#campo#`.

`DetailCrud.vue` es el contenedor del detalle:

- si `tabMode=false`, muestra boton `pi pi-arrow-left` para volver al listado;
- si `tabMode=true`, muestra refrescar y cerrar pestana;
- puede montar `FormBuilder` con `componentId` o `detailComponentId`;
- si no hay componente dinamico, usa slots.

No se encontro un componente estatico heredado tipo `PolizaDetail.vue`. El detalle original depende de metadata, layout, tabs, componentes dinamicos y posibles workflows.

## Estado iLiniumTech actual

`PolizasTable.vue`:

- usa `RouterLink` desde la columna `numero`;
- anade accion visual con icono `pi pi-eye`;
- envia `detailQuery` para conservar filtros/paginacion al abrir detalle.

`PolizaDetailView.vue`:

- ruta explicita `/polizas/:id`;
- usa `GET /api/polizas/{id}` o fixture local;
- pre-valida configuracion runtime;
- en modo backend valida sesion mediante `/api/me`;
- exige broker si `polizasExecutionContextRequired`;
- exige permiso `polizas.detail`;
- limpia sesion local ante 401;
- boton de vuelta a `Polizas` conserva query params;
- muestra detalle read-only por secciones.

Secciones actuales:

- `Datos de poliza`;
- `Gestion`;
- `Tomador`.

## Propuesta Vue estatica

Mantener navegacion como producto normal:

- ruta `/polizas`;
- ruta `/polizas/:id`;
- `RouterLink` desde tabla;
- boton volver con query params;
- breadcrumb estatico;
- entrada directa por URL soportada;
- errores 401/403/404 sanitizados;
- sin tabs dinamicas heredadas.

No replicar:

- `FormBuilder`;
- `LAYOUT_DETAIL`;
- `addCustomTab` generico;
- `redirectParams` de busquedas guardadas;
- workflows de detalle;
- detalle por metadata;
- campos/acciones dinamicas desde `IAP_ComponentEvent`.

## Propuesta API estatica

`GET /api/polizas/{id}`:

- requiere `polizas.detail`;
- valida broker activo contra `allowedBrokerIds`;
- no debe revelar si la poliza existe en otro broker;
- devuelve DTO de detalle minimizado;
- no devuelve SQL, metadata, connection strings ni trazas internas.

DTO de detalle candidato:

- identificacion de poliza: `numero`, `certificado`, `tipoPoliza`, `estado`, `compania`, `ramo`;
- gestion: fechas, oficina, division, colaboradores, gestor, canal/fraccion de pago;
- tomador minimizado;
- prima anual si permiso de negocio lo permite;
- riesgo solo si no revela matricula/bastidor completos o si existe permiso.

## Campos sensibles

La seccion `Tomador` actual incluye:

- `documento`;
- `apellido1`;
- `apellido2`;
- `nombre`;
- `sexo`;
- `fechaNacimiento`;
- `edad`;
- `estadoCivil`;
- `hijos`;
- `regimenLaboral`;
- `profesion`;
- `email`;
- `telefono`.

Estos campos no deben llegar completos a produccion sin:

- auth real;
- permiso especifico;
- decision de minimizacion;
- auditoria de acceso al detalle;
- UAT funcional;
- validacion de proteccion de datos.

## Permisos candidatos

- `polizas.detail`: lectura del detalle.
- `polizas.detail.pii`: campos personales del tomador.
- `polizas.detail.financial`: prima y datos financieros si se separan.
- `polizas.detail.risk`: riesgo/matricula/bastidor si aplica.

Los permisos AppBuilder `View` y `List` son referencia historica, no autoridad runtime.

## Pestanas y navegacion

AppBuilder usa pestanas dinamicas para abrir detalles. En iLiniumTech se recomienda:

- no crear pestanas dinamicas como motor generico;
- usar rutas explicitas;
- si producto quiere multiples detalles abiertos, crear una funcionalidad propia con estado de UI, no con metadata heredada;
- conservar filtros via query params;
- no depender de `router.back()` como unica salida, porque la entrada directa debe funcionar.

## Riesgos

PII:

- el detalle es la zona de mayor riesgo de datos personales.
- no guardar capturas reales.
- no loguear DTO completo.

Autorizacion:

- ocultar enlace de detalle en UI no sustituye el 403 backend.
- el backend debe bloquear broker cruzado y permiso ausente.

Workflow:

- muchas acciones historicas de detalle pueden tener escrituras.
- no activar anulacion, duplicado, suspension, reemplazo, documentos o recibos sin SDD.

SQL:

- el detalle no debe usar `QueryStatic` ni queries de metadata.
- si se usa SQL real, debe ser whitelist y parametrizado.

## Pruebas obligatorias futuras

Frontend:

- link de detalle desde tabla;
- accion con icono de ojo;
- entrada directa `/polizas/:id`;
- vuelta conserva filtros;
- sin sesion no consulta detalle;
- sin broker no consulta detalle;
- sin `polizas.detail` no consulta detalle;
- 401 limpia sesion;
- DOM sin `IAP_`, `QueryStatic`, `Pantalla_Polizas`, SQL ni secrets.

Backend:

- `polizas.detail` obligatorio;
- broker cruzado denegado;
- id inexistente devuelve error sanitizado;
- id malicioso no interpola SQL;
- DTO no expone documento/email/telefono por defecto si no hay permiso.

QA/UAT:

- validar secciones y labels;
- confirmar campos de tomador;
- confirmar si prima/riesgo se muestran;
- confirmar comportamiento de polizas anuladas;
- confirmar auditoria para detalle.

## Bloqueos

- Falta UAT de campos de detalle.
- Falta auth real y matriz de permisos fina.
- Falta decision PII.
- Falta validacion SQL contra entorno autorizado.

## Estado final del subagente

Completado con evidencia para documentacion. El desarrollo futuro debe mantener rutas explicitas y evitar tabs dinamicas basadas en metadata.
