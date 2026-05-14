using System.Data;
using FluentAssertions;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Polizas.Sql;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class SqlServerSessionContextTests
{
    [Fact]
    public void BuildApplyQuery_parameterizes_session_context_keys_and_values()
    {
        var query = SqlServerSessionContext.BuildApplyQuery(new PolizasExecutionContext(
            BrokerId: 42,
            UserId: 7,
            ProfileId: 9,
            ProfileTypeId: "profile'; EXEC xp_cmdshell --",
            IsAdmin: true));

        query.CommandText.Should().Contain("@key=@brokerIdKey, @value=@brokerId");
        query.CommandText.Should().Contain("@key=@profileTypeIdKey, @value=@profileTypeId");
        query.CommandText.Should().NotContain("xp_cmdshell");
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@brokerIdKey" &&
            (string)parameter.Value == "brokerId" &&
            parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@brokerId" &&
            (int)parameter.Value == 42 &&
            parameter.DbType == SqlDbType.Int);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@profileTypeId" &&
            (string)parameter.Value == "profile'; EXEC xp_cmdshell --" &&
            parameter.DbType == SqlDbType.NVarChar);
        query.Parameters.Should().Contain(parameter =>
            parameter.Name == "@isAdmin" &&
            (bool)parameter.Value &&
            parameter.DbType == SqlDbType.Bit);
    }

    [Fact]
    public void BuildApplyQuery_clears_known_session_context_values_without_request_context()
    {
        var query = SqlServerSessionContext.BuildApplyQuery(context: null);

        query.CommandText.Should().Contain("sp_set_session_context");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@brokerIdKey");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@entityMainIdKey");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@userIdKey");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@profileIdKey");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@profileTypeIdKey");
        query.Parameters.Should().Contain(parameter => parameter.Name == "@isAdminKey");
        query.Parameters
            .Where(parameter => !parameter.Name.EndsWith("Key", StringComparison.Ordinal))
            .Should()
            .OnlyContain(parameter => parameter.Value == DBNull.Value);
    }
}
