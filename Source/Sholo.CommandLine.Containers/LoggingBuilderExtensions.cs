using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Sholo.CommandLine.Containers
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddSerilogConsole(this ILoggingBuilder loggingBuilder)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console(
                    LogEventLevel.Information,
                    "{Message:lj}{NewLine}{Exception}",
                    null,
                    null,
                    LogEventLevel.Warning,
                    AnsiConsoleTheme.Literate)
                .CreateLogger();

            loggingBuilder.AddSerilog(logger, true);

            return loggingBuilder;
        }
    }
}
