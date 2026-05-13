# Adopcion, madurez y origen demo

## Origen validado

Esta guia se ha contrastado con la rama buena indicada por el usuario:

`C:\Desarrollo\AcademiaLasCortes`, rama `demo`.

Material de `demo` revisado y adaptado:

- `docs/engineering/README.md`
- `docs/engineering/00-principios.md`
- `docs/engineering/01-arquitectura-monorepo.md`
- `docs/engineering/02-frontend-standard.md`
- `docs/engineering/03-backend-standard.md`
- `docs/engineering/04-testing-strategy.md`
- `docs/engineering/05-code-style-and-quality.md`
- `docs/engineering/06-ci-cd-quality-gates.md`
- `docs/engineering/07-dependencies-and-upgrades.md`
- `docs/engineering/08-security-and-configuration.md`
- `docs/engineering/security-baseline.md`
- `docs/engineering/security-review.md`
- `docs/engineering/runtime-secrets.md`
- `docs/engineering/secret-scanning.md`
- `docs/engineering/dependency-audit.md`
- `docs/engineering/testing/backend-integration-testing.md`
- `docs/engineering/testing/frontend-testing-ci.md`
- `docs/engineering/testing-rollout.md`
- `docs/engineering/standards-rollout.md`
- `docs/engineering/adoption/maturity-model.md`
- `docs/engineering/adoption/checklist.md`
- `docs/engineering/adr/*.md`
- `tools/security/*.ps1`
- `.gitleaks.toml`

No se ha copiado documentacion especifica de aplicaciones de AcademiaLasCortes que no aplica directamente a iLiniumTech. Se han importado criterios, patrones y gates.

## Modelo de madurez iLiniumTech

Nivel 0: inventariado.

- AppBuilder analizado.
- Riesgos iniciales documentados.
- Guias de ingenieria disponibles.
- No hay runtime propio todavia.

Nivel 1: reproducible.

- Primer proyecto iLiniumTech con comandos locales.
- Lockfiles versionados si hay Node.
- Builds reproducibles.
- Secret scan ejecutable.
- Auditoria de dependencias ejecutable.

Nivel 2: calidad basica.

- Typecheck/lint configurados.
- Build backend/frontend estable.
- Configuracion fuera de codigo.
- CORS audit si hay API.
- SDD usado para cambios funcionales.

Nivel 3: pruebas fundamentales.

- Unit tests para extractores y transformadores de metadata.
- Integration tests para API o lectura de BBDD.
- Smoke E2E para el primer componente renderizado.
- Pruebas de seguridad sobre secretos, auth, CORS y SQL dinamico.

Nivel 4: gates CI/CD.

- CI ejecuta secret scan, dependency audit, build, lint, unit tests e integration tests.
- E2E smoke antes de marcar MVP listo.
- Resultados publicados.
- Gates bloqueantes para secretos y vulnerabilidades criticas/altas.

Nivel 5: referencia modernizada.

- El MVP puede extraer y renderizar componentes reales.
- Las specs SDD generan issues claros.
- La arquitectura evita repetir riesgos de AppBuilder.
- Las excepciones estan documentadas.

## Checklist operativa antes de tocar codigo

- [ ] Revisar `docs/APPBUILDER_ANALISIS_ARQUITECTURA.md`.
- [ ] Revisar esta guia de ingenieria.
- [ ] Confirmar si el cambio requiere spec SDD.
- [ ] Confirmar que no se necesitan secretos en Git.
- [ ] Identificar comandos locales afectados.
- [ ] Identificar pruebas esperadas.
- [ ] Identificar superficie de seguridad: auth, BBDD, SQL, CORS, GraphQL, workflows, datos.

## Checklist de cierre

- [ ] Codigo acotado al objetivo.
- [ ] Documentacion actualizada.
- [ ] Secret scan ejecutado.
- [ ] Dependency audit ejecutado si hay paquetes.
- [ ] CORS audit ejecutado si hay API.
- [ ] Pruebas proporcionales ejecutadas.
- [ ] Riesgos residuales declarados.
- [ ] Spec SDD actualizada si cambio el alcance.

## Regla para proyectos legacy

La regla importada desde `demo` es directa:

1. No empezar por un refactor masivo.
2. Cerrar secretos y dependencias.
3. Hacer el build reproducible.
4. Anadir una prueba sobre el flujo mas critico.
5. Convertir el aprendizaje en checklist.
6. Repetir por modulo.
