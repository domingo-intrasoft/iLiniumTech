# Evidencia Polizas visual parity AppBuilder

Fecha: 2026-05-18
Rama: `codex/polizas-crud-bbdd`
SDD: `docs/sdd/specs/iLiniumTech/SDD-2026-014-polizas-appbuilder-visual-parity.md`
Tarea: `T-043-POL-APPBUILDER-VISUAL-SHELL`

## Objetivo del corte

Aproximar `/polizas` al aspecto operativo de la pantalla AppBuilder publicada, sin convertir la metadata en runtime y sin fabricar datos.

## Cambios aplicados

- La toolbar de `Polizas` queda mas compacta y con grupos de iconos mas cercanos a AppBuilder.
- El buscador principal se integra como franja de grid con placeholder `Buscar...`.
- Los filtros detallados siguen disponibles, pero quedan en panel colapsable para priorizar la lectura de tabla.
- La tabla pasa a formato denso AppBuilder-like con scroll horizontal y columnas objetivo:
  `Seleccion`, `Acciones`, `Cia.`, `Poliza`, `Certif.`, `N. Documento`, `Cliente`, `Situacion`, `Ramo`, `Riesgo/Matric.`.
- `Poliza` se muestra como enlace subrayado solo cuando el detalle esta permitido.
- `Situacion` usa badge compacto; el valor real `Vigor` se presenta como `En Vigor`.
- Acciones de detalle, edicion y baja mantienen permisos reales existentes.
- El menu contextual de fila y las acciones no implementadas permanecen deshabilitadas.

## Datos no inventados

La API/listado actual no entrega estos datos en el contrato de `PolizaListItem`:

- certificado;
- numero de documento;
- riesgo o matricula;
- logos reales de compania.

Para no simular datos, el primer corte muestra valor neutro `No informado` en esas columnas y documenta la diferencia como pendiente tecnico. `Cia.` usa el texto real de `compania` si el backend lo entrega; no se dibujan logos falsos.

## Validacion ejecutada

```powershell
cd .\iLiniumTech.Frontend
npx vitest run src/features/polizas/PolizasFilters.test.ts src/features/polizas/PolizasView.test.ts src/features/polizas/PolizasTable.test.ts
```

Resultado: `23/23` tests OK.

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
```

Resultados: format OK, lint OK, unit completo `198/198` tests OK y build OK.

```powershell
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Resultados: baseline documental OK, secret scan sin leaks y `git diff --check` sin errores. Git mostro avisos de finales de linea CRLF habituales en Windows, sin fallar el comando.

Smoke visual local T-043:

- URL verificada en navegador: `http://127.0.0.1:5175/polizas?page=1&pageSize=25`.
- Resultado: toolbar de polizas visible, buscador `Buscar...` visible, tabla `appbuilder-table` visible, columnas objetivo visibles, badge de situacion visible, scopes de flota/colectivas/externas visibles y `25` filas renderizadas.

## Smoke visual T-044

Fecha: 2026-05-18.

Smoke en navegador embebido local:

- URL verificada: `http://127.0.0.1:5175/polizas?page=1&pageSize=25`.
- Resultado: toolbar, buscador, tabla y columnas objetivo visibles.
- Observacion: el contexto backend local activo devolvia `0` polizas y no permitia validar visualmente el badge de situacion.

Smoke controlado de escritorio con Vite temporal en modo fixture:

```powershell
cd .\iLiniumTech.Frontend
$env:VITE_USE_BACKEND = "false"
npm run dev -- --host 127.0.0.1 --port 5180
# Playwright headless: viewport 1440x900, sesion demo local, /polizas?page=1&pageSize=25
```

Resultado:

- URL verificada: `http://127.0.0.1:5180/polizas?page=1&pageSize=25`.
- `hasToolbar=true`
- `hasSearch=true`
- `hasGrid=true`
- `hasStatusBadge=true`
- `statusBadgeText=En Vigor`
- `hasColumns=true`
- `missingColumns=[]`
- `hasScopes=true`
- `rowCount=2`
- `viewportWidth=1440`

Conclusion T-044: el corte visual queda aceptado para escritorio y documentado. No se aplicaron cambios de codigo adicionales en T-044.

## Diferencias pendientes frente a AppBuilder

- Los logos de aseguradora no se muestran porque no hay fuente autorizada en el contrato actual.
- `Certif.` queda sin dato hasta contrato API/UAT.
- `N. Documento`, `Cliente`, `Ramo` y `Riesgo/Matric.` pasan al carril de paridad de datos `T-047`.
- La barra superior global se aproxima con el shell actual; no se copian usuarios, contadores ni datos de entorno de AppBuilder.
- Los scopes `Flotas`, `Colectivas` y `Externas` siguen bloqueados o como rutas estaticas sin datos reales.

## Refinamiento T-045

Fecha: 2026-05-18.

Motivo: el usuario comparo la captura de AppBuilder publicado con el MVP y marco diferencias significativas de chrome, menu lateral, buscador y tabla.

Cambios aplicados:

- `AppShell` permite un modo visual AppBuilder para Polizas, con barra superior naranja de entorno demo, breadcrumb con slash final y badge `AunnaTech | Portal (DEMO)`.
- `AppSideMenu` permite ocultar submenus y leyenda en modo AppBuilder para aproximarse al menu lateral publicado sin eliminar rutas ni permisos.
- `/polizas` activa ese chrome especifico y oculta visualmente el runtime strip cuando solo contiene contexto normal; errores y avisos funcionales siguen existiendo en DOM y se muestran si aparecen.
- El buscador de Polizas se compacta como franja de grid con botones icon-only, manteniendo textos accesibles y acciones bloqueadas.
- La tabla queda mas plana y densa: summary visual oculto, warning de escritura solo accesible, acciones de fila reducidas al menu contextual salvo escrituras MVP realmente permitidas.
- `situacionpoliza-EV`, `EV`, `Vigor` y equivalentes se presentan como badge verde `En Vigor` sin cambiar el valor de backend.
- Las celdas sin contrato de listado dejan de mostrar `No informado` de forma visible; conservan `aria-label` y `title` para no simular datos.
- `Cia.` usa un fallback visual compacto basado en el valor entregado por la API/fixture, sin logos falsos.

Validacion dirigida ejecutada:

```powershell
cd .\iLiniumTech.Frontend
npx vitest run src/features/polizas/PolizasFilters.test.ts src/features/polizas/PolizasView.test.ts src/features/polizas/PolizasTable.test.ts src/layout/AppShell.test.ts src/layout/AppSideMenu.test.ts
```

Resultado: `38/38` tests OK.

Validacion completa ejecutada:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
```

Resultados: format OK, lint OK, unit completo `201/201` tests OK y build OK.

```powershell
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
git diff --check
```

Resultados: baseline documental OK, secret scan sin leaks y `git diff --check` sin errores. Git mostro avisos de finales de linea CRLF habituales en Windows, sin fallar el comando.

Smoke visual ejecutado:

- Navegador embebido local: `http://127.0.0.1:5175/polizas?page=1&pageSize=25`.
- Resultado medido: barra demo visible, menu lateral sin subnav/leyenda, runtime normal oculto, buscador visible, tabla visible, `25` filas reales renderizadas, badge `En Vigor`, sin `No informado` visible en celdas pendientes.
- Smoke controlado fixture en `http://127.0.0.1:5181/polizas?page=1&pageSize=25`, viewport `1900x970`: barra demo `1900x26`, sidebar `68px`, topbar `54px`, toolbar `43px`, buscador `49px`, subnav/leyenda ausentes, badge `En Vigor`.

Notas de honestidad:

- T-045 no incorporo logos reales ni campos sensibles porque aun no estaban en contrato de listado.
- T-047 incorpora documento, cliente, ramo descriptivo y riesgo porque el usuario lo convierte en objetivo explicito de MVP local/demo.
- La paridad pixel-perfect queda bloqueada hasta que producto/UAT autorice origen de logos, campos sensibles y reglas de visualizacion exactas para produccion.

## Paridad de datos T-047

Fecha: 2026-05-18.

Motivo: el usuario detecta que la comparacion contra AppBuilder sigue fallando porque el grid local no muestra `N. Documento`, nombre de cliente, `Ramo` descriptivo ni `Riesgo/Matric.`.

Analisis documentado:

- `docs/appbuilder/pages/polizas/data-contract-analysis.md`
- `docs/appbuilder/pages/polizas/components/listado-grid.md`

Cambios aplicados:

- `PolizaListItem` incorpora `documento` y `riesgo`.
- SQL de lectura usa `vw_ClientePolizas`, porque `Pantalla_Polizas` no contiene `NumDocumento`, `RazonSocial` ni `Riesgo`.
- Se proyectan `NumDocumento`, `RazonSocial`, `Ramo` y `Riesgo` desde la vista de cliente-polizas.
- `ramo` ya llega descriptivo en `vw_ClientePolizas`, siguiendo la logica AppBuilder de lookup descriptivo.
- La busqueda `cliente` contempla nombre completo, documento y codigo cliente.
- `PolizasTable.vue` deja de tratar `N. Documento` y `Riesgo/Matric.` como datos ausentes.
- `Autos Particulares` sanitiza `documento`, `clienteId` y `riesgo` al seguir fuera del objetivo activo.

Validacion dirigida inicial:

```powershell
dotnet test .\iLiniumTech.Backend\tests\iLiniumTech.Backend.Tests\iLiniumTech.Backend.Tests.csproj --configuration Release --filter "PolizasSqlQueryBuilderTests|PolizasApiTests"
cd .\iLiniumTech.Frontend
npm run test:unit -- PolizasTable.test.ts polizasApi.test.ts
```

Resultados: backend dirigido `95/95` OK y frontend dirigido `15/15` OK.

Correccion tras smoke SQL real:

- El primer intento contra BBDD local demostro que `Pantalla_Polizas` no contiene `NumDocumento`, `RazonSocial` ni `Riesgo`.
- Se inspecciono solo esquema, sin datos ni credenciales, y se eligio `vw_ClientePolizas` como origen explicito de lectura enriquecida.
- La escritura sigue usando `dbo.Poliza`; la visibilidad tecnica post-create sigue apoyandose en `Pantalla_Polizas`.

Smoke API/UI real final:

```powershell
dotnet run --project $env:TEMP\iliniumtech-api-crud-visible-smoke\iliniumtech-api-crud-visible-smoke.csproj
```

Resultados sanitizados:

- API: `ApiSqlCrudVisibleSmoke=OK`;
- create `201`;
- detail post-create `200`;
- search post-create `1`;
- update `204`;
- detail post-update `200`;
- delete `204`;
- detail post-delete `404`;
- limpieza exacta API/UI sin residuales;
- UI: `UiSqlCrudVisibleSmoke=OK`.

Smoke visual en navegador embebido con procesos frescos:

- URL: `http://127.0.0.1:5175/polizas?page=1&pageSize=25`;
- `rowCount=25`;
- columnas `N. Documento`, `Cliente`, `Ramo` y `Riesgo/Matric.` visibles;
- sin skeleton, sin error y sin `No informado` visible;
- badge `En Vigor` visible;
- el valor de cliente ya no cae al codigo `4692`; se muestra texto descriptivo desde la vista de BBDD local.

Validacion completa final:

```powershell
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit
npm run build
cd ..
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
git diff --check
```

Resultados: backend `149/149` OK, frontend unit `201/201` OK, format/lint/build OK, documentation baseline OK, secret scan sin leaks, dependency audit `0` findings, CORS audit OK y diff check sin errores.

## Riesgos residuales

- La paridad visual pixel-perfect aun requiere smoke visual manual con usuario y activos reales autorizados.
- Densificar la tabla puede requerir ajustes responsive posteriores.
- Cualquier dato sensible nuevo para rellenar columnas debe pasar por SDD, backend explicito, permisos y UAT.
