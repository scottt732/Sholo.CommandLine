using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context
{
    public interface IHostServicesContext
    {
        IConfiguration HostConfiguration { get; }
    }
}
