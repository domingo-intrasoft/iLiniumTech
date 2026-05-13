# App policy: frontend

## Scope

Frontend code under `iLiniumTech.Frontend`.

## Commands

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run lint
npm run typecheck
npm run test:unit
npm run build
```

Node.js must be `20.19.0` or newer.

## Required checks

- Unit tests for API clients, composables, metadata transforms, and visible whitelist behavior.
- Build and typecheck for UI changes.
- Playwright smoke before marking visual MVP work ready.

## Risks

- Never put secrets in `VITE_*` variables.
- Do not persist tokens in local storage for real authentication.
- Do not render arbitrary AppBuilder HTML, scripts, or events without sanitization and allowlists.
