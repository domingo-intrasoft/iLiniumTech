## Resumen

- 

## Spec / Issue

- Spec SDD:
- Issue:

## Pruebas ejecutadas

- [ ] `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- [ ] `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- [ ] `npm run lint`
- [ ] `npm run test:unit`
- [ ] `npm run build`
- [ ] `npm run format` si el cambio toca formato o se esta estabilizando Prettier
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-SecretScan.ps1`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-DependencyAudit.ps1 -FailOnFindings`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools/security/Invoke-CorsAudit.ps1 -FailOnFindings`

## Seguridad

- [ ] No se han introducido secretos ni cadenas de conexion.
- [ ] No se han incluido datos personales reales en fixtures, logs, capturas o artefactos.
- [ ] CORS, auth, SQL dinamico y permisos se mantienen o quedan documentados.
- [ ] Dependencias nuevas justificadas en la spec o ADR.

## Riesgos residuales

- 
