# Readiness de desarrollo por pagina AppBuilder

Fecha: 2026-05-16

Este documento consolida el estado de readiness de las paginas del menu actual de iLiniumTech. Complementa el inventario principal en [`README.md`](README.md) y debe usarse para orientar futuras IA antes de abrir SDD o programar.

## Regla de interpretacion

`Lista para desarrollo inicial` no significa cierre funcional final ni paridad total con AppBuilder. Significa que hay evidencia local suficiente para iniciar un incremento iLiniumTech revisado: pagina Vue estatica, API explicita, permisos de producto y pruebas.

`Investigacion parcial` significa que existe evidencia de dominio, busqueda, tablas, vistas, componentes genericos o subdocumentos, pero falta al menos una pieza obligatoria de pagina: `componentId`, layout, raiz/hijos, pestanas, submenus, datasources concretos, acciones o permisos historicos confirmados.

`Bloqueada por falta de metadata de pagina` significa que no se debe desarrollar todavia. La pagina necesita extraccion sanitizada, validacion funcional, SDD y, si aplica, DBA/UAT.

## Matriz consolidada

| Menu actual | Readiness | Evidencia util | Bloqueo principal | Proxima accion |
| --- | --- | --- | --- | --- |
| Agenda | Investigacion parcial | Entrada de menu iLiniumTech y evidencia generica de calendario/AppBuilder | No hay fila real de menu con `componentId`, ruta, permisos ni layout | Obtener metadata sanitizada o validacion funcional antes de SDD |
| Clientes | Investigacion parcial | Dominio cliente, vistas/repositorios heredados y subdocumentos de componentes candidatos | Falta metadata completa de pantalla original, layout, tabs y permisos confirmados | Confirmar pagina real y despues crear SDD read-only |
| Propuestas | Bloqueada por falta de metadata de pagina | Entrada de menu y modelo generico de menu/componentes | No hay `componentId`, arbol, datasources, filtros, acciones ni permisos | Esperar extraccion sanitizada de pagina |
| Polizas | Lista para desarrollo inicial | Metadata sanitizada, raiz `Busqueda Polizas`, componentes documentados, SDD y runtime iLiniumTech existentes | UAT/DBA/auth real siguen pendientes para cierre productivo | Usar docs existentes para incrementos pequenos de Polizas, siempre con SDD si cambia contrato o comportamiento |
| Autos Particulares | Aparcada | SDD tecnica previa y ruta hija deshabilitada bajo Polizas | No es objetivo MVP vigente; falta validacion funcional/DBA/UAT de division `Particulares` | Reactivar solo con decision de producto y SDD actualizada |
| Flotas | Bloqueada por falta de metadata de pagina | Solo entrada hija candidata bajo Polizas | Falta pagina, metadata, reglas de datos y permisos | No desarrollar hasta nueva evidencia |
| Colectivas | Bloqueada por falta de metadata de pagina | Solo entrada hija candidata bajo Polizas | Falta pagina, metadata, reglas de datos y permisos | No desarrollar hasta nueva evidencia |
| Recibos | Investigacion parcial | Dominio `Recibo`, vistas auxiliares y busqueda heredada | No hay metadata completa de pantalla, layout, pestanas, acciones, permisos ni UAT | Localizar pagina real y confirmar columnas/filtros/permisos |
| Suplementos | Investigacion parcial | Concepto de menu, busqueda, tablas y componentes genericos | Falta componente raiz, `componentId`, datasource concreto y tabs/submenus propios | Extraer metadata real antes de agentes de componentes |
| Siniestros | Investigacion parcial | Tablas/vistas de siniestros, dominio EIAC y busquedas heredadas | Falta `componentId`, layout, columnas/filtros/acciones reales y permisos historicos | Confirmar pantalla real, SDD, matriz de permisos y DBA/UAT |
| Liq.Cia | Investigacion parcial | Dominio de liquidacion de compania y vistas/reporting relacionadas | Falta pantalla completa: `componentId`, datasource, columnas, filtros, acciones, permisos, tabs | Obtener metadata o UAT owner antes de programar |
| Liq.Col | Investigacion parcial | Dominio de liquidacion de colaborador y relaciones de datos | Falta layout, `componentId`, tabs/submenus y permisos por grupo/perfil | Obtener metadata o UAT owner antes de programar |
| Informes | Bloqueada por falta de inventario funcional validado | Evidencia transversal de reporting y catalogos | No hay pagina concreta ni inventario aprobado de informes, parametros, formatos y permisos | Levantar inventario funcional de informes primero |
| Controles | Bloqueada por falta de metadata de pagina | Infraestructura generica de controles AppBuilder | No prueba una pagina funcional `Controles`; alto riesgo de migrar motor dinamico | No desarrollar; requerir pagina real y SDD especifica |
| Estadisticas | Bloqueada por falta de metadata de pagina | Controles genericos de graficos/dashboard | No hay raiz, `componentId`, datasource, permisos ni workflow concreto | Definir caso analitico de producto antes de cualquier UI |
| Administracion | Bloqueada por falta de metadata de pagina y riesgo de replicar Builder | Evidencia del area tecnica `Sistema > Builder` | Migrarla podria reintroducir AppBuilder runtime; no prueba la pagina iLiniumTech | Requerir decision explicita de producto/arquitectura |
| Configuracion | Bloqueada por falta de metadata de pagina | Configuracion visual/generica de AppBuilder y ApplicationDetail | No hay pagina concreta de menu `Configuracion`; componentes observados son genericos | Definir si sera configuracion de usuario o administrativa |
| Conectividad | Bloqueada por falta de metadata de pagina y riesgo REST/SOAP heredado | Componentes y servicios genericos de datasources REST/SOAP | No hay pantalla funcional concreta; ejecutar conectores genericos seria riesgo alto | Requerir SDD, threat model y permisos antes de diseno |
| By Aunna | Bloqueada por falta de metadata de pagina | Referencias de marca/tenant/tema | No hay menu, ruta, componente, datasource, permisos ni workflow de pagina | Validar si es pagina real o solo branding |
| Logs | Bloqueada por falta de metadata de pagina | Dominio tecnico de logs AppBuilder | No hay pantalla migrable confirmada ni permisos/filtros/retencion definidos | Abrir SDD de auditoria/observabilidad si producto lo necesita |

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
