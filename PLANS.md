# PLANS.md

Plantilla de planificacion para trabajo con IA en iLiniumTech.

Usa esta plantilla antes de iniciar cambios no triviales. El plan debe ser breve cuando la tarea sea pequena y mas detallado cuando toque contratos, seguridad, datos reales, CI o UX visible.

## Plantilla

### 1. Contexto

- Solicitud:
- Rama:
- SDD / issue / decision relacionada:
- Documentos leidos:
- Area afectada:

### 2. Objetivo

- Resultado esperado:
- Usuario o rol beneficiado:
- Como se verificara:

### 3. Fuera de alcance

- No se hara:
- Supuestos que no deben convertirse en codigo:
- Dependencias externas no disponibles:

### 4. Analisis inicial

- Estructura detectada:
- Convenciones aplicables:
- Riesgos tecnicos:
- Riesgos de seguridad/datos:
- Cambios concurrentes observados:

### 5. Plan por fases

Fase 1 - Descubrimiento:

- Archivos a leer:
- Contratos a revisar:
- Preguntas abiertas:

Fase 2 - Diseno:

- Enfoque elegido:
- Alternativas descartadas:
- Impacto esperado:

Fase 3 - Implementacion:

- Archivos previstos:
- Cambios por modulo:
- Migraciones/configuracion si aplica:

Fase 4 - Pruebas:

- Tests unitarios:
- Tests de integracion:
- Smoke manual o automatizado:
- Auditorias:

Fase 5 - Documentacion y cierre:

- Docs a actualizar:
- Evidencia a incluir en PR:
- Riesgos residuales:

### 6. Checklist antes de tocar archivos

- [ ] `git status --short --branch` revisado.
- [ ] SDD/decision consultada o creada si el cambio es funcional.
- [ ] Scope acotado.
- [ ] Secretos y datos personales identificados como no versionables.
- [ ] Riesgos de AppBuilder runtime revisados.
- [ ] Si hay cambios ajenos, no se van a revertir.

### 7. Checklist de validacion

Backend:

- [ ] `dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx`
- [ ] `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- [ ] `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`

Frontend:

- [ ] `cd .\iLiniumTech.Frontend`
- [ ] `npm ci`
- [ ] `npm run format`
- [ ] `npm run lint`
- [ ] `npm run test:unit`
- [ ] `npm run build`

Calidad y seguridad:

- [ ] `.\tools\quality\Invoke-MvpQualityGate.ps1`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings`
- [ ] `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings`
- [ ] `git diff --check`

### 8. Evidencia final

- Comandos ejecutados:
- Resultado:
- Archivos tocados:
- Pruebas no ejecutadas:
- Motivo de skips:
- Riesgos residuales:
- Bloqueos externos:
- Siguiente paso recomendado:

## Reglas para planes con multiples agentes

- Define un jefe por area: backend/datos, frontend/producto, calidad/CI/docs, seguridad/plataforma, producto/SDD.
- Cada jefe debe tener propiedad de rutas clara.
- Ningun jefe debe revertir cambios de otro.
- Las subtareas deben ser independientes y con salida verificable.
- La coordinacion final debe integrar resultados, ejecutar gates y dejar evidencia.
