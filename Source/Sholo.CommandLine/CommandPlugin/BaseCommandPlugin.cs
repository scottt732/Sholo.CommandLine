using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPlugin
{
    public abstract class BaseCommandPlugin : ICommandPlugin
    {
        public string Description { get; }

        protected BaseCommandPlugin(string description)
        {
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

            buildContext.CommonCommandConfigurationConfigurator.ConfigureConfigurationBuilder(
                commandConfigurationContext,
                commandConfigurationBuilder
            );

            ConfigureCommandConfiguration(commandConfigurationContext, commandConfigurationBuilder);
            var commandConfiguration = commandConfigurationBuilder.Build();

            return (commandConfigurationContext, commandConfiguration);
        }
    }
}
