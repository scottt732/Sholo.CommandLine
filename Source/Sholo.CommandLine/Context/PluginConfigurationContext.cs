using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.Context
{
    internal class PluginConfigurationContext : BasePluginConfigurationContext, IPluginConfigurationContext
    {
        public Action<ICommandServicesContext, IServiceCollection> ConfigureCommonCommandServices { get; }

        public PluginConfigurationContext(
            IConfigurationRoot hostConfiguration,
            IServiceProvider hostServices,
            Action<ILoggingBuilder> configureLogBuilder,
            Action<ICommandConfigurationContext, IConfigurationBuilder> configureCommonConfiguration,
            Action<ICommandServicesContext, IServiceCollection> configureCommonCommandServices)
            : base(
                hostConfiguration,
                hostServices,
                configureLogBuilder,
                configureCommonConfiguration)
        {
            ConfigureCommonCommandServices = configureCommonCommandServices;
        }
    }

    internal class PluginConfigurationContext<TCommandParameters> : BasePluginConfigurationContext, IPluginConfigurationContext<TCommandParameters>
        where TCommandParameters : class, new()
    {
        public Action<ICommandServicesContext<TCommandParameters>, IServiceCollection> ConfigureCommonCommandServices { get; }

        public PluginConfigurationContext(
            IConfigurationRoot hostConfiguration,
            IServiceProvider hostServices,
            Action<ILoggingBuilder> configureLogBuilder,
            Action<ICommandConfigurationContext, IConfigurationBuilder> configureCommonConfiguration,
            Action<ICommandServicesContext<TCommandParameters>, IServiceCollection> configureCommonCommandServices)
            : base(
                hostConfiguration,
                hostServices,
                configureLogBuilder,
                configureCommonConfiguration)
        {
            ConfigureCommonCommandServices = configureCommonCommandServices;
        }
    }
}
