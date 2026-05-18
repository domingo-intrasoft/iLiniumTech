# Coordinacion de agentes por pagina - 2026-05-18

Estado: bloque autonomo iniciado tras dejar `Polizas CRUD BBDD` como MVP local visible para pruebas.

## Objetivo del bloque

Preparar el desarrollo del resto de paginas del menu con una ronda documental segura por dominios funcionales. Esta ronda no autoriza backend nuevo, datos reales, escrituras, exportaciones, permisos nuevos ni consumo runtime de metadata AppBuilder.

## Reglas aplicadas

- `Polizas CRUD BBDD` queda como vertical de referencia local.
- `Autos Particulares` permanece aparcado.
- Las paginas fixture o `blockedSdd` siguen sin API real hasta tener SDD, contrato explicito, permisos backend, UAT y revision de seguridad si aplica.
- Los agentes solo pueden tocar documentacion de su dominio.
- Ningun agente debe modificar codigo de aplicacion, router, menu, servicios API, backend, SDD existentes ni `page-agent-rollout.md` durante esta ronda.
- No se deben versionar secretos, connection strings, dumps, datos personales ni capturas sensibles.

## Reparto de jefes

| Jefe | Paginas | Write scope documental |
| --- | --- | --- |
| Negocio diario | `Agenda`, `Clientes`, `Propuestas` | `docs/appbuilder/pages/agenda/**`, `clientes/**`, `propuestas/**`, evidencias QA especificas |
| Operativa seguros | `Siniestros`, `Recibos`, `Suplementos`, `Polizas / Flotas`, `Polizas / Colectivas` | `docs/appbuilder/pages/siniestros/**`, `recibos/**`, `suplementos/**`, `polizas-flotas/**`, `polizas-colectivas/**`, evidencias QA especificas |
| Reporting y liquidaciones | `Liq.Cia`, `Liq.Col`, `Informes`, `Estadisticas` | `docs/appbuilder/pages/liq-cia/**`, `liq-col/**`, `informes/**`, `estadisticas/**`, evidencias QA especificas |
| Tecnico/admin/seguridad | `Administracion`, `Configuracion`, `Conectividad`, `Controles`, `By Aunna`, `Logs` | `docs/appbuilder/pages/administracion/**`, `configuracion/**`, `conectividad/**`, `controles/**`, `by-aunna/**`, `logs/**`, evidencias QA especificas |

## Salida esperada por pagina

Cada jefe debe dejar documentado:

- estado actual detectado en frontend y documentacion;
- componentes, filtros, acciones y estados visibles;
- acciones bloqueadas y motivo;
- riesgos PII, tenant, permisos, seguridad, finanzas o AppBuilder runtime;
- permisos candidatos si el modulo pasa a carril API;
- dependencias UAT, DBA, producto o plataforma;
- tareas pequenas futuras independientes;
- criterios de aceptacion para carril fixture;
- criterios de aceptacion para una futura SDD read-only;
- pruebas obligatorias y evidencias antes de Done.

## Criterio de integracion posterior

El arquitecto debe revisar los informes de los jefes y decidir una de estas salidas por pagina:

- mantener fixture con hardening visual/test;
- abrir SDD read-only minimizada;
- mantener bloqueada por seguridad/producto;
- pedir UAT/DBA antes de planificar;
- crear tarea tecnica pequena con archivos permitidos y pruebas concretas.

## Evidencias de este bloque

- `git status --short --branch` revisado antes de empezar.
- `README.md`, `docs/PLAN_MAESTRO_IA.md`, `docs/ROADMAP_OBJETIVO_FINAL.md` y `docs/appbuilder/pages/page-agent-rollout.md` revisados.
- Router y menu reales revisados en `iLiniumTech.Frontend/src/router/index.ts` y `iLiniumTech.Frontend/src/layout/appNavigation.ts`.
- Agentes existentes reutilizados por limite de hilos, con write scopes disjuntos.
