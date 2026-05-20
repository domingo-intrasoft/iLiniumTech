# Pagina AppBuilder: Liq.Col

Fecha: 2026-05-16

Rol IA: jefe de pagina `Liq.Col`.

Ronda: documentacion y analisis. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se convierte metadata AppBuilder en contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Hay evidencia local suficiente para afirmar que existe un dominio de datos de liquidaciones de colaborador, relacionado con recibos, acuerdos de reparto, liquidacion de compania, tesoreria/oficina y reporting. Sin embargo, no hay evidencia local suficiente para reconstruir una pagina AppBuilder real de `Liq.Col`:

- no se encontro `componentId` de pantalla;
- no se encontro layout de pagina;
- no se encontraron pestanas o submenus propios;
- no se encontraron columnas/filtros/acciones historicas de una pantalla concreta;
- no se encontraron permisos historicos de pantalla por grupo/perfil;
- no se encontro ruta AppBuilder versionada ni flujo UI especifico.

Por tanto, `Liq.Col` no debe desarrollarse todavia como pagina iLiniumTech salvo que producto aporte una SDD y UAT, o que se extraiga metadata sanitizada suficiente de AppBuilder. Las entidades y vistas encontradas sirven como evidencia de dominio, no como contrato de UI/API.

Actualizacion 2026-05-20: se crea la SDD draft [`SDD-2026-018 Liq.Col read-only financiero minimizado`](../../../sdd/specs/iLiniumTech/SDD-2026-018-liq-col-read-only.md). La SDD prepara un primer corte futuro de listado read-only minimizado, pero no autoriza todavia API, SQL real, comisiones reales, liquidos, retenciones, banco, exportacion, detalle financiero ni escrituras.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`.
- `PLANS.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/appbuilder/pages/README.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- `docs/appbuilder/pages/recibos/README.md`.

Repositorio AppBuilder, solo lectura y evitando configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Eiac\EiacDbContext.cs`.

Restricciones aplicadas:

- No se abrieron `appsettings`, archivos `.config`, connection strings, dumps, capturas, binarios ni datos personales reales.
- No se ejecuto SQL.
- No se ejecuto extractor `Live`.
- No se copio ningun secreto ni valor de infraestructura.

## Evidencia de menu

### iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Liq.Col` aparece como item de primer nivel:

- etiqueta: `Liq.Col`;
- icono: `pi pi-list-check`;
- estado: `disabled: true`;
- ruta: no definida;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma que iLiniumTech reserva un hueco de navegacion, pero no existe pagina estatica propia ni contrato API asociado.

### Inventario documental

En `docs/appbuilder/pages/README.md`, `Liq.Col` figura como pagina pendiente de evidencia metadata local con documento objetivo `liq-col/README.md`.

### AppBuilder frontend

Se revisaron etiquetas de menu y constantes comunes. La evidencia encontrada no confirma una entrada de menu especifica `Liq.Col` en los archivos frontend locales revisados:

- `Messages.ts` define menus genericos de `Gestion`, pero no incluye etiqueta especifica para liquidaciones de colaborador.
- `GestionConst.ts` solo define claves genericas de gestion como cliente, poliza, recibo, riesgo, suplemento y siniestro.
- `TableIcons.ts` mapea iconos para esas gestiones genericas, sin clave especifica de `Liq.Col`.
- `NombreTablasConst.ts` si contiene tablas y vistas relacionadas con liquidaciones de colaborador, pero son constantes de dominio, no metadata de pantalla.

Conclusiones:

- No hay evidencia local de ruta o pagina frontend AppBuilder especifica para `Liq.Col`.
- No hay evidencia local de tabs o submenus especificos.
- No se puede inferir el diseno real desde constantes de tabla sin inventar producto.

## Evidencia metadata y datos

### Constantes AppBuilder localizadas

En `NombreTablasConst.ts` se localizaron constantes candidatas del dominio:

- `LIQUIDACIONIDENTIDAD = "LiquidacionIdentidad"`.
- `LIQUIDACIONIDENTIDADCONCEPTO = "LiquidacionIdentidadConcepto"`.
- `LIQUIDACIONIDENTIDADDETALLE = "LiquidacionIdentidadDetalle"`.
- `LIQUIDACIONIDENTIDADEXCLUSION = "LiquidacionIdentidadExclusion"`.
- `VW_RECIBO_LIQCOL = "vw_Recibo_LiqCol"`.
- `VW_RPT_LIQUIDACIONCOLABORADOR = "vw_rpt_LiquidacionColaborador"`.
- `VW_RPT_LIQUIDACIONCOLABORADORCONCEPTO = "vw_rpt_LiquidacionColaboradorConcepto"`.
- `PBI_ACUERDOSREPARTOLIQUIDACIONES = "PBI_AcuerdosRepartoLiquidaciones"`.

Tambien aparecen vistas y tablas de colaboradores, acuerdos de reparto, recibos y polizas que pueden estar relacionadas, pero no prueban por si solas una pantalla `Liq.Col`.

### Mapeos EF relevantes

En `ModeloDbContext.cs` se localizaron `DbSet` y mapeos para:

- `LiquidacionIdentidad`.
- `LiquidacionIdentidadConcepto`.
- `LiquidacionIdentidadDetalle`.
- `LiquidacionIdentidadExclusion`.
- `VwReciboLiqCol`.
- `VwRptLiquidacionColaborador`.
- `VwRptLiquidacionColaboradorConcepto`.

`LiquidacionIdentidad`:

- se mapea a tabla `LiquidacionIdentidad`;
- usa trigger `trg_CerrarNoValidados`;
- tiene indices por `F_Cierre`, `IdentidadId + F_Cierre`, `Fecha`, `IdOld + BrokerIntegracionId` e `IdentidadId`;
- contiene importes como `Base`, `Impuesto`, `PorcentajeImpuesto` y `Total`;
- contiene fechas como `Fecha`, `F_Cierre`, `FCR` y `FUM`;
- se relaciona con `BrokerIntegracion`, `Identidad`, `Oficina` y `Tesoreria`.

`LiquidacionIdentidadConcepto`:

- se mapea a tabla `LiquidacionIdentidadConcepto`;
- contiene `Concepto`, `IdImporteAfectado`, `IdImpuesto` e `Importe`;
- se relaciona con `LiquidacionCol` mediante `LiquidacionColId`.

`LiquidacionIdentidadDetalle`:

- se mapea a tabla `LiquidacionIdentidadDetalle`;
- tiene indices por `LiquidacionColId`, `ReciboId`, `IdOld + BrokerIntegracionId` y `LiqCiaDetalleId`;
- contiene importes y porcentajes como `Base`, `Pct`, `ReciboComision`, `ReciboComision2`, `ReciboPrimaNeta` y `ReciboPrimaTotal`;
- contiene validacion por `F_Validado` y `UValidado`;
- se relaciona con `AcuerdoRepartoPoliza`, `AcuerdoRepartoVersion`, `BrokerIntegracion`, `LiqCiaDetalle`, `LiquidacionCol` y `Recibo`.

`LiquidacionIdentidadExclusion`:

- se mapea a tabla `LiquidacionIdentidadExclusion`;
- usa trigger `trg_ValidarExclusion`;
- contiene `Motivo`, `MotivoValidacion` y `UValidado`;
- se relaciona con `Liquidacion` y `Recibo`.

`VwReciboLiqCol`:

- se mapea a vista `vw_Recibo_LiqCol`;
- contiene campos de colaborador, fechas, estado, base, porcentaje y primas de recibo.

`VwRptLiquidacionColaborador`:

- se mapea a vista `vw_rpt_LiquidacionColaborador`;
- contiene datos de colaborador, mediador, cliente, compania, poliza, recibo, ramo, tipo de recibo, primas, comisiones, impuestos, retencion y liquido.

`VwRptLiquidacionColaboradorConcepto`:

- se mapea a vista `vw_rpt_LiquidacionColaboradorConcepto`;
- contiene conceptos e importes asociados a una liquidacion.

### Relaciones de dominio observadas

La evidencia apunta a este grafo funcional de dominio, no a un layout de pantalla:

```mermaid
flowchart LR
  "LiquidacionIdentidad" --> "LiquidacionIdentidadDetalle"
  "LiquidacionIdentidad" --> "LiquidacionIdentidadConcepto"
  "LiquidacionIdentidad" --> "LiquidacionIdentidadExclusion"
  "LiquidacionIdentidadDetalle" --> "Recibo"
  "LiquidacionIdentidadDetalle" --> "LiquidacionCompaniaDetalle"
  "LiquidacionIdentidadDetalle" --> "AcuerdoRepartoPoliza"
  "LiquidacionIdentidadDetalle" --> "AcuerdoRepartoVersion"
  "LiquidacionIdentidad" --> "Identidad/Colaborador"
  "LiquidacionIdentidad" --> "Oficina"
  "LiquidacionIdentidad" --> "Tesoreria/Factura"
```

Lectura funcional prudente:

- `LiquidacionIdentidad` parece representar la cabecera de liquidacion de un colaborador/identidad.
- `LiquidacionIdentidadDetalle` parece representar lineas vinculadas a recibos y acuerdos de reparto.
- `LiquidacionIdentidadConcepto` parece representar importes o conceptos agregados de la liquidacion.
- `LiquidacionIdentidadExclusion` parece registrar recibos excluidos o pendientes de validacion.
- `LiquidacionCompaniaDetalle.LiqColDetalleId` y `LiquidacionIdentidadDetalle.LiqCiaDetalleId` sugieren relacion entre liquidacion de compania y liquidacion de colaborador.

Estas lecturas son inferencias tecnicas a partir de mapeos, no requisitos de producto.

## Componente raiz e hijos

### Componente raiz historico

No se encontro componente raiz AppBuilder real de `Liq.Col`.

La pagina podria haber estado construida desde metadata AppBuilder no versionada localmente, desde un modulo generico, o desde configuracion en BBDD. En esta ronda no se puede confirmar ninguna de esas opciones sin extractor `Live` o metadata sanitizada.

### Componentes hijos especificos

No se encontraron componentes hijos reales y especificos de `Liq.Col`.

No se crean documentos en `docs/appbuilder/pages/liq-col/components/*.md` porque no hay evidencia local suficiente de:

- pestanas;
- submenus;
- grids secundarios;
- acciones contextuales por seccion;
- formulario de cabecera;
- detalle de lineas;
- vista de conceptos;
- vista de exclusiones;
- vista de recibos asociados;
- vista de informes.

Si una ronda posterior aporta metadata sanitizada, entonces si se deberian crear subagentes por componente confirmado.

## Pestanas, submenus y areas internas

Estado: no confirmado.

No hay evidencia local verificable de pestanas o submenus de `Liq.Col`.

Areas candidatas por dominio de datos, pendientes de SDD/UAT:

- busqueda/listado de liquidaciones de colaborador;
- cabecera de liquidacion;
- detalle de recibos liquidados;
- conceptos de liquidacion;
- exclusiones o recibos no validados;
- relacion con Liq.Cia;
- acuerdos de reparto;
- informe o impresion de liquidacion;
- tesoreria/factura;
- auditoria de validacion/cierre.

Estas areas no deben convertirse en componentes hasta tener evidencia funcional.

## Datos candidatos

### Cabecera de liquidacion

Campos candidatos observados:

- `Id`;
- `IdentidadId`;
- `BrokerIntegracionId`;
- `OficinaId`;
- `TesoreriaId`;
- `Fecha`;
- `F_Cierre`;
- `Base`;
- `Impuesto`;
- `PorcentajeImpuesto`;
- `Total`;
- `IdDivisa`;
- `IdOld`;
- `FCR`, `FUM`, `UCR`, `UUM`.

### Detalle de liquidacion

Campos candidatos observados:

- `LiquidacionColId`;
- `ReciboId`;
- `LiqCiaDetalleId`;
- `AcuerdoRepartoPolizaId`;
- `AcuerdoRepartoVersionId`;
- `BrokerIntegracionId`;
- `Base`;
- `Pct`;
- `ReciboComision`;
- `ReciboComision2`;
- `ReciboPrimaNeta`;
- `ReciboPrimaTotal`;
- `IdBaseReparto`;
- `IdDivisa`;
- `F_Validado`;
- `UValidado`;
- `IdOld`.

### Conceptos

Campos candidatos observados:

- `LiquidacionColId`;
- `Concepto`;
- `IdImporteAfectado`;
- `IdImpuesto`;
- `Importe`;
- `FCR`, `FUM`, `UCR`, `UUM`.

### Exclusiones

Campos candidatos observados:

- `LiquidacionId`;
- `ReciboId`;
- `Motivo`;
- `MotivoValidacion`;
- `UValidado`;
- `FCR`, `FUM`, `UCR`, `UUM`.

### Vistas de reporting y recibos

Campos candidatos observados:

- `Colaborador`;
- `ColaboradorNumDocumento`;
- `Mediador`;
- `Cliente`;
- `Compania`;
- `Poliza`;
- `ReciboCia`;
- `Ramo`;
- `TipoRecibo`;
- `BaseReparto`;
- `DetalleClvReparto`;
- `RefClvReparto`;
- `F_Cierre`;
- `F_Efecto`;
- `FechaLiquidacion`;
- `Base`;
- `PrimaNeta`;
- `PrimaTotal`;
- `Comision`;
- `Comisiones`;
- `Impuesto`;
- `Impuestos`;
- `PorcentajeImpuesto`;
- `Retencion`;
- `Liquido`;
- `IdEstado`;
- `Pct`.

### Datos sensibles

El dominio de `Liq.Col` toca datos financieros, comisiones y potencialmente PII:

- nombres o razon social de cliente, colaborador y mediador;
- documento de colaborador;
- poliza y recibo;
- primas, comisiones, impuestos, retenciones y liquidos;
- motivos y observaciones de exclusion o validacion;
- referencias internas de reparto;
- posible informacion de tesoreria/factura.

Regla recomendada:

- listado minimizado;
- detalle solo con permisos especificos;
- documento de colaborador no visible por defecto;
- importes/comisiones bajo permiso funcional;
- motivos libres revisados por riesgo de PII;
- backend como unica autoridad de minimizacion.

## Filtros detectados o inferidos

No se encontro metadata local de filtros AppBuilder para una pantalla `Liq.Col`.

Filtros candidatos por campos observados, pendientes de confirmacion:

- fecha de liquidacion desde/hasta;
- fecha de cierre desde/hasta;
- colaborador/identidad;
- oficina;
- estado/cierre/validacion;
- poliza;
- recibo compania;
- ramo;
- compania;
- mediador;
- importe o liquido por rango;
- divisa;
- con/sin exclusiones;
- relacion con liquidacion de compania;
- broker.

Ninguno de estos filtros debe implementarse sin SDD/UAT, porque el significado de liquidado, cerrado, validado, excluido o relacionado con Liq.Cia puede tener reglas de negocio no visibles en los mapeos.

## Acciones historicas

No se encontro metadata local de acciones de pantalla para `Liq.Col`.

Acciones candidatas de dominio que requieren SDD propia:

- consultar liquidaciones;
- abrir detalle;
- ver recibos asociados;
- ver conceptos;
- ver exclusiones;
- ver relacion con Liq.Cia;
- exportar o imprimir informe;
- validar lineas;
- cerrar liquidacion;
- recalcular importes;
- generar liquidacion;
- anular o reabrir;
- crear factura/tesoreria asociada.

Para un primer incremento, la recomendacion seria solo read-only si producto desbloquea el modulo. Cualquier escritura debe quedar bloqueada hasta tener reglas transaccionales, auditoria, permisos, rollback y UAT.

## Permisos historicos

No se encontro metadata local suficiente de permisos historicos para `Liq.Col`.

Permisos iLiniumTech candidatos:

- `liqCol.catalogs`: catalogos de filtros confirmados.
- `liqCol.read`: listado read-only.
- `liqCol.detail`: detalle read-only.
- `liqCol.financial`: ver importes financieros completos.
- `liqCol.commissions`: ver comisiones, retenciones y liquidos.
- `liqCol.concepts`: ver conceptos de liquidacion.
- `liqCol.exclusions`: ver exclusiones y motivos.
- `liqCol.report`: generar o descargar informe.
- `liqCol.validate`: validar lineas, si alguna vez se aprueba escritura.
- `liqCol.close`: cerrar liquidacion, si alguna vez se aprueba escritura.
- `liqCol.write`: escritura general futura, bloqueada hasta SDD especifica.

Reglas:

- El backend debe autorizar antes de leer datos.
- El frontend solo refleja capacidades segun `/api/me`; no es barrera de seguridad.
- AppBuilder puede aportar evidencia historica, pero no motor runtime.
- Broker activo debe validarse contra brokers permitidos antes de resolver conexion.
- Errores 403/404 no deben revelar existencia de liquidaciones de otro broker.

## Propuesta Vue/API estatica

Esta propuesta no autoriza implementacion. Solo sirve como guia cuando producto desbloquee la pagina.

### Frontend

Rutas candidatas:

- `/liq-col`;
- `/liq-col/:id`.

Feature candidata:

- `iLiniumTech.Frontend/src/features/liq-col`.

Componentes estaticos candidatos:

- `LiqColView.vue`: listado de liquidaciones.
- `LiqColFilters.vue`: filtros confirmados por SDD/UAT.
- `LiqColTable.vue`: tabla minimizada.
- `LiqColDetailView.vue`: detalle read-only.
- `LiqColConceptsPanel.vue`: solo si se confirma area de conceptos.
- `LiqColDetailsPanel.vue`: solo si se confirma area de lineas/recibos.
- `LiqColExclusionsPanel.vue`: solo si se confirma area de exclusiones.
- `useLiqCol.ts`: estado de filtros, paginacion, permisos y llamadas API.
- `liqColTypes.ts`: contratos frontend.
- `liqColApi.ts`: cliente API explicito.

Estados obligatorios:

- cargando sesion;
- sin sesion;
- sin broker;
- sin permiso;
- sin permiso financiero;
- error backend;
- sin resultados;
- listado con paginacion;
- detalle no autorizado;
- detalle no encontrado sin revelar existencia cruzada.

Menu:

- `Liq.Col` debe seguir `disabled: true` hasta existir SDD, permiso y API.
- Permiso candidato de menu: `liqCol.read`.
- No debe tener hijos hasta confirmar submenus reales.

### Backend

Endpoints candidatos read-only:

- `GET /api/liq-col/catalogs`.
- `GET /api/liq-col`.
- `GET /api/liq-col/{id}`.
- `GET /api/liq-col/{id}/details`.
- `GET /api/liq-col/{id}/concepts`.
- `GET /api/liq-col/{id}/exclusions`.

Contratos candidatos:

- `LiqColSearchRequest`: filtros whitelisted, `page`, `pageSize`, `sort`.
- `LiqColListItem`: identificador estable, colaborador minimizado, fecha, cierre, estado, totales minimizados segun permisos.
- `LiqColDetail`: cabecera y secciones estaticas autorizadas.
- `LiqColDetailLine`: recibo/poliza/ramo/importes minimizados.
- `LiqColConcept`: concepto e importe segun permiso.
- `LiqColExclusion`: motivo minimizado o redaccionado segun permiso.
- `LiqColCatalogs`: estados, oficinas, ramos u otros catalogos confirmados.

Reglas backend obligatorias:

- API explicita, no endpoint generico de metadata.
- Validar broker antes de resolver conexion o consultar SQL.
- Usar parametros y whitelists.
- No usar `SELECT *`.
- Aplicar `SESSION_CONTEXT` solo con valores autenticados y limpiar por request.
- Sanitizar errores y devolver `correlationId`.
- No devolver PII, documentos, motivos libres o importes sensibles sin permiso.
- No proyectar nombres internos AppBuilder como contrato publico.

## Que no debe replicarse

- No replicar AppBuilder como runtime dinamico.
- No construir pantalla desde `NombreTablasConst`, EF metadata o tablas heredadas.
- No crear endpoints genericos de datasource, componente, query, menu o accion.
- No ejecutar SQL heredado no revisado.
- No implementar validacion, cierre, recalculo, generacion o anulacion sin SDD.
- No exponer documento de colaborador por defecto.
- No exponer comisiones, retenciones o liquido sin permiso.
- No mostrar motivos libres sin revisar riesgo PII.
- No confiar en API key, headers MVP o `DemoSession` como seguridad productiva.
- No aceptar broker desde header libre como autoridad.
- No registrar SQL completo, connection strings, cookies, tokens, documentos, emails, telefonos o direcciones.

## Riesgos

Riesgos funcionales:

- Confundir `Liq.Col` con una pantalla confirmada cuando la evidencia local solo prueba dominio de datos.
- Implementar reglas de cierre/validacion/reparto sin conocer negocio.
- Mezclar `Liq.Cia` y `Liq.Col` sin frontera funcional.
- Interpretar incorrectamente comision, liquido, retencion o impuestos.
- Presentar como cerrada una liquidacion que en negocio tiene estados adicionales.

Riesgos de datos:

- Exposicion de PII de cliente, colaborador o mediador.
- Exposicion de documentos identificativos.
- Exposicion de comisiones o importes sensibles.
- Exposicion de motivos libres con datos personales.
- Lectura cruzada entre brokers.
- Uso de vistas no autorizadas para produccion.

Riesgos tecnicos:

- Reintroducir AppBuilder por la puerta trasera usando metadata EF o constantes de tabla.
- Reusar SQL dinamico o vistas heredadas sin whitelist.
- Crear dependencia circular con `Recibos`, `Polizas`, `Clientes` o `Liq.Cia`.
- Implementar escrituras en tablas con triggers sin transacciones y auditoria.
- Contaminar `SESSION_CONTEXT` por pooling.

Riesgos de seguridad:

- 403/404 que revelen existencia de liquidaciones de otro broker.
- Logs con datos financieros o personales.
- Permisos aplicados solo en frontend.
- Importes o documentos visibles mediante exportacion aunque esten ocultos en UI.

## Pruebas necesarias para desarrollo futuro

Backend:

- `GET /api/liq-col/catalogs` exige sesion/permisos.
- `GET /api/liq-col` exige `liqCol.read`.
- `GET /api/liq-col/{id}` exige `liqCol.detail`.
- se rechaza broker ausente;
- se rechaza broker no permitido antes de SQL;
- sort/filtros fuera de whitelist se rechazan;
- payloads maliciosos no alteran SQL;
- PII/documentos/comisiones no se devuelven sin permiso;
- 401/403/404 no revelan existencia cruzada;
- errores publicos incluyen `correlationId`;
- `SESSION_CONTEXT` se establece y limpia por request si hay SQL real.

Frontend:

- menu `Liq.Col` visible/activo solo cuando exista permiso y feature desbloqueada;
- ruta protegida muestra estado seguro si no hay sesion;
- filtros confirmados se inicializan, limpian y conservan;
- paginacion y ordenacion no pierden filtros;
- detalle conserva vuelta al listado;
- estados loading/empty/error/no permission/no broker/no financial permission;
- no aparecen `AppBuilder`, `QueryStatic`, connection strings, SQL ni metadata en DOM.

QA/UAT:

- columnas y filtros confirmados por responsable funcional;
- reglas de importes, cierre, validacion y exclusiones confirmadas;
- comparativa controlada con AppBuilder o BBDD autorizada sin versionar datos reales;
- smoke visual desktop/mobile si se implementa UI;
- secret scan limpio;
- dependency audit sin findings bloqueantes;
- CORS audit si toca API/configuracion.

## Bloqueos y preguntas abiertas

Bloqueos externos:

- Falta metadata sanitizada de la pantalla real `Liq.Col`.
- Falta confirmar si `Liq.Col` es modulo independiente o vista/reporting vinculado a colaboradores/recibos/Liq.Cia.
- Falta matriz real de permisos.
- Falta validar origen SQL autorizado: tablas de liquidacion, vistas de reporting o vistas read-only especificas.
- Falta confirmar campos sensibles y reglas de minimizacion.
- Falta UAT owner.
- Falta decision de auth productiva.
- Falta confirmar reglas de cierre, validacion, recalculo, exclusiones y relacion con tesoreria.

Preguntas para producto/DBA:

- Que significa exactamente `Liq.Col` para el usuario final: liquidacion de colaborador, mediador, comercial u otra identidad?
- La pantalla debe ser read-only o incluye generacion, cierre, validacion o recalculo?
- Cual es el listado principal y cuales son sus columnas imprescindibles?
- Que filtros usa negocio diariamente?
- Que importes puede ver cualquier usuario y cuales requieren permiso?
- Deben mostrarse documentos de colaborador o cliente? Con que mascara?
- Que relacion debe tener con `Liq.Cia`, `Recibos`, `Polizas`, `Clientes` y tesoreria?
- Que vista o tabla recomienda DBA para lectura read-only por broker?
- Como se debe resolver el estado funcional de una liquidacion?
- Que comprobacion UAT permite decir que la pantalla esta correcta?

## Decision de subagentes de componentes

No se crearon subagentes de componentes en esta ronda.

Motivo: no se detectaron pestanas, submenus o componentes internos reales y especificos de `Liq.Col`. Los elementos encontrados son entidades, vistas y relaciones de dominio; no describen una UI AppBuilder concreta.

Si en una ronda posterior aparece metadata sanitizada con estructura real de pagina, se recomienda crear subagentes por cada area confirmada, por ejemplo:

- busqueda/listado;
- cabecera;
- lineas/recibos liquidados;
- conceptos;
- exclusiones;
- relacion Liq.Cia;
- informes/exportacion;
- acciones de validacion/cierre.

Cada subagente debera escribir solo en `docs/appbuilder/pages/liq-col/components/*.md` y no programar durante la fase documental.

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
rg -n -i "Liq\.Col|LiqCol|Liq Col|Liquidacion Colect|Liquidaciones Colect|Liq_Col" C:\Desarrollo\AppBuilder ...
rg -n -i "liquidacion|liquidaciones|liquidar|liq" C:\Desarrollo\AppBuilder ...
rg -n -i "vw_Recibo_LiqCol|vw_rpt_LiquidacionColaborador|LiquidacionIdentidad|LiquidacionColaborador|LiqCol" C:\Desarrollo\AppBuilder ...
Get-Content -Raw C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts
Get-Content -Raw C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts
Get-Content -Raw C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts
Get-Content -Raw C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts
```

Pendiente de ejecutar al cierre de escritura documental:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`.
- `git diff --check`.
- `git status --short --branch`.

## Clasificacion de pendientes

Completado con evidencia:

- entrada `Liq.Col` localizada en menu iLiniumTech;
- inventario documental de pagina localizado;
- constantes de dominio de liquidacion de colaborador localizadas;
- tablas/vistas EF relacionadas localizadas;
- relaciones con recibos, acuerdos de reparto, Liq.Cia, broker, oficina y tesoreria identificadas;
- no hay submenus ni pestanas especificas confirmadas localmente;
- no se ejecuto SQL ni extractor `Live`.

Pendiente tecnico:

- crear SDD de `Liq.Col` si producto prioriza el modulo;
- definir contrato API estatico;
- definir feature Vue estatica;
- definir permisos reales;
- definir tests backend/frontend;
- actualizar menu cuando haya permiso y ruta real.

Bloqueado externo:

- metadata real de pantalla;
- UAT funcional;
- regla DBA de lectura;
- matriz de permisos;
- auth productiva;
- clasificacion PII/importes/comisiones;
- reglas de cierre/validacion/recalculo.

## Readiness reporting/liquidaciones 2026-05-18

Actualizacion documental sin cambios de aplicacion. Esta seccion contrasta la evidencia historica anterior con el estado actual de la feature Vue protegida `iLiniumTech.Frontend/src/features/liquidaciones-colaborador`.

### Estado actual detectado

- Existe superficie frontend estatica para `/liq-col` con fixture local, filtros locales, tabla paginada y acciones sensibles deshabilitadas.
- La pantalla se declara `Solo lectura`, `Fixture local sin API`, `Datos minimizados y sanitizados`, `Detalle y conceptos no operativos` y `Comisiones y liquidos bloqueados`.
- El listado usa referencias, colaborador anonimizado, oficina, estado, fecha de liquidacion, cierre, resumen de recibos, resultado y alcance; no proyecta comisiones, liquidos, retenciones, documentos ni banco.
- Los tests de `LiquidacionesColaboradorView` comprueban render read-only, filtros locales, empty state, acciones bloqueadas y ausencia de marcadores AppBuilder, SQL, secretos, documentos, banco o importes reales.
- No hay API backend, SDD funcional aprobada para datos reales, permisos productivos ni UAT/DBA que autoricen lectura de comisiones.

### Acciones y exportaciones bloqueadas

- Detalle de liquidacion.
- Conceptos.
- Exportacion.
- Guardado de busqueda.
- Documentos, datos bancarios, comisiones, retenciones, liquidos, exclusiones, validacion, cierre, recalculo o cualquier escritura.

### Riesgos a mantener visibles

- Exposicion de comisiones, retenciones, liquidos, impuestos o acuerdos de reparto.
- PII de colaborador, mediador, cliente o documento identificativo.
- Motivos libres de exclusion/validacion con datos personales.
- Lectura cruzada entre brokers, oficinas o colaboradores.
- Exportaciones que incluyan importes o documentos aunque la UI los oculte.
- Reglas de cierre, validacion y reparto no confirmadas por negocio.

### Permisos candidatos

- `liqCol.catalogs`: catalogos de filtros aprobados.
- `liqCol.read`: listado minimizado read-only.
- `liqCol.detail`: detalle read-only minimizado.
- `liqCol.financial`: importes financieros generales.
- `liqCol.commissions`: comisiones, retenciones y liquidos.
- `liqCol.concepts`: conceptos de liquidacion.
- `liqCol.exclusions`: exclusiones y motivos revisados.
- `liqCol.export`: exportacion/reporting.
- `liqCol.close`: cierre/reapertura, solo si una SDD futura lo autoriza.
- `liqCol.write`: permiso paraguas de escritura futura, bloqueado por defecto.

### Dependencias UAT/DBA

- UAT debe confirmar si `Liq.Col` es colaborador, mediador, comercial u otra identidad; columnas minimas; filtros diarios; reglas de cierre; y tratamiento de comisiones.
- DBA debe validar fuente read-only, claves estables, relacion con `Liq.Cia`, `SESSION_CONTEXT`, aislamiento por broker, indices, whitelists y campos sensibles.
- Seguridad debe aprobar minimizacion de documentos, motivos libres, importes, exportaciones y logging.

### Tareas futuras pequenas

- Revisar y aprobar `SDD-2026-018 Liq.Col read-only financiero minimizado` con producto, DBA y seguridad.
- Separar permisos de lectura general y lectura de comisiones.
- Inventariar filtros candidatos y descartes con UAT.
- Proponer pruebas 401/403/tenant para `catalogs`, listado y detalle futuro.
- Mantener fixture Vue como superficie de conversacion, no como contrato de datos reales.

### Criterios de aceptacion para desbloquear

- SDD aprobada que diga expresamente que datos de colaborador, comisiones y liquidos se pueden leer y bajo que permisos.
- UAT owner y DBA owner asignados.
- API explicita sin metadata AppBuilder ni SQL heredado libre.
- Pruebas de 401/403, broker no permitido, filtros whitelist, minimizacion de PII/importes y errores sanitizados.
- Exportacion, cierre, recalculo, validacion y escrituras siguen bloqueadas salvo SDD especifica posterior.
