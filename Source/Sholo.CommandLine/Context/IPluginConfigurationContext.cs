using System;
using System.Collections.Generic;
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
        IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandServices { get; }
        IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandContainer { get; }
    }

    public interface IPluginConfigurationContext<in TCommandParameters>
        where TCommandParameters : class, new()
    {
        IConfigurationRoot HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        Action<ILoggingBuilder> ConfigureLogBuilder { get; }
        Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }
        IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandServices { get; }
        IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandContainer { get; }
    }
}
