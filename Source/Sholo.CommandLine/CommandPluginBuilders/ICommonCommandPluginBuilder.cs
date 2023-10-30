using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.CommandPlugins;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.CommandPluginBuilders;

public interface ICommonCommandPluginBuilder<out TSelf>
    where TSelf : ICommonCommandPluginBuilder<TSelf>
{
    TSelf WithName(string name);
    TSelf WithDescription(string description);
    TSelf ConfigureCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configurationBuilder);
    TSelf ConfigureCommand(Action<IPluginConfigurationContext, CommandLineApplication> command);

    TSelf WithCommand<TCommandPlugin>()
        where TCommandPlugin : class, ICommandPlugin, new();

    TSelf WithCommand(ICommandPlugin commandPlugin);
    ICommonCommandPlugin Build();
}
