# Pagina AppBuilder: Recibos

Fecha: 2026-05-16

Rol IA: jefe de pagina `Recibos`.

Ronda: documentacion y analisis. No se programa, no se ejecuta SQL, no se ejecuta extractor `Live` y no se convierte metadata AppBuilder en contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Hay evidencia local suficiente para afirmar que `Recibos` existe como opcion historica de gestion/busqueda y como dominio de datos (`Recibo`, `PBI_Recibos`, vistas auxiliares), pero no hay evidencia local suficiente de una pagina AppBuilder completa de `Recibos` con metadata de pantalla, layout, pestanas, submenus, acciones especificas, permisos historicos por grupo o contrato de UAT.

Actualizacion 2026-05-18: iLiniumTech ya contiene ruta protegida `/recibos` y vista Vue fixture/read-only con datos sanitizados, filtros locales, importes demo anonimizados y acciones de detalle/exportacion/cobro bloqueadas. Esa vista no autoriza API, datos reales, importes reales, datos bancarios, detalle, exportacion ni escritura sin SDD, permisos, DBA/UAT y revision de seguridad.

SDD draft abierta para el primer corte futuro: [`SDD-2026-009 Recibos read-only minimizado`](../../../sdd/specs/iLiniumTech/SDD-2026-009-recibos-read-only.md). La SDD limita el primer incremento a listado read-only minimizado y deja importes reales, banco, remesas, detalle, EIAC, exportacion y escrituras fuera de alcance.

Por tanto, el desarrollo de una pagina iLiniumTech de `Recibos` debe quedar bloqueado hasta obtener una de estas entradas:

- metadata AppBuilder sanitizada de la pantalla real de `Recibos`;
- SDD funcional aprobada por producto;
- UAT owner que confirme columnas, filtros, acciones, permisos y datos autorizados;
- validacion DBA de tablas/vistas, broker, permisos y campos sensibles.

## Fuentes revisadas

Repositorio iLiniumTech:

- `AGENTS.md`.
- `PLANS.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/appbuilder/pages/README.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- `docs/APPBUILDER_FLUJO_CONEXIONES_BROKER_POLIZAS.md`.
- `docs/appbuilder/polizas-detalle-analysis.md`.
- `docs/sdd/specs/iLiniumTech/SDD-2026-006-autos-particulares-mvp-read-only.md`.

Repositorio AppBuilder, solo lectura y sin abrir configuracion sensible:

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\localization\MessagesBusqueda.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\Search.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\SearchDetail.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\DetailCrud.vue`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\CrudTable.vue`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\Modelo\Repositorios\RepositorioBusqueda.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\MaestroDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Eiac\EiacDbContext.cs`.

Restricciones aplicadas:

- No se abrieron `appsettings`, archivos `.config`, connection strings, dumps, capturas, binarios ni datos personales reales.
- No se ejecuto SQL.
- No se ejecuto extractor `Live`.
- No se copio ningun secreto ni valor de infraestructura.

## Evidencia de menu y metadata

### iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Recibos` aparece como item de primer nivel:

- etiqueta: `Recibos`;
- icono: `pi pi-money-bill`;
- estado actual iLiniumTech: `fixture`;
- ruta: `/recibos`;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma que iLiniumTech ya tiene una superficie visible fixture/read-only para navegacion y pruebas. No existe aun contrato funcional con datos reales ni API propia.

### AppBuilder frontend

Evidencia localizada:

- `Messages.ts` contiene la etiqueta historica de menu `gestion.recibos`.
- `GestionConst.ts` define `RECIBO = 'REC'`.
- `TableIcons.ts` asocia `GestionConst.RECIBO` con tabla `Recibo` e icono `pi pi-ticket`.
- `MessagesBusqueda.ts` traduce `rec` como `Recibos` en la barra de busqueda generica.
- `NombreTablasConst.ts` declara:
  - `PBI_RECIBOS = "PBI_Recibos"`;
  - `RECIBO = "Recibo"`;
  - `SUPLEMENTORECIBO = "SuplementoRecibo"`;
  - vistas relacionadas de aduana, correo, poliza, cliente, liquidacion y reporting.

No se encontro en frontend una pagina Vue especifica llamada `Recibos`, ni ruta local especifica, ni metadata versionada de layout que describa pestanas o submenus propios de la pagina.

### AppBuilder backend

Evidencia localizada:

- `ModeloDbContext.cs` mapea `Recibo` como tabla y `PBI_Recibos` como vista.
- `MaestroDbContext.cs` tambien mapea `PBI_Recibos` como vista.
- `EiacDbContext.cs` mapea `Recibo` como vista y `EIACRecibo` como tabla EIAC.
- `RepositorioBusqueda.cs` contiene una busqueda generica que une `Recibo` con `Poliza`, `Identidad`, `Riesgo`, `Siniestro` y `Suplemento`. Es evidencia historica de busqueda amplia, no contrato valido para iLiniumTech.

No se encontro contrato API especifico de `Recibos` ni servicio de pagina que pueda migrarse directamente.

## Componente raiz e hijos

### Componente raiz historico inferido

La evidencia apunta a que `Recibos` se apoyaba en el sistema generico AppBuilder de busqueda/CRUD:

- componente de busqueda generica;
- selector de tipo de gestion por clave `REC`;
- tabla de resultados generica;
- detalle CRUD generico si el usuario abria un registro;
- configuracion dinamica por metadata no presente en los archivos locales revisados.

Esta inferencia se basa en las constantes de `GestionConst`, `TableIcons`, `MessagesBusqueda` y los componentes genericos bajo `crud/infrastructure/search`.

### Componentes hijos especificos de Recibos

No se detectaron componentes hijos propios de `Recibos` con evidencia local suficiente.

No se crean documentos `components/*.md` en esta ronda porque no se han identificado pestanas, submenus o componentes internos reales y especificos de la pagina `Recibos`.

Componentes genericos AppBuilder observados, no especificos de Recibos:

- `Search.vue`;
- `SearchDetail.vue`;
- `DetailCrud.vue`;
- `CrudTable.vue`;
- `SearchFields.vue`;
- `SaveSearch_*`;
- `NewRegister.vue`;
- `MasiveUpdate.vue`.

Estos componentes no deben replicarse como runtime dinamico. Solo pueden inspirar patrones estaticos revisados si una SDD posterior lo aprueba.

## Pestanas, submenus y areas internas

Estado: no confirmado.

No hay evidencia local de:

- pestanas especificas de `Recibos`;
- submenu bajo `Recibos`;
- rutas hijas de `Recibos`;
- acciones agrupadas por pestana;
- layout propio de detalle de recibo;
- vista de remesas como pestana;
- vista de EIAC como pestana;
- vista de liquidaciones como pestana;
- vista de incidencias/devoluciones como pestana.

Areas candidatas por dominio de datos, pendientes de confirmar:

- listado/busqueda de recibos;
- detalle read-only de recibo;
- relacion con poliza;
- relacion con cliente;
- situacion del recibo;
- fechas de efecto, vencimiento y cobro;
- importes y comisiones;
- canal/gestor de cobro;
- remesas o cuenta bancaria;
- EIAC/conciliacion;
- liquidaciones de compania/colaborador;
- incidencias, duplicados o aduana.

Estas areas son hipotesis de producto, no decisiones de desarrollo.

## Datos detectados

### Entidades y vistas candidatas

Entidades/vistas encontradas en constantes y EF:

- `Recibo`.
- `PBI_Recibos`.
- `EIACRecibo`.
- `vw_ClienteRecibos`.
- `vw_Pol_UltimoRecibo`.
- `vw_Pol_UltimoReciboLiquidado`.
- `vw_Recibo_LiqCia`.
- `vw_Recibo_LiqCol`.
- `vw_RecibosCobroMediador`.
- `vw_Correo_Recibo`.
- `vw_rpt_Recibo`.
- `vw_Aduana_Recibo`.
- `vw_Aduana_Recibo_AcuerdoIngresoDiferencias`.
- `vw_Aduana_ReciboCobradoMediador`.
- `vw_Aduana_ReciboIncidencias`.
- `vw_Aduana_RecibosDuplicados`.
- `SuplementoRecibo`.

### Campos observados

Campos repetidos en `Recibo`, `PBI_Recibos`, `vw_ClienteRecibos`, vistas de aduana y EIAC:

- identificadores: `Id`, `BrokerId`, `BrokerReciboId`, `BrokerPolizaId`, `BrokerClienteId`, `PolizaId`, `ReciboIdAnterior`;
- poliza: `Poliza`, `Ramo`, `SubRamo`, `SituacionPoliza`;
- recibo: `ReciboCia`, `Tipo`, `IdTipoRecibo`, `IdTipoReciboCia`, `Situacion`, `SituacionRecibo`;
- fechas: `F_Efecto`, `F_Vencimiento`, `F_Cobro`, `F_CobroMediador`, `F_Estado`, `F_Validado`, `F_Anulacion`;
- importes: `PrimaNeta`, `PrimaTotal`, `Comision`, `Comision2`, `Consorcio`, `IPS`, `OtrosGastos`, `OtrosImpuesto`;
- cobro: `CanalCobro`, `IdCanalCobro`, `GestorCobro`, `IdGestor`, `FraccionPago`;
- organizacion: `Oficina`, `Corredor`, `Colaboradores`, `Tecnico`;
- compania/cliente: `CiaDgs`, `CiaRazonSocial`, `Cliente`;
- EIAC/aduana: `EIAC`, `UValidado`, `Observaciones`, `TipoDiferencia`, `PctAplicado`, `PctEsperado`.

### Campos sensibles o de riesgo

El dominio de recibos puede exponer PII, datos bancarios y datos financieros. Deben minimizarse antes de cualquier implementacion:

- cliente/persona: `Cliente`, `Titular`, `NumDocumento`, `IdentidadNumDocumento`, direccion, poblacion, provincia, telefono o email si aparecen en vistas relacionadas;
- banco/cobro: `Cuenta`, `NumCuenta`, `CuentaBancariaId`, `IBAN` o equivalentes si aparecen;
- importes: primas, comisiones, impuestos, liquidaciones;
- datos de compania y mediador;
- observaciones libres;
- identificadores internos y legacy.

Regla recomendada: listado con minimizacion estricta; detalle ampliado solo con permisos especificos y backend como autoridad.

## Filtros detectados o inferidos

No se encontro metadata local de filtros AppBuilder de la pagina `Recibos`.

Filtros candidatos por evidencia de campos:

- busqueda libre por recibo compania y poliza;
- situacion del recibo;
- tipo de recibo;
- fechas de efecto, vencimiento y cobro;
- compania;
- ramo/subramo;
- gestor/canal de cobro;
- cliente;
- broker;
- importes por rango;
- estado EIAC/validacion/incidencias si se incluye esa area.

Estos filtros requieren confirmacion funcional. No deben implementarse automaticamente desde nombres de campo heredados.

## Acciones historicas

No se encontro metadata local de acciones especificas para `Recibos`.

Acciones genericas observadas en AppBuilder:

- buscar;
- limpiar filtros;
- cerrar pestanas;
- guardar busqueda;
- nuevo registro;
- importar;
- guardar;
- actualizacion masiva.

Estas acciones pertenecen al motor generico CRUD/busqueda de AppBuilder. No deben migrarse por defecto.

Acciones de negocio candidatas que deben tener SDD propia si se priorizan:

- ver detalle de recibo;
- exportar listado;
- ver poliza asociada;
- ver cliente asociado;
- ver liquidaciones asociadas;
- ver remesa/cobro;
- ver incidencia/devolucion;
- imprimir o generar documento de recibo;
- reenviar comunicacion;
- modificar situacion/cobro;
- conciliacion EIAC.

Para MVP inicial de `Recibos`, la recomendacion es empezar solo con read-only: listado, filtros confirmados y detalle minimizado.

## Permisos historicos

No se encontro metadata local suficiente de permisos historicos por grupo, perfil o directiva para `Recibos`.

Permisos iLiniumTech candidatos:

- `recibos.catalogs`: catalogos de filtros confirmados.
- `recibos.read`: listado read-only.
- `recibos.detail`: detalle read-only.
- `recibos.financial`: ver importes/comisiones no minimizados.
- `recibos.bank`: ver datos bancarios o de remesa, si alguna vez se aprueba.
- `recibos.eiac`: ver estados EIAC/conciliacion.
- `recibos.export`: exportar resultados.
- `recibos.write`: escritura futura, bloqueada hasta SDD especifica.

Reglas:

- AppBuilder puede servir como evidencia de permisos historicos, nunca como motor runtime.
- El backend debe autorizar antes de leer datos.
- El frontend solo oculta o muestra capacidades segun `/api/me`, pero no es barrera de seguridad.
- Broker activo debe validarse contra brokers permitidos antes de resolver conexion o consultar datos.

## Propuesta Vue/API estatica

Esta propuesta no es autorizacion para programar; es una guia si producto desbloquea la pagina.

### Frontend

Ruta candidata:

- `/recibos`.
- `/recibos/:id`.

Feature candidata:

- `iLiniumTech.Frontend/src/features/recibos`.

Componentes estaticos candidatos:

- `RecibosView.vue`: pagina de listado.
- `RecibosFilters.vue`: filtros confirmados por SDD/UAT.
- `RecibosTable.vue`: tabla con columnas minimizadas.
- `ReciboDetailView.vue`: detalle read-only con secciones estaticas.
- `useRecibos.ts`: estado de filtros, paginacion, errores, permisos y llamadas API.
- `recibosTypes.ts`: contratos frontend.
- `recibosApi.ts`: cliente API explicito.

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

- `Recibos` debe pasar de `disabled: true` a ruta solo cuando exista SDD y endpoint.
- Permiso candidato: `recibos.read`.
- No debe tener hijos hasta confirmar submenus reales.

### Backend

Endpoints candidatos:

- `GET /api/recibos/catalogs`.
- `GET /api/recibos`.
- `GET /api/recibos/{id}`.

Contratos candidatos:

- `RecibosSearchRequest`: filtros whitelisted, `page`, `pageSize`, `sort`.
- `ReciboListItem`: identificador estable, recibo compania, poliza, situacion, fechas, prima total minimizada si procede, canal/gestor si procede.
- `ReciboDetail`: secciones estaticas y minimizadas segun permisos.
- `RecibosCatalogs`: situaciones, tipos, canales, gestores u otros catalogos confirmados.

Backend debe:

- usar API explicita, no endpoint generico de metadata;
- validar broker antes de SQL;
- usar parametros y whitelists;
- aplicar `SESSION_CONTEXT` solo con valores autenticados;
- sanitizar errores y devolver `correlationId`;
- no proyectar PII/banco/comisiones sin permiso;
- no exponer SQL, nombres internos sensibles ni connection strings.

## Que no debe replicarse

- No replicar AppBuilder como runtime dinamico.
- No consumir `PBI_Recibos`, `Recibo` o metadata para construir pantalla en vuelo.
- No exponer endpoints genericos de datasource, componente, query o menu.
- No ejecutar SQL heredado de busqueda generica.
- No migrar botones genericos (`Nuevo`, `Importar`, `Actualizacion masiva`, `Guardar`) sin SDD.
- No mostrar datos bancarios ni personales por defecto.
- No aceptar broker desde header libre como autoridad.
- No confiar en API key o `DemoSession` como seguridad productiva.
- No usar `SELECT *`.
- No registrar SQL completo, cookies, tokens, cuentas, documentos, telefonos, emails o direcciones.

## Riesgos

Riesgos funcionales:

- Confundir `Recibos` como pantalla independiente cuando en AppBuilder podia ser busqueda generica o pestaña de cliente/poliza.
- Implementar filtros no validados por negocio.
- Presentar importes o estados con interpretacion incorrecta.
- Mezclar recibos de poliza, EIAC, liquidaciones y remesas sin frontera funcional.

Riesgos de datos:

- Exposicion de PII del cliente.
- Exposicion de datos bancarios.
- Exposicion de importes/comisiones sin permiso.
- Lectura cruzada entre brokers.
- Uso de vistas no autorizadas para UAT o produccion.

Riesgos tecnicos:

- Reintroducir AppBuilder con otro nombre mediante un generador de pantallas.
- Reusar SQL dinamico heredado.
- Usar campos legacy como contrato API estable.
- Crear dependencias circulares con `Polizas`, `Clientes`, `Liq.Cia`, `Liq.Col` o `Siniestros`.

Riesgos de seguridad:

- Errores 403/404 que revelen existencia de recibos de otro broker.
- Logs con datos personales o bancarios.
- `SESSION_CONTEXT` contaminado por pooling.
- Permisos de frontend sin validacion backend.

## Pruebas necesarias para desarrollo futuro

Backend:

- `GET /api/recibos/catalogs` exige sesion/permisos.
- `GET /api/recibos` exige `recibos.read`.
- `GET /api/recibos/{id}` exige `recibos.detail`.
- broker ausente devuelve error seguro.
- broker no permitido no resuelve conexion ni consulta SQL.
- sort/filtros fuera de whitelist se rechazan.
- payloads maliciosos no alteran SQL.
- PII/banco/comisiones no se devuelven sin permiso.
- 401/403/404 no revelan existencia cruzada.
- errores publicos incluyen `correlationId` cuando aplique.

Frontend:

- menu `Recibos` visible/activo solo con permiso y feature desbloqueada.
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

- Falta metadata sanitizada de la pantalla real de `Recibos`.
- Falta confirmar si `Recibos` debe ser modulo independiente o vista vinculada a cliente/poliza.
- Falta matriz real de permisos para recibos.
- Falta validar origen SQL autorizado: tabla `Recibo`, vista `PBI_Recibos`, `vw_ClienteRecibos` u otra vista controlada.
- Falta confirmar campos sensibles y reglas de minimizacion.
- Falta UAT owner.
- Falta decision de auth productiva y permisos reales.

Preguntas para producto/DBA:

- Cual es el objetivo MVP de `Recibos`: busqueda general, detalle desde poliza/cliente, gestion de cobro, remesas, liquidaciones o EIAC?
- Que columnas son imprescindibles en listado?
- Que filtros usa negocio diariamente?
- Que datos financieros pueden ver todos los usuarios y cuales requieren permiso?
- Deben mostrarse datos bancarios? Si si, bajo que permiso y mascara?
- Que relacion debe tener con `Polizas`, `Clientes`, `Liq.Cia`, `Liq.Col` y `Siniestros`?
- Que estados de recibo son funcionalmente relevantes?
- Debe existir escritura o sera read-only?
- Que vista o tabla recomienda DBA para lectura read-only por broker?

## Decision de subagentes de componentes

No se crearon subagentes de componentes en esta ronda.

Motivo: no se detectaron pestanas, submenus o componentes internos reales y especificos de `Recibos`. Los componentes localizados son genericos de busqueda/CRUD AppBuilder y no constituyen componentes de producto iLiniumTech.

Si en una ronda posterior aparece metadata sanitizada con estructura real de la pagina, se recomienda crear subagentes por cada area confirmada, por ejemplo:

- listado/busqueda;
- detalle de recibo;
- remesas/cobro;
- EIAC/conciliacion;
- liquidaciones;
- comunicaciones/documentos;
- incidencias/devoluciones.

Cada subagente debera escribir solo en `docs/appbuilder/pages/recibos/components/*.md` y no programar durante la fase documental.

## Evidencia de comandos

Comandos ejecutados en modo lectura/documentacion:

```powershell
git status --short --branch
rg --files docs\appbuilder\pages
rg -n -i "recibos|recibo" docs ...
rg -n -i "\brecibos?\b|recibos" C:\Desarrollo\AppBuilder ...
rg -n -i "PBI_RECIBOS|PBI_Recibos|GestionConst\.RECIBO|RECIBO|recibos" C:\Desarrollo\AppBuilder\src\frontend ...
rg -n -i "PBI_RECIBOS|PBI_Recibos|GestionConst\.RECIBO|RECIBO|recibos" C:\Desarrollo\AppBuilder\src\backend ...
```

Validacion ejecutada al cierre de escritura documental:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`: OK.
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`: OK, no leaks.
- `git diff --check`: OK.
- `git status --short --branch`: revisado; rama `codex/static-polizas-data-api`, cambios documentales sin commit.

## Clasificacion de pendientes

Completado con evidencia:

- entrada `Recibos` localizada en menu iLiniumTech;
- etiquetas/localizacion AppBuilder localizadas;
- clave de gestion `REC` localizada;
- tabla/vista `Recibo` y `PBI_Recibos` localizadas;
- vistas relacionadas localizadas;
- no hay submenus ni pestanas especificas confirmadas localmente;
- no se ejecuto SQL ni extractor `Live`.

Pendiente tecnico:

- crear SDD de `Recibos` si producto prioriza el modulo;
- definir contrato API estatico;
- definir feature Vue estatica;
- definir tests backend/frontend;
- actualizar menu cuando haya permiso y ruta real.

Bloqueado externo:

- metadata real de pantalla;
- UAT funcional;
- regla DBA de lectura;
- matriz de permisos;
- auth productiva;
- clasificacion PII/banco/importes.

## Readiness Operativa seguros 2026-05-18

Estado actual:

- Frontend fixture protegido en `/recibos`, con `RecibosView.vue` y `RecibosView.test.ts`.
- SDD draft relacionada: `SDD-2026-009 Recibos read-only minimizado`.
- Carril permitido ahora: documentacion, fixture read-only, pruebas de acciones bloqueadas y preparacion de UAT/DBA.
- Carril bloqueado: API real, importes reales, detalle, cobro, remesas, datos bancarios, EIAC, exportacion y escrituras.

Patrones detectados en el frontend actual:

- `AppShell` compartido.
- Fixtures locales anonimizados y sin importes reales.
- Filtros locales por recibo/cliente, poliza, situacion, tipo y vencimiento desde.
- Tabla con importes demo anonimizados, paginacion, empty state y acciones bloqueadas.
- Texto accesible `recibos-blocked-actions` enlazado con botones y campos restringidos.
- Tests que verifican render, filtrado, limpieza, empty state, acciones bloqueadas y ausencia de marcadores runtime/secretos.

Bloqueos de PII, finanzas y workflows:

- No exponer importes reales, primas, comisiones, impuestos, liquidaciones, diferencias ni filtros por importe.
- No exponer cuenta, IBAN, mandato, remesa, titular bancario ni datos de cobro sensibles.
- No exponer documento, telefono, email, direccion, observaciones, incidencias ni comunicaciones.
- No activar cobro, anulacion, conciliacion EIAC, documentos, exportacion ni escritura sin SDD posterior.

Permisos candidatos:

- Primer corte: `recibos.catalogs`, `recibos.read`.
- Posteriores: `recibos.detail`, `recibos.financial`, `recibos.bank`, `recibos.eiac`, `recibos.export`.
- Escritura bloqueada: `recibos.write` o permisos especificos por cobro/anulacion/remesa si producto los aprueba.

Dependencias UAT/DBA:

- Confirmar columnas y filtros minimos del listado.
- Confirmar origen SQL autorizado y regla de broker/tenant.
- Confirmar que `recibos.read` no devuelve importes reales.
- Confirmar catalogos de situacion, tipo y canal.
- Confirmar si `Recibos` es modulo independiente, vista desde Polizas/Clientes o ambas cosas.

Tareas futuras pequenas:

- `producto-recibos-uat-columns`: cerrar columnas/filtros sin importes reales.
- `security-recibos-finance-review`: clasificar importes, banco, remesas y EIAC.
- `backend-recibos-readonly-contract`: preparar DTOs, query y validadores whitelisted.
- `frontend-recibos-fixture-hardening`: mantener acciones financieras bloqueadas y revisar accesibilidad.

Criterios de aceptacion del siguiente incremento:

- SDD revisada y UAT/DBA desbloqueados antes de datos reales.
- Sin runtime AppBuilder ni SQL heredado.
- Backend valida sesion, permiso y broker antes de consultar.
- Listado no proyecta banco, PII ni importes reales.
- Tests backend/frontend y auditorias aplicables documentados.
