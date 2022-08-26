using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface ICommandServicesContext
    {
        ILoggerFactory LoggerFactory { get; }
        IConfiguration HostConfiguration { get; }
        IConfiguration CommandConfiguration { get; }
        IServiceProvider HostServices { get; }
    }

    public interface ICommandServicesContext<out TParameters> : ICommandServicesContext
        where TParameters : class, new()
    {
        TParameters Parameters { get; }
    }
}
