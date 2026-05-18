# Pagina AppBuilder: Siniestros

Fecha: 2026-05-16

Rol IA: jefe de pagina `Siniestros`.

Ronda: solo documentacion. No se programa frontend, backend, extractor `Live` ni SQL. La informacion aqui recogida es evidencia para SDD, trazabilidad y scaffolding revisado; no es contrato runtime.

## Estado final

`bloqueado externo` para desarrollo funcional.

Hay evidencia local suficiente para afirmar que `Siniestros` existe como entrada de menu, clave de busqueda/gestion y dominio de datos amplio en AppBuilder. Tambien hay evidencia de tablas, vistas y areas de negocio relacionadas con siniestros, indemnizaciones, franquicias, intervinientes, EIAC, aduana, agenda, cliente y poliza.

Actualizacion 2026-05-18: iLiniumTech ya contiene ruta protegida `/siniestros` y vista Vue fixture/read-only con datos sanitizados, filtros locales y acciones de detalle/exportacion bloqueadas. Esa vista no cambia la clasificacion funcional de este documento: sigue sin autorizar API, datos reales, detalle, exportacion ni escritura sin SDD, permisos, DBA/UAT y revision de seguridad.

SDD draft abierta para el primer corte futuro: [`SDD-2026-008 Siniestros read-only minimizado`](../../../sdd/specs/iLiniumTech/SDD-2026-008-siniestros-read-only.md). La SDD limita el primer incremento a listado read-only minimizado y deja detalle, intervinientes, importes, EIAC, observaciones y exportacion fuera de alcance.

No hay evidencia local suficiente para afirmar una pantalla AppBuilder completa de `Siniestros` con:

- `componentId` raiz;
- ruta real;
- padre/hijos de menu;
- tabs, submenus o areas internas confirmadas;
- layout, columnas, filtros y acciones reales;
- datasource concreto de la pantalla;
- permisos historicos por grupo/perfil;
- reglas UAT validadas.

Por tanto, no se deben crear componentes ni programar la pagina iLiniumTech de `Siniestros` hasta obtener metadata sanitizada de la pantalla real, SDD funcional aprobada, matriz de permisos y validacion DBA/UAT.

## Regla de producto aplicada

iLiniumTech no es un runtime dinamico tipo AppBuilder. AppBuilder se usa como fuente heredada de conocimiento, trazabilidad y analisis. La futura pagina de `Siniestros`, si producto la aprueba, debera quedar como:

- frontend Vue/TypeScript estatico;
- API .NET explicita;
- contratos propios;
- permisos de producto aplicados en backend;
- acceso a datos por repositorios revisados, parametros y whitelists;
- sin renderizado, queries, permisos ni workflows decididos por metadata AppBuilder en runtime.

## Fuentes revisadas

### iLiniumTech

- `AGENTS.md`.
- `PLANS.md`.
- `README.md`.
- `docs/PLAN_MAESTRO_IA.md`.
- `docs/ROADMAP_OBJETIVO_FINAL.md`.
- `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- `docs/appbuilder/pages/README.md`.
- `docs/appbuilder/pages/clientes/components/siniestros.md`.
- `iLiniumTech.Frontend/src/layout/appNavigation.ts`.

### AppBuilder, solo fuentes locales no sensibles

- `C:\Desarrollo\AppBuilder\src\frontend\Builder\src\infrastructure\localization\menus\Messages.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\entidades\builder\crud\infrastructure\search\localization\MessagesBusqueda.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\GestionConst.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\TableIcons.ts`.
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\Constantes\ProfileConst.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Dominio\AppBuilder.Dominio\migraciones peris.txt`, solo la linea de catalogo `identidad-EXS`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\Modelo\Repositorios\RepositorioBusqueda.cs`, solo busqueda de referencias a `Siniestro`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Dapper\Maestro\Repositorios\RepositorioBusqueda.cs`, solo busqueda de referencias a `Siniestro`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Maestro\MaestroDbContext.cs`.
- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Eiac\EiacDbContext.cs`.

### Fuentes evitadas

- `appsettings*`.
- Archivos `.config`.
- Connection strings.
- Dumps, backups, capturas sensibles y datos personales reales.
- Extractor `Live`.
- Cualquier consulta SQL real.

## Evidencia de menu

### Menu actual iLiniumTech

En `iLiniumTech.Frontend/src/layout/appNavigation.ts`, `Siniestros` aparece como item de primer nivel:

- etiqueta: `Siniestros`;
- icono: `pi pi-exclamation-triangle`;
- estado actual iLiniumTech: `fixture`;
- ruta: `/siniestros`;
- permiso iLiniumTech: no definido;
- hijos: no definidos.

Esto confirma que iLiniumTech ya tiene una superficie visible fixture/read-only para navegacion y pruebas. No existe aun contrato funcional con datos reales ni API propia.

### Localizacion y busqueda AppBuilder

Evidencia encontrada en frontend AppBuilder:

- `Messages.ts` contiene la etiqueta `gestion.sin` como `Siniestros`.
- `MessagesBusqueda.ts` contiene la opcion `sin` en la barra generica de busqueda.
- `GestionConst.ts` define `SINIESTRO = 'SIN'`.
- `TableIcons.ts` asocia la clave de gestion `SIN` con la tabla `Siniestro` y un icono historico.
- `NombreTablasConst.ts` declara tablas y vistas relacionadas con siniestros.

Esta evidencia apunta a un tipo de gestion/busqueda historico, no a una pantalla concreta versionada en codigo fuente.

### Perfil/catalogo historico

Evidencia encontrada en backend AppBuilder:

- `ProfileConst.cs` define `SINIESTROS = "identidad-EXS"`.
- `migraciones peris.txt` inserta el catalogo `identidad-EXS` con descripcion `Siniestros`.

Esto podria representar un perfil, identidad o clasificacion historica relacionada con siniestros. No debe transformarse automaticamente en permiso iLiniumTech sin analisis funcional, porque no hay matriz de permisos real ni equivalencia validada.

## Evidencia metadata encontrada y ausente

### Encontrada

Evidencia local positiva:

- existe entrada `Siniestros` en el menu objetivo de iLiniumTech;
- existe etiqueta historica AppBuilder para menu y busqueda;
- existe clave de gestion `SIN`;
- existe tabla principal `Siniestro`;
- existen tablas hijas `SiniestroFranquicia`, `SiniestroIndemnizacion` y `SiniestroInterviniente`;
- existen vistas `PBI_Siniestros`, `PBI_SiniestrosAcuerdos`, `vw_Aduana_Siniestro`, `vw_ClienteSiniestros` y `vw_Sin_Colaborador`;
- existen tablas/vistas EIAC especializadas para siniestros;
- la tabla `Siniestro` tiene triggers historicos vinculados a agenda y referencia de mediador.

### Ausente

No se encontro, en fuentes locales seguras:

- fila concreta de menu AppBuilder para `Siniestros`;
- `componentId` raiz de la pantalla;
- `urlComponentStatic` o `urlRouteComponentStatic` asociados;
- metadata de layout, columnas, filtros o datasource;
- tabs reales de la pagina;
- submenus reales bajo `Siniestros`;
- acciones especificas de pantalla;
- reglas de permisos por grupo o perfil;
- consultas autorizadas de lectura para iLiniumTech;
- SDD funcional;
- UAT owner.

## Raiz e hijos

### Raiz historica candidata

La evidencia apunta a que `Siniestros` podia participar en la busqueda/gestion generica de AppBuilder mediante:

- clave `SIN`;
- tabla `Siniestro`;
- icono historico asociado;
- componentes genericos de busqueda/CRUD;
- metadata de pantalla no localizada en los archivos seguros revisados.

Esta raiz es inferida, no confirmada.

### Hijos o componentes internos

No se detectaron hijos reales de menu ni componentes internos especificos de `Siniestros`.

No se crean documentos en `docs/appbuilder/pages/siniestros/components/*.md` en esta ronda porque hacerlo convertiria areas de datos en tabs inventadas.

Areas candidatas por dominio, pendientes de confirmar:

- busqueda/listado de siniestros;
- detalle read-only de siniestro;
- relacion con poliza/riesgo;
- relacion con cliente;
- franquicias;
- indemnizaciones;
- intervinientes;
- agenda/eventos asociados;
- EIAC/aduana/conciliacion;
- seguimiento por tramitador o colaborador.

Estas areas no son contrato de UI.

## Datos detectados

### Tabla `Siniestro`

`ModeloDbContext.cs` mapea la tabla `Siniestro`.

Campos o conceptos observados:

- identificadores y relaciones: `Id`, `RiesgoPolizaId`, `ReciboId`, `DireccionId`, `BrokerIntegracionId`, `IdOld`;
- referencias: `ReferenciaCia`, `ReferenciaCiaAsistencia`, `ReferenciaCliente`, `ReferenciaMediador`;
- fechas: `F_Siniestro`, `F_Parte`, `F_Cierre`, `F_Prescripcion`, `F_Revision`, `FCR`, `FUM`;
- clasificaciones: `IdCausa`, `IdConvenio`, `IdCulpa`, `IdDivisa`, `IdOrigen`, `IdPrioridad`, `IdSituacion`, `IdEstado`;
- textos: `Descripcion`, `Danos`, `DanosCoche`, `Garantias`, `Franquicia`;
- importes: `Indemnizacion`, `Reserva`;
- asignacion: `TramitadorId`;
- auditoria: `UCR`, `UUM`.

Relaciones observadas:

- `BrokerIntegracion`;
- `Direccion`;
- `Recibo`;
- `RiesgoPoliza`.

Triggers observados:

- `Tgr_AgendaSiniestroEvento`;
- `Tgr_CalculaReferenciaMediador`.

No se abrio la definicion SQL de los triggers. Solo se documento su existencia por mapeo EF.

### Tablas hijas directas

`SiniestroFranquicia`:

- `Descripcion`;
- `Importe`;
- `IdDivisa`;
- auditoria;
- relacion obligatoria con `Siniestro`.

`SiniestroIndemnizacion`:

- `Importe`;
- `Indemnizacion`;
- `Franquicia`;
- `Adelanto`;
- `Diferencia`;
- `ReclamacionCliente`;
- `F_Pago`;
- `Observaciones`;
- `IdDivisa`;
- auditoria;
- relacion obligatoria con `Siniestro`.

`SiniestroInterviniente`:

- `Nombre`;
- `Email`;
- `Telefono`;
- `IdTipo`;
- `Observaciones`;
- `IdOld`;
- auditoria;
- relacion obligatoria con `Siniestro`.

Estos datos son altamente sensibles y no autorizan por si solos tabs de UI.

### Vistas principales y reporting

`PBI_Siniestros` y `PBI_SiniestrosAcuerdos`:

- broker/cliente/poliza/siniestro como identificadores compuestos;
- causa, culpa, estado, situacion;
- fechas de siniestro, parte, cierre, revision y prescripcion;
- descripcion, danos, danos de coche, garantias;
- importes de indemnizacion;
- oficina, corredor y tramitador;
- relacion con acuerdo de reparto en la segunda vista.

`vw_ClienteSiniestros`:

- cliente, poliza, compania, ramo;
- referencias de compania y mediador;
- situacion, estado, prioridad y tecnico;
- riesgo, danos, descripcion y etiquetas;
- reserva;
- fechas de parte, siniestro, cierre y revision.

`vw_Aduana_Siniestro`:

- descripcion del siniestro;
- fechas de documento, parte, siniestro y validacion;
- situacion, convenio y culpa;
- lugar, numero de siniestro y numero de suplemento;
- observaciones;
- pago neto;
- poliza y polizaId.

`vw_Sin_Colaborador`:

- vista relacionada con colaborador/poliza; el mapeo EF local no aporta campos suficientes en el fragmento revisado para definir comportamiento.

### Dominio EIAC

`EiacDbContext.cs` mapea un dominio EIAC de siniestros:

- `EIACSiniestro`;
- `EIACSiniestroAcciones`;
- `EIACSiniestroAsegurado`;
- `EIACSiniestroAutoridades`;
- `EIACSiniestroContrarios`;
- `EIACSiniestroConvenios`;
- `EIACSiniestroDanios`;
- `EIACSiniestroIndemnizacion`;
- `EIACSiniestroInterviniente`;
- `EIACSiniestroSituaciones`;
- vistas de aduana EIAC para consolidar incidencias, pendientes de crear, revisar conciliacion y sin conciliar.

Campos observados en este dominio:

- numero de siniestro;
- poliza;
- fechas de parte, siniestro, validacion y rechazo;
- situacion, convenio, culpa, tipo, proceso y clase de mediador;
- lugar;
- descripcion y descripcion de reserva;
- tramitador;
- importes de reserva, indemnizacion, pagos, recobros y cambio;
- matricula;
- lesionados;
- acciones;
- danios;
- indemnizaciones;
- intervinientes;
- datos de entidad, mediador, conductor, direccion y persona de contacto.

Este bloque tiene riesgo alto de datos personales, datos de salud/lesiones, vehiculos, importes y observaciones libres. No debe entrar en MVP sin SDD de seguridad y minimizacion.

## Filtros detectados o inferidos

No se encontro metadata local de filtros de la pagina `Siniestros`.

Filtros candidatos por campos detectados, pendientes de UAT:

- busqueda por referencia de mediador o compania;
- numero de siniestro;
- poliza;
- cliente, solo si se aprueba minimizacion;
- compania;
- ramo;
- situacion/estado;
- causa/culpa/convenio;
- prioridad;
- tramitador;
- fechas de siniestro, parte, cierre y revision;
- importe/reserva, solo con permiso financiero;
- origen/EIAC/aduana, solo si entra en alcance;
- broker activo siempre desde sesion backend, nunca desde parametro libre.

No se debe implementar ningun filtro solo porque exista un campo heredado.

## Acciones historicas y candidatas

No se encontro metadata local de acciones especificas de la pagina `Siniestros`.

Acciones genericas AppBuilder observadas en busqueda/gestion:

- buscar;
- limpiar filtros;
- cerrar pestanas;
- guardar busqueda;
- nuevo registro;
- importar;
- guardar;
- actualizacion masiva.

Estas acciones pertenecen al motor generico de AppBuilder. No deben migrarse por defecto.

Acciones de negocio candidatas, todas pendientes de SDD:

- ver detalle read-only de siniestro;
- abrir poliza asociada;
- abrir cliente asociado;
- ver agenda/eventos relacionados;
- ver intervinientes;
- ver indemnizaciones;
- ver franquicias;
- ver datos EIAC o aduana;
- exportar listado;
- crear o modificar siniestro;
- registrar situacion o accion;
- validar/rechazar EIAC;
- gestionar pagos, reservas o recobros;
- generar documentos o comunicaciones.

Recomendacion: si se desbloquea un primer incremento, debe ser read-only y minimizado.

## Permisos historicos y permisos candidatos

### Historicos observados

Evidencia historica:

- clave/catalogo `identidad-EXS` asociado a `Siniestros`;
- clave de gestion `SIN`;
- visibilidad de menu/busqueda en componentes genericos.

No hay evidencia local de una matriz de permisos concreta por pantalla, accion, grupo, broker, perfil u oficina.

### iLiniumTech candidatos

Permisos read-only candidatos:

- `siniestros.catalogs`;
- `siniestros.read`;
- `siniestros.detail`;
- `siniestros.relatedPolicies`;
- `siniestros.relatedClients`.

Permisos sensibles candidatos:

- `siniestros.financial`: importes, reservas, indemnizaciones, recobros, pagos;
- `siniestros.participants`: intervinientes, conductores, persona de contacto, telefonos y emails;
- `siniestros.eiac`: datos EIAC, aduana, validacion, rechazo y conciliacion;
- `siniestros.notes`: observaciones y textos libres;
- `siniestros.export`.

Permisos de escritura bloqueados hasta SDD:

- `siniestros.create`;
- `siniestros.update`;
- `siniestros.actions.write`;
- `siniestros.eiac.validate`;
- `siniestros.payments.write`;
- `siniestros.delete`.

Regla: el backend debe aplicar permisos antes de leer o proyectar datos. El frontend solo reflejara capacidades autorizadas por `/api/me`.

## Propuesta Vue/API estatica futura

Esta propuesta no autoriza desarrollo. Es guia para una SDD futura si producto desbloquea `Siniestros`.

### Frontend

Ruta candidata:

- `/siniestros`;
- `/siniestros/:id`.

Feature candidata:

- `iLiniumTech.Frontend/src/features/siniestros`.

Componentes Vue candidatos:

- `SiniestrosView.vue`: listado read-only.
- `SiniestrosFilters.vue`: filtros confirmados por SDD/UAT.
- `SiniestrosTable.vue`: tabla con columnas minimizadas.
- `SiniestroDetailView.vue`: detalle read-only.
- `SiniestroSummaryPanel.vue`: resumen de referencia, situacion y fechas.
- `SiniestroRelatedPolicyPanel.vue`: enlace controlado a poliza si existe permiso.
- `SiniestroParticipantsPanel.vue`: solo si se aprueba permiso sensible.
- `SiniestroFinancialPanel.vue`: solo si se aprueba permiso financiero.
- `SiniestroEiacPanel.vue`: solo si se aprueba alcance EIAC.
- `useSiniestros.ts`.
- `siniestrosTypes.ts`.
- `siniestrosApi.ts`.

Estados obligatorios:

- cargando sesion;
- sin sesion;
- sin broker;
- sin permiso;
- error backend;
- sin resultados;
- listado paginado;
- detalle no autorizado;
- detalle no encontrado sin revelar existencia cruzada;
- campos sensibles ocultos por permiso.

Menu:

- `Siniestros` debe seguir `disabled: true` hasta que existan SDD, ruta, API y permiso.
- Permiso candidato de menu: `siniestros.read`.
- No se deben anadir hijos al menu hasta confirmar submenus reales o decidirlos como producto iLiniumTech.

### Backend

Endpoints candidatos:

- `GET /api/siniestros/catalogs`.
- `GET /api/siniestros`.
- `GET /api/siniestros/{id}`.
- `GET /api/siniestros/{id}/participants`, solo si se aprueba.
- `GET /api/siniestros/{id}/financial`, solo si se aprueba.
- `GET /api/siniestros/{id}/eiac`, solo si se aprueba.

Contratos candidatos:

- `SiniestrosSearchRequest`: filtros whitelisted, `page`, `pageSize`, `sort`.
- `SiniestroListItem`: referencia opaca, poliza minimizada, situacion, fechas principales, tramitador si procede, estado y compania.
- `SiniestroDetail`: secciones estaticas con minimizacion por permiso.
- `SiniestrosCatalogs`: situaciones, estados, causas, prioridades, tramitadores u otros catalogos confirmados.

Reglas backend:

- validar sesion y broker activo antes de resolver conexion;
- validar `currentBrokerId` contra `allowedBrokerIds`;
- usar whitelists de sort, filtros y columnas;
- parametrizar todos los valores;
- no ejecutar `QueryStatic` ni SQL heredado libre;
- no proyectar texto libre o datos personales sin decision de minimizacion;
- no revelar si un siniestro existe cuando broker o permiso no autorizan;
- devolver errores sanitizados con `correlationId` cuando aplique;
- no loguear referencias completas, telefonos, emails, observaciones, datos de lesionados, matriculas ni importes sensibles.

## Que no debe replicarse

- No replicar un renderer AppBuilder de pantallas, tabs, menus, datasources o workflows.
- No leer metadata AppBuilder en runtime para construir `Siniestros`.
- No exponer endpoints genericos de componente, datasource, query o menu.
- No ejecutar SQL historico de busqueda generica.
- No migrar acciones genericas de alta, edicion, importacion, guardado o actualizacion masiva sin SDD.
- No exponer `Descripcion`, `Danos`, observaciones, lesionados, intervinientes, emails, telefonos, direcciones, matriculas o importes por defecto.
- No confiar en API key, header libre de broker o `DemoSession` como seguridad productiva.
- No usar `SELECT *`.
- No registrar SQL completo, connection strings, cookies, tokens ni datos personales.

## Riesgos

Riesgos funcionales:

- Confundir dominio de datos con una pantalla real.
- Mezclar siniestros de cliente, poliza, EIAC, aduana y reporting en un unico modulo sin frontera funcional.
- Presentar importes, reservas o pagos con interpretacion incorrecta.
- Activar acciones de escritura que disparen triggers o procesos heredados desconocidos.

Riesgos de datos:

- Exposicion de PII en intervinientes, conductores, direcciones, telefonos y emails.
- Exposicion de datos de salud o lesionados.
- Exposicion de matriculas o datos de vehiculo.
- Exposicion de importes, reservas, indemnizaciones, pagos y recobros.
- Exposicion de observaciones y textos libres.

Riesgos tecnicos:

- Reintroducir AppBuilder con otro nombre mediante metadata runtime.
- Reusar SQL dinamico heredado.
- Usar vistas PBI/aduana/EIAC sin validar si son autorizadas para producto.
- Contaminar `SESSION_CONTEXT` por pooling si se consulta SQL real.
- Lectura cruzada entre brokers.

Riesgos de seguridad:

- 403/404 que revelan existencia de un siniestro de otro broker.
- Logs con descripcion, danos, lesionados, emails, telefonos o importes.
- Permisos solo en frontend sin enforcement backend.
- Exportaciones o descargas sin control de PII.

## Pruebas necesarias para desarrollo futuro

Backend:

- `GET /api/siniestros/catalogs` exige sesion/permisos.
- `GET /api/siniestros` exige `siniestros.read`.
- `GET /api/siniestros/{id}` exige `siniestros.detail`.
- broker ausente devuelve error seguro.
- broker no permitido no resuelve conexion ni consulta SQL.
- filtros y sort fuera de whitelist se rechazan.
- payloads maliciosos no alteran SQL.
- campos sensibles no se devuelven sin permisos especificos.
- 401/403/404 no revelan existencia cruzada.
- errores publicos incluyen `correlationId`.
- textos libres no aparecen en logs.

Frontend:

- menu `Siniestros` visible o activo solo cuando feature y permiso esten desbloqueados.
- ruta protegida valida `/api/me`.
- estados de sin sesion, sin broker y sin permiso.
- filtros inicializan, limpian y conservan paginacion de forma predecible.
- detalle vuelve al listado conservando filtros si se adopta el patron de Polizas.
- no se muestran campos sensibles si falta permiso.
- no aparecen `AppBuilder`, `IAP_`, `QueryStatic`, connection strings, SQL ni metadata en DOM.

QA/UAT:

- SDD aprobada antes de programar.
- columnas y filtros confirmados por responsable funcional.
- matriz de permisos y PII validada.
- comparativa controlada con AppBuilder o BBDD autorizada sin versionar datos reales.
- smoke visual desktop/mobile si se implementa UI.
- secret scan limpio.
- dependency audit sin findings bloqueantes.
- CORS audit si toca API/configuracion.

## Bloqueos y preguntas abiertas

Bloqueos externos:

- falta metadata sanitizada de la pantalla real de `Siniestros`;
- falta confirmar si `Siniestros` debe ser modulo independiente, vista desde cliente/poliza o ambas cosas;
- falta matriz real de permisos;
- falta validacion DBA de tabla/vistas autorizadas;
- falta decision de PII y minimizacion;
- falta decision sobre EIAC/aduana: fuera o dentro del primer alcance;
- falta UAT owner;
- falta auth productiva y permisos efectivos.

Preguntas para producto/DBA/seguridad:

- Cual es el primer objetivo: listado general, detalle desde poliza, detalle desde cliente, seguimiento de tramitador o EIAC?
- Que columnas son imprescindibles en listado?
- Que filtros usa negocio diariamente?
- Se pueden mostrar descripcion, danos u observaciones? Si si, con que permiso?
- Se pueden mostrar intervinientes, telefonos, emails, direcciones o matriculas?
- Se pueden mostrar importes de reserva, indemnizacion, pagos o recobros?
- Que vistas recomienda DBA para lectura read-only por broker?
- Que triggers o procesos se disparan en escrituras y deben quedar fuera?
- Como se debe resolver 403/404 para no revelar existencia de siniestros cruzados?

## Decision de subagentes de componentes

No se crearon subagentes de componentes en esta ronda.

Motivo: no se detectaron pestanas, submenus ni componentes internos reales y especificos de `Siniestros`. La evidencia encontrada describe dominio de datos y busqueda generica AppBuilder, no composicion visual confirmada.

Si en una ronda posterior aparece metadata sanitizada o SDD funcional, se recomienda crear subagentes documentales por cada area confirmada:

- listado/busqueda;
- detalle resumen;
- poliza/cliente relacionados;
- franquicias;
- indemnizaciones;
- intervinientes;
- agenda;
- EIAC/aduana;
- permisos y PII.

Cada subagente debera escribir solo en `docs/appbuilder/pages/siniestros/components/*.md` durante la fase documental.

## Evidencia de comandos

Comandos ejecutados en modo lectura/documentacion:

```powershell
git status --short --branch
rg -n -i "siniestro|siniestros" C:\Desarrollo\AppBuilder\src
rg -n -i "siniestro|siniestros" docs iLiniumTech.Frontend iLiniumTech.Backend tools
rg -n -i "identidad-EXS|GestionConst\.SINIESTRO|PBI_Siniestros|vw_ClienteSiniestros|vw_Aduana_Siniestro|EIACSiniestro" C:\Desarrollo\AppBuilder\src
rg -n -i "componentId.*siniestro|siniestro.*componentId|Pantalla_Siniestros|Pantalla_Siniestro" C:\Desarrollo\AppBuilder\src docs
Get-Content ...\Messages.ts
Get-Content ...\MessagesBusqueda.ts
Get-Content ...\GestionConst.ts
Get-Content ...\TableIcons.ts
Get-Content ...\NombreTablasConst.ts
Get-Content ...\ProfileConst.cs
Get-Content ...\ModeloDbContext.cs
Get-Content ...\MaestroDbContext.cs
Get-Content ...\EiacDbContext.cs
```

Restricciones aplicadas:

- no se abrieron archivos `appsettings*` ni `.config`;
- no se copiaron secretos;
- no se ejecutaron consultas SQL;
- no se ejecuto extractor `Live`;
- no se programo frontend ni backend.

## Clasificacion de pendientes

Completado con evidencia:

- entrada `Siniestros` localizada en menu iLiniumTech;
- localizacion AppBuilder localizada;
- clave de gestion `SIN` localizada;
- tabla `Siniestro` localizada;
- tablas hijas localizadas;
- vistas PBI, cliente, aduana, colaborador y EIAC localizadas;
- triggers historicos sobre `Siniestro` detectados por mapeo EF;
- no hay tabs/submenus/componentes reales confirmados localmente;
- no se ejecuto SQL ni extractor `Live`.

Pendiente tecnico:

- redactar SDD si producto prioriza `Siniestros`;
- definir alcance read-only inicial;
- definir contratos API estaticos;
- definir feature Vue estatica;
- definir matriz de permisos iLiniumTech;
- definir minimizacion de PII, importes, textos libres e intervinientes;
- actualizar menu cuando haya ruta y permiso real.

Bloqueado externo:

- metadata real de pantalla;
- UAT funcional;
- validacion DBA de origen de lectura;
- decision de EIAC/aduana;
- auth productiva;
- matriz de permisos;
- clasificacion PII y seguridad.
