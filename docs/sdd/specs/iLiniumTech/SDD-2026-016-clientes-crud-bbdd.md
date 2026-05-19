# SDD: Clientes CRUD BBDD local

## Metadata

- Spec ID: SDD-2026-016
- Work Item: T-302-CLIENTES-CRUD-SDD
- Aplicacion: iLiniumTech
- Tipo: feature/security/data
- Tamano SDD: L
- Estado SDD: spec-ready local guarded
- Responsable funcional: pendiente UAT
- Responsable tecnico: IA/Codex
- Fecha: 2026-05-19

## Contexto

La decision `docs/DECISION_DATOS_REALES_LOCALES.md` cambia la prioridad del MVP: las pantallas funcionales deben evolucionar hacia datos reales en la BBDD local del usuario, sin volver a un runtime dinamico AppBuilder ni mantener fixtures como objetivo funcional.

`Clientes` ya tiene un primer corte read-only SQL local documentado en `SDD-2026-010-clientes-read-only.md` y evidencia en `docs/qa/clientes-sql-readonly-local-evidence.md`. Ese corte confirma el agregado minimo:

- `IdentidadCliente.ClienteId` como clave de cliente y FK hacia `Identidad.Id`;
- `Identidad.BrokerIntegracionId` como filtro broker;
- `IdentidadCliente.FCR` e `Identidad.FCR` como fechas tecnicas;
- `Identidad.NumDocumento` existe, pero no se proyecta ni se usa como identificador publico;
- campos candidatos de identidad: `NombreCompleto`, `Nombre`, `Apellido1`, `Apellido2`, `RazonSocial`, `IdTipoDocumento`, `IdActividad`, `IdIdioma`, `IdCanalCobro`, `IdGestor`, `ComercialId`.

Clientes es una vertical de alto riesgo por PII. Esta SDD desbloquea una implementacion CRUD local y guardada para el MVP, pero no autoriza escrituras libres sobre clientes historicos ni exposicion de documento, contacto, direccion, banco o datos personales ampliados.

## Objetivo

Definir una vertical `Clientes CRUD BBDD local` capaz de:

- listar clientes desde SQL local real con contrato minimizado;
- crear clientes MVP locales reales sobre `Identidad` + `IdentidadCliente`;
- actualizar solo filas creadas por iLiniumTech MVP;
- eliminar de forma logica solo filas creadas por iLiniumTech MVP;
- mantener escrituras desactivadas por defecto;
- validar permisos, broker, transacciones, auditoria y limpieza verificable;
- conectar el frontend `/clientes` a API explicita sin metadata runtime.

## Fuera de alcance

- Escritura sobre clientes historicos creados por AppBuilder u otros sistemas.
- Borrado fisico de `Identidad` o `IdentidadCliente`.
- `NumDocumento`, tipo de documento, NIF/CIF/NIE, pasaporte o busqueda por documento.
- Email, telefono, direccion, IBAN, cuenta bancaria, mandato, contacto o datos personales ampliados.
- Fechas personales, salud, observaciones libres, notas o documentos adjuntos.
- Ficha completa, tabs de polizas, recibos, riesgos, siniestros, suplementos o metricas economicas.
- Exportacion masiva, importacion, duplicados avanzados, workflows o comunicaciones.
- Conversion automatica de permisos AppBuilder a permisos iLiniumTech.
- SQL heredado libre, endpoints genericos de datasource o renderer dinamico desde metadata.

## Contrato funcional

Lectura existente:

- `GET /api/clientes/catalogs`
- `GET /api/clientes`

Escritura MVP propuesta:

- `POST /api/clientes`
- `PUT /api/clientes/{id}`
- `DELETE /api/clientes/{id}`

El recurso publico usa `ClienteId` como id tecnico local. No se usa `NumDocumento` como clave, filtro ni parte de URL. La API no debe revelar si un cliente existe en otro broker.

### Payload create

Campos permitidos:

| Campo | Regla |
| --- | --- |
| `nombreMostrable` | Requerido, 2..120 caracteres, sin saltos de linea, se persiste en campos de nombre aprobados por el mapeo SQL. |
| `tipoCliente` | Opcional, catalogo local `particular` o `empresa`; no implica documento fiscal. |
| `segmento` | Opcional, solo si existe catalogo autorizado o valor tecnico no sensible. |
| `idIdioma` | Opcional, entero catalogado si la columna es nullable o DBA confirma valor por defecto. |
| `idActividad` | Opcional, entero catalogado si la columna es nullable o DBA confirma valor por defecto. |
| `idCanalCobro` | Opcional, solo etiqueta/codigo general; no cuenta ni banco. |
| `idGestor` | Opcional, solo si el catalogo esta autorizado para el broker. |
| `comercialId` | Opcional, solo si el catalogo esta autorizado para el broker. |

Campos generados por backend:

- `referencia`/`IdOld`: `ILMVP-CLI-<token>`.
- `BrokerIntegracionId`: desde contexto autenticado o cabecera MVP local, nunca desde payload.
- `FCR`, `FUM`, `UCR`, `UUM`: desde reloj y usuario de ejecucion.

### Payload update

Campos editables:

- `nombreMostrable`
- `tipoCliente`
- `segmento`
- `idIdioma`
- `idActividad`
- `idCanalCobro`
- `idGestor`
- `comercialId`

Debe incluir al menos un campo editable. La actualizacion solo aplica si:

- `Identidad.Id = IdentidadCliente.ClienteId`;
- `Identidad.BrokerIntegracionId = @brokerId`;
- `Identidad.IdOld LIKE 'ILMVP-CLI-%'`;
- `Identidad.IdOld NOT LIKE 'ILMVP-CLI-DELETED-%'`.

### Delete

`DELETE /api/clientes/{id}` es baja logica MVP, no borrado fisico.

Regla propuesta:

- solo filas creadas por iLiniumTech (`IdOld LIKE 'ILMVP-CLI-%'`);
- actualizar `Identidad.IdOld` a `ILMVP-CLI-DELETED-<id>-<token>`;
- actualizar `FUM`/`UUM`;
- excluir filas `ILMVP-CLI-DELETED-%` de listados MVP;
- si el esquema local no permite este marcador sin romper integridad, el endpoint debe responder `409` sanitizado y documentar `DELETE_BLOCKED_SCHEMA`.

No se debe eliminar fisicamente ninguna fila ni tocar clientes historicos.

## Seguridad

- Autenticacion obligatoria en todos los endpoints.
- Autorizacion exacta por permiso: `clientes.read`, `clientes.create`, `clientes.update`, `clientes.delete`.
- `Clientes:WritesEnabled=false` por defecto.
- Fuera de entorno local/demo, las escrituras requieren opt-in explicito igual que las verticales con CRUD real.
- Broker/tenant resuelto en backend antes de abrir conexion o ejecutar SQL.
- `brokerId`, usuario tecnico y permisos no se aceptan desde el payload.
- SQL parametrizado y estructura de filtros/sort por whitelist.
- Sin `SELECT *`, SQL libre, nombres de tablas en errores publicos ni valores SQL en logs.
- Sin documento, contacto, direccion, banco, observaciones ni PII ampliada en contrato publico.
- Errores sanitizados con `correlationId` cuando aplique.
- Auditoria sin PII y sin valores libres del payload.
- Secretos, connection strings, dumps y capturas con datos personales quedan fuera de Git.

## Reglas de negocio

- `Clientes:WritesEnabled=false` por defecto.
- Permisos nuevos:
  - `clientes.create`
  - `clientes.update`
  - `clientes.delete`
- Las escrituras requieren tambien `clientes.read` para verificar visibilidad del recurso.
- Fuera de entorno local/demo, las escrituras requieren el mismo opt-in de demo usado por las verticales con escritura.
- `Clientes:Repository=Sql` es obligatorio para CRUD real.
- El broker se resuelve antes de abrir conexion de modelo o ejecutar SQL.
- Ningun endpoint acepta `brokerId` desde el frontend.
- No hay fallback silencioso a in-memory cuando `VITE_USE_BACKEND=true` y `Clientes:Repository=Sql` esta configurado.
- La API debe fallar cerrado si faltan permisos, broker, flag o configuracion SQL.
- Los mensajes publicos deben ser sanitizados y, cuando aplique, incluir `correlationId`.

## Mapeo SQL minimo

Crear cliente debe operar en una transaccion unica:

1. Insertar `Identidad` con broker, marcador `ILMVP-CLI-*`, nombre minimizado y campos tecnicos.
2. Insertar `IdentidadCliente` apuntando al nuevo `Identidad.Id`.
3. Releer por `ClienteId`, broker y marcador MVP.
4. Confirmar que el contrato visible no proyecta PII prohibida.
5. Hacer commit solo si todos los pasos anteriores son correctos.

Actualizar cliente debe operar en una transaccion unica:

1. Bloquear o comprobar existencia por `ClienteId`, broker y marcador MVP.
2. Aplicar solo columnas whitelisted.
3. Actualizar `FUM`/`UUM`.
4. Releer y devolver contrato minimizado o `204`.

Eliminar cliente debe operar en una transaccion unica:

1. Comprobar existencia por `ClienteId`, broker y marcador MVP.
2. Marcar baja logica con `ILMVP-CLI-DELETED-*`.
3. Verificar que la fila ya no aparece en el listado MVP.

SQL obligatorio:

- parametrizado;
- sin concatenar valores de usuario;
- sort/filtros por whitelist;
- `WHERE` con broker en toda lectura/escritura;
- sin `SELECT *`;
- sin imprimir valores SQL ni datos de filas reales.

## Matriz de campos

Permitidos en contrato MVP:

- `id`
- `referencia`
- `nombreMostrable`
- `tipoCliente`
- `estado` si procede de catalogo autorizado
- `segmento` si procede de catalogo autorizado
- `gestor` como alias/codigo autorizado
- `canalCobro` como etiqueta general no bancaria
- `fechaAlta` tecnica

Permitidos en DML solo si la columna existe y pasa validacion local:

- `NombreCompleto`
- `Nombre`
- `Apellido1`
- `Apellido2`
- `RazonSocial`
- `IdActividad`
- `IdIdioma`
- `IdCanalCobro`
- `IdGestor`
- `ComercialId`
- `FCR`, `FUM`, `UCR`, `UUM`
- `IdOld` como marcador MVP
- `BrokerIntegracionId`

Prohibidos en contrato publico y logs:

- `NumDocumento`
- tipo de documento legal
- email, telefono, direccion, IBAN, cuenta bancaria, mandato
- nacimiento, sexo, estado civil, salud, profesion, observaciones
- polizas, recibos, siniestros, riesgos o importes relacionados
- connection strings, secretos, SQL completo con valores o dumps.

## Auditoria

Eventos minimos, sin PII:

- `clientes.create.requested`
- `clientes.create.succeeded`
- `clientes.create.failed`
- `clientes.update.requested`
- `clientes.update.succeeded`
- `clientes.update.failed`
- `clientes.delete.requested`
- `clientes.delete.succeeded`
- `clientes.delete.failed`

Campos permitidos:

- `operation`
- `clienteId` tecnico si ya existe
- `referencePrefix` o hash no reversible de `IdOld`
- `brokerId`
- `userId`/alias tecnico
- `correlationId`
- `result`
- `reasonCode` sanitizado

Campos prohibidos:

- nombre, documento, telefono, email, direccion, cuenta, valores libres del payload y SQL.

## Frontend MVP

`/clientes` debe seguir siendo una pagina Vue/TypeScript estatica, no generada por metadata.

Capacidades esperadas:

- listar desde API cuando `VITE_USE_BACKEND=true`;
- mostrar acciones create/edit/delete solo si `/api/me` o capacidades equivalentes conceden permiso;
- dialogos pequenos con campos permitidos;
- no mostrar ni solicitar documento, contacto, direccion o banco;
- avisos de error sanitizados;
- confirmar baja logica antes de `DELETE`;
- no ofrecer exportacion ni ficha completa en este corte.

## Configuracion local

Variables esperadas, siempre fuera de Git:

```powershell
$env:Clientes__Repository = "Sql"
$env:Clientes__ConnectionResolver = "AppBuilderMaster"
$env:ConnectionStrings__AppBuilderMaster = "<secret-local-fuera-de-git>"
$env:Clientes__WritesEnabled = "true"
```

Alternativa directa si se confirma por entorno local:

```powershell
$env:Clientes__Repository = "Sql"
$env:ConnectionStrings__ClientesModel = "<secret-local-fuera-de-git>"
$env:Clientes__WritesEnabled = "true"
```

Frontend local:

```powershell
$env:VITE_USE_BACKEND = "true"
$env:VITE_AUTH_MODE = "demo-session"
$env:VITE_API_BASE_URL = "http://localhost:5150"
```

## Plan de pruebas

Backend unit/API:

- 401 sin sesion.
- 403 sin `clientes.create`, `clientes.update` o `clientes.delete`.
- 403 con `Clientes:WritesEnabled=false`.
- Create con payload minimo valido.
- Create rechaza campos prohibidos o texto demasiado largo.
- Update rechaza clientes no MVP.
- Update rechaza cliente de otro broker sin revelar existencia.
- Delete rechaza clientes no MVP.
- Delete aplica baja logica y no borrado fisico.
- SQL builder usa parametros y whitelists.
- Errores publicos no incluyen PII ni SQL.

Frontend:

- Modo fixture/offline sigue usable cuando `VITE_USE_BACKEND=false`.
- Modo backend muestra acciones segun permisos.
- Dialogo create/edit no contiene documento, contacto, direccion ni banco.
- Delete pide confirmacion y refresca listado.
- Estados empty/error/loading sin PII.

SQL smoke local:

- Preparar referencia `ILMVP-CLI-SMOKE-*`.
- Verificar residuales previos `0` para esa referencia.
- Crear cliente con datos sinteticos.
- Leer listado y localizarlo por referencia o id tecnico, sin imprimir nombre real.
- Actualizar un campo permitido.
- Eliminar logicamente.
- Confirmar que no aparece en listado y que residuales activos son `0`.
- Si queda rastro logico `ILMVP-CLI-DELETED-*`, documentar conteo tecnico sin valores personales.
- No imprimir connection string ni filas reales.

## Criterios de aceptacion

- [ ] SDD enlazada desde planes operativos.
- [ ] API CRUD implementada solo bajo `Clientes:Repository=Sql`.
- [ ] `Clientes:WritesEnabled=false` bloquea create/update/delete.
- [ ] Permisos `clientes.create/update/delete` probados.
- [ ] Create/update/delete usan transacciones.
- [ ] Update/delete solo afectan filas `ILMVP-CLI-*`.
- [ ] No hay borrado fisico.
- [ ] El contrato no incluye documento, contacto, direccion ni banco.
- [ ] Smoke SQL local documentado con limpieza o `SKIPPED_ENV_MISSING`.
- [ ] Frontend conectado sin runtime de metadata AppBuilder.
- [ ] Secret scan limpio.

## Riesgos residuales

- El esquema puede exigir columnas obligatorias no confirmadas para `Identidad` o `IdentidadCliente`; la implementacion debe inspeccionar metadata local y fallar cerrado si no puede insertar de forma segura.
- `NombreCompleto`/`RazonSocial` pueden ser PII. El MVP local puede usarlos como alias visible por decision de datos reales locales, pero no deben salir de entorno local sin revision de privacidad.
- No hay semantica funcional de baja confirmada. Por eso `DELETE` es marcador MVP y no baja de negocio.
- Un cliente puede tener relaciones con polizas/recibos si se edita uno historico. Esta SDD lo prohibe limitando update/delete a filas MVP.
- El smoke local necesita configuracion de secretos fuera de Git; si no existe, debe quedar `SKIPPED_ENV_MISSING`.

## Definicion de hecho

- [ ] Implementacion backend y frontend revisada contra esta SDD.
- [ ] Tests dirigidos backend/frontend OK.
- [ ] Smoke SQL local OK o `SKIPPED_ENV_MISSING` justificado sin secretos.
- [ ] Evidencia QA actualizada.
- [ ] Plan operativo movido a la siguiente tarea.
- [ ] Riesgos residuales actualizados.
