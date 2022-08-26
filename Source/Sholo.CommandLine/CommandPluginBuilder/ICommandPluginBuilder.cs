using System;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.CommandPluginBuilder
{
    public interface ICommandPluginBuilder<out TSelf, TCommand> : ICoreCommandPluginBuilder<TSelf>
        where TSelf : ICommandPluginBuilder<TSelf, TCommand>
        where TCommand : ICommand
    {
        TSelf ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> configure);
        TSelf ConfigureCommandContainer<TContainerBuilder>(Action<ICommandServicesContext, TContainerBuilder> configure);
    }

    public interface ICommandPluginBuilder<out TSelf, TCommand, out TParameters> : ICoreCommandPluginBuilder<TSelf>
        where TSelf : ICommandPluginBuilder<TSelf, TCommand, TParameters>
        where TCommand : ICommand<TParameters>
        where TParameters : class, new()
    {
        TSelf ConfigureCommandServices(Action<ICommandServicesContext<TParameters>, IServiceCollection> configure);
        TSelf ConfigureCommandContainer<TContainerBuilder>(Action<ICommandServicesContext<TParameters>, TContainerBuilder> configure);
        TSelf ConfigureParameters(Action<ICommandParameterizationContext, TParameters> parameters);
    }
}
