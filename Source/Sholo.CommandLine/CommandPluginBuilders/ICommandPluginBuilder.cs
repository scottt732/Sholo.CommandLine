using System;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Commands;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMember.Global
namespace Sholo.CommandLine.CommandPluginBuilders;

public interface ICommandPluginBuilder<out TSelf, TCommand, out TParameters> : ICommonCommandPluginBuilder<TSelf>
    where TSelf : ICommandPluginBuilder<TSelf, TCommand, TParameters>
    where TCommand : ICommand<TParameters>
    where TParameters : class, new()
{
    TSelf ConfigureParameters(Action<ICommandParameterizationContext, TParameters> parameters);
    TSelf ConfigureCommandServices(Action<ICommandServicesContext<TParameters>, IServiceCollection> services);
}

public interface ICommandPluginBuilder<out TSelf, TCommand> : ICommonCommandPluginBuilder<TSelf>
    where TSelf : ICommandPluginBuilder<TSelf, TCommand>
    where TCommand : ICommand
{
    TSelf ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> services);
}

public interface ICommandPluginBuilder<out TSelf> : ICommonCommandPluginBuilder<TSelf>
    where TSelf : ICommandPluginBuilder<TSelf>
{
    TSelf ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> services);
}
