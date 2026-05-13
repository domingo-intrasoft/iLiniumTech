## Summary

-

## SDD / Issue

- Closes #
- Spec:

## Validation

- [ ] Backend build/test: `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- [ ] Frontend install/lint/test/build: `npm ci`, `npm run lint`, `npm run test:unit`, `npm run build`
- [ ] Secret scan: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1`
- [ ] Dependency audit: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-DependencyAudit.ps1 -FailOnFindings`
- [ ] CORS audit: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-CorsAudit.ps1 -FailOnFindings`

## Security

- [ ] No secrets, connection strings, dumps, logs, tokens, or real personal data were committed.
- [ ] Authentication and authorization behavior is unchanged or documented.
- [ ] Dynamic SQL, if touched, uses whitelist validation and parameters.
- [ ] Logs and errors do not expose infrastructure details.

## Notes

-
