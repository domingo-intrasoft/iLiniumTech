param(
    [string]$Repository = $env:GITHUB_REPOSITORY,
    [Parameter(Mandatory = $true)]
    [ValidateRange(1, [int]::MaxValue)]
    [int]$IssueNumber,
    [string]$Token = $env:GITHUB_TOKEN,
    [bool]$DryRun = $true,
    [string]$JsonOutputPath = "",
    [string]$MarkdownOutputPath = "",
    [switch]$UseMock
)

$ErrorActionPreference = "Stop"

$readyStatus = "status:ready-for-ai"
$inProgressStatus = "status:ai-in-progress"
$blockedLabels = @("status:blocked", "human-decision-required")
$engineLabels = @("ai-codex", "ai-claude", "ai-codex-web", "ai-claude-web")
$marker = "<!-- ai-manual-worker-handoff:$IssueNumber -->"

function Invoke-GitHubApi {
    param(
        [ValidateSet("Get", "Post", "Patch", "Put")]
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
        "User-Agent" = "iLiniumTech-ai-manual-worker"
    }

    $arguments = @{
        Method = $Method
        Uri = $Uri
        Headers = $headers
    }

    if ($null -ne $Body) {
        $arguments.Body = ($Body | ConvertTo-Json -Depth 10)
        $arguments.ContentType = "application/json"
    }

    Invoke-RestMethod @arguments
}

function ConvertTo-GitHubItems {
    param([object]$Value)

    if ($null -eq $Value) {
        return
    }

    if ($Value -is [System.Array]) {
        foreach ($item in $Value) {
            $item
        }

        return
    }

    $Value
}

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

function New-MockIssue {
    [pscustomobject]@{
        number = $IssueNumber
        title = "AI: definir contrato operativo Issue a agente a PR"
        state = "open"
        body = "## Rama base`n`ci-cd`n`n## Paths permitidos`ndocs/ai/**."
        html_url = "https://github.com/example/repo/issues/$IssueNumber"
        labels = @(
            @{ name = "status:ready-for-ai" },
            @{ name = "ai-codex" },
            @{ name = "app:docs" },
            @{ name = "ai-task" }
        )
    }
}

function Get-Issue {
    if ($UseMock) {
        return New-MockIssue
    }

    if ([string]::IsNullOrWhiteSpace($Repository)) {
        throw "Repository is required. Provide -Repository owner/name or set GITHUB_REPOSITORY."
    }

    Invoke-GitHubApi -Method Get -Uri "https://api.github.com/repos/$Repository/issues/$IssueNumber"
}

function Get-ActiveLockLabels {
    param([int]$CurrentIssueNumber)

    if ($UseMock) {
        return @()
    }

    $encodedLabel = [System.Uri]::EscapeDataString($inProgressStatus)
    $uri = "https://api.github.com/repos/$Repository/issues?state=open&labels=$encodedLabel&sort=updated&direction=desc&per_page=100"
    $activeIssues = @(ConvertTo-GitHubItems -Value (Invoke-GitHubApi -Method Get -Uri $uri))

    @($activeIssues |
        Where-Object { -not $_.pull_request -and [int]$_.number -ne $CurrentIssueNumber } |
        ForEach-Object { Get-LabelNames -Issue $_ } |
        Where-Object { $_ -like "lock:*" } |
        Sort-Object -Unique)
}

function Get-BaseBranch {
    param(
        [string[]]$Labels,
        [string]$Body
    )

    $explicitBranch = Get-ExplicitBaseBranch -Body $Body
    if ($explicitBranch) {
        return $explicitBranch
    }

    if ($Labels -contains "app:infrastructure") {
        return "ci-cd"
    }

    "develop"
}

function Get-ExplicitBaseBranch {
    param([string]$Body)

    if ([string]::IsNullOrWhiteSpace($Body)) {
        return $null
    }

    $patterns = @(
        "(?im)^\s*##\s*Rama base\s*\r?\n\s*`?(main|develop|ci-cd)`?",
        "(?im)^\s*##\s*Base branch\s*\r?\n\s*`?(main|develop|ci-cd)`?",
        "(?im)^\s*[-*]?\s*(Rama base|Base branch)\s*:\s*`?(main|develop|ci-cd)`?"
    )

    foreach ($pattern in $patterns) {
        $match = [regex]::Match($Body, $pattern)
        if ($match.Success) {
            return $match.Groups[$match.Groups.Count - 1].Value
        }
    }

    $null
}

function ConvertTo-BranchSlug {
    param([string]$Title)

    $slug = $Title.ToLowerInvariant() -replace "[^a-z0-9]+", "-"
    $slug = $slug.Trim("-")

    if ([string]::IsNullOrWhiteSpace($slug)) {
        return "task"
    }

    if ($slug.Length -gt 48) {
        $slug = $slug.Substring(0, 48).Trim("-")
    }

    $slug
}

function New-WorkerBranchName {
    param(
        [object]$Issue,
        [string]$Engine,
        [string]$App
    )

    $slug = ConvertTo-BranchSlug -Title ([string]$Issue.title)

    if ($App -eq "app:infrastructure") {
        return "codex/gh-$($Issue.number)-$slug"
    }

    if ($Engine -eq "ai-claude") {
        return "claude/gh-$($Issue.number)-$slug"
    }

    "codex/gh-$($Issue.number)-$slug"
}

function Get-BranchHeadSha {
    param([string]$Branch)

    if ($UseMock) {
        return "0000000000000000000000000000000000000000"
    }

    $ref = Invoke-GitHubApi -Method Get -Uri "https://api.github.com/repos/$Repository/git/ref/heads/$Branch"
    [string]$ref.object.sha
}

function Test-BranchExists {
    param([string]$Branch)

    if ($UseMock) {
        return $false
    }

    try {
        Invoke-GitHubApi -Method Get -Uri "https://api.github.com/repos/$Repository/git/ref/heads/$Branch" | Out-Null
        return $true
    }
    catch {
        if ($_.Exception.Response -and [int]$_.Exception.Response.StatusCode -eq 404) {
            return $false
        }

        throw
    }
}

function New-GitHubBranch {
    param(
        [string]$BaseBranch,
        [string]$WorkerBranch
    )

    if ($DryRun -or $UseMock) {
        return "skipped"
    }

    if (Test-BranchExists -Branch $WorkerBranch) {
        return "exists"
    }

    $sha = Get-BranchHeadSha -Branch $BaseBranch
    Invoke-GitHubApi -Method Post -Uri "https://api.github.com/repos/$Repository/git/refs" -Body @{
        ref = "refs/heads/$WorkerBranch"
        sha = $sha
    } | Out-Null

    "created"
}

function Set-IssueInProgress {
    param(
        [object]$Issue,
        [string[]]$Labels
    )

    if ($DryRun -or $UseMock) {
        return "skipped"
    }

    $nextLabels = @($Labels | Where-Object { $_ -notlike "status:*" })
    $nextLabels += $inProgressStatus
    $nextLabels = @($nextLabels | Sort-Object -Unique)

    Invoke-GitHubApi -Method Patch -Uri "https://api.github.com/repos/$Repository/issues/$($Issue.number)" -Body @{
        labels = $nextLabels
    } | Out-Null

    "updated"
}

function ConvertTo-HandoffMarkdown {
    param([object]$Summary)

    $lines = @()
    $lines += $marker
    $lines += "# AI manual worker handoff"
    $lines += ""
    $lines += "- Issue: #$($Summary.issueNumber)"
    $lines += "- Dry run: $($Summary.dryRun)"
    $lines += "- Ready: $($Summary.ready)"
    $lines += "- Engine: $($Summary.engine)"
    $lines += "- App: $($Summary.app)"
    $lines += "- Base branch: $($Summary.baseBranch)"
    $lines += "- Worker branch: $($Summary.workerBranch)"
    $lines += "- Branch action: $($Summary.branchAction)"
    $lines += "- Label action: $($Summary.labelAction)"
    $lines += "- Active lock conflicts: $(@($Summary.conflictingLocks) -join ', ')"
    $lines += ""
    $lines += "## Problems"

    if (@($Summary.problems).Count -eq 0) {
        $lines += "- None"
    }
    else {
        foreach ($problem in @($Summary.problems)) {
            $lines += "- $problem"
        }
    }

    $lines += ""
    $lines += "## Next step"
    $lines += "Use the worker branch for the bounded implementation, open a PR against the base branch, and include tests/security evidence. This manual worker does not execute Codex automatically."
    $lines -join [Environment]::NewLine
}

function Set-HandoffComment {
    param([string]$Markdown)

    if ($DryRun -or $UseMock) {
        return "skipped"
    }

    $commentUri = "https://api.github.com/repos/$Repository/issues/$IssueNumber/comments"
    $comments = @(ConvertTo-GitHubItems -Value (Invoke-GitHubApi -Method Get -Uri $commentUri))
    $existing = $comments |
        Where-Object { [string]$_.body -like "$marker*" } |
        Sort-Object -Property created_at -Descending |
        Select-Object -First 1

    if ($existing) {
        Invoke-GitHubApi -Method Patch -Uri "https://api.github.com/repos/$Repository/issues/comments/$($existing.id)" -Body @{ body = $Markdown } | Out-Null
        return "updated"
    }

    Invoke-GitHubApi -Method Post -Uri $commentUri -Body @{ body = $Markdown } | Out-Null
    "created"
}

$issue = Get-Issue
$labels = @(Get-LabelNames -Issue $issue)
$statusLabels = @($labels | Where-Object { $_ -like "status:*" })
$matchedBlockedLabels = @($labels | Where-Object { $blockedLabels -contains $_ })
$matchedEngineLabels = @($labels | Where-Object { $engineLabels -contains $_ })
$appLabels = @($labels | Where-Object { $_ -like "app:*" })
$lockLabels = @($labels | Where-Object { $_ -like "lock:*" })
$activeLocks = @(Get-ActiveLockLabels -CurrentIssueNumber $IssueNumber)
$conflictingLocks = @($lockLabels | Where-Object { $activeLocks -contains $_ })
$problems = @()

if ($issue.pull_request) {
    $problems += "Target is a pull request, not an issue."
}

if ($issue.state -ne "open") {
    $problems += "Issue is not open."
}

if ($statusLabels.Count -ne 1) {
    $problems += "Expected exactly one status:* label; found $($statusLabels.Count)."
}
elseif ($statusLabels[0] -notin @($readyStatus, $inProgressStatus)) {
    $problems += "Status '$($statusLabels[0])' is not eligible for manual worker start."
}

foreach ($label in $matchedBlockedLabels) {
    $problems += "Blocked by label '$label'."
}

if ($matchedEngineLabels.Count -ne 1) {
    $problems += "Expected exactly one engine label; found $($matchedEngineLabels.Count)."
}
elseif ($matchedEngineLabels[0] -notin @("ai-codex", "ai-codex-web")) {
    $problems += "Manual Codex worker cannot handle engine '$($matchedEngineLabels[0])'."
}

if ($appLabels.Count -ne 1) {
    $problems += "Expected exactly one app:* label; found $($appLabels.Count)."
}

foreach ($lock in $conflictingLocks) {
    $problems += "Lock '$lock' is already active on another AI issue."
}

$engine = if ($matchedEngineLabels.Count -eq 1) { $matchedEngineLabels[0] } else { $null }
$app = if ($appLabels.Count -eq 1) { $appLabels[0] } else { $null }
$baseBranch = if ($app) { Get-BaseBranch -Labels $labels -Body ([string]$issue.body) } else { $null }
$workerBranch = if ($engine -and $app) { New-WorkerBranchName -Issue $issue -Engine $engine -App $app } else { $null }
$ready = ($problems.Count -eq 0)
$branchAction = "skipped"
$labelAction = "skipped"
$commentAction = "skipped"

if ($ready) {
    $branchAction = New-GitHubBranch -BaseBranch $baseBranch -WorkerBranch $workerBranch
    $labelAction = Set-IssueInProgress -Issue $issue -Labels $labels
}

$summary = [pscustomobject]@{
    repository = if ($UseMock -and [string]::IsNullOrWhiteSpace($Repository)) { "mock/repository" } else { $Repository }
    issueNumber = $IssueNumber
    issueTitle = [string]$issue.title
    issueUrl = [string]$issue.html_url
    dryRun = [bool]$DryRun
    ready = $ready
    status = if ($statusLabels.Count -eq 1) { $statusLabels[0] } else { $null }
    engine = $engine
    app = $app
    locks = $lockLabels
    activeLocks = $activeLocks
    conflictingLocks = $conflictingLocks
    baseBranch = $baseBranch
    workerBranch = $workerBranch
    branchAction = $branchAction
    labelAction = $labelAction
    commentAction = $commentAction
    problems = $problems
}

$markdown = ConvertTo-HandoffMarkdown -Summary $summary

if ($ready) {
    $commentAction = Set-HandoffComment -Markdown $markdown
    $summary.commentAction = $commentAction
    $markdown = ConvertTo-HandoffMarkdown -Summary $summary
}

$json = $summary | ConvertTo-Json -Depth 10

Write-Host $markdown
Write-Host ""
Write-Host $json

if (-not [string]::IsNullOrWhiteSpace($JsonOutputPath)) {
    $json | Set-Content -LiteralPath $JsonOutputPath -Encoding utf8
}

if (-not [string]::IsNullOrWhiteSpace($MarkdownOutputPath)) {
    $markdown | Set-Content -LiteralPath $MarkdownOutputPath -Encoding utf8
}

if (-not $ready) {
    throw "Issue #$IssueNumber is not eligible for manual worker start."
}
