# Enrutado de motores AI

Fecha: 2026-05-13

## Objetivo

Elegir motor y modo de ejecucion segun riesgo, contexto local requerido y superficie afectada.

## Codex local

Usar para:

- cambios con lectura extensa del repo;
- cambios de backend, frontend o tests;
- trabajo que requiera ejecutar comandos locales;
- investigacion de fallos reproducibles en el workspace.

No usar si faltan secretos necesarios y no hay alternativa con fixtures o mocks.

## Codex en GitHub Actions

Usar para:

- issues SDD pequenas;
- cambios documentales o de codigo con alcance claro;
- PRs donde los checks puedan validar el resultado sin entorno local privado.

Evitar en tareas que requieran BBDD de prueba no disponible en Actions.

## Claude

Usar como alternativa para:

- revision de documentacion amplia;
- contrastar planes o specs;
- cambios UI/documentales de bajo riesgo cuando Codex no sea el motor elegido.

## Escalada humana

Usar `needs-human` si:

- hay secretos o conexiones reales;
- la spec no define fuera de alcance;
- se pide tocar rutas sensibles sin aprobacion;
- el cambio puede exponer datos personales;
- hay conflicto entre documentacion, codigo y peticion del usuario.

## Tabla rapida

| Riesgo | Ejemplo | Motor recomendado |
| --- | --- | --- |
| Bajo | README, spec, test pequeno | `ai-codex` |
| Medio | endpoint nuevo protegido, componente UI | `ai-codex` con PR y checks |
| Alto | SQL real, auth, workflows, CI seguridad | humano + `ai-codex` tras aprobacion |
| Bloqueado | secretos, dumps, credenciales | `needs-human` |
