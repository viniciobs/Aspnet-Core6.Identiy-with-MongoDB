using Infra.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ioc.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddCustomHealthChecks(
            this IHealthChecksBuilder builder,
            IConfiguration configuration) =>
            builder
                .AddInfraHealthCheck(configuration);
    }
}
