using Microsoft.Extensions.Logging;

namespace Consumer.Factories
{
    internal class LoggerFactory
    {
        public static ILogger CreateLogger()
        {
            using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
                builder
                    .AddConsole()
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug));

            return loggerFactory.CreateLogger<Program>();
        }
    }
}
