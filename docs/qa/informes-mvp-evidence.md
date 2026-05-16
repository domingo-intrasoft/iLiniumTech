# Informes MVP read-only evidence

Fecha: 2026-05-16

## Alcance

Pantalla `/informes` evolucionada desde placeholder a catalogo estatico de riesgo/readiness.

Cambios esperados:

- Vista Vue estatica con `AppShell`, filtros locales y tabla fixture.
- Catalogo de 4 categorias candidatas marcadas como no confirmadas, no ejecutables o pendientes de SDD.
- Copy explicito: sin API, sin descarga, sin ejecucion y sin informes aprobados todavia.
- Acciones de ejecucion, descarga e historial deshabilitadas.
- Sin conexion backend, sin metadata runtime y sin uso de fuentes heredadas como contrato productivo.

## Comandos recomendados

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- InformesView
npm run build
```

Auditoria minima:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
```

Gate completo antes de PR integradora:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

## Evidencia esperada

- Renderiza `Informes` con `Solo lectura`, `Fixture local sin API`, `Sin ejecucion ni descarga` y `Evidencia AppBuilder insuficiente`.
- Muestra 4 categorias candidatas paginadas.
- Filtra por texto, area, estado de aprobacion y riesgo.
- Empty state cuando no hay coincidencias.
- Botones `Ejecutar`, `Descargar` y acciones de fila permanecen deshabilitados.

## Evidencia ejecutada en integracion

Comandos:

```powershell
cd .\iLiniumTech.Frontend
npm run format
npm run lint
npm run test:unit -- InformesView LiquidacionesCompaniaView LiquidacionesColaboradorView
npm run test:unit
npm run build
```

Resultado:

- Formato correcto.
- Lint correcto.
- Tests dirigidos: 3 archivos, 12 tests pasados.
- Regresion frontend completa: 38 archivos, 133 tests pasados.
- Build frontend correcto.

Validaciones adicionales desde raiz:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1 -NoReport
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -ReportDir $env:TEMP\... -FailOnFindings
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
git diff --check
```

Resultado:

- Documentation baseline correcto.
- Secret scan sin leaks.
- CORS audit sin findings.
- Dependency audit con 0 findings.
- Backend: 85 tests pasados.
- Whitespace diff correcto; Git solo aviso normalizacion LF/CRLF.

Smoke visual en navegador:

- Ruta: `http://127.0.0.1:5174/informes`.
- Estado inicial: titulo `Informes`, `Fixture local sin API`, `Sin ejecucion ni descarga`, `4 categorias`.
- Filtro probado: texto `recibos`.
- Resultado filtrado: `1 categoria`, fila `Recibos y remesas`, estado `Requiere SDD`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- No existe catalogo funcional aprobado de informes reales.
- No hay permisos `informes.read`, `informes.execute` ni `informes.download` definidos por backend.
- No hay API de catalogo, ejecucion, historial ni descarga.
- Reporting puede implicar PII, importes, comisiones, facturacion o datos bancarios; requiere SDD y revision de seguridad antes de activar descargas.
- La evidencia heredada solo permite listar areas candidatas; no valida columnas, parametros, propietarios funcionales ni UAT.

## Bloqueos externos

- Falta SDD funcional aprobada para Informes.
- Falta inventario validado de informes, formatos, parametros, permisos y responsables.
- Falta decision de arquitectura sobre generacion local, delegada o externa de documentos.
- Falta validacion DBA/UAT sobre fuentes, campos sensibles, broker, retencion y auditoria.
