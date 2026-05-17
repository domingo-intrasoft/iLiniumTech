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
