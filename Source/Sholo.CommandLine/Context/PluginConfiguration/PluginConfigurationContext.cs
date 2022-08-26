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
    public class PluginConfigurationContext : IPluginConfigurationContext
    {
        public IConfiguration HostConfiguration { get; }
        public IServiceProvider HostServices { get; }
        public ILoggerFactory LoggerFactory { get; }
        public IHostServiceProviderFactoryBuilder HostServiceProviderFactoryBuilder { get; }
        public ICommandPluginConfigurationBuilderConfigurator CommonCommandConfigurationConfigurator { get; }
        public ICommandPluginServiceProviderFactoryBuilder CommonCommandServiceProviderFactoryBuilder { get; }
        public IServiceProviderFactoryBuilder ParentServiceProviderFactoryBuilder { get; }

        public IEnumerable<IServiceProviderFactoryBuilder> GetServiceProviderFactoryBuilders(
            IServiceProviderFactoryBuilder commandServiceProviderFactoryBuilder,
            bool inheritParentServiceConfiguration
        )
        {
            var result = new Stack<IServiceProviderFactoryBuilder>();
            result.Push(commandServiceProviderFactoryBuilder);

            if (inheritParentServiceConfiguration)
            {
                var parent = ParentServiceProviderFactoryBuilder;
                while (parent != null)
                {
                    result.Push(parent);
                    parent = parent.Parent;
                }
            }

            result.Push(CommonCommandServiceProviderFactoryBuilder);

            while (result.Count > 0)
            {
                yield return result.Pop();
            }
        }

        public PluginConfigurationContext(
            IConfiguration hostConfiguration,
            IServiceProvider hostServices,
            ILoggerFactory loggerFactory,
            IHostServiceProviderFactoryBuilder hostServiceProviderFactoryBuilder,
            ICommandPluginConfigurationBuilderConfigurator commonCommandConfigurationConfigurator,
            ICommandPluginServiceProviderFactoryBuilder commonCommandServiceProviderFactoryBuilder,
            IServiceProviderFactoryBuilder parentServiceProviderFactoryBuilder
        )
        {
            HostConfiguration = hostConfiguration;
            HostServices = hostServices;
            LoggerFactory = loggerFactory;
            HostServiceProviderFactoryBuilder = hostServiceProviderFactoryBuilder;
            CommonCommandConfigurationConfigurator = commonCommandConfigurationConfigurator;
            CommonCommandServiceProviderFactoryBuilder = commonCommandServiceProviderFactoryBuilder;
            ParentServiceProviderFactoryBuilder = parentServiceProviderFactoryBuilder;
        }
    }
}
