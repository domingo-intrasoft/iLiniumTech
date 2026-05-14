param(
    [string]$Repository = $env:GITHUB_REPOSITORY,
    [int]$IssueNumber = 0,
    [ValidateRange(1, 50)]
    [int]$Top = 10,
    [string]$Token = $env:GITHUB_TOKEN,
    [switch]$Comment,
    [bool]$DiagnosticsOnly = $true,
    [string]$JsonOutputPath = "",
    [string]$MarkdownOutputPath = "",
    [switch]$UseMock
)

$ErrorActionPreference = "Stop"

$requiredReadyLabel = "status:ready-for-ai"
$blockedLabels = @("status:blocked", "status:ai-in-progress", "human-decision-required")
$engineLabels = @("ai-codex", "ai-claude", "ai-codex-web", "ai-claude-web")

function Get-LabelNames {
    param([object]$Issue)

    @($Issue.labels | ForEach-Object {
        if ($_ -is [string]) {
            $_
        }
        else {
            [string]$_.name
        }
    }) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
}

function New-MockIssues {
    @(
        [pscustomobject]@{
            number = 101
            title = "Valid AI task"
            state = "open"
            html_url = "https://github.com/example/repo/issues/101"
            labels = @(
                @{ name = "status:ready-for-ai" },
                @{ name = "ai-codex" },
                @{ name = "app:api" },
                @{ name = "lock:pipelines" }
            )
        },
        [pscustomobject]@{
            number = 102
            title = "Blocked AI task"
            state = "open"
            html_url = "https://github.com/example/repo/issues/102"
            labels = @(
                @{ name = "status:ready-for-ai" },
                @{ name = "status:blocked" },
                @{ name = "ai-claude" },
                @{ name = "app:docs" }
            )
        }
    )
}

function Invoke-GitHubApi {
    param(
        [ValidateSet("Get", "Post", "Patch")]
        [string]$Method,
        [string]$Uri,
        [object]$Body = $null
    )

    if ([string]::IsNullOrWhiteSpace($Token)) {
        throw "GITHUB_TOKEN is required unless -UseMock is specified."
    }

    $headers = @{
        Authorization = "Bearer $Token"
        Accept = "application/vnd.github+json"
        "X-GitHub-Api-Version" = "2022-11-28"
        "User-Agent" = "iLiniumTech-ai-issue-dispatcher"
    }

    $args = @{
        Method = $Method
        Uri = $Uri
        Headers = $headers
    }

    if ($null -ne $Body) {
        $args.Body = ($Body | ConvertTo-Json -Depth 10)
        $args.ContentType = "application/json"
    }

    Invoke-RestMethod @args
}

function Get-CandidateIssues {
    if ($UseMock) {
        return New-MockIssues
    }

    if ([string]::IsNullOrWhiteSpace($Repository)) {
        throw "Repository is required. Provide -Repository owner/name or set GITHUB_REPOSITORY."
    }

    if ($IssueNumber -gt 0) {
        $uri = "https://api.github.com/repos/$Repository/issues/$IssueNumber"
        return @(Invoke-GitHubApi -Method Get -Uri $uri)
    }

    $encodedLabel = [System.Uri]::EscapeDataString($requiredReadyLabel)
    $uri = "https://api.github.com/repos/$Repository/issues?state=open&labels=$encodedLabel&sort=updated&direction=desc&per_page=100"
    $issues = @(Invoke-GitHubApi -Method Get -Uri $uri)
    $issues |
        Where-Object { -not $_.pull_request } |
        Select-Object -First $Top
}

function Test-Issue {
    param(
        [object]$Issue,
        [string[]]$ActiveLocks
    )

    $labels = @(Get-LabelNames -Issue $Issue)
    $matchedBlockedLabels = @($labels | Where-Object { $blockedLabels -contains $_ })
    $matchedEngineLabels = @($labels | Where-Object { $engineLabels -contains $_ })
    $statusLabels = @($labels | Where-Object { $_ -like "status:*" })
    $appLabels = @($labels | Where-Object { $_ -like "app:*" })
    $lockLabels = @($labels | Where-Object { $_ -like "lock:*" })
    $conflictingLocks = @($lockLabels | Where-Object { $ActiveLocks -contains $_ })
    $problems = @()

    if ($Issue.pull_request) {
        $problems += "Candidate is a pull request, not an issue."
    }

    if ($Issue.state -ne "open") {
        $problems += "Issue is not open."
    }

    if ($statusLabels.Count -ne 1) {
        $problems += "Expected exactly one status:* label; found $($statusLabels.Count)."
    }
    elseif ($statusLabels[0] -ne $requiredReadyLabel) {
        $problems += "Status '$($statusLabels[0])' is not eligible; expected '$requiredReadyLabel'."
    }

    foreach ($label in $matchedBlockedLabels) {
        $problems += "Blocked by label '$label'."
    }

    if ($matchedEngineLabels.Count -ne 1) {
        $problems += "Expected exactly one engine label; found $($matchedEngineLabels.Count)."
    }

    if ($appLabels.Count -ne 1) {
        $problems += "Expected exactly one app:* label; found $($appLabels.Count)."
    }

    foreach ($lock in $conflictingLocks) {
        $problems += "Lock '$lock' is already active on another AI issue."
    }

    [pscustomobject]@{
        number = [int]$Issue.number
        title = [string]$Issue.title
        url = [string]$Issue.html_url
        state = [string]$Issue.state
        ready = ($problems.Count -eq 0)
        engine = if ($matchedEngineLabels.Count -eq 1) { $matchedEngineLabels[0] } else { $null }
        status = if ($statusLabels.Count -eq 1) { $statusLabels[0] } else { $null }
        apps = $appLabels
        locks = $lockLabels
        blockedLabels = $matchedBlockedLabels
        problems = $problems
    }
}

function Get-ActiveLocks {
    if ($UseMock) {
        return @("lock:database")
    }

    $encodedLabel = [System.Uri]::EscapeDataString("status:ai-in-progress")
    $uri = "https://api.github.com/repos/$Repository/issues?state=open&labels=$encodedLabel&sort=updated&direction=desc&per_page=100"
    $activeIssues = @(Invoke-GitHubApi -Method Get -Uri $uri)

    @($activeIssues |
        Where-Object { -not $_.pull_request } |
        ForEach-Object { Get-LabelNames -Issue $_ } |
        Where-Object { $_ -like "lock:*" } |
        Sort-Object -Unique)
}

function ConvertTo-MarkdownSummary {
    param([object]$Summary)

    $lines = @()
    $lines += "<!-- ai-issue-dispatcher-diagnostics -->"
    $lines += "# AI issue dispatcher diagnostics"
    $lines += ""
    $lines += "- Repository: $($Summary.repository)"
    $lines += "- Diagnostics only: $($Summary.diagnosticsOnly)"
    $lines += "- Candidates checked: $($Summary.candidatesChecked)"
    $lines += "- Ready candidates: $($Summary.readyCount)"
    $lines += "- Active locks: $(@($Summary.activeLocks) -join ', ')"
    $lines += ""
    $lines += "| Issue | Ready | Engine | Apps | Locks | Problems |"
    $lines += "| --- | --- | --- | --- | --- | --- |"

    foreach ($result in @($Summary.results)) {
        $issue = "[#$($result.number)]($($result.url))"
        $apps = if (@($result.apps).Count -gt 0) { @($result.apps) -join ", " } else { "-" }
        $locks = if (@($result.locks).Count -gt 0) { @($result.locks) -join ", " } else { "-" }
        $engine = if ($result.engine) { $result.engine } else { "-" }
        $problems = if (@($result.problems).Count -gt 0) { @($result.problems) -join "<br>" } else { "-" }
        $lines += "| $issue | $($result.ready) | $engine | $apps | $locks | $problems |"
    }

    $lines -join [Environment]::NewLine
}

if (-not $DiagnosticsOnly) {
    throw "Only diagnostics mode is supported. This dispatcher must not launch agents or mutate code."
}

$issues = @(Get-CandidateIssues)
$activeLocks = @(Get-ActiveLocks)
$results = @($issues | ForEach-Object { Test-Issue -Issue $_ -ActiveLocks $activeLocks })

$summary = [pscustomobject]@{
    repository = if ($UseMock -and [string]::IsNullOrWhiteSpace($Repository)) { "mock/repository" } else { $Repository }
    diagnosticsOnly = [bool]$DiagnosticsOnly
    generatedAtUtc = (Get-Date).ToUniversalTime().ToString("o")
    issueNumber = if ($IssueNumber -gt 0) { $IssueNumber } else { $null }
    top = $Top
    candidatesChecked = $results.Count
    readyCount = @($results | Where-Object { $_.ready }).Count
    activeLocks = $activeLocks
    results = $results
}

$json = $summary | ConvertTo-Json -Depth 10
$markdown = ConvertTo-MarkdownSummary -Summary $summary

Write-Host $markdown
Write-Host ""
Write-Host $json

if (-not [string]::IsNullOrWhiteSpace($JsonOutputPath)) {
    $json | Set-Content -LiteralPath $JsonOutputPath -Encoding utf8
}

if (-not [string]::IsNullOrWhiteSpace($MarkdownOutputPath)) {
    $markdown | Set-Content -LiteralPath $MarkdownOutputPath -Encoding utf8
}

if ($Comment) {
    if ($UseMock) {
        Write-Host "Mock mode: comment was requested but no GitHub comment was posted."
    }
    elseif ($IssueNumber -le 0) {
        throw "-Comment requires -IssueNumber so diagnostics are posted to one explicit issue."
    }
    else {
        $commentUri = "https://api.github.com/repos/$Repository/issues/$IssueNumber/comments"
        $existingComments = @(Invoke-GitHubApi -Method Get -Uri $commentUri)
        $existingDiagnostic = $existingComments |
            Where-Object { [string]$_.body -like "<!-- ai-issue-dispatcher-diagnostics -->*" } |
            Sort-Object -Property created_at -Descending |
            Select-Object -First 1

        if ($existingDiagnostic) {
            $updateUri = "https://api.github.com/repos/$Repository/issues/comments/$($existingDiagnostic.id)"
            Invoke-GitHubApi -Method Patch -Uri $updateUri -Body @{ body = $markdown } | Out-Null
            Write-Host "Updated diagnostics comment on issue #$IssueNumber."
        }
        else {
            Invoke-GitHubApi -Method Post -Uri $commentUri -Body @{ body = $markdown } | Out-Null
            Write-Host "Posted diagnostics comment to issue #$IssueNumber."
        }
    }
}
