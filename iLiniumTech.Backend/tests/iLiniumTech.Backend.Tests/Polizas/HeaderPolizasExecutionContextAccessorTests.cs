using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
                .Build());

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
            configuration);

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
            configuration);

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
            configuration);

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
            new ConfigurationBuilder().Build());

        accessor.Current.Should().BeNull();
    }
}
