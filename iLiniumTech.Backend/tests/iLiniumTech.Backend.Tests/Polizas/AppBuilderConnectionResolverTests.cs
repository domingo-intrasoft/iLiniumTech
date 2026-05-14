using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.Data.SqlClient;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class AppBuilderConnectionResolverTests
{
    [Fact]
    public void Protector_decrypts_appbuilder_aes_values()
    {
        const string key = "unit-test-key";
        var encrypted = EncryptLikeAppBuilder("ModeloDb", key);

        var protector = new AppBuilderConnectionValueProtector(key);

        protector.DecryptData(encrypted).Should().Be("ModeloDb");
    }

    [Fact]
    public void Protector_returns_plain_values_when_key_is_not_configured()
    {
        var protector = new AppBuilderConnectionValueProtector(null);

        protector.DecryptData("ModeloDb").Should().Be("ModeloDb");
    }

    [Fact]
    public void BuildModelConnectionString_decrypts_fields_and_sets_sql_options()
    {
        const string key = "unit-test-key";
        var protector = new AppBuilderConnectionValueProtector(key);
        var record = new AppBuilderModelConnectionRecord(
            Server: EncryptLikeAppBuilder("ModeloSql", key),
            Database: EncryptLikeAppBuilder("IL_Modelo", key),
            DatabaseUser: EncryptLikeAppBuilder("readonly_user", key),
            DatabasePassword: EncryptLikeAppBuilder("readonly_password", key));

        var connectionString = AppBuilderMasterPolizasConnectionStringProvider
            .BuildModelConnectionString(record, protector);

        var builder = new SqlConnectionStringBuilder(connectionString);
        builder.DataSource.Should().Be("ModeloSql");
        builder.InitialCatalog.Should().Be("IL_Modelo");
        builder.UserID.Should().Be("readonly_user");
        builder.Password.Should().Be("readonly_password");
        builder.TrustServerCertificate.Should().BeTrue();
        builder.IntegratedSecurity.Should().BeFalse();
        builder.MultipleActiveResultSets.Should().BeTrue();
    }

    [Fact]
    public void BuildModelConnectionString_rejects_incomplete_records()
    {
        var record = new AppBuilderModelConnectionRecord(
            Server: "ModeloSql",
            Database: string.Empty,
            DatabaseUser: "readonly_user",
            DatabasePassword: "readonly_password");

        var act = () => AppBuilderMasterPolizasConnectionStringProvider
            .BuildModelConnectionString(record, new AppBuilderConnectionValueProtector(null));

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*incomplete*");
    }

    [Fact]
    public async Task AppBuilderMaster_resolver_requires_runtime_broker_context()
    {
        var resolver = new AppBuilderMasterPolizasConnectionStringProvider(
            "Server=localhost;Database=Master;User Id=user;Password=password;TrustServerCertificate=True",
            new TestExecutionContextAccessor(null));

        var act = async () => await resolver.GetConnectionStringAsync(CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*broker execution context*");
    }

    private static string EncryptLikeAppBuilder(string value, string key)
    {
        var inputBytes = Encoding.UTF8.GetBytes(value);
        var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        var saltBytes = new byte[] { 17, 25, 83, 20, 7, 85, 22, 8 };

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        var derivedBytes = Rfc2898DeriveBytes.Pbkdf2(
            keyBytes,
            saltBytes,
            1000,
            HashAlgorithmName.SHA1,
            (aes.KeySize / 8) + (aes.BlockSize / 8));
        aes.Key = derivedBytes[..(aes.KeySize / 8)];
        aes.IV = derivedBytes[(aes.KeySize / 8)..];
        aes.Mode = CipherMode.CBC;

        using var output = new MemoryStream();
        using (var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
        }

        CryptographicOperations.ZeroMemory(derivedBytes);
        return Convert.ToBase64String(output.ToArray());
    }

    private sealed class TestExecutionContextAccessor(PolizasExecutionContext? current) : IPolizasExecutionContextAccessor
    {
        public PolizasExecutionContext? Current => current;
    }
}
