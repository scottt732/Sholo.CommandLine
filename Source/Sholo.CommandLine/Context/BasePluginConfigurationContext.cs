using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sholo.CommandLine.Context;

internal class BasePluginConfigurationContext
{
    public IConfigurationRoot HostConfiguration { get; }
    public IServiceProvider HostServices { get; }
    public Action<IHostLoggingContext, ILoggingBuilder> ConfigureLogBuilder { get; }
    public Action<ICommandConfigurationContext, IConfigurationBuilder> ConfigureCommonConfiguration { get; }

    protected BasePluginConfigurationContext(
        IConfigurationRoot hostConfiguration,
        IServiceProvider hostServices,
        Action<IHostLoggingContext, ILoggingBuilder> configureLogBuilder,
        Action<ICommandConfigurationContext, IConfigurationBuilder> configureCommonConfiguration)
    {
        HostConfiguration = hostConfiguration;
        HostServices = hostServices;
        ConfigureLogBuilder = configureLogBuilder;
        ConfigureCommonConfiguration = configureCommonConfiguration;
    }
}
