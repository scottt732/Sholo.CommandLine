using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Sholo.CommandLine.Context;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.CommandPlugins;

public interface ICommonCommandPlugin
{
    string CommandName { get; }
    string Description { get; }

    void ConfigureCommandConfiguration(ICommandConfigurationContext context, IConfigurationBuilder builder);
    void ConfigureCommand(IPluginConfigurationContext context, CommandLineApplication command);
}
