using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context;

internal class HostServicesContext : IHostServicesContext
{
    public IConfigurationRoot HostConfiguration { get; }

    public HostServicesContext(IConfigurationRoot hostConfiguration)
    {
        HostConfiguration = hostConfiguration;
    }
}
