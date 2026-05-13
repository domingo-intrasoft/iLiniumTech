# App policy: api

## Scope

Backend code under `iLiniumTech.Backend`.

## Commands

```powershell
dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

## Required checks

- Unit tests for validators, query objects, and mappers.
- Integration tests for API routing, authentication, CORS, and error sanitization.
- Security audit when auth, configuration, SQL, or data access changes.

## Risks

- API key and future auth handling.
- Dynamic SQL for AppBuilder-derived datasources.
- Personal and financial data in policy records.
- `SESSION_CONTEXT` and permissions by profile, office, or manager.
