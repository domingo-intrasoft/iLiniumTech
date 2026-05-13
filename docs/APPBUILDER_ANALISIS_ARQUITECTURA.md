# AppBuilder - Analisis tecnico y memoria para agentes

Fecha de elaboracion: 2026-05-13

Repositorio destino: `C:\Desarrollo\AiBuilder\iLiniumTech`

Fuente analizada: `C:\Desarrollo\AppBuilder`

Estado de esta fase: analisis de codigo fuente. No se ha ejecutado todavia la aplicacion, no se ha consultado la demo web y no se ha conectado contra las BBDD del entorno de pruebas desde este repositorio.

Decision posterior de producto: este documento describe como funciona AppBuilder, pero iLiniumTech no debe convertirse en un runtime dinamico equivalente. La metadata descrita aqui sirve para extraccion, migracion, trazabilidad y scaffolding inicial; el producto objetivo es frontend Vue estatico + backend API de datos.

## 1. Proposito de este documento

Este documento existe para que un agente de IA pueda continuar el proyecto iLiniumTech sin tener que volver a leer minuciosamente todo AppBuilder en cada intervencion.

El objetivo mayor del proyecto es transformar un sistema generado por AppBuilder en un flujo de desarrollo gobernado por especificaciones. La fase final esperada es que, a partir de la configuracion real almacenada en las BBDD de AppBuilder, se generen issues en formato SDD, y que el CI/CD de GitHub pueda desarrollar progresivamente una aplicacion MVP.

Antes de implementar codigo nuevo, los agentes deben aplicar tambien la guia de ingenieria importada desde AcademiaLasCortes y adaptada a iLiniumTech: `docs/engineering/README.md`.

La primera meta tecnica es mas pequena y verificable:

1. Entender como AppBuilder organiza aplicaciones, pantallas, componentes, fuentes de datos, formulas, flujos y conexiones.
2. Documentar el comportamiento suficiente para traducir un subconjunto a producto iLiniumTech.
3. En una fase posterior, leer el entorno de pruebas indicado por el usuario.
4. Extraer un componente real configurado en AppBuilder.
5. Levantar localmente una primera version Vue/API que represente ese componente con codigo explicito.

Regla critica: este repositorio no debe contener credenciales, passwords, cadenas de conexion completas, tokens, claves API ni dumps de BBDD. Si se necesita conectividad, se usaran variables de entorno, secret stores o ficheros locales ignorados por git.

## 2. Resumen ejecutivo

AppBuilder es una plataforma generadora de aplicaciones dinamicas. El comportamiento principal de cada aplicacion no esta codificado como pantallas estaticas, sino como metadata en BBDD.

Esa metadata describe:

- Aplicaciones, versiones y entornos.
- Menus, layouts y componentes visuales.
- Propiedades de cada componente.
- Eventos, permisos, traducciones y expresiones.
- Fuentes de datos asociadas a componentes.
- Campos, aliases, filtros, ordenaciones, lookups y configuraciones de CRUD.
- Procesos, colas, workflows y tareas.
- Llamadas dinamicas a BBDD, procedimientos, REST, SOAP/WSDL y Swagger/OpenAPI.

La aplicacion resultante se construye en tiempo de ejecucion:

1. El frontend Vue arranca una aplicacion generica.
2. Lee la configuracion de aplicacion y usuario.
3. Carga rutas y componentes desde el backend.
4. El backend GraphQL/REST consulta las BBDD Builder, Master, Model, Logs, Documentos, Motor y otras segun el tipo de conexion.
5. El frontend renderiza controles dinamicos con PrimeVue y componentes propios.
6. Los CRUD consultan y modifican datos mediante operaciones genericas Search y Data.
7. Las expresiones y workflows ejecutan reglas configuradas.

La idea para iLiniumTech no debe ser copiar AppBuilder ni reconstruir su runtime dinamico. El camino prudente para el MVP es extraer el minimo subconjunto de metadata necesario para una pantalla real y convertirlo en especificaciones, contratos y codigo revisado:

- Leer conexiones y metadata desde `IL_Maestro` y la BBDD de programa.
- Identificar aplicacion, version, entorno, menu y componente inicial.
- Extraer `IapComponent`, `IapDataSource`, enlaces componente-fuente, campos y configuraciones de campo.
- Disenar una ruta de lectura API propia y, despues, casos de uso de update solo si tienen SDD explicita.
- Construir pantallas Vue estaticas que reflejen el comportamiento decidido, no un renderer generico de metadata.
- Generar issues SDD pequenos, verificables y ligados a contratos de datos.

## 3. Mapa de codigo fuente

Raiz analizada:

`C:\Desarrollo\AppBuilder`

Estructura principal:

```text
C:\Desarrollo\AppBuilder
  src
    backend
      AppBuilder.Backend.sln
      Dominio
      Aplicacion
      Infraestructura
    frontend
      package.json
      Builder
      Ilinium
      Peris
      shared
      tools
```

El repositorio fuente `C:\Desarrollo\AppBuilder` esta en rama `master` y aparecia atrasado respecto a su remoto. No se ha modificado ni actualizado durante este analisis.

## 4. Arquitectura backend

Solucion principal:

`C:\Desarrollo\AppBuilder\src\backend\AppBuilder.Backend.sln`

La organizacion declarada por carpetas es:

- `Dominio`: entidades, contratos, constantes y modelos compartidos.
- `Aplicacion`: servicios de aplicacion, logica de negocio y casos de uso.
- `Infraestructura`: APIs, acceso a datos, helpers, publicacion, generacion y utilidades.

Proyectos centrales observados:

- `Dominio\AppBuilder.Dominio\AppBuilder.Dominio.csproj`
- `Aplicacion\AppBuilder.Aplicacion\AppBuilder.Aplicacion.csproj`
- `Infraestructura\Comun\AppBuilder.Comun\AppBuilder.Comun.csproj`
- `Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\AppBuilder.Infraestructura.DataAccess.csproj`
- `Infraestructura\Datos\AppBuilder.Infraestructura.Business\AppBuilder.Infraestructura.Business.csproj`
- `Infraestructura\Apis\Intrasoft.ApiBuilder\Intrasoft.ApiBuilder.csproj`
- `Infraestructura\Apis\Intrasoft.ApiBuilderRender\Intrasoft.ApiBuilderRender.csproj`
- `Infraestructura\Apis\Intrasoft.ApiAuth\Intrasoft.ApiAuth.csproj`
- `Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Intrasoft.ApiBuilderCommon.csproj`

Hay mezcla de target frameworks: se observaron proyectos en .NET moderno, incluidos `net10.0`, y tambien proyectos auxiliares o legacy en frameworks anteriores como `net8.0`, `net9.0`, `net48` y `net481`. Para cualquier compilacion futura conviene fijar primero que SDKs estan instalados y que proyectos son realmente necesarios para el MVP.

### 4.1 Entrypoints backend

`Intrasoft.ApiBuilder`

- API principal de AppBuilder.
- Expone GraphQL en `/apiBuilder`.
- Expone endpoints REST genericos.
- Integra MCP en `/mcp`.
- Usa persisted queries.
- Configura autenticacion multiple.
- Puede ejecutar migraciones o comprobaciones de BBDD al inicio.

`Intrasoft.ApiBuilderRender`

- API orientada al render o ejecucion de aplicaciones.
- Comparte gran parte del patron con ApiBuilder.
- Tiene una superficie GraphQL mas estrecha.

`Intrasoft.ApiAuth`

- API de autenticacion.
- Expone Swagger y ReDoc.
- Soporta JWT, autenticacion Microsoft, Google y flujos de usuario.

Tambien existen proyectos de herramientas y publicacion que no parecen formar parte del runtime minimo, pero son importantes para generacion estatica, exportaciones y despliegues.

### 4.2 Flujo backend general

El patron repetido es:

```text
Frontend Vue
  -> Apollo/PQL o REST
  -> ApiBuilder / ApiBuilderRender
  -> GraphQL RootQuery / RootMutation o controladores REST
  -> bll*
  -> Servicios de aplicacion
  -> Repositorios EF Core o Dapper
  -> BBDD dinamica segun usuario, aplicacion y tipo de conexion
```

Los controladores REST genericos heredan de una base comun y delegan en `bllApi.doOperation`. La superficie REST observada incluye:

- `GET api/schema`
- `POST api/list`
- `POST api/update`
- `POST api/add`
- `POST api/delete`

## 5. Arquitectura de datos

AppBuilder se apoya en varias BBDD. La BBDD maestra conoce conexiones hacia el resto, y las BBDD especificas contienen configuracion, modelos de negocio o datos auxiliares.

Tipos de BBDD observados en constantes:

- `BUILDER`: base de configuracion del programa AppBuilder.
- `MAESTRO`: base maestra con aplicaciones, entornos y conexiones.
- `MODELO`: base principal del modelo de negocio.
- `SOLICITUDES`: solicitudes o procesos auxiliares.
- `LOG`: logs.
- `DOCUMENTOS_MA` y `DOCUMENTOS_MO`: documentos asociados a maestro/modelo.
- `MOTOR`: motor de procesos.
- `EIAC`: modelo especifico adicional.

El usuario ha indicado que, para una fase posterior, el entorno de pruebas parte de `IL_Maestro` y que la BBDD de programa es `AunnaTechADM`. Las credenciales de ese entorno se gestionaran fuera del repositorio.

### 5.1 Contextos EF Core

Contextos relevantes:

- `AppBuilderDbContext`: contiene entidades `IAP_*`, asociadas al Builder.
- `AppMasterDbContext`: contiene entidades `IAPM_*`, asociadas al maestro nuevo.
- `AppModelDbContext`: contiene entidades `IAPMO_*`, asociadas al modelo.
- Contextos legacy como `MaestroDbContext`, `ModeloDbContext` y similares existen, pero algunos aparecen excluidos del proyecto principal actual.

El runtime moderno parece favorecer `AppMasterDbContext` y `AppModelDbContext` frente a los contextos legacy.

### 5.2 Dapper y SQL dinamico

Ademas de EF Core, el sistema usa Dapper para consultas dinamicas de alto rendimiento o de estructura configurable.

Piezas clave:

- `DapperContext`: crea conexiones Dapper.
- `Dapper\Shared\BaseRepository`: repositorio base parcial.
- `Dapper\AppBuilder\Repositorios\RepositorioSearch`: construye y ejecuta busquedas dinamicas.

Riesgo tecnico: aunque muchos valores se parametrizan, partes estructurales como nombres de tabla, select, where, order, set, procedimientos o fragments SQL se componen dinamicamente. Para iLiniumTech, cualquier extraccion o generacion de SQL debe tratar estos fragments como no confiables hasta validarlos contra metadata esperada.

### 5.3 Unidad de trabajo y facades

No hay una unidad de trabajo clasica pura. El patron observado es una fachada manual:

- `Main.cs` crea conexiones dinamicas segun usuario y tipo de BBDD.
- `MainEF.cs` instancia areas de servicio como Builder, Master, Model, Solicitudes, Document, Log, Engine y AI.

Este patron importa para el MVP porque el acceso a datos no se resuelve solo por una cadena de conexion global. Depende de usuario, aplicacion, entorno y tipo de BBDD solicitado.

## 6. Modelo de conexiones

Las conexiones no estan pensadas como valores fijos de configuracion por aplicacion web. AppBuilder las resuelve dinamicamente.

Piezas clave:

- `HelperUsuario.RetrieveUserConnection(...)`
- `ConnectionExtension`
- `HelperMaster`
- `HelperAppMaster`
- `HelperAppModel`
- `TipoBBDDConst`
- Entidades como `IapApplicationConnection` o `IapmConnection`

Flujo conceptual:

```text
Request autenticada
  -> contexto de usuario
  -> aplicacion / entorno / tipo BBDD
  -> lectura de conexion en maestro
  -> desencriptado de campos
  -> construccion de connection string
  -> DbContext o DapperContext especifico
```

La BBDD Builder puede salir de un estado global (`HelperMaster._connStringBuilder`) y otros modelos se resuelven desde maestro o desde metadata de aplicacion.

Para el MVP hay que evitar dos errores:

- No asumir una unica BBDD.
- No persistir en codigo las credenciales del entorno de pruebas.

## 7. Seguridad, autenticacion y secretos

### 7.1 Autenticacion

Builder y Render configuran autenticacion multiple mediante `ConfigureMultipleAuth`.

Modos observados:

- JWT Bearer.
- ApiKey.
- Microsoft Bearer.
- Google Bearer.
- `MultiAuth` como esquema compuesto.

La activacion depende de configuracion, especialmente `FeatureToggles:IsAuthJwt`.

Orden de middleware observado:

```text
UseAuthentication
middlewares opcionales de logs/encriptacion
persisted query middleware
UserContextMiddleware
UseAuthorization
endpoints GraphQL/REST/MCP
```

### 7.2 Encriptacion de conexiones

La encriptacion de campos de conexion se implementa con helpers como:

- `SecurityHelper`
- `bllEncrypt`
- `EncryptHelper`
- `EncryptionFunctions`

Se observo AES con PBKDF2 y salt fijo en alguna implementacion. Tambien se observo una caracteristica importante: algunos helpers devuelven el valor de entrada si el desencriptado falla. Eso permite coexistencia de valores cifrados y planos, pero reduce garantias de seguridad.

### 7.3 Hallazgos de secretos en fuente original

Durante el analisis se observaron credenciales en texto dentro del proyecto fuente AppBuilder, especialmente en scripts de scaffolding, publish settings o appsettings de herramientas.

No se han copiado esos secretos a este repositorio.

Recomendacion para fases futuras:

- Tratar esos ficheros como material sensible.
- No incluirlos en prompts, issues ni documentacion.
- Sustituirlos por variables de entorno o referencias a GitHub Secrets.
- Anadir escaneo de secretos al pipeline de iLiniumTech.

## 8. APIs y contratos expuestos

### 8.1 GraphQL

ApiBuilder expone GraphQL en:

`/apiBuilder`

Root queries observadas:

- Catalog
- Application
- Component
- Menu
- ObjectGroup
- Search
- DataSource
- WorkFlow
- Expression
- Code
- Master
- Document
- Chat
- Model
- Api
- AI
- Engine

Mutations observadas:

- Data CRUD.
- Catalogos.
- Application.
- Component.
- Menu.
- ObjectGroup.
- Localization.
- Workflow.
- DataSource.
- Expression.
- Help.
- Code.
- Master, usuario, mail, SMS, import.
- Cache.
- Document.
- Encrypt.
- Process.

ApiBuilderRender replica parte de esta superficie, pero mas enfocada a render y uso.

### 8.2 Persisted queries

El frontend no depende solo de queries GraphQL libres. Hay una pipeline de persisted queries:

```text
gql en codigo fuente
  -> extraccion a .graphql
  -> generacion de manifest
  -> generacion de mapa TypeScript PQL
  -> copia de persisted-query-manifest.json al backend
  -> llamadas por hash desde Apollo
```

Esto es importante porque un backend sin manifest correcto puede fallar o rechazar operaciones. Para un MVP independiente se puede empezar con GraphQL normal o REST, pero si se reutiliza ApiBuilder real hay que respetar el manifest.

### 8.3 REST generico

Endpoints REST genericos:

- `api/schema`
- `api/list`
- `api/update`
- `api/add`
- `api/delete`

Estos endpoints funcionan como una fachada generica hacia operaciones de AppBuilder.

### 8.4 MCP

ApiBuilder y Render integran MCP en `/mcp`.

Herramientas observadas conceptualmente:

- Echo.
- Definition.
- List.
- Update.
- Add.
- Delete.

Tambien hay un proyecto MCP separado con endpoints especificos de presupuestos. Se observaron conexiones hardcoded en ese proyecto fuente, por lo que cualquier reutilizacion requiere redaccion previa.

## 9. Arquitectura frontend

Raiz frontend:

`C:\Desarrollo\AppBuilder\src\frontend`

Monorepo npm con workspaces:

- `shared`
- `builder`
- `ilinium`
- `peris`

Tecnologias:

- Vue 3.
- Vue CLI.
- TypeScript.
- PrimeVue.
- Vue Router.
- Vuex.
- Apollo Client.
- Inversify.
- Libreria interna `@ilinium/shared`.

Scripts relevantes:

- `extract:operations`
- `generate:manifest`
- `generate:pql:map`
- `generate:pql`
- `serve` por workspace, por ejemplo `builder`
- variantes de serve/build para clientes concretos

### 9.1 Entrada de Builder

Archivos clave:

- `Builder\src\main.ts`
- `Builder\src\AppWrapper.vue`
- `Builder\src\infrastructure\container.ts`
- `shared\src\infrastructure\apollo\HelperApolloClient.ts`
- `shared\src\graphql\pql.ts`

El arranque crea el contenedor, configura Apollo, reconstruye rutas segun login/configuracion y renderiza la aplicacion con wrappers genericos.

### 9.2 Render dinamico

Piezas clave:

- `FormBuilder.vue`
- `ComponentRender.ts`
- `ComponentRenderAux.ts`
- `ComponentRenderHelper.ts`
- controles `Dynamic*`
- editores PrimeVue bajo `shared`

El frontend no conoce todas las pantallas de antemano. Carga metadata de componentes, atributos, eventos, expresiones, permisos y fuentes de datos. Luego elige controles dinamicos segun `idType`, catalogos y configuracion del componente.

`FormBuilder` es el contenedor central:

- Recibe aplicacion, version y componente.
- Carga arbol de componentes.
- Gestiona operaciones pendientes.
- Ejecuta eventos.
- Controla formularios embebidos o modales.
- Decide si usa un componente estatico local o render runtime.

### 9.3 Estado cliente

Estado y servicios:

- Vuex con modulos de autenticacion y configuracion.
- Persistencia con SecureLS.
- Apollo para GraphQL.
- IndexedDB/cache para algunas queries cacheables.
- Inversify para resolver servicios y repositorios.

## 10. Modelo dinamico de UI y CRUD

La metadata principal gira alrededor de componentes y fuentes de datos.

Entidades conceptuales:

- `IapComponent`: define componente visual, propiedades, eventos, permisos, expresiones y workflows.
- `IapDataSource`: define fuente de datos, campos, aliases, lookups y configuracion de servicio.
- `IapComponentDataSource`: vincula componente con fuente; define autoload, autosave, queryWhere, queryOrder, queryGroup, queryHaving, max rows y otras opciones.
- `IapComponentDataSourceFieldConfiguration`: define comportamiento UX por campo: visible en lista, visible en edicion, obligatorio, filtro, orden, agregado, valor por defecto y parametros de lookup.

Flujo de carga de una pantalla:

```text
Ruta / componente inicial
  -> FormBuilder
  -> Component_GET_COMPONENT_FILE
  -> lista/arbol de IapComponent
  -> atributos, eventos, expresiones, workflows y datasources
  -> ComponentRenderAux elige controles Dynamic*
  -> controles renderizados con PrimeVue/componentes propios
```

Flujo CRUD de lectura:

```text
DynamicCrudTabla
  -> CrudTable
  -> seleccion de IapComponentDataSource
  -> construccion de filtros/lazy params
  -> Search_SEARCH
  -> bllSearch
  -> ServicioSearch
  -> RepositorioSearch
  -> SQL/Dapper o servicio externo
  -> SearchDetail muestra resultados
```

Flujo CRUD de escritura:

```text
SearchDetail o formulario custom
  -> Data_UPDATE
  -> bllData
  -> servicio de datos
  -> repositorio
  -> insert/update/delete sobre modelo configurado
```

Lookups:

```text
LookUpEditor / DynamicSearchControl
  -> Search_SEARCHLOOKUP
  -> fuente configurada
  -> resultados filtrados para selector
```

Operaciones PQL importantes:

- `Component_GET_COMPONENT_FILE`
- `ComponentDataSource_GET_ALL_BY_COMPONENTID`
- `ComponentDataSource_GET_BY_COMPONENTLIST`
- `DataSource_GETALL`
- `DataSource_GETBYID`
- `DataSource_GET_CUSTOM`
- `Search_SEARCH`
- `Search_SEARCH_MULTIPLE`
- `Search_SEARCHLOOKUP`
- `Search_PREVIEW_DATA`
- `Data_UPDATE`
- `SearchConfig_*`

Para el MVP, el contrato minimo debe poder representar:

- Componente raiz.
- Hijos ordenados.
- Tipo de control.
- Atributos necesarios para render.
- Datasource asociado.
- Campos de datasource.
- Configuracion visible/editable/obligatoria.
- Filtros iniciales.
- Resultado tabular.

## 11. Formulas y expresiones

AppBuilder tiene un sistema de expresiones usado para calculos, valores dinamicos, condiciones, visibilidad, filtros y workflows.

Backend:

- `ExpressionEngine.cs`
- Usa NCalc.
- Registra parametros desde diccionarios.
- Registra funciones mediante `HelperFunctionEngine`.
- Ejecuta detalles de expresion ordenados por `ProcessOrder`.
- Guarda resultados en variables reutilizables por expresiones posteriores.

Frontend:

- `expressionEngine.ts`
- Usa `ncalcjs`.
- Replica parte del comportamiento para calculos en cliente.
- Trabaja con pares tipo `{ key, value }`.

Modelo conceptual:

```text
Expression
  -> detalles ordenados
  -> cada detalle calcula una variable
  -> variables previas alimentan calculos posteriores
  -> resultado se aplica a atributo, filtro, control, workflow o dato
```

Implicacion para iLiniumTech:

No conviene implementar todo el motor de formulas al principio. Para el primer MVP hay que detectar si el componente elegido depende de expresiones. Si depende, implementar solo las funciones necesarias o fijar valores calculados durante la extraccion.

## 12. Workflows, procesos y motor

El motor de workflows permite ejecutar acciones anidadas y procesos configurables.

Piezas principales:

- `WorkflowExecutor.cs`
- `RegisterNodeHandlers`
- `EntityEngineBusiness.cs`
- `TaskExecutor.cs`
- Entidades `IapenProcess`, `IapenProcessEntity`, `IapenProcessCommand`
- Tareas y ejecuciones `IapenTaskExecution`, `IapenTaskExecutionNode`

Workflow conceptual:

```text
Proceso o comando
  -> localiza workflow general o especifico de entidad
  -> carga nodos, controles, variables y conexiones
  -> inyecta expresiones en controles de nodos
  -> encuentra start/end
  -> procesa nodos segun dependencias
  -> cada nodo produce ExecutionResult
  -> el resultado decide la siguiente conexion
  -> registra entradas, salidas y estado
```

Tipos de nodos o acciones observadas:

- Cache.
- Cambio de componente.
- Condicion.
- DataSource.
- Delay.
- Mostrar formulario.
- Enviar mail.
- Enviar SMS.
- Procedimiento.
- Guardar.
- Set value.
- Switch.
- Texto.
- Wallet.
- Script.

Procesos:

- `ProcessMutation.init` puede encolar inicializaciones.
- Add/update/delete de proceso encolan operaciones de motor.
- Delete elimina workflows y expresiones relacionados.
- `EntityEngineBusiness` inicializa tareas y procesa colas.
- `TaskExecutor` acaba delegando en `WorkflowExecutor`.

Observacion tecnica importante: se observo una llamada final a `CallCustomCommandProcess(ProccesCommand.Before)` al cierre de `ExecuteWorkflowAsync`. Por nombre parece posible que deberia ejecutarse un `After`, pero este documento no cambia el comportamiento; solo deja constancia para futura revision.

Para el MVP se recomienda dejar workflows fuera salvo que el componente elegido los necesite para cargar datos o validar acciones.

## 13. Servicios externos REST, SOAP y Swagger/OpenAPI

AppBuilder puede consumir servicios externos configurados como fuentes de datos.

Frontend:

- Servicios de DataSourceService.
- Repositorios Apollo.
- Operaciones para WSDL, Swagger, SOAP y REST.

Backend:

- `DataSourceServiceMutation`
- `RepositorioService`
- `ServicioSearch`

Capacidades observadas:

- Leer WSDL.
- Parsear namespaces, metodos, mensajes y bindings SOAP.
- Ejecutar llamadas SOAP con SOAPAction.
- Leer Swagger/OpenAPI desde URL o fichero base64.
- Detectar servers, paths, operaciones y schemas.
- Construir llamadas REST con URL, metodo, token bearer, parametros y cuerpo JSON.
- Registrar request/response.

En el camino dinamico, `ServicioSearch` puede convertir campos planos en JSON, resolver configuracion por entorno y llamar al servicio configurado como si fuese una datasource.

Implicacion para MVP:

Primero priorizar datasource SQL. Solo incluir REST/SOAP si el componente real elegido depende de ello.

## 14. Herramientas de generacion, publicacion y datos estaticos

Ademas del runtime, el backend contiene herramientas para:

- Generacion de modelos EF desde BBDD.
- Publicacion de aplicaciones.
- Generacion de datos estaticos.
- Exportacion a SQLite.
- Copias o transformaciones de datos SQL Server.

Piezas observadas:

- `DataAccess.GenerateModel`
- `GenerateStaticData`
- `PublishApp`
- `BootSharp`
- utilidades `BllSqlLite`, `BllSqlServer`, `SqlServerToSQLite`

`GenerateStaticData` puede generar datos compartidos y BBDD local. Esto podria ser util en el futuro si el MVP busca un modo offline o una muestra local reproducible.

Atencion: algunas herramientas de publicacion y scaffolding contienen secretos en el proyecto fuente original. No deben copiarse.

## 15. Observabilidad, logs y auditoria

El sistema contiene BBDD y servicios asociados a logs, documentacion y engine. Tambien hay middleware opcional de logs/encriptacion y registro de ejecuciones de workflows.

Puntos importantes:

- Las ejecuciones de workflow registran nodos, entradas, salidas y estados.
- Las llamadas REST/SOAP pueden registrar request/response.
- Hay BBDD `LOG` como tipo de conexion.
- Existen herramientas y servicios de error reporting o envio de mail en configuracion.

Para iLiniumTech, los issues SDD deberian exigir trazabilidad minima:

- ID de componente.
- ID de datasource.
- SQL o servicio resuelto, redaccion de secretos incluida.
- Resultado de carga.
- Errores de validacion de metadata.

## 16. Riesgos y decisiones tecnicas detectadas

Riesgos:

- Secretos en el repositorio fuente original.
- Mezcla de target frameworks.
- Dependencias externas o rutas locales.
- SQL dinamico.
- CORS permisivo en configuraciones observadas.
- Estado global para algunas conexiones.
- Persisted query manifest necesario para backend real.
- Helpers de desencriptado con fallback silencioso.
- Workflows y expresiones pueden esconder comportamiento no evidente en una pantalla.
- Algunos catch devuelven null o degradan errores, complicando diagnostico.

Decisiones recomendadas para iLiniumTech:

- Empezar por lectura controlada, no por migracion completa.
- Guardar contratos extraidos en JSON sanitizado, no dumps de BBDD completos.
- Elegir un componente real con dependencias simples.
- Generar issues SDD pequenos.
- Mantener la extraccion de metadata fuera del runtime productivo.
- Implementar frontend Vue estatico y backend API de datos con contratos explicitos.
- Mantener un documento de trazabilidad entre metadata AppBuilder y artefactos iLiniumTech.

## 17. Plan recomendado para la siguiente fase

### Fase 2.1: Preparar acceso seguro

- Crear un mecanismo local ignorado por git para la conexion al entorno de pruebas.
- Validar conectividad a la BBDD maestra indicada por el usuario sin imprimir secretos.
- Confirmar que la BBDD de programa es `AunnaTechADM`.
- Listar aplicaciones, versiones y entornos disponibles.

### Fase 2.2: Inventario de metadata

Extraer, de forma solo lectura:

- Aplicaciones.
- Versiones.
- Menus.
- Componentes raiz.
- Componentes hijos.
- Datasources.
- Campos.
- Enlaces componente-datasource.
- Configuraciones por campo.
- Expresiones asociadas.
- Workflows asociados.

Salida esperada:

- JSON sanitizado por aplicacion/componente.
- Informe de dependencias.
- Candidato recomendado para MVP.

### Fase 2.3: Seleccion de componente MVP

Criterios para elegir componente:

- Tiene datasource SQL claro.
- No depende de workflows complejos.
- Usa controles ya mapeables.
- Puede cargarse con una query Search simple.
- Permite comprobar visualmente tabla, formulario o detalle.

### Fase 2.4: Primer producto local Vue/API

Salida recomendada:

1. Pantalla Vue estatica para el componente elegido.
2. Backend API read-only con DTOs y validacion propia.
3. Repositorio in-memory primero y SQL seguro despues.
4. Trazabilidad AppBuilder visible en docs o endpoint diagnostico, no como motor de UI.

No se recomienda reutilizar el frontend AppBuilder ni montar un proxy contra ApiBuilder real para el producto iLiniumTech. Pueden servir como referencia de analisis o UAT, pero no como arquitectura final.

### Fase 2.5: Generacion de issues SDD

Crear issues por capas:

- Conexion segura y lectura de maestro.
- Extractor offline de metadata de aplicacion.
- Extractor de componentes.
- Extractor de datasource/campos.
- Contrato JSON sanitizado.
- Pantalla Vue estatica de la funcionalidad elegida.
- API read-only de datos.
- Repositorio SQL parametrizado con whitelist.
- Casos de uso de update solo con spec propia.
- Pruebas de componente real.

## 18. Plantilla SDD recomendada

Cada issue generado debe tener esta estructura:

```md
# SDD: <titulo>

## Contexto
Que comportamiento de AppBuilder se quiere reproducir y que metadata lo justifica.

## Objetivo
Resultado observable que debe existir al cerrar el issue.

## Alcance
Incluido y excluido.

## Contrato de datos
Entradas, salidas, campos obligatorios y ejemplos sanitizados.

## Criterios de aceptacion
- Dado...
- Cuando...
- Entonces...

## Notas tecnicas
Rutas de AppBuilder relacionadas, tablas/entidades conceptuales y decisiones de implementacion.

## Pruebas
Unitarias, integracion o comprobacion manual esperada.

## Seguridad
Confirmar que no se guardan secretos ni datos sensibles.
```

## 19. Tabla de trazabilidad inicial

| Area | Codigo fuente AppBuilder | Concepto que debe conocer iLiniumTech |
| --- | --- | --- |
| API principal | `Intrasoft.ApiBuilder` | GraphQL, REST generico, MCP, auth |
| API render | `Intrasoft.ApiBuilderRender` | Runtime de render y ejecucion |
| Auth | `Intrasoft.ApiAuth` | Login, JWT, proveedores externos |
| Data access | `AppBuilder.Infraestructura.DataAccess` | EF Core, Dapper, contexts |
| Business | `AppBuilder.Infraestructura.Business` | bll*, servicios, unidad manual |
| Frontend builder | `src\frontend\Builder` | Entrada Vue y wrapper |
| Frontend shared | `src\frontend\shared` | Controles, repositorios, Apollo, render |
| Componentes | Entidades `IapComponent*` | Metadata visual |
| Datasources | Entidades `IapDataSource*` | Origen de datos y campos |
| CRUD | `CrudTable`, `SearchDetail`, `Search`, `Data` | Listado, edicion, borrado |
| Expresiones | `ExpressionEngine`, `expressionEngine.ts` | Formulas y calculos |
| Workflows | `WorkflowExecutor`, engine | Flujos y procesos |
| Servicios | `RepositorioService`, `DataSourceServiceMutation` | REST, SOAP, OpenAPI |
| Publicacion | `PublishApp`, `GenerateStaticData` | Generacion y despliegue |

## 20. Comandos orientativos para desarrollo futuro

Estos comandos son orientativos y deben validarse con las versiones reales instaladas. No incluyen credenciales.

Frontend Builder:

```powershell
cd C:\Desarrollo\AppBuilder\src\frontend
npm install
npm run serve --workspace=builder
```

Generacion PQL:

```powershell
cd C:\Desarrollo\AppBuilder\src\frontend
npm run generate:pql
```

Backend:

```powershell
cd C:\Desarrollo\AppBuilder\src\backend
dotnet restore .\AppBuilder.Backend.sln
dotnet build .\AppBuilder.Backend.sln
```

Antes de ejecutar ApiBuilder real hay que revisar configuracion local, persisted query manifest, SDK .NET, conexiones y secretos.

## 21. Glosario operativo

AppBuilder:
Generador propio de Intrasoft basado en metadata de BBDD.

Builder:
BBDD o API que contiene la configuracion del programa dinamico.

Maestro:
BBDD que conoce aplicaciones, entornos y conexiones hacia otras BBDD.

Modelo:
BBDD principal de negocio de una aplicacion concreta.

Datasource:
Definicion de origen de datos: tabla, vista, procedimiento, REST, SOAP u otro.

Component:
Elemento visual o contenedor renderizable.

ComponentDataSource:
Vinculo entre componente y datasource.

FieldConfiguration:
Reglas UX y CRUD por campo.

Expression:
Formula dinamica evaluada en backend o frontend.

Workflow:
Flujo de nodos que ejecuta acciones, condiciones y procesos.

PQL:
Persisted Query Layer usado por el frontend para llamar GraphQL mediante hashes/manifiesto.

SDD:
Spec Driven Development. Issue con especificacion verificable para que CI/CD o agentes implementen comportamiento.

## 22. Reglas para agentes futuros

- Leer este documento antes de analizar de nuevo `C:\Desarrollo\AppBuilder`.
- No copiar secretos desde AppBuilder ni desde configuraciones locales.
- Si se necesita una cadena de conexion, pedirla o leerla desde un mecanismo local ignorado por git.
- Mantener el primer MVP pequeno: un componente real, una datasource, una ruta Search, una comprobacion visual o automatizada.
- Documentar cada inferencia relevante con la ruta de codigo o tabla conceptual que la justifica.
- Generar issues SDD solo cuando el contrato de datos este claro.
- Evitar reimplementar workflows, expresiones y servicios externos salvo que el componente MVP lo exija.
