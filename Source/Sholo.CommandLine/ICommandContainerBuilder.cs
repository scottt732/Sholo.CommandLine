using System;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.CommandPluginBuilder;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine
{
    public interface ICommandContainerBuilder<out TSelf>
        where TSelf : ICommandContainerBuilder<TSelf>
    {
        TSelf WithCommand<TCommand>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand>> configurator)
                where TCommand : ICommand;

        TSelf WithCommand<TCommand, TCommandParameters>(
            string name,
            string description,
            Action<CommandPluginBuilder<TCommand, TCommandParameters>> configurator)
                where TCommand : ICommand<TCommandParameters>
                where TCommandParameters : class, new();

        // TSelf WithCommand<TCommandPlugin, TCommand>()
        //     where TCommandPlugin : class, ICommandPlugin<TCommand>, new()
        //     where TCommand : ICommand;
        // TSelf WithCommand<TCommandPlugin, TCommand, TParameters>()
        //     where TCommandPlugin : class, ICommandPlugin<TCommand, TParameters>, new()
        //     where TCommand : ICommand<TParameters>
        //     where TParameters : class, new();
    }
}
