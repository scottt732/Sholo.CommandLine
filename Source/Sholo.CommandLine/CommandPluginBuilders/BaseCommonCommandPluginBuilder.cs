using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.CommandPlugins;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.CommandPluginBuilders;

public abstract class BaseCommonCommandPluginBuilder<TSelf>
    : ICommonCommandPluginBuilder<TSelf>
    where TSelf : BaseCommonCommandPluginBuilder<TSelf>, new()
{
    protected string Name { get; set; }
    protected string Description { get; set; }

    protected List<Action<ICommandConfigurationContext, IConfigurationBuilder>> CommandConfigurationConfiguration { get; } = new List<Action<ICommandConfigurationContext, IConfigurationBuilder>>();
    protected List<Action<IPluginConfigurationContext, CommandLineApplication>> CommandConfiguration { get; } = new List<Action<IPluginConfigurationContext, CommandLineApplication>>();
    protected IDictionary<string, ICommandPlugin> CommandPlugins { get; } = new Dictionary<string, ICommandPlugin>(StringComparer.OrdinalIgnoreCase);

    public TSelf WithName(string commandName)
    {
        Name = commandName;
        return (TSelf)this;
    }

    public TSelf WithDescription(string description)
    {
        Description = description;
        return (TSelf)this;
    }

    public TSelf ConfigureCommand(Action<IPluginConfigurationContext, CommandLineApplication> command)
    {
        CommandConfiguration.Add(command);
        return (TSelf)this;
    }

    public TSelf WithCommand<TCommandPlugin>()
        where TCommandPlugin : class, ICommandPlugin, new()
    {
        var commandPlugin = new TCommandPlugin();
        CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        return (TSelf)this;
    }

    public TSelf WithCommand(ICommandPlugin commandPlugin)
    {
        CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        return (TSelf)this;
    }

    public TSelf ConfigureCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configurationBuilder)
    {
        CommandConfigurationConfiguration.Add(configurationBuilder);
        return (TSelf)this;
    }

    public abstract ICommonCommandPlugin Build();
}
