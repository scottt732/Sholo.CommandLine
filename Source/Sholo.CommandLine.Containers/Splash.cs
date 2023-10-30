using Microsoft.Extensions.Logging;

namespace Sholo.CommandLine.Containers;

public class Splash : ISplash
{
    private ILogger<Splash> Logger { get; }

    public Splash(ILogger<Splash> logger)
    {
        Logger = logger;
    }

    public virtual void WriteSplashMessage(string applicationName, string version)
    {
        if (!string.IsNullOrEmpty(version))
        {
            Logger.LogInformation($"{applicationName} {version}");
        }
        else
        {
            Logger.LogInformation($"{applicationName}");
        }

        Logger.LogInformation(string.Empty);
    }

    public virtual void WriteShutdownMessage(string applicationName, string version)
    {
        Logger.LogInformation(string.Empty);
    }
}
