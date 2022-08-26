using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Configuration;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPlugin.Lambda
{
    public abstract class BaseLambdaCommandPlugin
    {
        public string Description { get; }

        private ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>[] CommandConfigurationConfiguration { get; }
        private ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>[] CommandConfiguration { get; }

        protected BaseLambdaCommandPlugin(
            string description,
            IEnumerable<ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>> commandConfigurationConfiguration,
            IEnumerable<ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            CommandConfigurationConfiguration = commandConfigurationConfiguration.ToArray();
            CommandConfiguration = commandConfiguration.ToArray();
        }

        public void ConfigureCommandConfiguration(ICommandConfigurationContext context, IConfigurationBuilder builder)
        {
            foreach (var configurator in CommandConfigurationConfiguration)
            {
                configurator.Configure(context, builder);
            }
        }

        public void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command)
        {
            foreach (var configurator in CommandConfiguration)
            {
                configurator.Configure(context, command);
            }
        }
    }
}
