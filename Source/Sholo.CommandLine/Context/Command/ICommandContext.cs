using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface ICommandContext
    {
        IConfiguration HostConfiguration { get; }
        IServiceProvider HostServices { get; }
        IConfiguration CommandConfiguration { get; }
        IServiceProvider ServiceProvider { get; set; }
        CommandLineApplication Command { get; }
    }

    public interface ICommandContext<out TParameters> : ICommandContext
        where TParameters : class, new()
    {
        TParameters Parameters { get; }
    }
}
