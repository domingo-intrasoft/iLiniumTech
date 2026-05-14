using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class HeaderPolizasExecutionContextAccessorTests
{
    [Fact]
    public void Current_reads_broker_context_from_headers()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.BrokerIdHeaderName] = "42";
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.UserIdHeaderName] = "7";
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.ProfileIdHeaderName] = "9";
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.ProfileTypeIdHeaderName] = "profile-type";
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.IsAdminHeaderName] = "true";
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = httpContext },
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Polizas:AllowHeaderExecutionContext"] = "true"
                })
                .Build(),
            DevelopmentEnvironment());

        var current = accessor.Current;

        current.Should().NotBeNull();
        current!.BrokerId.Should().Be(42);
        current.EntityMainId.Should().Be(42);
        current.UserId.Should().Be(7);
        current.ProfileId.Should().Be(9);
        current.ProfileTypeId.Should().Be("profile-type");
        current.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public void Current_ignores_headers_unless_header_context_is_enabled()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.BrokerIdHeaderName] = "42";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:BrokerId"] = "84"
            })
            .Build();
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = httpContext },
            configuration,
            DevelopmentEnvironment());

        var current = accessor.Current;

        current.Should().NotBeNull();
        current!.BrokerId.Should().Be(84);
    }

    [Fact]
    public void Current_rejects_invalid_broker_header_when_header_context_is_enabled()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.BrokerIdHeaderName] = "not-a-broker";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:BrokerId"] = "84"
            })
            .Build();
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = httpContext },
            configuration,
            DevelopmentEnvironment());

        var act = () => accessor.Current;

        act.Should().Throw<PolizasExecutionContextException>()
            .WithMessage("*X-Broker-Id*positive integer*");
    }

    [Fact]
    public void Current_uses_configuration_as_fallback()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:BrokerId"] = "84",
                ["Polizas:UserId"] = "10",
                ["Polizas:ProfileId"] = "11",
                ["Polizas:ProfileTypeId"] = "fallback-profile",
                ["Polizas:IsAdmin"] = "false"
            })
            .Build();
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() },
            configuration,
            DevelopmentEnvironment());

        var current = accessor.Current;

        current.Should().NotBeNull();
        current!.BrokerId.Should().Be(84);
        current.UserId.Should().Be(10);
        current.ProfileId.Should().Be(11);
        current.ProfileTypeId.Should().Be("fallback-profile");
        current.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void Current_is_null_without_header_or_fallback()
    {
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = new DefaultHttpContext() },
            new ConfigurationBuilder().Build(),
            DevelopmentEnvironment());

        accessor.Current.Should().BeNull();
    }

    [Fact]
    public void Current_ignores_header_context_outside_development_without_override()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.BrokerIdHeaderName] = "42";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:BrokerId"] = "84"
            })
            .Build();
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = httpContext },
            configuration,
            ProductionEnvironment());

        var current = accessor.Current;

        current.Should().NotBeNull();
        current!.BrokerId.Should().Be(84);
    }

    [Fact]
    public void Current_can_enable_header_context_outside_development_with_explicit_override()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderPolizasExecutionContextAccessor.BrokerIdHeaderName] = "42";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:AllowHeaderExecutionContextOutsideDevelopment"] = "true",
                ["Polizas:BrokerId"] = "84"
            })
            .Build();
        var accessor = new HeaderPolizasExecutionContextAccessor(
            new HttpContextAccessor { HttpContext = httpContext },
            configuration,
            ProductionEnvironment());

        var current = accessor.Current;

        current.Should().NotBeNull();
        current!.BrokerId.Should().Be(42);
    }

    private static IHostEnvironment DevelopmentEnvironment() => new TestHostEnvironment("Development");

    private static IHostEnvironment ProductionEnvironment() => new TestHostEnvironment("Production");

    private sealed class TestHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "iLiniumTech.Backend.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
