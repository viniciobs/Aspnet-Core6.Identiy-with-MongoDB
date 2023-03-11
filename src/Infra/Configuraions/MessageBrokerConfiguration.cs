namespace Infra.Configuraions
{
    public record MessageBrokerConfiguration
    {
        public string BootstrapServers { get; init; }
        public string Topic { get; init; }
        public string SecurityProtocol { get; init; }
        public string SaslMechanism { get; init; }
        public string SaslUsername { get; init; }
        public string SaslPassword { get; init; }
    }
}
