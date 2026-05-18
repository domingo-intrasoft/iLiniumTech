# Pagina AppBuilder: Suplementos

Fecha: 2026-05-16

Rol: jefe de pagina `Suplementos`.

Ronda: solo documentacion. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se consulta configuracion sensible.

## Regla de migracion

`Suplementos` no debe implementarse como runtime dinamico AppBuilder. La evidencia heredada se usa para analisis, trazabilidad, futura SDD y scaffolding revisado. El resultado final, si se desbloquea, debe ser una pantalla Vue/TypeScript estatica y una API .NET explicita con contratos, permisos y consultas propias.

## Estado ejecutivo

Estado final de esta ronda: `bloqueado externo`.

Motivo: hay evidencia local suficiente para confirmar que `Suplementos` existe como concepto de menu, busqueda y dominio de datos, pero no hay evidencia local suficiente de la metadata concreta de pagina: no se ha encontrado un componente raiz AppBuilder identificado, ni `componentId`, ni ruta, ni datasource concreto, ni tabs/submenus propios de la pantalla. Sin esa metadata o validacion funcional no se debe iniciar desarrollo.

Actualizacion 2026-05-18: iLiniumTech ya contiene ruta protegida `/suplementos` y vista Vue fixture/read-only con datos sanitizados, filtros locales, paginacion y acciones de detalle/exportacion bloqueadas. Esa vista no autoriza API, datos reales, detalle, importes, banco, documentos, workflows, adjuntos ni escritura sin SDD, permisos, DBA/UAT y revision de seguridad.

SDD draft abierta para el primer corte futuro: [`SDD-2026-013 Suplementos read-only minimizado`](../../../sdd/specs/iLiniumTech/SDD-2026-013-suplementos-read-only.md). La SDD limita el primer incremento a listado read-only minimizado y deja detalle, tabs por tipo, PII, importes, banco, documentos, recibos/declaraciones relacionados, exportacion, workflows y escrituras fuera de alcance.

Decision de subagentes de componentes: no se crean subagentes. No se detectaron pestanas, submenus o componentes internos reales y especificos de `Suplementos`; solo se detectaron componentes genericos AppBuilder (`Search`, `SearchDetail`, `DynamicCrudTabla`, `DynamicTabView`, etc.) y tablas de especializacion de datos, que no prueban por si solas que existan tabs/paneles de la pagina.

## Fuentes revisadas

### Gobierno iLiniumTech

- `AGENTS.md`.
- `PLANS.md`.
- `README.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/appbuilder/pages/README.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.

### AppBuilder frontend

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenu.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppSubMenu.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\templates\prime\apollo\layout\AppMenuItem.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\ObjectGroupConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\FiltroBusquedaConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\MessagesBusqueda.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\functions\searchTypeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builderMaster\user\domain\const\SearchConfigTypeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\domain\iapMenu.ts`.

### AppBuilder backend/modelo

- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Eiac\EiacDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\MaestroDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\Modelo\Repositorios\RepositorioBusqueda.cs`.

### Documentacion relacionada

- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md` solo para confirmar que documentos, recibos y suplementos estaban fuera de ese alcance.

## Busquedas ejecutadas

Todas las busquedas fueron locales y de solo lectura.

- `rg --files C:\Desarrollo\AppBuilder ... | rg -i "suplement|supplement"`.
- `rg -n -i "suplement|supplement" C:\Desarrollo\AppBuilder ...`.
- `rg -n -i "suplement|supplement" docs iLiniumTech.Frontend iLiniumTech.Backend tools ...`.
- `rg -n -i "Suplementos|Supplements|GestionConst\.SUPLEMENTO|SUPLEMENTO = 'SIP'|sup:'Suplementos'" C:\Desarrollo\AppBuilder\src ...`.
- `rg -n "modelBuilder\.Entity<Suplemento|modelBuilder\.Entity<PbiSuplemento|modelBuilder\.Entity<Vw.*Suplemento" ...\ModeloDbContext.cs`.
- `rg -n "modelBuilder\.Entity<Suplemento|modelBuilder\.Entity<Eiacsuplemento|modelBuilder\.Entity<VwAduanaEiacsuplemento|modelBuilder\.Entity<.*Suplemento" ...\EiacDbContext.cs`.

Exclusiones aplicadas: `bin`, `obj`, `.vs`, `node_modules`, `dist`, `coverage`, `appsettings*`, `*.config`, rutas con `connection`, dumps, backups, imagenes, PDFs y salidas sensibles. No se abrieron connection strings ni appsettings.

## Evidencia encontrada

### Menu iLiniumTech actual

En `iLiniumTech.Frontend/src/layout/appNavigation.ts` existe una entrada:

- label: `Suplementos`.
- icono: `pi pi-link`.
- estado actual iLiniumTech: `fixture`.
- ruta: `/suplementos`.
- sin permiso `requiredPermission`.
- sin hijos.

Interpretacion: iLiniumTech reconoce `Suplementos` como entrada de dominio y ya tiene superficie fixture/read-only para navegacion y pruebas. No existe aun contrato funcional con datos reales, API propia, permiso productivo ni submenu definido.

### Menu AppBuilder heredado

En `Messages.ts` se observa la clave de localizacion:

- `gestion.sup`: `Suplementos` en espanol.
- `gestion.sup`: `Supplements` en ingles.

En el shell AppBuilder:

- `AppMenu.vue` toma los menus desde `store.getters.getApplicationMenus`.
- `AppSubMenu.vue` renderiza la coleccion ordenada.
- `AppMenuItem.vue` soporta item con `items`, `to`, `url`, `visible`, `disabled`, `command` y clases.
- `IapMenu` define menu por metadata con `componentId`, `urlComponentStatic`, `urlRouteComponentStatic`, `parentId`, `objectGroups`, `groups`, `component` e `inverseParent`.

Interpretacion: AppBuilder probablemente obtiene la estructura real de `Suplementos` desde metadata en BBDD. En codigo local no aparece el registro de menu concreto con `componentId` o hijos.

### Busqueda generica AppBuilder

En `GestionConst.ts`:

- `SUPLEMENTO = 'SIP'`.

En `TableIcons.ts`:

- `GestionConst.SUPLEMENTO` apunta a tabla `suplemento` e icono `pi pi-calendar-plus`.

En `MessagesBusqueda.ts`:

- `bar.sup`: `Suplementos` / `Supplements`.

En `Search.vue`:

- modos de busqueda: `Avanzada`, `Simple` y `General` si existe `searchComponentId`.
- botones: buscar, limpiar filtros, cerrar pestanas, guardar busqueda.
- soporte de listas, columnas guardadas y ultimas busquedas mediante `SearchConfig`.
- filtros generados por datasource/componente, no por codigo estatico.

En `SearchDetail.vue`:

- tabla de resultados con paginacion, ordenacion multiple, filtros de columna, busqueda global, selector de columnas, exportacion, refrescar, nuevo registro, importar, actualizacion masiva, guardar busqueda y acciones por fila.
- acciones condicionadas por permisos historicos `ObjectGroupConst`.

Interpretacion: `Suplementos` podria haberse mostrado dentro del CRUD/buscador generico, pero no se encontro metadata local que indique que campos, columnas o acciones concretas usaba.

### Permisos historicos genericos

En `ObjectGroupConst.ts` se detectan permisos historicos genericos:

- `add`.
- `edit`.
- `list` como detalle/listado.
- `delete`.
- `view`.
- `import`.
- `export`.
- `execute`.

En componentes dinamicos AppBuilder se aplican para mostrar u ocultar controles, acciones, exportacion, detalle y edicion. No se encontro una matriz concreta de permisos para la pagina `Suplementos`.

### Datos de modelo detectados

En `ModeloDbContext.cs` se detectan entidades principales:

- `Suplemento`.
- `PbiSuplemento`.
- `VwAduanaSuplemento`.
- `VwClienteSuplemento`.
- `VwCorreoSuplemento`.
- `VwSuplementosDatosAdicionale`.

Tambien se detectan tablas uno-a-uno especializadas por `SuplementoId`:

- `SuplementoAltaBajaRiesgo`.
- `SuplementoAnulacionPoliza`.
- `SuplementoAportacionExtraordinaria`.
- `SuplementoBaseCalculo`.
- `SuplementoBeneficiario`.
- `SuplementoBonusMalus`.
- `SuplementoCertificadoDuplicado`.
- `SuplementoDatosTomador`.
- `SuplementoDecenal`.
- `SuplementoDefensaCartera`.
- `SuplementoDomiciliacionBancaria`.
- `SuplementoFraccionPago`.
- `SuplementoGarantia`.
- `SuplementoGestorCobro`.
- `SuplementoProrrogaDuracion`.
- `SuplementoRegularizacion`.
- `SuplementoRenovacion`.
- `SuplementoRescate`.
- `SuplementoSegundaCapa`.
- `SuplementoSuspension`.
- `SuplementoTomador`.

Relaciones del suplemento base:

- `Suplemento` pertenece a `Poliza` por `PolizaId`.
- `Suplemento` pertenece a `BrokerIntegracion` por `BrokerIntegracionId`.
- `Suplemento` tiene relacion muchos-a-muchos con `Recibo` mediante `SuplementoRecibo`.
- `Suplemento` tiene relacion muchos-a-muchos con `RegularizacionDeclaracion` mediante `SuplementoDeclaracion`.
- La tabla base tiene trigger `trg_SuplementoLog`.

Campos base observados en mapping:

- identificacion y enlace: `Id`, `PolizaId`, `BrokerIntegracionId`, `IdOld`.
- clasificacion: `IdTipo`, `IdSituacion`, `IdTipoComunicacion`, `IdDivisa`, `Aplicacion`.
- referencias: `Referencia`, `ReferenciaCia`.
- fechas: `F_Efecto`, `FCR`, `FUM`.
- importes/textos: `Concepto`, `Valor`, `ValorAnterior`.
- comunicacion: `EmailComunicacion`.
- auditoria: `UCR`, `UUM`.

En `EiacDbContext.cs` se detecta `EIACSuplemento`, con:

- relacion con `EIACPoliza`.
- trigger `Tgr_EIACSuplementoCambios`.
- campos de situacion, tipo, numero de suplemento, descripcion, fechas de efecto/emision/vencimiento/situacion/validacion/rechazo, usuarios de validacion/rechazo.
- vistas de aduana EIAC relacionadas con incidencias, pendientes de crear, revisar conciliacion y sin conciliar.

### Repositorio de busqueda legacy

`RepositorioBusqueda.cs` contiene una consulta legacy orientativa que une `Poliza`, `Identidad`, `Recibo`, `RiesgoPoliza`, `Riesgo`, `Siniestro` y `Suplemento`. Esa consulta:

- parece antigua o incompleta;
- usa composicion estructural de SQL;
- no debe copiarse al producto iLiniumTech;
- solo sirve como pista de relaciones funcionales alrededor de poliza/suplemento.

## Evidencia ausente

No se ha encontrado localmente:

- componente raiz AppBuilder de `Suplementos`;
- `componentId` del menu `Suplementos`;
- registro de `IapMenu` concreto para `Suplementos`;
- ruta historica concreta;
- datasource concreto de la pagina;
- columnas reales del listado de `Suplementos`;
- filtros reales por defecto de la pagina;
- detalle real de suplemento;
- tabs o submenus especificos de `Suplementos`;
- acciones historicas especificas de suplemento;
- reglas de visibilidad por broker/perfil/oficina/usuario;
- permisos historicos concretos por grupo;
- workflows o expresiones especificos de suplemento;
- UAT funcional o propietario de negocio.

## Componente raiz e hijos

### Componente raiz candidato

No confirmado.

Candidato plausible, no implementable aun: pantalla de busqueda/listado `Suplementos` basada en el CRUD generico AppBuilder, porque existen `GestionConst.SUPLEMENTO`, `TableIcons`, localizacion de busqueda y entidad `Suplemento`.

Bloqueo: falta el registro de metadata que vincule la entrada de menu con el componente y datasource.

### Componentes hijos candidatos

No confirmados.

Las tablas especializadas por tipo de suplemento podrian terminar como secciones de detalle, paneles condicionales o tabs segun `IdTipo`, pero no hay evidencia local de que AppBuilder las presentara como tabs/submenus. No se crean documentos en `components/*.md` para no inventar estructura.

## Pestanas y submenus

No se detectaron pestanas o submenus reales de `Suplementos`.

Senales genericas:

- AppBuilder soporta menus jerarquicos (`IapMenu.parentId`, `inverseParent`, `items` en render).
- AppBuilder soporta tabs dinamicos (`DynamicTabView`, `DynamicTabPanel`, `DynamicTabPanels`, `DynamicTabList`).
- El CRUD generico soporta detalle y formularios relacionados.

Pero no hay prueba de que `Suplementos` usara una pestana concreta ni de cuales serian sus hijos.

## Datos y catalogos candidatos

### Datos base candidatos

Para un futuro MVP read-only de `Suplementos`, el contrato minimo candidato deberia salir de `Suplemento` y enlazar con `Poliza`:

- identificador interno del suplemento;
- numero/referencia del suplemento, si se confirma campo funcional;
- poliza asociada;
- referencia de compania;
- tipo de suplemento;
- situacion;
- fecha de efecto;
- concepto;
- valor anterior/nuevo o resumen de cambio, con minimizacion;
- broker efectivo;
- indicadores de relacion con recibos/declaraciones, si aportan valor funcional.

### Datos sensibles

Campos con riesgo alto de PII o dato financiero:

- `EmailComunicacion`.
- `SuplementoDatosTomador.Email`.
- `SuplementoDatosTomador.NumDocumento`.
- `SuplementoDatosTomador.Nombre`, apellidos, direccion, telefonos y movil.
- `SuplementoBeneficiario.CIF`, razon social, domicilio y datos de prestamo.
- `SuplementoDomiciliacionBancaria.IBAN`, titular y documento.
- importes y primas en tablas de calculo, defensa de cartera, rescate, suspension, segunda capa o aportacion extraordinaria.

Regla para iLiniumTech: estos campos no deben aparecer en listado ni detalle por defecto sin permiso explicito, clasificacion PII y UAT. Si se necesitan, deben ir en DTOs minimizados o en secciones protegidas.

### Catalogos candidatos

No se han confirmado catalogos concretos. Por nombres de campo, probablemente haran falta catalogos para:

- tipo de suplemento (`IdTipo`);
- situacion (`IdSituacion`);
- tipo de comunicacion (`IdTipoComunicacion`);
- divisa (`IdDivisa`);
- motivo de anulacion;
- forma/fraccion de pago;
- base de calculo;
- concepto;
- periodicidad;
- tipo de prima;
- tipo de tasa;
- tipo de documento;
- provincia/localidad/codigo postal;
- gestor/canal de cobro;
- tipo de rescate.

Bloqueo: hay que localizar metadata real de lookups/catalogos o validarlo con BBDD/UAT antes de implementarlo.

## Filtros candidatos

No se detectaron filtros reales por defecto. Para SDD futura, filtros candidatos razonables, pendientes de confirmar:

- poliza o referencia de poliza;
- referencia del suplemento;
- referencia de compania;
- tipo de suplemento;
- situacion;
- fecha de efecto desde/hasta;
- concepto o texto libre;
- broker activo, siempre derivado de sesion/backend y nunca de input libre;
- cliente/tomador solo si el permiso y minimizacion PII lo permiten.

La pantalla no debe heredar filtros dinamicos de metadata en runtime. Los filtros finales deben ser un query object backend con whitelist y pruebas.

## Acciones candidatas

Acciones historicas genericas observadas en CRUD:

- buscar;
- limpiar filtros;
- cerrar pestanas;
- guardar busqueda;
- busqueda simple/avanzada/general;
- selector de columnas;
- exportar;
- refrescar;
- nuevo registro;
- importar;
- actualizacion masiva;
- ver detalle;
- eliminar.

Acciones candidatas para iLiniumTech en una version posterior:

- listado read-only;
- filtros;
- paginacion;
- ordenacion limitada por whitelist;
- vuelta al listado conservando filtros.

El primer corte propuesto por `SDD-2026-013` queda limitado a listado read-only minimizado; el detalle read-only pasa a SDD posterior.

Acciones que deben quedar fuera hasta SDD especifica:

- alta de suplemento;
- edicion;
- anulacion;
- importacion;
- actualizacion masiva;
- borrado;
- exportacion con PII;
- ejecucion de workflows;
- envio de comunicaciones;
- conciliacion EIAC;
- documentos/recibos/declaraciones relacionados.

## Permisos iLiniumTech candidatos

Permisos propuestos, no implementados:

- `suplementos.catalogs`: consultar catalogos de filtros.
- `suplementos.read`: acceder al listado.
- `suplementos.detail`: acceder al detalle.
- `suplementos.export`: exportar, si se aprueba.
- `suplementos.create`: crear, solo con SDD de escritura.
- `suplementos.update`: modificar, solo con SDD de escritura.
- `suplementos.delete`: eliminar, si existiera caso funcional aprobado.
- `suplementos.import`: importar, solo con SDD y validacion de seguridad.
- `suplementos.execute`: ejecutar acciones/workflows explicitamente modelados.

Regla: la visibilidad del menu puede depender de estos permisos iLiniumTech, pero la seguridad real debe aplicarse siempre en backend antes de leer datos.

## Propuesta Vue/API estatica

### Frontend candidato

Ruta actual fixture y futura funcional: `/suplementos`.

Feature candidata:

- `iLiniumTech.Frontend/src/features/suplementos/SuplementosView.vue`.
- `SuplementosFilters.vue`.
- `SuplementosTable.vue`.
- `SuplementoDetailView.vue`.
- `useSuplementos.ts`.
- `suplementosTypes.ts`.

Patron recomendado: seguir el patron de `polizas` cuando aporte consistencia, sin convertirlo en un generador generico.

Estados obligatorios:

- cargando sesion;
- sin sesion;
- sin broker activo;
- sin permiso;
- cargando catalogos;
- cargando listado;
- sin resultados;
- error backend sanitizado;
- detalle no autorizado/no encontrado sin revelar existencia cruzada.

### Backend candidato

Endpoints futuros:

- `GET /api/suplementos/catalogs`.
- `GET /api/suplementos`.
- `GET /api/suplementos/{id}` solo en SDD posterior de detalle.

Contratos candidatos:

- `SuplementosQuery` con filtros explicitamente permitidos.
- `SuplementosListItem`.
- `SuplementoDetail`.
- `SuplementosCatalogs`.

Reglas backend:

- validar sesion y broker antes de resolver conexion;
- no aceptar broker por header libre en entornos reales;
- aplicar permisos `suplementos.*`;
- usar queries parametrizadas y whitelists;
- no ejecutar `QueryStatic`;
- no leer metadata AppBuilder en runtime;
- sanitizar errores con `correlationId`;
- no exponer PII por defecto.

### Datos

Repositorio candidato:

- `ISuplementosRepository`.
- Implementacion `InMemory` con fixtures anonimizadas solo para desarrollo.
- Implementacion SQL read-only solo cuando DBA confirme vista/tablas, permisos, broker y PII.

Se recomienda priorizar una vista autorizada de lectura, por ejemplo una vista funcional preparada por DBA, antes que consultar directamente todas las tablas especializadas.

## Riesgos

- Confundir tablas especializadas con tabs reales de UI.
- Reintroducir runtime dinamico de AppBuilder si se intenta renderizar desde `IapMenu`, `IapComponent` o datasource en produccion.
- Copiar SQL legacy incompleto o generico de `RepositorioBusqueda`.
- Exponer PII de tomador, beneficiario, documento, email, telefono, direccion, IBAN o importes.
- Permitir lectura cruzada de brokers por no validar `currentBrokerId` contra `allowedBrokerIds`.
- Mostrar referencias de poliza/suplemento que permitan inferir existencia de registros no autorizados.
- Activar acciones de escritura/importacion/ejecucion sin SDD y sin auditoria.
- Usar catalogos/lookups historicos sin confirmar equivalencia funcional.
- Presentar `Suplementos` como MVP cerrado sin UAT ni metadata real.

## Pruebas necesarias para desarrollo futuro

### Backend

- 401 sin sesion.
- 403 sin `suplementos.read`.
- 403/404 generico para broker no autorizado.
- filtros parametrizados y payloads maliciosos.
- whitelist de sort.
- paginacion estable.
- minimizacion PII en listado.
- detalle con permiso `suplementos.detail`.
- no revelacion de existencia en detalle cruzado.
- errores sanitizados con `correlationId`.

### Frontend

- guard de ruta segun sesion y permisos.
- no llama a API si falta broker o permiso.
- filtros y paginacion con estado reproducible.
- empty/error/loading/no permission.
- vuelta de detalle conservando filtros.
- no renderiza nombres internos AppBuilder ni `QueryStatic`.
- accesibilidad de filtros, tabla, botones y foco.

### QA/UAT

- UAT de columnas del listado.
- UAT de filtros reales.
- UAT de detalle y secciones sensibles.
- comparativa contra AppBuilder solo como evidencia manual/sanitizada.
- smoke visual desktop/mobile.
- secret scan antes de PR.
- dependency audit y CORS audit si toca runtime.

## Bloqueos

Bloqueos externos:

- Obtener metadata real y sanitizada del menu `Suplementos`: `IapMenu`, `componentId`, jerarquia, permisos historicos, componente raiz y datasources.
- Confirmar con producto si `Suplementos` es pantalla independiente, subflujo de `Polizas` o ambos.
- Confirmar con DBA vista/tablas autorizadas y regla multi-tenant/broker.
- Confirmar matriz de permisos real.
- Confirmar PII y campos que pueden mostrarse por rol.
- Definir propietario UAT.

Pendientes tecnicos:

- Revisar y aprobar `SDD-2026-013` antes de conectar API o datos reales.
- Preparar extractor offline/sanitizado especifico de pagina cuando haya entorno autorizado.
- Definir contrato API y DTOs minimizados.
- Definir fixtures anonimizadas si se decide un corte MVP.

Completado con evidencia:

- Revisadas guias de gobierno y decision de arquitectura.
- Revisada navegacion estatica actual de iLiniumTech.
- Revisadas fuentes locales AppBuilder sin abrir configuracion sensible.
- Documentada evidencia encontrada y ausente.
- Confirmado que no se deben crear subagentes de componentes en esta ronda por falta de tabs/submenus especificos.

## Siguiente paso recomendado

Antes de programar, lanzar un agente de extraccion offline/documental para localizar metadata real de `Suplementos` en un artefacto sanitizado o entorno read-only aprobado. La salida minima que debe traer es:

- menu `Suplementos` con `componentId`, padre, hijos y permisos;
- componente raiz con `idType`, `idSubType`, nombre y descripcion;
- componentes hijos/tabs/submenus si existen;
- datasource principal y campos visibles/buscables, sin SQL sensible;
- acciones/eventos asociados, sin ejecutar workflows;
- object groups/permisos historicos;
- propuesta de SDD con UAT owner.

Hasta tener esa salida, el desarrollo de `Suplementos` queda bloqueado de forma honesta.

## Readiness Operativa seguros 2026-05-18

Estado actual:

- Frontend fixture protegido en `/suplementos`, con `SuplementosView.vue` y `SuplementosView.test.ts`.
- SDD draft relacionada: `SDD-2026-013 Suplementos read-only minimizado`.
- Carril permitido ahora: documentacion, fixture read-only, pruebas de bloqueo y preparacion de UAT/DBA.
- Carril bloqueado: API real, detalle, tabs por tipo, alta/edicion/anulacion, workflows, adjuntos, importes, banco, documentos, recibos/declaraciones relacionados y exportacion.

Patrones detectados en el frontend actual:

- `AppShell` compartido.
- Fixtures locales anonimizados.
- Filtros locales por referencia/concepto, poliza, tipo, situacion y fecha efecto desde.
- Tabla paginada, empty state y acciones deshabilitadas.
- Texto accesible `suplementos-blocked-actions` enlazado con botones bloqueados y campos restringidos.
- Tests que verifican render, paginacion, filtrado, empty state y ausencia de marcadores runtime/secretos/campos sensibles.

Bloqueos de PII, finanzas y workflows:

- No exponer tomador, beneficiario, documento, email, telefono, direccion, IBAN, titular bancario ni datos de prestamo.
- No exponer importes, primas, valores anteriores/nuevos, tasas, comisiones, rescates, aportaciones ni calculos.
- No convertir tablas especializadas por tipo en tabs reales sin evidencia de pantalla/UAT.
- No activar documentos, comunicaciones, EIAC, recibos/declaraciones relacionados, workflows ni escrituras.

Permisos candidatos:

- Primer corte: `suplementos.catalogs`, `suplementos.read`.
- Posteriores: `suplementos.detail`, `suplementos.export`, `suplementos.create`, `suplementos.update`, `suplementos.delete`, `suplementos.import`, `suplementos.execute`.
- La lectura inicial no concede PII, importes, banco, documentos, relacion con recibos/declaraciones ni workflows.

Dependencias UAT/DBA:

- Confirmar si `Suplementos` es pagina independiente, subflujo de Polizas o ambas cosas.
- Confirmar columnas y filtros minimos del listado.
- Confirmar origen SQL autorizado y regla de broker/tenant.
- Confirmar catalogos de tipo/situacion y equivalencia funcional.
- Confirmar campos prohibidos por rol y politica de detalle futuro.

Tareas futuras pequenas:

- `producto-suplementos-uat-columns`: cerrar columnas/filtros del listado.
- `security-suplementos-pii-finance-review`: clasificar PII, banco, importes, riesgo y documentos.
- `backend-suplementos-readonly-contract`: preparar DTOs y validadores sin SQL real.
- `frontend-suplementos-fixture-hardening`: mantener acciones bloqueadas y revisar accesibilidad.

Criterios de aceptacion del siguiente incremento:

- SDD revisada y UAT/DBA desbloqueados antes de datos reales.
- Sin runtime AppBuilder ni SQL heredado.
- Backend valida sesion, permiso y broker antes de consultar.
- Listado no proyecta PII, banco, importes, documentos, riesgo ni textos libres.
- Tests backend/frontend y auditorias aplicables documentados.
