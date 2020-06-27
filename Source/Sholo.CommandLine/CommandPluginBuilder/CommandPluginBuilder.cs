using Sholo.CommandLine.Command;
using Sholo.CommandLine.CommandPlugin;

namespace Sholo.CommandLine.CommandPluginBuilder
{
    public sealed class CommandPluginBuilder<TCommand, TParameters>
        : BaseCommandPluginBuilder<CommandPluginBuilder<TCommand, TParameters>, TCommand, TParameters>
        where TParameters : class, new()
        where TCommand : ICommand<TParameters>
    {
        public override ICommonCommandPlugin Build()
            => new LambdaCommandPlugin<TCommand, TParameters>(
                Name,
                Description,
                CommandConfigurationConfiguration,
                CommandServicesConfiguration,
                CommandConfiguration,
                ParameterConfiguration);
    }

    public sealed class CommandPluginBuilder<TCommand>
        : BaseCommandPluginBuilder<CommandPluginBuilder<TCommand>, TCommand>
        where TCommand : ICommand
    {
        public override ICommonCommandPlugin Build()
            => new LambdaCommandPlugin<TCommand>(
                Name,
                Description,
                CommandConfigurationConfiguration,
                CommandServicesConfiguration,
                CommandConfiguration);
    }

    public sealed class CommandPluginBuilder
        : BaseCommandPluginBuilder<CommandPluginBuilder>
    {
        public override ICommonCommandPlugin Build()
            => new LambdaCommandPlugin(
                Name,
                Description,
                CommandConfigurationConfiguration,
                CommandServicesConfiguration,
                CommandConfiguration);
    }
}
