# Pagina AppBuilder: By Aunna

Fecha de analisis: 2026-05-16.

Rol de esta ronda: jefe de pagina para `By Aunna`.

Estado final: `bloqueado externo`.

Motivo del estado: `By Aunna` existe como entrada futura en el menu objetivo de iLiniumTech, pero no se ha encontrado metadata local suficiente para identificar una pantalla AppBuilder real. No hay `menuId`, `componentId`, ruta, arbol de componentes, tabs, submenus, datasources, filtros, acciones ni permisos historicos concretos. Las referencias locales a `Aunna` encontradas corresponden a marca/tenant/tema o comentarios tecnicos, no a una pagina funcional verificable.

## Regla base aplicada

iLiniumTech no debe reconstruir AppBuilder como runtime dinamico. La metadata heredada sirve para analisis, trazabilidad, SDD y scaffolding revisado. Si se aprueba una pagina `By Aunna`, debera quedar como Vue/TypeScript estatico y backend .NET con API explicita.

No se debe hacer que el frontend/backend productivo consulte `IAP_Menu`, `IAP_Component`, `IAP_DataSource`, configuracion de branding, themes, workflows o permisos AppBuilder para pintar, autorizar o ejecutar esta pantalla en runtime.

## Alcance de esta ronda

Incluido:

- leer gobierno, roadmap, plan maestro y navegacion actual;
- buscar evidencia local segura en iLiniumTech y AppBuilder;
- documentar evidencia encontrada y ausente;
- decidir si existen tabs, submenus o componentes internos reales que justifiquen subagentes;
- dejar propuesta futura solo como orientacion, no como contrato.

Fuera de alcance:

- programar frontend/backend;
- ejecutar extractor `Live`;
- ejecutar SQL;
- abrir `appsettings*`, `*.config`, connection strings, dumps, backups, capturas sensibles o datos personales reales;
- inventar estructura funcional.

## Fuentes revisadas

Documentacion iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `docs/appbuilder/menu-lateral-analysis.md`
- `docs/appbuilder/login-auth-multitenant-application-analysis.md`
- documentos ya generados en `docs/appbuilder/pages/*/README.md` como patron de inventario documental

Codigo iLiniumTech:

- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

Fuentes AppBuilder revisadas, sin abrir configuraciones sensibles ni ejecutar SQL:

- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\template\infrastructure\prime\LayoutHelper.ts`
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\Mail\ServicioMail.cs`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder`
- rutas seguras bajo `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion`

Fuentes evitadas expresamente:

- `appsettings*`
- `*.config`
- archivos con `connection` o `Connection` en el nombre
- dumps, backups, comprimidos y capturas
- datos personales reales
- extractor `Live`
- consultas SQL reales

## Busquedas ejecutadas

Se usaron busquedas locales con `rg` sobre `C:\Desarrollo\AppBuilder` e iLiniumTech, acotando globs para evitar configuraciones sensibles.

Terminos principales:

- `By Aunna`
- `ByAunna`
- `by-aunna`
- `by_aunna`
- `sitemap`
- `Aunna`
- `menu`
- `componentId`
- `tab`
- `submenu`

Resultados relevantes:

- `By Aunna`, `ByAunna`, `by-aunna` y `by_aunna` no aparecen en fuentes AppBuilder revisadas.
- En iLiniumTech solo aparece `By Aunna` en `docs/appbuilder/pages/README.md` y `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- `Aunna` aparece en AppBuilder como referencia de tema visual `aunna` dentro de `LayoutHelper.ts`.
- `Aunna` aparece tambien como ejemplo de despliegue/cliente en un comentario tecnico de `ServicioMail.cs`.
- No se encontro una fila local de menu, localizacion, ruta estatica, componente raiz, datasource, permiso o workflow asociado a `By Aunna`.

## Evidencia en iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts` existe una entrada:

| Campo | Valor |
| --- | --- |
| `label` | `By Aunna` |
| `icon` | `pi pi-sitemap` |
| `disabled` | `true` |
| `to` | ausente |
| `requiredPermission` | ausente |
| `children` | ausente |

Interpretacion:

- iLiniumTech reconoce `By Aunna` como item futuro de navegacion.
- Actualmente no hay ruta Vue.
- Actualmente no hay pagina implementada.
- Actualmente no hay permiso iLiniumTech definido.
- Actualmente no hay contrato API.
- Actualmente no hay submenus ni hijos en la navegacion estatica.

## Evidencia AppBuilder generica de menu

AppBuilder resuelve el menu desde metadata. La operacion `Menu_GET_BY_APPLICATION_ID.graphql` carga menus por:

- administrador;
- `profileId`;
- `applicationId`;
- version.

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

El modelo `IapMenu` en TypeScript confirma que una entrada heredada puede estar enlazada a un `componentId`, a rutas estaticas o a jerarquia por `parentId`. `HelperMenu.ts` transforma esa lista plana en arbol, usando `title` como etiqueta, `idIcon` como icono, `active` como visibilidad, `componentId` o rutas estaticas como base de navegacion y `parentId` para hijos.

Para `By Aunna`, esta evidencia solo explica como se cargaria una entrada heredada si existiera. No identifica la fila concreta. Sin una extraccion sanitizada de `IAP_Menu` para la aplicacion/version real, no se puede conocer:

- id de menu;
- padre;
- orden;
- estado activo;
- icono real;
- ruta;
- componente raiz;
- hijos;
- permisos historicos.

## Evidencia AppBuilder de marca/tema

`LayoutHelper.ts` contiene una paleta llamada `aunna` con color principal corporativo. Esta evidencia indica soporte de tema visual o branding en el motor/plantilla AppBuilder, no una pantalla funcional.

`ServicioMail.cs` menciona `Aunna` como ejemplo de despliegue/cliente que puede configurar secrets de correo en su propio `appsettings.json`. No se abrio ningun `appsettings` ni se leyo ningun secreto. Esta referencia indica contexto multi-cliente/despliegue, no una pagina `By Aunna`.

Interpretacion:

- `Aunna` aparece como marca, tenant o configuracion visual.
- `By Aunna` podria ser un item de menu de branding, enlace corporativo, agrupador, placeholder o modulo no extraido.
- No hay evidencia local que permita tratarlo como pagina de negocio.

## Evidencia ausente

No se ha encontrado, en fuentes locales seguras:

- fila metadata de menu concreta para `By Aunna`;
- localizacion AppBuilder `By Aunna`;
- `componentId` raiz;
- ruta estatica;
- padre o hijos de menu heredados;
- arbol de componentes;
- tabs reales;
- submenus reales;
- componentes internos reales;
- datasources;
- filtros;
- columnas;
- catalogos;
- acciones;
- workflows;
- permisos historicos por perfil/grupo;
- relacion con broker, aplicacion o tenant mas alla de branding generico;
- origen de datos de negocio.

## Raiz, hijos, tabs y submenus

Estado detectado:

| Elemento | Resultado |
| --- | --- |
| Menu raiz heredado | No encontrado |
| `componentId` raiz | No encontrado |
| Ruta estatica heredada | No encontrada |
| Hijos de menu | No encontrados |
| Tabs reales | No encontrados |
| Submenus reales | No encontrados |
| Componentes internos reales | No encontrados |
| Datasources reales | No encontrados |
| Workflows reales | No encontrados |

Decision de esta ronda:

- No se crean subagentes de componentes.
- No se crean ficheros bajo `docs/appbuilder/pages/by-aunna/components/*.md`.

Razon: no hay tabs, submenus ni componentes internos reales de `By Aunna` detectados. Crear documentos de componentes ahora generaria falsa trazabilidad y podria empujar a desarrollar una pantalla inventada.

Condicion para crear subagentes en una ronda futura:

- disponer de una fila sanitizada de `IAP_Menu` o equivalente con `title = By Aunna` o localizacion confirmada;
- disponer del `componentId` raiz o ruta estatica confirmada;
- disponer del arbol sanitizado de componentes si existe;
- clasificar cada hijo real como tab, submenu, grid, formulario, toolbar, enlace, panel de branding, dashboard u otro tipo;
- asignar un subagente por componente real con escritura acotada a `docs/appbuilder/pages/by-aunna/components/<slug>.md`.

## Datos y origenes

Datos concretos de `By Aunna`:

- No encontrados.

No se ha identificado:

- tabla o vista real;
- datasource AppBuilder;
- query heredada;
- catalogos;
- claves primarias;
- relacion funcional con broker;
- campos visibles;
- campos sensibles;
- origen en Master, Modelo, Builder, reporting u otra base.

Regla para futuro desarrollo:

- no crear endpoints de `By Aunna` hasta confirmar que problema de negocio resuelve;
- no usar configuracion de tema `aunna` como contrato funcional;
- no reutilizar tablas del motor AppBuilder como si fueran datos de producto;
- no ejecutar `QueryStatic` ni construir SQL desde metadata;
- validar broker y permisos antes de resolver cualquier conexion;
- clasificar PII antes de exponer campos si aparece un origen de datos real.

## Filtros

No hay filtros heredados verificables para `By Aunna`.

Filtros candidatos solo si producto define el caso de uso:

- texto libre;
- estado;
- categoria;
- fecha;
- broker;
- aplicacion;
- tipo de contenido;
- origen o canal.

Estos filtros son hipotesis de producto, no evidencia AppBuilder. No deben implementarse sin SDD/UAT.

## Acciones

No hay acciones heredadas verificables para `By Aunna`.

Acciones candidatas solo si producto aprueba alcance:

- abrir contenido corporativo;
- listar recursos;
- consultar detalle;
- descargar documento;
- navegar a enlace externo autorizado;
- administrar contenido;
- publicar o despublicar;
- auditar cambios.

Si `By Aunna` fuese un area de comunicacion, intranet, branding o enlaces corporativos, cada accion debe definirse como caso de uso iLiniumTech. No debe conectarse a workflows heredados ni a rutas dinamicas AppBuilder.

## Permisos historicos

Evidencia generica:

- AppBuilder usa menu, componentes y object groups para decidir visibilidad y operaciones.
- Las acciones historicas observadas en otros analisis incluyen `view`, `list`, `add`, `edit`, `delete`, `import`, `export` y `execute`.
- El menu heredado depende de aplicacion, version, perfil, administrador y directivas.

Permisos historicos concretos de `By Aunna`:

- No encontrados.

Permisos iLiniumTech candidatos, pendientes de SDD:

- `byAunna.read`
- `byAunna.detail`
- `byAunna.content.read`
- `byAunna.content.manage`
- `byAunna.export`
- `byAunna.audit.read`

No se debe activar ningun permiso de escritura, publicacion, administracion o ejecucion hasta tener auth productiva, matriz funcional, broker validado, auditoria y UAT.

## Propuesta Vue estatica futura

Solo si producto confirma el significado de `By Aunna` y se crea SDD, la primera version recomendable deberia ser read-only.

Ruta candidata:

- `/by-aunna`

Menu candidato:

- `label`: `By Aunna`
- `icon`: pendiente de validar; el actual es `pi pi-sitemap`, probablemente placeholder
- `requiredPermission`: `byAunna.read`
- `disabled`: `false` solo cuando exista SDD, API, tests y UAT minimo

Estructura candidata si fuese una pagina de contenido:

- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaView.vue`
- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaFilters.vue`
- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaContentList.vue`
- `iLiniumTech.Frontend/src/features/by-aunna/ByAunnaDetailView.vue`
- `iLiniumTech.Frontend/src/features/by-aunna/byAunnaTypes.ts`
- `iLiniumTech.Frontend/src/features/by-aunna/useByAunna.ts`
- `iLiniumTech.Frontend/src/services/byAunna.ts`

Estados obligatorios:

- cargando;
- sin resultados;
- error backend;
- sin sesion;
- sin broker si aplica;
- sin permiso;
- datos no disponibles por bloqueo externo.

Reglas frontend:

- no consumir metadata AppBuilder;
- leer identidad, broker y permisos desde `/api/me`;
- no mostrar acciones sin permiso efectivo;
- no usar temas AppBuilder como fuente de comportamiento;
- conservar filtros en query params solo si se valida que sera pantalla diaria;
- no mostrar nombres de tablas, `QueryStatic`, `IAP_*`, rutas internas ni detalles de infraestructura en DOM.

## Propuesta API estatica futura

Solo si hay SDD, origen de datos autorizado y permiso funcional confirmado:

- `GET /api/by-aunna/catalogs`
- `GET /api/by-aunna`
- `GET /api/by-aunna/{id}`

Si se aprueban escrituras o administracion de contenido:

- `POST /api/by-aunna`
- `PUT /api/by-aunna/{id}`
- `POST /api/by-aunna/{id}/publish`
- `POST /api/by-aunna/{id}/unpublish`
- `GET /api/by-aunna/{id}/audit`

Reglas backend:

- validar sesion antes de resolver datos;
- validar `currentBrokerId` contra `allowedBrokerIds` si la informacion depende de broker;
- aplicar permisos por endpoint;
- usar DTOs explicitos;
- usar repositorios explicitos;
- usar whitelists para filtros, sort y columnas;
- parametrizar valores;
- devolver errores sanitizados con `correlationId`;
- minimizar PII por defecto;
- registrar auditoria si hay detalle, descarga, exportacion, publicacion o administracion.

## Que no debe replicarse

No replicar:

- menu runtime desde `IAP_Menu`;
- rutas dinamicas por `componentId`, `urlComponentStatic` o `urlRouteComponentStatic`;
- carga de pantallas por `IAP_Component`;
- datasources desde `IAP_DataSource`;
- configuracion de tema `aunna` como comportamiento funcional;
- branding heredado como decision de permisos;
- workflows, expresiones o integraciones genericas;
- permisos AppBuilder como autoridad runtime;
- SQL heredado o `QueryStatic`;
- pantalla administrativa de tenant/cliente sin decision explicita de producto y seguridad.

## Riesgos

Riesgos de producto:

- Convertir una entrada placeholder de menu en funcionalidad inventada.
- Confundir branding `Aunna` con una pagina de negocio.
- Implementar un area corporativa sin responsable funcional.
- No saber si `By Aunna` pertenece a cliente, broker, aplicacion, intranet, ayuda, branding o administracion.

Riesgos tecnicos:

- Reintroducir runtime dinamico por la puerta trasera.
- Acoplar la UI a temas, menus o metadata AppBuilder.
- Crear una abstraccion generica de contenidos sin caso de uso real.
- Definir rutas o endpoints antes de saber el origen de datos.

Riesgos de seguridad:

- Exponer configuracion interna de cliente/tenant.
- Publicar enlaces, documentos o contenido sin autorizacion.
- Filtrar rutas internas, infraestructura, SQL, secretos o datos personales.
- Usar API key/demo-session como seguridad productiva.

Riesgos multi-tenant:

- No saber si el contenido es global, por broker, por aplicacion, por perfil o por usuario.
- Resolver conexion antes de validar broker.
- Exponer contenido de otro broker o entorno.

Riesgos de privacidad:

- Si fuese area de comunicacion o documentos, podria incluir datos personales, comerciales o contractuales.
- Si fuese administracion de tenant, podria incluir configuracion sensible.

## Bloqueos

Bloqueos externos:

- Confirmar con producto que significa `By Aunna` dentro de la aplicacion final.
- Confirmar si existe en AppBuilder actual o es funcionalidad nueva.
- Obtener metadata sanitizada de `IAP_Menu` para la aplicacion/version real.
- Obtener `componentId` raiz o ruta estatica si existe.
- Obtener arbol sanitizado de componentes si existe.
- Obtener datasources/campos/configuraciones sanitizadas si se va a usar como evidencia.
- Confirmar origen de datos autorizado con DBA o responsable de producto.
- Confirmar si tiene alcance read-only, contenido, enlaces, administracion, dashboard o workflow.
- Confirmar permisos funcionales por broker, perfil, oficina, gestor y usuario si aplica.
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

- validar SDD de `By Aunna`;
- validar metadata sanitizada si se obtiene;
- validar que no se introduce runtime AppBuilder;
- validar responsable funcional y UAT owner.

Backend:

- 401 sin sesion;
- 403 sin permiso `byAunna.read`;
- broker ausente si aplica;
- broker no permitido si aplica;
- validacion de filtros y sort;
- payloads maliciosos;
- errores sanitizados con `correlationId`;
- no fuga de metadata, SQL, rutas internas ni secretos;
- tests de minimizacion PII si aplica;
- tests de auditoria si hay detalle, descarga, exportacion, publicacion o administracion.

Frontend:

- menu visible solo con permiso si se activa;
- ruta protegida;
- estados loading, empty, error, sin broker y sin permiso;
- filtros y paginacion si se aprueban;
- detalle read-only si se aprueba;
- no aparecen `AppBuilder`, `QueryStatic`, `IAP_`, nombres SQL, rutas internas ni connection strings en DOM.

QA/UAT:

- smoke desktop/mobile;
- comparativa con AppBuilder solo con datos autorizados y sanitizados si existe pantalla heredada;
- validacion de etiquetas, contenido, filtros y acciones;
- evidencia sin capturas sensibles ni datos personales reales.

Seguridad:

- secret scan;
- dependency audit si se introduce libreria nueva;
- CORS audit si se toca API/configuracion;
- revision de PII;
- threat review si se confirma que `By Aunna` permite administracion, publicacion o enlaces externos.

## Decision sobre subagentes de componentes

No se crean subagentes en esta ronda.

Razon: no hay componentes internos reales de `By Aunna`. Las referencias a `Aunna` son de tema/branding/despliegue y no son hijos verificables de una pagina concreta.

Si aparece metadata en una ronda futura, los posibles subagentes dependeran de lo encontrado. Ejemplos de categorias posibles, no confirmadas:

- filtros;
- listado;
- detalle;
- contenido;
- enlaces;
- dashboard;
- administracion;
- auditoria;
- tabs;
- submenus;
- toolbar.

Ninguna de estas categorias debe convertirse en documento ni codigo hasta disponer de evidencia real.

## Estado final de pagina

Clasificacion:

- Completado con evidencia: analisis de menu iLiniumTech, mecanismo generico AppBuilder de menus y referencias AppBuilder a `Aunna` como tema/branding.
- Pendiente tecnico: localizar metadata sanitizada real de `By Aunna`, si existe.
- Bloqueado externo: definicion funcional, origen de datos, permisos reales, broker/multi-tenant y UAT.

Conclusion:

`By Aunna` no esta listo para desarrollo. El siguiente paso correcto es preguntar a producto que significa exactamente esta entrada de menu y obtener metadata sanitizada o validacion funcional antes de planificar frontend/backend.

## Readiness tecnico/admin actualizado 2026-05-18

Estado actual en iLiniumTech:

- Existe ruta protegida `/by-aunna` como pagina Vue estatica.
- Existe fixture local read-only con contenido candidato, filtros locales y paginacion.
- No existe API backend de By Aunna.
- No existen permisos iLiniumTech aprobados para By Aunna.
- No hay enlaces reales, descargas, publicacion, detalle operativo ni contenido externo.
- La evidencia frontend verifica que no se renderizan marcadores como `IAP_`, `QueryStatic`, `ComponentDataSource`, `connectionString`, `SELECT *`, `Pantalla_`, `appsettings`, endpoints reales, tokens, API keys, emails ni documentos.

Acciones bloqueadas:

- publicar;
- abrir enlace;
- descargar;
- abrir detalle;
- administrar contenido;
- usar branding o tema heredado como contrato funcional.

Permisos candidatos no aprobados:

- `byAunna.read`;
- `byAunna.detail`;
- `byAunna.content.read`;
- `byAunna.content.manage`;
- `byAunna.export`;
- `byAunna.audit.read`.

Threat model requerido antes de cualquier dato/API:

- confirmar si es branding, contenido, intranet, enlaces o modulo funcional;
- validacion de enlaces externos y prevencion de phishing/open redirect;
- permisos para publicar/despublicar;
- revision de documentos, descargas y contenido con PII;
- auditoria de cambios de contenido;
- separacion por broker/tenant si aplica;
- redaccion de rutas internas y configuracion de marca.

Tareas futuras pequenas recomendadas:

1. Confirmar significado de `By Aunna` con producto.
2. Decidir si es pagina real o solo branding.
3. Preparar SDD de contenido read-only si se aprueba.
4. Definir politica de enlaces externos autorizados.
5. Mantener acciones de publicacion/enlaces deshabilitadas.

Criterios de aceptacion para avanzar:

- SDD aprobada y owner UAT;
- permisos backend definidos;
- allowlist de enlaces si existen;
- contenido sanitizado y sin secretos;
- auditoria para publicacion o administracion;
- ninguna metadata AppBuilder ni tema heredado como runtime funcional.
