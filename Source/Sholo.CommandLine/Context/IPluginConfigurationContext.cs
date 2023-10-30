using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMemberInSuper.Global

namespace Sholo.CommandLine.Context;

[PublicAPI]
public interface IPluginConfigurationContext
{
    IConfigurationRoot HostConfiguration { get; }
    IServiceProvider HostServices { get; }
    Action<IHostLoggingContext, ILoggingBuilder> ConfigureLogBuilder { get; }
    Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }
    Action<ICommandServicesContext, IServiceCollection> ConfigureCommonCommandServices { get; }
}

[PublicAPI]
public interface IPluginConfigurationContext<in TCommandParameters>
    where TCommandParameters : class, new()
{
    IConfigurationRoot HostConfiguration { get; }
    IServiceProvider HostServices { get; }
    Action<IHostLoggingContext, ILoggingBuilder> ConfigureLogBuilder { get; }
    Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }
    Action<ICommandServicesContext<TCommandParameters>, IServiceCollection> ConfigureCommonCommandServices { get; }
}
