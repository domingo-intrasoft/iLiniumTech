# Componente Clientes: resumen e identidad

Fecha: 2026-05-16

Estado: `bloqueado externo para desarrollo final`.

## Rol del componente

Panel candidato de cabecera o primera pestana de ficha de cliente. Debe mostrar un resumen minimizado de identidad, clasificacion comercial y metricas agregadas si estan autorizadas.

## Evidencia AppBuilder

Fuentes:

- `C:\Desarrollo\AppBuilder\src\backend\Infraestructura\Datos\AppBuilder.Infraestructura.DataAccess\Entity Framework\Modelo\ModeloDbContext.cs`
- `C:\Desarrollo\AppBuilder\src\frontend\shared\src\common\domain\constantes\NombreTablasConst.ts`

Entidades/vistas detectadas:

- `Identidad`
- `IdentidadCliente`
- `IdentidadClienteAcuerdo`
- `vw_ClienteInfo`
- `vw_ClienteDato`
- `vw_Cliente_DatosAdicionales`
- `vw_ClienteAcuerdo`
- `vw_Cliente_ColaboradorLider`

Campos detectados:

- `Identidad`: nombre, apellidos, razon social, nombre completo, tipo/documento, actividad, idioma, profesion, sexo, estado civil, fechas personales.
- `IdentidadCliente`: `ClienteId`, canal de cobro, gestor, comercial.
- `IdentidadClienteAcuerdo`: acuerdo de reparto, fecha alta/baja.
- `vw_ClienteInfo`: prima neta/total y comision de periodos actual/anterior.
- `vw_ClienteDato`: tipo y valor de dato adicional.
- `vw_Cliente_DatosAdicionales`: descripcion.
- `vw_Cliente_ColaboradorLider`: email.
- `vw_ClienteAcuerdo`: colaborador y descripcion.

## Evidencia ausente

- No consta que todas estas vistas se muestren en la misma ficha.
- No consta orden visual, etiquetas, tabs ni permisos por campo.
- No consta si `vw_ClienteInfo` debe estar en resumen o en un panel analitico separado.

## Propuesta de UI estatica

Componente candidato:

- `ClienteResumenPanel.vue`

Secciones candidatas:

- cabecera: nombre/razon social minimizado e identificador interno;
- clasificacion: gestor, comercial, canal de cobro;
- metricas: primas/comisiones si producto confirma y permisos lo permiten;
- datos adicionales: solo valores clasificados como no sensibles;
- acuerdos: resumen de acuerdo activo si se confirma.

No mostrar por defecto:

- documento completo;
- fecha de nacimiento;
- caducidad de documento;
- email;
- telefono;
- direccion;
- datos bancarios;
- observaciones libres.

## API candidata

- `GET /api/clientes/{clienteId}`

DTO minimo candidato:

- `clienteId`
- `nombreMostrable`
- `tipoCliente`
- `gestor`
- `comercial`
- `canalCobro`
- `metricasResumen` opcional y autorizada
- `datosAdicionales` solo si se aprueba

## Permisos candidatos

- `clientes.detail`
- `clientes.pii.read` para datos personales ampliados, si producto lo aprueba.
- `clientes.financialSummary.read` para primas/comisiones, si se considera informacion sensible.

## Pruebas obligatorias

- no mostrar PII sin permiso;
- 403/404 generico para cliente de otro broker;
- DTO sin documento completo por defecto;
- UI con estado "sin permisos para datos ampliados";
- logs sin nombre/documento/email.

## Riesgos

- Concentracion de PII.
- Mezcla de datos de identidad base con rol cliente.
- Email de colaborador/lider puede ser dato personal corporativo.
- Metricas economicas pueden requerir permiso propio.

## Bloqueo

Falta decision de minimizacion PII y alcance de ficha.
