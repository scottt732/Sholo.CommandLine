using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface ICommandParameterizationContext
    {
        IConfigurationRoot HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        IConfigurationRoot CommandConfiguration { get; }
        IServiceProvider ServiceProvider { get; set; }
        CommandLineApplication Command { get; }
    }
}
