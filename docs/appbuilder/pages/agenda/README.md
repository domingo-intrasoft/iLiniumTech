# Agenda - analisis documental AppBuilder

Fecha: 2026-05-16

Estado: documentacion de evidencia parcial. Desarrollo bloqueado hasta localizar metadata completa o validacion funcional autorizada.

## Rol de esta ronda

Este documento recoge el analisis como jefe de pagina para `Agenda`. La ronda es solo documental: no se ha programado frontend, backend, extractor Live ni SQL.

Regla base aplicada: iLiniumTech no debe convertirse en un runtime dinamico tipo AppBuilder. La evidencia heredada sirve para comprender, trazar y preparar SDD/scaffolding revisado. La futura pantalla Agenda, si se aprueba, debera ser Vue/TypeScript estatico y API .NET explicita.

## Fuentes revisadas

iLiniumTech:

- `AGENTS.md`
- `PLANS.md`
- `README.md`
- `docs/PLAN_MAESTRO_IA.md`
- `docs/ROADMAP_OBJETIVO_FINAL.md`
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`
- `docs/appbuilder/pages/README.md`
- `docs/appbuilder/menu-lateral-analysis.md`
- `docs/appbuilder/login-auth-multitenant-application-analysis.md`
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`

AppBuilder, solo fuentes locales no sensibles:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\infrastructure\funciones\HelperSecurity.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\ObjectGroupConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Constants\FullCalendarTypeConst.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicFullCalendar.vue`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\domain\Controls\CalendarClass.ts`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\form\infrastructure\controls\editorTemplates\prime\DynamicCalendar\DynamicCalendar.vue`
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`

Fuentes evitadas expresamente:

- `appsettings*`, `Web.config`, `App.config`, connection strings, dumps, backups, capturas sensibles, datos personales reales.
- Extractor `Live` y consultas SQL reales.

## Evidencia encontrada

### Menu actual iLiniumTech

`iLiniumTech.Frontend/src/layout/appNavigation.ts` contiene una entrada de menu:

- label: `Agenda`
- icono: `pi pi-calendar`
- estado: `disabled: true`
- ruta: ausente
- permiso iLiniumTech: ausente
- hijos: ausentes

Esto confirma que Agenda existe en la taxonomia inicial del menu, pero no existe pagina implementada ni contrato funcional iLiniumTech.

### Localizacion AppBuilder

`Messages.ts` define la etiqueta de menu dentro de `inicio`:

- espanol: `Agenda`
- ingles: `Diary`

La ubicacion bajo `inicio` sugiere que historicamente Agenda pudo pertenecer al area de inicio/panel personal, no necesariamente a una pantalla de gestion comparable a Polizas.

### Menu dinamico AppBuilder

`Menu_GET_BY_APPLICATION_ID.graphql` muestra que AppBuilder cargaba menus por administrador, perfil, aplicacion y version. El contrato devuelve, entre otros:

- `id`
- `parentId`
- `order`
- `title`
- `active`
- `idIcon`
- `componentId`
- `urlComponentStatic`
- `urlRouteComponentStatic`
- `keepAlive`
- `localizations`

`HelperMenu.ts` transforma esa lista plana en arbol, usando `title` como etiqueta, `idIcon` como icono, `componentId` o rutas estaticas para navegacion y `active` como visibilidad.

No se encontro en fuentes locales no sensibles una fila concreta de menu para Agenda con `componentId`, ruta estatica, padre, hijos, orden o permisos. Por tanto, no hay evidencia local suficiente para afirmar cual era el componente raiz real de la pagina Agenda.

### Tabla y vista de modelo

`NombreTablasConst.ts` declara:

- `AGENDA = "Agenda"`
- `VW_AGENDA = "vw_Agenda"`

`ModeloDbContext.cs` contiene:

- `DbSet<Agendum> Agenda`
- `DbSet<VwAgendum> VwAgenda`

La configuracion de `Agenda` observada en `ModeloDbContext` incluye campos y restricciones relevantes:

- Indice por `FInicio`.
- Indice unico parcial por `IdOld` y `BrokerIntegracionId` cuando `IdOld` no es nulo.
- `Asunto` con longitud maxima 100.
- `Descripcion` con longitud maxima 2000.
- `F_Inicio` y `F_Fin` como `date`.
- `Hora_Inicio` y `Hora_Fin` con precision 0.
- `IdObjeto`, `IdOld`, `IdPrioridad`.
- `IdentidadId` con longitud maxima 1000.
- `BrokerIntegracionId` con valor por defecto historico.
- Campos de auditoria `FCR`, `FUM`, `UCR`, `UUM`.
- Relacion obligatoria con `BrokerIntegracion`.

La configuracion de `vw_Agenda` observada incluye:

- Vista sin clave.
- Campos `Title`, `Start`, `End`, `IdentidadId`.
- `Title` con longitud maxima 100.
- `Start` y `End` como textos de longitud maxima 4000.

La existencia de `vw_Agenda` encaja con una representacion de eventos de calendario, pero no confirma por si sola la pantalla, filtros, permisos ni acciones de usuario.

### Relacion con siniestros

`ModeloDbContext.cs` configura la tabla `Siniestro` con el trigger `Tgr_AgendaSiniestroEvento`.

Esta evidencia sugiere que algunos eventos de Agenda pueden generarse o mantenerse desde cambios en siniestros. No se ha abierto SQL del trigger ni se ha ejecutado ninguna consulta, por lo que no se conoce la regla funcional exacta.

### Control generico FullCalendar

`DynamicFullCalendar.vue` es un control generico AppBuilder basado en `@fullcalendar/vue3` con plugins:

- `dayGrid`
- `timeGrid`
- `list`
- `interaction`

Comportamientos observados:

- Vista inicial configurable por atributo `defaultView`, con fallback `dayGridMonth`.
- Cabecera con navegacion anterior/siguiente, titulo central y botones `dayGridMonth`, `timeGridWeek`, `timeGridDay`, `listWeek` y `Actualizar`.
- Carga eventos desde un datasource generico y los convierte mediante `fieldsDefinition` y `dataSourceMapping`.
- Abre dialogo al seleccionar fecha o evento.
- Puede usar `commandAdd` y `commandEdit` para workflows personalizados.
- Si no hay workflow personalizado, usa formularios genericos `NewRegister` o `FormBuilder`.
- Permite actualizar fechas al mover/cambiar un evento mediante `eventChange` y `doOperationData`.
- Solo renderiza si el componente esta visible y `canDoOperation(ObjectGroupConst.VIEW)` devuelve acceso.

Esta es evidencia de una tecnologia de calendario disponible en AppBuilder, pero no prueba que la pagina Agenda concreta la usara en esta aplicacion/version/broker. Por eso queda documentado como candidato, no como componente real confirmado.

## Evidencia ausente

No se ha encontrado, en fuentes locales seguras:

- Fila metadata de menu concreta para `Agenda`.
- `componentId` raiz de Agenda.
- Padre/hijos reales dentro del menu heredado.
- Ruta estatica o componente estatico asociado.
- Metadata de componentes de la pantalla Agenda.
- Pestañas, submenus o areas internas confirmadas.
- Datasource concreto asociado a la pantalla.
- `fieldsDefinition` y `dataSourceMapping` concretos para Agenda.
- Catalogo historico de prioridad/tipo/estado asociado a `IdPrioridad`.
- Workflows reales de alta, edicion, navegacion o detalle.
- Matriz de permisos por perfil/grupo para Agenda.
- Query segura y autorizada para listar eventos.
- Regla exacta del trigger de siniestros.

## Subagentes de componentes

No se han creado subagentes de componentes ni archivos bajo `docs/appbuilder/pages/agenda/components/`.

Motivo: aunque existe un control generico `DynamicFullCalendar`, no hay evidencia local suficiente que lo vincule como componente hijo real de la pagina Agenda. Crear documentacion de componente como si fuese parte confirmada de Agenda generaria una falsa certeza para el desarrollo posterior.

Si en una ronda futura aparece metadata completa, los subagentes recomendados serian:

- `AgendaCalendar`: calendario de eventos si se confirma `DynamicFullCalendar` o equivalente.
- `AgendaEventDetail`: detalle modal o pagina de evento si se confirma alta/edicion/consulta.
- `AgendaFilters`: filtros por rango, prioridad, identidad u objeto si se confirman en metadata/UAT.
- `AgendaRelatedObjectNavigation`: navegacion a objeto relacionado si se confirma `IdObjeto`/`IdentidadId`.

## Lectura funcional provisional

La evidencia apunta a una agenda basada en eventos con:

- titulo/asunto;
- descripcion;
- fecha de inicio y fin;
- hora de inicio y fin;
- prioridad;
- relacion con una identidad u objeto;
- aislamiento por `BrokerIntegracionId`;
- posible alimentacion automatica desde siniestros.

Esta lectura es provisional. No debe convertirse en codigo sin SDD y validacion funcional.

## Datos candidatos

Campos candidatos para un DTO read-only futuro:

- `id`: identificador interno no reversible si procede.
- `title`: titulo visible del evento, minimizado si contiene PII.
- `start`: fecha/hora inicio normalizada ISO 8601.
- `end`: fecha/hora fin normalizada ISO 8601 o nula.
- `priorityId`: prioridad historica si se valida catalogo.
- `relatedObjectType`: tipo de objeto relacionado si se valida `IdObjeto`.
- `relatedObjectId`: identificador opaco, nunca libre si puede contener PII.
- `brokerId`: no enviado como campo de confianza al frontend; debe salir del contexto backend.

Campos que requieren especial cuidado:

- `Descripcion`: texto libre con alto riesgo de datos personales o informacion sensible.
- `IdentidadId`: longitud maxima 1000; podria contener una lista, serializacion o identificadores multiples.
- `IdObjeto`: puede inducir navegacion generica heredada; en iLiniumTech solo debe mapearse a enlaces explicitos aprobados.
- `Start` y `End` de `vw_Agenda`: estan mapeados como texto, por lo que necesitan parseo controlado y pruebas de fechas.

## Filtros candidatos

No hay filtros confirmados por metadata. Si producto aprueba Agenda, los filtros minimos deberian definirse en SDD:

- rango de fechas obligatorio o por defecto;
- texto sobre asunto, si se permite;
- prioridad, si existe catalogo validado;
- tipo de objeto relacionado, si `IdObjeto` es funcionalmente fiable;
- origen del evento, si hay eventos generados por siniestros u otros modulos;
- broker activo siempre desde sesion backend, no desde parametro libre.

No se debe filtrar por SQL heredado ni por strings de metadata en runtime.

## Acciones candidatas

Acciones observadas como posibles en controles genericos, no confirmadas para Agenda:

- refrescar calendario;
- cambiar vista mensual/semanal/diaria/lista;
- abrir evento;
- seleccionar fecha para crear evento;
- mover evento para cambiar fechas;
- ejecutar alta/edicion por workflow `commandAdd` o `commandEdit`.

Para iLiniumTech, el primer incremento seguro deberia ser read-only:

- consultar eventos por rango;
- ver detalle minimizado;
- navegar a objeto relacionado solo si hay contrato explicito.

Alta, edicion, borrado, drag-and-drop y cambios generados por workflow deben quedar fuera hasta SDD propia, permisos efectivos y UAT.

## Permisos historicos observados

AppBuilder maneja permisos por grupos/objetos con acciones observadas:

- `add`
- `edit`
- `delete`
- `view`
- `list`
- `import`
- `export`
- `execute`

`DynamicFullCalendar` comprueba `view` para renderizar. Las acciones de alta/edicion dependen de configuracion de componente, datasource y workflows, pero no hay evidencia concreta de permisos Agenda por grupo/perfil.

Permisos iLiniumTech candidatos, no aprobados:

- `agenda.read`
- `agenda.detail`
- `agenda.create`
- `agenda.update`
- `agenda.delete`
- `agenda.reschedule`
- `agenda.export`

Recomendacion: iniciar solo con `agenda.read` y `agenda.detail` si se aprueba un MVP read-only. Cualquier permiso de escritura debe esperar decision de producto/seguridad.

## Propuesta Vue/API estatica futura

### Frontend

Ruta propuesta solo tras SDD:

- `/agenda`

Estructura candidata:

- `iLiniumTech.Frontend/src/features/agenda/AgendaView.vue`
- `iLiniumTech.Frontend/src/features/agenda/AgendaCalendar.vue`
- `iLiniumTech.Frontend/src/features/agenda/AgendaFilters.vue`
- `iLiniumTech.Frontend/src/features/agenda/AgendaEventDetailView.vue` o modal controlado
- `iLiniumTech.Frontend/src/features/agenda/agendaTypes.ts`
- `iLiniumTech.Frontend/src/features/agenda/useAgenda.ts`
- `iLiniumTech.Frontend/src/services/agenda.ts`

Comportamiento esperado:

- no consumir metadata AppBuilder;
- leer sesion desde `/api/me`;
- bloquear si falta broker activo;
- bloquear si falta `agenda.read`;
- conservar rango/vista/filtros en query params si se requiere navegacion reproducible;
- mostrar estados de carga, vacio, error, sin permiso y sin broker;
- no mostrar descripciones largas ni PII sin permiso especifico.

### Backend

Endpoints candidatos solo tras SDD:

- `GET /api/agenda/catalogs`
- `GET /api/agenda/events?start=YYYY-MM-DD&end=YYYY-MM-DD&view=month|week|day|list`
- `GET /api/agenda/events/{id}`

Escrituras fuera del primer incremento:

- `POST /api/agenda/events`
- `PUT /api/agenda/events/{id}`
- `DELETE /api/agenda/events/{id}`
- `PATCH /api/agenda/events/{id}/schedule`

Reglas backend:

- validar broker activo contra `allowedBrokerIds`;
- resolver conexion por broker desde contexto autenticado;
- usar repositorio explicito, parametros y whitelists;
- no ejecutar `QueryStatic`, workflows, FormBuilder ni `doOperationData` heredado;
- normalizar fechas y zonas horarias;
- aplicar minimizacion de PII;
- devolver errores sanitizados con `correlationId` cuando aplique.

## Que no debe replicarse

No replicar:

- carga de menu runtime desde `Menu_GET_BY_APPLICATION_ID`;
- rutas dinamicas por `componentId`, `urlComponentStatic` o `urlRouteComponentStatic`;
- `FormBuilder` generico como runtime de Agenda;
- `NewRegister` generico como alta/edicion productiva;
- workflows `commandAdd` y `commandEdit` sin SDD propia;
- actualizacion de registros por drag-and-drop antes de permisos/UAT;
- SQL estructural heredado o metadatos como fuente de queries;
- scheduler/workflow interno de AppBuilder como si fuese la pagina Agenda.

## Riesgos

- Metadata insuficiente: no se puede saber con certeza la pantalla real.
- PII: `Asunto` y `Descripcion` son texto libre; pueden contener datos personales, siniestros, salud, telefonos, correos u observaciones sensibles.
- Multi-tenant: `BrokerIntegracionId` exige aislamiento por broker antes de consultar datos reales.
- Fechas: `Agenda` separa fecha y hora, mientras `vw_Agenda` expone `Start`/`End` como texto. Hay riesgo de conversion, zona horaria y eventos de dia completo.
- Escrituras: el control generico permite modificar eventos; en iLiniumTech esto no debe habilitarse sin permisos y UAT.
- Siniestros: el trigger `Tgr_AgendaSiniestroEvento` sugiere efectos secundarios entre siniestros y agenda. Cualquier escritura puede afectar procesos no documentados.
- Permisos: AppBuilder usa object groups historicos, pero no hay matriz concreta para Agenda.
- UX: sin metadata de componentes, una replica visual podria ser inventada.

## Pruebas necesarias para desarrollo futuro

Documental:

- validar este inventario con producto;
- localizar metadata concreta de Agenda mediante extractor offline autorizado;
- crear SDD de Agenda antes de programar.

Backend:

- tests de anonimo, sin broker, broker no permitido y permiso ausente;
- tests de rango de fechas;
- tests de normalizacion `F_Inicio`/`Hora_Inicio` y `F_Fin`/`Hora_Fin`;
- tests de parseo defensivo si se usa `vw_Agenda.Start`/`End`;
- tests de PII minimizada;
- tests de aislamiento por broker;
- tests de errores sanitizados y `correlationId`.

Frontend:

- tests de guard de sesion/permisos;
- tests de filtros/rango/vista en URL si aplica;
- tests de estados `loading`, `empty`, `error`, `forbidden`, `missing broker`;
- smoke visual de calendario en desktop y movil;
- pruebas de no mostrar acciones de escritura si el MVP es read-only.

Seguridad:

- secret scan;
- dependency audit si se introduce libreria de calendario nueva;
- CORS audit si se tocan endpoints/configuracion;
- revision de PII antes de UAT.

## Bloqueos

Bloqueos para desarrollo:

- falta metadata local concreta de menu/componente Agenda;
- falta confirmar si Agenda es una pantalla propia, un panel de inicio o un componente dentro de otra pagina;
- falta datasource real y mapeo de campos;
- falta catalogo de prioridad/tipo/estado;
- falta matriz de permisos;
- falta decision de si el primer MVP sera read-only o tendra escrituras;
- falta validacion DBA/UAT sobre `Agenda`, `vw_Agenda` y trigger de siniestros;
- falta auth productiva y permisos efectivos para cualquier uso real.

## Estado final

Clasificacion: `bloqueado externo`.

Evidencia parcial completada:

- Agenda esta en el menu iLiniumTech como entrada deshabilitada.
- AppBuilder contiene localizacion `Agenda/Diary`.
- AppBuilder contiene tabla `Agenda` y vista `vw_Agenda`.
- AppBuilder contiene un control generico `DynamicFullCalendar` compatible con eventos de calendario.
- AppBuilder contiene permisos historicos genericos por acciones.

No hay evidencia suficiente para programar la pagina sin inventar comportamiento. El siguiente paso correcto es obtener metadata concreta de Agenda con un extractor offline autorizado o validacion funcional de producto/DBA, y despues redactar una SDD de Agenda.
