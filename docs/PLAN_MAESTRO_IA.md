# Plan maestro IA iLiniumTech

Fecha: 2026-05-15

Estado: guia operativa de largo plazo para agentes IA, jefes de area y colaboradores humanos.

## Principio no negociable

iLiniumTech no es un AppBuilder dinamico reconstruido en Vue. AppBuilder es fuente heredada de analisis, trazabilidad, comparativa, SDD y scaffolding revisado. El producto final debe quedar como:

- frontend Vue/TypeScript con pantallas y componentes escritos como codigo fuente mantenible;
- backend .NET con API explicita por caso de uso;
- datos consultados mediante contratos, parametros, whitelists y permisos propios;
- ninguna decision runtime de UI, permisos, queries o workflows basada en metadata AppBuilder.

Si una tarea pide algo que no encaja con este principio o no tiene sentido dentro del producto, el arquitecto IA debe preguntar antes de planificar o programar.

## Estado real reanalizado

Base entregada:

- Gobierno inicial: `AGENTS.md`, `PLANS.md`, DoD, workflow IA, roadmap, SDD y gates locales.
- Frontend: Vue 3 + Vite + TypeScript con `/login`, shell estatico, menu lateral, `/polizas`, detalle de poliza y paginas protegidas estaticas del menu. `Autos Particulares` existe como ruta tecnica aparcada/deshabilitada.
- Backend: API .NET con `/health`, `/ready`, `/api/auth/login`, `/api/auth/logout`, `/api/me`, `/api/polizas/catalogs`, `/api/polizas`, `/api/polizas/{id}` y politicas iniciales `polizas.catalogs`, `polizas.read`, `polizas.detail`. El nuevo objetivo MVP activo es evolucionar `Polizas` a CRUD contra BBDD local autorizada, segun `SDD-2026-007`.
- Broker activo: selector frontend y `POST /api/auth/broker` entregados para `DemoSession`, validando contra `allowedBrokerIds`, releyendo `/api/me` y refrescando Polizas.
- Datos: repositorio `InMemory` por defecto, repositorio SQL read-only activable por configuracion, resolver `AppBuilderMaster`, whitelists, parametros y `SESSION_CONTEXT` parametrizado. Para el objetivo CRUD, las escrituras deben usar contratos iLiniumTech explicitos, transacciones, permisos propios y BBDD local de pruebas sin versionar secretos.
- Seguridad MVP: API key temporal, `DemoSession` y headers MVP clasificados como compatibilidad local/demo, no como auth productiva.
- QA/CI: gate local, validacion documental, auditorias de secretos/dependencias/CORS, CI baseline, CodeQL y preview dry-run.
- Extractor offline: `tools/extractor/polizas-metadata` con modos `Fixture`, `DryRun` y `Live`, sin uso runtime productivo.

Puntos aparcados o no cerrados:

- `Autos Particulares` existe como incremento tecnico anterior, pero no es el objetivo MVP actual. No debe ampliarse ni presentarse como objetivo principal sin nueva confirmacion funcional, SDD y UAT.
- Las paginas estaticas del menu son visibles para orientar navegacion y pruebas, pero estan bloqueadas para datos reales, filtros funcionales, escrituras, exportaciones, permisos finos o APIs propias hasta SDD/API/UAT.
- La autenticacion productiva no esta decidida.
- Falta matriz real de permisos por broker, perfil, oficina, gestor y usuario.
- SQL de Polizas CRUD ya tiene smoke transaccional con rollback contra BBDD local autorizada; falta smoke API/UI real con backend SQL y confirmar claves de `SESSION_CONTEXT` con DBA.
- Falta destino preview real, environments, secrets y branch protection final.
- Falta UAT funcional con responsable humano y datos autorizados.

## Horizonte de producto

El objetivo final es una aplicacion profesional Frontend -> Backend, evolucionable como cualquier producto de software, no dependiente de metadata heredada.

El MVP vigente queda actualizado el 2026-05-18 y debe centrarse en:

1. login y sesion controlada;
2. menu lateral y shell estatico profesional;
3. pantalla de polizas con CRUD controlado contra BBDD local autorizada;
4. permisos backend efectivos para lectura y escritura de polizas;
5. multi-tenant/broker validado antes de resolver conexion o escribir datos;
6. calidad, seguridad, documentacion y evidencia de cierre;
7. una vez cerrado Polizas CRUD, crear agentes por pagina del menu para analizar, documentar y evolucionar el resto de paginas con SDD propia.

Todo lo que exceda ese MVP debe entrar como SDD propia.

## Organizacion continua de agentes

El arquitecto/coordinador principal no debe programar todas las piezas grandes a la vez. Debe coordinar jefes de area y reservarse la integracion final, el control de alcance, la revision de riesgos y la decision de cierre.

Jefes recomendados:

- Jefe Producto/Arquitectura: SDD, alcance, decisiones, roadmap, riesgos de producto y UAT.
- Jefe Backend/Datos/Seguridad: API, auth, permisos, multi-tenant, SQL, `SESSION_CONTEXT`, errores sanitizados y pruebas backend.
- Jefe Frontend/UX: login, shell, menu, polizas, componentes Vue, accesibilidad, estados y smoke visual.
- Jefe QA/DevOps/Gobierno IA: gates, CI, seguridad, preview, evidencias, PR y Definition of Done.
- DBA/Entorno real: actor externo; valida BBDD, cuentas read-only, vistas/tablas, claves de contexto, PII y muestras UAT.

Reglas de reparto:

- Cada jefe puede dividir trabajo en agentes programadores, revisores o exploradores.
- Cada tarea debe declarar archivos permitidos, archivos prohibidos, dependencias, pruebas y documentacion.
- Ningun agente debe revertir cambios ajenos.
- Los jefes reportan rutas tocadas, comandos, resultados, skips, riesgos y bloqueos.
- El arquitecto integra, ejecuta gates, actualiza documentacion y decide si se puede hacer commit/push.
- Si dos tareas comparten contrato backend/frontend, primero se congela contrato en SDD o decision corta.

Cadencia por incremento:

1. Confirmar objetivo y sentido de producto.
2. Leer `AGENTS.md`, `PLANS.md`, este plan, roadmap, decision de arquitectura y SDD relacionada.
3. Dividir en tareas pequenas con propiedad de rutas.
4. Lanzar jefes en paralelo.
5. Integrar resultados en ramas `codex/<area>-<descripcion>`.
6. Ejecutar pruebas y auditorias aplicables.
7. Actualizar documentacion y evidencias.
8. Commit y push solo cuando el cierre sea explicable.

## Preguntas obligatorias antes de aceptar objetivos ambiguos

Si el usuario pide un modulo, vertical o alcance que no aparece claro en el producto actual, preguntar:

- Que problema de negocio resuelve y para que usuario?
- Existe en AppBuilder actual o es funcionalidad nueva?
- Cual es la pantalla, menu, accion o flujo original que se quiere estudiar?
- Que datos necesita y de que origen autorizado salen?
- Que permisos y broker aplican?
- Hay SDD, decision o UAT owner?
- Es parte del MVP actual o un incremento posterior?

No asumir que un nombre de negocio implica una regla de BBDD valida. El caso `Autos Particulares` queda como recordatorio: si no hay regla confirmada para el concepto funcional, se documenta bloqueo y se pregunta.

## Fase 0 - Gobierno, mapa y control de alcance

Objetivo: asegurar que cualquier agente entiende el producto antes de tocar codigo.

Pasos y tareas:

- Mantener `AGENTS.md`, `PLANS.md`, `docs/workflows/ai-development-flow.md`, DoD y roadmap como fuente minima.
- Revisar cada nuevo objetivo contra `docs/DECISION_PRODUCTO_ARQUITECTURA.md`.
- Crear o actualizar SDD si el cambio afecta producto, datos, seguridad, permisos, contrato API o UX visible.
- Clasificar cada iniciativa: backend, frontend, datos, seguridad, QA/CI, docs, UAT o entorno real.
- Marcar explicitamente lo que queda fuera de alcance.
- Registrar falsos objetivos o objetivos aparcados para que no se conviertan en deuda invisible.

Agentes:

- Producto/Arquitectura dirige.
- QA/DevOps revisa DoD y evidencias.

DoD y evidencia:

- Documentos base leidos.
- SDD/issue/decision enlazada o bloqueo escrito.
- Scope y fuera de alcance claros.
- `git status --short --branch` revisado.
- Riesgo AppBuilder runtime revisado.

## Fase 1 - Cierre profesional del MVP actual

Objetivo: congelar una base demostrable antes de ampliar alcance.

Pasos y tareas:

- Confirmar que `/login -> /polizas -> detalle -> logout` funciona en modo demo/backend.
- Alinear documentacion con el estado real: `DemoSession`, API key y headers MVP son temporales.
- Revisar que el menu lateral sea estatico y no dependa de metadata.
- Revisar que polizas no use metadata AppBuilder en runtime.
- Ejecutar regresion backend/frontend y smoke visual cuando aplique.
- Crear evidencia de cierre del MVP con puntos `completado con evidencia`, `pendiente tecnico` y `bloqueado externo`.

Agentes:

- Frontend/UX: smoke login, shell, menu y polizas.
- Backend/Datos: auth demo, `/api/me`, politicas y errores 401/403.
- QA/DevOps: gates y evidencia.

DoD y evidencia:

- Backend build/tests verdes o justificacion de no aplicacion.
- Frontend format/lint/unit/build verdes si se toca UI.
- Secret scan limpio.
- No hay datos reales ni secretos.
- Captura o descripcion smoke si cambia UI.
- Roadmap actualizado.

## Fase 2 - Login, sesion y shell robusto

Objetivo: convertir el login MVP en una experiencia fiable mientras se decide auth real.

Pasos y tareas:

- Priorizar sesion backend `DemoSession` sobre cualquier sesion local cuando `VITE_USE_BACKEND=true`.
- Tratar expiracion, 401, 403 y logout de forma consistente.
- Separar `/login` del shell protegido.
- Mantener `/api/me` como contrato unico de identidad, broker, permisos y modo de autenticacion.
- Mostrar estados claros: cargando sesion, sin sesion, sin broker, sin permiso, error backend.
- Probar refresh de navegador, entrada directa a ruta protegida y logout.
- Impedir fallback silencioso a fixtures si el backend falla en modo backend real.

Agentes:

- Frontend/UX dirige la experiencia y tests de guards/composables.
- Backend/Datos valida contrato `/api/me` y cookies.
- QA/DevOps aporta smoke visual.

DoD y evidencia:

- Tests unitarios de auth/session/guards.
- Smoke `login -> polizas -> logout`.
- Errores publicos sanitizados.
- No se loguean tokens, cookies ni datos personales.

## Fase 3 - Autenticacion productiva y permisos efectivos

Objetivo: sustituir compatibilidad MVP por identidad y autorizacion reales.

Pasos y tareas:

- Decidir proveedor auth: Microsoft Entra ID, OIDC corporativo, credenciales propias u otro.
- Definir ambientes: Local, Demo/UAT, Preview y Production.
- Definir contrato interno de contexto: usuario, broker activo, brokers permitidos, perfil, roles, permisos y `correlationId`.
- Crear servicio backend de contexto actual como autoridad unica.
- Mapear claims/sesion externa a permisos iLiniumTech.
- Definir matriz inicial: `polizas.catalogs`, `polizas.read`, `polizas.detail`, y futuros permisos por accion.
- Mantener `DemoSession` solo con opt-in local/demo.
- Deshabilitar API key y headers MVP como fuente de permisos en preview/produccion.
- Cubrir pruebas de anonimo, token invalido, token caducado, broker ausente, broker cruzado y permiso ausente.

Agentes:

- Backend/Datos/Seguridad dirige.
- Producto/Arquitectura valida modelo funcional de permisos.
- Frontend/UX consume `/api/me` sin inventar permisos.
- QA/DevOps valida gates y threat model.

DoD y evidencia:

- Decision de proveedor auth aprobada o bloqueo humano escrito.
- 401/403 sanitizados con `correlationId`.
- API key no concede permisos funcionales.
- Headers MVP no prevalecen sobre claims.
- Matriz de permisos documentada.

## Fase 4 - Multi-tenant, broker y maestro de BBDD

Objetivo: impedir lectura cruzada entre brokers antes de consultar datos reales.

Pasos y tareas:

- Validar `currentBrokerId` contra `allowedBrokerIds` antes de resolver conexion.
- Definir endpoint o flujo explicito para cambiar broker activo; nunca confiar en header libre.
- Mantener `IAPM_Connection` como fuente temporal de conexion de modelo, no de pantallas.
- Validar con DBA filtros reales del resolver: activo, no deshabilitado, tipo modelo, entorno y aplicacion si aplica.
- Usar cuenta minima para maestro y cuenta read-only para modelo.
- Redactar por completo connection strings, servidor, base, usuario y password en logs.
- Cachear descriptores de conexion con TTL corto si se aprueba.
- Crear readiness no invasivo que no filtre detalles de infraestructura.

Agentes:

- Backend/Datos dirige.
- DBA/Entorno real valida reglas externas.
- QA/DevOps revisa no secretos y logs.

DoD y evidencia:

- Broker no permitido devuelve 403/404 generico antes de SQL.
- Broker sin conexion no filtra datos internos.
- No se loguean connection strings ni credenciales.
- Pruebas de concurrencia o aislamiento si hay pooling y `SESSION_CONTEXT`.
- Bloqueos DBA documentados si no hay entorno.

## Fase 5 - Datos reales read-only y `SESSION_CONTEXT`

Objetivo: consultar BBDD real autorizada sin abrir superficie dinamica ni exponer PII.

Pasos y tareas:

- Validar `SqlPolizasRepository` contra BBDD test/UAT autorizada.
- Confirmar tabla/vista final para polizas o sustituir por vistas autorizadas.
- Mantener whitelists de columnas, ordenacion y filtros en codigo iLiniumTech.
- Confirmar claves exactas de `SESSION_CONTEXT` con DBA.
- Poblar `SESSION_CONTEXT` desde auth real, no desde headers MVP.
- Sobrescribir o limpiar contexto por request para evitar contaminacion con pooling.
- Clasificar campos sensibles: documento, cliente, telefono, email, direccion, matricula, bastidor y datos financieros.
- Definir minimizacion, mascara o vaciado por permiso.
- Comparar resultados con AppBuilder solo como UAT/evidencia, no como runtime.

Agentes:

- Backend/Datos dirige repositorios e integracion.
- DBA/Entorno real valida BBDD, vistas y contexto.
- Producto/UAT valida muestras.
- QA/DevOps revisa seguridad y evidencias sin datos reales.

DoD y evidencia:

- Integracion SQL ejecutada o bloqueo externo clasificado.
- Payloads maliciosos en filtros/sort cubiertos por tests.
- `SELECT *`, SQL heredado libre y `QueryStatic` no se ejecutan.
- No hay fuga entre brokers.
- No se versionan capturas o muestras sensibles.

## Fase 6 - Polizas CRUD BBDD profesional

Objetivo: convertir Polizas en una pantalla diaria de trabajo con CRUD controlado contra BBDD local de pruebas, no solo una demo read-only.

SDD activa: `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`.

Estado 2026-05-18: backend CRUD, baja tecnica MVP, UI de alta/edicion/baja y smoke SQL transaccional estan implementados. El siguiente bloque obligatorio es smoke API/UI real con backend SQL, `Polizas:WritesEnabled`, permisos de escritura y limpieza verificable.

Pasos y tareas:

- Mantener listado, busqueda, filtros y paginacion con estado predecible.
- Llevar filtros relevantes a query params si mejora navegacion/soporte.
- Confirmar orden inicial, page sizes y columnas visibles con UAT.
- Separar campos sensibles por permiso.
- Normalizar estados loading, empty, error, no permission y no broker.
- Mejorar detalle con breadcrumb, vuelta conservando filtros y secciones revisables.
- Evitar revelar existencia de poliza en 403/404 cruzados.
- Migrar el identificador SQL de recurso hacia `dbo.Poliza.Id`; el numero visible vive en `Poliza`.
- Anadir permisos `polizas.create`, `polizas.update` y `polizas.delete`.
- Crear DTOs de escritura separados de lectura.
- Implementar create/update/delete con SQL parametrizado, transacciones y `SESSION_CONTEXT`.
- Bloquear escrituras si `Polizas:WritesEnabled` no esta activo.
- No usar delete fisico en el MVP: aplicar baja tecnica solo a registros `ILMVP-`, marcandolos `ILMVP-DELETED-` y ocultandolos del listado/detalle.
- Preparar pruebas de regresion para filtros, limpieza, paginacion, detalle, alta, edicion, borrado permitido y borrado bloqueado.

Agentes:

- Frontend/UX dirige componentes y flujo.
- Backend/Datos garantiza contrato y autorizacion.
- Producto/UAT valida columnas y comportamiento.
- QA/DevOps ejecuta smoke visual.

DoD y evidencia:

- Unit tests frontend y backend de filtros, contratos, validaciones y permisos CRUD.
- Pruebas SQL locales con rollback o limpieza verificable.
- Smoke `/polizas` y `/polizas/:id`.
- Smoke create -> read -> update -> baja tecnica cuando exista UI CRUD.
- Smoke API/UI real contra backend SQL local antes de abrir desarrollo del resto de paginas.
- Sin nombres SQL/AppBuilder/metadata en DOM.
- Sin secretos ni datos personales reales versionados.
- UAT o bloqueo funcional documentado.

## Fase 7 - Menu lateral, navegacion y sistema UX

Objetivo: que la aplicacion se parezca a un producto estable y permita crecer por pantallas explicitas.

Pasos y tareas:

- Mantener navegacion en codigo fuente, por ejemplo `appNavigation.ts`.
- Filtrar visibilidad por permisos iLiniumTech, no por menus AppBuilder.
- Definir taxonomia de menu: Polizas como MVP vigente, futuros modulos solo si tienen SDD.
- Implementar estados disabled o hidden de forma clara segun permiso/feature flag.
- Mejorar responsive real del menu y topbar.
- Anadir selector de broker solo cuando backend valide broker activo.
- Extraer componentes compartidos utiles: tabla, panel de busqueda, paginacion, empty/error/loading, detalle.
- Evitar sobreabstraccion que recree AppBuilder con otro nombre.
- Revisar accesibilidad: foco visible, teclado, contraste, landmarks y textos en botones.

Agentes:

- Frontend/UX dirige.
- Producto/Arquitectura valida taxonomia.
- Backend/Datos aporta permisos y broker.
- QA/DevOps ejecuta smoke desktop/mobile si cambia visual.

DoD y evidencia:

- Menu estatico y testeado.
- Usuario ve solo capacidades permitidas.
- Broker activo explicito y validado por backend.
- UI sin solapes ni textos cortados en viewport principal.
- Smoke visual documentado si cambia UI.

## Fase 8 - Seguridad, privacidad, auditoria y observabilidad

Objetivo: poder operar con datos reales sin filtrar secretos ni datos personales.

Pasos y tareas:

- Mantener gitleaks/secret scan bloqueante.
- Mantener dependency audit sin findings high/critical salvo excepcion humana.
- Mantener CORS sin `AllowAnyOrigin` y sin origenes improvisados.
- Sanitizar errores publicos con `correlationId`.
- Estandarizar logs estructurados: usuario interno, broker, permiso, resultado y correlacion.
- No loguear tokens, cookies, SQL completo, connection strings, documento, telefono, email o direccion.
- Definir auditoria de accesos sensibles: busqueda, detalle, denegados y cambio de broker.
- Evaluar OpenTelemetry o trazas solo si aporta operacion real.
- Separar `/health` de readiness.

Agentes:

- QA/DevOps/Gobierno y Backend/Datos comparten liderazgo.
- Seguridad/Plataforma revisa CI, secrets y configuracion.

DoD y evidencia:

- Reportes security limpios.
- Evidencia de logs sanitizados.
- Threat model actualizado para auth, broker, SQL y PII.
- Riesgos residuales aceptados o bloqueados.

## Fase 9 - CI, preview y entrega controlada

Objetivo: que cada entrega sea repetible, revisable y demostrable.

Pasos y tareas:

- Consolidar checks GitHub: documentacion, backend, frontend, security, CodeQL y preview dry-run.
- Activar branch protection solo tras ejecuciones verdes estables.
- Sustituir placeholders CODEOWNERS por equipos/personas reales antes de exigirlo.
- Definir destino preview para frontend estatico y backend API.
- Crear GitHub environment `preview` con reviewers y secrets reales fuera del repo.
- Ejecutar `preview-dry-run.yml` y revisar artefactos antes de deploy real.
- Definir smoke API/UI contra preview.
- Documentar rollback y runbook de demo.

Agentes:

- QA/DevOps dirige.
- Seguridad/Plataforma valida secrets/environments.
- Backend y Frontend aportan smoke especifico.

DoD y evidencia:

- PR obligatoria contra `main`.
- Checks requeridos estables.
- Artefactos de tests y seguridad publicados.
- Preview no usa metadata AppBuilder runtime.
- Secrets solo en GitHub/environment, no en Git.

## Fase 10 - Extractor offline y migracion guiada por SDD

Objetivo: usar AppBuilder como conocimiento migrable sin convertirlo en dependencia productiva.

Pasos y tareas:

- Definir cuando se ejecuta extractor y quien puede hacerlo.
- Mantener salida por defecto fuera de versionado salvo decision explicita.
- Sanitizar secretos, SQL, connection strings y datos personales.
- Separar trazabilidad heredada de sugerencias de scaffolding.
- Revisar cada artefacto antes de convertirlo en SDD o codigo.
- Usar metadata para descubrir campos/pantallas, no para decidir runtime.
- Bloquear cualquier PR que consuma JSON extractor desde frontend/backend productivo.

Agentes:

- Producto/Arquitectura dirige criterios.
- Backend/Datos revisa seguridad de consultas.
- QA/DevOps valida tests y redaccion.

DoD y evidencia:

- Extractor live solo con entorno read-only autorizado.
- Artefactos sanitizados.
- Ningun runtime productivo lee metadata heredada.
- Todo scaffold aceptado queda como codigo revisado.

## Fase 11 - Expansiones funcionales posteriores y resto de paginas

Objetivo: anadir capacidades nuevas solo como producto explicito.

Pasos y tareas:

- No iniciar desarrollo real de otras paginas hasta cerrar el MVP de Polizas CRUD BBDD con evidencia o documentar un bloqueo tecnico explicito.
- Cuando Polizas CRUD este cerrado, crear agentes por pagina del menu.
- Cada agente de pagina debe analizar componentes AppBuilder originales, documentar SDD/alcance y proponer implementacion estatica Vue/API explicita.
- Priorizar con producto: detalle ampliado, exportacion, documentos, recibos, siniestros, acciones o escrituras.
- Crear SDD por cada caso de uso.
- Definir datos afectados, permisos, auditoria, rollback y UAT.
- Separar workflows heredados en casos de uso iLiniumTech.
- Para escrituras: transacciones, validacion, idempotencia si aplica, auditoria y pruebas de integracion.
- No ejecutar REST/SOAP/workflows heredados de forma generica.

Agentes:

- Producto/Arquitectura prioriza.
- Backend/Datos disena contratos y transacciones.
- Frontend/UX implementa experiencia explicita.
- QA/DevOps exige UAT y evidencia.

DoD y evidencia:

- SDD aprobada.
- Permisos y auditoria definidos.
- Tests unitarios/integracion/smoke.
- UAT firmado o bloqueo externo.
- Riesgos residuales aceptados.

## Fase 12 - Preparacion productiva

Objetivo: estar listo para operar con usuarios y datos reales.

Pasos y tareas:

- Configurar entornos sin secretos en Git.
- Retirar API key, headers MVP y demo-session en entornos con datos reales.
- Completar observabilidad minima: health, readiness, logs sanitizados, correlacion y alertas.
- Ejecutar pruebas de carga basicas sobre rutas criticas.
- Revisar proteccion de datos y PII.
- Definir runbooks de incidencias, releases, rollback y soporte.
- Confirmar backup/restore o continuidad si aplica.
- Obtener UAT firmado por cada flujo incluido.
- Documentar riesgos residuales aceptados por responsable humano.

Agentes:

- QA/DevOps y Seguridad/Plataforma dirigen operacion.
- Backend/Frontend corrigen hallazgos.
- Producto/UAT firma funcionalmente.
- DBA/Entorno real valida datos y permisos.

DoD y evidencia:

- Produccion no usa mecanismos demo.
- UAT firmado.
- Branch protection estable.
- Preview/prod con secrets fuera de Git.
- Riesgos residuales aceptados.
- No existe dependencia runtime de AppBuilder para UI, permisos, queries o workflows.

## Orden recomendado de proximos incrementos

1. Mantener como objetivo activo `SDD-2026-007 Polizas CRUD BBDD MVP`.
2. Migrar identificador SQL de Polizas a `dbo.Poliza.Id` para evitar escrituras por numero duplicado.
3. Anadir permisos `polizas.create`, `polizas.update`, `polizas.delete` y pruebas 401/403.
4. Implementar backend CRUD local con `Polizas:WritesEnabled`, transacciones, SQL parametrizado y baja tecnica limitada a registros MVP.
5. Ejecutar create -> read -> update -> baja tecnica contra BBDD local autorizada sin versionar credenciales.
6. Implementar UI CRUD en `/polizas` solo cuando backend y permisos esten listos.
7. Ejecutar regresion backend/frontend, E2E y auditorias.
8. Documentar evidencia QA y riesgos residuales.
9. Solo despues, crear agentes por pagina del menu para planificar y desarrollar el resto con SDD propia.
10. Consolidar CI/preview dry-run y branch protection.

## Riesgos principales

- Reintroducir AppBuilder por la puerta trasera mediante metadata para menus, pantallas, queries, permisos o workflows.
- Perpetuar API key, headers MVP o `DemoSession` como auth productiva.
- Resolver conexion o consultar SQL antes de validar broker autorizado.
- Contaminar `SESSION_CONTEXT` por pooling o headers manipulables.
- Exponer PII por reutilizar columnas heredadas sin clasificacion.
- Usar fixtures como fallback silencioso en modo backend real.
- Presentar como cerrado un concepto funcional sin regla BBDD/UAT confirmada.
- Activar preview o required checks antes de tener ejecuciones verdes y secretos bien configurados.
- Guardar connection strings, dumps, capturas sensibles o datos personales reales en Git.

## Comandos de referencia

Backend:

```powershell
dotnet restore .\iLiniumTech.Backend\iLiniumTech.Backend.slnx
dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release
```

Frontend:

```powershell
cd .\iLiniumTech.Frontend
npm ci
npm run format
npm run lint
npm run test:unit
npm run build
```

Gate local:

```powershell
.\tools\quality\Invoke-MvpQualityGate.ps1
```

Validacion documental:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1
```

Auditorias:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-DependencyAudit.ps1 -FailOnFindings
powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-CorsAudit.ps1 -FailOnFindings
```

## Regla de cierre

Una fase, PR o incremento no esta Done si falta cualquiera de estos puntos aplicables:

- SDD, issue o decision enlazada.
- Gates aplicables verdes o skips justificados.
- Evidencia reproducible sin secretos ni datos reales.
- Seguridad limpia.
- UAT si afecta comportamiento funcional.
- Riesgos residuales documentados.
- Bloqueos externos clasificados con responsable.
- Confirmacion explicita de que no se introdujo runtime AppBuilder.
