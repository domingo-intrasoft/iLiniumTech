# SDD: Agenda CRUD BBDD local

## Metadata

- Spec ID: SDD-2026-015
- Work Item: objetivo humano 2026-05-19 "Pasa ya todo a vertical de CRUD real contra BBDD local"
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready local
- Responsable funcional: pendiente UAT
- Responsable tecnico: IA/Codex
- Fecha: 2026-05-19

## Contexto

El objetivo activo cambia de paginas fixture/read-only a verticales CRUD reales contra BBDD local. `Polizas` sigue siendo la referencia de seguridad: API explicita, permisos, flag de escritura, SQL parametrizado, transacciones, contexto broker y evidencia.

Agenda es la primera pagina candidata fuera de Polizas porque AppBuilder expone tabla `Agenda` y vista `vw_Agenda` con un alcance acotado. Clientes, Recibos, Siniestros, Suplementos y Propuestas quedan en cola por mayor riesgo de PII, finanzas, triggers o origen funcional no confirmado.

## Objetivo

Entregar una primera vertical Agenda capaz de:

- listar eventos desde API explicita;
- crear eventos MVP reales en tabla `dbo.Agenda` cuando `Agenda:Repository=Sql` y `Agenda:WritesEnabled=true`;
- actualizar solo campos acotados;
- eliminar de forma logica solo filas creadas por iLiniumTech MVP;
- exponer acciones visibles en frontend cuando `VITE_USE_BACKEND=true`;
- mantener fixture local read-only cuando el backend no este habilitado.

## Fuera de alcance

- Calendario dinamico tipo FullCalendar.
- Detalle completo de evento.
- `Descripcion` libre de usuario en contrato publico.
- `IdentidadId`, participantes, direcciones, telefonos, emails o datos sensibles.
- Navegacion generica por `IdObjeto`.
- Borrado fisico de eventos historicos.
- Escritura de eventos creados por AppBuilder u otros sistemas.
- Triggers de siniestros o workflows heredados.

## Evidencia AppBuilder

Detectado:

- `DbSet<Agendum> Agenda` en `ModeloDbContext`.
- Tabla candidata `dbo.Agenda`.
- Vista `dbo.vw_Agenda` sin clave para lectura/calendario.
- Campos detectados: `Id`, `IdOld`, `Asunto`, `Descripcion`, `F_Inicio`, `F_Fin`, `Hora_Inicio`, `Hora_Fin`, `IdPrioridad`, `IdObjeto`, `IdentidadId`, `BrokerIntegracionId`, `FCR`, `FUM`, `UCR`, `UUM`.
- FK obligatoria `BrokerIntegracionId`.

Inferido:

- `Agenda.Id` es la clave tecnica de recurso para iLiniumTech.
- `IdOld` se usa como referencia externa y se reserva con prefijo `ILMVP-AGE-` para filas creadas por iLiniumTech.
- La baja segura del MVP se modela como baja logica modificando `IdOld` a `ILMVP-AGE-DELETED-<Id>`.

## Contrato de datos

Lectura:

- `GET /api/agenda`
- `GET /api/agenda/events`
- `GET /api/agenda/catalogs`

Escritura:

- `POST /api/agenda`
- `PUT /api/agenda/{id}`
- `DELETE /api/agenda/{id}`

Campos publicos:

- `id`
- `referencia`
- `titulo`
- `inicio`
- `fin`
- `estado`
- `prioridad`
- `origen`
- `objetoRelacionadoTipo`

Payload create:

- `referencia`: requerido, debe empezar por `ILMVP-AGE-`.
- `titulo`: requerido, maximo 100.
- `inicio`: requerido.
- `fin`: opcional, no menor que `inicio`.
- `prioridad`: opcional.
- `objetoRelacionadoTipo`: opcional, texto corto.

Payload update:

- `titulo`, `inicio`, `fin`, `prioridad`, `objetoRelacionadoTipo`.
- Debe incluir al menos un campo editable.

## Reglas de negocio

- Solo se escriben filas MVP con `IdOld LIKE 'ILMVP-AGE-%'`.
- No se actualizan ni eliminan filas que ya esten marcadas como `ILMVP-AGE-DELETED-%`.
- Toda escritura SQL usa `WHERE Id = @id AND BrokerIntegracionId = @brokerId`.
- `BrokerIntegracionId` sale del contexto de ejecucion autenticado/cabecera MVP local, no del payload.
- `UCR`/`UUM` salen del contexto de usuario si existe.
- `Descripcion` queda fija y sanitizada para filas MVP locales.
- `DELETE` es baja logica MVP, no borrado fisico.

## Criterios de aceptacion

- [x] Dado un usuario con permiso `agenda.read`, cuando consulta Agenda, entonces recibe un contrato minimizado sin `Descripcion`, `IdentidadId` ni `IdObjeto`.
- [x] Dado `Agenda:WritesEnabled=false`, cuando intenta crear, actualizar o eliminar, entonces recibe 403.
- [x] Dado un usuario con permisos `agenda.create/update/delete` y writes habilitado, cuando opera una fila MVP, entonces la API permite create/update/delete logico.
- [x] Dado un payload con referencia sin prefijo `ILMVP-AGE-`, cuando intenta crear, entonces recibe error de validacion.
- [x] No se guardan secretos ni datos sensibles en Git.

## Seguridad

- Permisos nuevos: `agenda.create`, `agenda.update`, `agenda.delete`.
- `Agenda:WritesEnabled=false` por defecto.
- Fuera de Development, las escrituras requieren opt-in demo igual que Polizas.
- SQL parametrizado sin concatenar entradas de usuario.
- Sort y filtros por whitelist.
- Sin secretos ni connection strings versionados.
- Sin PII directa en contrato publico.

## Configuracion local

Opciones soportadas:

```powershell
$env:Agenda__Repository = "Sql"
$env:Agenda__ConnectionResolver = "AppBuilderMaster"
$env:ConnectionStrings__AppBuilderMaster = "<secret-local-fuera-de-git>"
$env:Agenda__WritesEnabled = "true"
```

Alternativa directa a modelo local:

```powershell
$env:Agenda__Repository = "Sql"
$env:ConnectionStrings__AgendaModel = "<secret-local-fuera-de-git>"
$env:Agenda__WritesEnabled = "true"
```

El frontend usa:

```powershell
$env:VITE_USE_BACKEND = "true"
$env:VITE_AUTH_MODE = "demo-session"
$env:VITE_API_BASE_URL = "http://localhost:5146"
```

## Plan de pruebas

Unitarias/API:

- 401 sin autenticacion.
- 403 sin permiso exacto.
- 403 si `Agenda:WritesEnabled` no esta activo.
- Create/update/delete con permisos y flag.
- Validacion de prefijo `ILMVP-AGE-`.
- Sort/filtros maliciosos rechazados.

Frontend:

- Fixture read-only sigue funcionando con `VITE_USE_BACKEND=false`.
- Modo backend habilita acciones CRUD.
- No se exponen marcadores de metadata AppBuilder, secretos ni PII.

SQL local:

- Smoke create-read-update-delete con fila `ILMVP-AGE-*`.
- Verificar `ResidualBefore=0` y `ResidualAfter=0` para la referencia usada.
- Verificar que otro broker no ve ni modifica la fila.
- No imprimir connection strings ni datos reales.

## Riesgos

- `Asunto` puede contener PII en datos historicos; no debe enriquecerse con busquedas libres amplias sin UAT.
- `IdObjeto` puede abrir navegacion generica AppBuilder; se mantiene como texto corto no navegable.
- `vw_Agenda.Start/End` son texto; el primer SQL usa tabla `Agenda` para evitar parseo ambiguo.
- No hay campo de baja funcional confirmado; el MVP solo marca filas propias.

## Definicion de hecho

- [x] API explicita de CRUD Agenda.
- [x] Permisos y flag de escritura.
- [x] SQL parametrizado con filtro broker.
- [x] Transacciones en create/update/delete.
- [x] Frontend conectado a API en modo backend y fixture en modo local.
- [x] Tests dirigidos backend/frontend.
- [ ] Smoke contra BBDD local con secretos configurados fuera de Git.
- [ ] UAT valida campos definitivos y semantica de baja.
