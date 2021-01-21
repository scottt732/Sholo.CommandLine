using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.CommandPlugin;
using Sholo.CommandLine.Context;

public interface IServicesConfiguration<in TServicesContext>
{
}

public interface IServicesConfiguration<in TServicesContext, in TTarget> : IServicesConfiguration<TServicesContext>
{
    void Configure(TServicesContext context, TTarget configure);
}

public class ServicesConfiguration<TServicesContext, TTarget> : IServicesConfiguration<TServicesContext, TTarget>
{
    private Action<TServicesContext, TTarget> Factory { get; }

    public ServicesConfiguration(Action<TServicesContext, TTarget> factory)
    {
        Factory = factory;
    }

    public void Configure(TServicesContext context, TTarget configure)
    {
        Factory?.Invoke(context, configure);
    }
}

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.AppBuilder
{
    public abstract class BaseCommandLineAppBuilder<TSelf> : ICommandLineAppBuilder<TSelf>
            where TSelf : BaseCommandLineAppBuilder<TSelf>, new()
    {
        private string Name { get; set; }
        private string Description { get; set; }

        private Func<IHostServicesContext, IServiceCollection, IServiceProvider> HostServiceProviderFactory { get; set; }
        private Func<ICommandServicesContext, IServiceCollection, IServiceProvider> CommandServiceProviderFactory { get; set; }

        private IList<Action<ILoggingBuilder>> LoggingBuilderConfiguration { get; } = new List<Action<ILoggingBuilder>>();
        private IList<Action<IHostServicesContext, IServiceCollection>> HostServicesConfiguration { get; } = new List<Action<IHostServicesContext, IServiceCollection>>();
        private IList<IServicesConfiguration<IHostServicesContext>> HostContainerConfiguration { get; } = new List<IServicesConfiguration<IHostServicesContext>>();
        private IList<Action<IHostContext, IConfigurationBuilder>> HostConfigurationConfiguration { get; } = new List<Action<IHostContext, IConfigurationBuilder>>();

        private IList<Action<ICommandConfigurationContext, IConfigurationBuilder>> CommonCommandConfigurationConfiguration { get; } = new List<Action<ICommandConfigurationContext, IConfigurationBuilder>>();
        private IList<ServicesConfiguration<ICommandServicesContext, IServiceCollection>> CommonCommandServicesConfiguration { get; } = new List<ServicesConfiguration<ICommandServicesContext, IServiceCollection>>();
        private IList<IServicesConfiguration<ICommandServicesContext>> CommonCommandContainerConfiguration { get; } = new List<IServicesConfiguration<ICommandServicesContext>>();

        private IDictionary<string, ICommonCommandPlugin> CommandPlugins { get; } = new Dictionary<string, ICommonCommandPlugin>(StringComparer.OrdinalIgnoreCase);

        public TSelf WithName(string commandName)
        {
            Name = commandName;
            return (TSelf)this;
        }

        public TSelf WithDescription(string description)
        {
            Description = description;
            return (TSelf)this;
        }

        public TSelf UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            HostServiceProviderFactory = (ctx, sc) => CreateHostServiceProvider(ctx, sc, factory);
            CommandServiceProviderFactory = (ctx, sc) => CreateCommandServiceProviderFactory(ctx, sc, factory);

            return (TSelf)this;
        }

        private IServiceProvider CreateCommandServiceProviderFactory<TContainerBuilder>(
            ICommandServicesContext ctx,
            IServiceCollection sc,
            IServiceProviderFactory<TContainerBuilder> factory)
        {
            var containerBuilder = factory.CreateBuilder(sc);

            foreach (var commonCommandContainerConfiguration in CommonCommandContainerConfiguration.Cast<IServicesConfiguration<ICommandServicesContext, TContainerBuilder>>())
            {
                commonCommandContainerConfiguration.Configure(ctx, containerBuilder);
            }

            var result = factory.CreateServiceProvider(containerBuilder);
            return result;
        }

        private IServiceProvider CreateHostServiceProvider<TContainerBuilder>(
            IHostServicesContext ctx,
            IServiceCollection sc,
            IServiceProviderFactory<TContainerBuilder> factory)
        {
            var containerBuilder = factory.CreateBuilder(sc);

            foreach (var hostContainerAction in HostContainerConfiguration.Cast<IServicesConfiguration<IHostServicesContext, TContainerBuilder>>())
            {
                hostContainerAction.Configure(ctx, containerBuilder);
            }

            var result = factory.CreateServiceProvider(containerBuilder);
            return result;
        }

        public TSelf UseServiceProviderFactory<TContainerBuilder>(Func<IHostServicesContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            HostServiceProviderFactory = (ctx, sc) =>
            {
                var serviceProviderFactory = factory.Invoke(ctx);
                var containerBuilder = serviceProviderFactory.CreateBuilder(sc);
                var result = serviceProviderFactory.CreateServiceProvider(containerBuilder);
                return result;
            };
            return (TSelf)this;
        }

        public TSelf UseDefaultServiceProvider(Action<ServiceProviderOptions> configure)
        {
            return UseDefaultServiceProvider((_, options) => configure(options));
        }

        public TSelf UseDefaultServiceProvider(Action<IHostServicesContext, ServiceProviderOptions> configure)
        {
            return UseServiceProviderFactory(context =>
            {
                var options = new ServiceProviderOptions();
                configure(context, options);
                return new DefaultServiceProviderFactory(options);
            });
        }

        public TSelf ConfigureHostConfiguration(Action<IHostContext, IConfigurationBuilder> configuration)
        {
            HostConfigurationConfiguration.Add(configuration);
            return (TSelf)this;
        }

        public TSelf ConfigureHostServices(Action<IHostServicesContext, IServiceCollection> services)
        {
            HostServicesConfiguration.Add(services);
            return (TSelf)this;
        }

        public TSelf ConfigureHostContainer<TContainerBuilder>(Action<IHostServicesContext, TContainerBuilder> container)
        {
            HostContainerConfiguration.Add(new ServicesConfiguration<IHostServicesContext, TContainerBuilder>(container));
            return (TSelf)this;
        }

        public TSelf ConfigureLogging(Action<ILoggingBuilder> loggingConfiguration)
        {
            LoggingBuilderConfiguration.Add(loggingConfiguration);
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configuration)
        {
            CommonCommandConfigurationConfiguration.Add(configuration);
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandServices(Action<ICommandServicesContext, IServiceCollection> services)
        {
            CommonCommandServicesConfiguration.Add(new ServicesConfiguration<ICommandServicesContext, IServiceCollection>(services));
            return (TSelf)this;
        }

        public TSelf ConfigureCommonCommandContainer<TContainerBuilder>(Action<ICommandServicesContext, TContainerBuilder> services)
        {
            CommonCommandContainerConfiguration.Add(new ServicesConfiguration<ICommandServicesContext, TContainerBuilder>(services));
            return (TSelf)this;
        }

        public TSelf WithCommand<TCommandPlugin>()
            where TCommandPlugin : class, ICommonCommandPlugin, new()
        {
            var commandPlugin = new TCommandPlugin();

            CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
            return (TSelf)this;
        }

        public TSelf WithCommand(ICommandPlugin commandPlugin)
        {
            CommandPlugins.Add(commandPlugin.CommandName, commandPlugin);
            return (TSelf)this;
        }

        public ICommandLineApp Build()
        {
            var hostContext = new HostContext();

            var hostConfiguration = BuildHostConfiguration(hostContext);

            var hostServicesContext = new HostServicesContext(hostConfiguration);

            var hostServices = BuildHostServices(hostServicesContext);

            var configureLogBuilder = new Action<ILoggingBuilder>(
                lb =>
                {
                    foreach (var configurator in this.LoggingBuilderConfiguration)
                    {
                        configurator.Invoke(lb);
                    }
                });

            var configureCommonConfiguration = new Action<ICommandConfigurationContext, IConfigurationBuilder>(
                (ctx, cb) =>
                {
                    foreach (var configurator in this.CommonCommandConfigurationConfiguration)
                    {
                        configurator.Invoke(ctx, cb);
                    }
                });

            // var configureCommonCommandServices = new Action<ICommandServicesContext, IServiceCollection>(
            //     (ctx, sc) =>
            //     {
            //         foreach (var configurator in this.CommonCommandServicesConfiguration)
            //         {
            //             configurator.Configure(ctx, sc);
            //         }
            //
            //     }
            // );

            var pluginConfigurationContext = new PluginConfigurationContext(
                hostConfiguration,
                hostServices,
                configureLogBuilder,
                configureCommonConfiguration,
                CommonCommandServicesConfiguration,
                CommonCommandContainerConfiguration);

            var rootCommandPlugin = CreateRootCommand(pluginConfigurationContext);

            var app = new CommandLineApplication();
            rootCommandPlugin.ConfigureCommand(pluginConfigurationContext, app);
            return new CommandLineApp(app);
        }

        protected virtual ICommonCommandPlugin CreateRootCommand(IPluginConfigurationContext pluginConfigurationContext)
            => new CommandPluginBuilder.CommandPluginBuilder()
                .WithName(this.Name)
                .WithDescription(this.Description)
                .ConfigureCommand((ctx, commandLineApp) =>
                {
                    commandLineApp.HelpOption(true);

                    foreach (var commandLinePlugin in CommandPlugins)
                    {
                        var commandName = commandLinePlugin.Key;
                        var plugin = commandLinePlugin.Value;

                        commandLineApp.Command(commandName, childCommand =>
                        {
                            childCommand.Description = plugin.Description;

                            plugin.ConfigureCommand(pluginConfigurationContext, childCommand);
                        });
                    }

                    commandLineApp.OnExecute(() =>
                    {
                        commandLineApp.ShowHelp();
                        return 1;
                    });
                })
                .Build();

        protected virtual IConfigurationRoot BuildHostConfiguration(IHostContext hostContext)
        {
            var hostConfigurationBuilder = new ConfigurationBuilder();
            foreach (var hostConfigurator in HostConfigurationConfiguration)
            {
                hostConfigurator.Invoke(hostContext, hostConfigurationBuilder);
            }

            var hostConfiguration = hostConfigurationBuilder.Build();
            return hostConfiguration;
        }

        protected virtual IServiceProvider BuildHostServices(IHostServicesContext hostServicesContext)
        {
            var hostServiceCollection = new ServiceCollection();

            hostServiceCollection.AddLogging(loggingBuilder =>
            {
                foreach (var loggingBuilderConfigurator in this.LoggingBuilderConfiguration)
                {
                    loggingBuilderConfigurator.Invoke(loggingBuilder);
                }
            });

            foreach (var hostServicesConfigurator in this.HostServicesConfiguration)
            {
                hostServicesConfigurator.Invoke(hostServicesContext, hostServiceCollection);
            }

            var hostServices = HostServiceProviderFactory.Invoke(hostServicesContext, hostServiceCollection);
            return hostServices;
        }
    }
}
