# Pagina AppBuilder: Informes

Fecha: 2026-05-16

Rol IA: jefe de pagina `Informes`.

Ronda: solo documentacion. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se convierte metadata AppBuilder en contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Hay evidencia local de capacidades transversales relacionadas con informes/reporting en AppBuilder:

- vistas de datos con prefijo `vw_rpt_*`;
- vista `Poliza_InformeVigor`;
- tipo de dato `tipodato-rlink` para enlaces de lanzador de informes;
- configuracion `appconfig-template-urlreportlauncher`;
- boton generico de PDF dentro del grid de busqueda AppBuilder.

Pero no hay evidencia local suficiente para afirmar que existe una pagina AppBuilder concreta de menu `Informes` con `menuId`, `componentId`, componente raiz, hijos, tabs, submenus, datasource, filtros, permisos historicos o acciones propias.

Por tanto, no se debe desarrollar una pantalla iLiniumTech de `Informes` hasta obtener al menos una de estas entradas:

- metadata AppBuilder sanitizada de la pantalla real de `Informes`;
- SDD funcional aprobada por producto;
- inventario validado de informes disponibles, parametros, permisos, formatos y propietarios;
- validacion DBA/UAT sobre vistas, campos sensibles, broker, retencion y auditoria;
- decision de arquitectura sobre generacion/descarga de documentos y si se usa o no un launcher externo.

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
- `docs/appbuilder/pages/recibos/README.md`, como referencia de estilo y evidencias de vistas `vw_rpt_*`.
- Busquedas locales en `docs/**`, `iLiniumTech.Frontend/**` e `iLiniumTech.Backend/**`.

Repositorio AppBuilder, solo lectura y sin abrir configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\catalog\domain\const\CatalogConfigurationConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\catalog\domain\const\CatalogDataTypeConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\CatalogConfigurationConst.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\CatalogDataTypeConst.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Aplicacion\AppBuilder.Aplicacion\Servicios\Builder\App\ServicioSearch.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.

Restricciones aplicadas:

- No se abrieron `appsettings`, archivos `.config`, connection strings, dumps, capturas, binarios ni datos personales reales.
- No se ejecuto SQL.
- No se ejecuto extractor `Live`.
- No se copiaron secretos, URLs reales de launcher, credenciales ni valores de infraestructura.

## Evidencia de menu y metadata

### iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Informes` aparece como item de primer nivel:

- etiqueta: `Informes`;
- icono: `pi pi-file`;
- estado: `disabled: true`;
- ruta: no definida;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma que existe un hueco de navegacion objetivo, pero no una pantalla estatica ya desarrollada.

En `docs/appbuilder/pages/README.md`, `Informes` esta incluido en el inventario de paginas con estado inicial `Pendiente de evidencia metadata local`.

### AppBuilder frontend

No se encontro etiqueta historica `Informes` en la localizacion de menus revisada. `Messages.ts` contiene opciones de gestion como `Agenda`, `Clientes`, `Polizas`, `Recibos`, `Riesgos`, `Suplementos` y `Siniestros`, pero no confirma una entrada `Informes`.

`GestionConst.ts` define claves de busqueda/gestion para:

- `CLI`;
- `POL`;
- `REC`;
- `SIN`;
- `RIE`;
- `SIP`.

No se encontro una clave equivalente para `Informes`.

`TableIcons.ts` asocia iconos a entidades de gestion, pero no contiene entrada de `Informes`.

Evidencia transversal de reporting:

- `CatalogDataTypeConst.ts` define `RPTLAUNCHERLINK = 'tipodato-rlink'`.
- `CatalogConfigurationConst.ts` define `URLREPORTLAUNCHER = 'appconfig-template-urlreportlauncher'`.
- `SearchDetail.vue` renderiza un boton con icono `pi pi-file-pdf` cuando una columna tiene tipo `RPTLAUNCHERLINK`.
- `SearchDetail.vue` abre una ventana nueva concatenando la URL configurada del launcher con un parametro `query`.

Esta evidencia indica que AppBuilder puede mostrar enlaces de informes dentro de resultados de busqueda genericos. No confirma una pagina independiente `Informes`.

### AppBuilder backend

Evidencia transversal de reporting:

- `CatalogDataTypeConst.cs` define `RPTLAUNCHERLINK = "tipodato-rlink"`.
- `CatalogConfigurationConst.cs` define `URLREPORTLAUNCHER = "appconfig-template-urlreportlauncher"`.
- `ServicioSearch.cs` detecta campos `tipodato-rlink` y cifra el valor antes de enviarlo al frontend.
- `ModeloDbContext.cs` mapea varias vistas `vw_rpt_*` y `Poliza_InformeVigor`.

No se encontro contrato API especifico de `Informes`, servicio de pagina ni componente raiz de menu que pueda migrarse directamente.

## Componente raiz e hijos

### Componente raiz

No identificado.

No hay evidencia local suficiente de:

- `menuId` historico de `Informes`;
- `componentId` raiz;
- nombre de componente AppBuilder;
- datasource principal;
- relacion raiz-hijo;
- `QueryStatic` o configuracion sanitizada asociada a una pantalla `Informes`.

### Componentes hijos

No identificados.

No se crean documentos `components/*.md` en esta ronda porque no se han detectado pestanas, submenus o componentes internos reales y especificos de `Informes`.

Piezas AppBuilder observadas, pero genericas y no especificas de `Informes`:

- `SearchDetail.vue` para grids genericos;
- `RPTLAUNCHERLINK` para columnas que abren informes;
- `URLREPORTLAUNCHER` como configuracion global de launcher;
- vistas `vw_rpt_*` como fuentes de datos de reporting;
- cifrado de parametros en `ServicioSearch.cs`.

Estas piezas no deben replicarse como runtime dinamico. Solo pueden servir como evidencia para una SDD y para diseno de APIs explicitas.

## Pestanas, submenus y areas internas

Estado: no confirmado.

No hay evidencia local de:

- pestanas especificas de `Informes`;
- submenu bajo `Informes`;
- categorias historicas de informes;
- buscador de informes;
- listado de informes favoritos o recientes;
- historial de ejecuciones;
- descarga de PDF/Excel como pagina propia;
- parametros por informe;
- rutas hijas;
- permisos por categoria.

Areas candidatas por nombres de vistas detectadas, pendientes de confirmar:

- informes de polizas;
- informes de recibos;
- informes de riesgos;
- informes de liquidacion de compania;
- informes de liquidacion de colaborador;
- informes de facturas;
- informes de mandato SEPA;
- informes de remesas;
- informes de cliente-polizas;
- informe de vigor de polizas.

Estas areas son hipotesis tecnicas derivadas de nombres de vistas, no decisiones funcionales.

## Datos detectados

Vistas y fuentes candidatas localizadas en constantes y EF:

- `Poliza_InformeVigor`.
- `vw_rpt_ClienteInformePolizas`.
- `vw_rpt_Factura`.
- `vw_rpt_FacturaDetalleLiquido`.
- `vw_rpt_LiquidacionColaborador`.
- `vw_rpt_LiquidacionColaboradorConcepto`.
- `vw_rpt_LiquidacionCompania`.
- `vw_rpt_LiquidacionCompaniaConcepto`.
- `vw_rpt_MandatoSEPA`.
- `vw_rpt_Recibo`.
- `vw_rpt_RemesaDetalle`.
- `vw_rpt_RiesgoPoliza`.

Datos sensibles probables:

- identidad de cliente, tomador, asegurado, colaborador o compania;
- documentos legales, telefonos, emails y direcciones si las vistas los proyectan;
- polizas, recibos, riesgos, remesas y mandatos;
- importes, primas, comisiones, liquidaciones, facturas e impuestos;
- datos bancarios o SEPA;
- observaciones libres o textos de documentos;
- identificadores internos legacy.

Regla recomendada: no exponer ninguna vista `vw_rpt_*` directamente en iLiniumTech. Si se implementa un informe, el backend debe definir DTO, parametros, whitelists, permisos, minimizacion de PII y auditoria por caso de uso.

## Filtros y parametros

No se encontro metadata local de filtros o parametros de la pagina `Informes`.

Parametros candidatos que requeririan SDD y UAT:

- rango de fechas;
- broker activo;
- oficina;
- compania;
- colaborador;
- ramo/subramo;
- situacion;
- poliza;
- recibo;
- cliente;
- formato de salida;
- idioma;
- incluir/excluir datos sensibles.

Ninguno de estos parametros debe implementarse automaticamente desde nombres de columnas heredadas.

## Acciones historicas

Acciones confirmadas a nivel generico AppBuilder:

- mostrar boton PDF en un grid cuando el campo es `tipodato-rlink`;
- abrir launcher externo usando `appconfig-template-urlreportlauncher`;
- cifrar o codificar el valor de consulta antes de entregarlo al frontend.

Acciones no confirmadas para una pagina `Informes`:

- listar informes;
- ejecutar informe;
- descargar PDF;
- descargar Excel;
- programar informes;
- enviar por email;
- guardar favoritos;
- ver historial;
- compartir enlace;
- administrar plantillas.

Estas acciones deben tener SDD propia, permisos propios y pruebas de seguridad antes de entrar en producto.

## Permisos

No se localizaron permisos historicos especificos de `Informes`.

Permisos iLiniumTech candidatos, no aprobados:

- `informes.read`: ver catalogo/listado de informes disponibles.
- `informes.execute`: ejecutar un informe con parametros.
- `informes.download`: descargar un resultado generado.
- `informes.admin`: administrar definiciones o permisos de informes.
- `informes.polizas`, `informes.recibos`, `informes.liquidaciones`: permisos por dominio si producto decide segmentar.

Regla recomendada: el backend debe ser la autoridad. La UI solo oculta o deshabilita acciones segun `/api/me`, pero la API debe aplicar 401/403, broker y permisos efectivos.

## Propuesta Vue/API estatica futura

No desarrollar en esta ronda.

Si producto aprueba SDD, la propuesta estatica seria:

Frontend Vue:

- ruta `/informes`;
- pagina `InformesView.vue` escrita como codigo fuente, no renderizada desde metadata;
- catalogo estatico o API explicita de informes aprobados;
- formularios de parametros tipados por informe;
- estados `loading`, `empty`, `error`, `sin permiso`, `sin broker` y `ejecucion en progreso`;
- descargas mediante enlaces backend de corta duracion, no por URL externa cruda.

Backend API:

- `GET /api/informes/catalogo` para informes permitidos al usuario;
- `POST /api/informes/{codigo}/ejecuciones` para solicitar generacion;
- `GET /api/informes/ejecuciones/{id}` para consultar estado;
- `GET /api/informes/ejecuciones/{id}/download` para descarga autorizada;
- validacion estricta de parametros por informe;
- whitelists de fuentes/columnas en codigo;
- broker y permisos aplicados antes de tocar datos;
- auditoria de ejecucion, descarga, usuario, broker y `correlationId`;
- errores publicos sanitizados.

No debe hacerse:

- endpoint generico que reciba nombre de vista, SQL, `QueryStatic`, datasource o metadata;
- UI que construya campos desde `IAP_Component` o `IAP_DataSource`;
- frontend que reciba URL de launcher y la abra sin validacion backend;
- descarga de informes con datos sensibles sin permisos y auditoria.

## Riesgos

Seguridad y datos:

- filtracion masiva de PII por exportaciones PDF/Excel;
- exposicion de importes, comisiones, liquidaciones o datos bancarios;
- generacion de informes para broker no permitido;
- uso de launcher externo como open redirect o canal de exfiltracion;
- parametros cifrados heredados tratados como autoridad;
- caching de documentos con datos de otro broker/usuario;
- logs con parametros sensibles;
- informes enviados por email sin control de destinatario;
- retencion de documentos generados sin politica.

Arquitectura:

- caer en un runtime generico de reporting basado en metadata;
- exponer vistas `vw_rpt_*` directamente como API;
- reutilizar `tipodato-rlink` como contrato productivo;
- acoplar iLiniumTech a la URL/configuracion de AppBuilder.

Producto/UAT:

- nombres de vistas no equivalen a informes de negocio aprobados;
- falta propietario funcional de cada informe;
- falta definir formato, campos, filtros y permisos;
- falta decidir si la pagina `Informes` es un modulo general o solo accesos contextuales desde otras pantallas.

## Pruebas necesarias para desarrollo futuro

Antes de implementar:

- SDD con catalogo de informes aprobados.
- Revision de seguridad de exportaciones/documentos.
- Inventario DBA de vistas/fuentes autorizadas y campos sensibles.
- UAT owner por informe.

Backend:

- anonimo devuelve 401;
- usuario sin permiso devuelve 403 con `correlationId`;
- broker no permitido no genera ni lista informes;
- parametros invalidos devuelven 400 sanitizado;
- no se aceptan nombres de vista, SQL, rutas, URLs ni codigos no whitelistados;
- descarga requiere usuario/broker/permisos vigentes;
- logs no contienen PII ni parametros sensibles completos;
- pruebas de aislamiento de `SESSION_CONTEXT` si aplica.

Frontend:

- ruta protegida por sesion;
- `Informes` visible solo si producto lo activa;
- acciones deshabilitadas si falta permiso;
- formulario de parametros valida antes de enviar;
- estados de ejecucion y error no filtran detalles internos;
- no se abre URL externa no aprobada.

QA/CI:

- `dotnet build`.
- `dotnet test`.
- `npm run format`.
- `npm run lint`.
- `npm run test:unit`.
- `npm run build`.
- `Invoke-SecretScan.ps1`.
- auditoria de dependencias y CORS si toca API/configuracion.
- smoke visual solo cuando exista pagina activa.

## Bloqueos

Bloqueos externos:

- no hay metadata local sanitizada de la pagina `Informes`;
- no hay `componentId` raiz ni relacion de hijos;
- no hay tabs/submenus verificados;
- no hay catalogo funcional de informes aprobado;
- no hay permisos historicos especificos;
- no hay contrato validado de launcher externo;
- no hay decision sobre generacion local, delegada o externa de documentos;
- no hay validacion DBA/UAT de vistas `vw_rpt_*`;
- no hay auth productiva final para permisos y auditoria de descargas.

Pendiente tecnico:

- decidir si `Informes` sera pagina propia o solo acciones contextuales desde Polizas/Recibos/Liquidaciones;
- crear SDD de `Informes` si se prioriza;
- definir permisos iLiniumTech definitivos;
- definir estrategia de minimizacion, mascara, descarga, auditoria y retencion.

Completado con evidencia:

- navegacion iLiniumTech revisada;
- busquedas locales en iLiniumTech y AppBuilder ejecutadas sin abrir configuracion sensible;
- evidencias transversales de reporting documentadas;
- ausencia de pagina concreta `Informes` documentada;
- subagentes/component docs no creados por falta de tabs/submenus reales.
