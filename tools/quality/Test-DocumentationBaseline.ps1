param()

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")

function Test-RequiredFile {
    param([string]$Path)

    $fullPath = Join-Path $repoRoot $Path
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf)) {
        throw "Required file not found: $Path"
    }
}

function Test-ContentPattern {
    param(
        [string]$Path,
        [string]$Pattern,
        [string]$Description
    )

    $fullPath = Join-Path $repoRoot $Path
    $content = Get-Content -Raw -LiteralPath $fullPath
    if ($content -notmatch $Pattern) {
        throw "$Path does not contain expected content: $Description"
    }
}

Push-Location $repoRoot
try {
    $requiredFiles = @(
        "README.md",
        "docs/ROADMAP_OBJETIVO_FINAL.md",
        "docs/MVP_POLIZAS_PLAN.md",
        "docs/DECISION_PRODUCTO_ARQUITECTURA.md",
        "docs/engineering/README.md",
        "docs/engineering/08-quality-phase-7-dod.md",
        "docs/sdd/templates/spec-template.md",
        "docs/sdd/templates/security-review-template.md",
        ".github/workflows/ci.yml",
        ".github/workflows/security.yml",
        ".github/workflows/codeql.yml",
        ".github/workflows/preview-dry-run.yml",
        ".github/CODEOWNERS",
        ".github/pull_request_template.md",
        ".github/ISSUE_TEMPLATE/sdd-feature.yml",
        "tools/quality/Invoke-MvpQualityGate.ps1",
        "tools/security/Invoke-SecretScan.ps1",
        "tools/security/Invoke-DependencyAudit.ps1",
        "tools/security/Invoke-CorsAudit.ps1"
    )

    foreach ($file in $requiredFiles) {
        Test-RequiredFile -Path $file
    }

    $sddFiles = Get-ChildItem -Path "docs/sdd/specs/iLiniumTech" -Filter "SDD-*.md" -File
    if ($sddFiles.Count -lt 4) {
        throw "Expected at least 4 iLiniumTech SDD specs. Found $($sddFiles.Count)."
    }

    foreach ($sdd in $sddFiles) {
        $relativePath = Resolve-Path -Relative $sdd.FullName
        $content = Get-Content -Raw -LiteralPath $sdd.FullName
        foreach ($heading in @("## Metadata", "## Objetivo", "## Fuera de alcance", "## Criterios de aceptacion", "## Seguridad", "## Plan de pruebas", "## Definicion de hecho")) {
            if ($content -notmatch [regex]::Escape($heading)) {
                throw "$relativePath is missing required heading: $heading"
            }
        }
    }

    Test-ContentPattern -Path "README.md" -Pattern "ROADMAP_OBJETIVO_FINAL\.md" -Description "canonical roadmap link"
    Test-ContentPattern -Path "docs/engineering/README.md" -Pattern "ROADMAP_OBJETIVO_FINAL\.md" -Description "canonical roadmap link"
    Test-ContentPattern -Path "docs/ROADMAP_OBJETIVO_FINAL.md" -Pattern "iLiniumTech no es un runtime dinamico tipo AppBuilder" -Description "architecture decision"
    Test-ContentPattern -Path "docs/ROADMAP_OBJETIVO_FINAL.md" -Pattern "Jefe calidad, CI y documentacion" -Description "quality owner section"
    Test-ContentPattern -Path "docs/ROADMAP_OBJETIVO_FINAL.md" -Pattern "Bloqueos actuales" -Description "real environment blockers"
    Test-ContentPattern -Path ".github/workflows/ci.yml" -Pattern "dotnet build" -Description "backend build in CI"
    Test-ContentPattern -Path ".github/workflows/ci.yml" -Pattern "npm run format" -Description "frontend format in CI"
    Test-ContentPattern -Path ".github/workflows/ci.yml" -Pattern "npm run lint" -Description "frontend lint in CI"
    Test-ContentPattern -Path ".github/workflows/ci.yml" -Pattern "Test-DocumentationBaseline\.ps1" -Description "documentation baseline in CI"
    Test-ContentPattern -Path ".github/workflows/security.yml" -Pattern "gitleaks" -Description "secret scan in security workflow"
    Test-ContentPattern -Path ".github/workflows/codeql.yml" -Pattern "github/codeql-action/analyze" -Description "CodeQL analysis workflow"
    Test-ContentPattern -Path ".github/workflows/preview-dry-run.yml" -Pattern "DEPLOY_PREVIEW_ENABLED: `"false`"" -Description "preview deploy disabled by default"
    Test-ContentPattern -Path ".github/CODEOWNERS" -Pattern "OWNER-REVIEW-REQUIRED" -Description "placeholder owner for sensitive paths"
    Test-ContentPattern -Path ".github/CODEOWNERS" -Pattern "Replace it with real users or teams" -Description "CODEOWNERS enforcement warning"
    Test-ContentPattern -Path ".github/pull_request_template.md" -Pattern "Riesgos residuales" -Description "PR residual risks section"

    Write-Host "# Documentation Baseline"
    Write-Host ""
    Write-Host "Required docs, SDD headings, CI files and roadmap links are present."
}
finally {
    Pop-Location
}
