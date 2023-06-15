using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddInfraHealthCheck(
            this IHealthChecksBuilder builder,
            IConfiguration configuration)
        {
            var mongoSettings = configuration
                .GetSection(nameof(MongoDbSettings))
                .Get<MongoDbSettings>();

            return builder.AddMongoDb(
                mongodbConnectionString: mongoSettings.ConnectionString,
                mongoDatabaseName: mongoSettings.DatabaseName);
        }
    }
}