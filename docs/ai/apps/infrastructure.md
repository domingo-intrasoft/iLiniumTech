# App policy: infrastructure

## Scope

- `.github/**`
- `tools/security/**`
- `docs/ai/**`
- `docs/engineering/**`
- repository governance and CI/CD automation

## Required checks

- Validate YAML syntax where tooling is available.
- Keep workflow job names stable.
- Do not make security checks weaker without approval.
- Do not add secrets to workflow logs.
- Do not enable deployments without protected environments.

## Branch

Infrastructure changes should branch from `ci-cd`.

## Risks

- Broken required checks can block all development.
- Over-broad tokens or permissions can expose the repository.
- Automated repair workflows can create loops if not constrained.
