using Confluent.Kafka;
using Infra.Configuraions;
using System.Text.Json;

namespace Infra.Services.Implementations
{
    public class KafkaBroker : IMessageProducer
    {
        private readonly ProducerConfig _configuration;
        private readonly string _topic;

        internal ProducerConfig GetConfig() => _configuration;

        public KafkaBroker(MessageBrokerConfiguration configuration)
        {
            _topic = configuration.Topic;

            Enum.TryParse(configuration.SecurityProtocol, ignoreCase: true, out SecurityProtocol securityProtocol);
            Enum.TryParse(configuration.SaslMechanism, ignoreCase: true, out SaslMechanism saslMechanism);

            _configuration = new ProducerConfig
            {
                BootstrapServers = configuration.BootstrapServers,
                SaslUsername = configuration.SaslUsername,
                SaslPassword = configuration.SaslPassword,
                SecurityProtocol = securityProtocol,
                SaslMechanism = saslMechanism,
                MessageTimeoutMs = configuration.TimeoutMs,
                MessageSendMaxRetries = configuration.MaxRetries,
            };
        }

        public async Task<bool> SendMessageAsync(object data, CancellationToken cancellationToken)
        {
            var producer = new ProducerBuilder<Null, string>(_configuration).Build();

            var result = await producer.ProduceAsync(
                _topic,
                new Message<Null, string> 
                {
                    Value = JsonSerializer.Serialize(data) 
                },
                cancellationToken);

            producer.Dispose();

            return result.Status is PersistenceStatus.Persisted;
        }
    }
}
