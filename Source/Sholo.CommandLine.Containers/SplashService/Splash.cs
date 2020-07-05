using System;

namespace Sholo.CommandLine.Containers.SplashService
{
    public class Splash : ISplash
    {
        public virtual void WriteSplashMessage(string applicationName, string version)
        {
            if (!string.IsNullOrEmpty(version))
            {
                Console.WriteLine($"{applicationName} {version}");
            }
            else
            {
                Console.WriteLine($"{applicationName}");
            }

            Console.WriteLine(string.Empty);
        }

        public virtual void WriteShutdownMessage(string applicationName, string version)
        {
            Console.WriteLine("Shutting down");
        }
    }
}
