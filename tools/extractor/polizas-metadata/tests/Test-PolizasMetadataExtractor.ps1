param()

$ErrorActionPreference = "Stop"

$toolRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
Import-Module (Join-Path $toolRoot "PolizasMetadataExtractor.psm1") -Force

function Assert-True {
    param(
        [bool]$Condition,
        [string]$Message
    )

    if (-not $Condition) {
        throw $Message
    }
}

function Assert-Equal {
    param(
        [object]$Actual,
        [object]$Expected,
        [string]$Message
    )

    if ($Actual -ne $Expected) {
        throw "$Message Actual=[$Actual] Expected=[$Expected]"
    }
}

$tempDir = Join-Path $env:TEMP ("iliniumtech-polizas-extractor-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

try {
    $outputPath = Join-Path $tempDir "polizas.metadata.sanitized.json"
    $document = Invoke-PolizasMetadataExtraction -Mode Fixture -OutputPath $outputPath

    Assert-True (Test-Path -LiteralPath $outputPath -PathType Leaf) "Debe escribir el JSON sanitizado."
    Assert-Equal $document.runtimeContract $false "La salida no debe declararse contrato runtime."
    Assert-Equal $document.appBuilder.applicationId 2 "Debe incluir applicationId canonico."
    Assert-Equal $document.appBuilder.menuId 10 "Debe incluir menuId canonico."
    Assert-Equal $document.appBuilder.rootComponentId 2824 "Debe incluir rootComponentId canonico."
    Assert-Equal $document.appBuilder.crudComponentId 2825 "Debe incluir crudComponentId canonico."
    Assert-Equal $document.appBuilder.componentDataSourceId 354 "Debe incluir componentDataSourceId canonico."
    Assert-Equal $document.appBuilder.dataSourceId 146 "Debe incluir dataSourceId canonico."
    Assert-True ($document.metadata.fields.Count -ge 17) "Debe mapear los campos relevantes de Polizas."
    Assert-True ($document.scaffolding.columns.Count -ge 10) "Debe separar sugerencias de columnas."
    Assert-True ($document.scaffolding.filters.Count -ge 8) "Debe separar sugerencias de filtros."

    $json = Get-Content -Raw -LiteralPath $outputPath
    Assert-True ($json -notmatch "SELECT Poliza") "No debe persistir SQL raw de QueryStatic."
    Assert-True ($json -notmatch "SELECT \* FROM") "No debe persistir SQL raw del datasource."
    Assert-True ($json -match "REDACTED_SQL_FRAGMENT") "Debe marcar SQL heredado como redaccionado."

    $polizaField = $document.metadata.fields | Where-Object { $_.name -eq "Poliza" } | Select-Object -First 1
    Assert-Equal $polizaField.searchConfigParams.parsed.control "text" "Debe parsear searchConfigParams JSON."
    Assert-Equal $polizaField.searchConfigParams.parsed.operator "contains" "Debe conservar operadores soportados."

    $aplicacionField = $document.metadata.fields | Where-Object { $_.name -eq "Aplicacion" } | Select-Object -First 1
    Assert-Equal $aplicacionField.searchConfigParams.parsed.control "text" "Debe parsear searchConfigParams clave-valor."

    $tipoPolizaField = $document.metadata.fields | Where-Object { $_.name -eq "IdTipoPoliza" } | Select-Object -First 1
    Assert-Equal $tipoPolizaField.searchConfigParams.parsed.control "select" "No debe redaccionar controles UI tipo select como si fueran SQL."

    $redacted = Protect-MetadataValue -Value "Server=db;Initial Catalog=x;User ID=u"
    Assert-Equal $redacted "[REDACTED]" "Debe redaccionar textos con forma de connection string."

    $redactedSql = Protect-MetadataValue -Value "SELECT * FROM Pantalla_Polizas"
    Assert-Equal $redactedSql "[REDACTED_SQL_FRAGMENT]" "Debe redaccionar fragmentos SQL reales."

    $dryRun = Invoke-PolizasMetadataExtraction -Mode DryRun
    Assert-Equal $dryRun.mode "DryRun" "Debe soportar modo DryRun sin BBDD."
    Assert-True ($dryRun.warnings -contains "DryRun no conecta con BBDD; solo valida el contrato y la forma de salida.") "DryRun debe explicar su alcance."

    Write-Host "# Polizas Metadata Extractor Tests"
    Write-Host ""
    Write-Host "All extractor unit tests passed."
}
finally {
    Remove-Item -LiteralPath $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}
