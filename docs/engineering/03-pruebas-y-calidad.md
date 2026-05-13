# Pruebas y calidad

## Piramide objetivo

1. Unitarias: muchas, rapidas, sin red ni BBDD real.
2. Integracion: menos, validan API, persistencia, auth y wiring.
3. E2E: pocas, cubren flujos criticos completos.
4. Seguridad: gates automatizados y pruebas focalizadas en riesgos.

## Pruebas unitarias prioritarias

Extractor de metadata:

- mapea componentes AppBuilder a contratos iLiniumTech;
- filtra campos no soportados;
- conserva orden y jerarquia;
- redacciona valores sensibles;
- falla con errores claros si falta metadata obligatoria.

Datasources:

- valida nombres de tabla, vista y columna contra whitelist;
- parametriza valores;
- rechaza filtros no soportados;
- calcula pagina, orden y limites de forma estable.

Expresiones:

- evalua solo funciones permitidas;
- controla errores y timeouts;
- no ejecuta codigo arbitrario.

Render:

- cada tipo de componente soportado genera un modelo visible;
- props y eventos tienen tipos;
- estados vacio, loading y error no rompen layout.

## Pruebas backend de integracion

Cuando exista API:

- arranque con configuracion valida;
- arranque bloqueado si faltan secretos obligatorios;
- endpoints protegidos devuelven 401/403 sin token o sin permiso;
- endpoints publicos estan documentados;
- CORS acepta solo origenes configurados;
- errores no filtran SQL, rutas fisicas ni connection strings;
- lectura de BBDD usa usuario de minimo privilegio;
- operaciones Search y Data se comportan con datos de prueba.

Herramientas:

- xUnit.
- FluentAssertions.
- `Microsoft.AspNetCore.Mvc.Testing`.
- SQLite in-memory si basta.
- Testcontainers SQL Server si hace falta SQL Server real.

## Pruebas frontend

Herramientas:

- Vitest.
- Vue Test Utils.
- happy-dom o jsdom.
- Playwright.

Priorizar:

- stores o composables con reglas;
- transformadores de metadata;
- validadores;
- componentes dinamicos con reglas de interaccion;
- clientes API y tratamiento de errores.

Evitar:

- snapshots enormes;
- probar internals de PrimeVue;
- duplicar E2E con tests unitarios fragiles.

## E2E Playwright

Estructura recomendada:

```text
tests/e2e/tests/
  smoke/
  critical/
  regression/
```

Smoke MVP minimo:

- la app arranca;
- carga configuracion sin secretos en consola;
- renderiza el componente real extraido;
- muestra datos o estado vacio controlado;
- bloquea una accion sin permiso;
- no hay errores de red inesperados.

Config recomendada:

- screenshots solo en fallo;
- trace en primer retry;
- JUnit en CI;
- fixtures estables;
- datos anonimizados.

## Pruebas de seguridad obligatorias

Gates automatizados:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-CorsAudit.ps1 -FailOnFindings
```

Casos de seguridad a cubrir:

- no hay secretos en repo ni artefactos;
- no hay `AllowAnyOrigin`;
- no hay origenes CORS hardcodeados en `Program.cs`;
- filtros maliciosos no modifican SQL estructural;
- usuarios sin permiso no leen ni escriben;
- logs no contienen token, password, connection string ni datos personales innecesarios;
- dependencias high/critical quedan bloqueadas o justificadas.

## Definicion de done

Un cambio funcional no esta terminado hasta que:

- cumple la spec SDD;
- tiene pruebas proporcionales al riesgo;
- pasa build/typecheck/lint si existen;
- pasa secret scan;
- documenta pruebas ejecutadas;
- declara cualquier riesgo residual.
