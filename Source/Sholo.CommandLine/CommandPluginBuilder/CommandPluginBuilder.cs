using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.CommandPlugin;
using Sholo.CommandLine.CommandPlugin.Lambda;
using Sholo.CommandLine.Configuration;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

namespace Sholo.CommandLine.CommandPluginBuilder
{
    public sealed class CommandPluginBuilder<TCommand>
        : BaseCommandPluginBuilder<CommandPluginBuilder<TCommand>>, ICommandPluginBuilder<CommandPluginBuilder<TCommand>, TCommand>
        where TCommand : ICommand
    {
        private List<ITargetConfiguration<ICommandServicesContext, IServiceCollection>> CommandServicesConfiguration { get; } = new List<ITargetConfiguration<ICommandServicesContext, IServiceCollection>>();
        private List<ITargetConfiguration<ICommandServicesContext>> CommandContainerConfiguration { get; } = new List<ITargetConfiguration<ICommandServicesContext>>();

        public CommandPluginBuilder<TCommand> ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> configure)
        {
            CommandServicesConfiguration.Add(new TargetConfiguration<ICommandServicesContext, IServiceCollection>(configure));
            return this;
        }

        public CommandPluginBuilder<TCommand> ConfigureCommandContainer<TContainerBuilder>(Action<ICommandServicesContext, TContainerBuilder> configure)
        {
            CommandContainerConfiguration.Add(new TargetConfiguration<ICommandServicesContext, TContainerBuilder>(configure));
            return this;
        }

        public override ICommandPlugin Build(IPluginConfigurationContext pluginConfigurationContext)
            => new LambdaCommandPlugin<TCommand>(
                Description,
                CommandConfigurationConfiguration,
                CommandServicesConfiguration,
                CommandContainerConfiguration,
                CommandConfiguration);
    }

    public sealed class CommandPluginBuilder<TCommand, TParameters>
        : BaseCommandPluginBuilder<CommandPluginBuilder<TCommand, TParameters>>, ICommandPluginBuilder<CommandPluginBuilder<TCommand, TParameters>, TCommand, TParameters>
        where TCommand : ICommand<TParameters>
        where TParameters : class, new()
    {
        private List<ITargetConfiguration<ICommandServicesContext<TParameters>, IServiceCollection>> CommandServicesConfiguration { get; } = new List<ITargetConfiguration<ICommandServicesContext<TParameters>, IServiceCollection>>();
        private List<ITargetConfiguration<ICommandServicesContext<TParameters>>> CommandContainerConfiguration { get; } = new List<ITargetConfiguration<ICommandServicesContext<TParameters>>>();
        private List<ITargetConfiguration<ICommandParameterizationContext, TParameters>> ParameterConfiguration { get; } = new List<ITargetConfiguration<ICommandParameterizationContext, TParameters>>();

        public CommandPluginBuilder<TCommand, TParameters> ConfigureCommandServices(Action<ICommandServicesContext<TParameters>, IServiceCollection> configure)
        {
            CommandServicesConfiguration.Add(new TargetConfiguration<ICommandServicesContext<TParameters>, IServiceCollection>(configure));
            return this;
        }

        public CommandPluginBuilder<TCommand, TParameters> ConfigureCommandContainer<TContainerBuilder>(Action<ICommandServicesContext<TParameters>, TContainerBuilder> configure)
        {
            CommandContainerConfiguration.Add(new TargetConfiguration<ICommandServicesContext<TParameters>, TContainerBuilder>(configure));
            return this;
        }

        public CommandPluginBuilder<TCommand, TParameters> ConfigureParameters(Action<ICommandParameterizationContext, TParameters> parameters)
        {
            ParameterConfiguration.Add(new TargetConfiguration<ICommandParameterizationContext, TParameters>(parameters));
            return this;
        }

        public override ICommandPlugin Build(IPluginConfigurationContext pluginConfigurationContext)
            => new LambdaCommandPlugin<TCommand, TParameters>(
                Description,
                CommandConfigurationConfiguration,
                CommandServicesConfiguration,
                CommandContainerConfiguration,
                CommandConfiguration,
                ParameterConfiguration);
    }
}
