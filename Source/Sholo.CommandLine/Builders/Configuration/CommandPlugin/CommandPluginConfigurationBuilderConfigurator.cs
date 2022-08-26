using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.Builders.Configuration.CommandPlugin
{
    public class CommandPluginConfigurationBuilderConfigurator : BaseConfigurationBuilderConfigurator<ICommandConfigurationContext>, ICommandPluginConfigurationBuilderConfigurator
    {
        public ICommandPluginConfigurationBuilderConfigurator CommonCommandPluginConfigurationBuilderConfigurator { get; }

        public CommandPluginConfigurationBuilderConfigurator(ICommandPluginConfigurationBuilderConfigurator commonCommandPluginConfigurationBuilderConfigurator = null)
        {
            CommonCommandPluginConfigurationBuilderConfigurator = commonCommandPluginConfigurationBuilderConfigurator;
        }

        public override IConfiguration BuildHostConfiguration(ICommandConfigurationContext context)
        {
            var commandPluginConfigurationBuilder = new ConfigurationBuilder();

            ConfigureConfigurationBuilder(context, commandPluginConfigurationBuilder);

            CommonCommandPluginConfigurationBuilderConfigurator?.ConfigureConfigurationBuilder(context, commandPluginConfigurationBuilder);

            var hostConfiguration = commandPluginConfigurationBuilder.Build();

            return hostConfiguration;
        }
    }
}
