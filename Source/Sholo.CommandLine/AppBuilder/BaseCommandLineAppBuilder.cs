using System;
using System.Collections.Generic;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.CommandPluginBuilders;
using Sholo.CommandLine.CommandPlugins;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.AppBuilder;

[PublicAPI]
public abstract class BaseCommandLineAppBuilder<TSelf> : ICommandLineAppBuilder<TSelf>
    where TSelf : BaseCommandLineAppBuilder<TSelf>, new()
{
    private string Name { get; set; }
    private string Description { get; set; }

    private IList<Action<IHostLoggingContext, ILoggingBuilder>> LoggingBuilderConfiguration { get; } = new List<Action<IHostLoggingContext, ILoggingBuilder>>();
    private IList<Action<IHostServicesContext, IServiceCollection>> HostServicesConfiguration { get; } = new List<Action<IHostServicesContext, IServiceCollection>>();
    private IList<Action<IHostContext, IConfigurationBuilder>> HostConfigurationConfiguration { get; } = new List<Action<IHostContext, IConfigurationBuilder>>();

    private IList<Action<ICommandConfigurationContext, IConfigurationBuilder>> CommonCommandConfigurationConfiguration { get; } = new List<Action<ICommandConfigurationContext, IConfigurationBuilder>>();
    private IList<Action<ICommandServicesContext, IServiceCollection>> CommonCommandServicesConfiguration { get; } = new List<Action<ICommandServicesContext, IServiceCollection>>();

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

    public TSelf ConfigureLogging(Action<IHostLoggingContext, ILoggingBuilder> loggingConfiguration)
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
        CommonCommandServicesConfiguration.Add(services);
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

        var configureLogBuilder = new Action<IHostLoggingContext, ILoggingBuilder>(
            (ctx, lb) =>
            {
                foreach (var configurator in LoggingBuilderConfiguration)
                {
                    configurator.Invoke(ctx, lb);
                }
            });

        var configureCommonConfiguration = new Action<ICommandConfigurationContext, IConfigurationBuilder>(
            (ctx, cb) =>
            {
                foreach (var configurator in CommonCommandConfigurationConfiguration)
                {
                    configurator.Invoke(ctx, cb);
                }
            });

        var configureCommonCommandServices = new Action<ICommandServicesContext, IServiceCollection>(
            (ctx, sc) =>
            {
                foreach (var configurator in CommonCommandServicesConfiguration)
                {
                    configurator.Invoke(ctx, sc);
                }

                sc.TryAddSingleton(ctx.LoggerFactory);
                sc.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            }
        );

        var pluginConfigurationContext = new PluginConfigurationContext(
            hostConfiguration,
            hostServices,
            configureLogBuilder,
            configureCommonConfiguration,
            configureCommonCommandServices);

        var rootCommandPlugin = CreateRootCommand(pluginConfigurationContext);

        var app = new CommandLineApplication();
        rootCommandPlugin.ConfigureCommand(pluginConfigurationContext, app);
        return new CommandLineApp(app);
    }

    protected virtual ICommonCommandPlugin CreateRootCommand(IPluginConfigurationContext pluginConfigurationContext)
        => new CommandPluginBuilder()
            .WithName(Name ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown")
            .WithDescription(Description ?? "My awesome utility")
            .ConfigureCommand((_, commandLineApp) =>
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

    protected virtual ServiceProvider BuildHostServices(IHostServicesContext hostServicesContext)
    {
        var hostServiceCollection = new ServiceCollection();
        var emptyServiceProvider = new ServiceCollection().BuildServiceProvider();

        var earlyHostConfiguration = new HostLoggingContext(hostServicesContext.HostConfiguration, emptyServiceProvider);
        hostServiceCollection.AddLogging(loggingBuilder =>
        {
            foreach (var loggingBuilderConfigurator in LoggingBuilderConfiguration)
            {
                loggingBuilderConfigurator.Invoke(earlyHostConfiguration, loggingBuilder);
            }
        });

        foreach (var hostServicesConfigurator in HostServicesConfiguration)
        {
            hostServicesConfigurator.Invoke(hostServicesContext, hostServiceCollection);
        }

        hostServiceCollection.AddSingleton<IHostLoggingContext>(sp => new HostLoggingContext(hostServicesContext.HostConfiguration, sp));

        var hostServices = hostServiceCollection.BuildServiceProvider();
        return hostServices;
    }
}
