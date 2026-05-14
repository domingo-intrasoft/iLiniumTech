param(
    [ValidateSet("Fixture", "DryRun", "Live")]
    [string]$Mode = "Fixture",

    [string]$FixturePath = (Join-Path $PSScriptRoot "fixtures\polizas-metadata.fixture.json"),

    [string]$OutputPath = "reports\polizas-metadata\polizas.metadata.sanitized.json",

    [switch]$PassThru
)

$ErrorActionPreference = "Stop"

Import-Module (Join-Path $PSScriptRoot "PolizasMetadataExtractor.psm1") -Force

$document = Invoke-PolizasMetadataExtraction -Mode $Mode -FixturePath $FixturePath -OutputPath $OutputPath

Write-Host "# Polizas Metadata Extractor"
Write-Host ""
Write-Host "Mode: $Mode"
Write-Host "Output: $OutputPath"
Write-Host "Runtime contract: false"
Write-Host "Notice: sanitized offline artifact for SDD/scaffolding review only."

if ($PassThru) {
    $document | ConvertTo-Json -Depth 20
}
