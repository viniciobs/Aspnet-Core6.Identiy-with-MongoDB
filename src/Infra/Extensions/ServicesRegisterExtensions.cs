using Infra.Configuraions;
using Infra.Services;
using Infra.Services.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Extensions
{
    public static class ServicesRegisterExtensions
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
        {
            var brokerConfiguration = configuration
                .GetSection(nameof(MessageBrokerConfiguration))
                .Get<MessageBrokerConfiguration>();

            return services
                .AddSingleton(brokerConfiguration)
                .AddScoped<IMessageProducer, KafkaBroker>();
        }
    }
}
