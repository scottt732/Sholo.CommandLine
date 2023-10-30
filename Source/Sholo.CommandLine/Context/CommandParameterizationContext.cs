using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context;

public class CommandParameterizationContext : ICommandParameterizationContext
{
    public IConfigurationRoot HostConfiguration { get; }
    public IServiceProvider HostServices { get; }
    public IConfigurationRoot CommandConfiguration { get; }
    public IServiceProvider ServiceProvider { get; set; }
    public CommandLineApplication Command { get; }

    public CommandParameterizationContext(
        IConfigurationRoot hostConfiguration,
        IServiceProvider hostServices,
        IConfigurationRoot commandConfiguration,
        CommandLineApplication command)
    {
        HostConfiguration = hostConfiguration;
        HostServices = hostServices;
        CommandConfiguration = commandConfiguration;
        Command = command;
    }
}
