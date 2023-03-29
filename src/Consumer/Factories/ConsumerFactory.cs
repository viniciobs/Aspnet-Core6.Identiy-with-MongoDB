using Confluent.Kafka;

namespace Consumer.Factories
{
    internal class ConsumerFactory
    {
        public static IConsumer<Ignore, string> CreateConsumer()
        {
            var consumer = new ConsumerBuilder<Ignore, string>(
                new ConsumerConfig
                {
                    BootstrapServers = "localhost:9092",
                    GroupId = "AspnetIdentityWithMongo",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                }).Build();

            consumer.Subscribe("emails-pending-confirmation");

            return consumer;
        }
    }
}
