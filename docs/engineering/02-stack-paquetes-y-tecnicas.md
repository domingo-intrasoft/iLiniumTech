# Stack, paquetes y tecnicas

Este documento toma como referencia el trabajo de AcademiaLasCortes, pero no obliga a instalar paquetes hasta que exista una app concreta. Sirve como baseline para que los futuros desarrollos arranquen con tecnologia moderna y verificable.

## Frontend objetivo

Para una UI nueva:

- Vue 3 con Composition API.
- TypeScript en modo `strict`.
- Vite como bundler.
- Pinia para estado compartido.
- Vue Router para navegacion.
- PrimeVue si la app es administrativa o de CRUD dinamico.
- Tailwind CSS solo si se decide por ADR o si el producto lo pide claramente.
- Axios o `fetch` encapsulado en un cliente propio.
- Vitest para unit tests.
- Vue Test Utils para componentes.
- happy-dom o jsdom para entorno de test.
- Playwright para E2E.
- ESLint flat config.
- Prettier.
- vue-tsc.

Scripts objetivo:

```json
{
  "scripts": {
    "dev": "vite",
    "typecheck": "vue-tsc -b",
    "lint": "eslint . --max-warnings=0",
    "format": "prettier . --check",
    "test:unit": "vitest run",
    "test:unit:ci": "vitest run --reporter=default --reporter=junit --outputFile=reports/vitest-junit.xml",
    "build": "npm run typecheck && vite build",
    "preview": "vite preview"
  }
}
```

## Backend objetivo

Para una API nueva:

- ASP.NET Core Web API.
- .NET alineado con el SDK instalado en CI y con el runtime objetivo del proyecto.
- Nullable reference types activado.
- EF Core para persistencia estructurada.
- Dapper solo detras de repositorios seguros y con whitelists.
- FluentValidation o validacion equivalente para DTOs.
- OpenAPI/Swagger para contrato observable en entornos no productivos.
- xUnit.
- FluentAssertions.
- NSubstitute o Moq cuando haga falta mocking.
- `Microsoft.AspNetCore.Mvc.Testing` para integracion.
- Testcontainers para SQL Server si el comportamiento depende de SQL Server real.

Comandos objetivo:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet run --project <App>.Api.csproj
dotnet list package --vulnerable --include-transitive
```

## Tecnicas de resolucion recomendadas

Secretos:

- Variables de entorno y vault en vez de appsettings con valores reales.
- Gitleaks en local y CI.
- Rotacion inmediata si se detecta exposicion.

SQL dinamico:

- Query object o AST interno.
- Whitelist de tablas, columnas y operaciones.
- Parametros para valores.
- Pruebas de inyeccion sobre filtros y ordenaciones.

Cifrado:

- ASP.NET Core Data Protection, DPAPI o Key Vault para secretos que deban desencriptarse.
- Hashing robusto para passwords.
- No usar fallback silencioso que devuelva texto plano si falla el desencriptado.

Autorizacion:

- Policies y handlers.
- Permisos por operacion y recurso.
- Tests 401/403 para endpoints protegidos.

Render dinamico:

- Contratos JSON sanitizados.
- Tipos discriminados para componentes.
- Validacion del schema antes de renderizar.
- Lista cerrada de controles soportados en el MVP.

Expresiones y workflows:

- Sandbox y allowlist de funciones.
- Timeouts para ejecuciones.
- Registro de inputs/outputs redaccion de datos sensibles.
- No ejecutar scripts arbitrarios sin aislamiento.

Observabilidad:

- Logs estructurados.
- Correlation ID.
- Redaccion de secretos.
- Metricas basicas de errores y latencia.
- Trazas solo cuando no expongan datos sensibles.

## Dependencias nuevas

Antes de anadir una dependencia:

- justificar el problema que resuelve;
- comprobar si ya hay alternativa;
- revisar mantenimiento y vulnerabilidades;
- fijar version con lockfile;
- evitar upgrades mezclados con funcionalidad;
- anadir ADR si afecta a varias apps o al runtime base.
