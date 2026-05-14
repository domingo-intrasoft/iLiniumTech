# Guia de ingenieria iLiniumTech

Esta carpeta trae a iLiniumTech las reglas de desarrollo, seguridad, pruebas y calidad aprendidas en `C:\Desarrollo\AcademiaLasCortes`, adaptadas al objetivo especifico de este proyecto: extraer conocimiento de AppBuilder, generar especificaciones SDD y construir un producto Vue/API sin repetir los riesgos de seguridad observados en el sistema actual.

Decision base: iLiniumTech no es un runtime dinamico tipo AppBuilder. La metadata heredada se usa para extraccion, trazabilidad y scaffolding revisado.

## Documentos

- [Principios](00-principios.md)
- [Seguridad y configuracion](01-seguridad-y-configuracion.md)
- [Stack, paquetes y tecnicas](02-stack-paquetes-y-tecnicas.md)
- [Pruebas y calidad](03-pruebas-y-calidad.md)
- [CI/CD y gates](04-ci-cd-y-gates.md)
- [Riesgos AppBuilder y resolucion moderna](05-appbuilder-riesgos-y-resolucion.md)
- [Adopcion, madurez y origen demo](06-adopcion-madurez-y-origen-demo.md)
- [Decisiones tecnicas iniciales](07-decisiones-tecnicas-iniciales.md)
- [Quality gate fase 7 y DoD](08-quality-phase-7-dod.md)
- [Roadmap objetivo final](../ROADMAP_OBJETIVO_FINAL.md)
- [Decision producto/arquitectura](../DECISION_PRODUCTO_ARQUITECTURA.md)
- [SDD Auth y permisos de producto](../sdd/specs/iLiniumTech/SDD-2026-005-auth-permisos-producto.md)
- [Plantilla SDD](../sdd/templates/spec-template.md)
- [Plantilla de revision de seguridad](../sdd/templates/security-review-template.md)

## Minimo profesional para empezar a desarrollar

Antes de implementar funcionalidades reales, el proyecto debe tener:

- instalacion reproducible con lockfiles cuando haya Node;
- build reproducible para cada app o servicio;
- `typecheck`, `lint`, `test:unit` y `build` cuando aplique;
- pruebas de integracion cuando se lea BBDD o se exponga API;
- smoke E2E para cualquier UI visible;
- escaneo de secretos con Gitleaks;
- auditoria de dependencias npm y NuGet;
- CORS restrictivo por configuracion;
- secretos fuera de Git;
- una spec SDD para cada cambio funcional relevante;
- checklist de seguridad si toca autenticacion, autorizacion, BBDD, workflows, pipelines o datos personales.

## Scripts incorporados

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/quality/Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-DependencyAudit.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-CorsAudit.ps1 -FailOnFindings
```

El gate MVP agrupa build, pruebas, smoke, auditorias, validacion documental y `git diff --check`. Los scripts de seguridad siguen disponibles por separado para diagnostico o ejecucion granular.
