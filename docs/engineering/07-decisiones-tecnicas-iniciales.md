# Decisiones tecnicas iniciales

Estas decisiones vienen de las ADR de `AcademiaLasCortes` en rama `demo`, adaptadas a iLiniumTech. Cuando el proyecto tenga codigo real, se podran convertir en ADR formales dentro de este repositorio.

## 0001 - Stack de pruebas frontend

Estado: adoptado como objetivo.

Decision:

- Vitest para pruebas unitarias frontend.
- Vue Test Utils para componentes Vue.
- Playwright para smoke y flujos completos.

Motivo:

- Encaja con Vue 3, Vite y TypeScript.
- Permite probar stores, composables, transformadores y componentes Vue estaticos.
- Evita depender de E2E para reglas pequenas.

Aplicacion en iLiniumTech:

- Probar transformadores de metadata AppBuilder solo como extraccion/scaffolding.
- Probar componentes Vue estaticos soportados en el MVP.
- Probar estados loading, empty, error y permisos.

## 0002 - Stack de pruebas backend

Estado: adoptado como objetivo.

Decision:

- xUnit para pruebas backend.
- FluentAssertions para aserciones legibles.
- `Microsoft.AspNetCore.Mvc.Testing` para integracion API.
- Testcontainers SQL Server cuando el comportamiento dependa de SQL Server real.

Motivo:

- Stack mantenible para .NET.
- Cubre routing, DI, autenticacion, serializacion y filtros reales.
- Permite separar unitarias rapidas de integracion.

Aplicacion en iLiniumTech:

- Probar arranque con y sin secretos obligatorios.
- Probar endpoints 401/403.
- Probar Search/Data con BBDD de test o fixtures.
- Probar que errores no filtran SQL ni connection strings.

## 0003 - Politica de versiones de paquetes

Estado: adoptado como objetivo.

Decision:

- Lockfiles versionados.
- `npm ci` en CI.
- Versiones NuGet fijadas en proyectos bajo gates.
- Sin comodines tipo `9.*` en paquetes criticos.
- Upgrades de tooling en cambios separados.

Motivo:

- Builds reproducibles.
- Diagnostico mas claro.
- Menos riesgo de vulnerabilidades reintroducidas por resoluciones transitivas.

Aplicacion en iLiniumTech:

- Cada app nueva debe nacer con lockfile.
- La auditoria de dependencias debe ejecutarse antes de aceptar nuevas librerias.
- Las dependencias para GraphQL, SQL, auth, scaffolding desde metadata o workflows requieren justificacion en spec o ADR.

## 0004 - Seguridad como gate temprano

Estado: adoptado para este proyecto.

Decision:

- Gitleaks desde el inicio.
- Auditoria de dependencias desde el inicio.
- CORS audit cuando haya API.
- Checklist de revision de seguridad para cambios sensibles.

Motivo:

- El analisis de AppBuilder detecto riesgos que no deben repetirse.
- iLiniumTech trabajara con conexiones, metadata heredada usada para extraccion y posiblemente datos reales de prueba.

Aplicacion en iLiniumTech:

- No commitear secretos aunque sean temporales.
- No usar datos reales en fixtures, capturas o reportes.
- No implementar SQL dinamico sin whitelist y pruebas.
- No exponer endpoints genericos sin autorizacion.
