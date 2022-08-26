using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface ICommandConfigurationContext
    {
        ILoggerFactory LoggerFactory { get; }
        IConfiguration HostConfiguration { get; }
        IServiceProvider HostServices { get; }
    }
}
