using System;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Builders.Configuration
{
    public interface IConfigurationBuilderConfigurator<TContext>
    {
        void AddConfiguration(Action<TContext, IConfigurationBuilder> configure);
        void ConfigureConfigurationBuilder(TContext context, ConfigurationBuilder configurationBuilder);
        IConfiguration BuildHostConfiguration(TContext context);
    }
}
