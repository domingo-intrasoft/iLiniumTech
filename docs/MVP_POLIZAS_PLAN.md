# MVP Polizas - Plan tecnico

Fecha: 2026-05-13

## Objetivo

Construir el primer MVP de iLiniumTech alrededor del componente de polizas configurado en AppBuilder, sin migrar todo el generador y sin repetir sus riesgos de seguridad.

## Metadata real localizada

Entorno consultado en solo lectura:

- BBDD maestra indicada por el usuario: `IL_Maestro`.
- BBDD de programa: `AunnaTechADM`.

No se ha guardado ninguna cadena de conexion ni password en este repositorio.

Aplicacion localizada:

- `IAP_Application.Id`: `2`.
- Nombre: `Aunna Tech`.
- Version: `1`.

Entrada de menu principal:

- `IAP_Menu.Id`: `10`.
- Titulo: `Polizas`.
- `ComponentId`: `2824`.
- Activo: `1`.

Componente candidato MVP:

- Raiz: `IAP_Component.Id = 2824`.
- Nombre: `Busqueda Polizas`.
- Tipo: `componentsapp-comp`.
- Categoria: `compcat-seacrh`.
- CRUD hijo: `IAP_Component.Id = 2825`.
- Nombre CRUD: `CrudPoliza`.
- Tipo CRUD: `tipocontrol-crudtbl`.

Datasource principal:

- `IAP_ComponentDataSource.Id`: `354`.
- `ComponentId`: `2825`.
- `DataSourceId`: `146`.
- Nombre enlace: `Dat_PantallaPolizas`.
- `IAP_DataSource.Name`: `Pantalla_Polizas`.
- Tipo: `datasourcetype-db`.
- BBDD objetivo: `tipobd-MO`.
- Objeto modelo: `Pantalla_Polizas`.

Campos relevantes detectados desde `IAP_ComponentDataSourceFieldConfiguration`:

- `Poliza`.
- `Aplicacion`.
- `IdTipoPoliza`.
- `NumDocumento`.
- `IdSituacion`.
- `IdRamo`.
- `Riesgo`.
- `F_Efecto`.
- `F_Vencimiento`.
- `F_Anulacion`.
- `IdMotivoAnulacion`.
- `Cia`.
- `PAnualCartera`.
- `NombreCompleto`.
- `AlertaInformativa`.
- `AlertaExclamativa`.
- `AlertaRstrictiva`.

## Corte MVP

El MVP actual es de solo lectura y se limita a:

- contrato de metadata de polizas;
- listado paginado;
- detalle por id;
- validacion de ordenacion por whitelist;
- UI inicial de tabla/filtros con datos anonimizados;
- pruebas unitarias e integracion basicas;
- gates de seguridad activos.

Fuera de alcance por ahora:

- escrituras;
- alta, baja, duplicar, suspender, revigorizar o reemplazar poliza;
- workflows;
- expresiones complejas;
- Google Wallet;
- llamadas REST/SOAP;
- permisos finos por oficina, gestor o perfil;
- lectura directa de `Pantalla_Polizas` en runtime.

## Proyectos creados

Backend:

```text
iLiniumTech.Backend/
  iLiniumTech.Backend.slnx
  src/
    iLiniumTech.Backend.Api
    iLiniumTech.Backend.Application
    iLiniumTech.Backend.Domain
    iLiniumTech.Backend.Infrastructure
  tests/
    iLiniumTech.Backend.Tests
```

Frontend:

```text
iLiniumTech.Frontend/
  src/
    features/polizas
    router
    services
    assets/styles
```

## API MVP

Endpoints:

```text
GET /health
GET /api/polizas/metadata
GET /api/polizas
GET /api/polizas/{id}
```

Seguridad actual:

- `/health` es anonimo.
- `/api/polizas/*` requiere cabecera `X-ILiniumTech-Api-Key`.
- La clave real debe configurarse en `ApiSecurity__ApiKey`.
- El placeholder `__SET_IN_ENVIRONMENT__` no autentica.
- CORS usa `Cors:AllowedOrigins`, sin `AllowAnyOrigin`.

## Contrato de metadata

La metadata expone referencias AppBuilder necesarias para trazabilidad:

```json
{
  "resource": "polizas",
  "version": 1,
  "appBuilder": {
    "applicationId": 2,
    "applicationVersion": 1,
    "menuId": 10,
    "rootComponentId": 2824,
    "crudComponentId": 2825,
    "componentDataSourceId": 354,
    "dataSourceId": 146,
    "dataSourceName": "Pantalla_Polizas",
    "modelObject": "Pantalla_Polizas"
  }
}
```

## Siguiente paso tecnico

Crear un extractor seguro de solo lectura:

1. Leer la conexion desde variable de entorno local o secret store.
2. No imprimir la cadena.
3. Consultar `AunnaTechADM` para metadata `IAP_*`.
4. Consultar BBDD modelo solo con whitelist.
5. Generar un JSON sanitizado de `Polizas`.
6. Sustituir fixture in-memory por repositorio SQL parametrizado.
7. Anadir integration tests contra BBDD de test o contenedor.

## Riesgos

- `Pantalla_Polizas` tiene restricciones por `SESSION_CONTEXT`, perfil, gestores y oficinas.
- Los datos de polizas pueden incluir datos personales, financieros y de riesgo.
- El CRUD real arrastra muchos botones de accion y workflows.
- El campo `QueryStatic` de AppBuilder contiene SQL generado dinamicamente; no debe ejecutarse sin validacion.
- La UI real de AppBuilder usa templates de header, menus y botones que se han recortado para MVP.

## Criterio de exito de fase

- Proyecto backend y frontend creados.
- Contrato de polizas versionado.
- Metadata real identificada.
- Build y tests pasan.
- Escaneo de secretos limpio.
- Auditoria de dependencias sin high/critical.
- CORS audit limpio.
