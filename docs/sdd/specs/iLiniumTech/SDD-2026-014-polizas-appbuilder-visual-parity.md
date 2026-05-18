# SDD: Polizas visual parity AppBuilder

## Metadata

- Spec ID: SDD-2026-014
- Work Item: pendiente de crear
- Aplicacion: iLiniumTech
- Tipo: frontend/ux/product
- Tamano SDD: M
- Estado SDD: draft-active
- Responsable funcional: Intrasoft
- Responsable tecnico: iLiniumTech
- Fecha: 2026-05-18

## Contexto

El usuario ha aportado una captura de AppBuilder publicada en `https://demo.aunnatech.es/portal/polizas` y ha corregido la direccion del MVP: la pantalla de `Polizas` de iLiniumTech debe ser visualmente mucho mas fiel al producto anterior publicado.

El objetivo no es volver a AppBuilder ni renderizar metadata. La pantalla debe seguir siendo Vue/TypeScript estatico, con backend API real y CRUD real ya conectado a BBDD local de pruebas. La paridad visual se limita a layout, densidad, jerarquia, toolbar, grid, navegacion y estados visibles.

## Principio no negociable

- No usar metadata AppBuilder como runtime.
- No simular datos, logos, permisos, importes o acciones.
- No introducir APIs nuevas si no son necesarias para la paridad visual.
- No inventar campos: si la API no entrega un dato real, la UI debe ocultarlo, mostrar valor neutro aprobado o marcarlo como pendiente tecnico, nunca fabricar informacion.
- El CRUD real de `Polizas` permanece gobernado por `SDD-2026-007`.

## Actualizacion de alcance - 2026-05-18

Tras revisar la captura comparativa, la paridad ya no puede cerrarse solo con layout. El usuario identifica como brecha funcional que faltan `N. Documento`, nombre de cliente, `Ramo` descriptivo y `Riesgo/Matric.`.

Decision:

- ampliar el contrato de listado de `Polizas` para transportar esos campos desde la BBDD local de pruebas;
- documentar la logica AppBuilder de lookups en `docs/appbuilder/pages/polizas/data-contract-analysis.md`;
- mantener la pantalla como Vue/TypeScript estatico y API .NET explicita;
- sanitizar rutas derivadas no activas, especialmente `Autos Particulares`, para no exponer campos sensibles fuera de su SDD.

## Objetivo del MVP visual

Conseguir que `/polizas` se parezca de forma reconocible a la pantalla AppBuilder publicada:

- menu lateral oscuro, compacto, con iconos y etiquetas cortas;
- cabecera superior con aviso demo, breadcrumb `Inicio / Polizas /`, contexto de aplicacion, usuario e iconos de modulo;
- toolbar horizontal densa de iconos agrupados;
- accesos de scope a la derecha: `Polizas de flota`, `Polizas colectivas`, `Polizas Externas` si siguen bloqueados;
- area de busqueda compacta con input principal y botones pequenos;
- tabla densa tipo grid con cabecera fija/limpia, iconos de orden/filtro y filas de altura reducida;
- columnas objetivo: selector, acciones, `Cia.`, `Poliza`, `Certif.`, `N.º Documento`, `Cliente`, `Situacion`, `Ramo`, `Riesgo/Matric.`;
- estados tipo badge, especialmente `En Vigor` verde;
- horizontal scroll cuando el ancho no permite todas las columnas;
- sin cards grandes, sin hero, sin composicion editorial.

## Diferencias detectadas en la captura

AppBuilder muestra:

- una barra superior naranja de entorno demo;
- sidebar azul oscuro de ancho estrecho con icono y texto por modulo;
- breadcrumb pequeno sobre fondo gris claro;
- barra superior de contexto con pill rojo `AunnaTech | Portal (DEMO)`, iconos circulares y usuario;
- botones de toolbar con iconos cuadrados compactos y grupos separados por bordes;
- tabs/scope buttons azules a la derecha;
- contador de resultados junto al titulo de tab, por ejemplo `(80921)`;
- buscador principal dentro de la superficie de grid;
- tabla muy densa, sin tarjetas, con lineas horizontales claras;
- primera columna de seleccion, segunda columna de menu contextual, tercera columna con logo/compania;
- links de poliza subrayados y pequenos indicadores de enlace;
- `Situacion` como badge verde redondeado;
- `Riesgo/Matric.` con texto largo truncable y scroll horizontal.

## Fuera de alcance

- Copiar assets o logos reales si no hay fuente autorizada en el producto.
- Simular logos de compania con imagenes falsas.
- Mostrar documento, nombre completo, matricula, riesgo completo o PII que la API no haya aprobado.
- Activar acciones de toolbar que no tengan implementacion real.
- Cambiar comportamiento de escritura CRUD sin SDD/QA.
- Crear endpoints genericos tipo pantalla/datasource.
- Reactivar Autos Particulares.

## Estrategia por pasos pequenos

### Paso 1 - Inventario visual y grid shell

- Crear inventario visual versionado en `docs/appbuilder/pages/polizas/visual-parity-mvp.md`.
- Ajustar `/polizas` para adoptar estructura densa: toolbar compacta, buscador en grid, contador visible y tabla sin cards grandes.
- Mantener datos actuales reales/fixture segun modo existente.
- No tocar backend.

### Paso 2 - Tabla AppBuilder-like

- Reordenar columnas para aproximar la captura.
- Convertir estado/situacion en badge verde cuando corresponda.
- Hacer links de poliza con estilo compacto.
- Preparar columna `Cia.` usando solo dato real disponible; si no hay logo autorizado, mostrar texto/codigo real o celda neutra.
- Mantener acciones no implementadas deshabilitadas.

### Paso 3 - Toolbar y scopes

- Reproducir grupos de iconos compactos.
- Mostrar scopes `Polizas de flota`, `Polizas colectivas` y `Polizas Externas` como botones visibles, manteniendo bloqueo si no hay SDD/API/UAT.
- Añadir tests de accesibilidad para botones bloqueados.

### Paso 4 - Smoke visual

- Ejecutar screenshot o smoke browser en `/polizas` con backend demo/local.
- Comparar checklist visual contra esta SDD.
- Documentar diferencias pendientes sin inventar datos.

## Archivos candidatos

Permitidos para primer incremento:

- `iLiniumTech.Frontend/src/features/polizas/PolizasView.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasFilters.vue`
- `iLiniumTech.Frontend/src/features/polizas/PolizasView.test.ts`
- `iLiniumTech.Frontend/src/features/polizas/PolizasTable.test.ts`
- `iLiniumTech.Frontend/src/assets/styles/main.scss`
- `docs/appbuilder/pages/polizas/visual-parity-mvp.md`
- `docs/qa/polizas-visual-parity-evidence.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`

Permitidos para el incremento de paridad de datos:

- `iLiniumTech.Backend/src/iLiniumTech.Backend.Domain/Polizas/PolizaListItem.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/Sql/PolizasSqlQueryBuilder.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/SqlPolizasRepository.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Infrastructure/Polizas/InMemoryPolizasRepository.cs`
- `iLiniumTech.Backend/src/iLiniumTech.Backend.Api/Program.cs`
- `iLiniumTech.Backend/tests/iLiniumTech.Backend.Tests/Polizas/**`
- `iLiniumTech.Frontend/src/features/polizas/**`
- `docs/appbuilder/pages/polizas/data-contract-analysis.md`
- `docs/appbuilder/pages/polizas/components/listado-grid.md`
- `docs/qa/polizas-visual-parity-evidence.md`
- `docs/PLAN_EJECUCION_CONTINUA_IA.md`

Prohibidos sin nueva decision:

- `iLiniumTech.Frontend/src/services/**`
- `iLiniumTech.Frontend/src/router/**`
- `docs/sdd/specs/iLiniumTech/SDD-2026-007-polizas-crud-bbdd.md`
- `.env*`, appsettings locales, dumps, capturas sensibles o credenciales.

## Criterios de aceptacion

- `/polizas` se reconoce visualmente como la pantalla AppBuilder de referencia.
- El listado muestra `documento`, `clienteNombre`, `ramo` descriptivo y `riesgo` cuando la API los entrega.
- `clienteNombre` no se sustituye por codigo de cliente salvo fallback tecnico por dato vacio.
- `ramo` se resuelve como descripcion de catalogo o fallback controlado al id.
- No hay datos simulados nuevos.
- Las acciones sin backend real siguen bloqueadas o deshabilitadas.
- CRUD real existente no se rompe.
- Los tests unitarios frontend pasan.
- `npm run format`, `npm run lint`, `npm run test:unit` y `npm run build` pasan.
- Secret scan limpio.
- Evidencia visual/documental actualizada.

## Seguridad

- No guardar la captura original en Git si contiene datos personales o contexto sensible.
- No copiar connection strings, tokens, usuarios reales, datos personales ni identificadores completos desde AppBuilder.
- No usar logos reales de companias si no hay fuente autorizada o dato real entregado por API.
- No mostrar campos sensibles solo para parecerse mas a la captura.
- No activar acciones que puedan modificar datos sin permisos y backend real.
- Mantener errores y estados bloqueados sin filtrar detalles internos.
- Mantener `DemoSession` como compatibilidad local/demo, no como seguridad productiva.

## Plan de pruebas

Frontend:

- `npx vitest run src/features/polizas/PolizasView.test.ts src/features/polizas/PolizasTable.test.ts`
- `npm run format`
- `npm run lint`
- `npm run test:unit`
- `npm run build`

Seguridad/documentacion:

- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\quality\Test-DocumentationBaseline.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File tools\security\Invoke-SecretScan.ps1`
- `git diff --check`

Smoke visual:

- Abrir `/polizas` en modo local/demo.
- Verificar toolbar, buscador, grid denso, columnas objetivo, badges y acciones bloqueadas.
- Documentar diferencias pendientes en `docs/qa/polizas-visual-parity-evidence.md`.

## Riesgos

- Confundir paridad visual con copia de runtime AppBuilder.
- Introducir datos falsos para llenar columnas.
- Mostrar PII o campos no aprobados para parecerse mas a la captura.
- Romper CRUD real al rediseñar la tabla.
- Convertir toolbar en botones decorativos sin estado accesible.

## Definicion de hecho

- SDD y guia visual enlazadas desde el plan continuo.
- Primer corte visual implementado sin tocar backend ni servicios.
- CRUD real existente sigue operativo.
- Tests frontend y build pasan.
- Secret scan limpio.
- Evidencia QA visual documenta lo conseguido y las diferencias pendientes.
