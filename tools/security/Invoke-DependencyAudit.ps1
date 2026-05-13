param(
  [string]$ReportDir = "security-reports",
  [switch]$FailOnFindings
)

$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $ReportDir | Out-Null
$ReportDir = (Resolve-Path $ReportDir).Path

$findings = 0
$summary = New-Object System.Collections.Generic.List[string]
$summary.Add("# Dependency Audit")
$summary.Add("")
$summary.Add("Generated: $(Get-Date -Format s)")
$summary.Add("")

$ignoredPathPattern = "(\\|/)(node_modules|bin|obj|dist|build|coverage|security-reports)(\\|/)"

$npmProjects = Get-ChildItem -Path "." -Recurse -Filter "package.json" -File |
  Where-Object { $_.FullName -notmatch $ignoredPathPattern } |
  ForEach-Object { $_.Directory.FullName } |
  Sort-Object -Unique

$summary.Add("## npm")
$summary.Add("")

foreach ($project in $npmProjects) {
  $displayPath = Resolve-Path -Relative $project
  $lockPath = Join-Path $project "package-lock.json"
  $summary.Add("### $displayPath")

  if (-not (Test-Path -LiteralPath $lockPath)) {
    $summary.Add("- skipped: package-lock.json not found")
    $summary.Add("")
    continue
  }

  $safeName = ($displayPath -replace "^[.][\\/]", "") -replace '[\\/:\.]', '-'
  if ([string]::IsNullOrWhiteSpace($safeName)) { $safeName = "root" }
  $reportPath = Join-Path $ReportDir "npm-$safeName.json"

  Push-Location $project
  try {
    npm audit --json | Out-File -FilePath $reportPath -Encoding utf8
  }
  finally {
    Pop-Location
  }

  try {
    $report = Get-Content -Raw $reportPath | ConvertFrom-Json
    $metadata = $report.metadata.vulnerabilities
    $total = [int]($metadata.info + $metadata.low + $metadata.moderate + $metadata.high + $metadata.critical)
    $findings += $total

    $summary.Add("- total: $total")
    $summary.Add("- critical: $($metadata.critical)")
    $summary.Add("- high: $($metadata.high)")
    $summary.Add("- moderate: $($metadata.moderate)")
    $summary.Add("- low: $($metadata.low)")
  }
  catch {
    $findings++
    $summary.Add("- audit output could not be parsed")
  }
  $summary.Add("")
}

$summary.Add("## .NET")
$summary.Add("")

$dotnetProjects = Get-ChildItem -Path "." -Recurse -Filter "*.csproj" -File |
  Where-Object { $_.FullName -notmatch $ignoredPathPattern } |
  Sort-Object FullName

foreach ($project in $dotnetProjects) {
  $displayPath = Resolve-Path -Relative $project.FullName
  $safeName = ($displayPath -replace "^[.][\\/]", "") -replace '[\\/:\.]', '-'
  $reportPath = Join-Path $ReportDir "dotnet-$safeName.txt"
  $summary.Add("### $displayPath")

  dotnet list $project.FullName package --vulnerable --include-transitive | Tee-Object -FilePath $reportPath
  $output = Get-Content -Raw $reportPath
  $hasFindings = $output -match 'has the following vulnerable packages|tiene los paquetes vulnerables siguientes'

  if ($hasFindings) {
    $findings++
    $summary.Add("- vulnerable packages detected")
  }
  else {
    $summary.Add("- no vulnerable packages detected")
  }
  $summary.Add("")
}

if ($npmProjects.Count -eq 0 -and $dotnetProjects.Count -eq 0) {
  $summary.Add("No npm or .NET projects found.")
  $summary.Add("")
}

$summary.Add("## Result")
$summary.Add("")
$summary.Add("Total finding count: $findings")

$summaryPath = Join-Path $ReportDir "dependency-audit-summary.md"
$summary | Set-Content -Path $summaryPath -Encoding utf8

Get-Content $summaryPath

if ($FailOnFindings -and $findings -gt 0) {
  throw "Dependency audit found $findings vulnerable project/package result(s)."
}
