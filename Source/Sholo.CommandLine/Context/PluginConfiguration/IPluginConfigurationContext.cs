using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sholo.CommandLine.Builders.Configuration.CommandPlugin;
using Sholo.CommandLine.Services;
using Sholo.CommandLine.Services.CommandPlugin;
using Sholo.CommandLine.Services.Host;

namespace Sholo.CommandLine.Context.PluginConfiguration
{
    public interface IPluginConfigurationContext
    {
        IConfiguration HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        ILoggerFactory LoggerFactory { get; }
        IHostServiceProviderFactoryBuilder HostServiceProviderFactoryBuilder { get; }
        ICommandPluginConfigurationBuilderConfigurator CommonCommandConfigurationConfigurator { get; }
        ICommandPluginServiceProviderFactoryBuilder CommonCommandServiceProviderFactoryBuilder { get; }
        IServiceProviderFactoryBuilder ParentServiceProviderFactoryBuilder { get; }

        IEnumerable<IServiceProviderFactoryBuilder> GetServiceProviderFactoryBuilders(
            IServiceProviderFactoryBuilder commandServiceProviderFactoryBuilder,
            bool inheritParentServiceConfiguration
        );
    }
}
