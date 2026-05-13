# iLiniumTech

Repositorio de trabajo para transformar conocimiento heredado de AppBuilder en un producto iLiniumTech guiado por especificaciones.

Decision critica: iLiniumTech no sera un runtime dinamico tipo AppBuilder. La metadata solo sirve para extraccion, migracion, trazabilidad y scaffolding inicial. El producto final sera un frontend Vue estatico mantenido como codigo fuente y un backend API de datos con contratos explicitos.

Documento principal:

- [docs/DECISION_PRODUCTO_ARQUITECTURA.md](docs/DECISION_PRODUCTO_ARQUITECTURA.md)
- [docs/APPBUILDER_ANALISIS_ARQUITECTURA.md](docs/APPBUILDER_ANALISIS_ARQUITECTURA.md)
- [docs/engineering/README.md](docs/engineering/README.md)
- [docs/PLAN_CICD_GITHUB_ONLY.md](docs/PLAN_CICD_GITHUB_ONLY.md)
- [docs/MVP_POLIZAS_PLAN.md](docs/MVP_POLIZAS_PLAN.md)
- [docs/MVP_POLIZAS_DIFERENCIAS.md](docs/MVP_POLIZAS_DIFERENCIAS.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md](docs/sdd/specs/iLiniumTech/SDD-2026-001-polizas-mvp.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md](docs/sdd/specs/iLiniumTech/SDD-2026-002-extractor-metadata-polizas.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md](docs/sdd/specs/iLiniumTech/SDD-2026-003-repositorio-sql-polizas.md)
- [docs/sdd/specs/iLiniumTech/SDD-2026-004-github-ci-baseline.md](docs/sdd/specs/iLiniumTech/SDD-2026-004-github-ci-baseline.md)
- [docs/ai/github-ai-policy.md](docs/ai/github-ai-policy.md)

## Principios de trabajo

- No guardar credenciales, cadenas de conexion completas ni secretos en este repositorio.
- Documentar rutas, responsabilidades y contratos tecnicos con suficiente detalle para crear issues SDD.
- Mantener la metadata de AppBuilder como insumo de extraccion/scaffolding, no como motor runtime del producto.
- Convertir cualquier scaffold util en codigo iLiniumTech revisado: Vue estatico y API backend explicita.
- Aplicar desde el inicio reglas de seguridad, pruebas y calidad heredadas de AcademiaLasCortes y adaptadas a los riesgos detectados en AppBuilder.

## Estado actual

Fase documental completada, decision de arquitectura fijada y primer corte MVP creado para el componente de polizas:

- metadata AppBuilder localizada y documentada;
- backend y frontend creados;
- UI local de polizas disponible;
- pruebas, build, formato y gates de seguridad ejecutados.
- siguiente orientacion: extractor offline/local, repositorio SQL read-only y evolucion frontend/backend por SDD, sin runtime dinamico.

## Proyectos creados

- `iLiniumTech.Backend`: API .NET 10 con contrato MVP de polizas.
- `iLiniumTech.Frontend`: Vue 3 + Vite + TypeScript para pantalla estatica inicial del componente de polizas.

## Configuracion local de polizas

Por defecto el backend usa fixtures anonimizados:

```json
"Polizas": {
  "Repository": "InMemory"
}
```

Para activar lectura SQL read-only en un entorno local autorizado:

```powershell
$env:Polizas__Repository = "Sql"
$env:ConnectionStrings__PolizasReadOnly = "<connection-string-local>"
# o bien
$env:ILINIUMTECH__POLIZAS_CONNECTION = "<connection-string-local>"
```

No guardar esas cadenas en Git. El repositorio SQL usa whitelist de columnas y parametros para valores; no interpreta metadata AppBuilder en runtime.

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

Para conectar el frontend al backend local en desarrollo, usar valores publicos de entorno como `VITE_USE_BACKEND=true`, `VITE_API_BASE_URL=http://localhost:5146` y `VITE_ILINIUMTECH_API_KEY=<clave-local>`. Esa clave de frontend solo sirve para desarrollo/demo; no debe tratarse como secreto de produccion.
