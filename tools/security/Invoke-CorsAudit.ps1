param(
  [switch]$FailOnFindings
)

$ErrorActionPreference = "Stop"

$ignoredPathPattern = "(\\|/)(bin|obj|dist|build|node_modules)(\\|/)"
$programFiles = Get-ChildItem -Path "." -Recurse -Filter "Program.cs" -File |
  Where-Object { $_.FullName -notmatch $ignoredPathPattern }

$findings = New-Object System.Collections.Generic.List[string]

foreach ($file in $programFiles) {
  $relativePath = Resolve-Path -Relative $file.FullName
  $content = Get-Content -Raw -LiteralPath $file.FullName

  if ($content -match '\.AllowAnyOrigin\s*\(') {
    $findings.Add("$relativePath uses AllowAnyOrigin.")
  }

  if ($content -match '\.WithOrigins\s*\(\s*"') {
    $findings.Add("$relativePath hardcodes CORS origins in Program.cs. Use Cors:AllowedOrigins.")
  }

  if ($content -match '(?i)AllowAll|Allow_All|CorsAllowAll') {
    $findings.Add("$relativePath contains a CORS policy name that looks permissive. Review it.")
  }
}

Write-Host "# CORS Audit"
Write-Host ""

if ($findings.Count -eq 0) {
  Write-Host "No CORS findings detected."
}
else {
  foreach ($finding in $findings) {
    Write-Host "- $finding"
  }
}

if ($FailOnFindings -and $findings.Count -gt 0) {
  throw "CORS audit found $($findings.Count) finding(s)."
}
