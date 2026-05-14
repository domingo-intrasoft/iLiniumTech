# Extractor offline metadata Polizas

Herramienta local para `SDD-2026-002`. Genera un JSON sanitizado de metadata AppBuilder de Polizas para analisis, trazabilidad, SDD y scaffolding revisado.

No es runtime de iLiniumTech. El frontend y el backend no deben consumir este JSON para decidir pantallas, rutas, permisos ni SQL en produccion.

## Modos

- `Fixture`: modo por defecto, usa `fixtures/polizas-metadata.fixture.json` y no conecta a BBDD.
- `DryRun`: genera el contrato minimo esperado sin BBDD ni fixture externa.
- `Live`: intenta leer metadata `IAP_*` con consultas `SELECT` parametrizadas. Requiere entorno local autorizado y cuenta read-only.

## Comandos

Fixture offline:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\extractor\polizas-metadata\Invoke-PolizasMetadataExtractor.ps1 -Mode Fixture
```

Dry-run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\extractor\polizas-metadata\Invoke-PolizasMetadataExtractor.ps1 -Mode DryRun
```

Live autorizado:

```powershell
$env:ILINIUMTECH__MASTER_CONNECTION = "<connection-string-local-read-only>"
$env:ILINIUMTECH__PROGRAM_DATABASE = "AunnaTechADM"
powershell -NoProfile -ExecutionPolicy Bypass -File tools\extractor\polizas-metadata\Invoke-PolizasMetadataExtractor.ps1 -Mode Live
```

No guardes esas variables ni sus valores en Git. Los errores de `Live` se sanitizan y no imprimen la cadena de conexion.

## Salida

Por defecto se escribe en `reports/polizas-metadata/polizas.metadata.sanitized.json`, ruta ignorada por Git. El JSON separa:

- `appBuilder`: identificadores canonicos `2`, `10`, `2824`, `2825`, `354` y `146`;
- `traceability`: evidencia AppBuilder sanitizada;
- `metadata`: campos, lookups y `searchConfigParams` parseados;
- `scaffolding`: columnas, filtros, labels y pruebas sugeridas para revision humana;
- `warnings`: claves no soportadas o configuracion que requiere decision.

`QueryStatic` y otros fragmentos SQL quedan solo como evidencia redaccionada con fingerprint SHA-256. La herramienta no los ejecuta.

## Tests

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\extractor\polizas-metadata\tests\Test-PolizasMetadataExtractor.ps1
```

Los tests no requieren Pester, red ni BBDD real.
