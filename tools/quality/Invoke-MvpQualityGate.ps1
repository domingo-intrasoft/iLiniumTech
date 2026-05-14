param(
    [switch]$SkipSecurity,
    [switch]$SkipFrontendBuild,
    [switch]$SkipSmoke,
    [string]$NodeExe = "",
    [string]$FrontendSmokeUrl = "",
    [string]$BackendSmokeUrl = "http://127.0.0.1:5146",
    [string]$BackendSmokeApiKey = "local-quality-gate-key"
)

$ErrorActionPreference = "Stop"

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..\..")
$frontendRoot = Join-Path $repoRoot "iLiniumTech.Frontend"
$backendSolution = Join-Path $repoRoot "iLiniumTech.Backend\iLiniumTech.Backend.slnx"
$backendApiProject = Join-Path $repoRoot "iLiniumTech.Backend\src\iLiniumTech.Backend.Api"
$reportsRoot = Join-Path $repoRoot "quality-reports"
$frontendReports = Join-Path $frontendRoot "reports"
$polizasMetadataExtractorTests = Join-Path $repoRoot "tools\extractor\polizas-metadata\tests\Test-PolizasMetadataExtractor.ps1"

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Command
    )

    Write-Host "==> $Name"
    $global:LASTEXITCODE = 0
    & $Command
    if ($global:LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $global:LASTEXITCODE."
    }
}

function Get-NodeVersion {
    param([string]$Executable)

    $version = if ([string]::IsNullOrWhiteSpace($Executable)) {
        node --version
    }
    else {
        & $Executable --version
    }

    return [version]($version.TrimStart("v"))
}

function Assert-NodeVersion {
    $minimumVersion = [version]"20.19.0"
    $version = Get-NodeVersion -Executable $NodeExe
    if ($version -lt $minimumVersion) {
        throw "Node.js $minimumVersion or newer is required. Current version: $version. Pass -NodeExe with a compatible node.exe or update PATH."
    }
}

function Invoke-Npm {
    param([Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments)

    if ([string]::IsNullOrWhiteSpace($NodeExe)) {
        npm @Arguments
        return
    }

    $nodePath = Resolve-Path -LiteralPath $NodeExe
    $nodeDir = Split-Path -Parent $nodePath
    $npmCommand = Get-Command npm -ErrorAction Stop
    $npmCli = Join-Path (Split-Path -Parent $npmCommand.Source) "node_modules\npm\bin\npm-cli.js"
    if (-not (Test-Path -LiteralPath $npmCli)) {
        throw "Could not locate npm-cli.js next to $($npmCommand.Source)."
    }

    $previousPath = $env:PATH
    try {
        $env:PATH = "$nodeDir;$previousPath"
        & $nodePath $npmCli @Arguments
    }
    finally {
        $env:PATH = $previousPath
    }
}

function Invoke-BackendSmoke {
    $logPath = Join-Path $reportsRoot "backend-smoke.log"
    $errorPath = Join-Path $reportsRoot "backend-smoke.err.log"
    $previousApiKey = $env:ApiSecurity__ApiKey
    $previousRepository = $env:Polizas__Repository

    $env:ApiSecurity__ApiKey = $BackendSmokeApiKey
    $env:Polizas__Repository = "InMemory"

    $process = Start-Process -FilePath dotnet `
        -ArgumentList @("run", "--no-build", "--configuration", "Release", "--project", $backendApiProject, "--urls", $BackendSmokeUrl) `
        -WorkingDirectory $repoRoot `
        -RedirectStandardOutput $logPath `
        -RedirectStandardError $errorPath `
        -WindowStyle Hidden `
        -PassThru

    try {
        $ready = $false
        for ($attempt = 0; $attempt -lt 30; $attempt++) {
            try {
                Invoke-RestMethod -Uri "$BackendSmokeUrl/health" -TimeoutSec 2 | Out-Null
                $ready = $true
                break
            }
            catch {
                Start-Sleep -Seconds 1
            }
        }

        if (-not $ready) {
            throw "Backend smoke server did not become ready. See $logPath and $errorPath."
        }

        $headers = @{ "X-ILiniumTech-Api-Key" = $BackendSmokeApiKey }
        Invoke-RestMethod -Uri "$BackendSmokeUrl/api/me" -Headers $headers -TimeoutSec 5 | Out-Null
        Invoke-RestMethod -Uri "$BackendSmokeUrl/api/polizas/catalogs" -Headers $headers -TimeoutSec 5 | Out-Null
        $polizas = Invoke-RestMethod -Uri "$BackendSmokeUrl/api/polizas?page=1&pageSize=5" -Headers $headers -TimeoutSec 5
        if ($null -eq $polizas.items) {
            throw "Backend smoke did not return a polizas items collection."
        }
    }
    finally {
        if ($process -and -not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
            $process.WaitForExit(5000) | Out-Null
        }

        $env:ApiSecurity__ApiKey = $previousApiKey
        $env:Polizas__Repository = $previousRepository
    }
}

function Invoke-FrontendSmoke {
    if ([string]::IsNullOrWhiteSpace($FrontendSmokeUrl)) {
        $indexPath = Join-Path $frontendRoot "dist\index.html"
        if (-not (Test-Path -LiteralPath $indexPath)) {
            throw "Frontend smoke requires dist/index.html. Run build or provide -FrontendSmokeUrl."
        }

        $index = Get-Content -Raw -LiteralPath $indexPath
        if ($index -notmatch 'assets/') {
            throw "Frontend smoke did not find built asset references in dist/index.html."
        }

        return
    }

    $response = Invoke-WebRequest -Uri $FrontendSmokeUrl -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -lt 200 -or $response.StatusCode -ge 400) {
        throw "Frontend smoke returned HTTP $($response.StatusCode)."
    }
}

Push-Location $repoRoot
try {
    New-Item -ItemType Directory -Force -Path $reportsRoot | Out-Null

    Invoke-Step "Backend restore" {
        dotnet restore $backendSolution
    }

    Invoke-Step "Backend build" {
        dotnet build $backendSolution --configuration Release --no-restore
    }

    Invoke-Step "Backend tests" {
        dotnet test $backendSolution --configuration Release --no-build --logger "trx;LogFileName=backend-tests.trx"
    }

    Push-Location $frontendRoot
    try {
        Invoke-Step "Frontend Node version" {
            Assert-NodeVersion
        }

        Invoke-Step "Frontend install" {
            Invoke-Npm ci
        }

        New-Item -ItemType Directory -Force -Path $frontendReports | Out-Null

        Invoke-Step "Frontend format" {
            Invoke-Npm run format
        }

        Invoke-Step "Frontend lint" {
            Invoke-Npm run lint
        }

        Invoke-Step "Frontend unit tests" {
            Invoke-Npm run test:unit:ci
        }

        if (-not $SkipFrontendBuild) {
            Invoke-Step "Frontend build" {
                Invoke-Npm run build
            }
        }
    }
    finally {
        Pop-Location
    }

    if (-not $SkipSmoke) {
        Invoke-Step "Backend HTTP smoke" {
            Invoke-BackendSmoke
        }

        Invoke-Step "Frontend smoke" {
            Invoke-FrontendSmoke
        }
    }

    if (-not $SkipSecurity) {
        Invoke-Step "Secret scan" {
            & (Join-Path $repoRoot "tools\security\Invoke-SecretScan.ps1") -ReportPath (Join-Path $reportsRoot "gitleaks-report.json")
        }

        Invoke-Step "Dependency audit" {
            & (Join-Path $repoRoot "tools\security\Invoke-DependencyAudit.ps1") -ReportDir (Join-Path $reportsRoot "security") -FailOnFindings
        }

        Invoke-Step "CORS audit" {
            & (Join-Path $repoRoot "tools\security\Invoke-CorsAudit.ps1") -FailOnFindings
        }
    }

    if (Test-Path -LiteralPath $polizasMetadataExtractorTests -PathType Leaf) {
        Invoke-Step "Polizas metadata extractor tests" {
            & $polizasMetadataExtractorTests
        }
    }

    Invoke-Step "Documentation baseline" {
        & (Join-Path $repoRoot "tools\quality\Test-DocumentationBaseline.ps1")
    }

    Invoke-Step "Git whitespace check" {
        git diff --check
    }
}
finally {
    Pop-Location
}
