# Pagina AppBuilder: Controles

Fecha de analisis: 2026-05-16.

Rol de esta ronda: jefe de pagina para `Controles`.

Estado final: `bloqueado externo`.

Motivo del estado: `Controles` existe como entrada futura en el menu objetivo de iLiniumTech, pero no se ha encontrado metadata local suficiente para identificar una pantalla AppBuilder real con `menuId`, `componentId`, arbol de componentes, tabs, submenus, datasources, filtros, acciones o permisos historicos concretos. La palabra `Control` aparece ampliamente en AppBuilder como parte del motor tecnico de render dinamico, pero esa evidencia no prueba la existencia de una pagina funcional de negocio llamada `Controles`.

## Regla base

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. La metadata heredada sirve para analisis, trazabilidad, SDD y scaffolding revisado. Si se aprueba una pagina `Controles`, debera quedar como Vue/TypeScript estatico y backend .NET con API explicita.

No se debe hacer que el frontend/backend productivo consulte `IAP_Menu`, `IAP_Component`, `IAP_DataSource`, catalogos de controles, workflows o expresiones heredadas para pintar, autorizar o ejecutar la pantalla en runtime.

## Fuentes revisadas

Documentacion iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- documentos ya generados en `docs/appbuilder/pages/*/README.md` como patron de inventario documental

Codigo iLiniumTech:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

Fuentes AppBuilder revisadas, sin abrir configuraciones sensibles ni ejecutar SQL:

- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Component_GET_COMPONENT_TREE_ALLOBJCETS_BY_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Entidades\AppBuilder\IapMenu.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\DynamicControlNameConst.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\ControlTypeConst.cs`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\workflow`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Apis\Intrasoft.ApiBuilder`

Fuentes evitadas expresamente:

- `appsettings*`
- `*.config`
- connection strings
- dumps, backups y capturas
- datos personales reales
- extractor `Live`
- consultas SQL reales

## Busquedas ejecutadas

Se usaron busquedas locales con `rg` sobre `C:\Desarrollo\AppBuilder` e iLiniumTech, acotando globs para evitar configuraciones sensibles. Terminos principales:

- `Controles`
- `Control`
- `Controls`
- `Menu`
- `IapMenu`
- `componentQuery`
- `getTreeAllObjectsById`
- `TabView`
- `TabPanel`
- `DynamicTab`
- `ControlTypeConst`
- `DynamicControlNameConst`

Resultado relevante:

- `Controles` no aparece como pagina concreta en las fuentes AppBuilder revisadas.
- No se ha encontrado `menuId`, `componentId`, `parentId`, `treePath`, ruta estatica ni localizacion concreta asociada a una pagina `Controles`.
- Existen muchas referencias a `Control`, `Controls` y `tipocontrol-*`, pero corresponden al motor AppBuilder de controles dinamicos, no a una pagina de negocio confirmada.
- Existen operaciones GraphQL para cargar menus y arboles de componentes, pero no hay un artefacto local sanitizado que contenga la fila real de menu o el arbol real de `Controles`.
- No se ha encontrado evidencia local de tabs, submenus o componentes internos reales de `Controles`.

## Evidencia en iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts` existe una entrada:

| Campo | Valor |
| --- | --- |
| `label` | `Controles` |
| `icon` | `pi pi-home` |
| `disabled` | `true` |
| `to` | ausente |
| `requiredPermission` | ausente |
| `children` | ausente |

Interpretacion:

- iLiniumTech reconoce `Controles` como item futuro de navegacion.
- Actualmente no hay ruta Vue.
- Actualmente no hay pagina implementada.
- Actualmente no hay permiso iLiniumTech definido.
- Actualmente no hay contrato API.
- Actualmente no hay submenus ni hijos en la navegacion estatica.

## Evidencia AppBuilder generica de menu

AppBuilder resuelve el menu desde metadata. La operacion `Menu_GET_BY_APPLICATION_ID.graphql` carga menus por:

- `administrator`;
- `profileId`;
- `applicationId`;
- `version`.

El contrato devuelve campos como:

- `id`;
- `applicationId`;
- `applicationVersion`;
- `componentId`;
- `urlComponentStatic`;
- `urlRouteComponentStatic`;
- `title`;
- `idIcon`;
- `parentId`;
- `contract`;
- `builtin`;
- `order`;
- `active`;
- `keepAlive`;
- `localizations`.

El modelo `IapMenu` en TypeScript y C# confirma que una entrada heredada puede estar enlazada a un `componentId`, a rutas estaticas o a jerarquia por `parentId`.

Para `Controles`, esa evidencia solo explica como se cargaria un menu heredado, pero no identifica la fila concreta. Sin una extraccion sanitizada de `IAP_Menu` para la aplicacion/version real, no se puede conocer:

- id de menu;
- padre;
- orden;
- estado activo;
- icono real;
- ruta;
- componente raiz;
- hijos;
- permisos historicos.

## Evidencia AppBuilder generica de componentes

La operacion `Component_GET_COMPONENT_TREE_ALLOBJCETS_BY_ID.graphql` puede devolver el arbol de una pantalla AppBuilder a partir de un `componentId`, con:

- identidad de componentes;
- `parentId`;
- `treePath`;
- `idType`;
- `idSubType`;
- atributos;
- datasources;
- configuracion de campos;
- lookups;
- eventos;
- expresiones;
- workflows;
- permisos por `objectGroups`.

Sin embargo, no existe en local un `componentId` raiz confirmado para `Controles`. Por tanto, no se puede pedir ni documentar su arbol real.

## Evidencia de controles genericos AppBuilder

AppBuilder contiene un catalogo tecnico amplio de controles dinamicos. Se observaron, entre otros:

- controles de formulario: `DynamicInputText`, `DynamicInputNumber`, `DynamicCalendar`, `DynamicDropDown`, `DynamicMultiSelect`, `DynamicCheckBox`, `DynamicRadioButton`;
- controles de estructura: `DynamicPanel`, `DynamicFieldset`, `DynamicTabView`, `TabPanel`, `DynamicDataTable`, `DynamicColumn`, `DynamicDataView`;
- controles de accion: `DynamicButton`, `DynamicSplitButton`, `DynamicToolBar`, `DynamicMenu`, `DynamicMenuItem`;
- controles especiales: `DynamicCrudTabla`, `DynamicCrudDocument`, `DynamicFullCalendar`, `DynamicLookUp`, `DynamicExpression`;
- constantes backend `tipocontrol-*` y layouts `layouttemp-*`.

Esta evidencia es importante para entender el motor heredado, pero no debe confundirse con una pagina `Controles`. Una pantalla llamada `Controles` podria ser:

- una pagina funcional de negocio aun no localizada;
- una pagina administrativa del propio AppBuilder;
- un menu placeholder trasladado al menu objetivo;
- una mala traduccion de "controles" como widgets del motor;
- un item que existia en una BBDD de metadata no presente en el repo local.

Con la evidencia actual no se puede elegir una de esas interpretaciones sin riesgo de inventar producto.

## Raiz, hijos, tabs y submenus

Estado detectado:

| Elemento | Resultado |
| --- | --- |
| Menu raiz heredado | No encontrado |
| `componentId` raiz | No encontrado |
| Ruta estatica heredada | No encontrada |
| Hijos de menu | No encontrados |
| Tabs reales | No encontradas |
| Submenus reales | No encontrados |
| Componentes internos reales | No encontrados |
| Datasources reales | No encontrados |
| Workflows reales | No encontrados |

Decision de esta ronda:

- No se crean subagentes de componentes.
- No se crean ficheros bajo `docs/appbuilder/pages/controles/components/*.md`.

Razon: no hay tabs, submenus ni componentes internos reales de `Controles` detectados. Crear documentos de componentes ahora generaria falsa trazabilidad y podria empujar a desarrollar una pantalla inventada.

Condicion para crear subagentes en una ronda futura:

- disponer de una fila sanitizada de `IAP_Menu` o equivalente con `title = Controles` o localizacion confirmada;
- disponer del `componentId` raiz o ruta estatica confirmada;
- disponer del arbol sanitizado de componentes;
- clasificar cada hijo real como tab, submenu, grid, formulario, toolbar, detalle, workflow u otro tipo;
- asignar un subagente por componente real con escritura acotada a `docs/appbuilder/pages/controles/components/<slug>.md`.

## Datos y origenes

Datos concretos de `Controles`:

- No encontrados.

No se ha identificado:

- tabla o vista real;
- datasource AppBuilder;
- query heredada;
- catalogos;
- claves primarias;
- relacion con broker;
- campos visibles;
- campos sensibles;
- origen en Modelo, Maestro, Solicitudes, Motor u otra base.

Regla para futuro desarrollo:

- no crear endpoints de `Controles` hasta confirmar que problema de negocio resuelve;
- no reutilizar tablas del motor AppBuilder de controles como si fueran datos de producto;
- no ejecutar `QueryStatic` ni construir SQL desde metadata;
- validar broker y permisos antes de resolver cualquier conexion;
- clasificar PII antes de exponer campos.

## Filtros

No hay filtros heredados verificables para `Controles`.

Filtros candidatos solo si producto define el caso de uso:

- texto libre;
- estado;
- tipo de control;
- fecha de alta/modificacion;
- responsable;
- broker/oficina/area;
- criticidad o prioridad;
- categoria funcional.

Estos filtros son hipotesis de producto, no evidencia AppBuilder. No deben implementarse sin SDD/UAT.

## Acciones

No hay acciones heredadas verificables para `Controles`.

Acciones candidatas solo si producto aprueba alcance:

- listar controles;
- consultar detalle;
- crear o editar control;
- activar/desactivar;
- asignar responsable;
- registrar revision;
- exportar;
- ejecutar una accion de validacion;
- consultar historico.

Advertencia: si `Controles` estuviera relacionado con el motor tecnico AppBuilder, no debe migrarse como administracion generica de componentes sin una decision explicita. Esa superficie seria de alto riesgo porque podria incluir pantallas, permisos, workflows, expresiones, SQL o integraciones.

## Permisos historicos

Evidencia generica:

- AppBuilder usa `objectGroups` con flags como `add`, `edit`, `list`, `delete`, `view`, `import`, `export` y `execute`.
- El menu heredado depende de aplicacion, version, perfil, administrador y directivas.
- Los componentes pueden tener permisos en menu, componente, datasource y evento.

Permisos historicos concretos de `Controles`:

- No encontrados.

Permisos iLiniumTech candidatos, pendientes de SDD:

- `controles.read`
- `controles.detail`
- `controles.create`
- `controles.update`
- `controles.disable`
- `controles.export`
- `controles.audit.read`
- `controles.execute`

No se debe activar ningun permiso de escritura o ejecucion hasta tener auth productiva, matriz funcional, broker validado, auditoria y UAT.

## Propuesta Vue estatica futura

Solo si producto confirma el significado de `Controles` y se crea SDD, la primera version recomendable deberia ser read-only.

Ruta candidata:

- `/controles`

Menu candidato:

- `label`: `Controles`
- `icon`: pendiente de validar; el actual es `pi pi-home`, probablemente placeholder
- `requiredPermission`: `controles.read`
- `disabled`: `false` solo cuando exista SDD, API, tests y UAT minimo

Estructura candidata:

- `iLiniumTech.Frontend/src/features/controles/ControlesView.vue`
- `iLiniumTech.Frontend/src/features/controles/ControlesFilters.vue`
- `iLiniumTech.Frontend/src/features/controles/ControlesTable.vue`
- `iLiniumTech.Frontend/src/features/controles/ControlDetailView.vue`
- `iLiniumTech.Frontend/src/features/controles/controlesTypes.ts`
- `iLiniumTech.Frontend/src/features/controles/useControles.ts`
- `iLiniumTech.Frontend/src/services/controles.ts`

Estados obligatorios:

- cargando;
- sin resultados;
- error backend;
- sin sesion;
- sin broker;
- sin permiso;
- datos no disponibles por bloqueo externo.

Reglas frontend:

- no consumir metadata AppBuilder;
- leer identidad, broker y permisos desde `/api/me`;
- no mostrar acciones sin permiso efectivo;
- conservar filtros en query params si se valida que sera pantalla diaria;
- no mostrar nombres de tablas, `QueryStatic`, `IAP_*` ni detalles internos en DOM.

## Propuesta API estatica futura

Solo si hay SDD, origen de datos autorizado y permiso funcional confirmado:

- `GET /api/controles/catalogs`
- `GET /api/controles`
- `GET /api/controles/{id}`

Si se aprueban escrituras:

- `POST /api/controles`
- `PUT /api/controles/{id}`
- `POST /api/controles/{id}/disable`
- `GET /api/controles/{id}/audit`

Reglas backend:

- validar sesion antes de resolver datos;
- validar `currentBrokerId` contra `allowedBrokerIds`;
- aplicar permisos por endpoint;
- usar DTOs explicitos;
- usar repositorios explicitos;
- usar whitelists para filtros, sort y columnas;
- parametrizar valores;
- devolver errores sanitizados con `correlationId`;
- minimizar PII por defecto;
- registrar auditoria si hay detalle, exportacion o ejecucion.

## Que no debe replicarse

No replicar:

- catalogo generico de `tipocontrol-*` como producto final;
- `FormBuilder` como runtime de la pagina;
- `DynamicControlNameConst` como contrato frontend;
- `ControlTypeConst` como motor de UI iLiniumTech;
- menus desde `IAP_Menu`;
- componentes desde `IAP_Component`;
- datasources desde `IAP_DataSource`;
- workflows de Rete;
- nodos `ExecuteScript`, `StoreProcedure`, `SendMail`, `SendSms` o similares sin SDD propia;
- permisos AppBuilder como autoridad runtime;
- SQL heredado o `QueryStatic`;
- pantalla administrativa del motor AppBuilder sin decision explicita de producto y seguridad.

## Riesgos

Riesgos de producto:

- Confundir el menu `Controles` con el catalogo tecnico de controles AppBuilder.
- Convertir un placeholder de navegacion en funcionalidad.
- Implementar una pantalla administrativa no solicitada.
- Ocultar bajo el nombre `Controles` un caso de uso que necesita definicion funcional propia.

Riesgos tecnicos:

- Reintroducir runtime dinamico por la puerta trasera.
- Crear componentes genericos que repliquen AppBuilder.
- Depender de metadata para UI, permisos, filtros o acciones.
- No conocer claves, joins, columnas, estados o relaciones reales.

Riesgos de seguridad:

- Exponer capacidades administrativas del motor heredado.
- Permitir ejecucion de workflows, scripts, procedimientos o integraciones.
- Filtrar SQL, metadata interna, tokens, connection strings o datos personales.
- Usar API key/demo-session como seguridad productiva.

Riesgos multi-tenant:

- No saber si los datos pertenecen a broker, aplicacion, perfil o administracion global.
- Resolver conexion antes de validar broker.
- Exponer controles de otro broker o entorno.

Riesgos de privacidad:

- Si `Controles` es una pantalla de seguimiento operativo, podria incluir nombres, observaciones, responsables, fechas o incidencias.
- Si es administracion tecnica, podria incluir payloads, valores de configuracion o rutas internas.

## Bloqueos

Bloqueos externos:

- Confirmar con producto que significa `Controles` dentro de la aplicacion final.
- Confirmar si existe en AppBuilder actual o es una funcionalidad nueva.
- Obtener metadata sanitizada de `IAP_Menu` para la aplicacion/version real.
- Obtener `componentId` raiz o ruta estatica de `Controles`.
- Obtener arbol sanitizado de componentes si existe.
- Obtener datasources/campos/configuraciones sanitizadas si se va a usar como evidencia.
- Confirmar origen de datos autorizado con DBA.
- Confirmar si tiene alcance read-only, CRUD, administracion, auditoria o workflow.
- Confirmar permisos funcionales por broker, perfil, oficina, gestor y usuario.
- Confirmar UAT owner.

Bloqueos tecnicos:

- No existe ruta Vue.
- No existe contrato API.
- No existe fixture anonimizado.
- No existe SDD.
- No existe matriz de permisos.
- No existe evidencia de tabs/submenus para subagentes de componentes.

## Pruebas necesarias para desarrollo futuro

Documentacion:

- validar SDD de `Controles`;
- validar metadata sanitizada si se obtiene;
- validar que no se introduce runtime AppBuilder.

Backend:

- 401 sin sesion;
- 403 sin permiso `controles.read`;
- broker ausente;
- broker no permitido;
- validacion de filtros y sort;
- payloads maliciosos;
- errores sanitizados con `correlationId`;
- no fuga de metadata, SQL ni secretos;
- tests de minimizacion PII si aplica;
- tests de auditoria si hay detalle/exportacion/ejecucion.

Frontend:

- menu visible solo con permiso;
- ruta protegida;
- estados loading, empty, error, sin broker y sin permiso;
- filtros y paginacion si se aprueban;
- detalle read-only si se aprueba;
- no aparecen `AppBuilder`, `QueryStatic`, `IAP_`, nombres SQL ni connection strings en DOM.

QA/UAT:

- smoke desktop/mobile;
- comparativa con AppBuilder solo con datos autorizados y sanitizados;
- validacion de columnas, etiquetas, filtros y acciones;
- evidencia sin capturas sensibles ni datos personales reales.

Seguridad:

- secret scan;
- dependency audit si se introduce libreria nueva;
- CORS audit si se toca API/configuracion;
- revision de PII;
- threat review si se confirma que `Controles` permite ejecucion o administracion.

## Decision sobre subagentes de componentes

No se crean subagentes en esta ronda.

Razon: no hay componentes internos reales de `Controles`. Las referencias a controles dinamicos son del motor AppBuilder y no son hijos verificables de una pagina concreta.

Si aparece metadata en una ronda futura, los posibles subagentes dependeran de lo encontrado. Ejemplos de categorias posibles, no confirmadas:

- filtros;
- listado;
- detalle;
- formulario;
- tabs;
- toolbar;
- auditoria;
- acciones/workflow.

Ninguna de estas categorias debe convertirse en documento ni codigo hasta disponer de evidencia real.

## Estado final de pagina

Clasificacion:

- Completado con evidencia: analisis de menu iLiniumTech y mecanismo generico AppBuilder de menus, componentes y controles.
- Pendiente tecnico: localizar metadata sanitizada real de `Controles`.
- Bloqueado externo: definicion funcional, origen de datos, permisos reales, broker/multi-tenant y UAT.

Conclusion:

`Controles` no esta listo para desarrollo. El siguiente paso correcto es preguntar a producto que significa exactamente esta entrada de menu y obtener metadata sanitizada o validacion funcional antes de planificar frontend/backend.

## Readiness tecnico/admin actualizado 2026-05-18

Estado actual en iLiniumTech:

- Existe ruta protegida `/controles` como pagina Vue estatica.
- Existe fixture local read-only con controles candidatos, filtros locales y paginacion.
- No existe API backend de controles.
- No existen permisos iLiniumTech aprobados para controles.
- No hay ejecucion real, validacion real, auditoria real ni workflow.
- La evidencia frontend verifica que no se renderizan marcadores como `IAP_`, `QueryStatic`, `ComponentDataSource`, `connectionString`, `SELECT *`, `Pantalla_`, `appsettings`, `datasource`, PII ni secretos.

Acciones bloqueadas:

- validar alcance;
- refrescar contra backend;
- exportar;
- abrir detalle real;
- ejecutar controles;
- activar workflows, scripts, procedimientos o motores heredados.

Permisos candidatos no aprobados:

- `controles.read`;
- `controles.detail`;
- `controles.create`;
- `controles.update`;
- `controles.disable`;
- `controles.export`;
- `controles.audit.read`;
- `controles.execute`.

Threat model requerido antes de cualquier dato/API:

- definicion funcional de que significa `Controles`;
- separacion explicita frente al motor de controles dinamicos AppBuilder;
- permisos por lectura, detalle, exportacion y ejecucion;
- bloqueo de workflows/scripts/procedimientos heredados;
- minimizacion de PII si los controles son operativos;
- auditoria de ejecuciones;
- aislamiento por broker/perfil/rol.

Tareas futuras pequenas recomendadas:

1. Confirmar el significado funcional de `Controles`.
2. Localizar metadata sanitizada solo como evidencia, no runtime.
3. Crear SDD read-only si existe caso de negocio.
4. Definir permisos candidatos sin concederlos.
5. Mantener pagina fixture y tests de no runtime AppBuilder.

Criterios de aceptacion para avanzar:

- problema de negocio y owner UAT confirmados;
- SDD aprobada;
- security review si hay ejecucion o auditoria;
- API explicita sin motor generico;
- ninguna dependencia runtime de `IAP_Component`, `IAP_DataSource`, catalogos de controles o workflows;
- acciones reales probadas con permisos backend y auditoria.
