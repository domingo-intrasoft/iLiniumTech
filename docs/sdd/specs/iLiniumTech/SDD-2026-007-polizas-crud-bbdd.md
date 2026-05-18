# SDD: Polizas CRUD BBDD MVP

## Metadata

- Spec ID: SDD-2026-007
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature/data/security
- Tamano SDD: L
- Estado SDD: draft-active
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

El objetivo MVP cambia: la pantalla de `Polizas` deja de ser solo read-only y debe evolucionar hacia CRUD contra BBDD local autorizada para pruebas. El principio de arquitectura se mantiene: iLiniumTech no debe reconstruir AppBuilder como runtime dinamico; la pantalla Vue, los contratos API, validaciones, permisos y comandos SQL viven como codigo iLiniumTech explicito.

La base actual ya incluye:

- frontend Vue para `/polizas` y `/polizas/:id`;
- backend .NET con `GET /api/polizas/catalogs`, `GET /api/polizas` y `GET /api/polizas/{id}`;
- repositorio `InMemory` por defecto;
- repositorio SQL read-only configurable;
- resolver `AppBuilderMaster` que localiza la BBDD modelo `tipobd-MO` desde `IL_Maestro`/`IAPM_Connection` por broker;
- `SESSION_CONTEXT` parametrizado antes de consultas SQL;
- permisos read-only `polizas.catalogs`, `polizas.read` y `polizas.detail`.

## Hallazgos locales iniciales

Analisis de solo lectura ejecutado el 2026-05-18 contra BBDD local de pruebas, sin versionar credenciales ni connection strings:

- `IL_Maestro.dbo.IAPM_Connection` existe y contiene conexiones modelo `tipobd-MO` por `IdentityId`.
- Para un broker local activo se resolvio una BBDD modelo de pruebas mediante el algoritmo compatible con AppBuilder; no se debe escribir esa conexion en Git.
- `dbo.Pantalla_Polizas` en la BBDD modelo es una `VIEW`, no una tabla.
- `dbo.Pantalla_Polizas` referencia `dbo.Poliza` y tablas relacionadas de identidad, direccion, riesgos, catalogos y alertas.
- `dbo.Pantalla_Polizas.Poliza` no es identificador unico: se detectaron duplicados por numero de poliza.
- Para CRUD, el identificador recurso debe migrar hacia `dbo.Poliza.Id` como id estable interno; `Poliza` debe tratarse como numero visible.
- `dbo.Poliza` tiene triggers de auditoria/calculo/ajustes/division/oficina/anulacion. Las escrituras deben pasar por transaccion, pruebas rollback y UAT.
- `dbo.Poliza` contiene campos sensibles y relaciones con cliente, cuenta bancaria, direccion, gestor, colaborador y riesgo. No deben exponerse ni editarse sin permiso/SDD.

## Objetivo

Entregar un MVP profesional de `Polizas` con CRUD controlado contra BBDD local de pruebas:

- `Create`: crear polizas de prueba marcadas como originadas por iLiniumTech MVP.
- `Read`: mantener listado, filtros y detalle contra BBDD, usando identificador estable.
- `Update`: editar solo campos permitidos por contrato y permisos.
- `Delete`: permitir borrado fisico solo para registros creados por iLiniumTech MVP o, si producto lo redefine, sustituir por anulacion/logical delete con SDD/UAT.

La primera implementacion debe ser segura aunque sea limitada. Es preferible un CRUD parcial, probado y bloqueado por permisos/configuracion, antes que abrir escrituras generales sobre cartera real.

## Fuera de alcance

- Ejecutar metadata AppBuilder como runtime de pantalla, permisos, queries o workflows.
- Crear un CRUD generico de tabla o datasource.
- Exponer connection strings, usuarios, passwords, dumps, capturas sensibles o datos personales reales.
- Abrir escritura productiva con API key, headers MVP o `demo-session` sin opt-in de entorno local/demo.
- Editar datos sensibles: documento, telefono, email, direccion, cuenta bancaria, riesgo completo, matricula, bastidor o datos financieros no aprobados.
- Ejecutar workflows heredados, REST/SOAP heredados o reglas AppBuilder genericas.
- Hacer borrado fisico de polizas existentes no creadas por el MVP iLiniumTech.

## Contrato objetivo API

Rutas objetivo bajo `/api/polizas`:

- `GET /api/polizas/catalogs`: catalogos de filtros y formularios.
- `GET /api/polizas`: listado paginado.
- `GET /api/polizas/{id}`: detalle por `dbo.Poliza.Id` en SQL.
- `POST /api/polizas`: alta controlada de poliza MVP.
- `PUT /api/polizas/{id}` o `PATCH /api/polizas/{id}`: modificacion de campos permitidos.
- `DELETE /api/polizas/{id}`: borrado restringido a registros creados por el MVP o accion equivalente aprobada.

Permisos objetivo:

- `polizas.catalogs`
- `polizas.read`
- `polizas.detail`
- `polizas.create`
- `polizas.update`
- `polizas.delete`

En entornos con datos reales, los permisos de escritura no deben concederse por API key ni cabeceras MVP. Para pruebas locales se puede usar `demo-session` con configuracion explicita y documentada.

## Campos iniciales permitidos

Campos candidatos para primer CRUD local, pendientes de UAT:

| Campo API | BBDD objetivo | Operacion | Regla |
| --- | --- | --- | --- |
| `numero` | `Poliza.Poliza` | Create/Update | Obligatorio en create; longitud maxima 50; validar unicidad junto a `ciaId` y `aplicacion` si aplica. |
| `aplicacion` | `Poliza.Aplicacion` | Create/Update | Valor controlado; longitud maxima 30. |
| `ciaId` | `Poliza.CiaId` | Create | Obligatorio; entero positivo. |
| `clienteId` | `Poliza.ClienteId` | Create | Obligatorio; entero positivo; no exponer documento ni datos personales. |
| `estado` | `Poliza.IdSituacion` | Create/Update | Valor catalogado. |
| `ramo` | `Poliza.IdRamo` | Create/Update | Valor catalogado. |
| `tipoPoliza` | `Poliza.IdTipoPoliza` | Create/Update | Valor catalogado. |
| `fechaEfecto` | `Poliza.F_Efecto` y `F_EfectoPrimero` | Create/Update | Fecha ISO valida. |
| `fechaVencimiento` | `Poliza.F_Vencimiento` | Create/Update | Opcional; si existe debe ser mayor o igual a efecto. |
| `primaAnual` | `Poliza.PAnualCartera` | Create/Update | Decimal no negativo; revisar si tambien actualiza `PAnualProduccion`. |

Marcador obligatorio para registros creados por el MVP:

- `IdSistemaOrigen = 'origen-iLiniumTech-MVP'`

El borrado fisico inicial solo puede afectar registros con ese marcador. Para registros existentes, `DELETE` debe devolver error funcional seguro hasta que producto defina anulacion/baja.

## Plan por fases

### Fase A - Preparacion y seguridad

- Actualizar plan maestro y roadmap con este objetivo como prioridad activa.
- Confirmar que el recurso SQL usa `dbo.Poliza.Id`, no `Poliza.Poliza`, para detalle y CRUD.
- Anadir permisos `polizas.create`, `polizas.update`, `polizas.delete` sin concederlos por defecto fuera de demo/local.
- Crear DTOs de escritura separados de DTOs de lectura.
- Bloquear escrituras si `Polizas:WritesEnabled` no esta activado.

### Fase B - Backend CRUD local

- Implementar comandos de create/update/delete en Application.
- Implementar SQL parametrizado sobre `dbo.Poliza`, no sobre metadata.
- Usar transaccion por operacion.
- Aplicar `SESSION_CONTEXT` antes de escribir.
- Sanitizar errores y devolver `correlationId`.
- Tests unitarios de validacion, permisos, query/command builder y errores.

### Fase C - Prueba BBDD local

- Ejecutar pruebas contra BBDD local autorizada sin versionar credenciales.
- Para escrituras de prueba, usar registros marcados `origen-iLiniumTech-MVP`.
- Probar create -> read -> update -> read -> delete en entorno local.
- No dejar datos residuales salvo que se documenten como fixture local intencionada.

### Fase D - Frontend CRUD

- Convertir acciones bloqueadas de `Polizas` en acciones reales solo cuando el contexto tenga permiso.
- Crear formulario de alta/edicion para campos aprobados.
- Mostrar estados guardando, guardado, conflicto, error y sin permiso.
- Confirmar delete con modal o pantalla de confirmacion.
- No mostrar campos sensibles no aprobados.

### Fase E - Despues de Polizas

Cuando el CRUD de `Polizas` este cerrado con evidencia, crear agentes por pagina del menu para analizar y evolucionar el resto de paginas. Cada agente debe partir de la documentacion AppBuilder ya generada, crear o actualizar SDD propia y no activar datos reales ni escrituras sin contrato equivalente.

## Criterios de aceptacion

- `Polizas CRUD BBDD` queda registrado como objetivo MVP activo en plan maestro, roadmap y reglas de agentes.
- El backend no ejecuta metadata AppBuilder como runtime de CRUD.
- El recurso SQL de escritura usa identificador estable `dbo.Poliza.Id` y no el numero visible duplicable.
- Las escrituras quedan bloqueadas por defecto y solo se habilitan con configuracion local/demo explicita.
- Las rutas CRUD exigen permisos `polizas.create`, `polizas.update` y `polizas.delete` segun corresponda.
- Las operaciones SQL usan parametros, transacciones y `SESSION_CONTEXT`.
- El delete fisico inicial solo puede afectar registros marcados como creados por iLiniumTech MVP.
- Frontend muestra acciones CRUD solo con permisos y contexto validos.
- La evidencia local no contiene connection strings, credenciales, dumps ni PII real.

## Seguridad

- No guardar secretos ni connection strings reales en Git.
- No confiar en API key, headers MVP ni valores del frontend para autorizar escrituras productivas.
- Validar broker permitido antes de resolver conexion o escribir.
- Sanitizar errores publicos con `correlationId` y sin nombres internos de tablas cuando no sean necesarios para el usuario.
- No loguear SQL completo, parametros sensibles, documento, telefono, email, cuenta bancaria, direccion ni riesgo completo.
- Mantener whitelist de campos editables en codigo iLiniumTech.
- Usar transacciones y limpieza verificable en pruebas locales.
- No permitir borrado fisico de registros no marcados `origen-iLiniumTech-MVP` hasta decision UAT.

## Plan de pruebas

Backend:

- `dotnet build .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- `dotnet test .\iLiniumTech.Backend\iLiniumTech.Backend.slnx --configuration Release`
- Tests API 401/403 por permiso ausente en create/update/delete.
- Tests de validacion de DTOs y longitudes.
- Tests de SQL parametrizado sin concatenar datos del usuario.
- Tests de borrado bloqueado para registros no MVP.
- Prueba local BBDD con rollback o limpieza verificable.

Frontend:

- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`
- Tests de permisos: botones visibles/habilitados solo con permisos.
- Tests de formulario: validacion, guardado, error, delete bloqueado.
- Smoke E2E de flujo visible cuando exista UI CRUD.

Seguridad:

- Secret scan limpio.
- Dependency audit sin findings bloqueantes.
- CORS audit si cambia API/configuracion.
- Evidencia de que no se versionan connection strings ni datos personales.

## Riesgos

- `Pantalla_Polizas.Poliza` no es unico; usarlo como id de escritura puede modificar o borrar mas de una fila.
- Triggers de `dbo.Poliza` pueden ejecutar reglas no documentadas por iLiniumTech.
- Borrado fisico de polizas reales puede ser funcionalmente incorrecto; el MVP limita delete a registros marcados por iLiniumTech.
- API key, headers MVP y demo-session no son seguridad productiva.
- Campos de cliente, banco, direccion y riesgo pueden exponer PII si se incorporan al formulario sin permisos.
- `SESSION_CONTEXT` y reglas por broker/perfil/oficina/gestor siguen pendientes de validacion funcional completa.

## Definicion de hecho

- Roadmap y plan maestro apuntan a `Polizas CRUD BBDD` como objetivo MVP activo.
- CRUD backend implementado con permisos, configuracion, validacion y transacciones.
- Frontend permite CRUD solo cuando hay permisos y contexto valido.
- Pruebas locales contra BBDD ejecutadas sin dejar secretos ni datos sensibles versionados.
- No hay runtime AppBuilder de metadata.
- Evidencias QA documentadas.
- Riesgos residuales y bloqueos externos actualizados.
