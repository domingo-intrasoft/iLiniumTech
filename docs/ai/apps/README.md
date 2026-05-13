# Apps y areas de iLiniumTech

Este directorio define las areas que el dispatcher y las issues SDD pueden usar para enrutar trabajo.

## Areas iniciales

| Area | Label | Rutas principales | Riesgo por defecto |
| --- | --- | --- | --- |
| Documentacion | `app:docs` | `README.md`, `docs/**` | Bajo |
| Backend | `app:backend` | `iLiniumTech.Backend/**` | Medio |
| Frontend | `app:frontend` | `iLiniumTech.Frontend/**` | Medio |
| Seguridad | `app:security` | `tools/security/**`, `.gitleaks.toml` | Alto |
| CI/CD | `app:infrastructure` | `.github/**` | Alto |
| Metadata AppBuilder | `app:metadata` | extractores offline, JSON sanitizado, trazabilidad/scaffolding | Alto |

## Regla de extension

Cada nueva app o area debe declarar:

- label;
- rutas incluidas;
- rutas sensibles;
- comandos locales obligatorios;
- pruebas minimas;
- datos o secretos que nunca pueden aparecer en artefactos.
