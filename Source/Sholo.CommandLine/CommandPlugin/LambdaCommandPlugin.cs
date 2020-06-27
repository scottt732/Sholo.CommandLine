using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.CommandPlugin
{
    public class LambdaCommandPlugin<TCommand, TParameters> : BaseLambdaCommandPlugin, ICommandPlugin<TCommand, TParameters>
        where TCommand : ICommand<TParameters>
        where TParameters : class, new()
    {
        private Action<ICommandServicesContext<TParameters>, IServiceCollection>[] CommandServicesConfiguration { get; }
        private Action<ICommandParameterizationContext, TParameters>[] ParameterConfiguration { get; }

        public LambdaCommandPlugin(
            string commandName,
            string description,
            IEnumerable<Action<ICommandConfigurationContext, IConfigurationBuilder>> commandConfigurationConfiguration,
            IEnumerable<Action<ICommandServicesContext<TParameters>, IServiceCollection>> commandServicesConfiguration,
            IEnumerable<Action<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration,
            IEnumerable<Action<ICommandParameterizationContext, TParameters>> parameterConfiguration
        )
            : base(
                commandName,
                description,
                commandConfigurationConfiguration,
                commandConfiguration
            )
        {
            CommandServicesConfiguration = commandServicesConfiguration.ToArray();
            ParameterConfiguration = parameterConfiguration.ToArray();
        }

        public void ConfigureCommandServices(ICommandServicesContext<TParameters> context, IServiceCollection services)
        {
            foreach (var configurator in CommandServicesConfiguration)
            {
                configurator.Invoke(context, services);
            }
        }

        public void ConfigureParameters(ICommandParameterizationContext context, TParameters parameters)
        {
            foreach (var configurator in ParameterConfiguration)
            {
                configurator.Invoke(context, parameters);
            }
        }
    }

    public class LambdaCommandPlugin<TCommand> : BaseLambdaCommandPlugin, ICommandPlugin<TCommand>
        where TCommand : ICommand
    {
        private Action<ICommandServicesContext, IServiceCollection>[] CommandServicesConfiguration { get; }

        public LambdaCommandPlugin(
            string commandName,
            string description,
            IEnumerable<Action<ICommandConfigurationContext, IConfigurationBuilder>> commandConfigurationConfiguration,
            IEnumerable<Action<ICommandServicesContext, IServiceCollection>> commandServicesConfiguration,
            IEnumerable<Action<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration)
            : base(
                commandName,
                description,
                commandConfigurationConfiguration,
                commandConfiguration)
        {
            CommandServicesConfiguration = commandServicesConfiguration.ToArray();
        }

        public void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection services)
        {
            foreach (var configurator in CommandServicesConfiguration)
            {
                configurator.Invoke(context, services);
            }
        }
    }

    public class LambdaCommandPlugin : BaseLambdaCommandPlugin, ICommandPlugin
    {
        private Action<ICommandServicesContext, IServiceCollection>[] CommandServicesConfiguration { get; }

        public LambdaCommandPlugin(
            string commandName,
            string description,
            IEnumerable<Action<ICommandConfigurationContext, IConfigurationBuilder>> commandConfigurationConfiguration,
            IEnumerable<Action<ICommandServicesContext, IServiceCollection>> commandServicesConfiguration,
            IEnumerable<Action<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration)
                : base(commandName, description, commandConfigurationConfiguration, commandConfiguration)
        {
            CommandServicesConfiguration = commandServicesConfiguration.ToArray();
        }

        public void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection services)
        {
            foreach (var configurator in CommandServicesConfiguration)
            {
                configurator.Invoke(context, services);
            }
        }
    }
}
