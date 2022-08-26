using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sholo.CommandLine.Context
{
    internal class CommandServicesContext : ICommandServicesContext
    {
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration HostConfiguration { get; }
        public IConfiguration CommandConfiguration { get; }
        public IServiceProvider HostServices { get; }

        public CommandServicesContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            IConfiguration commandConfiguration
        )
        {
            HostConfiguration = hostConfiguration;
            HostServices = hostServices;
            LoggerFactory = hostServices.GetRequiredService<ILoggerFactory>();
            CommandConfiguration = commandConfiguration;
        }
    }

    internal class CommandServicesContext<TCommandParameters> : CommandServicesContext, ICommandServicesContext<TCommandParameters>
        where TCommandParameters : class, new()
    {
        public TCommandParameters Parameters { get; }

        public CommandServicesContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            IConfiguration commandConfiguration,
            TCommandParameters parameters
        )
            : base(
                hostConfiguration,
                hostServices,
                commandConfiguration
            )
        {
            Parameters = parameters;
        }
    }
}
