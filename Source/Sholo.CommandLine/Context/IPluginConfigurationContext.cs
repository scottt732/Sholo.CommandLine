using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface IPluginConfigurationContext
    {
        IConfigurationRoot HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        Action<ILoggingBuilder> ConfigureLogBuilder { get; }
        Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }
        Action<ICommandServicesContext, IServiceCollection> ConfigureCommonCommandServices { get; }
    }

    public interface IPluginConfigurationContext<in TCommandParameters>
        where TCommandParameters : class, new()
    {
        IConfigurationRoot HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        Action<ILoggingBuilder> ConfigureLogBuilder { get; }
        Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }
        Action<ICommandServicesContext<TCommandParameters>, IServiceCollection> ConfigureCommonCommandServices { get; }
    }
}
