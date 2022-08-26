using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Configuration;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPlugin.Lambda
{
    public class LambdaCommandPlugin<TCommand, TParameters> : BaseLambdaCommandPlugin, ICommandPlugin<TCommand, TParameters>
        where TCommand : ICommand<TParameters>
        where TParameters : class, new()
    {
        private ITargetConfiguration<ICommandServicesContext<TParameters>, IServiceCollection>[] CommandServicesConfiguration { get; }
        private ITargetConfiguration<ICommandServicesContext<TParameters>>[] CommandContainerConfiguration { get; }
        private ITargetConfiguration<ICommandParameterizationContext, TParameters>[] ParameterConfiguration { get; }

        public LambdaCommandPlugin(
            string description,
            IEnumerable<ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>>
                commandConfigurationConfiguration,
            IEnumerable<ITargetConfiguration<ICommandServicesContext<TParameters>, IServiceCollection>>
                commandServicesConfiguration,
            IEnumerable<ITargetConfiguration<ICommandServicesContext<TParameters>>> commandContainerConfiguration,
            IEnumerable<ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration,
            IEnumerable<ITargetConfiguration<ICommandParameterizationContext, TParameters>> parameterConfiguration)
            : base(
                description,
                commandConfigurationConfiguration,
                commandConfiguration
            )
        {
            CommandServicesConfiguration = commandServicesConfiguration.ToArray();
            CommandContainerConfiguration = commandContainerConfiguration.ToArray();
            ParameterConfiguration = parameterConfiguration.ToArray();
        }

        public void ConfigureCommandServices(ICommandServicesContext<TParameters> context, IServiceCollection configure)
        {
            foreach (var configurator in CommandServicesConfiguration)
            {
                configurator.Configure(context, configure);
            }
        }

        public void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext<TParameters> context, TContainerBuilder configure)
        {
            foreach (var configurator in CommandContainerConfiguration)
            {
                configurator.ConfigureTarget(context, configure);
            }
        }

        public void ConfigureParameters(ICommandParameterizationContext context, TParameters configure)
        {
            foreach (var configurator in ParameterConfiguration)
            {
                configurator.Configure(context, configure);
            }
        }
    }

    public class LambdaCommandPlugin<TCommand> : BaseLambdaCommandPlugin, ICommandPlugin<TCommand>
        where TCommand : ICommand
    {
        private ITargetConfiguration<ICommandServicesContext, IServiceCollection>[] CommandServicesConfiguration { get; }
        private ITargetConfiguration<ICommandServicesContext>[] CommandContainerConfiguration { get; }

        public LambdaCommandPlugin(
            string description,
            IEnumerable<ITargetConfiguration<ICommandConfigurationContext, IConfigurationBuilder>>
                commandConfigurationConfiguration,
            IEnumerable<ITargetConfiguration<ICommandServicesContext, IServiceCollection>> commandServicesConfiguration,
            IEnumerable<ITargetConfiguration<ICommandServicesContext>> commandContainerConfiguration,
            IEnumerable<ITargetConfiguration<IPluginConfigurationContext, CommandLineApplication>> commandConfiguration)
            : base(
                description,
                commandConfigurationConfiguration,
                commandConfiguration)
        {
            CommandServicesConfiguration = commandServicesConfiguration.ToArray();
            CommandContainerConfiguration = commandContainerConfiguration.ToArray();
        }

        public void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection configure)
        {
            foreach (var configurator in CommandServicesConfiguration)
            {
                configurator.Configure(context, configure);
            }
        }

        public void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext context, TContainerBuilder configure)
        {
            foreach (var configurator in CommandContainerConfiguration)
            {
                configurator.ConfigureTarget(context, configure);
            }
        }
    }
}
