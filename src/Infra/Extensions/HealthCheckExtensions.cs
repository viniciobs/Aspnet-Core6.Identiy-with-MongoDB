using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Confluent.Kafka;
using Infra.Configuraions;
using Infra.Services.Implementations;
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
            return builder
                .AddMongoHealthCheck(configuration)
                .AddKafkaHealthCheck(configuration);
        }

        private static IHealthChecksBuilder AddMongoHealthCheck(
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

        private static IHealthChecksBuilder AddKafkaHealthCheck(
            this IHealthChecksBuilder builder,
            IConfiguration configuration)
        {
            var brokerConfiguration = configuration
                .GetSection(nameof(MessageBrokerConfiguration))
                .Get<MessageBrokerConfiguration>();
                
            var producerConfig = new KafkaBroker(brokerConfiguration).GetConfig();

            return builder.AddKafka(
               config: producerConfig,
               topic: brokerConfiguration.Topic);
        }
    }
}