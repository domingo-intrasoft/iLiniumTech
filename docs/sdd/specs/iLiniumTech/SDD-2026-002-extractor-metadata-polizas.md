# SDD: Extractor offline metadata polizas para scaffolding

## Metadata

- Spec ID: SDD-2026-002
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: feature
- Tamano SDD: M
- Estado SDD: spec-ready
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-13

## Contexto

El MVP de polizas ya identifica la metadata AppBuilder principal: aplicacion `2`, menu `10`, componente raiz `2824`, CRUD `2825`, `ComponentDataSource` `354` y datasource `146` sobre `Pantalla_Polizas`.

La siguiente fase necesita extraer configuracion real de solo lectura desde `IL_Maestro` y `AunnaTechADM`, sin guardar secretos ni dumps de BBDD.

Decision de arquitectura aplicable: iLiniumTech no sera un runtime dinamico tipo AppBuilder. Este extractor existe para migracion, trazabilidad, comparativa y scaffolding inicial. Su salida no debe ser una dependencia runtime del frontend ni del backend.

## Objetivo

Crear un extractor local/offline seguro que lea metadata `IAP_*` del componente de polizas y genere un JSON sanitizado y versionado para SDD, revision humana y scaffolding inicial de codigo iLiniumTech.

El resultado util debe acabar convertido en codigo fuente explicito: DTOs, columnas, etiquetas, filtros, pruebas o documentacion. No se aceptan pantallas ni queries que dependan de interpretar metadata AppBuilder en produccion.

## Fuera de alcance

- Leer datos reales de polizas.
- Ejecutar SQL dinamico de `QueryStatic`.
- Escribir en BBDD AppBuilder.
- Ejecutar workflows, expresiones complejas, REST o SOAP.
- Guardar connection strings, passwords, tokens o dumps.
- Renderizar UI desde metadata en runtime.
- Publicar un endpoint productivo que sirva metadata `IAP_*` como motor de la aplicacion.

## Contrato de datos

Entrada:

- `ILINIUMTECH__MASTER_CONNECTION`: connection string local, no versionada.
- `ILINIUMTECH__PROGRAM_DATABASE`: nombre de BBDD de programa, inicialmente `AunnaTechADM`.
- identificadores de aplicacion, menu, componente y datasource.

Salida:

- JSON sanitizado con aplicacion, version, menu, componentes, datasource, campos, lookups basicos, `searchConfigParams`, ordenes y dependencias.
- Informe de advertencias para campos o configuraciones no soportadas.
- Informe de scaffolding sugerido: columnas, filtros, labels y pruebas que deben revisarse antes de pasar a codigo.

El JSON no puede contener:

- connection strings;
- usuarios o passwords;
- datos personales;
- SQL no redaccionado si incluye valores sensibles.

## Reglas de negocio

- El extractor funciona en modo solo lectura.
- La salida conserva trazabilidad AppBuilder.
- Los campos desconocidos se reportan, no se ejecutan.
- `searchConfigParams` se parsea como estructura validada para analisis y scaffolding, no como string libre en runtime.
- Cualquier secreto ausente o placeholder debe fallar con mensaje claro y sin imprimir el valor.
- El frontend Vue no debe consumir este JSON para decidir controles, layouts o rutas en produccion.
- El backend API no debe consumir este JSON para construir SQL estructural en produccion.

## Criterios de aceptacion

- [ ] Existe comando local documentado para generar metadata sanitizada de polizas.
- [ ] El comando lee configuracion desde variables de entorno o secret store.
- [ ] La salida incluye referencias `2824`, `2825`, `354` y `146`.
- [ ] La salida incluye campos y configuracion de busqueda avanzada.
- [ ] La salida separa trazabilidad AppBuilder de sugerencias de scaffolding iLiniumTech.
- [ ] La documentacion del comando indica explicitamente que el JSON no es contrato runtime.
- [ ] No se imprimen connection strings ni passwords en consola o logs.
- [ ] Los errores de conexion no exponen secretos.
- [ ] Hay pruebas unitarias para mapeo, redaccion y campos obligatorios.
- [ ] Hay prueba de integracion opcional o marcada como skipped si no hay BBDD local.

## Impacto tecnico

Tooling:

- Nuevo proyecto, comando o script de extraccion fuera del camino runtime del producto.
- Contratos JSON de analisis/scaffolding.
- Validadores de metadata.

Documentacion:

- Actualizar `docs/MVP_POLIZAS_PLAN.md`.
- Actualizar `docs/DECISION_PRODUCTO_ARQUITECTURA.md` si cambia la decision.
- Actualizar spec SDD si cambia el alcance.

## Seguridad

- [ ] Secretos fuera de Git.
- [ ] Conexion con cuenta de minimo privilegio y solo lectura.
- [ ] Logs sin connection strings, SQL sensible ni datos personales.
- [ ] Salida JSON sanitizada.
- [ ] No se ejecuta `QueryStatic` ni SQL procedente de metadata AppBuilder.

## Plan de pruebas

- Unitarias: mapeo de `IAP_Component`, `IAP_DataSource`, campos y `searchConfigParams`.
- Integracion: conexion a BBDD de test si existe configuracion local.
- E2E/smoke: generar JSON y cargarlo en un validador.
- Revision de scaffolding: confirmar que el resultado se traduce a codigo estatico/API explicita antes de usarse.
- Seguridad: secret scan y revision de artefactos generados.
- Manual/UAT: comparar campos extraidos con captura y metadata documentada.

## Riesgos

- La metadata puede contener SQL, expresiones o configuracion dependiente de perfil.
- `searchConfigParams` puede variar por campo y requerir tolerancia.
- La cuenta de test puede tener permisos superiores a los necesarios.

## Work Items

- Crear issue GitHub: `SDD-2026-002 Extractor seguro metadata polizas`.
- Crear issue tecnico: validar formato JSON de salida.
- Crear issue tecnico: documentar ejecucion local sin secretos.
- Crear issue tecnico: definir proceso de revision para convertir scaffolding en codigo fuente.

## Definicion de hecho

- [ ] Criterios de aceptacion completados.
- [ ] Pruebas ejecutadas y documentadas.
- [ ] Gates de seguridad aplicables ejecutados.
- [ ] Documentacion actualizada.
