using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Sholo.CommandLine.Containers.SplashService
{
    public class SplashService : IHostedService
    {
        private IOptions<ApplicationNameAndVersion> Options { get; }
        private ISplash Splash { get; }

        public SplashService(IOptions<ApplicationNameAndVersion> options, ISplash splash)
        {
            Options = options;
            Splash = splash;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Splash.WriteSplashMessage(Options.Value.Name, Options.Value.Version);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Splash.WriteShutdownMessage(Options.Value.Name, Options.Value.Version);
            return Task.CompletedTask;
        }
    }
}
