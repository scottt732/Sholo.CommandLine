using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMemberInSuper.Global
namespace Sholo.CommandLine.Context;

public interface IHostServicesContext
{
    IConfigurationRoot HostConfiguration { get; }
}
