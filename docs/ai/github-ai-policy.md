# GitHub AI policy

## Purpose

iLiniumTech uses GitHub Issues as the queue for SDD-driven work. Each functional change should be represented by a small, reviewable issue with a clear data contract, acceptance criteria, test plan, and security notes.

The policy is adapted to this repository structure:

- `main`: stable branch and default branch.
- `develop`: product code branch.
- `ci-cd`: CI/CD, automation, repository governance, and security gates.

## Eligibility

An issue can be picked up by an AI workflow only when all of these are true:

- It is open.
- It has `ai-ready` or a GitHub Project status equivalent to `Ready for AI`.
- It has exactly one engine label: `ai-codex`, `ai-claude`, `ai-codex-web`, or `ai-claude-web`.
- It has exactly one app label such as `app:api`, `app:frontend`, `app:extractor`, `app:infrastructure`, or `app:docs`.
- It has a bounded scope and a validation plan.
- It does not have `needs-human`, `ai-running`, `ai-queued`, `validated`, `preview-ready`, or `done`.

An issue must be blocked with `needs-human` when it is missing security context, requires production secrets, asks for broad refactors, or changes platform rules without approval.

## Branch routing

Product changes branch from `develop`:

- `feature/gh-<issue>-<slug>`
- `fix/gh-<issue>-<slug>`
- `codex/gh-<issue>-<slug>`
- `claude/gh-<issue>-<slug>`

Platform changes branch from `ci-cd`:

- `infra/gh-<issue>-<slug>`
- `automation/gh-<issue>-<slug>`

Stable releases merge to `main` only through reviewed pull requests.

## Forbidden paths

These paths require explicit human approval before automated changes:

- `.github/workflows/security.yml`
- `.github/workflows/*`
- `.github/CODEOWNERS`
- `.gitleaks.toml`
- `tools/security/**`
- `docs/engineering/**`
- files containing environment, deployment, or authentication configuration

The default rule is simple: an agent may not weaken validation, remove security checks, or expand access without a written approval in the issue.

## Override format

Use this exact block in a GitHub issue comment when a platform or security-sensitive exception is intentional:

```text
[AI PLATFORM APPROVAL]
Allow: <specific path, module, or rule>
ValidFor: Issue #<number>
Reason: <why this is needed>
Risk: <expected impact>
Validation: <mandatory checks>
ApprovedBy: @<github-login>
```

Use this block for smaller technical exceptions:

```text
[TECHNICAL OVERRIDE]
Scope: <specific path, module, engine, or rule>
Allow: <specific permission>
ValidFor: Issue #<number>
Reason: <why this is needed>
Validation: <mandatory checks>
ApprovedBy: @<github-login>
```

## Security rules

Never put these in issues, pull requests, logs, fixtures, or committed files:

- connection strings;
- passwords;
- API keys;
- tokens;
- private certificates;
- dumps, backups, logs, or screenshots with real personal data;
- SQL containing credentials or production identifiers.

Dynamic SQL must use whitelist validation for structural names and parameters for values. AppBuilder `QueryStatic` fields are metadata, not executable code, until validated by an iLiniumTech query object.

## Required evidence

Each PR should state:

- linked issue and SDD;
- commands executed;
- test results;
- security checks executed;
- checks not executed and why;
- residual risks.

## Labels

Recommended labels:

- engines: `ai-codex`, `ai-claude`, `ai-codex-web`, `ai-claude-web`;
- state: `ai-candidate`, `ai-ready`, `ai-queued`, `ai-running`, `needs-human`, `validated`, `preview-ready`, `done`;
- risk: `ai-low-risk`, `ai-needs-review`, `ai-platform-change`, `ai-security-sensitive`;
- apps: `app:api`, `app:frontend`, `app:extractor`, `app:renderer`, `app:infrastructure`, `app:docs`, `app:core`;
- CI: `ci-failure`, `dependency`, `security`, `e2e`, `blocked-by-env`.
