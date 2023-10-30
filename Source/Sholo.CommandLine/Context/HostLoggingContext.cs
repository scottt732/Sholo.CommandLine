using System;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context;

public class HostLoggingContext : IHostLoggingContext
{
    public IConfiguration HostConfiguration { get; }
    public IServiceProvider HostServices { get; }

    public HostLoggingContext(IConfiguration hostConfiguration, IServiceProvider hostServices)
    {
        HostConfiguration = hostConfiguration;
        HostServices = hostServices;
    }
}
