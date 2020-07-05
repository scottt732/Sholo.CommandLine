using JetBrains.Annotations;

namespace Sholo.CommandLine.Containers.SplashService
{
    [PublicAPI]
    public interface ISplash
    {
        void WriteSplashMessage(string applicationName, string version);
        void WriteShutdownMessage(string applicationName, string version);
    }
}
