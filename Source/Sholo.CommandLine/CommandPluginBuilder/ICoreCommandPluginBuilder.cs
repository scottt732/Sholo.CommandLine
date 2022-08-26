using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPluginBuilder
{
    public interface ICoreCommandPluginBuilder<out TSelf> : ICommandContainerBuilder<TSelf>
        where TSelf : ICoreCommandPluginBuilder<TSelf>
    {
        TSelf ConfigureCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configurationBuilder);
        TSelf ConfigureCommand(Action<IPluginConfigurationContext, CommandLineApplication> command);
    }
}
