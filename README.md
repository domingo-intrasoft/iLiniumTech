# iLiniumTech

Repositorio de trabajo para extraer, documentar y convertir el comportamiento de AppBuilder en una base de desarrollo guiada por especificaciones.

La primera entrega del proyecto es una memoria tecnica del sistema actual, pensada para que futuros agentes de IA entiendan la arquitectura, el modelo dinamico de UI, las conexiones de BBDD, los flujos, las formulas y el camino hacia un MVP sin volver a analizar todo el codigo fuente.

Documento principal:

- [docs/APPBUILDER_ANALISIS_ARQUITECTURA.md](docs/APPBUILDER_ANALISIS_ARQUITECTURA.md)
- [docs/engineering/README.md](docs/engineering/README.md)
- [docs/PLAN_CICD_GITHUB_ONLY.md](docs/PLAN_CICD_GITHUB_ONLY.md)
- [docs/MVP_POLIZAS_PLAN.md](docs/MVP_POLIZAS_PLAN.md)
- [docs/MVP_POLIZAS_DIFERENCIAS.md](docs/MVP_POLIZAS_DIFERENCIAS.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md](docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md)

## Principios de trabajo

- No guardar credenciales, cadenas de conexion completas ni secretos en este repositorio.
- Documentar rutas, responsabilidades y contratos tecnicos con suficiente detalle para crear issues SDD.
- Mantener el proyecto preparado para una fase posterior de extraccion desde el entorno de pruebas y generacion automatica de historias.
- Aplicar desde el inicio reglas de seguridad, pruebas y calidad heredadas de AcademiaLasCortes y adaptadas a los riesgos detectados en AppBuilder.

## Estado actual

Fase documental completada y primer corte MVP creado para el componente de polizas:

- metadata AppBuilder localizada y documentada;
- backend y frontend creados;
- UI local de polizas disponible;
- pruebas, build, formato y gates de seguridad ejecutados.

## Proyectos creados

- `iLiniumTech.Backend`: API .NET 10 con contrato MVP de polizas.
- `iLiniumTech.Frontend`: Vue 3 + Vite + TypeScript para render inicial del componente de polizas.

## Comandos base

Backend:

```powershell
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run lint
npm run test:unit
npm run build
```

El frontend requiere Node.js 20.19 o superior, alineado con las guias traidas de `AcademiaLasCortes` rama `demo`.
