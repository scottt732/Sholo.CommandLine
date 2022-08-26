using System;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context.Logging
{
    public class LoggingContext : ILoggingContext
    {
        public IConfiguration Configuration { get; }

        public LoggingContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
