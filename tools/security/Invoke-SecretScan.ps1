param(
  [string]$Source = ".",
  [string]$ReportPath = "gitleaks-report.json",
  [string]$GitleaksPath = "",
  [switch]$IncludeGitHistory,
  [switch]$NoReport
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($GitleaksPath)) {
  $gitleaks = Get-Command gitleaks -ErrorAction SilentlyContinue
  if (-not $gitleaks) {
    $wingetGitleaks = Get-ChildItem -Path "$env:LOCALAPPDATA\Microsoft\WinGet\Packages" -Recurse -Filter gitleaks.exe -ErrorAction SilentlyContinue |
      Select-Object -First 1

    if ($wingetGitleaks) {
      $GitleaksPath = $wingetGitleaks.FullName
    }
    else {
      Write-Error "Gitleaks is not installed or not available in PATH. Install it before running this scan."
    }
  }
  else {
    $GitleaksPath = $gitleaks.Source
  }
}

$arguments = @(
  "detect",
  "--source", $Source,
  "--config", ".gitleaks.toml",
  "--redact",
  "--verbose"
)

if (-not $IncludeGitHistory) {
  $arguments += "--no-git"
}

if (-not $NoReport) {
  $reportDir = Split-Path -Parent $ReportPath
  if (-not [string]::IsNullOrWhiteSpace($reportDir)) {
    New-Item -ItemType Directory -Force -Path $reportDir | Out-Null
  }

  $arguments += @(
    "--report-format", "json",
    "--report-path", $ReportPath
  )
}

& $GitleaksPath @arguments
