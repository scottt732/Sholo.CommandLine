using Microsoft.Extensions.Configuration;

namespace Sholo.CommandLine.Context
{
    internal class HostServicesContext : IHostServicesContext
    {
        public IConfiguration HostConfiguration { get; }

        public HostServicesContext(IConfiguration hostConfiguration)
        {
            HostConfiguration = hostConfiguration;
        }
    }
}
