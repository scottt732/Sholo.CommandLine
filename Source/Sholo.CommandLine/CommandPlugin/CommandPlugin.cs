using System;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedVariable
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Local
namespace Sholo.CommandLine.CommandPlugin
{
    public class CommandPlugin<TCommand> : BaseCommandPlugin, ICommandPlugin<TCommand>
        where TCommand : class, ICommand
    {
        public CommandPlugin(string commandName, string description)
            : base(commandName, description)
        {
        }

        public virtual void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection services)
        {
        }

        public override void ConfigureCommand(IPluginConfigurationContext buildContext, CommandLineApplication command)
        {
            command.OnExecuteAsync(async cancellationToken =>
            {
                var (commandConfigurationContext, commandConfiguration) = BuildCommandConfiguration(buildContext);

                var (commandServicesContext, serviceProvider) = BuildCommandServiceProvider(
                    buildContext,
                    commandConfiguration);

                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var scopeServiceProvider = scope.ServiceProvider;

                    return await BuildAndRunCommand(
                        buildContext,
                        command,
                        commandConfiguration,
                        scopeServiceProvider,
                        commandServicesContext,
                        cancellationToken);
                }
            });
        }

        protected (ICommandServicesContext, IServiceProvider) BuildCommandServiceProvider(
            IPluginConfigurationContext buildContext,
            IConfigurationRoot commandConfiguration)
        {
            var commandServicesContext = new CommandServicesContext(
                buildContext.HostConfiguration,
                buildContext.HostServices,
                commandConfiguration);

            var commandServices = new ServiceCollection();
            commandServices.AddLogging(loggingBuilder =>
            {
                buildContext.ConfigureLogBuilder(loggingBuilder);
            });
            commandServices.AddSingleton<ICommand, TCommand>();

            buildContext.ConfigureCommonCommandServices(commandServicesContext, commandServices);
            ConfigureCommandServices(commandServicesContext, commandServices);

            var serviceProvider = commandServices.BuildServiceProvider();

            return (commandServicesContext, serviceProvider);
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
        public CommandPlugin(string commandName, string description)
            : base(commandName, description)
        {
        }

        public virtual void ConfigureParameters(ICommandParameterizationContext context, TCommandParameters parameters)
        {
        }

        public virtual void ConfigureCommandServices(ICommandServicesContext<TCommandParameters> context, IServiceCollection services)
        {
        }

        public override void ConfigureCommand(IPluginConfigurationContext buildContext, CommandLineApplication command)
        {
            command.OnExecuteAsync(async cancellationToken =>
            {
                var (commandConfigurationContext, commandConfiguration) = BuildCommandConfiguration(buildContext);

                var (commandParameterizationContext, parameters) = BuildParameters(
                    buildContext,
                    command,
                    commandConfiguration);

                var (commandServicesContext, serviceProvider) = BuildCommandServiceProvider(
                    buildContext,
                    commandConfiguration,
                    parameters,
                    this);

                var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    var scopeServiceProvider = scope.ServiceProvider;

                    return await BuildAndRunCommand(
                        buildContext,
                        command,
                        commandConfiguration,
                        scopeServiceProvider,
                        commandServicesContext,
                        parameters,
                        cancellationToken);
                }
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
                parameters);

            var commandServices = new ServiceCollection();
            commandServices.AddLogging(loggingBuilder =>
            {
                buildContext.ConfigureLogBuilder(loggingBuilder);
            });
            commandServices.AddSingleton<ICommand<TCommandParameters>, TCommand>();

            buildContext.ConfigureCommonCommandServices(commandServicesContext, commandServices);
            ConfigureCommandServices(commandServicesContext, commandServices);

            var serviceProvider = commandServices.BuildServiceProvider();

            return (commandServicesContext, serviceProvider);
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
