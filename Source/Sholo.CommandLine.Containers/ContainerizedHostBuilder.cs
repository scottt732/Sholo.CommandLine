using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sholo.CommandLine.Containers.SplashService;

namespace Sholo.CommandLine.Containers
{
    [PublicAPI]
    public static class ContainerizedHostBuilder
    {
        public static IHostBuilder Create(string[] args, string applicationName = null)
            => ConfigureHostBuilder<Splash>(new HostBuilder().UseOptions<ContainerizedAppOptions>(args), applicationName);

        public static IHostBuilder Create<TOptions>(string[] args, string applicationName = null)
            where TOptions : ContainerizedAppOptions
            => ConfigureHostBuilder<Splash>(new HostBuilder().UseOptions<TOptions>(args), applicationName);

        public static IHostBuilder Create<TOptions, TSplash>(string[] args, string applicationName = null)
            where TOptions : ContainerizedAppOptions
            where TSplash : class, ISplash
            => ConfigureHostBuilder<TSplash>(new HostBuilder().UseOptions<TOptions>(args), applicationName);

        public static IHostBuilder ConfigureHostBuilder<TSplash>(IHostBuilder hostBuilder, string applicationName)
            where TSplash : class, ISplash
                => hostBuilder
                    .UseVersionResource(applicationName)
                    .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
                    .ConfigureAppConfiguration((ctx, cb) =>
                    {
                        if (ctx.TryGetOptions<ContainerizedAppOptions>(out var options) && !string.IsNullOrEmpty(options?.ConfigFile))
                        {
                            var configFile = options.ConfigFile;
                            if (!File.Exists(configFile))
                            {
                                throw new Exception("The config file specified does not exist");
                            }

                            var configFileExtension = Path.GetExtension(configFile);

                            if (configFileExtension.Equals(".yaml", StringComparison.OrdinalIgnoreCase))
                            {
                                cb.AddYamlFile(configFile, false);
                            }
                            else if (configFileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase))
                            {
                                cb.AddJsonFile(configFile, false);
                            }
                            else
                            {
                                throw new Exception($"Unsupported config file extension: {configFileExtension}.  Expecting .yaml or .json");
                            }
                        }
                        else
                        {
                            cb.AddYamlFile("config.yaml", true);
                            cb.AddYamlFile("config.yml", true);
                            cb.AddJsonFile("config.json", true);
                        }

                        cb.AddEnvironmentVariables();

                        if (ctx.HostingEnvironment.IsDevelopment())
                        {
                            try
                            {
                                cb.AddUserSecrets(Assembly.GetEntryAssembly());
                            }
                            catch
                            {
                                // We tried
                            }
                        }
                    })
                    .ConfigureServices((ctx, services) =>
                    {
                        services.AddOptions<ApplicationNameAndVersion>()
                            .Configure(opt =>
                            {
                                opt.Name = ctx.Properties?["ApplicationName"]?.ToString() ?? string.Empty;
                                opt.Version = ctx.Properties?["ApplicationVersion"]?.ToString() ?? string.Empty;
                            });

                        services.AddSingleton<ISplash, TSplash>();
                        services.AddHostedService<SplashService.SplashService>();
                        services.AddLogging();
                    })
                    .ConfigureLogging((ctx, lb) => { lb.AddSerilogConsole(); })
                    .UseConsoleLifetime(c => { c.SuppressStatusMessages = true; });
    }
}
