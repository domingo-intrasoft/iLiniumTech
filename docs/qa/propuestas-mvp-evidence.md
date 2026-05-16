# Evidencia QA - Propuestas MVP read-only

Fecha: 2026-05-16.

## Alcance

- Pantalla `/propuestas` evolucionada desde placeholder a Vue estatico con `AppShell`.
- Datos fixture locales sanitizados, sin API y sin origen real confirmado.
- Filtros locales por referencia, estado, ramo/tipo y fecha desde.
- Tabla read-only con resultado, paginacion, empty state y acciones no operativas.
- Crear, convertir a poliza, documentos y exportar quedan deshabilitados.

## Fuera de alcance

- Sin API de propuestas.
- Sin emision real.
- Sin conversion real a poliza.
- Sin lectura de metadata heredada en runtime.
- Sin detalle, documentos, permisos productivos ni origen de datos real.

## Datos y seguridad

- Fixture con cuatro registros `PROP-2026-0001` a `PROP-2026-0004`.
- Solicitantes anonimos y canales demo.
- Importes expresados como etiquetas demo, no cuantias reales.
- Sin documento legal, telefono, email, direccion, cuenta bancaria, IBAN, tokens ni connection strings.
- SDD y origen de datos permanecen pendientes.

## Pruebas previstas

- `npm run test:unit -- PropuestasView`
- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings`

## Resultado

- `npm run test:unit -- PropuestasView`: OK, 4 tests passed.
- `npm run format`: OK, todos los ficheros formateados.
- `npm run lint`: OK, sin warnings.
- `npm run test:unit`: OK, 38 test files passed, 142 tests passed.
- `npm run build`: OK, typecheck y build Vite completados.
- Secret scan: OK, no leaks found.
- Dependency audit: OK tras reintento por bloqueo temporal de reporte; total finding count 0.
- Documentation baseline: OK.
- CORS audit: OK, sin findings.
- Backend: OK, 85 tests pasados.
- `git diff --check`: OK, sin errores de whitespace.

## Smoke visual

- Ruta: `http://127.0.0.1:5174/propuestas`.
- Estado inicial: titulo `Propuestas`, `Fixture local sin API`, `Sin emision ni conversion`, `4 propuestas`.
- Filtro probado: referencia/solicitante `0002`.
- Resultado filtrado: `1 propuesta`, fila `PROP-2026-0002`, solicitante `Solicitante anonimo 2`.
- Consola: 0 errores nuevos.

## Riesgos residuales

- Falta SDD funcional de Propuestas.
- Falta confirmar si Propuestas equivale o no a otro concepto de negocio.
- Falta definir origen de datos, permisos, UAT owner y contrato API.
- La pantalla solo es evidencia MVP estatica read-only.
