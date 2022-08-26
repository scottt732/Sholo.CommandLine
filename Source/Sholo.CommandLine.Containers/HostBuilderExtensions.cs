using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.Hosting;

namespace Sholo.CommandLine.Containers
{
    public static class HostBuilderExtensions
    {
        public static readonly string ContextPropertyName = "CommandLineArguments";

        public static IHostBuilder UseOptions<TOptions>(this IHostBuilder hostBuilder, string[] args)
            where TOptions : ContainerizedAppOptions
        {
            TOptions options = null;
            Parser.Default.ParseArguments<TOptions>(args).WithParsed(o => options = o);

            hostBuilder.Properties[ContextPropertyName] = options;

            return hostBuilder;
        }

        public static IHostBuilder UseVersionResource(this IHostBuilder hostBuilder, string applicationName = null)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var name = applicationName ?? entryAssembly?.GetName().Name ?? Environment.GetCommandLineArgs().FirstOrDefault();

            string version = null;
            if (entryAssembly != null)
            {
                var resourceName = entryAssembly.GetManifestResourceNames().SingleOrDefault(str => str.EndsWith("VERSION", StringComparison.Ordinal));
                if (resourceName != null)
                {
                    using (var stream = entryAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                var versionLine = sr.ReadLine();
                                if (versionLine != null)
                                {
                                    var versionParts = versionLine.Split('.');
                                    if (versionParts.Length == 2 && versionParts[0].All(char.IsDigit) && versionParts[1].All(char.IsDigit))
                                    {
                                        version = versionLine;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            hostBuilder.Properties["ApplicationName"] = name;
            hostBuilder.Properties["ApplicationVersion"] = version;

            return hostBuilder;
        }
    }
}
