using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context
{
    public class CommandParameterizationContext : ICommandParameterizationContext
    {
        public IConfiguration HostConfiguration { get; }
        public IServiceProvider HostServices { get; }
        public IConfiguration CommandConfiguration { get; }
        public IServiceProvider ServiceProvider { get; set; }
        public CommandLineApplication Command { get; }

        public CommandParameterizationContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            IConfiguration commandConfiguration,
            CommandLineApplication command)
        {
            HostConfiguration = hostConfiguration;
            HostServices = hostServices;
            CommandConfiguration = commandConfiguration;
            Command = command;
        }
    }
}
