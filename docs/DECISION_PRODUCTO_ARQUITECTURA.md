# Decision producto/arquitectura - iLiniumTech no es AppBuilder runtime

Fecha: 2026-05-13

## Decision critica

iLiniumTech no sera un runtime dinamico equivalente a AppBuilder.

La metadata de AppBuilder se usa como insumo de extraccion, migracion, trazabilidad y scaffolding inicial. No debe convertirse en un contrato que el producto final interprete en tiempo de ejecucion para construir pantallas, rutas, permisos, queries o workflows.

El producto objetivo es:

- frontend Vue 3 estatico, con pantallas y componentes definidos como codigo fuente mantenible;
- backend API de datos, con contratos explicitos, validacion, autorizacion y acceso a BBDD controlado;
- herramientas de extraccion fuera del camino critico de runtime;
- specs SDD como puente entre lo observado en AppBuilder y lo que se implementa en iLiniumTech.

## Por que

AppBuilder resuelve muchos comportamientos en runtime desde metadata: UI, datasources, expresiones, workflows, permisos y queries. Copiar ese modelo trasladaria al nuevo producto los mismos riesgos que se quieren evitar:

- SQL estructural construido desde configuracion;
- superficie generica dificil de auditar;
- UI y permisos dependientes de datos cambiantes;
- expresiones y workflows con efectos no obvios;
- coupling fuerte entre BBDD de configuracion y comportamiento productivo.

iLiniumTech necesita preservar conocimiento funcional, no reproducir el motor.

## Uso permitido de metadata

La metadata puede usarse para:

- localizar pantallas, campos, datasources y dependencias reales;
- generar inventarios y comparativas;
- producir JSON sanitizado de analisis;
- preparar issues SDD;
- scaffold inicial de DTOs, vistas, columnas, filtros o pruebas;
- mantener trazabilidad entre AppBuilder e iLiniumTech.

Todo scaffold generado debe revisarse y convertirse en codigo fuente explicito antes de entrar al producto.

## Uso no permitido de metadata

La metadata no debe usarse para:

- renderizar componentes arbitrarios en produccion;
- decidir rutas o layouts en runtime;
- ejecutar `QueryStatic` o fragments SQL heredados;
- construir queries estructurales desde strings libres;
- activar workflows, expresiones, REST/SOAP o acciones sin una spec propia;
- hacer que el frontend dependa de `IAP_*` para funcionar.

Si una capacidad necesita configuracion en runtime, debe ser configuracion de producto iLiniumTech, pequena, versionada y validada; no una replica generica de AppBuilder.

## Arquitectura objetivo

```text
AppBuilder BBDD/API
      |
      | solo lectura, local o pipeline controlado
      v
Extractor / migracion / scaffolding
      |
      | JSON sanitizado + SDD + trazabilidad
      v
Codigo iLiniumTech revisado
      |
      +--> Frontend Vue estatico
      |
      +--> Backend API de datos
              |
              v
          BBDD de producto / vistas autorizadas
```

En runtime productivo, el usuario navega una aplicacion Vue normal que llama endpoints de una API normal. La API puede leer datos reales, pero mediante repositorios explicitos, whitelists y contratos propios.

## Plan por fases frontend/backend

### Fase 0 - Decision y alineacion documental

- Marcar AppBuilder como fuente heredada de conocimiento, no como runtime a reconstruir.
- Actualizar SDD recientes para que extractor y SQL sean pasos de migracion/producto.
- Retirar o deprecar cualquier endpoint productivo de metadata de pantalla. La trazabilidad AppBuilder vive en documentacion, specs, JSON sanitizado offline o scaffolding revisado, no en contratos runtime.

### Fase 1 - Frontend Vue estatico de Polizas

- Consolidar `/polizas` como pantalla Vue mantenida a mano.
- Traducir la forma visual observada en AppBuilder a componentes estaticos iLiniumTech.
- Mantener estados `loading`, `empty`, `error` y autorizacion visibles.
- Probar filtros, tabla, detalle y regresiones visuales sin depender de metadata en runtime.

### Fase 2 - Backend API read-only

- Sustituir fixtures por repositorio SQL de solo lectura.
- Usar DTOs, query objects, whitelists y parametros.
- Aplicar auth, CORS restrictivo, logs sanitizados y errores sin detalles internos.
- Tratar campos personales como datos sensibles desde el primer acceso real.

### Fase 3 - Extraccion y scaffolding controlados

- Implementar extractor offline/local de metadata `IAP_*`.
- Generar artefactos sanitizados para comparacion, SDD y scaffolding.
- Convertir cualquier salida util en codigo revisado: tipos, columnas, etiquetas, pruebas o documentacion.
- No introducir dependencia runtime del frontend/backend sobre los artefactos crudos del extractor.

### Fase 4 - Evolucion funcional explicita

- Anadir detalle, acciones o escrituras solo con SDD propia.
- Modelar workflows heredados como casos de uso iLiniumTech, no como ejecucion generica.
- Implementar permisos de producto en backend, no solo visibilidad heredada de UI.
- Mantener trazabilidad a AppBuilder para auditoria y UAT.

### Fase 5 - Operacion y despliegue

- Publicar frontend como build estatico.
- Desplegar backend API con configuracion por entorno y secretos fuera de Git.
- Ejecutar gates de seguridad, dependencia, CORS, build y tests en CI.
- Mantener documentacion de decisiones cuando se cambien contratos o rutas de datos.

## Implicaciones para specs SDD

Cada SDD nueva debe declarar si usa metadata como:

- evidencia funcional;
- entrada de extractor;
- scaffold revisado;
- trazabilidad historica.

Ninguna SDD debe pedir "runtime dinamico", "render dinamico" o "motor generico AppBuilder" salvo que se abra explicitamente una decision de arquitectura que revoque este documento.
