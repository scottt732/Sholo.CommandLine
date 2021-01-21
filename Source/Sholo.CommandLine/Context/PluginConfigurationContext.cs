using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.Context
{
    internal class PluginConfigurationContext : BasePluginConfigurationContext, IPluginConfigurationContext
    {
        public IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandServices { get; }
        public IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandContainer { get; }

        public PluginConfigurationContext(
            IConfigurationRoot hostConfiguration,
            IServiceProvider hostServices,
            Action<ILoggingBuilder> configureLogBuilder,
            Action<ICommandConfigurationContext, IConfigurationBuilder> configureCommonConfiguration,
            IList<IServicesConfiguration<ICommandServicesContext>> configureCommonCommandServices,
            IList<IServicesConfiguration<ICommandServicesContext>> configureCommonCommandContainer)
            : base(
                hostConfiguration,
                hostServices,
                configureLogBuilder,
                configureCommonConfiguration)
        {
            ConfigureCommonCommandServices = configureCommonCommandServices;
            ConfigureCommonCommandContainer = configureCommonCommandContainer;
        }
    }

    internal class PluginConfigurationContext<TCommandParameters> : BasePluginConfigurationContext, IPluginConfigurationContext<TCommandParameters>
        where TCommandParameters : class, new()
    {
        public IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandServices { get; }
        public IList<IServicesConfiguration<ICommandServicesContext>> ConfigureCommonCommandContainer { get; }

        public PluginConfigurationContext(
            IConfigurationRoot hostConfiguration,
            IServiceProvider hostServices,
            Action<ILoggingBuilder> configureLogBuilder,
            Action<ICommandConfigurationContext, IConfigurationBuilder> configureCommonConfiguration,
            IList<IServicesConfiguration<ICommandServicesContext>> configureCommonCommandServices,
            IList<IServicesConfiguration<ICommandServicesContext>> configureCommonCommandContainer)
            : base(
                hostConfiguration,
                hostServices,
                configureLogBuilder,
                configureCommonConfiguration)
        {
            ConfigureCommonCommandServices = configureCommonCommandServices;
            ConfigureCommonCommandContainer = configureCommonCommandContainer;
        }
    }
}
