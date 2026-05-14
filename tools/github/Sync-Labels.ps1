param(
    [string]$Repository = "intrasoft-ilinium/iLiniumTech",
    [string]$LabelsPath = ".github/labels.json",
    [string]$Token = $env:GITHUB_TOKEN
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($Token)) {
    throw "GITHUB_TOKEN is required. Provide -Token or set the GITHUB_TOKEN environment variable."
}

if (-not (Test-Path -LiteralPath $LabelsPath)) {
    throw "Labels file not found: $LabelsPath"
}

$labels = Get-Content -LiteralPath $LabelsPath -Raw | ConvertFrom-Json
$headers = @{
    Authorization = "Bearer $Token"
    Accept = "application/vnd.github+json"
    "X-GitHub-Api-Version" = "2022-11-28"
    "User-Agent" = "iLiniumTech-label-sync"
}

foreach ($label in $labels) {
    $name = [string]$label.name
    $encodedName = [System.Uri]::EscapeDataString($name)
    $uri = "https://api.github.com/repos/$Repository/labels/$encodedName"
    $payload = @{
        name = $name
        color = [string]$label.color
        description = [string]$label.description
    } | ConvertTo-Json

    try {
        Invoke-RestMethod -Method Patch -Uri $uri -Headers $headers -Body $payload -ContentType "application/json" | Out-Null
        Write-Host "Updated label: $name"
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -ne 404) {
            throw
        }

        $createUri = "https://api.github.com/repos/$Repository/labels"
        Invoke-RestMethod -Method Post -Uri $createUri -Headers $headers -Body $payload -ContentType "application/json" | Out-Null
        Write-Host "Created label: $name"
    }
}
