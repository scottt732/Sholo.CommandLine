using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Builders.Services.CommandPlugin;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Local
namespace Sholo.CommandLine.CommandPlugin
{
    public class CommandPlugin<TCommand> : BaseCommandPlugin, ICommandPlugin<TCommand>
        where TCommand : class, ICommand
    {
        public CommandPlugin(string description)
            : base(description)
        {
        }

        public virtual void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection configure)
        {
        }

        public virtual void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext context, TContainerBuilder configure)
        {
        }

        public override void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command)
        {
            command.OnExecuteAsync(async cancellationToken =>
            {
                var (commandConfigurationContext, commandConfiguration) = BuildCommandConfiguration(context);

                var (commandServicesContext, serviceProvider) = BuildCommandServiceProvider(context, commandConfiguration);

                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();

                var scopeServiceProvider = scope.ServiceProvider;

                return await BuildAndRunCommand(
                    context,
                    command,
                    commandConfiguration,
                    scopeServiceProvider,
                    commandServicesContext,
                    cancellationToken);
            });
        }

        protected (ICommandServicesContext, IServiceProvider) BuildCommandServiceProvider(
            IPluginConfigurationContext buildContext,
            IConfigurationRoot commandConfiguration)
        {
            var commandServicesContext = new CommandServicesContext(
                buildContext.HostConfiguration,
                buildContext.HostServices,
                commandConfiguration
            );

            var commandPluginServiceProviderFactoryBuilder = new CommandPluginServiceProviderFactoryBuilder(
                buildContext.HostServiceProviderFactoryBuilder,
                buildContext.ParentServiceProviderFactoryBuilder
            );

            commandPluginServiceProviderFactoryBuilder.AddServicesConfiguration((ctx, sc) =>
            {
                sc.AddSingleton(buildContext.LoggerFactory);
                sc.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

                sc.AddSingleton<ICommand, TCommand>();
            });

            var serviceProviderFactoryBuilders = buildContext.GetServiceProviderFactoryBuilders(
                commandPluginServiceProviderFactoryBuilder,
                true
            );

            var commandServices = buildContext.HostServiceProviderFactoryBuilder.BuildCommandPluginServiceProvider(
                serviceProviderFactoryBuilders.ToArray()
            );

            return (commandServicesContext, commandServices);
        }

#pragma warning disable CA1801
        private async Task<int> BuildAndRunCommand(
            IPluginConfigurationContext buildContext,
            CommandLineApplication command,
            IConfigurationRoot commandConfiguration,
            IServiceProvider scopeServiceProvider,
            ICommandServicesContext commandServicesContext,
            CancellationToken cancellationToken)
        {
            var commandContext = new CommandContext(
                commandServicesContext.HostConfiguration,
                commandServicesContext.HostServices,
                commandConfiguration,
                scopeServiceProvider,
                command);

            var commandInstance = scopeServiceProvider.GetRequiredService<ICommand>();
            return await commandInstance.RunAsync(commandContext, cancellationToken);
        }
#pragma warning restore CA1801
    }

    public class CommandPlugin<TCommand, TCommandParameters> : BaseCommandPlugin, ICommandPlugin<TCommand, TCommandParameters>
        where TCommand : class, ICommand<TCommandParameters>
        where TCommandParameters : class, new()
    {
        public CommandPlugin(string description)
            : base(description)
        {
        }

        public virtual void ConfigureParameters(ICommandParameterizationContext context, TCommandParameters configure)
        {
        }

        public virtual void ConfigureCommandServices(ICommandServicesContext<TCommandParameters> context, IServiceCollection configure)
        {
        }

        public virtual void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext<TCommandParameters> context, TContainerBuilder configure)
        {
        }

        public override void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command)
        {
            command.OnExecuteAsync(async cancellationToken =>
            {
                var (commandConfigurationContext, commandConfiguration) = BuildCommandConfiguration(context);

                var (commandParameterizationContext, parameters) = BuildParameters(
                    context,
                    command,
                    commandConfiguration);

                var (commandServicesContext, serviceProvider) = BuildCommandServiceProvider(
                    context,
                    commandConfiguration,
                    parameters,
                    this);

                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();

                var scopeServiceProvider = scope.ServiceProvider;

                return await BuildAndRunCommand(
                    context,
                    command,
                    commandConfiguration,
                    scopeServiceProvider,
                    commandServicesContext,
                    parameters,
                    cancellationToken);
            });
        }

        protected (ICommandServicesContext<TCommandParameters>, IServiceProvider) BuildCommandServiceProvider<TCommandPlugin>(
            IPluginConfigurationContext buildContext,
            IConfigurationRoot commandConfiguration,
            TCommandParameters parameters,
#pragma warning disable CA1801
            TCommandPlugin commandPlugin
#pragma warning restore CA1801
        )
            where TCommandPlugin : ICommandPlugin<TCommand, TCommandParameters>
        {
            var commandServicesContext = new CommandServicesContext<TCommandParameters>(
                buildContext.HostConfiguration,
                buildContext.HostServices,
                commandConfiguration,
                parameters
            );

            var commandPluginServiceProviderFactoryBuilder = new CommandPluginServiceProviderFactoryBuilder<TCommandParameters>(
                buildContext.HostServiceProviderFactoryBuilder
            );

            commandPluginServiceProviderFactoryBuilder.AddServicesConfiguration((ctx, sc) =>
            {
                sc.AddSingleton(buildContext.LoggerFactory);
                sc.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

                sc.AddSingleton<ICommand<TCommandParameters>, TCommand>();
            });

            var serviceProviderFactoryBuilders = buildContext.GetServiceProviderFactoryBuilders(
                commandPluginServiceProviderFactoryBuilder,
                true
            );

            var commandServices = buildContext.HostServiceProviderFactoryBuilder.BuildCommandPluginServiceProvider(
                serviceProviderFactoryBuilders.ToArray()
            );

            return (commandServicesContext, commandServices);
        }

#pragma warning disable CA1801
        private async Task<int> BuildAndRunCommand(
            IPluginConfigurationContext buildContext,
            CommandLineApplication command,
            IConfigurationRoot commandConfiguration,
            IServiceProvider scopeServiceProvider,
            ICommandServicesContext commandServicesContext,
            TCommandParameters parameters,
            CancellationToken cancellationToken)
        {
            var commandContext = new CommandContext<TCommandParameters>(
                commandServicesContext.HostConfiguration,
                commandServicesContext.HostServices,
                commandConfiguration,
                scopeServiceProvider,
                command,
                parameters);

            var commandInstance = scopeServiceProvider.GetRequiredService<ICommand<TCommandParameters>>();
            return await commandInstance.RunAsync(commandContext, cancellationToken);
        }
#pragma warning restore CA1801

        private (ICommandParameterizationContext, TCommandParameters) BuildParameters(
            IPluginConfigurationContext buildContext,
            CommandLineApplication command,
            IConfigurationRoot commandConfiguration)
        {
            var commandParameterizationContext = new CommandParameterizationContext(
                buildContext.HostConfiguration,
                buildContext.HostServices,
                commandConfiguration,
                command);

            var parameters = new TCommandParameters();

            ConfigureParameters(commandParameterizationContext, parameters);
            return (commandParameterizationContext, parameters);
        }
    }
}
