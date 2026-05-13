# App policy: docs

## Scope

- `README.md`
- `docs/**`
- SDD templates and specs

## Required checks

- Keep AppBuilder references traceable.
- Keep secrets and real personal data out of examples.
- Update SDD status when implementation scope changes.

## Risks

- Documentation can accidentally become a secret leak.
- Broad plans can encourage work that is too large for one PR.
- Specs without acceptance criteria are not ready for AI execution.
