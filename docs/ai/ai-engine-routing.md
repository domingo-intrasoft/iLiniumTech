# AI engine routing

## Default routing

Use Codex for repository work:

- C# and .NET API changes;
- Vue, TypeScript, Vite, and tests;
- GitHub Actions and repository automation;
- security script updates;
- SDD and engineering documentation.

Use Claude only when a human intentionally selects it for broader product writing, alternative analysis, or comparison work.

## Local vs web/cloud

Prefer local agent work for:

- code that needs repository context;
- test-driven fixes;
- security-sensitive reasoning;
- changes that require local commands.

Use web/cloud execution only for low-risk bounded tasks when secrets and local-only context are not required.

## Security-sensitive blockers

Require `human-decision-required` and `status:blocked` before automated execution when the issue touches:

- real database credentials or connection setup;
- authentication or authorization;
- dynamic SQL;
- personal, financial, policy, or risk data;
- GitHub secrets, environments, permissions, or branch protection;
- deployment credentials or production infrastructure;
- AppBuilder workflows or expression execution.

## Branch selection

Route product tasks to `develop` unless the issue says otherwise.

Route infrastructure tasks to `ci-cd`, including:

- `.github/**`;
- `tools/security/**`;
- `docs/ai/**`;
- `docs/engineering/**`;
- CI/CD workflows;
- AI dispatcher and worker workflows.

Route release stabilization to `main` only through reviewed PRs.

## Size limits

AI tasks should normally change one slice:

- one backend feature;
- one frontend feature;
- one extractor step;
- one workflow;
- one doc/spec.

Split work when a task mixes package upgrades, architecture changes, security behavior, functional behavior, and visual UI changes.
