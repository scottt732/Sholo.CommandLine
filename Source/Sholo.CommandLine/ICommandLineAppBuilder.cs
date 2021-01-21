using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine
{
    public interface ICommandLineAppBuilder<out TSelf>
        where TSelf : ICommandLineAppBuilder<TSelf>
    {
        TSelf WithName(string name);
        TSelf WithDescription(string description);

        TSelf UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory);
        TSelf UseServiceProviderFactory<TContainerBuilder>(Func<IHostServicesContext, IServiceProviderFactory<TContainerBuilder>> factory);
        TSelf UseDefaultServiceProvider(Action<ServiceProviderOptions> configure);
        TSelf UseDefaultServiceProvider(Action<IHostServicesContext, ServiceProviderOptions> configure);

        TSelf ConfigureHostConfiguration(Action<IHostContext, IConfigurationBuilder> configuration);
        TSelf ConfigureLogging(Action<ILoggingBuilder> loggingConfiguration);
        TSelf ConfigureHostServices(Action<IHostServicesContext, IServiceCollection> services);
        TSelf ConfigureHostContainer<TContainerBuilder>(Action<IHostServicesContext, TContainerBuilder> container);
        TSelf ConfigureCommonCommandConfiguration(Action<ICommandConfigurationContext, IConfigurationBuilder> configuration);
        TSelf ConfigureCommonCommandServices(Action<ICommandServicesContext, IServiceCollection> services);
        TSelf ConfigureCommonCommandContainer<TContainerBuilder>(Action<ICommandServicesContext, TContainerBuilder> services);

        // TSelf WithCommand<TCommandPlugin>()
        //    where TCommandPlugin : class, TCommandPluginInterface, new();

        // TSelf WithCommand(Action<TCommandPluginBuilder> configurator);

        // ICommandLineApp Build();
    }
}
