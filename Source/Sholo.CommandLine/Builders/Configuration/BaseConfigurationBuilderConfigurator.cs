using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Configuration;

namespace Sholo.CommandLine.Builders.Configuration
{
    public abstract class BaseConfigurationBuilderConfigurator<TContext> : IConfigurationBuilderConfigurator<TContext>
    {
        public Collection<ITargetConfiguration<TContext, IConfigurationBuilder>> ConfigurationBuilderConfigurators { get; } = new Collection<ITargetConfiguration<TContext, IConfigurationBuilder>>();

        public void AddConfiguration(Action<TContext, IConfigurationBuilder> configure)
        {
            ConfigurationBuilderConfigurators.Add(new TargetConfiguration<TContext, IConfigurationBuilder>(configure));
        }

        public virtual IConfiguration BuildHostConfiguration(TContext context)
        {
            var hostConfigurationBuilder = new ConfigurationBuilder();

            ConfigureConfigurationBuilder(context, hostConfigurationBuilder);

            var hostConfiguration = hostConfigurationBuilder.Build();

            return hostConfiguration;
        }

        public void ConfigureConfigurationBuilder(TContext context, ConfigurationBuilder configurationBuilder)
        {
            foreach (var hostConfigurator in ConfigurationBuilderConfigurators)
            {
                hostConfigurator.Configure(context, configurationBuilder);
            }
        }
    }
}
