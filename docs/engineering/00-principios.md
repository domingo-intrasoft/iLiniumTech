# Principios

## Modernizacion progresiva

iLiniumTech no debe intentar reescribir AppBuilder entero en el primer movimiento. La estrategia correcta es extraer un comportamiento pequeno, entender sus dependencias y construir un MVP verificable.

## Seguridad antes que comodidad

La aplicacion actual contiene patrones peligrosos: secretos en fuentes auxiliares, SQL dinamico, CORS permisivo, cifrado reversible y configuracion sensible mezclada con codigo. En iLiniumTech esos riesgos se tratan como requisitos de diseno, no como tareas posteriores.

## Especificaciones vivas

Cada cambio funcional relevante debe nacer de una spec SDD versionada. La spec debe declarar objetivo, fuera de alcance, contrato de datos, criterios de aceptacion, pruebas y seguridad.

## Cambios pequenos y verificables

Evitar PRs que mezclen:

- nuevas dependencias;
- refactors amplios;
- cambios de arquitectura;
- cambios funcionales;
- cambios de pipeline;
- migraciones de BBDD.

## Testing proporcional al riesgo

No se busca cobertura decorativa. Se priorizan pruebas donde hay:

- conexiones y secretos;
- autenticacion o autorizacion;
- SQL dinamico;
- datos persistidos;
- transformaciones de metadata;
- render dinamico;
- expresiones y workflows;
- integraciones REST, SOAP o GraphQL.

## Configuracion explicita

Puertos, URLs, connection strings, claves, endpoints y entornos viven fuera del codigo. El codigo puede tener defaults locales no sensibles, pero no puede depender de rutas o secretos embebidos.

## Calidad automatizada

Todo gate exigido en CI debe poder ejecutarse localmente. Si un comando bloquea la entrega, debe estar documentado.

## Registro de decisiones

Cuando se introduzca una tecnologia nueva o se reemplace un patron de AppBuilder, debe quedar explicado en una spec, ADR o documento tecnico.
