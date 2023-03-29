using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Consumer.Handlers
{
    internal static class ExceptionHandler
    {
        public static void Handle(this Exception exception, ILogger logger)
        {
            if (exception is null)
            {
                logger.LogError("An error occurred but no exception was thrown");
                return;
            }

            if (exception is OperationCanceledException)
            {
                logger.LogWarning("Operation cancelled by user\nShutting down...");
                Environment.Exit(0);
            }

            if (exception is ConsumeException consumeException)
            {
                logger.LogError(consumeException.Error.Reason);
                return;
            }

            logger.LogError(exception.Message);
        }
    }
}
