using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sholo.CommandLine.Command;
using Sholo.CommandLine.Context;
using Sholo.CommandLine.Context.PluginConfiguration;

// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedTypeParameter
namespace Sholo.CommandLine.CommandPlugin
{
    public interface ICommandPlugin
    {
        string Description { get; }

        void ConfigureCommandConfiguration(ICommandConfigurationContext context, IConfigurationBuilder builder);
        void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command);
    }

    public interface ICommandPlugin<in TCommand> : ICommandPlugin
        where TCommand : ICommand
    {
        void ConfigureCommandServices(ICommandServicesContext context, IServiceCollection configure);
        void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext context, TContainerBuilder configure);
    }

    public interface ICommandPlugin<in TCommand, in TParameters> : ICommandPlugin
        where TCommand : ICommand<TParameters>
        where TParameters : class, new()
    {
        void ConfigureCommandServices(ICommandServicesContext<TParameters> context, IServiceCollection configure);
        void ConfigureCommandContainer<TContainerBuilder>(ICommandServicesContext<TParameters> context, TContainerBuilder configure);
        void ConfigureParameters(ICommandParameterizationContext context, TParameters configure);
    }
}
