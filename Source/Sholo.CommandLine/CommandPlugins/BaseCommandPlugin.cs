using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.CommandPlugins;

public abstract class BaseCommandPlugin : ICommonCommandPlugin
{
    public string CommandName { get; }
    public string Description { get; }

    protected BaseCommandPlugin(string commandName, string description)
    {
        CommandName = commandName;
        Description = description;
    }

    public virtual void ConfigureCommandConfiguration(ICommandConfigurationContext context, IConfigurationBuilder builder)
    {
    }

    public virtual void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command)
    {
    }

    protected virtual (ICommandConfigurationContext, IConfigurationRoot) BuildCommandConfiguration(
        IPluginConfigurationContext buildContext)
    {
        var commandConfigurationContext = new CommandConfigurationContext(
            buildContext.HostConfiguration,
            buildContext.HostServices);

        var commandConfigurationBuilder = new ConfigurationBuilder();
        buildContext.ConfigureCommonConfiguration(commandConfigurationContext, commandConfigurationBuilder);
        ConfigureCommandConfiguration(commandConfigurationContext, commandConfigurationBuilder);
        var commandConfiguration = commandConfigurationBuilder.Build();

        return (commandConfigurationContext, commandConfiguration);
    }
}
