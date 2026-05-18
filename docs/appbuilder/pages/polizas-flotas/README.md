# Polizas / Flotas - scope estatico MVP

Fecha: 2026-05-17

Estado: MVP estatico visible bajo Polizas. Bloqueado para datos reales, filtros funcionales, API nueva y acciones hasta SDD, permisos, regla funcional, DBA/UAT y contrato backend explicito.

## Rol de esta pagina

Este documento corrige el inventario de paginas para reflejar que `Flotas` ya existe como scope estatico del frontend actual.

Regla base aplicada: iLiniumTech no es un runtime dinamico tipo AppBuilder. La evidencia heredada y esta documentacion sirven para analisis, trazabilidad y SDD/scaffolding revisado. El producto debe seguir como Vue/TypeScript estatico y API .NET explicita.

## Detectado

- `src/layout/appNavigation.ts` contiene la entrada hija `Flotas` bajo `Polizas`.
- La ruta declarada es `/polizas/flotas`.
- Existe pantalla Vue estatica en `src/features/polizas-scopes/PolizasFlotasView.vue`.
- Existe prueba compartida de scopes en `src/features/polizas-scopes/PolizasScopesView.test.ts`.
- La pagina pertenece al carril `MVP estatico visible`: orienta navegacion y roadmap, pero no habilita datos reales.

## Inferido

- Por el nombre funcional, `Flotas` podria ser una vista, filtro o scope de polizas asociado a colectivos de vehiculos.
- Esa lectura no confirma por si sola ninguna regla SQL, columna, permiso, datasource, workflow ni equivalencia AppBuilder.
- Si producto la reactiva como funcionalidad real, debera definirse si es un filtro de `/polizas`, una pagina hija o un vertical con contrato propio.

## Fuera de alcance actual

- No hay metadata runtime.
- No hay API nueva.
- No hay consulta SQL nueva.
- No hay permiso nuevo efectivo.
- No hay datos reales, importes, PII, exportacion ni acciones.
- No hay paridad AppBuilder ni ejecucion de `QueryStatic`, workflows o componentes genericos heredados.

## Bloqueos para datos reales

Antes de conectar esta pantalla a datos reales hacen falta:

- SDD funcional de `Polizas / Flotas`.
- Regla funcional validada para identificar flotas.
- Permiso iLiniumTech aprobado, por ejemplo `polizas.scope.flotas.read` si se confirma el modelo de scope.
- Contrato API explicito o extension cerrada de `/api/polizas` con enum backend, nunca filtro libre desde label de menu.
- Validacion DBA/UAT contra entorno autorizado.
- Clasificacion de PII y minimizacion de campos visibles.
- Pruebas backend/frontend de broker, permisos, filtros y errores sanitizados.

## Decision documental

`Flotas` queda inventariada como scope estatico bajo Polizas. Puede permanecer visible para orientar pruebas de navegacion, pero debe mostrarse y tratarse como bloqueada para datos reales hasta cumplir los bloqueos anteriores.

No debe usarse metadata AppBuilder para construir menu, pantalla, permisos, queries ni workflows en runtime.

## Readiness Operativa seguros 2026-05-18

Estado actual:

- Frontend estatico protegido en `/polizas/flotas`, con `PolizasFlotasView.vue`.
- Test compartido: `PolizasScopesView.test.ts`.
- Estado de navegacion esperado: `blockedSdd`.
- No existe SDD funcional de Flotas ni permiso backend efectivo.

Componentes/patrones detectados:

- Usa `MvpPageShell` y una definicion estatica `MvpPageDefinition`.
- Muestra acciones deshabilitadas: filtrar flotas, ver poliza y exportar.
- Declara explicitamente que no infiere filtros por ramo, division, tipo o etiqueta.
- El test verifica ausencia de marcadores runtime peligrosos y menciona permiso candidato `polizas.scope.flotas.read`.

Bloqueos de PII, finanzas y workflows:

- No conectar a `/api/polizas` mediante texto libre de scope.
- No exponer cliente, documento, riesgo, vehiculo, matricula, bastidor, importes ni exportaciones.
- No activar detalle real ni acciones sobre polizas de flota sin SDD propia.

Permisos candidatos:

- `polizas.scope.flotas.read` si producto confirma que Flotas es scope de Polizas.
- Alternativa futura: permisos propios de un vertical si Flotas deja de ser simple scope.

Dependencias UAT/DBA:

- Confirmar regla funcional para identificar flotas.
- Confirmar si sera filtro de Polizas, pagina hija o vertical independiente.
- Confirmar origen SQL/vista y whitelists.
- Confirmar minimizacion de PII/vehiculo e impacto multi-tenant.

Tareas futuras pequenas:

- `producto-polizas-flotas-scope-decision`: decidir modelo funcional.
- `producto-sdd-polizas-flotas-readonly`: redactar SDD si se prioriza.
- `backend-polizas-flotas-filter-design`: proponer enum/filtro cerrado, no implementarlo sin SDD.
- `frontend-polizas-flotas-static-polish`: solo mejorar copy/test del placeholder.

Criterios de aceptacion para desbloquear:

- SDD aprobada con regla funcional y UAT.
- Permiso backend definido y probado.
- Broker validado antes de consultar.
- Filtro estructural cerrado y whitelisted.
- Sin metadata AppBuilder runtime ni SQL heredado.
