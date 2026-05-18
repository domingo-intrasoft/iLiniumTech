# Polizas - MVP de paridad visual con AppBuilder

Fecha: 2026-05-18

Estado: objetivo activo para el proximo MVP de `Polizas`.

## Objetivo

La pantalla `/polizas` de iLiniumTech debe acercarse mucho mas a la pantalla publicada de AppBuilder. El usuario ha aportado captura de `https://demo.aunnatech.es/portal/polizas` como referencia visual.

No se trata de hacer un renderer de AppBuilder. La pantalla debe seguir siendo codigo Vue mantenible, compilado y conectado al backend real de iLiniumTech.

## Checklist visual de referencia

- Barra demo superior naranja.
- Sidebar azul oscuro estrecho con iconos y etiquetas.
- Breadcrumb compacto: `Inicio / Polizas /`.
- Zona superior gris clara con contexto de aplicacion, usuario e iconos.
- Toolbar de iconos compactos, agrupados por separadores.
- Scope buttons oscuros a la derecha para flotas, colectivas y externas.
- Cabecera de grid con contador de resultados visible.
- Buscador principal ancho y botones pequenos a su derecha.
- Tabla densa sin cards, con lineas claras y cabecera compacta.
- Columnas: selector, acciones, `Cia.`, `Poliza`, `Certif.`, `N.º Documento`, `Cliente`, `Situacion`, `Ramo`, `Riesgo/Matric.`.
- Links de poliza subrayados.
- Badges verdes para `En Vigor`.
- Scroll horizontal para columnas largas.

## Reglas de fidelidad

- Si el backend no entrega logo real de compania, no se simula logo.
- Si el backend no entrega `Certif.` o `N.º Documento`, no se inventa valor.
- Si una accion no esta implementada de verdad, se muestra deshabilitada o se oculta.
- Si una ruta scope no tiene SDD/API/UAT, se mantiene bloqueada.
- Se prioriza densidad y estructura de AppBuilder sobre una UI tipo dashboard/landing.

## Primer incremento recomendado

ID de plan: `T-043-POL-APPBUILDER-VISUAL-SHELL`

Alcance:

- Reestructurar `PolizasView.vue` y estilos para aproximar toolbar, buscador y grid a AppBuilder.
- Reordenar `PolizasTable.vue` a columnas objetivo usando solo campos reales existentes.
- Crear evidencia QA visual.
- Mantener CRUD existente y permisos.
- No tocar backend ni servicios.

## Diferencias aceptables en el primer corte

- Logos reales de compania pueden quedar como texto/codigo si no hay dato autorizado.
- Algunas columnas pueden aparecer vacias o no renderizarse si no existen en contrato API.
- Iconos de toolbar pueden estar deshabilitados si no hay accion real.
- La barra superior puede aproximarse con los componentes actuales del shell sin copiar datos de sesion no disponibles.

## Evidencia requerida

- Tests frontend actualizados.
- Build frontend correcto.
- Secret scan limpio.
- Captura o descripcion smoke de `/polizas`.
- Lista de diferencias pendientes frente a la captura.
