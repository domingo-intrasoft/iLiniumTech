# Readiness de desarrollo por pagina AppBuilder

Fecha: 2026-05-16

Este documento consolida el estado de readiness de las paginas del menu actual de iLiniumTech. Complementa el inventario principal en [`README.md`](README.md) y debe usarse para orientar futuras IA antes de abrir SDD o programar.

## Regla de interpretacion

`Lista para desarrollo inicial` no significa cierre funcional final ni paridad total con AppBuilder. Significa que hay evidencia local suficiente para iniciar un incremento iLiniumTech revisado: pagina Vue estatica, API explicita, permisos de producto y pruebas.

`Investigacion parcial` significa que existe evidencia de dominio, busqueda, tablas, vistas, componentes genericos o subdocumentos, pero falta al menos una pieza obligatoria de pagina: `componentId`, layout, raiz/hijos, pestanas, submenus, datasources concretos, acciones o permisos historicos confirmados.

`MVP estatico visible` significa que se ha creado una ruta Vue protegida con contenido especifico de modulo, acciones deshabilitadas y avisos de riesgos. No habilita datos reales, acciones, API, permisos finos ni paridad AppBuilder.

`Bloqueada para datos/acciones reales` significa que la pagina puede existir como MVP estatico, pero necesita extraccion sanitizada, validacion funcional, SDD y, si aplica, DBA/UAT antes de conectar backend o exponer informacion sensible.

## Matriz consolidada

| Menu actual | Readiness | Evidencia util | Bloqueo principal | Proxima accion |
| --- | --- | --- | --- | --- |
| Agenda | MVP estatico visible; bloqueada para datos reales | Entrada de menu iLiniumTech y evidencia generica de calendario/AppBuilder | No hay fila real de menu con `componentId`, ruta, permisos ni layout | Obtener metadata sanitizada o validacion funcional antes de SDD/API |
| Clientes | MVP estatico visible; bloqueada para datos reales | Dominio cliente, vistas/repositorios heredados y subdocumentos de componentes candidatos | Falta metadata completa de pantalla original, layout, tabs y permisos confirmados | Confirmar pagina real y despues crear SDD read-only |
| Propuestas | MVP estatico visible; bloqueada para datos reales | Entrada de menu y modelo generico de menu/componentes | No hay `componentId`, arbol, datasources, filtros, acciones ni permisos | Esperar extraccion sanitizada de pagina |
| Polizas | Lista para desarrollo inicial | Metadata sanitizada, raiz `Busqueda Polizas`, componentes documentados, SDD y runtime iLiniumTech existentes | UAT/DBA/auth real siguen pendientes para cierre productivo | Usar docs existentes para incrementos pequenos de Polizas, siempre con SDD si cambia contrato o comportamiento |
| Autos Particulares | Aparcada | SDD tecnica previa y ruta hija deshabilitada bajo Polizas | No es objetivo MVP vigente; falta validacion funcional/DBA/UAT de division `Particulares` | Reactivar solo con decision de producto y SDD actualizada |
| Flotas | MVP estatico visible; bloqueada para datos reales | Solo entrada hija candidata bajo Polizas | Falta pagina, metadata, reglas de datos y permisos | Confirmar regla funcional antes de API |
| Colectivas | MVP estatico visible; bloqueada para datos reales | Solo entrada hija candidata bajo Polizas | Falta pagina, metadata, reglas de datos y permisos | Confirmar regla funcional antes de API |
| Recibos | MVP estatico visible; bloqueada para datos reales | Dominio `Recibo`, vistas auxiliares y busqueda heredada | No hay metadata completa de pantalla, layout, pestanas, acciones, permisos ni UAT | Localizar pagina real y confirmar columnas/filtros/permisos |
| Suplementos | MVP estatico visible; bloqueada para datos reales | Concepto de menu, busqueda, tablas y componentes genericos | Falta componente raiz, `componentId`, datasource concreto y tabs/submenus propios | Extraer metadata real antes de API |
| Siniestros | MVP estatico visible; bloqueada para datos reales | Tablas/vistas de siniestros, dominio EIAC y busquedas heredadas | Falta `componentId`, layout, columnas/filtros/acciones reales y permisos historicos | Confirmar pantalla real, SDD, matriz de permisos y DBA/UAT |
| Liq.Cia | MVP estatico visible; bloqueada para datos reales | Dominio de liquidacion de compania y vistas/reporting relacionadas | Falta pantalla completa: `componentId`, datasource, columnas, filtros, acciones, permisos, tabs | Obtener metadata o UAT owner antes de API |
| Liq.Col | MVP estatico visible; bloqueada para datos reales | Dominio de liquidacion de colaborador y relaciones de datos | Falta layout, `componentId`, tabs/submenus y permisos por grupo/perfil | Obtener metadata o UAT owner antes de API |
| Informes | MVP estatico visible; bloqueada para datos reales | Evidencia transversal de reporting y catalogos | No hay pagina concreta ni inventario aprobado de informes, parametros, formatos y permisos | Levantar inventario funcional de informes primero |
| Controles | MVP estatico visible; bloqueada para datos reales | Infraestructura generica de controles AppBuilder | No prueba una pagina funcional `Controles`; alto riesgo de migrar motor dinamico | Requerir pagina real y SDD especifica antes de API |
| Estadisticas | MVP estatico visible; bloqueada para datos reales | Controles genericos de graficos/dashboard | No hay raiz, `componentId`, datasource, permisos ni workflow concreto | Definir caso analitico de producto antes de API |
| Administracion | MVP estatico visible; acciones bloqueadas por riesgo Builder | Evidencia del area tecnica `Sistema > Builder` | Migrarla podria reintroducir AppBuilder runtime; no prueba la pagina iLiniumTech | Requerir decision explicita de producto/arquitectura |
| Configuracion | MVP estatico visible; acciones bloqueadas por riesgo configuracion | Configuracion visual/generica de AppBuilder y ApplicationDetail | No hay pagina concreta de menu `Configuracion`; componentes observados son genericos | Definir si sera configuracion de usuario o administrativa |
| Conectividad | MVP estatico visible; acciones bloqueadas por riesgo REST/SOAP heredado | Componentes y servicios genericos de datasources REST/SOAP | No hay pantalla funcional concreta; ejecutar conectores genericos seria riesgo alto | Requerir SDD, threat model y permisos antes de diseno |
| By Aunna | MVP estatico visible; bloqueada para datos reales | Referencias de marca/tenant/tema | No hay menu, ruta, componente, datasource, permisos ni workflow de pagina | Validar si es pagina real o solo branding |
| Logs | MVP estatico visible; bloqueada para datos reales | Dominio tecnico de logs AppBuilder | No hay pantalla migrable confirmada ni permisos/filtros/retencion definidos | Abrir SDD de auditoria/observabilidad si producto lo necesita |

## Plan operacional para el siguiente avance de paginas estaticas

Objetivo del siguiente avance: estabilizar el resto del menu como paginas Vue estaticas y protegidas, con contenido especifico por modulo, acciones no operativas y trazabilidad a esta documentacion. Este avance no autoriza APIs nuevas, datos reales, permisos finos, escrituras, workflows, SQL ni consumo runtime de metadata AppBuilder.

Decision de producto: separar dos carriles.

- `Desarrollable ahora con fixture/estatico`: se permite mejorar o completar la pagina visible con informacion documental, estados bloqueados, acciones deshabilitadas, tests de vista/ruta y sin llamadas a backend nuevo.
- `Requiere contrato backend/SDD`: cualquier listado con datos, detalle, filtros reales, catalogos, exportacion, importes, PII, auditoria, logs operativos o accion funcional necesita SDD, permisos iLiniumTech, contrato API, DBA/UAT si aplica y pruebas backend/frontend.

### Orden recomendado

1. Revisar smoke de menu/rutas para las paginas estaticas ya existentes y cerrar gaps visuales sin ampliar alcance funcional.
2. Enriquecer primero paginas de dominio con documentacion util y menor riesgo operativo: `Agenda`, `Propuestas`, `Clientes`, `Recibos`, `Suplementos`, `Siniestros`, `Liq.Cia`, `Liq.Col`, `Informes`, `Estadisticas`, `Polizas / Flotas` y `Polizas / Colectivas`.
3. Mantener como superficies bloqueadas, no como modulos operativos, las paginas tecnicas o de alto riesgo: `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna` y `Logs`.
4. No reactivar `Autos Particulares` salvo decision explicita de producto, SDD actualizada y UAT/DBA para la regla de division.

### Matriz de incrementos explicitos

| Pagina | Estado actual | Archivos frontend existentes | Documentacion AppBuilder disponible | Carril permitido ahora | Pruebas esperadas | Riesgo y dependencia |
| --- | --- | --- | --- | --- | --- | --- |
| Agenda | Ruta/pagina estatica protegida; evidencia parcial sin SDD funcional | `src/features/agenda/AgendaView.vue`, `AgendaView.test.ts` | `docs/appbuilder/pages/agenda/README.md` | Desarrollable ahora como placeholder enriquecido; API de eventos requiere SDD | Test de vista, router/nav, build frontend si se toca UI | Riesgo de calendarios con PII; depende de metadata real, permisos y UAT para datos |
| Clientes | Ruta/pagina estatica protegida; evidencia tecnica parcial | `src/features/clientes/ClientesView.vue`, `ClientesView.test.ts` | `clientes/README.md` y `clientes/components/*.md` | Placeholder enriquecido ahora; busqueda/listado/detalle reales requieren SDD/API | Test de vista, estados bloqueados, no llamadas API nuevas | PII alta; depende de pantalla real, campos visibles, permisos y DBA/UAT |
| Propuestas | Ruta/pagina estatica protegida; sin metadata concreta | `src/features/propuestas/PropuestasView.vue`, `PropuestasView.test.ts` | `propuestas/README.md` | Placeholder enriquecido ahora; datos/acciones requieren SDD/API | Test de vista, ruta y acciones deshabilitadas | Falta `componentId`, datasource y permisos; no inferir flujo comercial |
| Polizas | Pagina funcional inicial con listado/detalle/API existentes | `src/features/polizas/**` | `polizas/README.md`, `polizas/components/*.md`, SDDs de Polizas | No es placeholder: solo incrementos pequenos con SDD/decision si cambia contrato o UX visible | Unit frontend/backend, smoke `/polizas` y detalle, auditorias aplicables | Pendiente auth real, broker, DBA/UAT y PII; no romper MVP vigente |
| Autos Particulares | Incremento tecnico previo aparcado | `src/features/autos-particulares/**` | SDD-2026-006; sin README de pagina en esta carpeta | No desarrollar ahora | Solo regresion si cambios ajenos lo afectan | Requiere decision de producto, SDD actualizada, regla de division y UAT/DBA |
| Polizas / Flotas | Ruta/pagina estatica protegida; scope aparcado | `src/features/polizas-scopes/PolizasFlotasView.vue`, `PolizasScopesView.test.ts` | Submenus de Polizas; README especifico pendiente | Placeholder enriquecido ahora; filtro/listado real requiere SDD/API | Test compartido de scope, ruta y acciones deshabilitadas | Falta regla funcional de Flotas, permisos y datos autorizados |
| Polizas / Colectivas | Ruta/pagina estatica protegida; scope aparcado | `src/features/polizas-scopes/PolizasColectivasView.vue`, `PolizasScopesView.test.ts` | Submenus de Polizas; README especifico pendiente | Placeholder enriquecido ahora; filtro/listado real requiere SDD/API | Test compartido de scope, ruta y acciones deshabilitadas | Falta regla funcional de Colectivas, permisos y datos autorizados |
| Recibos | Ruta/pagina estatica protegida; bloqueada hasta SDD/API | `src/features/recibos/RecibosView.vue`, `RecibosView.test.ts` | `recibos/README.md` | Placeholder enriquecido ahora; importes, filtros y detalle requieren SDD/API | Test de vista, importes no operativos, no API real | Riesgo financiero/PII; depende de columnas, permisos, broker y UAT |
| Suplementos | Ruta/pagina estatica protegida; bloqueada hasta SDD/API | `src/features/suplementos/SuplementosView.vue`, `SuplementosView.test.ts` | `suplementos/README.md` | Placeholder enriquecido ahora; listado/detalle requieren SDD/API | Test de vista, acciones no operativas, no metadata runtime | PII y workflows; depende de metadata real, catalogos y permisos |
| Siniestros | Ruta/pagina estatica protegida; bloqueada hasta SDD/API | `src/features/siniestros/SiniestrosView.vue`, `SiniestrosView.test.ts` | `siniestros/README.md` | Placeholder enriquecido ahora; expedientes reales requieren SDD/API | Test de vista, estados bloqueados, sin datos sensibles | PII alta y datos de siniestro; depende de permisos, broker, DBA/UAT |
| Liq.Cia | Ruta/pagina estatica protegida; MVP estatico sin API real | `src/features/liquidaciones-compania/LiquidacionesCompaniaView.vue`, test asociado | `liq-cia/README.md` | Placeholder enriquecido ahora; importes/liquidacion real requieren SDD/API | Test de vista, acciones deshabilitadas, no importes reales | Riesgo financiero; depende de owner funcional, permisos y conciliacion |
| Liq.Col | Ruta/pagina estatica protegida; MVP estatico sin API real | `src/features/liquidaciones-colaborador/LiquidacionesColaboradorView.vue`, test asociado | `liq-col/README.md` | Placeholder enriquecido ahora; importes/liquidacion real requieren SDD/API | Test de vista, acciones deshabilitadas, no importes reales | Riesgo financiero y multi-tenant; depende de reglas por colaborador |
| Informes | Ruta/pagina estatica protegida; MVP estatico sin API real | `src/features/informes/InformesView.vue`, `InformesView.test.ts` | `informes/README.md` | Placeholder enriquecido ahora; informes ejecutables requieren SDD/API | Test de vista, parametros no operativos, sin descargas reales | Riesgo de reporting con PII/exportacion; depende de inventario de informes |
| Controles | Ruta/pagina estatica protegida; bloqueado externo | `src/features/controles/ControlesView.vue`, `ControlesView.test.ts` | `controles/README.md` | Solo placeholder bloqueado; no construir motor de controles | Test de vista bloqueada y ausencia de acciones | Alto riesgo de recrear AppBuilder; requiere pagina real y decision |
| Estadisticas | Ruta/pagina estatica protegida; MVP estatico sin API real | `src/features/estadisticas/EstadisticasView.vue`, `EstadisticasView.test.ts` | `estadisticas/README.md` | Placeholder enriquecido ahora; metricas reales requieren SDD/API | Test de vista, graficos no conectados a datos reales | Riesgo analitico/PII; depende de caso analitico y contrato de agregados |
| Administracion | Ruta/pagina estatica protegida; bloqueado externo | `src/features/administracion/AdministracionView.vue`, `AdministracionView.test.ts` | `administracion/README.md` | Solo placeholder bloqueado; no activar administracion operativa | Test de vista bloqueada y acciones deshabilitadas | Riesgo critico de reintroducir Builder; requiere decision arquitectura/seguridad |
| Configuracion | Ruta/pagina estatica protegida; bloqueado externo | `src/features/configuracion/ConfiguracionView.vue`, `ConfiguracionView.test.ts` | `configuracion/README.md` | Solo placeholder bloqueado; no editar configuracion real | Test de vista bloqueada y sin secretos | Riesgo de secretos/config runtime; requiere SDD y threat review |
| Conectividad | Ruta/pagina estatica protegida; bloqueado externo | `src/features/conectividad/ConectividadView.vue`, `ConectividadView.test.ts` | `conectividad/README.md` | Solo placeholder bloqueado; no ejecutar conectores | Test de vista bloqueada y acciones no operativas | Riesgo REST/SOAP/workflows heredados; requiere SDD y seguridad |
| By Aunna | Ruta/pagina estatica protegida; bloqueado externo | `src/features/by-aunna/ByAunnaView.vue`, `ByAunnaView.test.ts` | `by-aunna/README.md` | Solo placeholder bloqueado hasta definicion funcional | Test de vista bloqueada y copy de alcance | Falta pagina real; depende de decision si es branding o modulo |
| Logs | Ruta/pagina estatica protegida; bloqueado externo | `src/features/logs/LogsView.vue`, `LogsView.test.ts` | `logs/README.md` | Solo placeholder bloqueado; logs operativos requieren SDD/API | Test de vista bloqueada, sin datos reales ni payloads | Riesgo de PII, secretos y retencion; depende de politica de auditoria |

### Reglas de salida para agentes frontend

- Si una pagina esta en carril `placeholder enriquecido`, el agente solo puede mejorar la experiencia estatica existente: copy especifico, tarjetas de estado, acciones deshabilitadas, estados bloqueados y tests de vista/ruta.
- No crear servicios, composables de API, DTOs de datos reales, filtros funcionales, exportaciones, detalle real ni permisos nuevos sin SDD.
- No tocar `appNavigation.ts` ni `router/index.ts` en una tarea documental o de placeholder salvo que el coordinador lo asigne explicitamente.
- Si la pagina necesita datos, el resultado del agente debe ser una propuesta de SDD/API y lista de bloqueos, no codigo conectado.
- Todo cierre debe indicar que no se consume metadata AppBuilder en runtime.

## Regla especial para Polizas

Polizas es la unica pagina lista para desarrollo inicial porque ya tiene:

- pagina documentada: [`polizas/README.md`](polizas/README.md);
- componentes documentados en [`polizas/components`](polizas/components);
- SDD relacionada: [`SDD-2026-001-polizas-mvp.md`](../../sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md), [`SDD-2026-003-repositorio-sql-polizas.md`](../../sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md) y [`SDD-2026-005-auth-permisos-producto.md`](../../sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md);
- frontend/backend iLiniumTech ya existentes como codigo fuente explicito.

Los incrementos nuevos de Polizas siguen necesitando SDD o decision cuando cambien contrato API, permisos, datos, UX visible o seguridad.

## Regla especial para paginas con dominio pero sin layout

`Clientes`, `Recibos`, `Suplementos`, `Siniestros`, `Liq.Cia` y `Liq.Col` no deben confundirse con paginas listas. La evidencia actual ayuda a preparar SDD y preguntas de UAT, pero no confirma:

- `ComponentId` de pagina;
- componente raiz y arbol de hijos;
- layout, columnas, paneles y orden real;
- pestanas, submenus o areas internas;
- datasources concretos y filtros autorizados;
- acciones reales y permisos por perfil/grupo;
- campos sensibles visibles por permiso;
- owner funcional y muestras UAT autorizadas.

Hasta cerrar esos puntos, cualquier UI/API debe considerarse propuesta, no implementacion lista.

## Ciclo recomendado al recibir metadata real

1. Registrar evidencia sanitizada en el README de pagina, sin secretos, connection strings, dumps ni datos personales reales.
2. Clasificar la pagina como `Investigacion parcial` o `Lista para desarrollo inicial` segun completitud de metadata y validacion funcional.
3. Crear agentes por componente o pestana solo cuando existan raiz, hijos, tabs/submenus y datasources confirmados.
4. Convertir el resultado en SDD con contratos Vue/API, permisos iLiniumTech, riesgos, pruebas y UAT.
5. Implementar codigo iLiniumTech explicito; no consumir metadata AppBuilder en runtime.
6. Ejecutar validaciones aplicables y documentar pruebas omitidas, bloqueos externos y riesgos residuales.
