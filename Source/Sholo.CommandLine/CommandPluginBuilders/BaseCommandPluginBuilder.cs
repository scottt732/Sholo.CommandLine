using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Commands;
using Sholo.CommandLine.Context;

namespace Sholo.CommandLine.CommandPluginBuilders;

public abstract class BaseCommandPluginBuilder<TSelf, TCommand, TParameters> : BaseCommonCommandPluginBuilder<TSelf>, ICommandPluginBuilder<TSelf, TCommand, TParameters>
    where TSelf : BaseCommandPluginBuilder<TSelf, TCommand, TParameters>, new()
    where TCommand : ICommand<TParameters>
    where TParameters : class, new()
{
    protected List<Action<ICommandParameterizationContext, TParameters>> ParameterConfiguration { get; } = new List<Action<ICommandParameterizationContext, TParameters>>();
    protected List<Action<ICommandServicesContext<TParameters>, IServiceCollection>> CommandServicesConfiguration { get; } = new List<Action<ICommandServicesContext<TParameters>, IServiceCollection>>();

    public TSelf ConfigureParameters(Action<ICommandParameterizationContext, TParameters> parameters)
    {
        ParameterConfiguration.Add(parameters);
        return (TSelf)this;
    }

    public TSelf ConfigureCommandServices(Action<ICommandServicesContext<TParameters>, IServiceCollection> services)
    {
        CommandServicesConfiguration.Add(services);
        return (TSelf)this;
    }
}

public abstract class BaseCommandPluginBuilder<TSelf, TCommand> : BaseCommonCommandPluginBuilder<TSelf>, ICommandPluginBuilder<TSelf, TCommand>
    where TSelf : BaseCommandPluginBuilder<TSelf, TCommand>, new()
    where TCommand : ICommand
{
    protected List<Action<ICommandServicesContext, IServiceCollection>> CommandServicesConfiguration { get; } = new List<Action<ICommandServicesContext, IServiceCollection>>();

    public TSelf ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> services)
    {
        CommandServicesConfiguration.Add(services);
        return (TSelf)this;
    }
}

public abstract class BaseCommandPluginBuilder<TSelf> : BaseCommonCommandPluginBuilder<TSelf>, ICommandPluginBuilder<TSelf>
    where TSelf : BaseCommandPluginBuilder<TSelf>, new()
{
    protected List<Action<ICommandServicesContext, IServiceCollection>> CommandServicesConfiguration { get; } = new List<Action<ICommandServicesContext, IServiceCollection>>();

    public TSelf ConfigureCommandServices(Action<ICommandServicesContext, IServiceCollection> services)
    {
        CommandServicesConfiguration.Add(services);
        return (TSelf)this;
    }
}
