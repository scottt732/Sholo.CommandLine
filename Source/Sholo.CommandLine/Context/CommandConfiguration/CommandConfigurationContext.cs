using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sholo.CommandLine.Context
{
    internal class CommandConfigurationContext : ICommandConfigurationContext
    {
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration HostConfiguration { get; }
        public IServiceProvider HostServices { get; }

        public CommandConfigurationContext(IConfiguration hostConfiguration, IServiceProvider hostServices)
        {
            HostConfiguration = hostConfiguration;
            HostServices = hostServices;
            LoggerFactory = hostServices.GetRequiredService<ILoggerFactory>();
        }
    }
}
