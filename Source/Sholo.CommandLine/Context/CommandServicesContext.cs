using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.CommandPlugins;

namespace Sholo.CommandLine.Context;

internal class CommandServicesContext : ICommandServicesContext
{
    public string CommandName { get; }
    public string Description { get; }

    public ILoggerFactory LoggerFactory { get; }
    public IConfiguration HostConfiguration { get; }
    public IConfiguration CommandConfiguration { get; }
    public IServiceProvider HostServices { get; }

    public CommandServicesContext(
        ICommonCommandPlugin commandPlugin,
        IConfiguration hostConfiguration,
        IServiceProvider hostServices,
        IConfiguration commandConfiguration
    )
    {
        CommandName = commandPlugin.CommandName;
        Description = commandPlugin.Description;

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
        ICommonCommandPlugin commandPlugin,
        IConfiguration hostConfiguration,
        IServiceProvider hostServices,
        IConfiguration commandConfiguration,
        TCommandParameters parameters
    )
        : base(
            commandPlugin,
            hostConfiguration,
            hostServices,
            commandConfiguration
        )
    {
        Parameters = parameters;
    }
}
