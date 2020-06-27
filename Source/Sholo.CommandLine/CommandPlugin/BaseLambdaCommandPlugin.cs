using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.CommandPlugin
{
    public abstract class BaseLambdaCommandPlugin
    {
        public string CommandName { get; }
        public string Description { get; }

        private Action<ICommandConfigurationContext, IConfigurationBuilder>[] CommandConfigurationConfiguration { get; }
        private Action<IPluginConfigurationContext, CommandLineApplication>[] CommandConfiguration { get; }

        protected BaseLambdaCommandPlugin(
            string commandName,
            string description,
            IEnumerable<Action<ICommandConfigurationContext, IConfigurationBuilder>> commandConfigurationConfiguration,
            IEnumerable<Action<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration)
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CommandConfigurationConfiguration = commandConfigurationConfiguration.ToArray();
            CommandConfiguration = commandConfiguration.ToArray();
        }

        public void ConfigureCommandConfiguration(ICommandConfigurationContext context, IConfigurationBuilder builder)
        {
            foreach (var configurator in CommandConfigurationConfiguration)
            {
                configurator.Invoke(context, builder);
            }
        }

        public void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command)
        {
            foreach (var configurator in CommandConfiguration)
            {
                configurator.Invoke(context, command);
            }
        }
    }
}