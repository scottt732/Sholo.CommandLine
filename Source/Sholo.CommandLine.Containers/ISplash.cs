namespace Sholo.CommandLine.Containers;

[PublicAPI]
public interface ISplash
{
    void WriteSplashMessage(string applicationName, string version);
    void WriteShutdownMessage(string applicationName, string version);
}
