using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Commands;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedTypeParameter
namespace Sholo.CommandLine.CommandPlugins;

public interface ICommandPlugin : ICommonCommandPlugin
{
    void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection services);
}

public interface ICommandPlugin<in TCommand> : ICommonCommandPlugin
    where TCommand : ICommand
{
    void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection services);
}

public interface ICommandPlugin<in TCommand, in TParameters> : ICommonCommandPlugin
    where TCommand : ICommand<TParameters>
    where TParameters : class, new()
{
    void ConfigureCommandServices(ICommandServicesContext<TParameters> context, IServiceCollection services);
    void ConfigureParameters(ICommandParameterizationContext context, TParameters parameters);
}
