using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas;

internal static class PolizasCatalogProvider
{
    public static PolizasCatalogs CreateDefault() =>
        new(
            TipoPoliza:
            [
                new("Vigor", "En vigor"),
                new("Pendiente", "Pendiente"),
                new("Anulada", "Anulada"),
                new("Vencida", "Vencida")
            ],
            Compania:
            [
                new("Compania demo", "Compania demo")
            ],
            Ramo:
            [
                new("Autos", "Autos"),
                new("Hogar", "Hogar"),
                new("Salud", "Salud"),
                new("Vida", "Vida"),
                new("Empresa", "Empresa")
            ],
            Oficina:
            [
                new("Las Palmas", "Las Palmas"),
                new("Tenerife", "Tenerife")
            ],
            Division:
            [
                new("Particulares", "Particulares"),
                new("Empresas", "Empresas")
            ],
            Colaborador1:
            [
                new("COL-01", "Colaborador 1"),
                new("COL-02", "Colaborador 2")
            ],
            Administrativo:
            [
                new("ADM-01", "Administrativo 1"),
                new("ADM-02", "Administrativo 2")
            ],
            Comercial:
            [
                new("COM-01", "Comercial 1"),
                new("COM-02", "Comercial 2")
            ],
            Siniestros:
            [
                new("SIN-01", "Siniestros 1"),
                new("SIN-02", "Siniestros 2")
            ],
            Gestor:
            [
                new("GES-01", "Gestor 1"),
                new("GES-02", "Gestor 2")
            ],
            CanalCobro:
            [
                new("Banco", "Banco"),
                new("Tarjeta", "Tarjeta")
            ],
            FraccionPago:
            [
                new("Anual", "Anual"),
                new("Semestral", "Semestral"),
                new("Mensual", "Mensual")
            ],
            Ccaa:
            [
                new("Canarias", "Canarias"),
                new("Madrid", "Madrid")
            ],
            Sexo:
            [
                new("M", "Mujer"),
                new("H", "Hombre")
            ],
            EstadoCivil:
            [
                new("Soltero/a", "Soltero/a"),
                new("Casado/a", "Casado/a")
            ],
            RegimenLaboral:
            [
                new("Cuenta ajena", "Cuenta ajena"),
                new("Autonomo", "Autonomo")
            ],
            Profesion:
            [
                new("Administracion", "Administracion"),
                new("Ingenieria", "Ingenieria")
            ]);
}
