using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas;

public sealed class SqlPolizasRepository : IPolizasRepository
{
    private readonly string _connectionString;

    public SqlPolizasRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, PolizasSort sort, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);

        var baseQuery = @"
            FROM dbo.Pantalla_Polizas p
            LEFT JOIN dbo.vw_Cat_Ramos r ON p.IdRamo = r.Id
            WHERE 1=1";

        var whereClause = new System.Text.StringBuilder();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.Numero))
        {
            whereClause.Append(" AND p.Poliza LIKE @Numero");
            parameters.Add("Numero", $"%{request.Numero}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Cliente))
        {
            whereClause.Append(" AND p.NombreCompleto LIKE @Cliente");
            parameters.Add("Cliente", $"%{request.Cliente}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            whereClause.Append(" AND p.IdSituacion = @Estado");
            string stateCode = request.Estado.ToLowerInvariant() switch
            {
                "vigor" => "situacionpoliza-EV",
                "anulada" => "situacionpoliza-AN",
                _ => request.Estado
            };
            parameters.Add("Estado", stateCode);
        }

        if (request.FechaEfectoDesde.HasValue)
        {
            whereClause.Append(" AND p.F_Efecto >= @FechaEfectoDesde");
            parameters.Add("FechaEfectoDesde", request.FechaEfectoDesde.Value.ToDateTime(TimeOnly.MinValue));
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            whereClause.Append(" AND p.F_Efecto <= @FechaEfectoHasta");
            parameters.Add("FechaEfectoHasta", request.FechaEfectoHasta.Value.ToDateTime(TimeOnly.MaxValue));
        }

        if (!string.IsNullOrWhiteSpace(request.Compania))
        {
            whereClause.Append(" AND p.Cia LIKE @Compania");
            parameters.Add("Compania", $"%{request.Compania}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Ramo))
        {
            whereClause.Append(" AND p.IdRamo = @Ramo");
            parameters.Add("Ramo", request.Ramo);
        }

        if (!string.IsNullOrWhiteSpace(request.Documento))
        {
            whereClause.Append(" AND p.NumDocumento = @Documento");
            parameters.Add("Documento", request.Documento);
        }

        // Get total count (safe SQL Server format, CTE not used in subquery to prevent syntax error)
        var countSql = $"SELECT COUNT(*) {baseQuery} {whereClause}";
        var total = await connection.ExecuteScalarAsync<int>(new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        // Sorting (whitelist to avoid SQL injection)
        string sortField = sort.Field.ToLowerInvariant() switch
        {
            "numero" => "p.Poliza",
            "aplicacion" or "ramo" => "COALESCE(r.Descripcion, p.IdRamo)",
            "estado" => "p.IdSituacion",
            "compania" => "p.Cia",
            "cliente" => "p.NombreCompleto",
            "fechaefecto" => "p.F_Efecto",
            "fechavencimiento" => "p.F_Vencimiento",
            "primaanual" => "p.PAnualProduccion",
            _ => "p.F_Efecto"
        };
        string sortDirection = sort.Descending ? "DESC" : "ASC";

        // Dynamic paginated select query
        var itemsSql = $@"
            SELECT 
                p.Id, 
                p.Poliza AS Numero, 
                COALESCE(r.Descripcion, p.IdRamo) AS Aplicacion, 
                p.IdSituacion AS EstadoId,
                p.F_Efecto AS FechaEfecto,
                p.F_Vencimiento AS FechaVencimiento,
                p.PAnualProduccion AS PrimaAnual,
                p.Cia AS Compania,
                p.NombreCompleto AS ClienteNombre,
                p.NumDocumento AS ClienteDocumento,
                'CLI-' + CAST(p.ClienteId AS VARCHAR) AS ClienteId
            {baseQuery} {whereClause}
            ORDER BY {sortField} {sortDirection}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        parameters.Add("Offset", (request.Page - 1) * request.PageSize);
        parameters.Add("PageSize", request.PageSize);

        var itemsQuery = await connection.QueryAsync<dynamic>(itemsSql, parameters);

        var listItems = itemsQuery.Select(row => new PolizaListItem(
            Id: row.Id.ToString(),
            Numero: (string)(row.Numero ?? string.Empty),
            Aplicacion: (string)(row.Aplicacion ?? "Desconocido"),
            Estado: MapEstado((string)(row.EstadoId ?? string.Empty)),
            Ramo: (string)(row.Aplicacion ?? "Desconocido"),
            ClienteId: (string)(row.ClienteId ?? string.Empty),
            ClienteNombre: (string)(row.ClienteNombre ?? "Cliente Anonimo"),
            Compania: (string)(row.Compania ?? "Compania Demo"),
            FechaEfecto: row.FechaEfecto != null ? DateOnly.FromDateTime((DateTime)row.FechaEfecto) : DateOnly.MinValue,
            FechaVencimiento: row.FechaVencimiento != null ? DateOnly.FromDateTime((DateTime)row.FechaVencimiento) : DateOnly.MinValue,
            PrimaAnual: (decimal)(row.PrimaAnual ?? 0m),
            Moneda: "EUR"
        )).ToArray();

        return new PagedResult<PolizaListItem>(listItems, request.Page, request.PageSize, total);
    }

    public async Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (!int.TryParse(id, out var intId))
        {
            return null;
        }

        using var connection = new SqlConnection(_connectionString);

        const string sql = @"
            SELECT 
                p.Id, 
                p.Poliza AS Numero, 
                COALESCE(r.Descripcion, p.IdRamo) AS Aplicacion, 
                p.IdSituacion AS EstadoId,
                p.F_Efecto AS FechaEfecto,
                p.F_Vencimiento AS FechaVencimiento,
                p.PAnualProduccion AS PrimaAnual,
                p.Cia AS Compania,
                p.NombreCompleto AS ClienteNombre,
                p.NumDocumento AS ClienteDocumento,
                p.Riesgo AS RiesgoDesc,
                p.ModalidadDesc AS ProductoNombre,
                'CLI-' + CAST(p.ClienteId AS VARCHAR) AS ClienteId
            FROM dbo.Pantalla_Polizas p
            LEFT JOIN dbo.vw_Cat_Ramos r ON p.IdRamo = r.Id
            WHERE p.Id = @Id";

        var row = await connection.QueryFirstOrDefaultAsync<dynamic>(sql, new { Id = intId });
        if (row is null)
        {
            return null;
        }

        string estadoId = row.EstadoId ?? string.Empty;
        string numero = row.Numero ?? string.Empty;
        string aplicacion = row.Aplicacion ?? "Desconocido";
        string compania = row.Compania ?? "Compania Demo";
        string clienteId = row.ClienteId ?? string.Empty;
        string clienteNombre = row.ClienteNombre ?? "Cliente Anonimo";
        string clienteDocumento = row.ClienteDocumento;
        string productoNombre = row.ProductoNombre ?? aplicacion;
        string riesgoDesc = row.RiesgoDesc ?? $"Riesgo asociado a póliza {numero}";

        // 1. Fetch real risks from database
        const string risksSql = @"
            SELECT 
                rp.Id, 
                r.Descripcion,
                rp.Aplicacion AS TipoRiesgo,
                rp.F_Alta AS FechaAlta,
                rp.F_Baja AS FechaBaja
            FROM dbo.RiesgoPoliza rp
            INNER JOIN dbo.Riesgo r ON rp.RiesgoId = r.Id
            WHERE rp.PolizaId = @PolizaId";
        
        var risksQuery = await connection.QueryAsync<dynamic>(risksSql, new { PolizaId = intId });
        var risksList = risksQuery.Select(r => new PolizaRiesgo(
            Id: r.Id.ToString(),
            Descripcion: (string)(r.Descripcion ?? string.Empty),
            TipoRiesgo: (string)(r.TipoRiesgo ?? string.Empty),
            FechaAlta: r.FechaAlta != null ? DateOnly.FromDateTime((DateTime)r.FechaAlta) : (DateOnly?)null,
            FechaBaja: r.FechaBaja != null ? DateOnly.FromDateTime((DateTime)r.FechaBaja) : (DateOnly?)null
        )).ToList();

        // Fallback if no risks are found
        if (risksList.Count == 0)
        {
            risksList.Add(new PolizaRiesgo("R-001", riesgoDesc));
        }

        // 2. Fetch real receipts from database
        const string receiptsSql = @"
            SELECT 
                Id,
                ReciboCia AS Numero,
                IdSituacion AS Estado,
                IdSituacionCia AS EstadoCia,
                IdSituacionIdentidad AS EstadoColab,
                IdTipoRecibo AS Tipo,
                IdGestor AS Gestor,
                PrimaTotal,
                F_Efecto AS FechaEfecto,
                F_Vencimiento AS FechaVencimiento
            FROM dbo.Pantalla_Recibos
            WHERE PolizaId = @PolizaId";

        var receiptsQuery = await connection.QueryAsync<dynamic>(receiptsSql, new { PolizaId = intId });
        var receiptsList = receiptsQuery.Select(rec => new PolizaRecibo(
            Id: rec.Id.ToString(),
            Numero: (string)(rec.Numero ?? string.Empty),
            Estado: MapReciboEstado((string)(rec.Estado ?? string.Empty)),
            EstadoCia: MapReciboEstadoCia((string)(rec.EstadoCia ?? string.Empty)),
            EstadoColab: MapReciboEstadoCia((string)(rec.EstadoColab ?? string.Empty)),
            Tipo: MapReciboTipo((string)(rec.Tipo ?? string.Empty)),
            Gestor: MapReciboGestor((string)(rec.Gestor ?? string.Empty)),
            PrimaTotal: (decimal)(rec.PrimaTotal ?? 0m),
            FechaEfecto: rec.FechaEfecto != null ? DateOnly.FromDateTime((DateTime)rec.FechaEfecto) : DateOnly.MinValue,
            FechaVencimiento: rec.FechaVencimiento != null ? DateOnly.FromDateTime((DateTime)rec.FechaVencimiento) : DateOnly.MinValue
        )).ToList();

        return new PolizaDetail(
            Id: row.Id.ToString(),
            Numero: numero,
            Aplicacion: aplicacion,
            Estado: MapEstado(estadoId),
            Ramo: aplicacion,
            Compania: compania,
            Cliente: new PolizaCliente(
                Id: clienteId,
                Nombre: clienteNombre,
                Documento: clienteDocumento),
            Producto: new PolizaProducto(
                Nombre: productoNombre,
                Modalidad: "Estandar"),
            Vigencia: new PolizaVigencia(
                FechaInicio: row.FechaEfecto != null ? DateOnly.FromDateTime((DateTime)row.FechaEfecto) : DateOnly.MinValue,
                FechaVencimiento: row.FechaVencimiento != null ? DateOnly.FromDateTime((DateTime)row.FechaVencimiento) : DateOnly.MinValue,
                Renovacion: "Anual"),
            Financiero: new PolizaFinanciero(
                PrimaAnual: (decimal)(row.PrimaAnual ?? 0m),
                Moneda: "EUR"),
            Riesgos: risksList,
            Recibos: receiptsList
        );
    }

    private static string MapEstado(string statusId)
    {
        return statusId.ToLowerInvariant() switch
        {
            "situacionpoliza-ev" or "vigor" or "active" => "Vigor",
            "situacionpoliza-an" or "anulada" or "cancelled" => "Anulada",
            "situacionpoliza-pendiente" or "pendiente" or "pending" => "Pendiente",
            "situacionpoliza-pf" => "Pendiente Firma",
            _ => statusId
        };
    }

    private static string MapReciboEstado(string statusId)
    {
        if (string.IsNullOrWhiteSpace(statusId)) return "Desconocido";
        return statusId.ToLowerInvariant() switch
        {
            "situacionrecibo-co" or "cobrado" => "Cobrado",
            "situacionrecibo-an" or "anulado" => "Anulado",
            "situacionrecibo-pe" or "pendiente" => "Pendiente",
            "situacionrecibo-de" or "devuelto" => "Devuelto",
            _ => statusId
        };
    }

    private static string MapReciboEstadoCia(string statusId)
    {
        if (string.IsNullOrWhiteSpace(statusId)) return "Pendiente";
        return statusId.ToLowerInvariant() switch
        {
            "situacionrecibo-li" or "liquidado" => "Liquidado",
            "situacionrecibo-pe" or "pendiente" => "Pendiente",
            _ => statusId
        };
    }

    private static string MapReciboTipo(string tipoId)
    {
        if (string.IsNullOrWhiteSpace(tipoId)) return "Cartera";
        return tipoId.ToLowerInvariant() switch
        {
            "recibo-ca" => "Cartera",
            "recibo-np" => "Nueva Producción",
            "recibo-su" => "Suplemento",
            _ => tipoId
        };
    }

    private static string MapReciboGestor(string gestorId)
    {
        if (string.IsNullOrWhiteSpace(gestorId)) return "Compañía";
        return gestorId.ToLowerInvariant() switch
        {
            "gestion-co" or "correduria" => "Correduría",
            "gestion-ci" or "cia" or "compañia" or "compania" => "Compañía",
            _ => gestorId
        };
    }
}
