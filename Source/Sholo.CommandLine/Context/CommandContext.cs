using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context
{
    public class CommandContext<TParameters> : CommandContext, ICommandContext<TParameters>
        where TParameters : class, new()
    {
        public TParameters Parameters { get; }

        public CommandContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            IConfiguration commandConfiguration,
            IServiceProvider serviceProvider,
            CommandLineApplication command,
            TParameters parameters
        )
            : base(
                hostConfiguration,
                hostServices,
                commandConfiguration,
                serviceProvider,
                command
            )
        {
            Parameters = parameters;
        }
    }

    public class CommandContext : ICommandContext
    {
        public IConfiguration HostConfiguration { get; }
        public IServiceProvider HostServices { get; }
        public IConfiguration CommandConfiguration { get; }
        public IServiceProvider ServiceProvider { get; set; }
        public CommandLineApplication Command { get; }

        public CommandContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            IConfiguration commandConfiguration,
            IServiceProvider serviceProvider,
            CommandLineApplication command)
        {
            HostConfiguration = hostConfiguration;
            HostServices = hostServices;
            CommandConfiguration = commandConfiguration;
            ServiceProvider = serviceProvider;
            Command = command;
        }
    }
}
