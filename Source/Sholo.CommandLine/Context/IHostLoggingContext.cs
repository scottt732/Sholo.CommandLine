using System;
using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context;

public interface IHostLoggingContext
{
    IConfiguration HostConfiguration { get; }
    IServiceProvider HostServices { get; }
}
