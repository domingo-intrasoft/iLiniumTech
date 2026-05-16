# Pagina AppBuilder: Liq.Cia

Fecha: 2026-05-16

Rol IA: jefe de pagina `Liq.Cia`.

Ronda: solo documentacion. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se convierte metadata AppBuilder en contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Hay evidencia local suficiente para afirmar que `Liq.Cia` existe en el menu objetivo de iLiniumTech y que AppBuilder contiene un dominio de datos de liquidacion de compania: tablas `LiquidacionCompania*`, vistas `vw_LiquidacionCia`, `vw_Recibo_LiqCia`, vistas de reporting y conversiones de compania. Sin embargo, no hay evidencia local suficiente de una pagina AppBuilder completa de `Liq.Cia` con `componentId`, datasource de pantalla, columnas, filtros, acciones, permisos historicos, tabs o submenus.

Por tanto, el desarrollo de una pagina iLiniumTech de `Liq.Cia` debe quedar bloqueado hasta obtener una de estas entradas:

- metadata AppBuilder sanitizada de la pantalla real de `Liq.Cia`;
- SDD funcional aprobada por producto;
- UAT owner que confirme objetivo, columnas, filtros, acciones, permisos y datos autorizados;
- validacion DBA de tablas/vistas, broker, permisos, `SESSION_CONTEXT` y campos sensibles;
- decision de si `Liq.Cia` sera read-only, gestion operativa o flujo de conciliacion/liquidacion.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`.
- `PLANS.md`.
- `README.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/appbuilder/pages/README.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- `docs/appbuilder/pages/recibos/README.md`.
- `docs/appbuilder/pages/polizas/README.md`.
- `docs/appbuilder/pages/clientes/README.md`.

Repositorio AppBuilder, solo lectura y sin abrir configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\tools\graphql\operations\Menu_GET_BY_APPLICATION_ID.graphql`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\menu\infrastructure\HelperMenu.ts`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Helper\Intrasoft.ApiBuilderCommon\Business\App\bllMenu.cs`.
- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\router.js`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\MaestroDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\Repositorios\RepositorioLiquidacionCompaniaConversion.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\Repositorios\RepositorioLiquidacionCompaniaConversionDetalle.cs`.

Restricciones aplicadas:

- No se abrieron `appsettings`, archivos `.config`, connection strings, dumps, capturas, binarios ni datos personales reales.
- No se ejecuto SQL.
- No se ejecuto extractor `Live`.
- No se copio ningun secreto ni valor de infraestructura.

## Evidencia de menu y metadata

### iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Liq.Cia` aparece como item de primer nivel:

- etiqueta: `Liq.Cia`;
- icono: `pi pi-list`;
- estado: `disabled: true`;
- ruta: no definida;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma que iLiniumTech reserva el hueco de navegacion, pero todavia no hay pagina estatica propia.

### AppBuilder menu

La infraestructura AppBuilder revisada indica que el menu historico se cargaba dinamicamente:

- `Menu_GET_BY_APPLICATION_ID.graphql` recupera `id`, `parentId`, `order`, `title`, `idIcon`, `active`, `componentId`, `urlComponentStatic`, `urlRouteComponentStatic`, `keepAlive` y localizaciones.
- `HelperMenu.buildMenu` convierte la lista plana de `IapMenu` en arbol por `parentId`, usa `title` como etiqueta, `idIcon` como icono, `componentId`/rutas como destino y `active` como visibilidad.
- `bllMenu.getByApplication` filtra menus no administradores por grupos de perfil y directivas de objeto tipo menu con `View == true`.
- `router.js` contiene rutas base genericas; no hay ruta estatica especifica de `Liq.Cia` en el frontend local revisado.

Esta evidencia explica el mecanismo historico de menu, pero no identifica la fila real de `IapMenu` de `Liq.Cia`, su `componentId`, jerarquia, permiso concreto ni pantalla asociada.

### Etiquetas/localizacion

No se localizo una etiqueta AppBuilder especifica y versionada para `Liq.Cia` en las fuentes frontend revisadas. La etiqueta confirmada en esta ronda procede del menu estatico iLiniumTech, no de una fila AppBuilder extraida.

### Gestion generica

`GestionConst.ts` contiene claves para `Cliente`, `Poliza`, `Recibo`, `Siniestro` y `Suplemento`, pero no contiene una clave especifica de `Liq.Cia`. `TableIcons.ts` solo aporta asociacion de icono para `Recibo` dentro de las coincidencias revisadas. Esto sugiere que `Liq.Cia` no aparece como una gestion generica basica equivalente a esas claves en el frontend compartido revisado.

## Evidencia de datos

### Constantes frontend

`NombreTablasConst.ts` declara entidades/vistas relevantes:

- `LiquidacionCompania`.
- `LiquidacionCompaniaConcepto`.
- `LiquidacionCompaniaConversion`.
- `LiquidacionCompaniaConversionDetalle`.
- `LiquidacionCompaniaDetalle`.
- `LiquidacionCompaniaDetalleAuxiliar`.
- `LiquidacionCompaniaDetalleTmp`.
- `vw_LiquidacionCia`.
- `vw_Recibo_LiqCia`.
- `vw_rpt_LiquidacionCompania`.
- `vw_rpt_LiquidacionCompaniaConcepto`.

Estas constantes son evidencia de dominio de datos, no de pantalla.

### Modelo EF de BBDD modelo

`ModeloDbContext.cs` mapea:

- `LiquidacionCompaniaConceptos`.
- `LiquidacionCompaniaConversions`.
- `LiquidacionCompaniaConversionDetalles`.
- `LiquidacionCompaniaDetalles`.
- `LiquidacionCompaniaDetalleAuxiliars`.
- `LiquidacionCompaniaDetalleTmps`.
- `LiquidacionCompania`.
- `VwLiquidacionCia`.
- `VwReciboLiqCia`.
- `VwRptLiquidacionCompania`.
- `VwRptLiquidacionCompaniaConceptos`.

Relaciones e indices observados:

- `LiquidacionCompaniaConcepto` referencia `LiquidacionCiaId`.
- `LiquidacionCompaniaDetalle` tiene indices por `LiquidacionCiaId`, `LiqColDetalleId`, `ReciboId`, `MvtoId`, `IdGestor`, `Devolucion`, `UValidado`, `IdOld` y `BrokerIntegracionId`.
- `LiquidacionCompaniaDetalle` referencia `LiquidacionCia`, `Recibo`, `Mvto` y `BrokerIntegracion`.
- `LiquidacionCompaniaDetalleTmp` tiene indices por `LiquidacionId`, `DetalleId`, `ReciboId`, `PrimaNeta`, `PrimaTotal` y `ReciboCia`.
- `LiquidacionCompania` tiene indices por `CiaId`, `F_Cierre`, `FacturaId`, `Fecha`, `IdOld` y `BrokerIntegracionId`.
- `LiquidacionCompania` referencia `BrokerIntegracion`, `Factura` y `Oficina`.

Campos relevantes observados en mapeos:

- cabecera: `Fecha`, `F_Cierre`, `CiaId`, `FacturaId`, `OficinaId`, `Total`, `Complementaria`, `IdDivisa`;
- detalle: `ReciboId`, `MvtoId`, `ReciboCia`, `IdGestor`, `Mediador`, `FechaMovimiento`, `PrimaTotal`, `Comision`, `Comision2`, `Liquido`, `ImporteLiquidar`, `Devolucion`, `UValidado`, `F_Validado`, `Comentario`;
- temporal/importacion: `Poliza`, `PolApli`, `ReciboCia`, `TipoMovimiento`, `F_Efecto`, `F_Movimiento`, `F_Vencimiento`, `PrimaNeta`, `PrimaTotal`, `Comision`, `Comision2`, `CiaFicheroId`, `Colectivo`, `Gestor`, `Mediador`, `Observaciones`;
- vista `vw_LiquidacionCia`: `Descripcion`, `Fecha`, `FechaMvtoBancario`, importes bancarios/comisiones/desglose, `ImporteTotal`, `NombreCompleto`;
- vista `vw_Recibo_LiqCia`: `F_Cierre`, `Fecha`, `IdGestor`, `Liquido`, `PrimaTotal`;
- reporting `vw_rpt_LiquidacionCompania`: `Cia`, `Cliente`, `Poliza`, `ReciboCia`, `Tipo`, `TipoRecibo`, fechas, primas, comisiones, liquido, importes y gestor.

### Modelo EF de BBDD maestro

`MaestroDbContext.cs` mapea:

- `LiquidacionCompaniaConversion`;
- `LiquidacionCompaniaConversionDetalle`.

Los repositorios `RepositorioLiquidacionCompaniaConversion` y `RepositorioLiquidacionCompaniaConversionDetalle` son repositorios genericos EF sobre esas tablas. La evidencia sugiere configuracion de conversion/importacion de liquidaciones, pero no pantalla de usuario final.

## Componente raiz e hijos

### Componente raiz historico

No confirmado.

No se encontro localmente:

- `IapMenu.Id` de `Liq.Cia`;
- `ComponentId` raiz;
- `IAP_Component` de la pantalla;
- datasource de pantalla;
- `QueryStatic`;
- layout, form, grid o search config versionado para `Liq.Cia`.

### Hijos, tabs o submenus

No confirmados.

No se detectaron con evidencia local suficiente:

- tabs especificas de `Liq.Cia`;
- submenus bajo `Liq.Cia`;
- componentes internos reales;
- vista de detalle confirmada;
- pestana de recibos confirmada;
- pestana de conceptos confirmada;
- pestana de conciliacion/banco confirmada;
- accion de cierre, validacion, importacion o exportacion confirmada.

## Decision de subagentes de componentes

No se crearon subagentes de componentes en esta ronda.

Motivo: no se detectaron pestanas, submenus o componentes internos reales y especificos de `Liq.Cia`. Los artefactos localizados son tablas, vistas, reporting y repositorios genericos, no componentes de pagina verificables.

Si una ronda posterior aporta metadata sanitizada o SDD funcional, se recomienda crear subagentes documentales por areas confirmadas, por ejemplo:

- listado/busqueda de liquidaciones de compania;
- detalle de cabecera de liquidacion;
- detalle de recibos/movimientos;
- conceptos/desglose de importes;
- conciliacion bancaria;
- importacion/conversion;
- reporting/exportacion.

Cada subagente debera escribir solo en `docs/appbuilder/pages/liq-cia/components/*.md` y no programar durante la fase documental.

## Datos candidatos para producto

Los datos candidatos derivan de mapeos EF y constantes, no de metadata de pantalla. Deben confirmarse con DBA/producto antes de desarrollo.

Cabecera de liquidacion:

- identificador interno;
- compania;
- fecha;
- fecha de cierre;
- total;
- divisa;
- oficina;
- factura asociada;
- complementaria;
- broker/integracion.

Listado/resumen:

- descripcion;
- compania;
- fecha;
- fecha de movimiento bancario;
- importe bancario;
- importe total;
- importes por tipo o desglose;
- comisiones;
- nombre completo, si producto confirma que no expone PII o se minimiza.

Detalle de recibos/movimientos:

- recibo compania;
- poliza;
- tipo/tipo recibo;
- fechas de efecto, vencimiento, cobro y movimiento;
- prima total;
- comision;
- comision 2;
- liquido;
- gestor;
- mediador;
- devolucion;
- validacion;
- comentario/observaciones.

Conceptos:

- concepto;
- importe;
- impuesto;
- importe afectado.

Conversion/importacion:

- campo;
- tipo;
- condicion;
- origen;
- destino.

## Filtros detectados o inferidos

No se encontro metadata local de filtros AppBuilder de la pagina `Liq.Cia`.

Filtros candidatos por evidencia de campos:

- fecha desde/hasta;
- fecha cierre desde/hasta;
- compania;
- factura;
- oficina;
- estado de cierre o complementaria, si existe regla funcional;
- recibo compania;
- poliza;
- gestor;
- mediador;
- validado/no validado;
- devolucion;
- importe total o liquido por rango;
- broker, siempre derivado del contexto autorizado, nunca de un campo libre de UI.

Estos filtros no deben implementarse automaticamente. Requieren SDD, whitelist backend y UAT.

## Acciones historicas

No se encontro metadata local de acciones especificas para `Liq.Cia`.

Acciones candidatas que requieren SDD propia:

- buscar liquidaciones;
- abrir detalle;
- ver recibos incluidos;
- ver conceptos/desglose;
- validar o marcar detalle;
- cerrar liquidacion;
- gestionar devoluciones;
- importar fichero/conversion;
- conciliar con movimiento bancario;
- generar factura o relacionarla;
- exportar/reportar;
- navegar a recibo, poliza o compania;
- anular, recalcular o reabrir liquidacion.

Para un primer MVP seguro, si producto desbloquea esta pagina, la recomendacion es comenzar solo con read-only: listado, filtros confirmados y detalle minimizado. Cualquier escritura, importacion, conciliacion o cierre debe tener SDD, permisos, transacciones, auditoria y UAT.

## Permisos historicos

No se encontro metadata local suficiente de permisos historicos por grupo, perfil o directiva especifica para `Liq.Cia`.

Permisos iLiniumTech candidatos:

- `liqCia.catalogs`: catalogos de filtros confirmados.
- `liqCia.read`: listado read-only.
- `liqCia.detail`: detalle read-only.
- `liqCia.financial`: ver importes/comisiones/desgloses completos.
- `liqCia.export`: exportar resultados/reporting.
- `liqCia.validate`: validar detalle o liquidacion, si se aprueba.
- `liqCia.import`: importar/conversion de liquidaciones, si se aprueba.
- `liqCia.close`: cerrar o reabrir liquidacion, si se aprueba.
- `liqCia.write`: escritura general futura, bloqueada hasta SDD especifica.

Reglas:

- AppBuilder puede servir como evidencia de permisos historicos, nunca como motor runtime.
- El backend debe autorizar antes de leer datos.
- El frontend solo oculta o muestra capacidades segun `/api/me`, pero no es barrera de seguridad.
- Broker activo debe validarse contra brokers permitidos antes de resolver conexion o consultar datos.
- Importes, comisiones, devoluciones, facturas y datos bancarios requieren clasificacion y permiso explicito.

## Propuesta Vue/API estatica

Esta propuesta no autoriza programacion; es una guia si producto desbloquea la pagina.

### Frontend

Ruta candidata:

- `/liq-cia`.
- `/liq-cia/:id`.

Feature candidata:

- `iLiniumTech.Frontend/src/features/liq-cia`.

Componentes estaticos candidatos:

- `LiqCiaView.vue`: pagina de listado.
- `LiqCiaFilters.vue`: filtros confirmados por SDD/UAT.
- `LiqCiaTable.vue`: tabla con columnas minimizadas.
- `LiqCiaDetailView.vue`: detalle read-only.
- `LiqCiaRecibosPanel.vue`: panel de recibos/movimientos solo si se confirma.
- `LiqCiaConceptosPanel.vue`: panel de conceptos solo si se confirma.
- `useLiqCia.ts`: estado de filtros, paginacion, errores, permisos y llamadas API.
- `liqCiaTypes.ts`: contratos frontend.
- `liqCiaApi.ts`: cliente API explicito.

Estados obligatorios:

- cargando sesion;
- sin sesion;
- sin broker;
- sin permiso;
- error backend;
- sin resultados;
- listado con paginacion;
- detalle no autorizado;
- detalle no encontrado sin revelar existencia cruzada por broker.

Menu:

- `Liq.Cia` debe pasar de `disabled: true` a ruta solo cuando exista SDD y endpoint.
- Permiso candidato: `liqCia.read`.
- No debe tener hijos hasta confirmar submenus reales.

### Backend

Endpoints candidatos:

- `GET /api/liq-cia/catalogs`.
- `GET /api/liq-cia`.
- `GET /api/liq-cia/{id}`.
- `GET /api/liq-cia/{id}/recibos`, solo si producto confirma panel de detalle.
- `GET /api/liq-cia/{id}/conceptos`, solo si producto confirma panel de detalle.

Contratos candidatos:

- `LiqCiaSearchRequest`: filtros whitelisted, `page`, `pageSize`, `sort`.
- `LiqCiaListItem`: identificador estable, compania, fecha, cierre, total minimizado si procede, estado funcional si se confirma.
- `LiqCiaDetail`: cabecera y secciones estaticas minimizadas segun permisos.
- `LiqCiaCatalogs`: companias, oficinas, gestores, estados u otros catalogos confirmados.

Backend debe:

- usar API explicita, no endpoint generico de metadata;
- validar broker antes de SQL;
- usar parametros y whitelists;
- aplicar `SESSION_CONTEXT` solo con valores autenticados;
- sanitizar errores y devolver `correlationId`;
- no proyectar PII, banco, facturas, comisiones o comentarios sin permiso;
- no exponer SQL, nombres internos sensibles ni connection strings;
- no ejecutar procesos de cierre/importacion/conciliacion sin SDD.

## Que no debe replicarse

- No replicar AppBuilder como runtime dinamico.
- No construir la pantalla desde `IAP_Component`, `IapMenu`, datasources o constantes de tabla.
- No exponer endpoints genericos de datasource, componente, query, menu o accion.
- No ejecutar `QueryStatic` ni SQL heredado libre.
- No importar ficheros, cerrar liquidaciones, validar movimientos ni generar facturas sin SDD.
- No mostrar datos bancarios, facturas, comisiones o PII por defecto.
- No aceptar broker desde header libre como autoridad.
- No confiar en API key o `DemoSession` como seguridad productiva.
- No usar `SELECT *`.
- No registrar SQL completo, cookies, tokens, connection strings, cuentas, documentos, telefonos, emails, direcciones, comentarios libres ni importes sensibles si no es imprescindible y permitido.

## Riesgos

Riesgos funcionales:

- Confundir evidencia de datos con pantalla real.
- Implementar `Liq.Cia` como listado simple cuando negocio podria necesitar un flujo de conciliacion/cierre.
- Interpretar erroneamente importes: prima, comision, liquido, importe bancario, importe total o desglose por tipo.
- Mezclar liquidacion de compania con liquidacion de colaborador (`Liq.Col`), recibos, facturas o contabilidad sin frontera funcional.
- Exponer acciones de escritura que cambian estado financiero sin SDD.

Riesgos de datos:

- Exposicion de importes/comisiones/facturas sin permiso.
- Exposicion de PII en `NombreCompleto`, `Cliente`, `Poliza`, comentarios u observaciones.
- Exposicion de datos bancarios o conciliacion.
- Lectura cruzada entre brokers.
- Uso de vistas no autorizadas para UAT o produccion.

Riesgos tecnicos:

- Reintroducir AppBuilder con otro nombre mediante un generador de pantallas.
- Reusar SQL dinamico heredado.
- Usar campos legacy como contrato API estable.
- Crear dependencias circulares con `Recibos`, `Polizas`, `Clientes`, `Liq.Col`, `Informes` o `Administracion`.

Riesgos de seguridad:

- Errores 403/404 que revelen existencia de liquidaciones de otro broker.
- Logs con datos financieros, bancarios o personales.
- `SESSION_CONTEXT` contaminado por pooling.
- Permisos de frontend sin validacion backend.
- Exportaciones o reporting con datos excesivos.

## Pruebas necesarias para desarrollo futuro

Backend:

- `GET /api/liq-cia/catalogs` exige sesion/permisos.
- `GET /api/liq-cia` exige `liqCia.read`.
- `GET /api/liq-cia/{id}` exige `liqCia.detail`.
- broker ausente devuelve error seguro.
- broker no permitido no resuelve conexion ni consulta SQL.
- sort/filtros fuera de whitelist se rechazan.
- payloads maliciosos no alteran SQL.
- importes/comisiones/facturas no se devuelven sin permiso.
- PII/banco/comentarios no se devuelven sin permiso.
- 401/403/404 no revelan existencia cruzada.
- errores publicos incluyen `correlationId` cuando aplique.

Frontend:

- menu `Liq.Cia` visible/activo solo con permiso y feature desbloqueada.
- ruta protegida redirige o muestra estado seguro si no hay sesion.
- filtros se inicializan y limpian correctamente.
- paginacion y ordenacion no pierden filtros.
- detalle conserva vuelta al listado.
- estados loading/empty/error/no permission/no broker.
- no aparecen `AppBuilder`, `QueryStatic`, connection strings, SQL ni metadata en DOM.

QA/UAT:

- columnas y filtros confirmados por responsable funcional.
- comparativa controlada con AppBuilder o BBDD autorizada sin versionar datos reales.
- smoke visual desktop/mobile si se implementa UI.
- secret scan limpio.
- dependency audit sin findings bloqueantes.
- CORS audit si toca API/configuracion.

## Bloqueos y preguntas abiertas

Bloqueos externos:

- Falta metadata sanitizada de la pantalla real de `Liq.Cia`.
- Falta confirmar si `Liq.Cia` debe ser modulo independiente, vista de recibos, flujo financiero, conciliacion o reporting.
- Falta matriz real de permisos para liquidacion de compania.
- Falta validar origen SQL autorizado: `vw_LiquidacionCia`, `LiquidacionCompania`, `vw_rpt_LiquidacionCompania` u otra vista controlada.
- Falta confirmar campos sensibles y reglas de minimizacion.
- Falta UAT owner.
- Falta decision de auth productiva y permisos reales.
- Falta confirmar si hay escrituras: cierre, validacion, importacion, conciliacion, factura o devolucion.

Preguntas para producto/DBA:

- Cual es el objetivo MVP de `Liq.Cia`: consulta read-only, detalle financiero, conciliacion, cierre, importacion o reporting?
- Que columnas son imprescindibles en listado?
- Que filtros usa negocio diariamente?
- Que importes pueden ver todos los usuarios y cuales requieren permiso?
- Deben mostrarse datos bancarios, facturas, comisiones o comentarios? Bajo que permiso y mascara?
- Que relacion debe tener con `Recibos`, `Polizas`, `Clientes`, `Liq.Col` e `Informes`?
- Que estado funcional tiene una liquidacion y donde se calcula?
- Debe existir escritura o sera read-only?
- Que vista o tabla recomienda DBA para lectura read-only por broker?
- Que claves de `SESSION_CONTEXT` son obligatorias para que las vistas apliquen seguridad real?

## Evidencia de comandos

Comandos ejecutados en modo lectura/documentacion:

```powershell
git status --short --branch
Get-Content -Raw AGENTS.md
Get-Content -Raw PLANS.md
Get-Content -Raw docs\PLAN_MAESTRO_IA.md
Get-Content -Raw docs\ROADMAP_OBJETIVO_FINAL.md
Get-Content -Raw docs\appbuilder\pages\README.md
Get-Content -Raw iLiniumTech.Frontend\src\layout\appNavigation.ts
rg -n -i "Liq\.Cia|Liq Cia|LiqCia|Liquidacion.*Cia|LiquidacionCia" C:\Desarrollo\AppBuilder
rg -n "VW_LIQUIDACIONCIA|VW_RECIBO_LIQCIA|vw_LiquidacionCia|vw_Recibo_LiqCia|LiquidacionCompania|LiquidacionCia" C:\Desarrollo\AppBuilder\src
rg -n -i "liq|liquidacion|compania" C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes
```

Validacion documental y seguridad se ejecuta al cierre de escritura documental.

## Clasificacion de pendientes

Completado con evidencia:

- entrada `Liq.Cia` localizada en menu iLiniumTech;
- mecanismo AppBuilder de menu revisado;
- ausencia de ruta estatica AppBuilder especifica constatada en fuentes locales revisadas;
- constantes de tablas/vistas de liquidacion de compania localizadas;
- mapeos EF de tablas/vistas de liquidacion de compania localizados;
- repositorios de conversion de liquidacion de compania localizados;
- no hay tabs, submenus ni componentes especificos confirmados localmente;
- no se ejecuto SQL ni extractor `Live`.

Pendiente tecnico:

- crear SDD de `Liq.Cia` si producto prioriza el modulo;
- definir contrato API estatico;
- definir feature Vue estatica;
- definir minimizacion financiera/PII;
- definir tests backend/frontend;
- actualizar menu cuando haya permiso y ruta real.

Bloqueado externo:

- metadata real de pantalla;
- UAT funcional;
- regla DBA de lectura;
- matriz de permisos;
- auth productiva;
- clasificacion PII/financiera/bancaria;
- decision de alcance read-only frente a escritura/conciliacion/importacion.
