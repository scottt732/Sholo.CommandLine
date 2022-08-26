using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Builders.Configuration.CommandPlugin;
using Sholo.CommandLine.Builders.Configuration.Host;
using Sholo.CommandLine.Builders.Logging;
using Sholo.CommandLine.Builders.Services.CommandPlugin;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.CommandPlugin;
using Sholo.CommandLine.CommandPluginBuilder;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.Logging;
using Sholo.CommandLine.Context.PluginConfiguration;
using Sholo.CommandLine.Services.CommandPlugin;
using Sholo.CommandLine.Services.Host;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine
{
    public abstract class BaseCommandLineAppBuilder<TSelf> : ICommandLineAppBuilder<TSelf>
            where TSelf : BaseCommandLineAppBuilder<TSelf>, new()
    {
        private string Description { get; set; }

        private ILoggingBuilderConfigurator LoggingBuilderConfigurator { get; }
        private IHostServiceProviderFactoryBuilder HostServiceProviderFactoryBuilder { get; }
        private IHostConfigurationBuilderConfigurator HostConfigurationBuilderConfigurator { get; }
        private ICommandPluginConfigurationBuilderConfigurator CommonCommandConfigurationConfigurator { get; }
        private ICommandPluginServiceProviderFactoryBuilder CommonCommandServiceProviderFactoryBuilder { get; }

        private IDictionary<string, Func<IPluginConfigurationContext, ICommandPlugin>> CommandPlugins { get; } = new Dictionary<string, Func<IPluginConfigurationContext, ICommandPlugin>>(StringComparer.OrdinalIgnoreCase);

        protected BaseCommandLineAppBuilder()
        {
            LoggingBuilderConfigurator = new LoggingBuilderConfigurator();
            HostServiceProviderFactoryBuilder = new HostServiceProviderFactoryBuilder();
            HostConfigurationBuilderConfigurator = new HostConfigurationBuilderConfigurator();
            CommonCommandConfigurationConfigurator = new CommandPluginConfigurationBuilderConfigurator();
            CommonCommandServiceProviderFactoryBuilder = new CommandPluginServiceProviderFactoryBuilder(HostServiceProviderFactoryBuilder, null);
        }

        public TSelf WithDescription(string description)
        {
            Description = description;
            return (TSelf)this;
        }

        public TSelf UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            HostServiceProviderFactoryBuilder.SetServiceProviderFactory(factory);
            return (TSelf)this;
        }

        public TSelf UseServiceProviderFactory<TContainerBuilder>(Func<IHostServicesContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            throw new NotImplementedException();
        }

        public TSelf UseDefaultServiceProvider(Action<ServiceProviderOptions> configure)
        {
            throw new NotImplementedException();
        }

        public TSelf UseDefaultServiceProvider(Action<IHostServicesContext, ServiceProviderOptions> configure)
        {
            throw new NotImplementedException();
        }

        public TSelf ConfigureHostConfiguration(Action<IHostContext, IConfigurationBuilder> configuration)
        {
            HostConfigurationBuilderConfigurator.AddConfiguration(configuration);
            return (TSelf)this;
        }

        public TSelf ConfigureHostServices(Action<IHostServicesContext, IServiceCollection> services)
        {
            HostServiceProviderFactoryBuilder.AddServicesConfiguration(services);
            return (TSelf)this;
        }

        public TSelf ConfigureHostContainer<TContainerBuilder>(Action<IHostServicesContext, TContainerBuilder> container)
        {
            HostServiceProviderFactoryBuilder.AddContainerConfiguration(container);
            return (TSelf)this;
        }

        public TSelf ConfigureLogging(Action<ILoggingContext, ILoggingBuilder> loggingConfiguration)
        {
            LoggingBuilderConfigurator.AddLoggingBuilderConfiguration(loggingConfiguration);
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configuration)
        {
            CommonCommandConfigurationConfigurator.AddConfiguration(configuration);
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandServices(Action<ICommandServicesContext, IServiceCollection> services)
        {
            CommonCommandServiceProviderFactoryBuilder.AddServicesConfiguration(services);
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandContainer<TContainerBuilder>(Action<ICommandServicesContext, TContainerBuilder> services)
        {
            CommonCommandServiceProviderFactoryBuilder.AddContainerConfiguration(services);
            return (TSelf)this;
        }

        public TSelf WithCommand<TCommand>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand>> configurator
        )
            where TCommand : ICommand
        {
            var cpb = new CommandPluginBuilder<TCommand>()
                .WithDescription(description);

            configurator.Invoke(cpb);

            CommandPlugins[name] = b => cpb.Build(b);
            return (TSelf)this;
        }

        public TSelf WithCommand<TCommand, TCommandParameters>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand, TCommandParameters>> configurator
        )
            where TCommand : ICommand<TCommandParameters>
            where TCommandParameters : class, new()
        {
            var cpb = new CommandPluginBuilder<TCommand, TCommandParameters>()
                .WithDescription(description);

            configurator.Invoke(cpb);

            CommandPlugins[name] = b => cpb.Build(b);
            return (TSelf)this;
        }

        public ICommandLineApp Build()
        {
            var hostContext = new HostContext();
            var hostConfiguration = HostConfigurationBuilderConfigurator.BuildHostConfiguration(hostContext);

            var hostServicesContext = new HostServicesContext(hostConfiguration);
            HostServiceProviderFactoryBuilder.SetContext(hostServicesContext);
            HostServiceProviderFactoryBuilder.AddServicesConfiguration((ctx, sc) =>
            {
                var loggingContext = new LoggingContext(ctx.HostConfiguration);
                sc.AddLogging(lb =>
                {
                    LoggingBuilderConfigurator.ConfigureLoggingBuilder(loggingContext, lb);
                });
            });

            var hostServices = HostServiceProviderFactoryBuilder.BuildHostServiceProvider();
            var loggerFactory = hostServices.GetRequiredService<ILoggerFactory>();

            CommonCommandServiceProviderFactoryBuilder.AddServicesConfiguration((ctx, sc) =>
            {
                sc.AddSingleton(ctx.LoggerFactory);
                sc.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            });

            var pluginConfigurationContext = new PluginConfigurationContext(
                hostConfiguration,
                hostServices,
                loggerFactory,
                HostServiceProviderFactoryBuilder,
                CommonCommandConfigurationConfigurator,
                CommonCommandServiceProviderFactoryBuilder,
                null);

            var rootCommandPlugin = CreateRootCommand(pluginConfigurationContext);

            var app = new CommandLineApplication();
            rootCommandPlugin.ConfigureCommand(pluginConfigurationContext, app);
            return new CommandLineApp(app);
        }

        protected virtual ICommandPlugin CreateRootCommand(IPluginConfigurationContext pluginConfigurationContext)
            => new CommandPluginBuilder<RootCommand>()
                .WithDescription(this.Description)
                .ConfigureCommand((_, commandLineApp) =>
                {
                    commandLineApp.Name = "Test";
                    commandLineApp.HelpOption(true);

                    foreach (var commandLinePlugin in CommandPlugins)
                    {
                        var commandName = commandLinePlugin.Key;
                        var pluginFactory = commandLinePlugin.Value;

                        commandLineApp.Command(commandName, childCommand =>
                        {
                            var cmdContext = new PluginConfigurationContext(
                                pluginConfigurationContext.HostConfiguration,
                                pluginConfigurationContext.HostServices,
                                pluginConfigurationContext.LoggerFactory,
                                pluginConfigurationContext.HostServiceProviderFactoryBuilder,
                                pluginConfigurationContext.CommonCommandConfigurationConfigurator,
                                pluginConfigurationContext.CommonCommandServiceProviderFactoryBuilder,
                                null);

                            var plugin = pluginFactory.Invoke(cmdContext);

                            childCommand.Description = plugin.Description;

                            plugin.ConfigureCommand(pluginConfigurationContext, childCommand);
                        });
                    }
                })
                .Build(pluginConfigurationContext);

        // public TSelf WithCommand<TCommandPlugin>()
        //     where TCommandPlugin : class, ICommandPlugin, new()
        // {
        //     var commandPlugin = new TCommandPlugin();
        //     CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        //     return (TSelf)this;
        // }

        // public TSelf WithCommand(ICommandPlugin commandPlugin)
        // {
        //     CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
        //     return (TSelf)this;
        // }

        // public TSelf UseServiceProviderFactory<TContainerBuilder>(Func<IHostServicesContext, IServiceProviderFactory<TContainerBuilder>> factory)
        // {
        //     HostServiceProviderFactoryBuilder.SetServiceProviderFactory(factory);
        //     return (TSelf)this;
        // }
        // public TSelf UseDefaultServiceProvider(Action<ServiceProviderOptions> configure)
        // {
        //     return UseDefaultServiceProvider((_, options) => configure(options));
        // }
        // public TSelf UseDefaultServiceProvider(Action<IHostServicesContext, ServiceProviderOptions> configure)
        // {
        //     return UseServiceProviderFactory(context =>
        //     {
        //         var options = new ServiceProviderOptions();
        //         configure(context, options);
        //         return new DefaultServiceProviderFactory(options);
        //     });
        // }

        /*
        protected virtual IServiceProvider BuildHostServices(IHostServicesContext hostServicesContext)
        {
            var hostServiceCollection = new ServiceCollection();
            var hostServices = HostServiceProviderFactory.Invoke(hostServicesContext, hostServiceCollection);
            return hostServices;
        }

        private IServiceProvider CreateHostServiceProvider<TContainerBuilder>(
            IHostServicesContext ctx,
            IServiceCollection sc,
            IServiceProviderFactory<TContainerBuilder> factory)
        {
            foreach (var hostServicesConfigurator in this.HostServicesConfiguration)
            {
                hostServicesConfigurator.Configure(ctx, sc);
            }

            var containerBuilder = factory.CreateBuilder(sc);

            foreach (var hostContainerAction in HostContainerConfiguration.Cast<ITargetConfiguration<IHostServicesContext, TContainerBuilder>>())
            {
                hostContainerAction.Configure(ctx, containerBuilder);
            }

            var result = factory.CreateServiceProvider(containerBuilder);
            return result;
        }

        private IServiceProvider CreateCommandServiceProvider<TContainerBuilder>(
            ICommandServicesContext ctx,
            IServiceCollection sc,
            IServiceProviderFactory<TContainerBuilder> factory,
            ITargetConfiguration<ICommandServicesContext, IServiceCollection> configureCommandTarget,
            ITargetConfiguration<ICommandServicesContext> configureCommandContainer)
        {
            foreach (var configure in CommonCommandServicesConfiguration)
            {
                configure.Configure(ctx, sc);
            }

            configureCommandTarget.Configure(ctx, sc);

            sc.TryAddSingleton(ctx.LoggerFactory);
            sc.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));

            var containerBuilder = factory.CreateBuilder(sc);

            foreach (var commonCommandContainerConfiguration in CommonCommandContainerConfiguration.Cast<ITargetConfiguration<ICommandServicesContext, TContainerBuilder>>())
            {
                commonCommandContainerConfiguration.Configure(ctx, containerBuilder);
            }

            ((ITargetConfiguration<ICommandServicesContext, TContainerBuilder>)configureCommandContainer).Configure(ctx, containerBuilder);

            var result = factory.CreateServiceProvider(containerBuilder);

            return result;
        }
        */
    }
}
